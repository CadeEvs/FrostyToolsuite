using System;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class CasFileInfo : IFileInfo
{
    protected CasFileIdentifier m_casFileIdentifier;
    protected uint m_offset;
    protected uint m_size;
    private uint m_logicalOffset;

    internal CasFileInfo()
    {
    }
    
    public CasFileInfo(CasFileIdentifier inCasFileIdentifier, uint inOffset, uint inSize, uint inLogicalOffset)
    {
        m_casFileIdentifier = inCasFileIdentifier;
        m_offset = inOffset;
        m_size = inSize;
        m_logicalOffset = inLogicalOffset;
    }
    
    public CasFileInfo(bool inIsPatch, int inInstallChunkIndex, int inCasIndex, uint inOffset, uint inSize, uint inLogicalOffset)
    {
        m_casFileIdentifier = new CasFileIdentifier(inIsPatch, inInstallChunkIndex, inCasIndex);
        m_offset = inOffset;
        m_size = inSize;
        m_logicalOffset = inLogicalOffset;
    }

    public bool IsComplete() => m_logicalOffset == 0;

    public long GetSize() => m_size;

    public virtual Block<byte> GetRawData()
    {
        using (FileStream stream = new(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(m_casFileIdentifier)), FileMode.Open, FileAccess.Read))
        {
            stream.Position = m_offset;

            Block<byte> retVal = new((int)m_size);
            
            stream.ReadExactly(retVal.ToSpan());
            return retVal;
        }
    }

    public virtual Block<byte> GetData(int originalSize)
    {
        using (BlockStream stream = BlockStream.FromFile(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(m_casFileIdentifier)), m_offset, (int)m_size))
        {
            return Cas.DecompressData(stream, originalSize);
        }
    }

    void IFileInfo.SerializeInternal(DataStream stream)
    {
        SerializeInternal(stream);
    }

    void IFileInfo.DeserializeInternal(DataStream stream)
    {
        DeserializeInternal(stream);
    }
    
    protected virtual void DeserializeInternal(DataStream stream)
    {
        m_casFileIdentifier = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32());
        m_offset = stream.ReadUInt32();
        m_size = stream.ReadUInt32();
        m_logicalOffset = stream.ReadUInt32();
    }
    
    protected virtual void SerializeInternal(DataStream stream)
    {
        stream.WriteUInt32(CasFileIdentifier.ToFileIdentifier(m_casFileIdentifier));
        stream.WriteUInt32(m_offset);
        stream.WriteUInt32(m_size);
        stream.WriteUInt32(m_logicalOffset);
    }

    public bool Equals(CasFileInfo b)
    {
        return m_casFileIdentifier == b.m_casFileIdentifier &&
               m_offset == b.m_offset &&
               m_size == b.m_size &&
               m_logicalOffset == b.m_logicalOffset;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is CasFileInfo b)
        {
            return Equals(b);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_casFileIdentifier, m_offset, m_size, m_logicalOffset);
    }
}