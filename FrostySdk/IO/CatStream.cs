using System;
using System.IO;
using Frosty.Sdk.Managers.CatResources;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.IO;

public class CatStream : DataStream
{
    public const string CatMagic = "NyanNyanNyanNyan";

    public uint ResourceCount { get; }
    public uint PatchCount { get; }
    public uint EncryptedCount { get; }

    public CatStream(Stream inStream)
        : base(inStream, true)
    {
        string magic = ReadFixedSizedString(16);
        if (magic != CatMagic)
            return;

        ResourceCount = (uint)(Length - Position) / 0x20;
        PatchCount = 0;
        EncryptedCount = 0;

        if (!ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals, ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeed))
        {
            ResourceCount = ReadUInt32();
            PatchCount = ReadUInt32();

            if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda, ProfileVersion.Fifa17, ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Fifa18, ProfileVersion.NeedForSpeedPayback, ProfileVersion.Madden19, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
            {
                EncryptedCount = ReadUInt32();
                Position += 12;

                if (EncryptedCount != 0 && ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.NeedForSpeedPayback, ProfileVersion.Madden19, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    EncryptedCount = 0;
                    throw new Exception("test");
                }
            }
        }
    }

    public CatResourceEntry ReadResourceEntry()
    {
        CatResourceEntry entry = new()
        {
            Sha1 = ReadSha1(),
            Offset = ReadUInt32(),
            Size = ReadUInt32()
        };

        if (!ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals, ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeed))
        {
            entry.LogicalOffset = ReadUInt32();
        }
        
        entry.ArchiveIndex = ReadInt32() & 0xFF;
        return entry;
    }

    public CatResourceEntry ReadEncryptedEntry()
    {
        CatResourceEntry entry = new()
        {
            Sha1 = ReadSha1(),
            Offset = ReadUInt32(),
            Size = ReadUInt32(),
            LogicalOffset = ReadUInt32(),
            ArchiveIndex = ReadInt32() & 0xFF,
            OriginalSize = ReadUInt32(),
            IsEncrypted = true,
            KeyId = ReadFixedSizedString(8),
            Checksum = ReadBytes(32)
        };

        entry.EncryptedSize = entry.Size;
        entry.Size = entry.Size + (16 - (entry.Size % 16));

        return entry;
    }

    public CatPatchEntry ReadPatchEntry()
    {
        CatPatchEntry entry = new()
        {
            Sha1 = ReadSha1(),
            BaseSha1 = ReadSha1(),
            DeltaSha1 = ReadSha1()
        };
        return entry;
    }
}