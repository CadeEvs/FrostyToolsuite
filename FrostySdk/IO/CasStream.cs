
using System;
using System.IO;
using System.Security.Cryptography;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Profiles;

/* Cas files contain the data for the assets which are used by the game. For Ebx and Res those are just compressed and then stored.
 * Chunks on the other hand are a bit special. Texture chunks can be stored just as a range, so it stores a range of mips in the bundle and the full chunk in the superbundle.
 * The LogicalOffset, which is the offset of the FirstMip in the compressed chunk will also get stored in the catalog.
 */

namespace Frosty.Sdk.IO;

// TODO: maybe make this into static class
public class CasStream : DataStream
{
    private Stream? m_deltaStream;
    
    public CasStream(Stream inBaseStream, Stream? inDeltaStream = null)
        : base(inBaseStream)
    {
        m_deltaStream = inDeltaStream;
    }
    
    public CasStream(Stream inBaseStream, byte[]? inEncryptionKey, uint actualSize)
        : base(DecryptStream(inBaseStream, inEncryptionKey, actualSize))
    {
    }

    public CasStream(Stream inBaseStream, byte[]? inBaseEncryptionKey, uint actualBaseSize, Stream inDeltaStream, byte[]? inDeltaEncryptionKey, uint actualPatchSize)
        : this(DecryptStream(inBaseStream, inBaseEncryptionKey, actualBaseSize), DecryptStream(inDeltaStream, inDeltaEncryptionKey, actualPatchSize))
    {
    }

    public CasStream(Stream inBaseStream, byte[]? inBaseEncryptionKey, uint actualBaseSize, Stream inDeltaStream)
        : this(DecryptStream(inBaseStream, inBaseEncryptionKey, actualBaseSize), inDeltaStream)
    {
    }
    
    public byte[] Read()
    {
        MemoryStream outStream = new();

        if (m_deltaStream != null)
        {
            byte[] patchBuf = ReadPatched();
            outStream.Write(patchBuf, 0, patchBuf.Length);
        }

        if (m_stream != null)
        {
            while (Position < Length)
            {
                byte[] subBuffer = ReadBlock();
                outStream.Write(subBuffer, 0, subBuffer.Length);
            }
        }

        return outStream.ToArray();
    }

    private byte[] ReadPatched()
    {
        byte[] retVal;
        using (MemoryStream outStream = new())
        {
            using (CasStream reader = new(m_deltaStream!))
            {
                while (reader.Position < reader.Length)
                {
                    uint tmpVal = reader.ReadUInt32(Endian.Big);
                    int blockType = (int)(tmpVal & 0xF0000000) >> 28;

                    if (blockType == 0x00) // Read base blocks
                    {
                        int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                        while (blockCount-- > 0)
                        {
                            byte[] tmpBuffer = ReadBlock();
                            outStream.Write(tmpBuffer, 0, tmpBuffer.Length);
                        }
                    }
                    else if (blockType == 0x01) // Merge base w/delta
                    {
                        int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                        using (MemoryStream baseMs = new(ReadBlock()))
                        {
                            byte[] tmpBuffer = null;

                            while (blockCount-- > 0)
                            {
                                int offset = reader.ReadUInt16(Endian.Big);
                                int skipCount = reader.ReadUInt16(Endian.Big);

                                int numBytes = (int)(offset - baseMs.Position);
                                tmpBuffer = new byte[numBytes];

                                baseMs.Read(tmpBuffer, 0, numBytes);
                                outStream.Write(tmpBuffer, 0, numBytes);

                                tmpBuffer = reader.ReadBlock();
                                if (tmpBuffer != null)
                                    outStream.Write(tmpBuffer, 0, tmpBuffer.Length);

                                baseMs.Position += skipCount;
                            }

                            tmpBuffer = new byte[(int)(baseMs.Length - baseMs.Position)];

                            baseMs.Read(tmpBuffer, 0, tmpBuffer.Length);
                            outStream.Write(tmpBuffer, 0, tmpBuffer.Length);
                        }
                    }
                    else if (blockType == 0x02) // Merge base w/delta
                    {
                        using (MemoryStream baseMs = new(ReadBlock()))
                        {
                            int deltaBlockSize = (int)(tmpVal & 0x0FFFFFFF);
                            int newBlockSize = reader.ReadUInt16(Endian.Big) + 1;

                            byte[] tmpBuffer = new byte[newBlockSize];
                            long startPos = reader.Position;

                            int bufferOffset = 0;
                            while (reader.Position - startPos < deltaBlockSize)
                            {
                                ushort offset = reader.ReadUInt16(Endian.Big);
                                int skipCount = reader.ReadByte();
                                int addCount = reader.ReadByte();

                                bufferOffset += baseMs.Read(tmpBuffer, bufferOffset, (int)(offset - baseMs.Position));
                                baseMs.Position += skipCount;
                                bufferOffset += reader.Read(tmpBuffer, bufferOffset, addCount);
                            }

                            baseMs.Read(tmpBuffer, bufferOffset, newBlockSize - bufferOffset);
                            outStream.Write(tmpBuffer, 0, tmpBuffer.Length);
                        }
                    }
                    else if (blockType == 0x03) // Read delta blocks
                    {
                        int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                        while (blockCount-- > 0)
                        {
                            byte[] tmpBuffer = reader.ReadBlock();
                            outStream.Write(tmpBuffer, 0, tmpBuffer.Length);
                        }
                    }
                    else if (blockType == 0x04) // Skip blocks
                    {
                        int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                        while (blockCount-- > 0)
                            ReadBlock();
                    }
                }
            }

            m_deltaStream = null;
            retVal = outStream.ToArray();
        }
        
        return retVal;
    }
    
