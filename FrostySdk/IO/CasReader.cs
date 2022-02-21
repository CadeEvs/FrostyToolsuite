using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using FrostySdk.Managers;

namespace FrostySdk.IO
{
    public class CasReader : NativeReader
    {
        private Stream deltaStream;
        public CasReader(Stream inBaseStream, Stream inDeltaStream = null)
            : base(inBaseStream)
        {
            deltaStream = inDeltaStream;
        }

        public CasReader(Stream inStream, byte[] inEncryptionKey, uint actualSize)
            : base(DecryptStream(inStream, inEncryptionKey, actualSize))
        {
        }

        public CasReader(Stream inBaseStream, byte[] inBaseEncryptionKey, uint actualBaseSize, Stream inDeltaStream, byte[] inDeltaEncryptionKey, uint actualPatchSize)
            : this(DecryptStream(inBaseStream, inBaseEncryptionKey, actualBaseSize), DecryptStream(inDeltaStream, inDeltaEncryptionKey, actualPatchSize))
        {
        }

        public CasReader(Stream inBaseStream, byte[] inBaseEncryptionKey, uint actualBaseSize, Stream inDeltaStream)
            : this(DecryptStream(inBaseStream, inBaseEncryptionKey, actualBaseSize), inDeltaStream)
        {
        }

        public byte[] Read()
        {
            MemoryStream outStream = new MemoryStream();

            if (deltaStream != null)
            {
                byte[] patchBuf = ReadPatched();
                outStream.Write(patchBuf, 0, patchBuf.Length);
            }

            if (stream != null)
            {
                while (Position < Length)
                {
                    byte[] subBuffer = ReadBlock();
                    if (subBuffer == null)
                        break;
                    outStream.Write(subBuffer, 0, subBuffer.Length);
                }
            }

            return outStream.ToArray();
        }

        public byte[] ReadBlock()
        {
            int decompressedSize = ReadInt(Endian.Big);
            ushort compressionType = ReadUShort();
            int bufferSize = ReadUShort(Endian.Big);

            int flags = ((compressionType & 0xFF00) >> 8);
            bool useDictionary = false;
            byte[] buffer = null;

            if ((flags & 0x0F) != 0)
                bufferSize = ((flags & 0x0F) << 0x10) + bufferSize;
            if ((decompressedSize & 0xFF000000) != 0)
            {
                decompressedSize &= 0x00FFFFFF;
                useDictionary = true;
            }

            bool unobfuscate = (((compressionType >> 7) & 1) != 0) && ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19;
            compressionType = (ushort)(compressionType & 0x7F);

            if (compressionType == 0x00) // No Compression
                buffer = ReadUncompressed(bufferSize, unobfuscate);
            else if (compressionType == 0x02) // ZLib compression
                buffer = DecompressBlockZLib(bufferSize, decompressedSize);
            else if (compressionType == 0x0f) // ZStd Compression
                buffer = DecompressBlockZStd(bufferSize, decompressedSize, useDictionary, unobfuscate);
            else if (compressionType == 0x09) // Diff
                buffer = DecompressBlockLZ4(bufferSize, decompressedSize);
            else if (compressionType == 0x11) // Oodle (v6)
                buffer = DecompressOodle(bufferSize, decompressedSize, unobfuscate);
            else if (compressionType == 0x15) // Oodle (v4)
                buffer = DecompressOodle(bufferSize, decompressedSize, unobfuscate);

            return buffer;
        }

        private byte[] ReadUncompressed(int bufferSize, bool unobfuscate)
        {
            byte[] tmpBuffer = ReadBytes(bufferSize);
            if (unobfuscate)
            {
                byte[] obfusBlock = KeyManager.Instance.GetKey("Key3");
                for (int i = 0; i < bufferSize; i++)
                    tmpBuffer[i] ^= obfusBlock[i & 0x3FFF];
            }
            return tmpBuffer;
        }

        private byte[] DecompressOodle(int bufferSize, int decompressedSize, bool unobfuscate)
        {
            byte[] tmpBuffer = ReadBytes(bufferSize);
            if (unobfuscate)
            {
                byte[] obfusBlock = KeyManager.Instance.GetKey("Key3");
                for (int i = 0; i < bufferSize; i++)
                    tmpBuffer[i] ^= obfusBlock[i & 0x3FFF];
            }
            GCHandle Ptr1 = GCHandle.Alloc(tmpBuffer, GCHandleType.Pinned);

            byte[] outBuffer = new byte[decompressedSize];
            GCHandle Ptr2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            int retCode = Oodle.Decompress(Ptr1.AddrOfPinnedObject(), tmpBuffer.Length, Ptr2.AddrOfPinnedObject(), outBuffer.Length);

            Ptr1.Free();
            Ptr2.Free();

            return outBuffer;
        }

