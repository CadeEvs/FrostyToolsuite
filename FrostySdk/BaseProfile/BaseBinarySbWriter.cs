using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.BaseProfile
{
    public class BaseBinarySbWriter : IBinarySbWriter
    {
        public void Write(DbWriter writer, DbObject bundleObj, Endian endian)
        {
            writer.Write(0xDEADBABE, Endian.Big);

            long startPos = writer.Position;

            writer.Write((uint)BaseBinarySb.GetMagic() ^ BaseBinarySb.GetSalt(), endian);
            writer.Write(bundleObj.GetValue<DbObject>("ebx").Count + bundleObj.GetValue<DbObject>("res").Count + bundleObj.GetValue<DbObject>("chunks").Count, endian);
            writer.Write(bundleObj.GetValue<DbObject>("ebx").Count, endian);
            writer.Write(bundleObj.GetValue<DbObject>("res").Count, endian);
            writer.Write(bundleObj.GetValue<DbObject>("chunks").Count, endian);
            writer.Write(0xDEADBABE, endian);
            writer.Write(0xDEADBABE, endian);
            writer.Write(0xDEADBABE, endian);

            // Writing bundle to stream, bc we might need to compress it
            MemoryStream ms = new MemoryStream();
            using (NativeWriter bundleWriter = new NativeWriter(ms, true))
            {
                if (BaseBinarySb.GetMagic() == BaseBinarySb.Magic.Standard)
                {
                    // sha1's
                    foreach (DbObject ebx in bundleObj.GetValue<DbObject>("ebx"))
                        bundleWriter.Write(ebx.GetValue<Sha1>("sha1"));
                    foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                        bundleWriter.Write(res.GetValue<Sha1>("sha1"));
                    foreach (DbObject chunk in bundleObj.GetValue<DbObject>("chunks"))
                        bundleWriter.Write(chunk.GetValue<Sha1>("sha1"));
                }

                // names
                long nameOffset = 0;
                Dictionary<uint, long> stringToOffsetMap = new Dictionary<uint, long>();
                List<string> stringsToPrint = new List<string>();
                foreach (DbObject ebx in bundleObj.GetValue<DbObject>("ebx"))
                {
                    uint hash = (uint)Fnv1.HashString(ebx.GetValue<string>("name"));
                    if (!stringToOffsetMap.ContainsKey(hash))
                    {
                        stringsToPrint.Add(ebx.GetValue<string>("name"));
                        stringToOffsetMap.Add(hash, nameOffset);
                        nameOffset += ebx.GetValue<string>("name").Length + 1;
                    }
                    bundleWriter.Write((uint)stringToOffsetMap[hash], endian);
                    bundleWriter.Write(ebx.GetValue<int>("originalSize"), endian);
                }
                foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                {
                    uint hash = (uint)Fnv1.HashString(res.GetValue<string>("name"));
                    if (!stringToOffsetMap.ContainsKey(hash))
                    {
                        stringsToPrint.Add(res.GetValue<string>("name"));
                        stringToOffsetMap.Add(hash, nameOffset);
                        nameOffset += res.GetValue<string>("name").Length + 1;
                    }
                    bundleWriter.Write((uint)stringToOffsetMap[hash], endian);
                    bundleWriter.Write(res.GetValue<int>("originalSize"), endian);
                }

                // res
                foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                    bundleWriter.Write(res.GetValue<int>("resType"), endian);
                foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                    bundleWriter.Write(res.GetValue<byte[]>("resMeta"));
                foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                    bundleWriter.Write(res.GetValue<long>("resRid"), endian);

                // chunks
                foreach (DbObject chunk in bundleObj.GetValue<DbObject>("chunks"))
                {
                    bundleWriter.Write(chunk.GetValue<Guid>("id"), endian);
                    bundleWriter.Write(chunk.GetValue<int>("logicalOffset"), endian);
                    bundleWriter.Write(chunk.GetValue<int>("logicalSize"), endian);
                }

                // meta
                long metaOffset = 0;
                long metaSize = 0;
                if (bundleObj.GetValue<DbObject>("chunkMeta") != null && bundleObj.GetValue<DbObject>("chunks").Count != 0)
                {
                    metaOffset = bundleWriter.Position + 0x20;
                    using (DbWriter metaWriter = new DbWriter(new MemoryStream()))
                        bundleWriter.Write(metaWriter.WriteDbObject("chunkMeta", bundleObj.GetValue<DbObject>("chunkMeta")));
                    metaSize = ((bundleWriter.Position + 0x20) - metaOffset);
                }

                // strings
                long stringsOffset = bundleWriter.Position + 0x20;
                foreach (string str in stringsToPrint)
                    bundleWriter.WriteNullTerminatedString(str);

                while ((bundleWriter.Position + 0x24) % 16 != 0)
                    bundleWriter.Write((byte)0x00);

                // update all relevant offsets
                writer.Position = startPos + 0x14;
                writer.Write((uint)stringsOffset, endian);
                writer.Write((uint)metaOffset, endian);
                writer.Write((uint)metaSize, endian);
            }

            // Compress bundle if necessary
            byte[] buffer = new byte[ms.Length];
            if (BaseBinarySb.GetMagic() == BaseBinarySb.Magic.Encrypted)
            {
                byte[] key = KeyManager.Instance.GetKey("Key2");

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = key;
                    aes.Padding = PaddingMode.None;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        cryptoStream.Write(buffer, 0, buffer.Length);
                }
            }
            else
                buffer = ms.ToArray();

            ms.Dispose();
            writer.Position = startPos - 4;
            writer.Write(buffer.Length + 0x20, Endian.Big);
            writer.Position = startPos + 0x20;
            writer.Write(buffer);
        }
    }
}
