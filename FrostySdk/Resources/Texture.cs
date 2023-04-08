using System;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Resources;

public enum TextureType
{
    TT_2d = 0x0,
    TT_Cube = 0x1,
    TT_3d = 0x2,
    TT_2dArray = 0x3,
    TT_1dArray = 0x4,
    TT_1d = 0x5,
    TT_CubeArray = 0x6,
};

[Flags]
public enum TextureFlags
{
    Streaming = 1 << 0,
    SrgbGamma = 1 << 1,
    CpuResource = 1 << 2,
    OnDemandLoaded = 1 << 3,
    Mutable = 1 << 4,
    NoSkipmip = 1 << 5,
    XenonPackedMipmaps = 1 << 8,
    Ps3MemoryCell = 1 << 8,
    Ps3MemoryRsx = 1 << 9,
    StreamingAlways = 1 << 10,
    SwizzledData = 1 << 11
}

public class Texture : Resource
{
    public uint SecondMipOffset
    {
        get => m_compressedMipOffsets[0];
        set => m_compressedMipOffsets[0] = value;
    }

    public uint ThirdMipOffset
    {
        get => m_compressedMipOffsets[1];
        set => m_compressedMipOffsets[1] = value;
    }
    public TextureType TextureType;
    public TextureFlags TextureFlags;
    public ushort Width;
    public ushort Height;
    public ushort Depth;
    public byte MipCount;
    public byte FirstMip;
    public Guid ChunkId;
    public uint[] MipSizes = new uint[15];
    public uint ChunkSize;
    public uint NameHash;
    public string TextureGroup = string.Empty;
    
    // not stored in the actual texture just calculated
    public uint LogicalOffset;
    public uint LogicalSize;
    public uint RangeStart;
    public uint RangeEnd;


    private int m_version;
    private uint[] m_compressedMipOffsets = new uint[2];
    private int m_pixelFormat;
    private uint m_customPoolId;
    private uint[] m_unknowns = new uint[4];

    public override void Deserialize(DataStream stream)
    {
        base.Deserialize(stream);

        m_version = BitConverter.ToInt32(m_resMeta, 0);

        if (m_version == 0)
        {
            m_version = stream.ReadInt32();
        }

        if (m_version > 10)
        {
            m_compressedMipOffsets[0] = stream.ReadUInt32();
            m_compressedMipOffsets[1] = stream.ReadUInt32();
        }

        TextureType = (TextureType)stream.ReadInt32();
        m_pixelFormat = stream.ReadInt32();

        if (m_version > 11)
        {
            m_customPoolId = stream.ReadUInt32();
        }

        TextureFlags = (TextureFlags)(m_version > 10 ? stream.ReadUInt16() : stream.ReadUInt32());

        Width = stream.ReadUInt16();
        Height = stream.ReadUInt16();

        Depth = stream.ReadUInt16();
        
        ushort depth2 = stream.ReadUInt16();

        if (m_version < 11)
        {
            stream.ReadUInt16();
        }

        MipCount = stream.ReadByte();
        FirstMip = stream.ReadByte();

        ChunkId = stream.ReadGuid();
        
        for (int i = 0; i < 15; i++)
        {
            MipSizes[i] = stream.ReadUInt32();
        }

        ChunkSize = stream.ReadUInt32();
        
        if (m_version >= 13)
        {
            for (int i = 0; i < 4; i++)
            {
                m_unknowns[i] = stream.ReadUInt32();
            }
        }

        NameHash = stream.ReadUInt32();

        TextureGroup = stream.ReadFixedSizedString(16);
    }
}