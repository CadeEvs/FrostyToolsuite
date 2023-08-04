using System;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class CasFileInfo : IFileInfo
{
    public CasResourceInfo? GetBase() => m_base;
    
    private CasResourceInfo? m_base;
    private CasResourceInfo? m_delta;
    
    public CasFileInfo(CasResourceInfo? inBase, CasResourceInfo? inDelta = null)
    {
        m_base = inBase;
        m_delta = inDelta;
    }
    
    public CasFileInfo(CasFileIdentifier inCasFileIdentifier, uint inOffset, uint inSize, uint inLogicalOffset)
    {
        m_base = new CasResourceInfo(inCasFileIdentifier, inOffset, inSize, inLogicalOffset);
    }
    
    public CasFileInfo(CasFileIdentifier inCasFileIdentifier, uint inOffset, uint inSize, uint inLogicalOffset, string inKeyId)
    {
        m_base = new CasCryptoResourceInfo(inCasFileIdentifier, inOffset, inSize, inLogicalOffset, inKeyId);
    }

    public bool IsComplete() => true;
    public long GetSize() => m_base?.GetSize() ?? 0;
    
    public Block<byte> GetRawData()
    {
        throw new NotImplementedException();
    }

    public Block<byte> GetData(int inOriginalSize)
    {
        if (m_base is null)
        {
            throw new Exception("Base CasResourceInfo can't be null.");
        }
        
        if (m_delta is null)
        {
            return m_base.GetData(inOriginalSize);
        }

        using (BlockStream deltaStream = new(m_delta.GetRawData()))
        using (BlockStream baseStream = new(m_base.GetRawData()))
        {
            return Cas.DecompressData(deltaStream, baseStream, inOriginalSize);
        }
    }

    void IFileInfo.SerializeInternal(DataStream stream)
    {
        CasResourceInfo.Serialize(stream, m_base);
        CasResourceInfo.Serialize(stream, m_delta);
    }

    internal static CasFileInfo DeserializeInternal(DataStream stream)
    {
        return new CasFileInfo(CasResourceInfo.Deserialize(stream), CasResourceInfo.Deserialize(stream));
    }

    public bool Equals(CasFileInfo b)
    {
        return m_base == b.m_base &&
               m_delta == b.m_delta;
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
        return HashCode.Combine(m_base, m_delta);
    }
}