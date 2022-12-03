using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FrostySdk;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;

namespace MeshSetPlugin.Resources
{
    internal class MeshContainer
    {
        public class RelocPtr
        {
            public string Type;
            public long Offset;
            public object Data;
            public long DataOffset;

            public RelocPtr(string type, object data)
            {
                Type = type;
                Data = data;
            }
        }

        public class RelocArray : RelocPtr
        {
            public int Count;

            public RelocArray(string type, int count, object arrayData)
                : base(type, arrayData)
            {
                Count = count;
            }
        }

        private List<RelocPtr> relocPtrs = new List<RelocPtr>();
        private Dictionary<RelocPtr, string> strings = new Dictionary<RelocPtr, string>();

        public MeshContainer()
        {
        }

        public void AddString(object obj, string data, bool ignoreNull = false)
        {
            RelocPtr ptr = new RelocPtr("STR", obj);

            relocPtrs.Add(ptr);
            strings.Add(ptr, data + ((ignoreNull) ? "" : "\0"));
        }

        public void AddRelocPtr(string type, object obj)
        {
            relocPtrs.Add(new RelocPtr(type, obj));
        }

        public void WriteRelocPtr(string type, object obj, NativeWriter writer)
        {
            RelocPtr ptr = FindRelocPtr(type, obj);
            ptr.Offset = writer.Position;
            writer.Write(0xdeadbeefdeadbeef);
        }

        public void AddRelocArray(string type, int count, object arrayObj)
        {
            relocPtrs.Add(new RelocArray(type, count, arrayObj));
        }

        public void WriteRelocArray(string type, object arrayObj, NativeWriter writer)
        {
            RelocArray array = FindRelocPtr(type, arrayObj) as RelocArray;
            array.Offset = writer.Position + 4;
            writer.Write(array.Count);
            writer.Write(0xdeadbeefdeadbeef);
        }

        public void AddOffset(string type, object data, NativeWriter writer)
        {
            RelocPtr ptr = FindRelocPtr(type, data);
            if (ptr != null)
            {
                ptr.DataOffset = writer.Position;
            }
        }

        public void WriteStrings(NativeWriter writer)
        {
            foreach (var ptr in strings.Keys)
            {
                ptr.DataOffset = writer.Position;
                writer.WriteFixedSizedString(strings[ptr], strings[ptr].Length);
            }
        }

        public void FixupRelocPtrs(NativeWriter writer)
        {
            // fixup pointers
            foreach (RelocPtr ptr in relocPtrs)
            {
                writer.Position = ptr.Offset;
                writer.Write(ptr.DataOffset);
            }
        }

        public void WriteRelocTable(NativeWriter writer)
        {
            // relocation table
            writer.Position = writer.Length;
            foreach (RelocPtr ptr in relocPtrs)
            {
                writer.Write((uint)ptr.Offset);
            }
        }

        private RelocPtr FindRelocPtr(string type, object obj)
        {
            foreach (var ptr in relocPtrs)
            {
                if (ptr.Type == type && ptr.Data.Equals(obj))
                {
                    return ptr;
                }
            }
            return null;
        }
    }

    #region -- Helpers --
    internal static class ReaderExtensions
    {
        public static Vec3 ReadVec3(this NativeReader reader)
        {
            Vec3 vec = new Vec3
            {
                x = reader.ReadFloat(),
                y = reader.ReadFloat(),
                z = reader.ReadFloat(),
                pad = reader.ReadFloat() // padding
            };

            return vec;
        }
        public static Vec2 ReadVec2(this NativeReader reader)
        {
            Vec2 vec = new Vec2
            {
                x = reader.ReadFloat(),
                y = reader.ReadFloat()
            };
            return vec;
        }
        public static AxisAlignedBox2 ReadAxisAlignedBox2(this NativeReader reader)
        {
            AxisAlignedBox2 aab = new AxisAlignedBox2
            {
                min = reader.ReadVec2(),
                max = reader.ReadVec2()
            };
            return aab;
        }
        public static AxisAlignedBox ReadAxisAlignedBox(this NativeReader reader)
        {
            AxisAlignedBox aab = new AxisAlignedBox
            {
                min = reader.ReadVec3(),
                max = reader.ReadVec3()
            };
            return aab;
        }
        public static LinearTransform ReadLinearTransform(this NativeReader reader)
        {
            LinearTransform lt = new LinearTransform
            {
                right = reader.ReadVec3(),
                up = reader.ReadVec3(),
                forward = reader.ReadVec3(),
                trans = reader.ReadVec3()
            };
            return lt;
        }
    }
    internal static class WriterExtensions
    {
        public static void Write(this NativeWriter writer, Vec3 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
            writer.Write(vec.pad); // padding
        }
        public static void Write(this NativeWriter writer, AxisAlignedBox aab)
        {
            writer.Write(aab.min);
            writer.Write(aab.max);
        }
        public static void Write(this NativeWriter writer, LinearTransform lt)
        {
            writer.Write(lt.right);
            writer.Write(lt.up);
            writer.Write(lt.forward);
            writer.Write(lt.trans);
        }
    }
    #endregion

    #region -- Enumerations --
    public enum MeshType
    {
        MeshType_Rigid = 0,
        MeshType_Skinned = 1,
        MeshType_Composite = 2,
    }

    public enum IndexBufferFormat
    {
        IndexBufferFormat_16Bit = 0x0,
        IndexBufferFormat_32Bit = 0x1,
    }

    public enum PrimitiveType
    {
        PrimitiveType_PointList = 0x0,
        PrimitiveType_LineList = 0x1,
        PrimitiveType_LineStrip = 0x2,
        PrimitiveType_TriangleList = 0x3,
        PrimitiveType_TriangleStrip = 0x5,
        PrimitiveType_QuadList = 0x7,
        PrimitiveType_XenonRectList = 0x8,
        PrimitiveType_TrianglePatch = 0x9,
        PrimitiveTypeCount = 0xA,
    };

    [Flags]
    public enum MeshLayoutFlags : uint
    {
        IsBaseLod = 1 << 0,
        StreamInstancingEnable = 1 << 4,
        StreamingEnable = 1 << 6,
        VertexAnimationEnable = 1 << 7,
        Deformation = 1 << 8,
        MultiStreamEnable = 1 << 9,
        SubsetSortingEnable = 1 << 10,
        Inline = 1 << 11,
        AlternateBatchSorting = 1 << 12,
        ProjectedDecalsEnable = 1 << 15,
        ClothEnabled = 1 << 16,
        SrvEnable = 1 << 17,
        IsMeshFront = 1 << 28,
        IsDataAvailable = 1 << 29
    }

    [Flags]
    public enum MeshSetLayoutFlags : ulong
    {
        StreamingEnable = 1ul << 0,
        HalfResRenderEnable = 1ul << 1,
        StreamInstancingEnable = 1ul << 2,
        MovableParts = 1ul << 3,
        DrawProcessEnable = 1ul << 4,
        StreamingEnableAlways = 1ul << 5,
        DeformationEnable = 1ul << 6,
        UseLastLodForShadow = 1ul << 8,

        CastShadowLowEnable = 1ul << 9,
        CastShadowMediumEnable = 1ul << 10,
        CastShadowHighEnable = 1ul << 11,
        CastShadowUltraEnable = 1ul << 12,

        CastDynamicReflectionLowEnable = 1ul << 13,
        CastDynamicReflectionMediumEnable = 1ul << 14,
        CastDynamicReflectionHighEnable = 1ul << 15,
        CastDynamicReflectionUltraEnable = 1ul << 16,

        CastPlanarReflectionLowEnable = 1ul << 17,
        CastPlanarReflectionMediumEnable = 1ul << 18,
        CastPlanarReflectionHighEnable = 1ul << 19,
        CastPlanarReflectionUltraEnable = 1ul << 20,

        CastStaticReflectionLowEnable = 1ul << 21,
        CastStaticReflectionMediumEnable = 1ul << 22,
        CastStaticReflectionHighEnable = 1ul << 23,
        CastStaticReflectionUltraEnable = 1ul << 24,

        SubsetSortingEnable = 1ul << 26,
        LodFadeEnable = 1ul << 27,
        ProjectedDecalsEnable = 1ul << 28,
        ClothEnable = 1ul << 29,
        ZPassEnable = 1ul << 30,

        CastDistantShadowCache = 1ul << 31,

        // FIFA extra
        CastPlanarShadowLowEnable = 1ul << 32,
        CastPlanarShadowMediumEnable = 1ul << 33,
        CastPlanarShadowHighEnable = 1ul << 34,
        CastPlanarShadowUltraEnable = 1ul << 35,

        CastShadowInBakedLowEnable = 1ul << 36,
        CastShadowInBakedMediumEnable = 1ul << 37,
        CastShadowInBakedHighEnable = 1ul << 38,
        CastShadowInBakedUltraEnable = 1ul << 39,

        ForwardDepthPassEnable = 1ul << 40
    }

    public enum MeshSubsetCategory
    {
        MeshSubsetCategory_Opaque = 0x0,
        MeshSubsetCategory_Transparent = 0x1,
        MeshSubsetCategory_TransparentDecal = 0x2,
        MeshSubsetCategory_ZOnly = 0x3,
        MeshSubsetCategory_Shadow = 0x4,
        MeshSubsetCategoryCount = 0x5,
    }