        private byte[] DecompressBlockZStd(int bufferSize, int decompressedSize, bool useDictionary, bool unobfuscate)
        {
            byte[] tmpBuffer = ReadBytes(bufferSize);
            if (unobfuscate)
            {
                byte[] obfusBlock = KeyManager.Instance.GetKey("Key3");
                for (int i = 0; i < bufferSize; i++)
                    tmpBuffer[i] ^= obfusBlock[i & 0x3FFF];
            }
            GCHandle Ptr1 = GCHandle.Alloc(tmpBuffer, GCHandleType.Pinned);

            byte[] outBuffer = new byte[decompressedSize];
            GCHandle Ptr2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            ulong error = 0;
            if (useDictionary)
            {
                byte[] digestedDict = ZStd.GetDictionary();
                IntPtr handle = ZStd.Create();

                // create digested dictionary
                GCHandle dictPtr = GCHandle.Alloc(digestedDict, GCHandleType.Pinned);
                IntPtr ddict = ZStd.CreateDigestedDict(dictPtr.AddrOfPinnedObject(), digestedDict.Length);

                // decompress using digested dictionary
                error = ZStd.DecompressUsingDict(handle, Ptr2.AddrOfPinnedObject(), (ulong)outBuffer.Length, Ptr1.AddrOfPinnedObject(), (ulong)tmpBuffer.Length, ddict);

                dictPtr.Free();
                ZStd.FreeDigestedDict(ddict);
                ZStd.Free(handle);
            }
            else
            {
                // normal decompression
                error = ZStd.Decompress(Ptr2.AddrOfPinnedObject(), (ulong)outBuffer.Length, Ptr1.AddrOfPinnedObject(), (ulong)tmpBuffer.Length);
            }

            if (ZStd.IsError(error))
            {
                string errorString = Marshal.PtrToStringAnsi(ZStd.GetErrorName(error));
                throw new InvalidDataException(errorString);
            }

            Ptr1.Free();
            Ptr2.Free();

            return outBuffer;
        }

        private byte[] DecompressBlockZLib(int bufferSize, int decompressedSize)
        {
            byte[] tmpBuffer = ReadBytes(bufferSize);
            GCHandle ptr1 = GCHandle.Alloc(tmpBuffer, GCHandleType.Pinned);

            byte[] outBuffer = new byte[decompressedSize];
            GCHandle ptr2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            ZLib.ZStream stream = new ZLib.ZStream
            {
                avail_in = (uint)tmpBuffer.Length,
                avail_out = (uint)outBuffer.Length,
                next_in = ptr1.AddrOfPinnedObject(),
                next_out = ptr2.AddrOfPinnedObject()
            };

            IntPtr streamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(stream));
            Marshal.StructureToPtr(stream, streamPtr, true);

            int retCode = 0;
            retCode = ZLib.InflateInit(streamPtr, "1.2.11", Marshal.SizeOf<ZLib.ZStream>());
            retCode = ZLib.Inflate(streamPtr, 0);
            retCode = ZLib.InflateEnd(streamPtr);

            ptr1.Free();
            ptr2.Free();

            Marshal.FreeHGlobal(streamPtr);
            return outBuffer;
        }

        private byte[] DecompressBlockLZ4(int bufferSize, int decompressedSize)
        {
            byte[] tmpBuffer = ReadBytes(bufferSize);
            GCHandle ptr1 = GCHandle.Alloc(tmpBuffer, GCHandleType.Pinned);

            byte[] outBuffer = new byte[decompressedSize];
            GCHandle ptr2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            LZ4.Decompress(ptr1.AddrOfPinnedObject(), ptr2.AddrOfPinnedObject(), outBuffer.Length);

            ptr1.Free();
            ptr2.Free();

            return outBuffer;
        }

        private byte[] ReadPatched()
        {
            MemoryStream outStream = new MemoryStream();
            using (CasReader reader = new CasReader(deltaStream))
            {
                while (reader.Position < reader.Length)
                {
                    uint tmpVal = reader.ReadUInt(Endian.Big);
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
                        using (MemoryStream baseMs = new MemoryStream(ReadBlock()))
                        {
                            byte[] tmpBuffer = null;

                            while (blockCount-- > 0)
                            {
                                int offset = reader.ReadUShort(Endian.Big);
                                int skipCount = reader.ReadUShort(Endian.Big);

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
                        using (MemoryStream baseMs = new MemoryStream(ReadBlock()))
                        {
                            int deltaBlockSize = (int)(tmpVal & 0x0FFFFFFF);
                            int newBlockSize = reader.ReadUShort(Endian.Big) + 1;

                            byte[] tmpBuffer = new byte[newBlockSize];
                            long startPos = reader.Position;

                            int bufferOffset = 0;
                            while (reader.Position - startPos < deltaBlockSize)
                            {
                                ushort offset = reader.ReadUShort(Endian.Big);
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

            deltaStream = null;
            return outStream.ToArray();
        }

        private static Stream DecryptStream(Stream inStream, byte[] encryptionKey, uint actualSize)
        {
            if (encryptionKey == null)
                return inStream;

            int size = (int)inStream.Length;

            byte[] tmpBuffer = new byte[size];
            inStream.Read(tmpBuffer, 0, size);

            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = encryptionKey;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream decryptStream = new MemoryStream(tmpBuffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(decryptStream, decryptor, CryptoStreamMode.Read))
                        cryptoStream.Read(tmpBuffer, 0, size);
                }
            }

            inStream.Dispose();
            Array.Resize<byte>(ref tmpBuffer, (int)actualSize);
            return new MemoryStream(tmpBuffer);
        }
    }
}
