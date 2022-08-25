using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.Managers;

namespace FrostySdk.IO
{
    public class LegacyBinarySbReader : BinarySbReader
    {
        private bool containsUncompressedData;
        private readonly long bundleOffset;

        public LegacyBinarySbReader (Stream inStream, long inBundleOffset, IDeobfuscator inDeobfuscator)
            : base(inStream, inDeobfuscator)
        {
            bundleOffset = inBundleOffset;
        }

        public LegacyBinarySbReader(Stream inBaseStream, Stream inDeltaStream, IDeobfuscator inDeobfuscator)
            : base(inBaseStream, inDeobfuscator)
        {
            if (inDeltaStream != null)
            {
                Stream patchedStream = PatchStream(inDeltaStream);

                inDeltaStream.Dispose();
                inDeltaStream = null;

                if (patchedStream != null)
                {
                    stream?.Dispose();

                    stream = patchedStream;
                    streamLength = stream.Length;
                }
                bundleOffset = 0;
            }
        }

        public override DbObject ReadDbObject()
        {
            DbObject bundle = base.ReadDbObject();

            ReadDataBlock(bundle.GetValue<DbObject>("ebx"), containsUncompressedData, bundleOffset);
            ReadDataBlock(bundle.GetValue<DbObject>("res"), containsUncompressedData, bundleOffset);
            ReadDataBlock(bundle.GetValue<DbObject>("chunks"), containsUncompressedData, bundleOffset);

            return bundle;
        }

        public void ReadDataBlock(DbObject list, bool containsUncompressedData, long bundleOffset)
        {
            foreach (DbObject entry in list)
            {
                entry.AddValue("offset", bundleOffset + Position);

                long originalSize = entry.GetValue<long>("originalSize");
                long size = 0;

                if (containsUncompressedData)
                {
                    size = originalSize;
                    entry.AddValue("data", ReadBytes((int)originalSize));
                }
                else
                {
                    while (originalSize > 0)
                    {
                        int decompressedSize = ReadInt(Endian.Big);
                        ushort compressionType = ReadUShort();
                        int bufferSize = ReadUShort(Endian.Big);

                        int flags = ((compressionType & 0xFF00) >> 8);

                        if ((flags & 0x0F) != 0)
                            bufferSize = ((flags & 0x0F) << 0x10) + bufferSize;
                        if ((decompressedSize & 0xFF000000) != 0)
                            decompressedSize &= 0x00FFFFFF;

                        originalSize -= decompressedSize;

                        compressionType = (ushort)(compressionType & 0x7F);
                        if (compressionType == 0x00)
                            bufferSize = decompressedSize;

                        size += bufferSize + 8;
                        Position += bufferSize;
                    }
                }

                entry.AddValue("size", size);
                entry.AddValue("sb", true);
            }
        }

        private Stream PatchStream(Stream deltaStream)
        {
            deltaStream.Position += 8;
            MemoryStream patchedStream = new MemoryStream();

            using (NativeReader reader = new NativeReader(deltaStream))
            {
                uint dataOffset = reader.ReadUInt(Endian.Big);
                uint dataSize = reader.ReadUInt(Endian.Big);

                patchedStream.Write(reader.ReadBytes(4), 0, 4);
                if (stream != null)
                    ReadUInt(Endian.Big);

                while (reader.Position <= dataOffset + 0x0F)
                {
                    uint tmpVal = reader.ReadUInt(Endian.Big);
                    uint blockType = (tmpVal & 0xFF000000) >> 24;
                    int blockData = (int)(tmpVal & 0x00FFFFFF);

                    switch (blockType)
                    {
                        case 0x00: patchedStream.Write(ReadBytes(blockData), 0, blockData); break;
                        case 0x40: Position += blockData; break;
                        case 0x80: patchedStream.Write(reader.ReadBytes(blockData), 0, blockData); break;
                    }
                }

                using (CasReader dataReader = new CasReader(stream, deltaStream))
                {
                    byte[] dataBuffer = dataReader.Read();
                    patchedStream.Write(dataBuffer, 0, dataBuffer.Length);
                }
            }

            patchedStream.Position = 0;
            containsUncompressedData = true;

            return patchedStream;
        }
    }
}