    [Flags]
    public enum MeshSubsetCategoryFlags : ushort
    {
        MeshSubsetCategoryFlags_Opaque = 1 << 0,
        MeshSubsetCategoryFlags_Transparent = 1 << 1,
        MeshSubsetCategoryFlags_TransparentDecal = 1 << 2,
        MeshSubsetCategoryFlags_ZOnly = 1 << 3,
        MeshSubsetCategoryFlags_Shadow = 1 << 4,
        MeshSubsetCategoryFlags_DynamicReflection = 1 << 5, // DAI
        MeshSubsetCategoryFlags_PlanarReflection = 1 << 6,  // DAI
        MeshSubsetCategoryFlags_StaticReflection = 1 << 7,
        MeshSubsetCategoryFlags_ShadowOverride = 1 << 8,
        MeshSubsetCategoryFlags_DynamicReflectionOverride = 1 << 9,
        MeshSubsetCategoryFlags_PlanarReflectionOverride = 1 << 10,
        MeshSubsetCategoryFlags_StaticReflectionOverride = 1 << 11,
        MeshSubsetCategoryFlags_SunShadow = 1 << 12,
        MeshSubsetCategoryFlags_SunShadowOverride = 1 << 13,
        MeshSubsetCategoryFlags_LocalShadow = 1 << 14,
        MeshSubsetCategoryFlags_LocalShadowOverride = 1 << 15,

        MeshSubsetCategoryFlags_Normal = 0x7,
        MeshSubsetCategoryFlags_AllShadow = 0x5010,
        MeshSubsetCategoryFlags_All = 0xFFFF,
    }
    #endregion

    #region -- Structures --
    public struct Vec3
    {
        public float x, y, z;
        internal float pad;

        public static bool operator ==(Vec3 left, Vec3 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !left.Equals(right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Vec3 v3 = (Vec3)obj;
            return v3.x == x && v3.y == y && v3.z == z && v3.pad == pad;
        }
    }
    public struct Vec2
    {
        public float x, y;

        public static bool operator ==(Vec2 left, Vec2 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vec2 left, Vec2 right)
        {
            return !left.Equals(right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Vec2 v2 = (Vec2)obj;
            return v2.x == x && v2.y == y;
        }
    }
    public struct AxisAlignedBox2
    {
        public Vec2 min;
        public Vec2 max;

        public static bool operator ==(AxisAlignedBox2 left, AxisAlignedBox2 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(AxisAlignedBox2 left, AxisAlignedBox2 right)
        {
            return !left.Equals(right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            AxisAlignedBox2 aab2 = (AxisAlignedBox2)obj;
            return aab2.min == min && aab2.max == max;
        }
    }
    public struct AxisAlignedBox
    {
        public Vec3 min, max;

        public static bool operator ==(AxisAlignedBox left, AxisAlignedBox right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(AxisAlignedBox left, AxisAlignedBox right)
        {
            return !left.Equals(right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            AxisAlignedBox aab = (AxisAlignedBox)obj;
            return aab.min == min && aab.max == max;
        }
    }
    public struct LinearTransform
    {
        public Vec3 right, up, forward, trans;

        public static bool operator ==(LinearTransform left, LinearTransform right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(LinearTransform left, LinearTransform right)
        {
            return !left.Equals(right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            LinearTransform lt = (LinearTransform)obj;
            return lt.right == right && lt.up == up && lt.forward == forward && lt.trans == trans;
        }
    }
    #endregion

    #region -- Mesh --

    #region -- Section --
    public class MeshSetSection
    {
        public TangentSpaceCompressionType TangentSpaceCompressionType;
        public string Name => m_materialName;
        public int MaterialId => m_materialId;
        public uint LightMapUvMappingIndex => m_lightMapUvMappingIndex;
        public uint PrimitiveCount { get => m_primitiveCount; set => m_primitiveCount = value; }
        public uint StartIndex { get => m_startIndex; set => m_startIndex = value; }
        public uint VertexOffset { get => m_vertexOffset; set => m_vertexOffset = value; }
        public uint VertexCount { get => m_vertexCount; set => m_vertexCount = value; }
        public GeometryDeclarationDesc[] GeometryDeclDesc => m_geometryDeclarationDesc;
        public List<ushort> BoneList => m_boneList;
        public uint VertexStride => m_vertexStride;
        public PrimitiveType PrimitiveType => m_primitiveType;
        public byte BonesPerVertex
        {
            get => m_bonesPerVertex;
            set
            {
                m_bonesPerVertex = value;
                if (m_bonesPerVertex > 8)
                {
                    m_bonesPerVertex = 8;
                }
            }
        }
        public int DeclCount
        {
            get
            {
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.MirrorsEdgeCatalyst:
                    case (int)ProfileVersion.Battlefield1:
                    case (int)ProfileVersion.StarWarsBattlefrontII:
                    case (int)ProfileVersion.Battlefield5:
                    case (int)ProfileVersion.StarWarsSquadrons:
                        return 2;
                    default: return 1;
                }
            }
        }
        public bool HasUnknown => m_hasUnknown;
        public bool HasUnknown2 => m_hasUnknown2;
        public bool HasUnknown3 => m_hasUnknown3;

        private long m_offset1;
        private long m_offset2;
        private string m_materialName;
        private int m_materialId;
        private uint m_lightMapUvMappingIndex;
        private uint m_primitiveCount;
        private uint m_startIndex;
        private uint m_vertexOffset;
        private uint m_vertexCount;
        private GeometryDeclarationDesc[] m_geometryDeclarationDesc = new GeometryDeclarationDesc[2];
        private byte m_vertexStride;
        private PrimitiveType m_primitiveType;
        private byte m_bonesPerVertex;
        private ushort m_unk1;
        private uint m_unk2;

        private bool m_hasUnknown;
        private bool m_hasUnknown2;
        private bool m_hasUnknown3;

        private List<ushort> m_boneList = new List<ushort>();
        private List<float> m_texCoordRatios = new List<float>();
        private byte[] m_unknownData = null;
        private AxisAlignedBox m_boundingBox;
        private int m_sectionIndex;

        internal MeshSetSection()
        {
        }

        public MeshSetSection(NativeReader reader, AssetManager am, int index)
        {
            m_sectionIndex = index;
            m_offset1 = reader.ReadLong(); // runtime ptr, so always 0

            if (ProfilesLibrary.IsLoaded(ProfileVersion.MirrorsEdgeCatalyst, ProfileVersion.Battlefield1,
                ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                ProfileVersion.StarWarsSquadrons))
            {
                m_offset2 = reader.ReadLong();
            }

            long stringOffset = reader.ReadLong(); // materialName

            long boneListOffset = 0;
            uint boneCount = 0;

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Fifa22, ProfileVersion.Battlefield2042,
                ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
            {
                boneListOffset = reader.ReadLong();
                boneCount = reader.ReadUShort();

                m_bonesPerVertex = reader.ReadByte();
                m_unk1 = reader.ReadByte();

                m_materialId = reader.ReadUShort();

                m_vertexStride = reader.ReadByte();
                m_primitiveType = (PrimitiveType)reader.ReadByte();

                m_primitiveCount = reader.ReadUInt();
                m_startIndex = reader.ReadUInt();
                m_vertexOffset = reader.ReadUInt();
                m_vertexCount = reader.ReadUInt();

                m_unk2 = reader.ReadUInt();

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound))
                {
                    reader.ReadLong();
                }

                // texcoord ratios
                for (int i = 0; i < 6; i++)
                {
                    m_texCoordRatios.Add(reader.ReadFloat());
                }
            }
            else
            {
                m_materialId = reader.ReadInt();
                if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4,
                    ProfileVersion.PlantsVsZombiesGardenWarfare, ProfileVersion.NeedForSpeedRivals,
                    ProfileVersion.NeedForSpeedEdge))
                {
                    m_lightMapUvMappingIndex = reader.ReadUInt();
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    // texcoord ratios
                    for (int i = 0; i < 6; i++)
                    {
                        m_texCoordRatios.Add(reader.ReadFloat());
                    }
                }

                m_primitiveCount = reader.ReadUInt();
                m_startIndex = reader.ReadUInt();
                m_vertexOffset = reader.ReadUInt();
                m_vertexCount = reader.ReadUInt();

                // Fifa 21 stores boundingBox not at the end
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    m_boundingBox = reader.ReadAxisAlignedBox();
                }

                m_vertexStride = reader.ReadByte();
                m_primitiveType = (PrimitiveType)reader.ReadByte();

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    m_unk1 = reader.ReadUShort();
                }

                m_bonesPerVertex = reader.ReadByte();
                boneCount = reader.ReadByte();

                // Fifa 21 stores boneCount in a USHORT
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    boneCount = reader.ReadUShort();
                }

                // Fifa 17/18 store boneCount in a UINT
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                    ProfileVersion.Madden19, ProfileVersion.Fifa19,
                    ProfileVersion.Anthem, ProfileVersion.Madden20,
                    ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                    ProfileVersion.NeedForSpeedHeat))
                {
                    boneCount = reader.ReadUInt();
                }

