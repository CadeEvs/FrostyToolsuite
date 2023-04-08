using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.IO;

public static class BinaryBundle
{
    public enum Magic : uint
    {
        Standard = 0xED1CEDB8,
        Fifa = 0xC3889333,
        Encrypted = 0xC3E5D5C3
    }
    
    /// <summary>
    /// Dependent on the FB version, games have different salts.
    /// If the game uses a version newer than 2017 it uses "pecn" else it uses "pecm".
    /// <see cref="ProfileVersion.Battlefield5"/> is the only game that uses "arie".
    /// </summary>
    /// <returns>The salt, that the current game uses.</returns>
    public static uint GetSalt()
    {
        string salt = "pecm";

        if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
            salt = "arie";

        else if (ProfilesLibrary.DataVersion >= (int)ProfileVersion.Fifa19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
            salt = "pecn";


        byte[] bytes = Encoding.ASCII.GetBytes(salt);
        return (uint)(bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] << 0);
    }

    /// <summary>
    /// Only the games using the FifaAssetLoader use a different Magic than <see cref="Magic.Standard"/>.
    /// </summary>
    /// <returns>The magic the current game uses.</returns>
    public static Magic GetMagic()
    {
        switch (ProfilesLibrary.DataVersion)
        {
            // TODO: what game uses encrypted magic
            case (int)ProfileVersion.Fifa19:
            case (int)ProfileVersion.Madden20:
            case (int)ProfileVersion.Fifa20:
            case (int)ProfileVersion.Madden21:
                return Magic.Fifa;
            default:
                return Magic.Standard;
        }
    }

    public static bool IsValidMagic(Magic magic) => Enum.IsDefined(typeof(Magic), magic);
    
    /// <summary>
    /// Deserialize a binary stored bundle as <see cref="DbObject"/>.  
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> from which the bundle should be Deserialized from.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">Is thrown when there is no valid <see cref="Magic"/>.</exception>
    public static DbObject Deserialize(DataStream stream)
    {
        Endian endian = Endian.Big;
        List<Sha1> sha1 = new List<Sha1>();
        
        uint size = stream.ReadUInt32(Endian.Big);
        Magic magic = (Magic)(stream.ReadUInt32(endian) ^ GetSalt());

        // check what endian its written in
        if (!IsValidMagic(magic))
        {
            endian = Endian.Little;
            stream.Position -= 4;
            magic = (Magic)(stream.ReadUInt32(endian) ^ GetSalt());

            if (!IsValidMagic(magic))
            {
                throw new InvalidDataException("magic");
            }
        }

        bool containsSha1 = !(magic == Magic.Fifa || magic == Magic.Encrypted);

        uint totalCount = stream.ReadUInt32(endian);
        uint ebxCount = stream.ReadUInt32(endian);
        uint resCount = stream.ReadUInt32(endian);
        uint chunkCount = stream.ReadUInt32(endian);
        uint stringsOffset = stream.ReadUInt32(endian) - 0x20;
        uint metaOffset = stream.ReadUInt32(endian) - 0x20;
        uint metaSize = stream.ReadUInt32(endian);

        byte[] buffer = stream.ReadBytes((int)(size - 0x20));
        byte[] decrypted;

        // decrypt the data
        if (magic == Magic.Encrypted)
        {
            decrypted = new byte[buffer.Length];
            byte[] key = KeyManager.GetKey("Key2");

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = key;
                aes.Padding = PaddingMode.None;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (CryptoStream cryptoStream = new(new MemoryStream(buffer), decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.Read(decrypted, 0, buffer.Length);
                }
            }
        }
        else
        {
            decrypted = buffer;
        }

        DbObject bundle = new(new Dictionary<string, object>());
        using (DataStream dbReader = new(new MemoryStream(decrypted)))
        {
            for (int i = 0; i < totalCount; i++)
            {
                sha1.Add((containsSha1) ? dbReader.ReadSha1() : Sha1.Zero);
            }

            bundle.AddValue("ebx", new DbObject(ReadEbx(dbReader, endian, ebxCount, sha1, 0, stringsOffset)));
            bundle.AddValue("res", new DbObject(ReadRes(dbReader, endian, resCount, sha1, (int)ebxCount, stringsOffset)));
            bundle.AddValue("chunks", new DbObject(ReadChunks(dbReader, endian, chunkCount, sha1, (int)(ebxCount + resCount))));
            bundle.AddValue("dataOffset", (int)size);

            if (chunkCount > 0)
            {
                dbReader.Position = metaOffset;
                bundle.AddValue("chunkMeta", DbObject.Deserialize(dbReader)!);
                Debug.Assert(dbReader.Position == metaOffset + metaSize);
            }
        }

        return bundle;
    } 
    
    private static List<object> ReadEbx(DataStream stream, Endian endian, uint count, List<Sha1> sha1, int offset, long stringsOffset)
    {
        List<object> ebxList = new List<object>();

        for (int i = 0; i < count; i++)
        {
            DbObject entry = new(new Dictionary<string, object>());

            uint nameOffset = stream.ReadUInt32(endian);
            uint originalSize = stream.ReadUInt32(endian);

            long currentPos = stream.Position;
            stream.Position = stringsOffset + nameOffset;

            entry.AddValue("sha1", sha1[offset + i]);
            entry.AddValue("name", stream.ReadNullTerminatedString());
            entry.AddValue("nameHash", Utils.Utils.HashString(entry["name"].As<string>()));
            entry.AddValue("originalSize", originalSize);
            ebxList.Add(entry);

            stream.Position = currentPos;
        }

        return ebxList;
    }

    private static List<object> ReadRes(DataStream stream, Endian endian, uint count, List<Sha1> sha1, int offset, long stringsOffset)
    {
        List<object> resList = new List<object>();

        for (int i = 0; i < count; i++)
        {
            DbObject entry = new(new Dictionary<string, object>());

            uint nameOffset = stream.ReadUInt32(endian);
            uint originalSize = stream.ReadUInt32(endian);

            long currentPos = stream.Position;
            stream.Position = stringsOffset + nameOffset;

            entry.AddValue("sha1", sha1[offset++]);
            entry.AddValue("name", stream.ReadNullTerminatedString());
            entry.AddValue("nameHash", Utils.Utils.HashString(entry["name"].As<string>()));
            entry.AddValue("originalSize", originalSize);
            resList.Add(entry);

            stream.Position = currentPos;
        }

        foreach (DbObject res in resList)
        {
            res.AddValue("resType", stream.ReadUInt32(endian));
        }

        foreach (DbObject res in resList)
        {
            res.AddValue("resMeta", stream.ReadBytes(0x10));
        }

        foreach (DbObject res in resList)
        {
            res.AddValue("resRid", stream.ReadInt64(endian));
        }

        return resList;
    }

    private static List<object> ReadChunks(DataStream stream, Endian endian, uint count, List<Sha1> sha1, int offset)
    {
        List<object> chunkList = new List<object>();

        for (int i = 0; i < count; i++)
        {
            DbObject entry = new(new Dictionary<string, object>());

            Guid chunkId = stream.ReadGuid(endian);
            uint logicalOffset = stream.ReadUInt32(endian);
            uint logicalSize = stream.ReadUInt32(endian);
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
}