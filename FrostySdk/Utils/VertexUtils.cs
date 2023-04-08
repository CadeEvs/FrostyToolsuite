using System.IO;
using Frosty.Sdk.IO;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.Utils;
public enum VertexElementUsage : byte
{
    Unknown = 0x00,
    Pos = 0x01,
    BoneIndices = 0x02,
    BoneIndices2 = 0x03,
    BoneWeights = 0x04,
    BoneWeights2 = 0x05,
    Normal = 0x06,
    Tangent = 0x07,
    Binormal = 0x08,
    BinormalSign = 0x09,
    WorldTrans1 = 0x0A,
    WorldTrans2 = 0x0B,
    WorldTrans3 = 0x0C,
    InstanceId = 0x0D,
    InstanceUserData0 = 0x0E,
    PrevInstanceUserData0 = 0x0F,
    InstanceUserData1 = 0x10,
    PrevWorldTrans1 = 0x11,
    PrevWorldTrans2 = 0x12,
    PrevWorldTrans3 = 0x13,
    XenonIndex = 0x14,
    XenonBarycentric = 0x15,
    XenonQuadID = 0x16,
    Index = 0x17,
    ViewIndex = 0x18,
    Color0 = 0x1E,
    Color1 = 0x1F,
    TexCoord0 = 0x21,
    TexCoord1 = 0x22,
    TexCoord2 = 0x23,
    TexCoord3 = 0x24,
    TexCoord4 = 0x25,
    TexCoord5 = 0x26,
    TexCoord6 = 0x27,
    TexCoord7 = 0x28,
    DisplacementMapTexCoord = 0x29,
    RadiosityTexCoord = 0x2A,
    VisInfo = 0x2B,
    SpriteSize = 0x2C,
    PackedTexCoord0 = 0x2D,
    PackedTexCoord1 = 0x2E,
    PackedTexCoord2 = 0x2F,
    PackedTexCoord3 = 0x30,
    ClipDistance0 = 0x31,
    ClipDistance1 = 0x32,
    SubMaterialIndex = 0x33,
    TangentSpace = 0x34,
    BranchInfo = 0x3C,
    PosAndScale = 0x3D,
    Rotation = 0x3E,
    SpriteSizeAndUv = 0x3F,
    FadePos = 0x5A,
    SpawnTime = 0x5B,
    RegionIds = 0x64,
    BlendWeights = 0x65,
    PosAndSoftMul = 0x96,
    Alpha = 0x97,
    Misc0 = 0x98,
    Misc1 = 0x99,
    Misc2 = 0x9A,
    Misc3 = 0x9B,
    LeftAndRotation = 0x9C,
    UpAndNormalBlend = 0x9D,
    Hl2BasisL0 = 0x9E,
    Hl2BasisL1 = 0x9F,
    Hl2BasisL3 = 0xA0,
    PosAndRejectCulling = 0xA1,
    Shadow = 0xA2,
    CustomParams = 0xA3,
    PatchUv = 0xB4,
    Height = 0xB5,
    MaskUVs0 = 0xB6,
    MaskUVs1 = 0xB7,
    MaskUVs2 = 0xB8,
    MaskUVs3 = 0xB9,
    UserMasks = 0xBA,
    HeightfieldUv = 0xBB,
    MaskUv = 0xBC,
    GlobalColorUv = 0xBD,
    HeightfieldPixelSizeAndAspect = 0xBE,
    WorldPositionXz = 0xBF,
    TerrainTextureNodeUv = 0xC0,
    ParentTerrainTextureNodeUv = 0xC1,
    DeformationIndex = 0xCD,
    DeformationWeight = 0xCE,
    DeformationPosition = 0xCF,
    Delta = 0xD0,
    ElementIndex = 0xD1,
    Uv01 = 0xD2,
    WorldPos = 0xD3,
    EyeVector = 0xD4,
    LightParams1 = 0xDC,
    LightParams2 = 0xDD,
    LightSubParams = 0xDE,
    LightSideVector = 0xDF,
    LightInnerAndOuterAngle = 0xE0,
    LightDir = 0xE1,
    LightMatrix1 = 0xE2,
    LightMatrix2 = 0xE3,
    LightMatrix3 = 0xE4,
    LightMatrix4 = 0xE5,
    Custom = 0xE6,
    DestructionMaskDistance = 0xF0,
    DestructionMaskTexCoord = 0xF1,
    VertIndex = 0xFA,
}

