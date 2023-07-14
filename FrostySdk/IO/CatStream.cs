using System;
using Frosty.Sdk.Managers.CatResources;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.IO;

public class CatStream : IDisposable
{
    public uint ResourceCount { get; }
    public uint PatchCount { get; }
    public uint EncryptedCount { get; }
    
    private const string c_catMagic = "NyanNyanNyanNyan";

    private readonly DataStream m_stream;

    public CatStream(DataStream inStream)
    {
        m_stream = inStream;
        
        string magic = m_stream.ReadFixedSizedString(16);
        if (magic != c_catMagic)
        {
            return;
        }

        ResourceCount = (uint)(m_stream.Length - m_stream.Position) / 0x20;
        PatchCount = 0;
        EncryptedCount = 0;

        if (!ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals, ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeed))
        {
            ResourceCount = m_stream.ReadUInt32();
            PatchCount = m_stream.ReadUInt32();

            if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda, ProfileVersion.Fifa17, ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Fifa18, ProfileVersion.NeedForSpeedPayback, ProfileVersion.Madden19, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
            {
                EncryptedCount = m_stream.ReadUInt32();
                m_stream.Position += 12;

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
            Sha1 = m_stream.ReadSha1(),
            Offset = m_stream.ReadUInt32(),
            Size = m_stream.ReadUInt32()
        };

        if (!ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals, ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeed))
        {
            entry.LogicalOffset = m_stream.ReadUInt32();
        }
        
        entry.ArchiveIndex = m_stream.ReadInt32() & 0xFF;
        return entry;
    }

    public CatResourceEntry ReadEncryptedEntry()
    {
        CatResourceEntry entry = new()
        {
            Sha1 = m_stream.ReadSha1(),
            Offset = m_stream.ReadUInt32(),
            Size = m_stream.ReadUInt32(),
            LogicalOffset = m_stream.ReadUInt32(),
            ArchiveIndex = m_stream.ReadInt32() & 0xFF,
            OriginalSize = m_stream.ReadUInt32(),
            IsEncrypted = true,
            KeyId = m_stream.ReadFixedSizedString(8),
            Checksum = m_stream.ReadBytes(32)
        };

        return entry;
    }

    public CatPatchEntry ReadPatchEntry()
    {
        CatPatchEntry entry = new()
        {
            Sha1 = m_stream.ReadSha1(),
            BaseSha1 = m_stream.ReadSha1(),
            DeltaSha1 = m_stream.ReadSha1()
        };
        return entry;
    }

    public void Dispose()
    {
        m_stream.Dispose();
    }
}