using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Frosty.Sdk.IO;
using Frosty.Sdk.Resources;
using Frosty.Sdk.Utils.CompressionTypes;
using ImpromptuNinjas.ZStd;
using K4os.Compression.LZ4;

namespace Frosty.Sdk.Utils;

public static class CompressionUtils
{
    public static byte[] CompressTexture(byte[] inData, Texture texture, CompressionType compressionType)
    {
        using (MemoryStream ms = new())
        {
            uint first = 0;
            uint second = (uint)inData.Length;

            if (texture.MipCount > 1)
            {
                if (inData.Length > ProfilesLibrary.MaxBufferSize)
                {
                    int index = 0;
                    while (second > ProfilesLibrary.MaxBufferSize && index < texture.FirstMip)
                    {
                        first += texture.MipSizes[index];
                        second -= texture.MipSizes[index++];
                    }
                }
            }

            if (texture.LogicalOffset != first)
            {
                texture.LogicalOffset = first;
                texture.LogicalSize = second;
            }

            byte[] tmpData;
            if (first != 0)
            {
                tmpData = new byte[first];
                Array.Copy(inData, tmpData, first);

                tmpData = Compress(tmpData,compressionType, texture: texture);
                ms.Write(tmpData, 0, tmpData.Length);
                texture.RangeStart = (uint)ms.Length;
            }

            tmpData = new byte[second];
            Array.Copy(inData, first, tmpData, 0, second);

            tmpData = Compress(tmpData, compressionType, texture, first);
            ms.Write(tmpData, 0, tmpData.Length);

            texture.RangeEnd = (uint)ms.Length;
            if (texture.RangeStart == 0)
            {
                texture.RangeEnd = 0;
            }

            return ms.ToArray();
        }
    }
    
    public static byte[] Compress(byte[] inData, CompressionType compressionType, Texture? texture = null, uint offset = 0)
    {
        /* TODO: move this to modify/add res
        if (resType == ResourceType.SwfMovie)
        {
            compressionType = CompressionType.None;
        }
        */

        using DataStream writer = new(new MemoryStream());

        using (MemoryStream stream = new(inData))
        {
            stream.Position = 0x00;
            long length = stream.Length - stream.Position;
            long total = 0;
            long compSize = 0;

            while (length > 0)
            {
                int bufferSize = (int)Math.Min(length, ProfilesLibrary.MaxBufferSize);
                byte[] buffer = new byte[bufferSize];
                if (stream.Read(buffer, 0, bufferSize) != bufferSize)
                {
                    throw new Exception("Couldn't read enough bytes.");
                }

                ushort compressCode = 0x70;
                compressCode |= (ushort)((byte)compressionType << 8);
                ulong compressSize;
                byte[] compBuffer;

                switch (compressionType)
                {
                    case CompressionType.ZStd:
                        compressSize = CompressZStd(buffer, out compBuffer);
                        break;
                    case CompressionType.ZLib:
                        compressSize = CompressZlib(buffer, out compBuffer);
                        break;
                    case CompressionType.LZ4:
                        compressSize = CompressLZ4(buffer, out compBuffer);
                        break;
                    case CompressionType.None:
                        compressSize = CompressNone(buffer, out compBuffer);
                        break;
                    case CompressionType.OodleKraken:
                        compressSize = CompressOodle(buffer, out compBuffer, Oodle.OodleFormat.Kraken);
                        break;
                    case CompressionType.OodleSelkie:
                        compressSize = CompressOodle(buffer, out compBuffer, Oodle.OodleFormat.Selkie);
                        break;
                    case CompressionType.OodleLeviathan:
                        compressSize = CompressOodle(buffer, out compBuffer, Oodle.OodleFormat.Leviathan);
                        break;
                    default:
                        throw new NotImplementedException(compressionType.ToString());
                }

                if (compressSize > (uint)buffer.Length)
                {
                    compressionType = CompressionType.None;
                    stream.Position = 0;
                    writer.Position = 0;
                    length = stream.Length - stream.Position;
                    total = 0;
                    compSize = 0;
                    continue;
                }

                compressCode |= (ushort)((compressSize & 0xF0000) >> 16);

                writer.WriteInt32(bufferSize, Endian.Big);
                writer.WriteUInt16(compressCode, Endian.Big);
                writer.WriteUInt16((ushort)compressSize, Endian.Big);
                writer.Write(compBuffer, 0, (int)compressSize);

                length -= bufferSize;
                total += bufferSize;
                compSize += (long)compressSize + 8;

                if (texture?.MipCount > 1)
                {
                    if (total + offset == texture.MipSizes[0])
                    {
                        texture.SecondMipOffset = texture.ThirdMipOffset = (uint)compSize;
                    }
                    else if (total + offset == texture.MipSizes[0] + texture.MipSizes[1])
                    {
                        texture.ThirdMipOffset = (uint)compSize;
                    }
                }
            }
        }

        return writer.ToByteArray();
    }