public enum VertexElementFormat : byte
{
    None = 0x00,
    Float = 0x01,
    Float2 = 0x02,
    Float3 = 0x03,
    Float4 = 0x04,
    Half = 0x05,
    Half2 = 0x06,
    Half3 = 0x07,
    Half4 = 0x08,
    UByteN = 0x32,
    Byte4 = 0x0A,
    Byte4N = 0x0B,
    UByte4 = 0x0C,
    UByte4N = 0x0D,
    Short = 0x0E,
    Short2 = 0x0F,
    Short3 = 0x10,
    Short4 = 0x11,
    ShortN = 0x12,
    Short2N = 0x13,
    Short3N = 0x14,
    Short4N = 0x15,
    UShort2 = 0x16,
    UShort4 = 0x17,
    UShort2N = 0x18,
    UShort4N = 0x19,
    Int = 0x1A,
    Int2 = 0x1B,
    Int3 = 0x33,
    Int4 = 0x1C,
    IntN = 0x1D,
    Int2N = 0x1E,
    Int4N = 0x1F,
    UInt = 0x20,
    UInt2 = 0x21,
    UInt3 = 0x34,
    UInt4 = 0x22,
    UIntN = 0x23,
    UInt2N = 0x24,
    UInt4N = 0x25,
    Comp3_10_10_10 = 0x26,
    Comp3N_10_10_10 = 0x27,
    UComp3_10_10_10 = 0x28,
    UComp3N_10_10_10 = 0x29,
    Comp3_11_11_10 = 0x2A,
    Comp3N_11_11_10 = 0x2B,
    UComp3_11_11_10 = 0x2C,
    UComp3N_11_11_10 = 0x2D,
    Comp4_10_10_10_2 = 0x2E,
    Comp4N_10_10_10_2 = 0x2F,
    UComp4_10_10_10_2 = 0x30,
    UComp4N_10_10_10_2 = 0x31,
}

public enum VertexElementClassification
{
    PerVertex = 0,
    PerInstance = 1,
}

public enum TangentSpaceCompressionType
{
    TangentSpaceCompression_Default,
    TangentSpaceCompression_Quaternion,
    TangentSpaceCompression_Matrix,
    TangentSpaceCompression_AxisAngle_Compact,
    TangentSpaceCompression_AxisAngle_Precise
}

public struct VertexElement
{
    public VertexElementUsage Usage;
    public VertexElementFormat Format;
    public byte Offset;
    public byte StreamIndex;
    public int Size
    {
        get
        {
            switch (Format)
            {
                case VertexElementFormat.None: return 0;
                case VertexElementFormat.Float: return 4;
                case VertexElementFormat.Float2: return 8;
                case VertexElementFormat.Float3: return 12;
                case VertexElementFormat.Float4: return 16;
                case VertexElementFormat.Half: return 2;
                case VertexElementFormat.Half2: return 4;
                case VertexElementFormat.Half3: return 6;
                case VertexElementFormat.Half4: return 8;
                case VertexElementFormat.UByteN: return 1;
                case VertexElementFormat.Byte4: return 4;
                case VertexElementFormat.Byte4N: return 4;
                case VertexElementFormat.UByte4: return 4;
                case VertexElementFormat.UByte4N: return 4;
                case VertexElementFormat.Short: return 2;
                case VertexElementFormat.Short2: return 4;
                case VertexElementFormat.Short3: return 6;
                case VertexElementFormat.Short4: return 8;
                case VertexElementFormat.ShortN: return 2;
                case VertexElementFormat.Short2N: return 4;
                case VertexElementFormat.Short3N: return 6;
                case VertexElementFormat.Short4N: return 8;
                case VertexElementFormat.UShort2: return 4;
                case VertexElementFormat.UShort4: return 8;
                case VertexElementFormat.UShort2N: return 4;
                case VertexElementFormat.UShort4N: return 8;
                case VertexElementFormat.Int: return 4;
                case VertexElementFormat.Int2: return 8;
                case VertexElementFormat.Int3: return 12;
                case VertexElementFormat.Int4: return 16;
                case VertexElementFormat.IntN: return 4;
                case VertexElementFormat.Int2N: return 8;
                case VertexElementFormat.Int4N: return 16;
                case VertexElementFormat.UInt: return 4;
                case VertexElementFormat.UInt2: return 8;
                case VertexElementFormat.UInt3: return 12;
                case VertexElementFormat.UInt4: return 16;
                case VertexElementFormat.UIntN: return 4;
                case VertexElementFormat.UInt2N: return 8;
                case VertexElementFormat.UInt4N: return 16;
                case VertexElementFormat.Comp3_10_10_10: return 4;
                case VertexElementFormat.Comp3N_10_10_10: return 4;
                case VertexElementFormat.UComp3_10_10_10: return 4;
                case VertexElementFormat.UComp3N_10_10_10: return 4;
                case VertexElementFormat.Comp3_11_11_10: return 4;
                case VertexElementFormat.Comp3N_11_11_10: return 4;
                case VertexElementFormat.UComp3_11_11_10: return 4;
                case VertexElementFormat.UComp3N_11_11_10: return 4;
                case VertexElementFormat.Comp4_10_10_10_2: return 4;
                case VertexElementFormat.Comp4N_10_10_10_2: return 4;
                case VertexElementFormat.UComp4_10_10_10_2: return 4;
                case VertexElementFormat.UComp4N_10_10_10_2: return 4;
                default: return 0;
            }
        }
    }

