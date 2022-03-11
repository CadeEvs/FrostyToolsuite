using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FrostySdk;
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
                    return ptr;
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
    public enum MeshLayoutFlags
    {
        IsBaseLod = 0x1,
        StreamingEnable = 0x40,
        StreamInstancingEnable = 0x10,
        VertexAnimationEnable = 0x80,
        Inline = 0x800,
        IsDataAvailable = 0x20000000,
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
    public enum MeshSubsetCategoryFlags
    {
        MeshSubsetCategoryFlags_Opaque = 0x1,
        MeshSubsetCategoryFlags_Transparent = 0x2,
        MeshSubsetCategoryFlags_TransparentDecal = 0x4,
        MeshSubsetCategoryFlags_ZOnly = 0x8,
        MeshSubsetCategoryFlags_Shadow = 0x10,
        MeshSubsetCategoryFlags_DynamicReflection = 0x20, // DAI
        MeshSubsetCategoryFlags_PlanarReflection = 0x40,  // DAI
        MeshSubsetCategoryFlags_StaticReflection = 0x80,
        MeshSubsetCategoryFlags_ShadowOverride = 0x100,
        MeshSubsetCategoryFlags_DynamicReflectionOverride = 0x200,
        MeshSubsetCategoryFlags_PlanarReflectionOverride = 0x400,
        MeshSubsetCategoryFlags_StaticReflectionOverride = 0x800,
        MeshSubsetCategoryFlags_SunShadow = 0x1000,
        MeshSubsetCategoryFlags_SunShadowOverride = 0x2000,
        MeshSubsetCategoryFlags_LocalShadow = 0x4000,
        MeshSubsetCategoryFlags_LocalShadowOverride = 0x8000,

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
        public string Name => materialName;
        public int MaterialId => materialId;
        public uint UnknownInt => unknownInt1;
        public uint PrimitiveCount { get => primitiveCount; set => primitiveCount = value; }
        public uint StartIndex { get => startIndex; set => startIndex = value; }
        public uint VertexOffset { get => vertexOffset; set => vertexOffset = value; }
        public uint VertexCount { get => vertexCount; set => vertexCount = value; }
        public GeometryDeclarationDesc[] GeometryDeclDesc => geometryDeclarationDesc;
        public List<ushort> BoneList => boneList;
        public uint VertexStride => vertexStride;
        public PrimitiveType PrimitiveType => primitiveType;
        public byte BonesPerVertex
        {
            get => bonesPerVertex;
            set
            {
                bonesPerVertex = value;
                if (bonesPerVertex > 8)
                    bonesPerVertex = 8;
            }
        }
        //public static int Size
        //{
        //    get
        //    {
        //        switch (ProfilesLibrary.DataVersion)
        //        {
        //            case (int)ProfileVersion.MassEffectAndromeda: return 0xD0;
        //            case (int)ProfileVersion.StarWarsBattlefront: return 0xC0;
        //            case (int)ProfileVersion.MirrorsEdgeCatalyst: return 0x130;
        //            case (int)ProfileVersion.DragonAgeInquisition: return 0xC0;
        //            case (int)ProfileVersion.Battlefield4: return 0xC0;
        //            default: return 0;
        //        }
        //    }
        //}
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
        public bool HasUnknown => hasUnknown;
        public bool HasUnknown2 => hasUnknown2;
        public bool HasUnknown3 => hasUnknown3;

        private long offset1;
        private long offset2;
        private string materialName;
        private int materialId;
        private uint unknownInt1;
        private uint primitiveCount;
        private uint startIndex;
        private uint vertexOffset;
        private uint vertexCount;
        private GeometryDeclarationDesc[] geometryDeclarationDesc = new GeometryDeclarationDesc[2];
        private byte vertexStride;
        private PrimitiveType primitiveType;
        private byte bonesPerVertex;

        private bool hasUnknown;
        private bool hasUnknown2;
        private bool hasUnknown3;

        private List<ushort> boneList = new List<ushort>();
        private List<float> texCoordRatios = new List<float>();
        private byte[] unknownData = null;
        private int sectionIndex;

        internal MeshSetSection()
        {
        }

        //public MeshSetSection(string inName, int inMaterialId)
        //{
        //    materialName = inName;
        //    materialId = inMaterialId;

        //    int count = 44;
        //    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
        //        count = 48;
        //    else if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
        //        count = 36;
        //    unknownData = new byte[count];

        //    texCoordRatios = new List<float>() { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
        //    primitiveType = PrimitiveType.PrimitiveType_TriangleList;
        //}

        public MeshSetSection(NativeReader reader, AssetManager am, int index)
        {
            sectionIndex = index;
            offset1 = reader.ReadLong();// 0x18 bytes after name
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield1 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                offset2 = reader.ReadLong();

            long stringOffset = reader.ReadLong(); // materialName

            materialId = reader.ReadInt();
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesGardenWarfare && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedEdge)
                unknownInt1 = reader.ReadUInt();
            primitiveCount = reader.ReadUInt();
            startIndex = reader.ReadUInt();
            vertexOffset = reader.ReadUInt();
            vertexCount = reader.ReadUInt();
            vertexStride = reader.ReadByte();
            primitiveType = (PrimitiveType)reader.ReadByte();
            bonesPerVertex = reader.ReadByte();
            uint boneCount = reader.ReadByte();

            // Fifa 17/18 store boneCount in a UINT
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem
                || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat
                )
            {
                boneCount = reader.ReadUInt();
            }

            // MEC/BF1/SWBF2/SWS
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield1 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            {
                reader.ReadUInt();
                reader.ReadUInt();
                reader.ReadUInt();

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    bonesPerVertex = reader.ReadByte();
                    reader.ReadByte();
                    boneCount = reader.ReadUShort();
                }
                else
                {
                    bonesPerVertex = reader.ReadByte();
                    boneCount = reader.ReadUShort();
                    reader.ReadByte();
                }
            }

            // boneIndices
            long boneListOffset = reader.ReadLong();

            // Fifa18/SWBF2/NFS Payback/Anthem
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem
                || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons
                )
            {
                reader.ReadULong();
            }

            // Fifa 17/18
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
            {
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                    reader.ReadLong();

                long unknownOffset = reader.ReadLong();
                if (unknownOffset != 0)
                    hasUnknown = true;

                unknownOffset = reader.ReadLong();
                if (unknownOffset != 0)
                    hasUnknown2 = true;

                unknownOffset = reader.ReadLong();
                if (unknownOffset != 0)
                    hasUnknown3 = true;
            }

            // geometry declarations
            for (int geomDeclId = 0; geomDeclId < DeclCount; geomDeclId++)
            {
                geometryDeclarationDesc[geomDeclId].Elements = new GeometryDeclarationDesc.Element[GeometryDeclarationDesc.MaxElements];
                geometryDeclarationDesc[geomDeclId].Streams = new GeometryDeclarationDesc.Stream[GeometryDeclarationDesc.MaxStreams];

                for (int i = 0; i < GeometryDeclarationDesc.MaxElements; i++)
                {
                    GeometryDeclarationDesc.Element elem = new GeometryDeclarationDesc.Element
                    {
                        Usage = (VertexElementUsage)reader.ReadByte(),
                        Format = (VertexElementFormat)reader.ReadByte(),
                        Offset = reader.ReadByte(),
                        StreamIndex = reader.ReadByte()
                    };

                    geometryDeclarationDesc[geomDeclId].Elements[i] = elem;
                }
                for (int i = 0; i < GeometryDeclarationDesc.MaxStreams; i++)
                {
                    GeometryDeclarationDesc.Stream stream = new GeometryDeclarationDesc.Stream
                    {
                        VertexStride = reader.ReadByte(),
                        Classification = (VertexElementClassification)reader.ReadByte()
                    };

                    geometryDeclarationDesc[geomDeclId].Streams[i] = stream;
                }

                geometryDeclarationDesc[geomDeclId].ElementCount = reader.ReadByte();
                geometryDeclarationDesc[geomDeclId].StreamCount = reader.ReadByte();
                reader.ReadBytes(2); // padding
            }

            // texture ratios
            for (int i = 0; i < 6; i++)
                texCoordRatios.Add(reader.ReadFloat());

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
                case (int)ProfileVersion.StarWarsSquadrons:
                    count = 36;
                    break;
                default:
                    count = 44;
                    break;
            }
            unknownData = reader.ReadBytes(count);

            // section data bone list
            long curPos = reader.Position;
            reader.Position = boneListOffset;
            for (int k = 0; k < boneCount; k++)
                BoneList.Add(reader.ReadUShort());

            // strings
            reader.Position = stringOffset;
            materialName = reader.ReadNullTerminatedString();
            reader.Position = curPos;
        }

        public void SetBones(IEnumerable<ushort> bones)
        {
            boneList.Clear();
            foreach (ushort boneId in bones)
            {
                //if ((boneId & 0x8000) == 0)
                boneList.Add(boneId);
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
            meshContainer.AddString(sectionIndex + ":" + materialName, materialName);
            if (boneList.Count > 0)
                meshContainer.AddRelocPtr("BONELIST", boneList);
        }

        internal void Process(NativeWriter writer, MeshContainer meshContainer)
        {
            writer.Write(offset1);
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield1 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                writer.Write(offset2);
            meshContainer.WriteRelocPtr("STR", sectionIndex + ":" + materialName, writer);
            writer.Write(materialId);
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesGardenWarfare && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedEdge)
                writer.Write(unknownInt1);
            writer.Write(primitiveCount);
            writer.Write(startIndex);
            writer.Write(vertexOffset);
            writer.Write(vertexCount);

            writer.Write(vertexStride);
            writer.Write((byte)primitiveType);

            // Fifa17/Fifa18/Madden19/Fifa19/Anthem/Madden20/Fifa20 store boneCount in a UINT
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem
                || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20
#if FROSTY_ALPHA || FROSTY_DEVELOPER
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat
#endif
                )
            {
                writer.Write(bonesPerVertex);
                writer.Write((byte)0x00);
                writer.Write(boneList.Count);
            }

            // MEC/BF1/SWBF2/BFV store boneCount as a ushort
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield1 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            {
                writer.Write((ushort)0x00); // padding

                writer.Write((uint)0x00);
                writer.Write((uint)0x00);
                writer.Write((uint)0x00);

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    writer.Write(bonesPerVertex);
                    writer.Write((byte)0x00);
                    writer.Write((ushort)boneList.Count);
                }
                else
                {
                    writer.Write(bonesPerVertex);
                    writer.Write((ushort)boneList.Count);
                    writer.Write((byte)0x00);
                }
            }
            else
            {
                writer.Write(bonesPerVertex);
                writer.Write((byte)boneList.Count);
            }

            if (boneList.Count > 0)
                meshContainer.WriteRelocPtr("BONELIST", boneList, writer);
            else
                writer.Write((ulong)0);

            // Fifa18/SWBF2/NFS Payback/Anthem
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem
                || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons
