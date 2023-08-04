using System;
using System.Diagnostics;
using System.IO;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Profiles;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO;

public static class Cas
{
    public static Block<byte> DecompressData(DataStream inStream, int inOriginalSize)
    {
        Block<byte> outBuffer = new(inOriginalSize);
        while (inStream.Position < inStream.Length)
        {
            ReadBlock(inStream, outBuffer);
        }

        outBuffer.ResetShift();
        return outBuffer;
    }

    public static Block<byte> DecompressData(DataStream inDeltaStream, DataStream inBaseStream, int inOriginalSize)
    {
        Block<byte> outBuffer = new(inOriginalSize);
        while (inDeltaStream.Position < inDeltaStream.Length)
        {
            uint packed = inDeltaStream.ReadUInt32(Endian.Big);
            int instructionType = (int)(packed & 0xF0000000) >> 28;

            switch (instructionType)
            {
                case 0:
                {
                    // read base blocks
                    int blockCount = (int)(packed & 0x0FFFFFFF);
                    while (blockCount-- > 0)
                    {
                        ReadBlock(inBaseStream, outBuffer);
                    }
                    break;
                }
                case 1:
                {
                    // make large fixes in base block
                    int blockCount = (int)(packed & 0x0FFFFFFF);
                    
                    // read base block
                    Block<byte> toPatch = ReadBlock(inBaseStream);
                    
                    while (blockCount-- > 0)
                    {
                        uint packed2 = inDeltaStream.ReadUInt32(Endian.Big);

                        ushort offset = (ushort)((packed2 & 0xFFFF0000) >> 16);
                        ushort skipCount = (ushort)(packed2 & 0xFFFF);

                        // use base
                        int baseSize = offset - toPatch.ShiftAmount;
                        toPatch.CopyTo(outBuffer, baseSize);
                        toPatch.Shift(baseSize);
                        outBuffer.Shift(baseSize);
                        
                        // use delta
                        ReadBlock(inDeltaStream, outBuffer);
                        
                        // skip base
                        toPatch.Shift(skipCount);
                    }
                    
                    break;
                }
                case 2:
                {
                    // make small fixes in base block
                    int deltaBlockCount = (int)(packed & 0x0FFFFFFF) / 4;
                    
                    // read base block
                    Block<byte> toPatch = ReadBlock(inBaseStream);
                    
                    int newBlockSize = inDeltaStream.ReadUInt16(Endian.Big) + 1;
                    int currentOffset = outBuffer.ShiftAmount;

                    while (deltaBlockCount-- > 0)
                    {
                        uint packed2 = inDeltaStream.ReadUInt32(Endian.Big);
                        ushort offset = (ushort)((packed2 & 0xFFFF0000) >> 16);
                        byte skipCount = (byte)((packed2 & 0xFF00) >> 8);
                        byte addCount = (byte)(packed2 & 0xFF);
                        
                        // use base
                        int baseSize = offset - toPatch.ShiftAmount;
                        toPatch.CopyTo(outBuffer, baseSize);
                        toPatch.Shift(baseSize);
                        outBuffer.Shift(baseSize);

                        // skip base
                        toPatch.Shift(skipCount);
                        
                        // add delta
                        inDeltaStream.ReadExactly(outBuffer.ToSpan(0, addCount));
                        outBuffer.Shift(addCount);
                    }
                    
                    Debug.Assert(outBuffer.ShiftAmount - currentOffset == newBlockSize, "Fuck");
                    
                    break;
                }
                case 3:
                {
                    // read delta blocks
                    int blockCount = (int)(packed & 0x0FFFFFFF);
                    while (blockCount-- > 0)
                    {
                        ReadBlock(inDeltaStream, outBuffer);
                    }
                    break;
                }
                case 4:
                {
                    // skip blocks
                    int blockCount = (int)(packed & 0x0FFFFFFF);
                    while (blockCount-- > 0)
                    {
                        ReadBlock(inBaseStream);
                    }
                    break;
                }
                default:
                    throw new InvalidDataException("block type");
            }
        }
        

        outBuffer.ResetShift();
        return outBuffer;
    }

    private static unsafe int ReadBlock(DataStream inStream, Block<byte>? outBuffer)
    {
        ulong packed = inStream.ReadUInt64(Endian.Big);

        byte flags = (byte)(packed >> 56);
        int decompressedSize = (int)((packed >> 32) & 0x00FFFFFF);
        CompressionType compressionType = (CompressionType)(packed >> 24);
        // (packed >> 20) & 0xF should always be 7
        int bufferSize = (int)(packed & 0x000FFFFF);

        if ((compressionType & ~CompressionType.Obfuscated) == CompressionType.None)
        {
            bufferSize = decompressedSize;
        }
        
        Block<byte> compressedBuffer;
        if ((compressionType & ~CompressionType.Obfuscated) == CompressionType.None && outBuffer is not null)
        {
            compressedBuffer = new Block<byte>(outBuffer.Ptr, bufferSize);
            compressedBuffer.MarkMemoryAsFragile();
        }
        else
        {
            compressedBuffer = new Block<byte>(bufferSize);
        }
        
        inStream.ReadExactly(compressedBuffer);
        
        if (compressionType.HasFlag(CompressionType.Obfuscated))
        {
            // TODO: check if this is only set in fifa 19
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19))
            {
                throw new Exception("obfuscation");
            }
        