    public override string ToString()
    {
        return Usage.ToString();
    }
}

public struct GeometryDeclarationDesc
{
    public struct Element
    {
        public VertexElementUsage Usage;
        public VertexElementFormat Format;
        public byte Offset;
        public byte StreamIndex;
        public int Size
        {
            get
            {
                switch (Format)
                {
                    case VertexElementFormat.None: return 0;
                    case VertexElementFormat.Float: return 4;
                    case VertexElementFormat.Float2: return 8;
                    case VertexElementFormat.Float3: return 12;
                    case VertexElementFormat.Float4: return 16;
                    case VertexElementFormat.Half: return 2;
                    case VertexElementFormat.Half2: return 4;
                    case VertexElementFormat.Half3: return 6;
                    case VertexElementFormat.Half4: return 8;
                    case VertexElementFormat.UByteN: return 1;
                    case VertexElementFormat.Byte4: return 4;
                    case VertexElementFormat.Byte4N: return 4;
                    case VertexElementFormat.UByte4: return 4;
                    case VertexElementFormat.UByte4N: return 4;
                    case VertexElementFormat.Short: return 2;
                    case VertexElementFormat.Short2: return 4;
                    case VertexElementFormat.Short3: return 6;
                    case VertexElementFormat.Short4: return 8;
                    case VertexElementFormat.ShortN: return 2;
                    case VertexElementFormat.Short2N: return 4;
                    case VertexElementFormat.Short3N: return 6;
                    case VertexElementFormat.Short4N: return 8;
                    case VertexElementFormat.UShort2: return 4;
                    case VertexElementFormat.UShort4: return 8;
                    case VertexElementFormat.UShort2N: return 4;
                    case VertexElementFormat.UShort4N: return 8;
                    case VertexElementFormat.Int: return 4;
                    case VertexElementFormat.Int2: return 8;
                    case VertexElementFormat.Int3: return 12;
                    case VertexElementFormat.Int4: return 16;
                    case VertexElementFormat.IntN: return 4;
                    case VertexElementFormat.Int2N: return 8;
                    case VertexElementFormat.Int4N: return 16;
                    case VertexElementFormat.UInt: return 4;
                    case VertexElementFormat.UInt2: return 8;
                    case VertexElementFormat.UInt3: return 12;
                    case VertexElementFormat.UInt4: return 16;
                    case VertexElementFormat.UIntN: return 4;
                    case VertexElementFormat.UInt2N: return 8;
                    case VertexElementFormat.UInt4N: return 16;
                    case VertexElementFormat.Comp3_10_10_10: return 4;
                    case VertexElementFormat.Comp3N_10_10_10: return 4;
                    case VertexElementFormat.UComp3_10_10_10: return 4;
                    case VertexElementFormat.UComp3N_10_10_10: return 4;
                    case VertexElementFormat.Comp3_11_11_10: return 4;
                    case VertexElementFormat.Comp3N_11_11_10: return 4;
                    case VertexElementFormat.UComp3_11_11_10: return 4;
                    case VertexElementFormat.UComp3N_11_11_10: return 4;
                    case VertexElementFormat.Comp4_10_10_10_2: return 4;
                    case VertexElementFormat.Comp4N_10_10_10_2: return 4;
                    case VertexElementFormat.UComp4_10_10_10_2: return 4;
                    case VertexElementFormat.UComp4N_10_10_10_2: return 4;
                    default: return 0;
                }
            }
        }

        public override string ToString()
        {
            return Usage.ToString();
        }
    }
    public struct Stream
    {
        public byte VertexStride;
        public VertexElementClassification Classification;
    }