#if FROSTY_ALPHA || FROSTY_DEVELOPER
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat
#endif
                )
            {
                // unknown
                writer.Write((ulong)0);
            }

            // Fifa 17/18
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
            {
                // @todo
            }

            // geometry declarations
            for (int declId = 0; declId < DeclCount; declId++)
            {
                for (int elemId = 0; elemId < geometryDeclarationDesc[declId].Elements.Length; elemId++)
                {
                    writer.Write((byte)geometryDeclarationDesc[declId].Elements[elemId].Usage);
                    writer.Write((byte)geometryDeclarationDesc[declId].Elements[elemId].Format);
                    writer.Write(geometryDeclarationDesc[declId].Elements[elemId].Offset);
                    writer.Write(geometryDeclarationDesc[declId].Elements[elemId].StreamIndex);
                }
                for (int streamId = 0; streamId < geometryDeclarationDesc[declId].Streams.Length; streamId++)
                {
                    writer.Write(geometryDeclarationDesc[declId].Streams[streamId].VertexStride);
                    writer.Write((byte)geometryDeclarationDesc[declId].Streams[streamId].Classification);
                }
                writer.Write(geometryDeclarationDesc[declId].ElementCount);
                writer.Write(geometryDeclarationDesc[declId].StreamCount);
                writer.Write((ushort)0); // padding
            }

            // texcoord ratios
            for (int i = 0; i < 6; i++)
                writer.Write(texCoordRatios[i]);

            // unknown data
            writer.Write(unknownData);

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

        public MeshType Type { get => meshType; set => meshType = value; }
        public List<MeshSetSection> Sections => sections;
        public MeshLayoutFlags Flags => flags;
        public int IndexUnitSize
        {
            get
            {
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                    return (indexBufferFormat.formatEnum == IndexBufferFormat.IndexBufferFormat_16Bit) ? 16 : 32;

                int value = (int)Enum.Parse(TypeLibrary.GetType("RenderFormat"), "RenderFormat_R32_UINT");
                return (indexBufferFormat.format == value) ? 32 : 16;
            }
        }
        public uint IndexBufferSize { get => indexBufferSize; set => indexBufferSize = value; }
        public uint VertexBufferSize { get => vertexBufferSize; set => vertexBufferSize = value; }
        public int AdjacencyBufferSize => adjacencyBufferSize;
        public Guid ChunkId { get => chunkId; set => chunkId = value; }
        public string FullName
        {
            get { return shaderDebugName; }
            set
            {
                shaderDebugName = value + shortName.Substring(shortName.LastIndexOf("_"));
                name = value + shortName.Substring(shortName.LastIndexOf("_"));
                shortName = value.Substring(value.LastIndexOf("/") + 1) + shortName.Substring(shortName.LastIndexOf("_"));
                nameHash = (uint)Frosty.Hash.Fnv1.HashString(name);
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                shaderDebugName = "Mesh:" + value + shortName.Substring(shortName.LastIndexOf("_"));
                name = value + shortName.Substring(shortName.LastIndexOf("_"));
                shortName = value.Substring(value.LastIndexOf("/") + 1) + shortName.Substring(shortName.LastIndexOf("_"));
                nameHash = (uint)Frosty.Hash.Fnv1.HashString(name);
            }
        }
        public string ShortName
        {
            get { return shortName; }
            set
            {
                shortName = value + shortName.Substring(shortName.LastIndexOf("_"));
                name = name.Substring(0, name.LastIndexOf("/") + 1) + shortName;
                shaderDebugName = "Mesh:" + name;
                nameHash = (uint)Frosty.Hash.Fnv1.HashString(name);
            }
        }
        public int BoneCount => boneIndexArray.Count;
        public List<uint> BoneIndexArray => boneIndexArray;
        public List<uint> BoneShortNameArray => boneShortNameArray;
        public int PartCount => partBoundingBoxes.Count;
        public List<AxisAlignedBox> PartBoundingBoxes => partBoundingBoxes;
        public List<LinearTransform> PartTransforms => partTransforms;
        public List<List<int>> PartIndices => partIndices;
        public byte[] InlineData => inlineData;
        public List<List<byte>> CategorySubsetIndices => subsetCategories;

        //public int Size
        //{
        //    get
        //    {
        //        if (meshType == MeshType.MeshType_Rigid)
        //        {
        //            switch (ProfilesLibrary.DataVersion)
        //            {
        //                case (int)ProfileVersion.MassEffectAndromeda: return 0xA0;
        //                case (int)ProfileVersion.StarWarsBattlefront: return 0xA0;
        //                case (int)ProfileVersion.MirrorsEdgeCatalyst: return 0xB0;
        //                case (int)ProfileVersion.DragonAgeInquisition: return 0xB0;
        //            }
        //        }
        //        else if (meshType == MeshType.MeshType_Skinned)
        //        {
        //            switch (ProfilesLibrary.DataVersion)
        //            {
        //                case (int)ProfileVersion.MirrorsEdgeCatalyst: return 0xC0;
        //                default: return 0xB0;
        //        }
        //        }
        //        return 0;
        //    }
        //}
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

        private MeshType meshType;
        private uint maxInstances;
        private uint unknownUInt;
        private MeshLayoutFlags flags;
        private IndexBufferFormatStruct indexBufferFormat;
        private uint indexBufferSize;
        private uint vertexBufferSize;
        private int adjacencyBufferSize;
        private Guid chunkId;
        private string shaderDebugName;
        private string name;
        private string shortName;
        private uint nameHash;
        private List<MeshSetSection> sections = new List<MeshSetSection>();
        private List<List<byte>> subsetCategories = new List<List<byte>>();

        //private uint boneCount;
        private List<uint> boneIndexArray = new List<uint>();
        private List<uint> boneShortNameArray = new List<uint>();

        private List<AxisAlignedBox> partBoundingBoxes = new List<AxisAlignedBox>();
        private List<LinearTransform> partTransforms = new List<LinearTransform>();
        private List<List<int>> partIndices = new List<List<int>>();

        private byte[] inlineData;
        private uint inlineDataOffset;
        private byte[] adjacencyData;
        private bool hasBoneShortNames;

        public MeshSetLod(NativeReader reader, AssetManager am)
        {
            meshType = (MeshType)reader.ReadUInt();
            maxInstances = reader.ReadUInt();

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
                unknownUInt = reader.ReadUInt();

            uint sectionCount = reader.ReadUInt();
            long sectionOffset = reader.ReadLong();

            // sections
            for (uint i = 0; i < sectionCount; i++)
                sections.Add(null);

            // section categories
            for (int i = 0; i < MaxCategories; i++)
            {
                int count = reader.ReadInt();
                long subsetCategoryOffset = reader.ReadLong();
                subsetCategories.Add(new List<byte>());
                long curpos = reader.Position;
                reader.Position = subsetCategoryOffset;
                for (int z = 0; z < count; z++)
                    subsetCategories[i].Add(reader.ReadByte());
                reader.Position = curpos;
            }

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                unknownUInt = reader.ReadUInt();

            flags = (MeshLayoutFlags)reader.ReadUInt();
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesGardenWarfare && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
                indexBufferFormat.format = reader.ReadInt();
            else
                indexBufferFormat.formatEnum = (IndexBufferFormat)reader.ReadInt();

            indexBufferSize = reader.ReadUInt();
            vertexBufferSize = reader.ReadUInt();

            if (HasAdjacencyInMesh)
            {
                adjacencyBufferSize = reader.ReadInt();
                adjacencyData = new byte[adjacencyBufferSize];
            }

            chunkId = reader.ReadGuid();
            inlineDataOffset = reader.ReadUInt();

            long adjacencyBufferOffset = 0;
            if (HasAdjacencyInMesh)
                adjacencyBufferOffset = reader.ReadLong();

            long stringOffset01 = reader.ReadLong();
            long stringOffset02 = reader.ReadLong();
            long stringOffset03 = reader.ReadLong();

            nameHash = reader.ReadUInt();
            reader.ReadLong();

            // Bones/Parts
            uint bonePartCount = 0;
            long bonePartOffset01 = 0;
            long bonePartOffset02 = 0;
            long bonePartOffset03 = 0;

            // Fifa 17/18/SWBF2/NFS Payback/SWS/MEA/Anthem
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 ||
                     ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat ||
                     ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            {
                if (meshType == MeshType.MeshType_Skinned)
                {
                    bonePartCount = reader.ReadUInt();
                    bonePartOffset01 = reader.ReadLong();
                    // TODO: Check if this is right
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
                        bonePartOffset03 = reader.ReadLong();
                }
                else if (meshType == MeshType.MeshType_Composite)
                    bonePartOffset03 = reader.ReadLong();
            }
            else
            {
                // All others
                bonePartCount = reader.ReadUInt();
                if (meshType >= MeshType.MeshType_Skinned)
                {
                    bonePartOffset01 = reader.ReadLong();
                    bonePartOffset02 = reader.ReadLong();

                    if (meshType == MeshType.MeshType_Composite)
                        bonePartOffset03 = reader.ReadLong();
                }
            }

            reader.Pad(16);

            // bone data
            long curPos = reader.Position;
            if (meshType == MeshType.MeshType_Skinned)
            {
                reader.Position = bonePartOffset01;
                for (int i = 0; i < bonePartCount; i++)
                    boneIndexArray.Add(reader.ReadUInt());

                if (bonePartOffset02 != 0)
                {
                    reader.Position = bonePartOffset02;
                    for (int i = 0; i < bonePartCount; i++)
                        boneShortNameArray.Add(reader.ReadUInt());
                }
            }

            // part data
            else if (meshType == MeshType.MeshType_Composite)
            {
                if (bonePartOffset01 != 0)
                {
                    reader.Position = bonePartOffset01;
                    for (int i = 0; i < bonePartCount; i++)
                        partBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                }
                if (bonePartOffset02 != 0)
                {
                    reader.Position = bonePartOffset02;
                    for (int i = 0; i < bonePartCount; i++)
                        partTransforms.Add(reader.ReadLinearTransform());
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
                                    sectionPartIndices.Add((i * 8) + j);
                                b >>= 1;
                            }
                        }
                        partIndices.Add(sectionPartIndices);
                    }                
                }
            }

            // adjacency data
            reader.Position = adjacencyBufferOffset;
            adjacencyData = reader.ReadBytes(adjacencyBufferSize);

            // strings
            reader.Position = stringOffset01;
            shaderDebugName = reader.ReadNullTerminatedString();
            reader.Position = stringOffset02;
            name = reader.ReadNullTerminatedString();
            reader.Position = stringOffset03;
            shortName = reader.ReadNullTerminatedString();
            reader.Position = curPos;

            hasBoneShortNames = boneShortNameArray.Count > 0;
        }

        public void SetIndexBufferFormatSize(int newSize)
        {
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesGardenWarfare && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
            {
                indexBufferFormat.format = (int)((newSize == 2)
                    ? Enum.Parse(TypeLibrary.GetType("RenderFormat"), "RenderFormat_R16_UINT")
                    : Enum.Parse(TypeLibrary.GetType("RenderFormat"), "RenderFormat_R32_UINT")
                    );
            }
            else
            {
                indexBufferFormat.formatEnum = (newSize == 2)
                    ? IndexBufferFormat.IndexBufferFormat_16Bit
                    : IndexBufferFormat.IndexBufferFormat_32Bit;
            }
        }

        public bool IsSectionInCategory(MeshSetSection section, MeshSubsetCategory category)
        {
            int index = GetSectionIndex(section);
            if ((int)category >= subsetCategories.Count)
                return false;
            return subsetCategories[(int)category].Contains((byte)index);
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
                return;

            if (!subsetCategories[(int)category].Contains(index))
                subsetCategories[(int)category].Add(index);
        }

        public void SetParts(List<LinearTransform> inPartTransforms, List<AxisAlignedBox> inPartBBoxes)
        {
            partTransforms = inPartTransforms;
            partBoundingBoxes = inPartBBoxes;

            if (partTransforms.Count != partBoundingBoxes.Count)
            {
                for (int i = 0; i < partBoundingBoxes.Count; i++)
                {
                    if (i >= partTransforms.Count)
                    {
                        partTransforms.Add(new LinearTransform()
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
            boneIndexArray.Clear();
            boneShortNameArray.Clear();
        }

        public void AddBones(IEnumerable<ushort> bones, IEnumerable<string> boneNames)
        {
            foreach (ushort boneId in bones)
            {
                if (!boneIndexArray.Contains(boneId))
                    boneIndexArray.Add(boneId);
            }
            if (hasBoneShortNames)
            {
                foreach (string boneName in boneNames)
                {
                    uint hash = (uint)Frosty.Hash.Fnv1.HashString(boneName.ToLower());
                    if (!boneShortNameArray.Contains(hash))
                        boneShortNameArray.Add(hash);
                }
            }
        }

        public MeshSubsetCategoryFlags GetSectionCategories(int index)
        {
            MeshSubsetCategoryFlags flags = 0;
            for (int i = 0; i < subsetCategories.Count; i++)
            {
                if (subsetCategories[i].Contains((byte)index))
                    flags |= (MeshSubsetCategoryFlags)(1 << i);
            }
            return flags;
        }

        public void ClearCategories()
        {
            for (int i = 0; i < subsetCategories.Count; i++)
                subsetCategories[i].Clear();
        }

        public void ReadInlineData(NativeReader reader)
        {
            if (chunkId == Guid.Empty)
            {
                inlineData = reader.ReadBytes((int)(vertexBufferSize + indexBufferSize));
                while (reader.Position % 16 != 0)
                    reader.Position++;
            }
        }

        public void SetInlineData(byte[] inBuffer)
        {
            inlineData = inBuffer;
            chunkId = Guid.Empty;
            flags = MeshLayoutFlags.Inline;
        }

        private int GetSectionIndex(MeshSetSection inSection)
        {
            int index = -1;
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i] == inSection)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        internal void PreProcess(MeshContainer meshContainer, ref uint inInlineDataOffset)
        {
            inlineDataOffset = 0xFFFFFFFF;
            if (inlineData != null)
            {
                inlineDataOffset = inInlineDataOffset;
                inInlineDataOffset += (uint)inlineData.Length;
            }

            meshContainer.AddRelocArray("SECTION", sections.Count, sections);
            foreach (var section in sections)
            {
                section.PreProcess(meshContainer);
            }
            foreach (var category in subsetCategories)
            {
                meshContainer.AddRelocArray("SUBSET", category.Count, category);
            }
            if (HasAdjacencyInMesh && inlineDataOffset != 0xFFFFFFFF)
            {
                meshContainer.AddRelocPtr("ADJACENCY", adjacencyData);
            }
            meshContainer.AddString(shaderDebugName, "Mesh:", true);
            meshContainer.AddString(name, name.Replace(shortName, ""), true);
            meshContainer.AddString(shortName, shortName);

            if (meshType == MeshType.MeshType_Skinned)
            {
                if (boneIndexArray.Count != 0)
                    meshContainer.AddRelocPtr("BONES", boneIndexArray);

                // !SWBF2/NFS Payback/Anthem maybe ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville
                if (boneShortNameArray.Count != 0 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 &&
                         ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedPayback && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                {
                    meshContainer.AddRelocPtr("BONESNAMES", boneShortNameArray);
                }
            }
            else if (meshType == MeshType.MeshType_Composite)
            {
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.MassEffectAndromeda && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 &&
                     ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedHeat &&
                     ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedPayback && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                {
                    if (partBoundingBoxes.Count != 0)
                        meshContainer.AddRelocPtr("PARTBBOX", partBoundingBoxes);
                    if (partTransforms.Count != 0)
                        meshContainer.AddRelocPtr("PARTTRANSFORM", partTransforms);
                }
                if (partIndices.Count != 0)
                    meshContainer.AddRelocPtr("PARTINDICES", partIndices);
            }
        }

        internal void Process(NativeWriter writer, MeshContainer meshContainer)
        {
            writer.Write((int)meshType);
            writer.Write(maxInstances);
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
                writer.Write(unknownUInt);
            meshContainer.WriteRelocArray("SECTION", sections, writer);
            foreach (var category in subsetCategories)
            {
                meshContainer.WriteRelocArray("SUBSET", category, writer);
            }
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                writer.Write(unknownUInt);
            writer.Write((int)flags);
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesGardenWarfare && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
                writer.Write(indexBufferFormat.format);
            else
                writer.Write((int)indexBufferFormat.formatEnum);
            writer.Write(indexBufferSize);
            writer.Write(vertexBufferSize);
            if (HasAdjacencyInMesh)
                writer.Write(adjacencyData.Length/*adjacencyData.Length*/);
            writer.Write(chunkId);
            writer.Write(inlineDataOffset);
            if (HasAdjacencyInMesh)
            {
                if (inlineDataOffset != 0xFFFFFFFF)
                    meshContainer.WriteRelocPtr("ADJACENCY", adjacencyData, writer);
                else
                    writer.Write((ulong)0);
            }
            meshContainer.WriteRelocPtr("STR", shaderDebugName, writer);
            meshContainer.WriteRelocPtr("STR", name, writer);
            meshContainer.WriteRelocPtr("STR", shortName, writer);
            writer.Write(nameHash);
            writer.Write((long)0x00); // unknown

            if (meshType == MeshType.MeshType_Skinned)
            {
                writer.Write(BoneCount);

                if (boneIndexArray.Count != 0)
                    meshContainer.WriteRelocPtr("BONES", boneIndexArray, writer);

                // !SWBF2/NFS Payback/Anthem maybe ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 &&
                         ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedPayback && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                {
                    if (boneShortNameArray.Count != 0)
                        meshContainer.WriteRelocPtr("BONESNAMES", boneShortNameArray, writer);
                }
            }
            else if (meshType == MeshType.MeshType_Composite)
            {
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.MassEffectAndromeda && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 &&
                     ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedHeat &&
                     ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedPayback && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                {
                    if (partBoundingBoxes.Count != 0)
                        meshContainer.WriteRelocPtr("PARTBBOX", partBoundingBoxes, writer);
                }
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.MassEffectAndromeda && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 &&
                     ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedHeat &&
                     ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedPayback && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                {
                    if (partTransforms.Count != 0)
                        meshContainer.WriteRelocPtr("PARTTRANSFORM", partTransforms, writer);
                }
                if (partIndices.Count != 0)
                    meshContainer.WriteRelocPtr("PARTINDICES", partIndices, writer);
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
            get => tangentSpaceCompressionType;
            set
            {
                tangentSpaceCompressionType = value;
                foreach (MeshSetLod lod in lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                        section.TangentSpaceCompressionType = value;
                }
            }
        }
        public AxisAlignedBox BoundingBox => boundingBox;
        public List<MeshSetLod> Lods => lods;
        public MeshType Type { get => meshType; set => meshType = value; }
        public MeshLayoutFlags Flags => flags;
        public string FullName
        {
            get => fullname;
            set
            {
                fullname = value.ToLower();
                nameHash = (uint)Frosty.Hash.Fnv1.HashString(fullname);

                int id = fullname.LastIndexOf('/');
                name = (id != -1) ? fullname.Substring(id + 1) : "";
            }
        }
        public string Name => name;
        public int HeaderSize => BitConverter.ToUInt16(resMeta, 0x0c);

        public int MaxLodCount
        {
            get
            {
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.NeedForSpeedRivals:
                    case (int)ProfileVersion.DragonAgeInquisition:
                    case (int)ProfileVersion.Battlefield4:
                    case (int)ProfileVersion.PlantsVsZombiesGardenWarfare:
                        return 6;
                    default:
                        return 7;
                }
            }
        }

        private TangentSpaceCompressionType tangentSpaceCompressionType;
        private AxisAlignedBox boundingBox;
        private string fullname;
        private string name;
        private uint nameHash;
        private MeshType meshType;
        private MeshLayoutFlags flags;
        private List<MeshSetLod> lods = new List<MeshSetLod>();
        private List<uint> unknownUInts = new List<uint>();

        private ushort bonePartCount;
        private ushort boneCount;
        private List<ushort> boneIndices = new List<ushort>();
        private List<AxisAlignedBox> boneBoundingBoxes = new List<AxisAlignedBox>();

        private List<AxisAlignedBox> partBoundingBoxes = new List<AxisAlignedBox>();
        private List<LinearTransform> partTransforms = new List<LinearTransform>();

        public MeshSet()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            boundingBox = reader.ReadAxisAlignedBox();

            List<long> lodOffsets = new List<long>();
            for (int i = 0; i < MaxLodCount; i++)
                lodOffsets.Add(reader.ReadLong());

            long fullnameOffset = reader.ReadLong();
            long nameOffset = reader.ReadLong();

            nameHash = reader.ReadUInt();
            meshType = (MeshType)reader.ReadUInt();
            flags = (MeshLayoutFlags)reader.ReadUInt();

            switch (ProfilesLibrary.DataVersion)
            {
                case (int)ProfileVersion.Fifa17:
                    unknownUInts.Add(reader.ReadUInt());
                    unknownUInts.Add(reader.ReadUInt());
                    break;
                case (int)ProfileVersion.NeedForSpeedRivals:
                case (int)ProfileVersion.DragonAgeInquisition:
                case (int)ProfileVersion.Battlefield4:
                case (int)ProfileVersion.PlantsVsZombiesGardenWarfare:
                case (int)ProfileVersion.PlantsVsZombiesGardenWarfare2:
                case (int)ProfileVersion.NeedForSpeed:
                    break;
                case (int)ProfileVersion.NeedForSpeedEdge:
                    unknownUInts.Add(reader.ReadUShort());
                    break;
                case (int)ProfileVersion.Fifa18:
                case (int)ProfileVersion.Madden19:
                case (int)ProfileVersion.Fifa19:
                case (int)ProfileVersion.Fifa20:
                    for (int i = 0; i < 8; i++)
                        unknownUInts.Add(reader.ReadUInt());
                    break;
                case (int)ProfileVersion.Battlefield5:
                    for (int i = 0; i < 6; i++)
                        unknownUInts.Add(reader.ReadUInt());
                    break;
                case (int)ProfileVersion.Anthem:
                case (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville:
                case (int)ProfileVersion.NeedForSpeedHeat:
                    for (int i = 0; i < 7; i++)
                        unknownUInts.Add(reader.ReadUInt());
                    break;
                case (int)ProfileVersion.Madden20:
                    for (int i = 0; i < 8; i++)
                        unknownUInts.Add(reader.ReadUInt());
                    unknownUInts.Add(reader.ReadUShort());
                    break;
                default:
                    unknownUInts.Add(reader.ReadUInt());
                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.MassEffectAndromeda)
                    {
                        for (int i = 0; i < 5; i++)
                            unknownUInts.Add(reader.ReadUInt());
                        if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                            unknownUInts.Add(reader.ReadUInt());
                    }
                    break;
            }

            ushort lodCount = reader.ReadUShort();
            ushort sectionCount = reader.ReadUShort();

            // SWBF2/NFS Payback/Fifa 20/SWS
            if (meshType != MeshType.MeshType_Rigid && 
                (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda ||ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons || 
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || 
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat))
            {
                bonePartCount = reader.ReadUShort();
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20)
                    boneCount = reader.ReadUShort();
                else
                    boneCount = (ushort)reader.ReadUInt();


                if (meshType == MeshType.MeshType_Skinned && boneCount != 0)
                {
                    long boneIndicesOffset = reader.ReadLong();
                    long boneBoundingBoxesOffset = reader.ReadLong();
                    long curPos = reader.Position;

                    if (boneIndicesOffset != 0)
                    {
                        reader.Position = boneIndicesOffset;

                        for (int i = 0; i < boneCount; i++)
                            boneIndices.Add(reader.ReadUShort());
                    }
                    if (boneBoundingBoxesOffset != 0)
                    {
                        reader.Position = boneBoundingBoxesOffset;
                        for (int i = 0; i < boneCount; i++)
                            boneBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                    }

                    reader.Position = curPos;
                }
                else if (meshType == MeshType.MeshType_Composite && bonePartCount != 0)
                {
                    long partTransformsOffset = reader.ReadLong();
                    long partBoundingBoxesOffset = reader.ReadLong();
                    long curPos = reader.Position;

                    if (partTransformsOffset != 0)
                    {
                        reader.Position = partTransformsOffset;
                        for (int i = 0; i < bonePartCount; i++)
                            partTransforms.Add(reader.ReadLinearTransform());
                    }
                    if (partBoundingBoxesOffset != 0)
                    {
                        reader.Position = partBoundingBoxesOffset;
                        for (int i = 0; i < bonePartCount; i++)
                            partBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                    }

                    reader.Position = curPos;
                }
            }

            reader.Pad(16);

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5)
                reader.ReadBytes(16);

            // lods
            for (int i = 0; i < lodCount; i++)
            {
                Debug.Assert(reader.Position == lodOffsets[i]);
                lods.Add(new MeshSetLod(reader, am ));

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat)
                    lods[i].SetParts(partTransforms, partBoundingBoxes);
            }

            // sections
            int z = 0;
            foreach (MeshSetLod lod in lods)
            {
                for (int i = 0; i < lod.Sections.Count; i++)
                    lod.Sections[i] = new MeshSetSection(reader, am, z++);
            }

            // strings
            reader.Pad(16);
            reader.Position = fullnameOffset;
            fullname = reader.ReadNullTerminatedString();
            reader.Position = nameOffset;
            name = reader.ReadNullTerminatedString();

            // Fifa 17/18 unknown blocks
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19)
            {
                reader.Pad(16);
                foreach (MeshSetLod lod in lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                    {
                        if (section.HasUnknown)
                            reader.Position += section.VertexCount * sizeof(uint);
                    }
                }
                reader.Pad(16);
                List<int> sectionCounts = new List<int>();
                foreach (MeshSetLod lod in lods)
                {
                    foreach (MeshSetSection section in lod.Sections)
                    {
                        if (section.HasUnknown2)
                        {
                            int totalCount = 0;
                            for (int i = 0; i < section.VertexCount; i++)
                                totalCount += reader.ReadUShort();
                            sectionCounts.Add(totalCount);
                        }
                    }
                }
                reader.Pad(16);
                foreach (MeshSetLod lod in lods)
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

            //// subset categories
            //reader.Pad(16);
            //foreach (MeshSetLod lod in lods)
            //{
            //    for (int i = 0; i < lod.CategorySubsetIndices.Count; i++)
            //    {
            //        for (int j = 0; j < lod.CategorySubsetIndices[i].Count; j++)
            //            lod.CategorySubsetIndices[i][j] = reader.ReadByte();
            //    }
            //}

            // adjacency buffer
            reader.Pad(16);
            foreach (MeshSetLod lod in lods)
                reader.Position += lod.AdjacencyBufferSize;

            // lod bone / part data
            //reader.Pad(16);
            //foreach (MeshSetLod lod in lods)
            //{
            //    // Fifa 17/18/19, SWBF2, Madden
            //    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 ||
            //        ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            //    {
            //        // no padding between entries
            //        if (lod.Type == MeshType.MeshType_Skinned)
            //            reader.Position += (lod.BoneCount * sizeof(int));
            //        else if (lod.Type == MeshType.MeshType_Composite)
            //            reader.Position += (lod.Sections.Count * 0x18);
            //    }
            //    else
            //    {
            //        // All others
            //        if (lod.Type == MeshType.MeshType_Skinned)
            //        {
            //            reader.Position += (lod.BoneCount * sizeof(int));
            //            if (lod.BoneShortNameArray.Count != 0)
            //                reader.Position += (lod.BoneCount * sizeof(int));
            //        }
            //        else if (lod.Type == MeshType.MeshType_Composite)
            //        {
            //            reader.Position += (lod.PartCount * Marshal.SizeOf<AxisAlignedBox>()) + (lod.PartCount * Marshal.SizeOf<LinearTransform>());
            //            reader.Position += (lod.Sections.Count * 0x18);
            //        }
            //    }
            //}

            //int actualCount = boneCount;
            ////if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            ////    actualCount = bonePartCount;

            //// Bones/Parts
            //if (meshType == MeshType.MeshType_Skinned)
            //{
            //    reader.Pad(16);
            //    for (int i = 0; i < actualCount; i++)
            //        boneIndices.Add(reader.ReadUShort());
            //    reader.Pad(16);
            //    for (int i = 0; i < actualCount; i++)
            //        boneBoundingBoxes.Add(reader.ReadAxisAlignedBox());
            //}
            //else if (meshType == MeshType.MeshType_Composite)
            //{
            //    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
            //    {
            //        reader.Pad(16);
            //        for (int i = 0; i < bonePartCount; i++)
            //            partTransforms.Add(reader.ReadLinearTransform());
            //    }
            //    reader.Pad(16);
            //    for (int i = 0; i < bonePartCount; i++)
            //        partBoundingBoxes.Add(reader.ReadAxisAlignedBox());
            //}

            //inline data
            uint inlineDataOffset = BitConverter.ToUInt32(resMeta, 0);
            uint inlineDataSize = BitConverter.ToUInt32(resMeta, 4);
            if (inlineDataOffset != 0 && inlineDataSize != 0)
            {
                reader.Position = inlineDataOffset;
                foreach (MeshSetLod lod in lods)
                    lod.ReadInlineData(reader);
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
                foreach (var lod in lods)
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
                return false;

            foreach (var lt in partTransformList)
            {
                if (lt == nt)
                    return false;
            }

            return true;
        }

        private void PreProcess(MeshContainer meshContainer)
        {
            uint inlineDataOffset = 0;
            foreach (var lod in lods)
                lod.PreProcess(meshContainer, ref inlineDataOffset);

            foreach (var lod in lods)
                meshContainer.AddRelocPtr("LOD", lod);

            meshContainer.AddString(fullname, fullname.Replace(name, ""), true);
            meshContainer.AddString(name, name);

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat)
            {
                if (meshType == MeshType.MeshType_Skinned)
                {
                    if (boneIndices.Count != 0)
                        meshContainer.AddRelocPtr("BONEINDICES", boneIndices);
                    if (boneBoundingBoxes.Count != 0)
                        meshContainer.AddRelocPtr("BONEBBOXES", boneBoundingBoxes);
                }
                else if (meshType == MeshType.MeshType_Composite)
                {
                    //TODO: Test composite import
                    if (HasPartTransforms(partTransforms))
                        meshContainer.AddRelocPtr("PARTTRANSFORM", partTransforms);
                    if (partBoundingBoxes.Count != 0)
                        meshContainer.AddRelocPtr("PARTBBOXES", partBoundingBoxes);
                }
            }
        }

        private void Process(NativeWriter writer, MeshContainer meshContainer)
        {
            // header
            writer.Write(boundingBox);
            for (int i = 0; i < MaxLodCount; i++)
            {
                if (i < lods.Count)
                    meshContainer.WriteRelocPtr("LOD", lods[i], writer);
                else
                    writer.Write((ulong)0);
            }
            meshContainer.WriteRelocPtr("STR", fullname, writer);
            meshContainer.WriteRelocPtr("STR", name, writer);
            writer.Write(nameHash);
            writer.Write((uint)meshType);
            writer.Write((uint)flags);
            foreach (uint value in unknownUInts)
                writer.Write(value);
            writer.Write((ushort)lods.Count);

            ushort sectionCount = 0;
            foreach (var lod in lods)
                sectionCount += (ushort)lod.Sections.Count;

            writer.Write(sectionCount);

            if (meshType != MeshType.MeshType_Rigid && (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat))
            {
                writer.Write(bonePartCount);
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20)
                    writer.Write(boneCount);
                else
                    writer.Write((int)boneCount);

                if (meshType == MeshType.MeshType_Skinned)
                {
                    if (boneIndices.Count != 0)
                        meshContainer.WriteRelocPtr("BONEINDICES", boneIndices, writer);
                    if (boneBoundingBoxes.Count != 0)
                        meshContainer.WriteRelocPtr("BONEBBOXES", boneBoundingBoxes, writer);
                }
                else if (meshType == MeshType.MeshType_Composite)
                {
                    //TODO: Test composite import
                    if (HasPartTransforms(partTransforms))
                        meshContainer.WriteRelocPtr("PARTTRANSFORM", partTransforms, writer);
                    if (partBoundingBoxes.Count != 0)
                        meshContainer.WriteRelocPtr("PARTBBOXES", partBoundingBoxes, writer);
                }
            }

            writer.WritePadding(16);

            Debug.Assert(writer.Position == HeaderSize);

            // lods
            foreach (var lod in lods)
            {
                meshContainer.AddOffset("LOD", lod, writer);
                lod.Process(writer, meshContainer);
            }

            // sections
            foreach (var lod in lods)
            {
                meshContainer.AddOffset("SECTION", lod.Sections, writer);
                foreach (var section in lod.Sections)
                    section.Process(writer, meshContainer);
            }
            writer.WritePadding(16);

            // section bone list
            foreach (var lod in lods)
            {
                foreach (var section in lod.Sections)
                {
                    if (section.BoneList.Count > 0)
                    {
                        meshContainer.AddOffset("BONELIST", section.BoneList, writer);
                        foreach (var idx in section.BoneList)
                            writer.Write(idx);
                    }
                }
            }
            writer.WritePadding(16);

            // strings
            meshContainer.WriteStrings(writer);
            
            // Fifa 17/18 unkown block
            
            writer.WritePadding(16);

            // categories
            foreach (var lod in lods)
            {
                foreach (var category in lod.CategorySubsetIndices)
                {
                    meshContainer.AddOffset("SUBSET", category, writer);
                    writer.Write(category.ToArray());
                }
            }
            writer.WritePadding(16);

            // LOD bones
            foreach (var lod in lods)
            {
                if (meshType == MeshType.MeshType_Skinned)
                {
                    meshContainer.AddOffset("BONES", lod.BoneIndexArray, writer);
                    foreach (var idx in lod.BoneIndexArray)
                        writer.Write(idx);

                    if (lod.BoneShortNameArray.Count != 0 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 &&
                        ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                    {
                        meshContainer.AddOffset("BONESNAMES", lod.BoneShortNameArray, writer);
                        foreach (var idx in lod.BoneShortNameArray)
                            writer.Write(idx);
                    }
                }
                else if (meshType == MeshType.MeshType_Composite)
                {
                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa17 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa18 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden19 &&
                        ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsSquadrons)
                    {
                        if (lod.PartBoundingBoxes.Count != 0)
                        {
                            meshContainer.AddOffset("PARTBBOXES", lod.PartBoundingBoxes, writer);
                            foreach (var bbox in partBoundingBoxes)
                                writer.Write(bbox);
                            writer.WritePadding(16);
                        }
                        if (HasPartTransforms(lod.PartTransforms))
                        {
                            meshContainer.AddOffset("PARTTRANSFORM", lod.PartTransforms, writer);
                            foreach (var lt in partTransforms)
                                writer.Write(lt);
                            writer.WritePadding(16);
                        }
                    }
                    if (lod.PartIndices.Count != 0)
                    {
                        meshContainer.AddOffset("PARTINDICES", lod.PartIndices, writer);
                        foreach (var section in lod.PartIndices)
                        {
                            // TODO: partIndices writing
                        }
                    }
                }
            }
            writer.WritePadding(16);
            
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda ||
                    ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 ||
                    ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            {
                if (meshType == MeshType.MeshType_Skinned)
                {
                    if (boneIndices.Count != 0)
                    {
                        meshContainer.AddOffset("BONEINDICES", boneIndices, writer);
                        foreach (var idx in boneIndices)
                            writer.Write(idx);
                        writer.WritePadding(16);
                    }
                    if (boneBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddOffset("BONEBBOXES", boneBoundingBoxes, writer);
                        foreach (var bbox in boneBoundingBoxes)
                            writer.Write(bbox);
                        writer.WritePadding(16);
                    }
                }
                else if (meshType == MeshType.MeshType_Composite)
                {
                    if (HasPartTransforms(partTransforms))
                    {
                        meshContainer.AddOffset("PARTTRANSFORM", partTransforms, writer);
                        foreach (var lt in partTransforms)
                            writer.Write(lt);
                        writer.WritePadding(16);
                    }

                    if (partBoundingBoxes.Count != 0)
                    {
                        meshContainer.AddOffset("PARTBBOXES", partBoundingBoxes, writer);
                        foreach (var bbox in partBoundingBoxes)
                            writer.Write(bbox);
                        writer.WritePadding(16);
                    }
                }
            }
        }
    }
    #endregion

    #endregion
}
