using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.Managers;

namespace FrostySdk.IO
{
    public class BinarySbReader : DbReader
    {
        public const uint BinarySbMagic = 0x9D798ED5;

        public uint TotalCount { get => binarySbReader.TotalCount; }

        private IBinarySbReader binarySbReader;

        private bool containsUncompressedData;
        private readonly long bundleOffset;

        public BinarySbReader(Stream inStream, long inBundleOffset, IDeobfuscator inDeobfuscator)
            : base(inStream, inDeobfuscator)
        {
            binarySbReader = ProfilesLibrary.Profile.GetBinarySbReader();

            bundleOffset = inBundleOffset;
        }

        public BinarySbReader(Stream inBaseStream, Stream inDeltaStream, IDeobfuscator inDeobfuscator)
            : base(inBaseStream, inDeobfuscator)
        {
            binarySbReader = ProfilesLibrary.Profile.GetBinarySbReader();

            if (inDeltaStream != null)
            {
                Stream patchedStream = PatchStream(inDeltaStream);

                inDeltaStream.Dispose();
                inDeltaStream = null;

                if(patchedStream != null)
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
            return binarySbReader.ReadDbObject(this, containsUncompressedData, bundleOffset);
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