    public static int MaxElements => 16;
    public static int MaxStreams
    {
        get
        {
            switch (ProfilesLibrary.DataVersion)
            {
                case (int)ProfileVersion.NeedForSpeedRivals:
                case (int)ProfileVersion.PlantsVsZombiesGardenWarfare:
                case (int)ProfileVersion.DragonAgeInquisition:
                case (int)ProfileVersion.Battlefield4:
                case (int)ProfileVersion.PlantsVsZombiesGardenWarfare2:
                case (int)ProfileVersion.NeedForSpeed:
                case (int)ProfileVersion.NeedForSpeedEdge:
                    return 4;
                case (int)ProfileVersion.StarWarsBattlefront:
                    return 6;
                case (int)ProfileVersion.Battlefield5:
                    return 8;
                case (int)ProfileVersion.StarWarsBattlefrontII:
                case (int)ProfileVersion.Fifa18:
                case (int)ProfileVersion.Madden19:
                case (int)ProfileVersion.NeedForSpeedPayback:
                case (int)ProfileVersion.Fifa19:
                case (int)ProfileVersion.Anthem:
                case (int)ProfileVersion.Madden20:
                case (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville:
                case (int)ProfileVersion.NeedForSpeedHeat:
                case (int)ProfileVersion.Fifa20:
                case (int)ProfileVersion.StarWarsSquadrons:
                case (int)ProfileVersion.Fifa21:
                case (int)ProfileVersion.Madden22:
                case (int)ProfileVersion.Fifa22:
                case (int)ProfileVersion.Battlefield2042:
                case (int)ProfileVersion.Madden23:
                case (int)ProfileVersion.NeedForSpeedUnbound:
                    return 16;
                default:
                    return 8;
            }
        }
    }

    public Element[] Elements;
    public Stream[] Streams;
    public byte ElementCount;
    public byte StreamCount;

    public uint Hash => VertexUtils.CalcFletcher32(this);

    public static GeometryDeclarationDesc Create(Element[] elements)
    {
        GeometryDeclarationDesc geomDecl = new() {Elements = new Element[MaxElements]};

        int offset = 0;
        for (int i = 0; i < MaxElements; i++)
        {
            if (i < elements.Length)
            {
                geomDecl.Elements[i] = elements[i];
                geomDecl.Elements[i].Offset = (byte)offset;
                offset += geomDecl.Elements[i].Size;
            }
            else
            {
                geomDecl.Elements[i].Offset = 0xFF;
            }
        }

        geomDecl.Streams = new Stream[MaxStreams];
        geomDecl.Streams[0].Classification = VertexElementClassification.PerVertex;
        geomDecl.Streams[0].VertexStride = (byte)offset;

        geomDecl.ElementCount = (byte)elements.Length;
        geomDecl.StreamCount = 1;

        return geomDecl;
    }
}
public static class VertexUtils
{
    public static uint CalcFletcher32(GeometryDeclarationDesc geomDecl)
    {
        byte[] array = null;
        using (DataStream dataStream = new(new MemoryStream()))
        {
            foreach (GeometryDeclarationDesc.Element element in geomDecl.Elements)
            {
                dataStream.WriteByte((byte)element.Usage);
                dataStream.WriteByte((byte)element.Format);
                dataStream.WriteByte(element.Offset);
                dataStream.WriteByte(element.StreamIndex);
            }
            foreach (GeometryDeclarationDesc.Stream stream in geomDecl.Streams)
            {
                dataStream.WriteByte(stream.VertexStride);
                dataStream.WriteByte((byte)stream.Classification);
            }
            dataStream.WriteByte(geomDecl.ElementCount);
            dataStream.WriteByte(geomDecl.StreamCount);
            dataStream.WriteUInt16(0);

            array = dataStream.ToByteArray();
        }

        return CalcFletcher32Internal(array);
    }

    private static uint CalcFletcher32Internal(byte[] array)
    {
        int byteCount = array.Length;
        int a = byteCount >> 1;
        int b = 0; ;
        uint part1 = 0;
        uint part2 = 0;
        int idx = 0;

        if (a > 0)
        {
            int twoByteCount = 0;
            do
            {
                b = ((a - 0x168) & (a - 0x168 >> 0x1F)) + 0x168;
                twoByteCount = a - b;

                do
                {
                    ushort val = (ushort)((array[idx] << 8) | (array[idx + 1]));
                    idx += 2;

                    part1 += val;
                    part2 += part1;

                    b--;
                }
                while (b > 0);

                part1 = (ushort)((part1 >> 16) + part1);
                part2 = (ushort)(part2 + (part2 >> 16));
                a = twoByteCount;
            }
            while (twoByteCount > 0);
        }

        if ((byteCount & 1) != 0)
        {
            part1 += (ushort)(array[idx] << 8);
            part2 += part1;
        }

        return (((part1 & 0xFFFF0000) + (part1 << 16)) | ((ushort)part2 + (part2 >> 16)));
    }
}