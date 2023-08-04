using System;
using System.IO;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class CasResourceInfo
{
    private readonly CasFileIdentifier m_casFileIdentifier;
    private readonly uint m_offset;
    private readonly uint m_size;
    private readonly uint m_logicalOffset;
    
    public CasResourceInfo(CasFileIdentifier inCasFileIdentifier, uint inOffset, uint inSize, uint inLogicalOffset)
    {
        m_casFileIdentifier = inCasFileIdentifier;
        m_offset = inOffset;
        m_size = inSize;
        m_logicalOffset = inLogicalOffset;
    }
    
    public CasResourceInfo(bool inIsPatch, int inInstallChunkIndex, int inCasIndex, uint inOffset, uint inSize, uint inLogicalOffset)
    {
        m_casFileIdentifier = new CasFileIdentifier(inIsPatch, inInstallChunkIndex, inCasIndex);
        m_offset = inOffset;
        m_size = inSize;
        m_logicalOffset = inLogicalOffset;
    }

    public string GetPath() => FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(m_casFileIdentifier));

    protected uint GetOffset() => m_offset;
    
    public uint GetSize() => m_size;
    
    public virtual Block<byte> GetRawData()
    {
        using (FileStream stream = new(GetPath(), FileMode.Open, FileAccess.Read))
        {
            stream.Position = m_offset;

            Block<byte> retVal = new((int)m_size);
            
            stream.ReadExactly(retVal);
            return retVal;
        }
    }

    public virtual Block<byte> GetData(int inOriginalSize)
    {
        using (BlockStream stream = BlockStream.FromFile(GetPath(), m_offset, (int)m_size))
        {
            return Cas.DecompressData(stream, inOriginalSize);
        }
    }

    public static bool operator ==(CasResourceInfo? a, CasResourceInfo? b)
    {
        return a?.m_casFileIdentifier == b?.m_casFileIdentifier &&
               a?.m_offset == b?.m_offset &&
               a?.m_size == b?.m_size &&
               a?.m_logicalOffset == b?.m_logicalOffset;
    }

    public static bool operator !=(CasResourceInfo? a, CasResourceInfo? b)
    {
        return a?.m_casFileIdentifier != b?.m_casFileIdentifier ||
               a?.m_offset != b?.m_offset ||
               a?.m_size != b?.m_size ||
               a?.m_logicalOffset != b?.m_logicalOffset;
    }

    public bool Equals(CasResourceInfo b)
    {
        return m_casFileIdentifier == b.m_casFileIdentifier &&
               m_offset == b.m_offset &&
               m_size == b.m_size &&
               m_logicalOffset == b.m_logicalOffset;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is CasResourceInfo b)
        {
            return Equals(b);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_casFileIdentifier, m_offset, m_size, m_logicalOffset);
    }

    public static void Serialize(DataStream stream, CasResourceInfo? info)
    {
        if (info is null)
        {
            stream.WriteByte(0);
            return;
        }

        if (info is CasCryptoResourceInfo crypto)
        {
            stream.WriteByte(2);
            CasCryptoResourceInfo.SerializeInternal(stream, crypto);
            return;
        }

        stream.WriteByte(1);
        SerializeInternal(stream, info);
    }
    
    public static CasResourceInfo? Deserialize(DataStream stream)
    {
        byte type = stream.ReadByte();

        switch (type)
        {
            case 0:
                return null;
            case 1:
                return DeserializeInternal(stream);
            case 2:
                return CasCryptoResourceInfo.DeserializeInternal(stream);
            default:
                throw new Exception("Not a valid CasResourceInfo type. Has to be 0, 1 or 2.");
        }
    }
    
    protected static void SerializeInternal(DataStream stream, CasResourceInfo info)
    {
        stream.WriteUInt32(CasFileIdentifier.ToFileIdentifier(info.m_casFileIdentifier));
        stream.WriteUInt32(info.m_offset);
        stream.WriteUInt32(info.m_size);
        stream.WriteUInt32(info.m_logicalOffset);
    }

    private static CasResourceInfo DeserializeInternal(DataStream stream)
    {
        CasFileIdentifier file = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32());
        uint offset = stream.ReadUInt32();
        uint size = stream.ReadUInt32();
        uint logicalOffset = stream.ReadUInt32();

        return new CasResourceInfo(file, offset, size, logicalOffset);
    }
}