    public static ulong CompressNone(byte[] buffer, out byte[] compBuffer)
    {
        compBuffer = buffer;
        return (ulong)buffer.Length;
    }
    
    public static ulong CompressZStd(byte[] buffer, out byte[] compBuffer)
    {
        ulong size;
        using (ZStdCompressor cCtx = new())
        {
            // figure out about how big of a buffer you need
            nuint compressBufferSize = CCtx.GetUpperBound((nuint)buffer.Length);

            compBuffer = new byte[compressBufferSize];

            // set the compressor to the compression level
            cCtx.Set(CompressionParameter.CompressionLevel, ProfilesLibrary.ZStdCompressionLevel);

            // actually perform the compression operation
            size = cCtx.Compress(compBuffer, buffer);
        }

        return size;
    }
    
    public static byte[] DecompressZStd(byte[] buffer, int decompressedSize, bool useDictionary)
    {
        byte[] outBuffer = new byte[decompressedSize];

        // create a context
        using (ZStdDecompressor dCtx = new())
        {
            if (useDictionary)
            {
                ZStdDictionaryBuilder builder = new(ZStd.GetDictionary());
                    
                dCtx.UseDictionary(builder.CreateDecompressorDictionary());
            }

            dCtx.Decompress(outBuffer, buffer);
        }

        return outBuffer;
    }

    public static ulong CompressZlib(byte[] buffer, out byte[] compBuffer)
    {
        compBuffer = new byte[buffer.Length * 2];
        
        ulong size = 0;
        
        ZlibError error = Zlib.Pack(compBuffer, ref size , buffer, (ulong)buffer.LongLength);

        if (error != ZlibError.Okay)
        {
            throw new Exception(error.ToString());
        }
        
        return size;
    }
    
    public static byte[] DecompressZLib(byte[] buffer, int decompressedSize)
    {
        byte[] outBuffer = new byte[decompressedSize];

        ZlibError error = Zlib.Unpack(outBuffer, ref decompressedSize, buffer, buffer.Length);

        if (error != ZlibError.Okay)
        {
            throw new Exception(error.ToString());
        }

        return outBuffer;
    }
    
    public static ulong CompressLZ4(byte[] buffer, out byte[] compBuffer)
    {
        compBuffer = new byte[LZ4Codec.MaximumOutputSize(buffer.Length)];

        return (ulong)LZ4Codec.Encode(buffer, compBuffer);
    }

    public static byte[] DecompressLZ4(byte[] buffer, int decompressedSize)
    {
        byte[] outBuffer = new byte[decompressedSize];
        
        LZ4Codec.Decode(buffer, outBuffer);
        
        return outBuffer;
    }
    
    public static ulong CompressOodle(byte[] buffer, out byte[] compBuffer, Oodle.OodleFormat type)
    {
        compBuffer = new byte[Oodle.GetCompressedBufferSizeNeeded(buffer.Length)];
        
        GCHandle srcBufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        GCHandle compBufferHandle = GCHandle.Alloc(compBuffer, GCHandleType.Pinned);

        ulong size = Oodle.Compress(type, srcBufferHandle.AddrOfPinnedObject(), buffer.Length, compBufferHandle.AddrOfPinnedObject(),
            Oodle.OodleCompressionLevel.Optimal3, Oodle.GetOptions(type, Oodle.OodleCompressionLevel.Optimal3));
        
        srcBufferHandle.Free();
        compBufferHandle.Free();
        
        return size;
    }

    public static byte[] DecompressOodle(byte[] buffer, int decompressedSize)
    {
        byte[] outBuffer = new byte[decompressedSize];
        
        GCHandle compBufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        GCHandle outBufferHandle = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        Oodle.Decompress(compBufferHandle.AddrOfPinnedObject(), buffer.Length, outBufferHandle.AddrOfPinnedObject(),
            decompressedSize);

        return outBuffer;
    }
}