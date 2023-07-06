using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Loaders;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.IO;

public partial class BinaryBundle
{
    public enum Magic : uint
    {
        Standard = 0xED1CEDB8,
        Fifa = 0xC3889333,
        Encrypted = 0xC3E5D5C3
    }
 
    public EbxAssetEntry[] EbxList { get; }
    public ResAssetEntry[] ResList { get; }
    public ChunkAssetEntry[] ChunkList { get; }

    private BinaryBundle(DataStream stream)
    {
        Endian endian = Endian.Big;
        
        uint size = stream.ReadUInt32(Endian.Big);

        long startPos = stream.Position;
        
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

        bool containsSha1 = magic == Magic.Standard;

        uint totalCount = stream.ReadUInt32(endian);
        int ebxCount = stream.ReadInt32(endian);
        int resCount = stream.ReadInt32(endian);
        int chunkCount = stream.ReadInt32(endian);
        long stringsOffset = stream.ReadUInt32(endian) + (magic == Magic.Encrypted ? 0 : startPos);
        long metaOffset = stream.ReadUInt32(endian) + (magic == Magic.Encrypted ? 0 : startPos);
        uint metaSize = stream.ReadUInt32(endian);

        EbxList = new EbxAssetEntry[ebxCount];
        ResList = new ResAssetEntry[resCount];
        ChunkList = new ChunkAssetEntry[chunkCount];

        Sha1[] sha1 = new Sha1[totalCount];

        // decrypt the data
        if (magic == Magic.Encrypted)
        {
            if (stream is not BlockStream blockStream)
            {
                throw new Exception();
            }
            blockStream.Decrypt(KeyManager.GetKey("BundleEncryptionKey"), (int)(size - 0x20), PaddingMode.None);
        }
        
        // read sha1s
        for (int i = 0; i < totalCount; i++)
        {
            sha1[i] = containsSha1 ? stream.ReadSha1() : Sha1.Zero;
        }

        int j = 0;
        for (int i = 0; i < ebxCount; i++, j++)
        {
            uint nameOffset = stream.ReadUInt32(endian);
            uint originalSize = stream.ReadUInt32(endian);

            long currentPos = stream.Position;
            stream.Position = stringsOffset + nameOffset;
            string name = stream.ReadNullTerminatedString();

            EbxList[i] = new EbxAssetEntry(name, sha1[j], -1, originalSize);

            stream.Position = currentPos;
        }

        long resTypeOffset = stream.Position + resCount * 2 * sizeof(uint);
        long resMetaOffset = stream.Position + resCount * 2 * sizeof(uint) + resCount * sizeof(uint);
        long resRidOffset = stream.Position + resCount * 2 * sizeof(uint) + resCount * sizeof(uint) + resCount * 0x10;
        for (int i = 0; i < resCount; i++, j++)
        {
            uint nameOffset = stream.ReadUInt32(endian);
            uint originalSize = stream.ReadUInt32(endian);

            long currentPos = stream.Position;
            stream.Position = stringsOffset + nameOffset;
            string name = stream.ReadNullTerminatedString();

            stream.Position = resTypeOffset + i * sizeof(uint);
            uint resType = stream.ReadUInt32();
            
            stream.Position = resMetaOffset + i * 0x10;
            byte[] resMeta = stream.ReadBytes(0x10);
            
            stream.Position = resRidOffset + i * sizeof(ulong);
            ulong resRid = stream.ReadUInt64();

            ResList[i] = new ResAssetEntry(name, sha1[j], -1, originalSize, resRid, resType, resMeta);

            stream.Position = currentPos;
        }

        stream.Position = resRidOffset + resCount * sizeof(ulong);
        for (int i = 0; i < chunkCount; i++, j++)
        {
            ChunkList[i] = new ChunkAssetEntry(stream.ReadGuid(endian), sha1[j], -1, stream.ReadUInt32(endian),
                stream.ReadUInt32(endian));
        }

        // we need to set the correct position, since the string table comes after the meta
        stream.Position = startPos + size;
    }
    
    /// <summary>
    /// Dependent on the FB version, games have different salts.
    /// If the game uses a version newer than 2017 it uses "pecn", else it uses "pecm".
    /// <see cref="ProfileVersion.Battlefield5"/> is the only game that uses "arie".
    /// </summary>
    /// <returns>The salt, that the current game uses.</returns>
    public static uint GetSalt()
    {
        string salt = "pecm";

        if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
        {
            salt = "arie";
        }

        else if (ProfilesLibrary.DataVersion >= (int)ProfileVersion.Fifa19 && !ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsSquadrons))
        {
            salt = "pecn";
        }


        byte[] bytes = Encoding.ASCII.GetBytes(salt);
        return (uint)(bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] << 0);
    }

    /// <summary>
    /// Only the games using the <see cref="KelvinAssetLoader"/> use a different Magic than <see cref="Magic.Standard"/>.
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
    public static BinaryBundle Deserialize(DataStream stream)
    {
        return new BinaryBundle(stream);
    }
}