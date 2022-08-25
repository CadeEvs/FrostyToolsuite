using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

        public DbObject ReadDbObject(DbReader reader)
        {
            uint size = reader.ReadUInt(Endian.Big);
            BaseBinarySb.Magic magic = (BaseBinarySb.Magic)(reader.ReadUInt(endian) ^ BaseBinarySb.GetSalt());

            // check what endian its written in
            if (!BaseBinarySb.IsValidMagic(magic))
            {
                endian = Endian.Little;
                reader.Position -= 4;
                magic = (BaseBinarySb.Magic)(reader.ReadUInt(endian) ^ BaseBinarySb.GetSalt());

                if (!BaseBinarySb.IsValidMagic(magic))
                    throw new InvalidDataException("magic");
            }

            bool containsSha1 = !(magic == BaseBinarySb.Magic.Fifa || magic == BaseBinarySb.Magic.Encrypted);

            totalCount = reader.ReadUInt(endian);
            ebxCount = reader.ReadUInt(endian);
            resCount = reader.ReadUInt(endian);
            chunkCount = reader.ReadUInt(endian);
            stringsOffset = reader.ReadUInt(endian) - 0x20;
            metaOffset = reader.ReadUInt(endian) - 0x20;
            metaSize = reader.ReadUInt(endian);

            byte[] buffer = reader.ReadBytes((int)(size - 0x20));

            // decrypt the data
            if (magic == BaseBinarySb.Magic.Encrypted)
            {
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
                    sha1.Add((containsSha1) ? dbReader.ReadSha1() : Sha1.Zero);

                bundle.AddValue("ebx", new DbObject(ReadEbx(dbReader)));
                bundle.AddValue("res", new DbObject(ReadRes(dbReader)));
                bundle.AddValue("chunks", new DbObject(ReadChunks(dbReader)));
                bundle.AddValue("dataOffset", (int)size);

                if (chunkCount > 0)
                {
                    dbReader.Position = metaOffset;
                    bundle.AddValue("chunkMeta", dbReader.ReadDbObject());
                }
            }

            return bundle;
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
                reader.Position = stringsOffset + nameOffset;

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
                reader.Position = stringsOffset + nameOffset;

                entry.AddValue("sha1", sha1[offset++]);
                entry.AddValue("name", reader.ReadNullTerminatedString());
                entry.AddValue("nameHash", Fnv1.HashString(entry.GetValue<string>("name")));
                entry.AddValue("originalSize", originalSize);
                resList.Add(entry);

                reader.Position = currentPos;
            }

            foreach (DbObject res in resList)
                res.AddValue("resType", reader.ReadUInt(endian));

            foreach (DbObject res in resList)
                res.AddValue("resMeta", reader.ReadBytes(0x10));

            foreach (DbObject res in resList)
                res.AddValue("resRid", reader.ReadLong(endian));

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
