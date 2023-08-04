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
                        Block<byte> b = toPatch.Slice(0, offset - toPatch.ShiftAmount);
                        b.CopyTo(outBuffer);
                        toPatch.Shift(b.Size);
                        outBuffer.Shift(b.Size);
                        
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
                        Block<byte> b = toPatch.Slice(0, offset - toPatch.ShiftAmount);
                        b.CopyTo(outBuffer);
                        toPatch.Shift(b.Size);
                        outBuffer.Shift(b.Size);

                        // skip base
                        toPatch.Shift(skipCount);
                        
                        // add delta
                        inDeltaStream.ReadExactly(outBuffer.Slice(0, addCount).ToSpan());
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

    private static int ReadBlock(DataStream inStream, Block<byte>? outBuffer)
    {
        ulong packed = inStream.ReadUInt64(Endian.Big);

        byte flags = (byte)(packed >> 56);
        int decompressedSize = (int)((packed >> 32) & 0x00FFFFFF);
        CompressionType compressionType = (CompressionType)((packed >> 24) & 0x7F);
        int bufferSize = (int)(packed & 0x000FFFFF);

        Block<byte> compressedBuffer;
        if ((compressionType & ~CompressionType.Obfuscated) == CompressionType.None && outBuffer is not null)
        {
            compressedBuffer = outBuffer.Slice(bufferSize);
        }
        else
        {
            compressedBuffer = new Block<byte>(bufferSize);
        }
        
        inStream.ReadExactly(compressedBuffer.ToSpan());
        
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
                break;
            case CompressionType.ZLib:
                //ZLib.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize));
                break;
            case CompressionType.ZStd:
                //ZStd.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), flags != 0 ? CompressionFlags.ZStdUseDicts : CompressionFlags.None);
                break;
            case CompressionType.LZ4:
                //LZ4.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize));
                break;
            case CompressionType.OodleKraken:
                //Oodle.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), CompressionFlags.OodleKraken);
                break;
            case CompressionType.OodleSelkie:
                //Oodle.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), CompressionFlags.OodleSelkie);
                break;
            case CompressionType.OodleLeviathan:
                //Oodle.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), CompressionFlags.OodleLeviathan);
                break;
            default:
                throw new NotImplementedException($"Compression type: {compressionType}");
        }

        if ((compressionType & ~CompressionType.Obfuscated) != CompressionType.None || outBuffer is null)
        {
            compressedBuffer.Dispose();   
        }
        outBuffer?.Shift(decompressedSize);

        return decompressedSize;
    }
    
    private static Block<byte> ReadBlock(DataStream inStream)
    {
        ulong packed = inStream.ReadUInt64(Endian.Big);

        byte flags = (byte)(packed >> 56);
        int decompressedSize = (int)((packed >> 32) & 0x00FFFFFF);
        CompressionType compressionType = (CompressionType)((packed >> 24) & 0x7F);
        int bufferSize = (int)(packed & 0x000FFFFF);

        Block<byte> outBuffer = new(decompressedSize);
        
        Block<byte> compressedBuffer;
        if ((compressionType & ~CompressionType.Obfuscated) == CompressionType.None)
        {
            compressedBuffer = outBuffer.Slice(bufferSize);
        }
        else
        {
            compressedBuffer = new Block<byte>(bufferSize);
        }
        
        inStream.ReadExactly(compressedBuffer.ToSpan());
        
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
                break;
            case CompressionType.ZLib:
                //ZLib.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize));
                break;
            case CompressionType.ZStd:
                //ZStd.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), flags != 0 ? CompressionFlags.ZStdUseDicts : CompressionFlags.None);
                break;
            case CompressionType.LZ4:
                //LZ4.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize));
                break;
            case CompressionType.OodleKraken:
                //Oodle.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), CompressionFlags.OodleKraken);
                break;
            case CompressionType.OodleSelkie:
                //Oodle.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), CompressionFlags.OodleSelkie);
                break;
            case CompressionType.OodleLeviathan:
                //Oodle.Decompress(compressedBuffer, ref outBuffer.Slice(0, decompressedSize), CompressionFlags.OodleLeviathan);
                break;
            default:
                throw new NotImplementedException($"Compression type: {compressionType}");
        }

        if ((compressionType & ~CompressionType.Obfuscated) != CompressionType.None)
        {
            compressedBuffer.Dispose();   
        }
        outBuffer?.Shift(decompressedSize);

        return outBuffer;
    }
}