    private byte[] ReadBlock()
    {
        int decompressedSize = ReadInt32(Endian.Big);
        ushort compressionType = ReadUInt16();
        int bufferSize = ReadUInt16(Endian.Big);

        int flags = ((compressionType & 0xFF00) >> 8);
        bool useDictionary = false;
        byte[] buffer;

        if ((flags & 0x0F) != 0)
        {
            bufferSize = ((flags & 0x0F) << 0x10) + bufferSize;
        }

        if ((decompressedSize & 0xFF000000) != 0)
        {
            decompressedSize &= 0x00FFFFFF;
            useDictionary = true;
        }

        byte[] compressedBuffer = ReadBytes(bufferSize);
        
        if (((compressionType >> 7) & 1) != 0)
        {
            // TODO: check if this is only set in fifa 19
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19))
            {
                throw new Exception("obfuscation");
            }
            
            byte[] obfusBlock = KeyManager.GetKey("CasObfuscationKey");
            for (int i = 0; i < bufferSize; i++)
            {
                compressedBuffer[i] ^= obfusBlock[i & 0x3FFF];
            }
        }
        compressionType = (ushort)(compressionType & 0x7F);

        if (compressionType == 0x00) // None
        {
            buffer = compressedBuffer;
        }
        else if (compressionType == 0x02) // ZLib
        {
            buffer = Utils.CompressionUtils.DecompressZLib(compressedBuffer, decompressedSize);
        }
        else if (compressionType == 0x0F) // ZStd
        {
            buffer = Utils.CompressionUtils.DecompressZStd(compressedBuffer, decompressedSize, useDictionary);
        }
        else if (compressionType == 0x09) // LZ4
        {
            buffer = Utils.CompressionUtils.DecompressLZ4(compressedBuffer, decompressedSize);
        }
        else if (compressionType == 0x11) // Oodle Kraken
        {
            buffer = Utils.CompressionUtils.DecompressOodle(compressedBuffer, decompressedSize);
        }
        else if (compressionType == 0x15) // Oodle Selkie
        {
            buffer = Utils.CompressionUtils.DecompressOodle(compressedBuffer, decompressedSize);
        }
        else if (compressionType == 0x19) // Oodle Leviathan
        {
            buffer = Utils.CompressionUtils.DecompressOodle(compressedBuffer, decompressedSize);
        }
        else
        {
            throw new NotImplementedException($"Compression type: {compressionType}");
        }

        return buffer;
    }
    
    private static Stream DecryptStream(Stream inStream, byte[]? encryptionKey, uint actualSize)
    {
        if (encryptionKey == null)
            return inStream;

        ICryptoTransform decryptor;

        using (Aes aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.IV = encryptionKey;

            decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        }

        byte[] retVal = new byte[actualSize];

        using (CryptoStream stream = new(inStream, decryptor, CryptoStreamMode.Read))
        {
            stream.Read(retVal, 0, retVal.Length);
        }
        
        return new MemoryStream(retVal);
    }
}