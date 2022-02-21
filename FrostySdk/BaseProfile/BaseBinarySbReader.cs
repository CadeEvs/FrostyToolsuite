using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace FrostySdk.BaseProfile
{
    public class BaseBinarySbReader : IBinarySbReader
    {
        public uint TotalCount { get => totalCount; }

        private uint totalCount;

        private uint ebxCount;
        private uint resCount;
        private uint chunkCount;
        private uint stringsOffset;
        private uint metaOffset;
        private uint metaSize;

        private Endian endian = Endian.Big;

        private List<Sha1> sha1 = new List<Sha1>();

        public DbObject ReadDbObject(DbReader reader, bool containsUncompressedData, long bundleOffset)
        {
            uint dataOffset = reader.ReadUInt(Endian.Big) + 4;
            uint magic = reader.ReadUInt(endian) ^ 0x7065636E;

            bool containsSha1 = !(magic == 0xC3889333 || magic == 0xC3E5D5C3);

            totalCount = reader.ReadUInt(endian);
            ebxCount = reader.ReadUInt(endian);
            resCount = reader.ReadUInt(endian);
            chunkCount = reader.ReadUInt(endian);
            stringsOffset = reader.ReadUInt(endian) - 0x24;
            metaOffset = reader.ReadUInt(endian) - 0x24;
            metaSize = reader.ReadUInt(endian);

            byte[] buffer = (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem
                             || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville
                             || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat
                )
                    ? reader.ReadToEnd()
                    : reader.ReadBytes((int)(dataOffset - reader.Position));

            if (magic == 0xC3889333)
                containsSha1 = false;
            else if (magic == 0xC3E5D5C3)
            {
                containsSha1 = false;
                byte[] key = KeyManager.Instance.GetKey("Key2");

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = key;
                    aes.Padding = PaddingMode.None;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream decryptStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(decryptStream, decryptor, CryptoStreamMode.Read))
                            cryptoStream.Read(buffer, 0, buffer.Length);
                    }
                }
            }

            DbObject bundle = new DbObject(new Dictionary<string, object>());
            using (DbReader dbReader = new DbReader(new MemoryStream(buffer), null))
            {
                for (int i = 0; i < totalCount; i++)
                    sha1.Add((containsSha1)
                        ? dbReader.ReadSha1()
                        : Sha1.Zero
                    );

                bundle.AddValue("ebx", new DbObject(ReadEbx(dbReader)));
                bundle.AddValue("res", new DbObject(ReadRes(dbReader)));
                bundle.AddValue("chunks", new DbObject(ReadChunks(dbReader)));
                bundle.AddValue("dataOffset", (int)(dataOffset - 4));

                if (chunkCount > 0)
                {
                    dbReader.Position = metaOffset + 4;
                    bundle.AddValue("chunkMeta", dbReader.ReadDbObject());
                }
            }

            reader.Position = dataOffset;
            if (reader.Position == reader.Length)
                return bundle;

            if (magic == 0xED1CEDB8)
                return bundle;

            ReadDataBlock(reader, bundle.GetValue<DbObject>("ebx"), containsUncompressedData, bundleOffset);
            ReadDataBlock(reader, bundle.GetValue<DbObject>("res"), containsUncompressedData, bundleOffset);
            ReadDataBlock(reader, bundle.GetValue<DbObject>("chunks"), containsUncompressedData, bundleOffset);

            System.Diagnostics.Debug.Assert(reader.Position <= reader.Length);
            return bundle;
        }

        public void ReadDataBlock(DbReader reader, DbObject list, bool containsUncompressedData, long bundleOffset)
        {
            foreach (DbObject entry in list)
            {
                entry.AddValue("offset", bundleOffset + reader.Position);

                long originalSize = entry.GetValue<long>("originalSize");
                long size = 0;

                if (containsUncompressedData)
                {
                    size = originalSize;
                    entry.AddValue("data", reader.ReadBytes((int)originalSize));
                }
                else
                {
                    while (originalSize > 0)
                    {
                        int decompressedSize = reader.ReadInt(Endian.Big);
                        ushort compressionType = reader.ReadUShort();
                        int bufferSize = reader.ReadUShort(Endian.Big);

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
                        reader.Position += bufferSize;
                    }
                }

                entry.AddValue("size", size);
                entry.AddValue("sb", true);
            }
        }

        public List<object> ReadEbx(DbReader reader)
        {
            List<object> ebxList = new List<object>();

            for (int i = 0; i < ebxCount; i++)
            {
                DbObject entry = new DbObject(new Dictionary<string, object>());

                uint nameOffset = reader.ReadUInt(endian);
                uint originalSize = reader.ReadUInt(endian);

                long currentPos = reader.Position;
                reader.Position = 4 + stringsOffset + nameOffset;

                entry.AddValue("sha1", sha1[i]);
                entry.AddValue("name", reader.ReadNullTerminatedString());
                entry.AddValue("nameHash", Fnv1.HashString(entry.GetValue<string>("name")));
                entry.AddValue("originalSize", originalSize);
                ebxList.Add(entry);

                reader.Position = currentPos;
            }

            return ebxList;
        }

        public List<object> ReadRes(DbReader reader)
        {
            List<object> resList = new List<object>();
            int offset = (int)ebxCount;

            for (int i = 0; i < resCount; i++)
            {
                DbObject entry = new DbObject(new Dictionary<string, object>());

                uint nameOffset = reader.ReadUInt(endian);
                uint originalSize = reader.ReadUInt(endian);

                long currentPos = reader.Position;
                reader.Position = 4 + stringsOffset + nameOffset;

                entry.AddValue("sha1", sha1[offset++]);
                entry.AddValue("name", reader.ReadNullTerminatedString());
                entry.AddValue("nameHash", Fnv1.HashString(entry.GetValue<string>("name")));
                entry.AddValue("originalSize", originalSize);
                resList.Add(entry);

                reader.Position = currentPos;
            }

            foreach (DbObject res in resList)
                res.AddValue("resType", reader.ReadUInt(Endian.Big));

            foreach (DbObject res in resList)
                res.AddValue("resMeta", reader.ReadBytes(0x10));

            foreach (DbObject res in resList)
                res.AddValue("resRid", reader.ReadLong(Endian.Big));

            return resList;
        }

        public List<object> ReadChunks(DbReader reader)
        {
            List<object> chunkList = new List<object>();
            int offset = (int)(ebxCount + resCount);

            for (int i = 0; i < chunkCount; i++)
            {
                DbObject entry = new DbObject(new Dictionary<string, object>());

                Guid chunkId = reader.ReadGuid(endian);
                uint logicalOffset = reader.ReadUInt(endian);
                uint logicalSize = reader.ReadUInt(endian);
                long originalSize = (logicalOffset & 0xFFFF) | logicalSize;

                entry.AddValue("id", chunkId);
                entry.AddValue("sha1", sha1[offset + i]);
                entry.AddValue("logicalOffset", logicalOffset);
                entry.AddValue("logicalSize", logicalSize);
                entry.AddValue("originalSize", originalSize);

                chunkList.Add(entry);
            }

            return chunkList;
        }

        public void ReadChunkMeta(DbReader reader, DbObject bundle, uint metaOffset)
        {
            reader.Position = metaOffset + 4;
            bundle.AddValue("chunkMeta", reader.ReadDbObject());
        }
    }
}