                // MEC/BF1/SWBF2/SWS
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.MirrorsEdgeCatalyst, ProfileVersion.Battlefield1,
                    ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                    ProfileVersion.StarWarsSquadrons))
                {
                    reader.ReadUInt();
                    reader.ReadUInt();
                    reader.ReadUInt();

                    if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                        ProfileVersion.StarWarsSquadrons))
                    {
                        m_bonesPerVertex = reader.ReadByte();
                        reader.ReadByte();
                        boneCount = reader.ReadUShort();
                    }
                    else
                    {
                        m_bonesPerVertex = reader.ReadByte();
                        boneCount = reader.ReadUShort();
                        reader.ReadByte();
                    }
                }

                // boneIndices
                boneListOffset = reader.ReadLong();

                // Fifa18/SWBF2/NFS Payback/Anthem
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.NeedForSpeedPayback,
                    ProfileVersion.Fifa18, ProfileVersion.Madden19,
                    ProfileVersion.Fifa19, ProfileVersion.Anthem,
                    ProfileVersion.Madden20, ProfileVersion.Fifa20,
                    ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.NeedForSpeedHeat,
                    ProfileVersion.StarWarsSquadrons))
                {
                    reader.ReadULong();
                }

                // Fifa 17/18
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                    ProfileVersion.Madden19))
                {
                    long unknownOffset = reader.ReadLong();
                    if (unknownOffset != 0)
                    {
                        m_hasUnknown = true;
                    }

                    unknownOffset = reader.ReadLong();
                    if (unknownOffset != 0)
                    {
                        m_hasUnknown2 = true;
                    }

                    unknownOffset = reader.ReadLong();
                    if (unknownOffset != 0)
                    {
                        m_hasUnknown3 = true;
                    }
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    reader.ReadLong(); // some hash
                }
            }

            // geometry declarations
            for (int geomDeclId = 0; geomDeclId < DeclCount; geomDeclId++)
            {
                m_geometryDeclarationDesc[geomDeclId].Elements = new GeometryDeclarationDesc.Element[GeometryDeclarationDesc.MaxElements];
                m_geometryDeclarationDesc[geomDeclId].Streams = new GeometryDeclarationDesc.Stream[GeometryDeclarationDesc.MaxStreams];

                for (int i = 0; i < GeometryDeclarationDesc.MaxElements; i++)
                {
                    GeometryDeclarationDesc.Element elem = new GeometryDeclarationDesc.Element
                    {
                        Usage = (VertexElementUsage)reader.ReadByte(),
                        Format = (VertexElementFormat)reader.ReadByte(),
                        Offset = reader.ReadByte(),
                        StreamIndex = reader.ReadByte()
                    };

                    m_geometryDeclarationDesc[geomDeclId].Elements[i] = elem;
                }
                for (int i = 0; i < GeometryDeclarationDesc.MaxStreams; i++)
                {
                    GeometryDeclarationDesc.Stream stream = new GeometryDeclarationDesc.Stream
                    {
                        VertexStride = reader.ReadByte(),
                        Classification = (VertexElementClassification)reader.ReadByte()
                    };

                    m_geometryDeclarationDesc[geomDeclId].Streams[i] = stream;
                }

                m_geometryDeclarationDesc[geomDeclId].ElementCount = reader.ReadByte();
                m_geometryDeclarationDesc[geomDeclId].StreamCount = reader.ReadByte();
                reader.ReadBytes(2); // padding
            }
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
            {
                reader.Pad(16);
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Fifa22, ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
            {
                reader.ReadLong(); // some hash

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa22))
                {
                    reader.ReadLong(); // some other hash
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
                {
                    reader.ReadUInt(); // some other hash
                }

                reader.Pad(16);
                m_boundingBox = reader.ReadAxisAlignedBox();
            }
            else
            {
                // texcoord ratios
                for (int i = 0; i < 6; i++)
                {
                    m_texCoordRatios.Add(reader.ReadFloat());
                }

                // unknown data block
                int count = 0;
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.MassEffectAndromeda:
                        count = 48;
                        break;
                    case (int)ProfileVersion.NeedForSpeed:
                    case (int)ProfileVersion.PlantsVsZombiesGardenWarfare2:
                    case (int)ProfileVersion.NeedForSpeedPayback:
                        count = 40;
                        break;
                    case (int)ProfileVersion.StarWarsBattlefront:
                    case (int)ProfileVersion.MirrorsEdgeCatalyst:
                    case (int)ProfileVersion.Fifa17:
                    case (int)ProfileVersion.Battlefield1:
                    case (int)ProfileVersion.Fifa19:
                    case (int)ProfileVersion.Anthem:
                    case (int)ProfileVersion.Battlefield5:
                    case (int)ProfileVersion.Madden20:
                    case (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville:
                    case (int)ProfileVersion.NeedForSpeedHeat:
                    case (int)ProfileVersion.Fifa20:
                        count = 36;
                        break;
                    default:
                        count = 44;
                        break;
                }
                m_unknownData = reader.ReadBytes(count);
            }

            // section data bone list
            long curPos = reader.Position;
            reader.Position = boneListOffset;
            for (int k = 0; k < boneCount; k++)
            {
                BoneList.Add(reader.ReadUShort());
            }

            // strings
            reader.Position = stringOffset;
            m_materialName = reader.ReadNullTerminatedString();
            reader.Position = curPos;
        }

        public void SetBones(IEnumerable<ushort> bones)
        {
            m_boneList.Clear();
            foreach (ushort boneId in bones)
            {
                //if ((boneId & 0x8000) == 0)
                m_boneList.Add(boneId);
            }
        }

        //public void SetVertexElements(List<GeometryDeclarationDesc.Element> inVertexElements)
        //{
        //    for (int declId = 0; declId < DeclCount; declId++)
        //    {
        //        vertexStride = 0;
        //        geometryDeclarationDesc[declId].Elements = new GeometryDeclarationDesc.Element[GeometryDeclarationDesc.MaxElements];

        //        for (int elemId = 0; elemId < GeometryDeclarationDesc.MaxElements; elemId++)
        //        {
        //            geometryDeclarationDesc[declId].Elements[elemId].Offset = 0xFF;
        //            if (elemId < inVertexElements.Count)
        //            {
        //                GeometryDeclarationDesc.Element elem = inVertexElements[elemId];
        //                elem.Offset = (byte)vertexStride;
        //                geometryDeclarationDesc[declId].Elements[elemId] = elem;
        //                geometryDeclarationDesc[declId].ElementCount++;
        //                vertexStride += (byte)elem.Size;
        //            }
        //        }

        //        geometryDeclarationDesc[declId].Streams = new GeometryDeclarationDesc.Stream[GeometryDeclarationDesc.MaxStreams];
        //        geometryDeclarationDesc[declId].Streams[0].VertexStride = vertexStride;
        //        geometryDeclarationDesc[declId].StreamCount = 1;

        //        if(declId == 1)
        //            geometryDeclarationDesc[declId].StreamCount = 2;
        //    }
        //}

        internal void PreProcess(MeshContainer meshContainer)
        {
            meshContainer.AddString(m_sectionIndex + ":" + m_materialName, m_materialName);
            if (m_boneList.Count > 0)
            {
                meshContainer.AddRelocPtr("BONELIST", m_boneList);
            }
        }

        internal void Process(NativeWriter writer, MeshContainer meshContainer)
        {
            writer.Write(m_offset1);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.MirrorsEdgeCatalyst, ProfileVersion.Battlefield1,
                            ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                            ProfileVersion.StarWarsSquadrons))
            {
                writer.Write(m_offset2);
            }

            meshContainer.WriteRelocPtr("STR", m_sectionIndex + ":" + m_materialName, writer);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Fifa22, ProfileVersion.Battlefield2042,
                ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
            {
                if (m_boneList.Count > 0)
                {
                    meshContainer.WriteRelocPtr("BONELIST", m_boneList, writer);
                }
                else
                {
                    writer.Write((ulong)0);
                }

                writer.Write(m_bonesPerVertex);
                writer.Write((byte)m_unk1);

                writer.Write(m_materialId);

                writer.Write(m_vertexStride);
                writer.Write((byte)m_primitiveType);

                writer.Write(m_primitiveCount);
                writer.Write(m_startIndex);
                writer.Write(m_vertexOffset);
                writer.Write(m_vertexCount);

                writer.Write(m_unk2);

                // texcoord ratios
                for (int i = 0; i < 6; i++)
                {
                    writer.Write(m_texCoordRatios[i]);
                }
            }
            else
            {
                writer.Write(m_materialId);
                if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4,
                    ProfileVersion.PlantsVsZombiesGardenWarfare, ProfileVersion.NeedForSpeedRivals, ProfileVersion.NeedForSpeedEdge))
                {
                    writer.Write(m_lightMapUvMappingIndex);
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    // texcoord ratios
                    for (int i = 0; i < 6; i++)
                    {
                        writer.Write(m_texCoordRatios[i]);
                    }
                }

                writer.Write(m_primitiveCount);
                writer.Write(m_startIndex);
                writer.Write(m_vertexOffset);
                writer.Write(m_vertexCount);

                // Fifa 21 stores boundingBox not at the end
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    writer.Write(m_boundingBox);
                }

                writer.Write(m_vertexStride);
                writer.Write((byte)m_primitiveType);

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    writer.Write(m_unk1);
                }

                // Fifa 21 stores boneCount in a USHORT
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    writer.Write(m_bonesPerVertex);
                    writer.Write((byte)0x00);
                    writer.Write((ushort)m_boneList.Count);
                }

                // Fifa17/Fifa18/Madden19/Fifa19/Anthem/Madden20/Fifa20 store boneCount in a UINT
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                        ProfileVersion.Madden19, ProfileVersion.Fifa19,
                        ProfileVersion.Anthem, ProfileVersion.Madden20,
                        ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                        ProfileVersion.NeedForSpeedHeat))
                {
                    writer.Write(m_bonesPerVertex);
                    writer.Write((byte)0x00);
                    writer.Write(m_boneList.Count);
                }

                // MEC/BF1/SWBF2/BFV store boneCount as a ushort
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.MirrorsEdgeCatalyst, ProfileVersion.Battlefield1,
                                    ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                                    ProfileVersion.StarWarsSquadrons))
                {
                    writer.Write((ushort)0x00); // padding

                    writer.Write((uint)0x00);
                    writer.Write((uint)0x00);
                    writer.Write((uint)0x00);

                    if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                                            ProfileVersion.StarWarsSquadrons))
                    {
                        writer.Write(m_bonesPerVertex);
                        writer.Write((byte)0x00);
                        writer.Write((ushort)m_boneList.Count);
                    }
                    else
                    {
                        writer.Write(m_bonesPerVertex);
                        writer.Write((ushort)m_boneList.Count);
                        writer.Write((byte)0x00);
                    }
                }
                else
                {
                    writer.Write(m_bonesPerVertex);
                    writer.Write((byte)m_boneList.Count);
                }

                if (m_boneList.Count > 0)
                {
                    meshContainer.WriteRelocPtr("BONELIST", m_boneList, writer);
                }
                else
                {
                    writer.Write((ulong)0);
                }

                // Fifa18/SWBF2/NFS Payback/Anthem
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.NeedForSpeedPayback,
                        ProfileVersion.Fifa18, ProfileVersion.Madden19,
                        ProfileVersion.Fifa19, ProfileVersion.Anthem,
                        ProfileVersion.Madden20, ProfileVersion.Fifa20,
                        ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.NeedForSpeedHeat,
                        ProfileVersion.StarWarsSquadrons, ProfileVersion.Fifa21))
                {
                    // unknown
                    writer.Write((ulong)0);
                }

                // Fifa 17/18
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                                    ProfileVersion.Madden19))
                {
                    // @todo
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
                {
                    // @todo
                }
            }

            // geometry declarations
            for (int declId = 0; declId < DeclCount; declId++)
            {
                for (int elemId = 0; elemId < m_geometryDeclarationDesc[declId].Elements.Length; elemId++)
                {
                    writer.Write((byte)m_geometryDeclarationDesc[declId].Elements[elemId].Usage);
                    writer.Write((byte)m_geometryDeclarationDesc[declId].Elements[elemId].Format);
                    writer.Write(m_geometryDeclarationDesc[declId].Elements[elemId].Offset);
                    writer.Write(m_geometryDeclarationDesc[declId].Elements[elemId].StreamIndex);
                }
                for (int streamId = 0; streamId < m_geometryDeclarationDesc[declId].Streams.Length; streamId++)
                {
                    writer.Write(m_geometryDeclarationDesc[declId].Streams[streamId].VertexStride);
                    writer.Write((byte)m_geometryDeclarationDesc[declId].Streams[streamId].Classification);
                }
                writer.Write(m_geometryDeclarationDesc[declId].ElementCount);
                writer.Write(m_geometryDeclarationDesc[declId].StreamCount);
                writer.Write((ushort)0); // padding
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21))
            {
                writer.WritePadding(16);
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Fifa22, ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
            {
                // @todo
                // some hash

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa22))
                {
                    // some other hash
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
                {
                    // some other hash
                }

                writer.WritePadding(16);
                writer.Write(m_boundingBox);
            }
            else
            {
                // texcoord ratios
                for (int i = 0; i < 6; i++)
                {
                    writer.Write(m_texCoordRatios[i]);
                }

                // unknown data
                writer.Write(m_unknownData);
            }

        }
    }
    #endregion

    #region -- Lod --
    public class MeshSetLod
    {
        [StructLayout(LayoutKind.Explicit)]
        struct IndexBufferFormatStruct
        {
            [FieldOffset(0)] public int format;
            [FieldOffset(0)] public IndexBufferFormat formatEnum;
        }

        public MeshType Type { get => m_meshType; set => m_meshType = value; }
        public List<MeshSetSection> Sections => m_sections;
        public MeshLayoutFlags Flags => m_flags;
        public int IndexUnitSize
        {
            get
            {
                if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4,
                    ProfileVersion.PlantsVsZombiesGardenWarfare, ProfileVersion.NeedForSpeedRivals))
                {
                    return (m_indexBufferFormat.formatEnum == IndexBufferFormat.IndexBufferFormat_16Bit) ? 16 : 32;
                }

                int value = (int)Enum.Parse(TypeLibrary.GetType("RenderFormat"), "RenderFormat_R32_UINT");
                return (m_indexBufferFormat.format == value) ? 32 : 16;
            }
        }
        public uint IndexBufferSize { get => m_indexBufferSize; set => m_indexBufferSize = value; }
        public uint VertexBufferSize { get => m_vertexBufferSize; set => m_vertexBufferSize = value; }
        public int AdjacencyBufferSize => m_adjacencyBufferSize;
        public Guid ChunkId { get => m_chunkId; set => m_chunkId = value; }
        public string FullName
        {
            get { return m_shaderDebugName; }
            set
            {
                m_shaderDebugName = value + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_name = value + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_shortName = value.Substring(value.LastIndexOf("/") + 1) + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_nameHash = (uint)Frosty.Hash.Fnv1.HashString(m_name);
            }
        }
        public string Name
        {
            get { return m_name; }
            set
            {
                m_shaderDebugName = "Mesh:" + value + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_name = value + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_shortName = value.Substring(value.LastIndexOf("/") + 1) + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_nameHash = (uint)Frosty.Hash.Fnv1.HashString(m_name);
            }
        }
        public string ShortName
        {
            get { return m_shortName; }
            set
            {
                m_shortName = value + m_shortName.Substring(m_shortName.LastIndexOf("_"));
                m_name = m_name.Substring(0, m_name.LastIndexOf("/") + 1) + m_shortName;
                m_shaderDebugName = "Mesh:" + m_name;
                m_nameHash = (uint)Frosty.Hash.Fnv1.HashString(m_name);
            }
        }
        public int BoneCount => m_boneIndexArray.Count;
        public List<uint> BoneIndexArray => m_boneIndexArray;
        public List<uint> BoneShortNameArray => m_boneShortNameArray;
        public int PartCount => m_partBoundingBoxes.Count;
        public List<AxisAlignedBox> PartBoundingBoxes => m_partBoundingBoxes;
        public List<LinearTransform> PartTransforms => m_partTransforms;
        public List<List<int>> PartIndices => m_partIndices;
        public byte[] InlineData => m_inlineData;
        public List<List<byte>> CategorySubsetIndices => m_subsetCategories;
        public int MaxCategories
        {
            get
            {
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.NeedForSpeedRivals:
                    case (int)ProfileVersion.DragonAgeInquisition:
                    case (int)ProfileVersion.Battlefield4:
                    case (int)ProfileVersion.PlantsVsZombiesGardenWarfare:
                        return 4;
                    default:
                        return 5;
                }
            }
        }
        public bool HasAdjacencyInMesh
        {
            get
            {
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.NeedForSpeedRivals:
                    case (int)ProfileVersion.DragonAgeInquisition:
                    case (int)ProfileVersion.Battlefield4:
                    case (int)ProfileVersion.PlantsVsZombiesGardenWarfare:
                    case (int)ProfileVersion.MirrorsEdgeCatalyst:
                    case (int)ProfileVersion.Battlefield1:
                    case (int)ProfileVersion.StarWarsBattlefrontII:
                    case (int)ProfileVersion.Battlefield5:
                    case (int)ProfileVersion.StarWarsSquadrons:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private MeshType m_meshType;
        private uint m_maxInstances;
        private uint m_unknownUInt;
        private MeshLayoutFlags m_flags;
        private IndexBufferFormatStruct m_indexBufferFormat;
        private uint m_indexBufferSize;
        private uint m_vertexBufferSize;
        private int m_adjacencyBufferSize;
        private Guid m_chunkId;
        private string m_shaderDebugName;
        private string m_name;
        private string m_shortName;
        private uint m_nameHash;
        private List<MeshSetSection> m_sections = new List<MeshSetSection>();
        private List<List<byte>> m_subsetCategories = new List<List<byte>>();

        //private uint boneCount;
        private List<uint> m_boneIndexArray = new List<uint>();
        private List<uint> m_boneShortNameArray = new List<uint>();

        private List<AxisAlignedBox> m_partBoundingBoxes = new List<AxisAlignedBox>();
        private List<LinearTransform> m_partTransforms = new List<LinearTransform>();
        private List<List<int>> m_partIndices = new List<List<int>>();

        private byte[] m_inlineData;
        private uint m_inlineDataOffset;
        private byte[] m_adjacencyData;
        private bool m_hasBoneShortNames;

        public MeshSetLod(NativeReader reader, AssetManager am, ref int sectionIndex)
        {
            m_meshType = (MeshType)reader.ReadUInt();
            m_maxInstances = reader.ReadUInt();

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem))
            {
                m_unknownUInt = reader.ReadUInt();
            }

            uint sectionCount = reader.ReadUInt();
            long sectionOffset = reader.ReadLong();

            // sections
            long curPos = reader.Position;
            reader.Position = sectionOffset;
            for (int i = 0; i < sectionCount; i++)
            {
                m_sections.Add(new MeshSetSection(reader, am, sectionIndex++));
            }

            reader.Position = curPos;

            // section categories
            for (int i = 0; i < MaxCategories; i++)
            {
                int count = reader.ReadInt();
                long subsetCategoryOffset = reader.ReadLong();
                m_subsetCategories.Add(new List<byte>());
                curPos = reader.Position;
                reader.Position = subsetCategoryOffset;
                for (int z = 0; z < count; z++)
                {
                    m_subsetCategories[i].Add(reader.ReadByte());
                }

                reader.Position = curPos;
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Madden19))
            {
                m_unknownUInt = reader.ReadUInt();
            }

            m_flags = (MeshLayoutFlags)reader.ReadUInt();

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.PlantsVsZombiesGardenWarfare,
                ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeedRivals))
            {
                m_indexBufferFormat.format = reader.ReadInt();
            }
            else
            {
                m_indexBufferFormat.formatEnum = (IndexBufferFormat)reader.ReadInt();
            }

            m_indexBufferSize = reader.ReadUInt();
            m_vertexBufferSize = reader.ReadUInt();

            if (HasAdjacencyInMesh)
            {
                m_adjacencyBufferSize = reader.ReadInt();
                m_adjacencyData = new byte[m_adjacencyBufferSize];
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound))
            {
                reader.ReadLong();
            }

            m_chunkId = reader.ReadGuid();
            m_inlineDataOffset = reader.ReadUInt();

            long adjacencyBufferOffset = 0;
            if (HasAdjacencyInMesh)
            {
                adjacencyBufferOffset = reader.ReadLong();
            }

            long stringOffset01 = reader.ReadLong();
            long stringOffset02 = reader.ReadLong();
            long stringOffset03 = reader.ReadLong();

            m_nameHash = reader.ReadUInt();
            reader.ReadLong();

            // Bones/Parts
            uint bonePartCount = 0;
            long bonePartOffset01 = 0;
            long bonePartOffset02 = 0;
            long bonePartOffset03 = 0;

            // Fifa 17/18/SWBF2/NFS Payback/SWS/MEA/Anthem
            if (MeshSet.HasNewPartBoneLayout)
            {
                if (m_meshType == MeshType.MeshType_Skinned)
                {
                    bonePartCount = reader.ReadUInt();
                    bonePartOffset01 = reader.ReadLong();
                }
                else if (m_meshType == MeshType.MeshType_Composite)
                {
                    bonePartOffset03 = reader.ReadLong();
                }
            }
            else
            {
                // All others
                bonePartCount = reader.ReadUInt();
                if (m_meshType >= MeshType.MeshType_Skinned)
                {
                    bonePartOffset01 = reader.ReadLong();
                    bonePartOffset02 = reader.ReadLong();

                    if (m_meshType == MeshType.MeshType_Composite)
                    {
                        bonePartOffset03 = reader.ReadLong();
                    }
                }
            }

            reader.Pad(16);

            // bone data
            curPos = reader.Position;
            if (m_meshType == MeshType.MeshType_Skinned)
            {
                reader.Position = bonePartOffset01;
                for (int i = 0; i < bonePartCount; i++)
                {
                    m_boneIndexArray.Add(reader.ReadUInt());
                }

                if (bonePartOffset02 != 0)
                {
                    reader.Position = bonePartOffset02;
                    for (int i = 0; i < bonePartCount; i++)
                    {
                        m_boneShortNameArray.Add(reader.ReadUInt());
                    }
                }
            }

            // part data
            else if (m_meshType == MeshType.MeshType_Composite)
            {
                if (bonePartOffset01 != 0)
                {
                    reader.Position = bonePartOffset01;
                    for (int i = 0; i < bonePartCount; i++)
                    {
                        m_partBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                    }
                }
                if (bonePartOffset02 != 0)
                {
                    reader.Position = bonePartOffset02;
                    for (int i = 0; i < bonePartCount; i++)
                    {
                        m_partTransforms.Add(reader.ReadLinearTransform());
                    }
                }
                if (bonePartOffset03 != 0)
                {
                    reader.Position = bonePartOffset03;

                    for (int s = 0; s < sectionCount; s++)
                    {
                        List<int> sectionPartIndices = new List<int>();
                        for (int i = 0; i < 0x18; i++)
                        {
                            int b = reader.ReadByte();
                            for (int j = 0; j < 8; j++)
                            {
                                if ((b & 0x01) != 0)
                                {
                                    sectionPartIndices.Add((i * 8) + j);
                                }
                                b >>= 1;
                            }
                        }
                        m_partIndices.Add(sectionPartIndices);
                    }
                }
            }

            // adjacency data
            reader.Position = adjacencyBufferOffset;
            m_adjacencyData = reader.ReadBytes(m_adjacencyBufferSize);

            // strings
            reader.Position = stringOffset01;
            m_shaderDebugName = reader.ReadNullTerminatedString();
            reader.Position = stringOffset02;
            m_name = reader.ReadNullTerminatedString();
            reader.Position = stringOffset03;
            m_shortName = reader.ReadNullTerminatedString();
            reader.Position = curPos;

            m_hasBoneShortNames = m_boneShortNameArray.Count > 0;
        }

        public void SetIndexBufferFormatSize(int newSize)
        {
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.PlantsVsZombiesGardenWarfare,
                                        ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeedRivals))
            {
                m_indexBufferFormat.format = (int)((newSize == 2)
                    ? Enum.Parse(TypeLibrary.GetType("RenderFormat"), "RenderFormat_R16_UINT")
                    : Enum.Parse(TypeLibrary.GetType("RenderFormat"), "RenderFormat_R32_UINT")
                    );
            }
            else
            {
                m_indexBufferFormat.formatEnum = (newSize == 2)
                    ? IndexBufferFormat.IndexBufferFormat_16Bit
                    : IndexBufferFormat.IndexBufferFormat_32Bit;
            }
        }

        public bool IsSectionInCategory(MeshSetSection section, MeshSubsetCategory category)
        {
            int index = GetSectionIndex(section);
            if ((int)category >= m_subsetCategories.Count)
            {
                return false;
            }

            return m_subsetCategories[(int)category].Contains((byte)index);
        }

        public bool IsSectionRenderable(MeshSetSection section)
        {
            return section.PrimitiveCount > 0 && (
                IsSectionInCategory(section, MeshSubsetCategory.MeshSubsetCategory_Opaque) ||
                IsSectionInCategory(section, MeshSubsetCategory.MeshSubsetCategory_Transparent) ||
                IsSectionInCategory(section, MeshSubsetCategory.MeshSubsetCategory_TransparentDecal)
                );
        }

        public void SetSectionCategory(MeshSetSection inSection, MeshSubsetCategory category)
        {
            byte index = (byte)GetSectionIndex(inSection);
            if (index == 0xFF)
            {
                return;
            }

            if (!m_subsetCategories[(int)category].Contains(index))
            {
                m_subsetCategories[(int)category].Add(index);
            }
        }

        public void SetParts(List<LinearTransform> inPartTransforms, List<AxisAlignedBox> inPartBBoxes)
        {
            m_partTransforms = inPartTransforms;
            m_partBoundingBoxes = inPartBBoxes;

            if (m_partTransforms.Count != m_partBoundingBoxes.Count)
            {
                for (int i = 0; i < m_partBoundingBoxes.Count; i++)
                {
                    if (i >= m_partTransforms.Count)
                    {
                        m_partTransforms.Add(new LinearTransform()
                        {
                            right = new Vec3() { x = 1.0f, y = 0.0f, z = 0.0f },
                            up = new Vec3() { x = 0.0f, y = 1.0f, z = 0.0f },
                            forward = new Vec3() { x = 0.0f, y = 0.0f, z = 1.0f },
                            trans = new Vec3() { x = 0.0f, y = 0.0f, z = 0.0f }
                        });
                    }
                }
            }
        }

        public void ClearBones()
        {
            m_boneIndexArray.Clear();
            m_boneShortNameArray.Clear();
        }

        public void AddBones(IEnumerable<ushort> bones, IEnumerable<string> boneNames)
        {
            foreach (ushort boneId in bones)
            {
                if (!m_boneIndexArray.Contains(boneId))
                {
                    m_boneIndexArray.Add(boneId);
                }
            }
            if (m_hasBoneShortNames)
            {
                foreach (string boneName in boneNames)
                {
                    uint hash = (uint)Frosty.Hash.Fnv1.HashString(boneName.ToLower());
                    if (!m_boneShortNameArray.Contains(hash))
                    {
                        m_boneShortNameArray.Add(hash);
                    }
                }
            }
        }

        public MeshSubsetCategoryFlags GetSectionCategories(int index)
        {
            MeshSubsetCategoryFlags flags = 0;
            for (int i = 0; i < m_subsetCategories.Count; i++)
            {
                if (m_subsetCategories[i].Contains((byte)index))
                {
                    flags |= (MeshSubsetCategoryFlags)(1 << i);
                }
            }
            return flags;
        }

        public void ClearCategories()
        {
            for (int i = 0; i < m_subsetCategories.Count; i++)
            {
                m_subsetCategories[i].Clear();
            }
        }

        public void ReadInlineData(NativeReader reader)
        {
            if (m_chunkId == Guid.Empty)
            {
                m_inlineData = reader.ReadBytes((int)(m_vertexBufferSize + m_indexBufferSize));
                while (reader.Position % 16 != 0)
                {
                    reader.Position++;
                }
            }
        }

        public void SetInlineData(byte[] inBuffer)
        {
            m_inlineData = inBuffer;
            m_chunkId = Guid.Empty;
#if FROSTY_DEVELOPER
            Debug.Assert(m_flags.HasFlag(MeshLayoutFlags.Inline) || m_flags.HasFlag(MeshLayoutFlags.ClothEnabled));
#endif
        }

        private int GetSectionIndex(MeshSetSection inSection)
        {
            int index = -1;
            for (int i = 0; i < m_sections.Count; i++)
            {
                if (m_sections[i] == inSection)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        internal void PreProcess(MeshContainer meshContainer, ref uint inInlineDataOffset)
        {
            m_inlineDataOffset = 0xFFFFFFFF;
            if (m_inlineData != null)
            {
                m_inlineDataOffset = inInlineDataOffset;
                inInlineDataOffset += (uint)m_inlineData.Length;
            }

            meshContainer.AddRelocArray("SECTION", m_sections.Count, m_sections);
            foreach (var section in m_sections)
            {
                section.PreProcess(meshContainer);
            }
            foreach (var category in m_subsetCategories)
            {
                meshContainer.AddRelocArray("SUBSET", category.Count, category);
            }
            if (HasAdjacencyInMesh && m_inlineDataOffset != 0xFFFFFFFF)
            {
                meshContainer.AddRelocPtr("ADJACENCY", m_adjacencyData);
            }
            meshContainer.AddString(m_shaderDebugName, "Mesh:", true);
            meshContainer.AddString(m_name, m_name.Replace(m_shortName, ""), true);
            meshContainer.AddString(m_shortName, m_shortName);

            if (m_meshType == MeshType.MeshType_Skinned)
            {
                if (m_boneIndexArray.Count != 0)
                {
                    meshContainer.AddRelocPtr("BONES", m_boneIndexArray);
                }

                if (m_boneShortNameArray.Count != 0 && !MeshSet.HasNewPartBoneLayout)
                {
                    meshContainer.AddRelocPtr("BONESNAMES", m_boneShortNameArray);
                }
            }
            else if (m_meshType == MeshType.MeshType_Composite)
            {
                if (!MeshSet.HasNewPartBoneLayout)
                {
                    if (m_partBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddRelocPtr("PARTBBOX", m_partBoundingBoxes);
                    }

                    if (m_partTransforms.Count != 0)
                    {
                        meshContainer.AddRelocPtr("PARTTRANSFORM", m_partTransforms);
                    }
                }
                if (m_partIndices.Count != 0)
                {
                    meshContainer.AddRelocPtr("PARTINDICES", m_partIndices);
                }
            }
        }

        internal void Process(NativeWriter writer, MeshContainer meshContainer)
        {
            writer.Write((int)m_meshType);
            writer.Write(m_maxInstances);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem))
            {
                writer.Write(m_unknownUInt);
            }

            meshContainer.WriteRelocArray("SECTION", m_sections, writer);
            foreach (var category in m_subsetCategories)
            {
                meshContainer.WriteRelocArray("SUBSET", category, writer);
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Madden19))
            {
                writer.Write(m_unknownUInt);
            }

            writer.Write((int)m_flags);

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.PlantsVsZombiesGardenWarfare,
                            ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeedRivals))
            {
                writer.Write(m_indexBufferFormat.format);
            }
            else
            {
                writer.Write((int)m_indexBufferFormat.formatEnum);
            }

            writer.Write(m_indexBufferSize);
            writer.Write(m_vertexBufferSize);

            if (HasAdjacencyInMesh)
            {
                writer.Write(m_adjacencyData.Length);
            }

            writer.Write(m_chunkId);
            writer.Write(m_inlineDataOffset);

            if (HasAdjacencyInMesh)
            {
                if (m_inlineDataOffset != 0xFFFFFFFF)
                {
                    meshContainer.WriteRelocPtr("ADJACENCY", m_adjacencyData, writer);
                }
                else
                {
                    writer.Write((long)0);
                }
            }
            meshContainer.WriteRelocPtr("STR", m_shaderDebugName, writer);
            meshContainer.WriteRelocPtr("STR", m_name, writer);
            meshContainer.WriteRelocPtr("STR", m_shortName, writer);

            writer.Write(m_nameHash);
            writer.Write((long)0); // unknown

            if (m_meshType == MeshType.MeshType_Skinned)
            {
                writer.Write(BoneCount);

                if (m_boneIndexArray.Count != 0)
                {
                    meshContainer.WriteRelocPtr("BONES", m_boneIndexArray, writer);
                }
                if (m_boneShortNameArray.Count != 0 && !MeshSet.HasNewPartBoneLayout)
                {
                    meshContainer.WriteRelocPtr("BONESNAMES", m_boneShortNameArray, writer);
                }
            }
            else if (m_meshType == MeshType.MeshType_Composite)
            {
                if (!MeshSet.HasNewPartBoneLayout)
                {
                    if (m_partBoundingBoxes.Count != 0)
                    {
                        meshContainer.WriteRelocPtr("PARTBBOX", m_partBoundingBoxes, writer);
                    }
                    if (m_partTransforms.Count != 0)
                    {
                        meshContainer.WriteRelocPtr("PARTTRANSFORM", m_partTransforms, writer);
                    }
                }
                if (m_partIndices.Count != 0)
                {
                    meshContainer.WriteRelocPtr("PARTINDICES", m_partIndices, writer);
                }
            }
            writer.WritePadding(16);
        }
    }
    #endregion

    #region -- MeshSet --
    public class MeshSet : Resource
    {
        public TangentSpaceCompressionType TangentSpaceCompressionType
        {
            get => m_tangentSpaceCompressionType;
            set
            {
                m_tangentSpaceCompressionType = value;
                foreach (MeshSetLod lod in m_lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                    {
                        section.TangentSpaceCompressionType = value;
                    }
                }
            }
        }
        public AxisAlignedBox BoundingBox => m_boundingBox;
        public List<MeshSetLod> Lods => m_lods;
        public MeshType Type { get => m_meshType; set => m_meshType = value; }
        public MeshSetLayoutFlags Flags => m_flags;
        public string FullName
        {
            get => m_fullname;
            set
            {
                m_fullname = value.ToLower();
                m_nameHash = (uint)Frosty.Hash.Fnv1.HashString(m_fullname);

                int id = m_fullname.LastIndexOf('/');
                m_name = (id != -1) ? m_fullname.Substring(id + 1) : "";
            }
        }
        public string Name
        {
            get => m_name;
            set
            {
                m_name = value.ToLower();


                int id = m_fullname.LastIndexOf('/');
                m_fullname = ((id != -1) ? m_fullname.Substring(0, id + 1) : "") + m_name;

                m_nameHash = (uint)Frosty.Hash.Fnv1.HashString(m_fullname);
            }
        }
        public uint NameHash { get => m_nameHash; set => m_nameHash = value; }

        public int HeaderSize => BitConverter.ToUInt16(resMeta, 0x0c);

        public const int MaxLodCount = 6;

        public static bool HasNewPartBoneLayout
        {
            get
            {
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.Anthem:
                    case (int)ProfileVersion.Fifa17:
                    case (int)ProfileVersion.Fifa18:
                    case (int)ProfileVersion.Fifa19:
                    case (int)ProfileVersion.Fifa20:
                    case (int)ProfileVersion.Fifa21:
                    case (int)ProfileVersion.Fifa22:
                    case (int)ProfileVersion.Madden19:
                    case (int)ProfileVersion.Madden20:
                    case (int)ProfileVersion.Madden21:
                    case (int)ProfileVersion.Madden22:
                    case (int)ProfileVersion.Madden23:
                    case (int)ProfileVersion.MassEffectAndromeda:
                    case (int)ProfileVersion.NeedForSpeedHeat:
                    case (int)ProfileVersion.NeedForSpeedPayback:
                    case (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville:
                    case (int)ProfileVersion.StarWarsBattlefrontII:
                    case (int)ProfileVersion.StarWarsSquadrons:
                    case (int)ProfileVersion.Battlefield2042:
                    case (int)ProfileVersion.NeedForSpeedUnbound:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private TangentSpaceCompressionType m_tangentSpaceCompressionType;
        private AxisAlignedBox m_boundingBox;
        private string m_fullname;
        private string m_name;
        private uint m_nameHash;
        private MeshType m_meshType;
        private MeshSetLayoutFlags m_flags;
        private ushort[] m_lodFadeDistanceFactors = new ushort[MaxLodCount * 2];
        private short m_shaderDrawOrder;
        private short m_shaderDrawOrderUserSlot;
        private short m_shaderDrawOrderSubOrder;
        private List<MeshSetLod> m_lods = new List<MeshSetLod>();

        private ushort m_unknownUShort;

        private ushort m_bonePartCount;
        private ushort m_boneCount;
        private List<ushort> m_boneIndices = new List<ushort>();
        private List<AxisAlignedBox> m_boneBoundingBoxes = new List<AxisAlignedBox>();
        private List<AxisAlignedBox> m_partBoundingBoxes = new List<AxisAlignedBox>();
        private List<LinearTransform> m_partTransforms = new List<LinearTransform>();

        private byte[] m_unknownbfv;

        public MeshSet()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            m_boundingBox = reader.ReadAxisAlignedBox();

            List<long> lodOffsets = new List<long>();
            for (int i = 0; i < MaxLodCount; i++)
            {
                lodOffsets.Add(reader.ReadLong());
            }

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals, ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4, ProfileVersion.PlantsVsZombiesGardenWarfare))
            {
                reader.ReadLong();
            }

            long fullnameOffset = reader.ReadLong();
            long nameOffset = reader.ReadLong();

            m_nameHash = reader.ReadUInt();
            m_meshType = (MeshType)reader.ReadUInt();

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound))
            {
                m_meshType = (MeshType)((uint)m_meshType & 0xFF);
                // unk
                reader.Position += 12;
            }

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.MassEffectAndromeda,
                ProfileVersion.NeedForSpeedEdge, ProfileVersion.NeedForSpeedRivals,
                ProfileVersion.PlantsVsZombiesGardenWarfare, ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4, ProfileVersion.PlantsVsZombiesGardenWarfare2,
                ProfileVersion.NeedForSpeed))
            {
                for (int i = 0; i < MaxLodCount * 2; i++)
                {
                    m_lodFadeDistanceFactors[i] = reader.ReadUShort();
                }
            }
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                ProfileVersion.Madden19, ProfileVersion.Fifa19,
                ProfileVersion.Madden20, ProfileVersion.Fifa20,
                ProfileVersion.Madden21, ProfileVersion.Fifa21,
                ProfileVersion.Madden22, ProfileVersion.Fifa22,
                ProfileVersion.Madden23))
            {
                m_flags = (MeshSetLayoutFlags)reader.ReadULong();
            }
            else
            {
                m_flags = (MeshSetLayoutFlags)reader.ReadUInt();
            }

            if (HasNewPartBoneLayout)
            {
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Madden23))
                {
                    m_shaderDrawOrder = reader.ReadShort();
                    m_shaderDrawOrderUserSlot = reader.ReadShort();
                }
                else
                {
                    m_shaderDrawOrder = reader.ReadByte();
                    m_shaderDrawOrderUserSlot = reader.ReadByte();
                }
                m_shaderDrawOrderSubOrder = reader.ReadShort();
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedEdge, ProfileVersion.Madden20))
            {
                m_unknownUShort = reader.ReadUShort();
            }

            ushort lodCount = reader.ReadUShort();
            ushort sectionCount = reader.ReadUShort();

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
            {
                ushort unk1 = reader.ReadUShort();
                ushort unk2 = reader.ReadUShort();
                ushort unk3 = reader.ReadUShort();
                ushort unk4 = reader.ReadUShort();
                ushort unk5 = reader.ReadUShort();
                ushort unk6 = reader.ReadUShort();
            }

            // part/bone data not stored per lod
            if (m_meshType != MeshType.MeshType_Rigid && HasNewPartBoneLayout)
            {
                m_bonePartCount = reader.ReadUShort();
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden20, ProfileVersion.Madden22, ProfileVersion.Madden23))
                {
                    m_boneCount = (ushort)reader.ReadUInt();
                }
                else
                {
                    m_boneCount = reader.ReadUShort();
                }

                if (m_meshType == MeshType.MeshType_Skinned)
                {
                    long boneIndicesOffset = reader.ReadLong();
                    long boneBoundingBoxesOffset = reader.ReadLong();
                    long curPos = reader.Position;

                    if (boneIndicesOffset != 0)
                    {
                        reader.Position = boneIndicesOffset;

                        for (int i = 0; i < m_boneCount; i++)
                        {
                            m_boneIndices.Add(reader.ReadUShort());
                        }
                    }
                    if (boneBoundingBoxesOffset != 0)
                    {
                        reader.Position = boneBoundingBoxesOffset;
                        for (int i = 0; i < m_boneCount; i++)
                        {
                            m_boneBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                        }
                    }

                    reader.Position = curPos;
                }
                else if (m_meshType == MeshType.MeshType_Composite)
                {
                    long partTransformsOffset = reader.ReadLong();
                    long partBoundingBoxesOffset = reader.ReadLong();
                    long curPos = reader.Position;

                    if (partTransformsOffset != 0)
                    {
                        reader.Position = partTransformsOffset;
                        for (int i = 0; i < m_bonePartCount; i++)
                        {
                            m_partTransforms.Add(reader.ReadLinearTransform());
                        }
                    }
                    if (partBoundingBoxesOffset != 0)
                    {
                        reader.Position = partBoundingBoxesOffset;
                        for (int i = 0; i < m_bonePartCount; i++)
                        {
                            m_partBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                        }
                    }

                    reader.Position = curPos;
                }
            }

            reader.Pad(16);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
            {
                m_unknownbfv = reader.ReadBytes(16);
            }

            // lods
            int z = 0;
            for (int i = 0; i < lodCount; i++)
            {
                Debug.Assert(reader.Position == lodOffsets[i]);
                m_lods.Add(new MeshSetLod(reader, am, ref z));

                if (HasNewPartBoneLayout)
                {
                    m_lods[i].SetParts(m_partTransforms, m_partBoundingBoxes);
                }
            }

            // strings
            reader.Pad(16);
            reader.Position = fullnameOffset;
            m_fullname = reader.ReadNullTerminatedString();
            reader.Position = nameOffset;
            m_name = reader.ReadNullTerminatedString();

            // Fifa 17/18/19 unknown blocks
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                ProfileVersion.Madden19, ProfileVersion.Fifa19))
            {
                reader.Pad(16);
                foreach (MeshSetLod lod in m_lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                    {
                        if (section.HasUnknown)
                        {
                            reader.Position += section.VertexCount * sizeof(uint);
                        }
                    }
                }
                reader.Pad(16);
                List<int> sectionCounts = new List<int>();
                foreach (MeshSetLod lod in m_lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                    {
                        if (section.HasUnknown2)
                        {
                            int totalCount = 0;
                            for (int i = 0; i < section.VertexCount; i++)
                            {
                                totalCount += reader.ReadUShort();
                            }

                            sectionCounts.Add(totalCount);
                        }
                    }
                }
                reader.Pad(16);
                foreach (MeshSetLod lod in m_lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                    {
                        if (section.HasUnknown3)
                        {
                            reader.Position += sectionCounts[0] * sizeof(ushort);
                            sectionCounts.RemoveAt(0);
                        }
                    }
                }
            }

            //inline data
            uint inlineDataOffset = BitConverter.ToUInt32(resMeta, 0);
            uint inlineDataSize = BitConverter.ToUInt32(resMeta, 4);

            if (inlineDataOffset != 0 && inlineDataSize != 0)
            {
                reader.Position = inlineDataOffset;
                foreach (MeshSetLod lod in m_lods)
                {
                    lod.ReadInlineData(reader);
                }
            }

        }

        public override byte[] SaveBytes()
        {
            MeshContainer meshContainer = new MeshContainer();
            PreProcess(meshContainer);

            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                Process(writer, meshContainer);

                uint startInlineRelocPos = (uint)writer.Position;
                uint inlineDataSize = 0;
                uint relocTableSize = 0;

                // inline data
                foreach (var lod in m_lods)
                {
                    if (lod.ChunkId == Guid.Empty)
                    {
                        writer.Write(lod.InlineData);
                        writer.WritePadding(16);
                    }
                }
                inlineDataSize = (uint)(writer.Position - startInlineRelocPos);

                // relocation table
                meshContainer.FixupRelocPtrs(writer);
                meshContainer.WriteRelocTable(writer);

                relocTableSize = (uint)(writer.Position - startInlineRelocPos - inlineDataSize);

                unsafe
                {
                    // update the res meta
                    fixed (byte* ptr = &resMeta[0])
                    {
                        *(uint*)(ptr + 0) = startInlineRelocPos;
                        *(uint*)(ptr + 4) = inlineDataSize;
                        *(uint*)(ptr + 8) = relocTableSize;
                    }
                }

                return writer.ToByteArray();
            }
        }

        private bool HasPartTransforms(List<LinearTransform> partTransformList)
        {
            LinearTransform nt = new LinearTransform()
            {
                right = new Vec3() { x = 1.0f, y = 0.0f, z = 0.0f },
                up = new Vec3() { x = 0.0f, y = 1.0f, z = 0.0f },
                forward = new Vec3() { x = 0.0f, y = 0.0f, z = 1.0f },
                trans = new Vec3() { x = 0.0f, y = 0.0f, z = 0.0f }
            };

            if (partTransformList.Count == 0)
            {
                return false;
            }

            foreach (var lt in partTransformList)
            {
                if (lt == nt)
                {
                    return false;
                }
            }

            return true;
        }

        private void PreProcess(MeshContainer meshContainer)
        {
            uint inlineDataOffset = 0;
            foreach (var lod in m_lods)
            {
                lod.PreProcess(meshContainer, ref inlineDataOffset);
            }

            foreach (var lod in m_lods)
            {
                meshContainer.AddRelocPtr("LOD", lod);
            }

            meshContainer.AddString(m_fullname, m_fullname.Replace(m_name, ""), true);
            meshContainer.AddString(m_name, m_name);

            if (HasNewPartBoneLayout)
            {
                if (m_meshType == MeshType.MeshType_Skinned)
                {
                    if (m_boneIndices.Count != 0)
                    {
                        meshContainer.AddRelocPtr("BONEINDICES", m_boneIndices);
                    }

                    if (m_boneBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddRelocPtr("BONEBBOXES", m_boneBoundingBoxes);
                    }
                }
                else if (m_meshType == MeshType.MeshType_Composite)
                {
                    if (HasPartTransforms(m_partTransforms))
                    {
                        meshContainer.AddRelocPtr("PARTTRANSFORM", m_partTransforms);
                    }

                    if (m_partBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddRelocPtr("PARTBBOXES", m_partBoundingBoxes);
                    }
                }
            }
        }

        private void Process(NativeWriter writer, MeshContainer meshContainer)
        {
            // header
            writer.Write(m_boundingBox);
            for (int i = 0; i < MaxLodCount; i++)
            {
                if (i < m_lods.Count)
                {
                    meshContainer.WriteRelocPtr("LOD", m_lods[i], writer);
                }
                else
                {
                    writer.Write((ulong)0);
                }
            }

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals, ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4, ProfileVersion.PlantsVsZombiesGardenWarfare))
            {
                writer.Write((long)0);
            }

            meshContainer.WriteRelocPtr("STR", m_fullname, writer);
            meshContainer.WriteRelocPtr("STR", m_name, writer);
            writer.Write(m_nameHash);
            writer.Write((uint)m_meshType);

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.MassEffectAndromeda,
                ProfileVersion.NeedForSpeedEdge, ProfileVersion.NeedForSpeedRivals,
                ProfileVersion.PlantsVsZombiesGardenWarfare, ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4, ProfileVersion.PlantsVsZombiesGardenWarfare2,
                ProfileVersion.NeedForSpeed))
            {
                for (int i = 0; i < MaxLodCount * 2; i++)
                {
                    writer.Write(m_lodFadeDistanceFactors[i]);
                }
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                            ProfileVersion.Madden19, ProfileVersion.Fifa19,
                            ProfileVersion.Madden20, ProfileVersion.Fifa20,
                            ProfileVersion.Madden21, ProfileVersion.Fifa21,
                            ProfileVersion.Madden22, ProfileVersion.Fifa22,
                            ProfileVersion.Madden23))
            {
                writer.Write((ulong)m_flags);
            }
            else
            {
                writer.Write((uint)m_flags);
            }

            if (HasNewPartBoneLayout)
            {
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden22, ProfileVersion.Madden23))
                {
                    writer.Write(m_shaderDrawOrder);
                    writer.Write(m_shaderDrawOrderUserSlot);
                }
                else
                {
                    writer.Write((byte)m_shaderDrawOrder);
                    writer.Write(m_shaderDrawOrderUserSlot);
                }
                writer.Write((byte)m_shaderDrawOrderSubOrder);
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedEdge, ProfileVersion.Madden20))
            {
                writer.Write(m_unknownUShort);
            }

            writer.Write((ushort)m_lods.Count);

            ushort sectionCount = 0;
            foreach (var lod in m_lods)
            {
                sectionCount += (ushort)lod.Sections.Count;
            }

            writer.Write(sectionCount);

            if (m_meshType != MeshType.MeshType_Rigid && HasNewPartBoneLayout)
            {
                writer.Write(m_bonePartCount);
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden20, ProfileVersion.Madden22, ProfileVersion.Madden23))
                {
                    writer.Write((int)m_boneCount);
                }
                else
                {
                    writer.Write(m_boneCount);
                }

                if (m_meshType == MeshType.MeshType_Skinned)
                {
                    if (m_boneIndices.Count != 0)
                    {
                        meshContainer.WriteRelocPtr("BONEINDICES", m_boneIndices, writer);
                    }
                    else
                    {
                        writer.Write((long)0);
                    }

                    if (m_boneBoundingBoxes.Count != 0)
                    {
                        meshContainer.WriteRelocPtr("BONEBBOXES", m_boneBoundingBoxes, writer);
                    }
                    else
                    {
                        writer.Write((long)0);
                    }
                }
                else if (m_meshType == MeshType.MeshType_Composite)
                {
                    if (HasPartTransforms(m_partTransforms))
                    {
                        meshContainer.WriteRelocPtr("PARTTRANSFORM", m_partTransforms, writer);
                    }
                    else
                    {
                        writer.Write((long)0);
                    }

                    if (m_partBoundingBoxes.Count != 0)
                    {
                        meshContainer.WriteRelocPtr("PARTBBOXES", m_partBoundingBoxes, writer);
                    }
                    else
                    {
                        writer.Write((long)0);
                    }
                }
            }

            writer.WritePadding(16);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
            {
                writer.Write(m_unknownbfv);
            }

            Debug.Assert(writer.Position == HeaderSize);

            // lods
            foreach (var lod in m_lods)
            {
                meshContainer.AddOffset("LOD", lod, writer);
                lod.Process(writer, meshContainer);
            }

            // sections
            foreach (var lod in m_lods)
            {
                meshContainer.AddOffset("SECTION", lod.Sections, writer);
                foreach (var section in lod.Sections)
                {
                    section.Process(writer, meshContainer);
                }
            }
            writer.WritePadding(16);

            // section bone list
            foreach (var lod in m_lods)
            {
                foreach (var section in lod.Sections)
                {
                    if (section.BoneList.Count > 0)
                    {
                        meshContainer.AddOffset("BONELIST", section.BoneList, writer);
                        foreach (var idx in section.BoneList)
                        {
                            writer.Write(idx);
                        }
                    }
                }
            }

            writer.WritePadding(16);

            // strings
            meshContainer.WriteStrings(writer);

            // Fifa 17/18 unkown block

            writer.WritePadding(16);

            // categories
            foreach (var lod in m_lods)
            {
                foreach (var category in lod.CategorySubsetIndices)
                {
                    meshContainer.AddOffset("SUBSET", category, writer);
                    writer.Write(category.ToArray());
                }
            }

            writer.WritePadding(16);

            // LOD bones
            foreach (var lod in m_lods)
            {
                if (m_meshType == MeshType.MeshType_Skinned)
                {
                    meshContainer.AddOffset("BONES", lod.BoneIndexArray, writer);
                    foreach (var idx in lod.BoneIndexArray)
                    {
                        writer.Write(idx);
                    }

                    if (lod.BoneShortNameArray.Count != 0 && !HasNewPartBoneLayout)
                    {
                        meshContainer.AddOffset("BONESNAMES", lod.BoneShortNameArray, writer);
                        foreach (var idx in lod.BoneShortNameArray)
                        {
                            writer.Write(idx);
                        }
                    }
                }
                else if (m_meshType == MeshType.MeshType_Composite)
                {
                    if (!HasNewPartBoneLayout)
                    {
                        if (lod.PartBoundingBoxes.Count != 0)
                        {
                            meshContainer.AddOffset("PARTBBOXES", lod.PartBoundingBoxes, writer);
                            foreach (var bbox in m_partBoundingBoxes)
                            {
                                writer.Write(bbox);
                            }

                            writer.WritePadding(16);
                        }
                        if (HasPartTransforms(lod.PartTransforms))
                        {
                            meshContainer.AddOffset("PARTTRANSFORM", lod.PartTransforms, writer);
                            foreach (var lt in m_partTransforms)
                            {
                                writer.Write(lt);
                            }

                            writer.WritePadding(16);
                        }
                    }
                    if (lod.PartIndices.Count != 0)
                    {
                        meshContainer.AddOffset("PARTINDICES", lod.PartIndices, writer);
                        foreach (var section in lod.PartIndices)
                        {
                            byte[] buffer = new byte[0x18];
                            for (int i = 0; i < section.Count; i++)
                            {
                                int index = section[i] / 8;
                                buffer[index] |= (byte)(1 << (section[i] % 8));
                            }
                            writer.Write(buffer);
                        }
                    }
                }
            }

            writer.WritePadding(16);

            if (HasNewPartBoneLayout)
            {
                if (m_meshType == MeshType.MeshType_Skinned)
                {
                    if (m_boneIndices.Count != 0)
                    {
                        meshContainer.AddOffset("BONEINDICES", m_boneIndices, writer);
                        foreach (var idx in m_boneIndices)
                        {
                            writer.Write(idx);
                        }

                        writer.WritePadding(16);
                    }
                    if (m_boneBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddOffset("BONEBBOXES", m_boneBoundingBoxes, writer);
                        foreach (var bbox in m_boneBoundingBoxes)
                        {
                            writer.Write(bbox);
                        }

                        writer.WritePadding(16);
                    }
                }
                else if (m_meshType == MeshType.MeshType_Composite)
                {
                    if (HasPartTransforms(m_partTransforms))
                    {
                        meshContainer.AddOffset("PARTTRANSFORM", m_partTransforms, writer);
                        foreach (var lt in m_partTransforms)
                        {
                            writer.Write(lt);
                        }

                        writer.WritePadding(16);
                    }

                    if (m_partBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddOffset("PARTBBOXES", m_partBoundingBoxes, writer);
                        foreach (var bbox in m_partBoundingBoxes)
                        {
                            writer.Write(bbox);
                        }

                        writer.WritePadding(16);
                    }
                }
            }
        }
    }
    #endregion

    #endregion
}
