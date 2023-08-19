using System;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class PathFileInfo : IFileInfo
{
    private string m_path = string.Empty;
    private uint m_offset;
    private uint m_size;
    private uint m_logicalOffset;

    internal PathFileInfo()
    {
    }
    
    public PathFileInfo(string inPath, uint inOffset, uint inSize, uint inLogicalOffset)
    {
        m_path = inPath;
        m_offset = inOffset;
        m_size = inSize;
        m_logicalOffset = inLogicalOffset;
    }

    public bool IsComplete() => m_logicalOffset != 0;

    public Block<byte> GetRawData()
    {
        using (FileStream stream = new(FileSystemManager.ResolvePath(m_path), FileMode.Open, FileAccess.Read))
        {
            stream.Position = m_offset;

            Block<byte> retVal = new((int)m_size);
            
            stream.ReadExactly(retVal);
            return retVal;
        }
    }

    public Block<byte> GetData(int originalSize = 0)
    {
        using (BlockStream stream = BlockStream.FromFile(FileSystemManager.ResolvePath(m_path), m_offset, (int)m_size))
        {
            return Cas.DecompressData(stream, originalSize);
        }
    }
    
    void IFileInfo.SerializeInternal(DataStream stream)
    {
        stream.WriteNullTerminatedString(m_path);
        stream.WriteUInt32(m_offset);
        stream.WriteUInt32(m_size);
        stream.WriteUInt32(m_logicalOffset);
    }

    internal static PathFileInfo DeserializeInternal(DataStream stream)
    {
        return new PathFileInfo(stream.ReadNullTerminatedString(), stream.ReadUInt32(), stream.ReadUInt32(),
            stream.ReadUInt32());
    }

    public bool Equals(PathFileInfo b)
    {
        return m_path == b.m_path &&
               m_offset == b.m_offset &&
               m_size == b.m_size &&
               m_logicalOffset == b.m_logicalOffset;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is PathFileInfo b)
        {
            return Equals(b);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_path, m_offset, m_size, m_logicalOffset);
    }
}