using System;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class PatchFileInfo : IFileInfo
{
    private IFileInfo m_delta;
    private IFileInfo m_base;

    internal PatchFileInfo()
    {
    }
    
    public PatchFileInfo(IFileInfo inDelta, IFileInfo inBase)
    {
        m_delta = inDelta;
        m_base = inBase;
    }

    public bool IsComplete() => m_base.IsComplete();

    public long GetSize()
    {
        throw new NotImplementedException();
    }

    public Block<byte> GetRawData()
    {
        using (BlockStream deltaStream = new(m_delta.GetRawData()))
        using (BlockStream baseStream = new(m_base.GetRawData()))
        {
            uint tmpVal = deltaStream.ReadUInt32(Endian.Big);
            int blockType = (int)(tmpVal & 0xF0000000) >> 28;

            switch (blockType)
            {
                case 0:
                    // read base block
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    throw new InvalidDataException("block type");
            }
        }
        throw new NotImplementedException();
    }

    public Block<byte> GetData(int originalSize = 0)
    {
        using (BlockStream deltaStream = new(m_delta.GetRawData()))
        using (BlockStream baseStream = new(m_base.GetRawData()))
        {
            return Cas.DecompressData(deltaStream, baseStream, originalSize);
        }
    }
    
    void IFileInfo.DeserializeInternal(DataStream stream)
    {
        m_delta = IFileInfo.Deserialize(stream);
        m_base = IFileInfo.Deserialize(stream);
    }

    void IFileInfo.SerializeInternal(DataStream stream)
    {
        IFileInfo.Serialize(stream, m_delta);
        IFileInfo.Serialize(stream, m_base);
    }
    
    public bool Equals(PatchFileInfo b)
    {
        return m_delta.Equals(b.m_delta) &&
               m_base.Equals(b.m_base);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is PatchFileInfo b)
        {
            return Equals(b);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_delta.GetHashCode(), m_base.GetHashCode());
    }
}