            byte[] key = KeyManager.GetKey("CasObfuscationKey");
            for (int i = 0; i < bufferSize; i++)
            {
                compressedBuffer[i] ^= key[i & 0x3FFF];
            }
        }
        
        // TODO: compression methods https://github.com/users/CadeEvs/projects/1?pane=issue&itemId=25477869
        switch (compressionType & ~CompressionType.Obfuscated)
        {
            case CompressionType.None:
                // we already read the data into the outBuffer so nothing to do
                break;
            case CompressionType.ZLib:
                //ZLib.Decompress(compressedBuffer, ref outBuffer);
                break;
            case CompressionType.ZStd:
                //ZStd.Decompress(compressedBuffer, ref outBuffer, flags != 0 ? CompressionFlags.ZStdUseDicts : CompressionFlags.None);
                break;
            case CompressionType.LZ4:
                //LZ4.Decompress(compressedBuffer, ref outBuffer);
                break;
            case CompressionType.OodleKraken:
                //Oodle.Decompress(compressedBuffer, ref outBuffer, CompressionFlags.OodleKraken);
                break;
            case CompressionType.OodleSelkie:
                //Oodle.Decompress(compressedBuffer, ref outBuffer, CompressionFlags.OodleSelkie);
                break;
            case CompressionType.OodleLeviathan:
                //Oodle.Decompress(compressedBuffer, ref outBuffer, CompressionFlags.OodleLeviathan);
                break;
            default:
                throw new NotImplementedException($"Compression type: {compressionType}");
        }

        compressedBuffer.Dispose();
        outBuffer?.Shift(decompressedSize);

        return decompressedSize;
    }
    
    private static Block<byte> ReadBlock(DataStream inStream)
    {
        ulong packed = inStream.ReadUInt64(Endian.Big);

        byte flags = (byte)(packed >> 56);
        int decompressedSize = (int)((packed >> 32) & 0x00FFFFFF);
        CompressionType compressionType = (CompressionType)(packed >> 24);
        // (packed >> 20) & 0xF should always be 7
        int bufferSize = (int)(packed & 0x000FFFFF);

        Block<byte> outBuffer = new(decompressedSize);

        if ((compressionType & ~CompressionType.Obfuscated) == CompressionType.None)
        {
            bufferSize = decompressedSize;
        }
        
        Block<byte> compressedBuffer;
        if ((compressionType & ~CompressionType.Obfuscated) == CompressionType.None)
        {
            compressedBuffer = outBuffer;
        }
        else
        {
            compressedBuffer = new Block<byte>(bufferSize);
        }
        
        inStream.ReadExactly(compressedBuffer);
        
        if (compressionType.HasFlag(CompressionType.Obfuscated))
        {
            // TODO: check if this is only set in fifa 19
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19))
            {
                throw new Exception("obfuscation");
            }
        
            byte[] key = KeyManager.GetKey("CasObfuscationKey");
            for (int i = 0; i < bufferSize; i++)
            {
                compressedBuffer[i] ^= key[i & 0x3FFF];
            }
        }
        
        // TODO: compression methods https://github.com/users/CadeEvs/projects/1?pane=issue&itemId=25477869
        switch (compressionType & ~CompressionType.Obfuscated)
        {
            case CompressionType.None:
                // we already read the data into the outBuffer so nothing to do
                break;
            case CompressionType.ZLib:
                //ZLib.Decompress(compressedBuffer, ref outBuffer);
                break;
            case CompressionType.ZStd:
                //ZStd.Decompress(compressedBuffer, ref outBuffer, flags != 0 ? CompressionFlags.ZStdUseDicts : CompressionFlags.None);
                break;
            case CompressionType.LZ4:
                //LZ4.Decompress(compressedBuffer, ref outBuffer);
                break;
            case CompressionType.OodleKraken:
                //Oodle.Decompress(compressedBuffer, ref outBuffer, CompressionFlags.OodleKraken);
                break;
            case CompressionType.OodleSelkie:
                //Oodle.Decompress(compressedBuffer, ref outBuffer, CompressionFlags.OodleSelkie);
                break;
            case CompressionType.OodleLeviathan:
                //Oodle.Decompress(compressedBuffer, ref outBuffer, CompressionFlags.OodleLeviathan);
                break;
            default:
                throw new NotImplementedException($"Compression type: {compressionType}");
        }

        // dispose compressed buffer, unless there wasn't a compressed buffer
        if ((compressionType & ~CompressionType.Obfuscated) != CompressionType.None)
        {
            compressedBuffer.Dispose();   
        }

        return outBuffer;
    }
}