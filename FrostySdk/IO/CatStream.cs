using System;
using Frosty.Sdk.Managers.CatResources;

namespace Frosty.Sdk.IO;

public class CatStream : IDisposable
{
    public uint ResourceCount { get; }
    public uint PatchCount { get; }
    public uint EncryptedCount { get; }
    
    private const string c_catMagic = "NyanNyanNyanNyan";

    private readonly DataStream m_stream;
    private readonly bool m_isNewFormat;

    public CatStream(string inFilename)
    {
        m_isNewFormat = ProfilesLibrary.FrostbiteVersion > "2014.4.11";
        m_stream = BlockStream.FromFile(inFilename, m_isNewFormat);
        
        string magic = m_stream.ReadFixedSizedString(16);
        if (magic != c_catMagic)
        {
            return;
        }

        ResourceCount = (uint)(m_stream.Length - m_stream.Position) / 0x20;
        PatchCount = 0;
        EncryptedCount = 0;

        // 2013.2, 2014.1, 2014.4.11
        if (m_isNewFormat)
        {
            ResourceCount = m_stream.ReadUInt32();
            PatchCount = m_stream.ReadUInt32();

            // 2015.4.6, 2019-PR5, 2016.4.7, 2016.4.4, 2018.0
            if (ProfilesLibrary.FrostbiteVersion >= "2015")
            {
                EncryptedCount = m_stream.ReadUInt32();
                m_stream.Position += 12;
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

        if (m_isNewFormat)
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