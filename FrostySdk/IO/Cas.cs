using System;
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
            uint tmpVal = inDeltaStream.ReadUInt32(Endian.Big);
            int blockType = (int)(tmpVal & 0xF0000000) >> 28;

            switch (blockType)
            {
                case 0:
                {
                    // read base blocks
                    int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                    while (blockCount-- > 0)
                    {
                        ReadBlock(inBaseStream, outBuffer);
                    }
                    break;
                }
                case 1:
                {
                    // merge base with delta
                    break;
                }
                case 2:
                {
                    // merge base with delta
                    break;
                }
                case 3:
                {
                    // read delta blocks
                    int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                    while (blockCount-- > 0)
                    {
                        ReadBlock(inDeltaStream, outBuffer);
                    }
                    break;
                }
                case 4:
                {
                    // skip blocks
                    int blockCount = (int)(tmpVal & 0x0FFFFFFF);
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

    private static void ReadBlock(DataStream inStream, Block<byte>? outBuffer = null)
    {
        int decompressedSize = inStream.ReadInt32(Endian.Big);
        int bufferSize = inStream.ReadInt32(Endian.Big);

        byte flags = (byte)(decompressedSize >> 24);
        decompressedSize &= 0x00FFFFFF;
        
        CompressionType compressionType = (CompressionType)(bufferSize >> 24);
        // always 7 in (bufferSize & 0x00F0000) >> 20
        bufferSize &= 0x000FFFFF;

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
    }
}