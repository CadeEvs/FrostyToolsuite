using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.IO;
using FrostySdk;
using Frosty.Core.Mod;
using Frosty.Core.IO;
using FrostySdk.Managers.Entries;
using LevelEditorPlugin.Resources.Hkx;

namespace LevelEditorPlugin.Resources
{
    namespace Hkx
    {
        public class hkBaseClass
        {
            public long Offset;

            public virtual void Serialize(NativeReader reader, HkxHeader header)
            {
            }
        }

        public class hkpStaticCompoundShape : hkBaseClass
        {
            public class hkpInstance
            {
                public Vector3 Translation { get; set; }
                public Vector4 Rotation { get; set; }
                public Vector3 Scale { get; set; }
                public uint FilterInfo { get; set; }
                public uint ChildFilterInfoMask { get; set; }
                public uint UserData { get; set; }

                public long DataOffset { get; set; }

                public void Serialize(NativeReader reader)
                {
                    DataOffset = reader.Position;
                    Translation = new Vector3(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                    );
                    reader.ReadFloat();

                    Rotation = new Vector4(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                    );

                    Scale = new Vector3(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                    );
                    reader.ReadFloat();

                    reader.ReadUInt();
                    FilterInfo = reader.ReadUInt();
                    ChildFilterInfoMask = reader.ReadUInt();
                    UserData = reader.ReadUInt();
                }
            }

            public class hkpTreeNode
            {
                public List<int> XYZ { get; set; }
                public int HiData { get; set; }
                public int LoData { get; set; }
            }

            public class hkpDomain
            {
                public Vector4 Min { get; set; }
                public Vector4 Max { get; set; }

                public void Serialize(NativeReader reader)
                {
                    Min = new Vector4(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                        );
                    Max = new Vector4(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                        );
                }
            }

            public class hkpTree
            {
                public List<hkpTreeNode> Nodes { get; set; }
                public hkpDomain Domain { get; set; }

                public void Serialize(NativeReader reader)
                {
                    Nodes = new List<hkpTreeNode>();
                    Domain = new hkpDomain();

                    int NumTreeNodes = reader.ReadInt();
                    reader.BaseStream.Seek(8, SeekOrigin.Current);

                    Domain.Serialize(reader);
                }
            }

            public byte DispatchType { get; set; }
            public byte BitsPerKey { get; set; }
            public byte ShapeInfoCodecType { get; set; }
            public uint UserData { get; set; }
            public byte BvTreeType { get; set; }
            public byte NumBitsForChildShapeKey { get; set; }
            public List<hkpInstance> Instances { get; set; }
            public hkpTree Tree { get; set; }

            public hkpStaticCompoundShape()
            {
            }

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                reader.BaseStream.Seek(9, SeekOrigin.Current);
                DispatchType = reader.ReadByte();
                BitsPerKey = reader.ReadByte();
                ShapeInfoCodecType = reader.ReadByte();
                UserData = reader.ReadUInt();
                BvTreeType = reader.ReadByte();
                reader.BaseStream.Seek(7, SeekOrigin.Current);
                NumBitsForChildShapeKey = reader.ReadByte();
                reader.BaseStream.Seek(11, SeekOrigin.Current);
                int NumInstances = reader.ReadInt();
                reader.BaseStream.Seek(28, SeekOrigin.Current);

                Tree = new hkpTree();
                Tree.Serialize(reader);

                reader.BaseStream.Seek(16, SeekOrigin.Current);
                Instances = new List<hkpInstance>();

                for (int i = 0; i < NumInstances; i++)
                {
                    hkpInstance CurInstance = new hkpInstance();
                    CurInstance.Serialize(reader);

                    Instances.Add(CurInstance);
                }
            }
        }

        public class hknpShape : hkBaseClass
        {
            public int unknown;
            public hkRefCountedProperties Properties;

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                reader.Position += 0x11;
                unknown = reader.ReadByte();
                reader.Position += 0x0E;
                Properties = header.ReadObject(reader) as hkRefCountedProperties;
            }
        }

        public class hknpCapsuleShape : hknpShape
        {
        }
        public class hknpConvexPolytopeShape : hknpShape
        {
            public List<Vector4> Vertices = new List<Vector4>();
            public List<Vector4> Planes = new List<Vector4>();
            public List<byte[]> Faces = new List<byte[]>();
            public List<byte> Indices = new List<byte>();

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                base.Serialize(reader, header);
                reader.Position += 0x08;

                int numVertices = header.BeginRelArrayRead(reader);
                for (int i = 0; i < numVertices; i++)
                {
                    Vertices.Add(new Vector4(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                        ));
                }
                header.EndRelArrayRead(reader);

                reader.Position += 0x0c;

                int numPlanes = header.BeginRelArrayRead(reader);
                for (int i = 0; i < numPlanes; i++)
                {
                    Planes.Add(new Vector4(
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat(),
                        reader.ReadFloat()
                        ));
                }
                header.EndRelArrayRead(reader);

                int numFaces = header.BeginRelArrayRead(reader);
                for (int i = 0; i < numFaces; i++)
                {
                    Faces.Add(new byte[]
                    {
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte()
                    });
                }
                header.EndRelArrayRead(reader);

                int numIndices = header.BeginRelArrayRead(reader);
                for (int i = 0; i < numIndices; i++)
                {
                    Indices.Add(reader.ReadByte());
                }
                header.EndRelArrayRead(reader);
            }
        }
        public class hknpSphereShape : hknpShape
        {
        }
        public class hknpCompressedMeshShapeData : hkBaseClass
        {
        }
        public class hknpCompressedMeshShape : hknpShape
        {
        }

        public class hknpStaticCompoundShape : hknpShape
        {
            public class hknpInstance
            {
                public Matrix Transform;
                public Matrix Rotation;
                public Vector3 Translation;
                public Vector3 Scale;
                public float Test;
                public hkBaseClass shape;
                //public ushort shapeTag;
                //public ushort destructionTag;
                public uint[] unknowns = new uint[10];
                public long DataOffset { get; set; }

                public uint tmp1;
                public uint tmp2;
                public uint tmp3;
                public uint tmp4;
                public uint tmp5;

                public void Serialize(NativeReader reader, HkxHeader header)
                {
                    DataOffset = reader.Position;

                    Matrix m = new Matrix();
                    m.M11 = reader.ReadFloat(); m.M12 = reader.ReadFloat(); m.M13 = reader.ReadFloat(); tmp1 = reader.ReadUInt();
                    m.M21 = reader.ReadFloat(); m.M22 = reader.ReadFloat(); m.M23 = reader.ReadFloat(); tmp2 = reader.ReadUInt();
                    m.M31 = reader.ReadFloat(); m.M32 = reader.ReadFloat(); m.M33 = reader.ReadFloat(); tmp3 = reader.ReadUInt();
                    m.M41 = reader.ReadFloat(); m.M42 = reader.ReadFloat(); m.M43 = reader.ReadFloat(); tmp4 = reader.ReadUInt();
                    Transform = m;
                    Rotation = m;//Quaternion.RotationMatrix(m);

                    //Translation = new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
                    //tmp4 = reader.ReadFloat();

                    Scale = new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
                    tmp5 = reader.ReadUInt();

                    shape = header.ReadObject(reader);
                    //shapeTag = reader.ReadUShort();
                    //destructionTag = reader.ReadUShort();

                    for (int i = 0; i < 10; i++)
                        unknowns[i] = reader.ReadUInt();
                }
            }

            public List<hknpInstance> Instances = new List<hknpInstance>();
            public Vector4 Unknown1;
            public Vector4 Unknown2;
            public hknpStaticCompoundShapeData ShapeData;

            public hknpStaticCompoundShape()
            {
            }

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                base.Serialize(reader, header);

                reader.Position += 0x38;
                long arrayOffset = header.ReadArray(reader);
                int numInstances = reader.ReadInt();

                reader.Position += 0x14;
                Unknown1 = new Vector4(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );
                Unknown2 = new Vector4(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );

                reader.Position += 0x20;
                ShapeData = header.ReadObject(reader) as hknpStaticCompoundShapeData;

                reader.Position = arrayOffset;
                for (int i = 0; i < numInstances; i++)
                {
                    hknpInstance inst = new hknpInstance();
                    inst.Serialize(reader, header);

                    Instances.Add(inst);
                }
            }
        }

        public class hknpStaticCompoundShapeData : hkBaseClass
        {
            public class TreeNode
            {
                public ushort Index;
                public byte X;
                public byte Y;
                public byte Z;

                public bool HasChildren;
                public List<TreeNode> Children = new List<TreeNode>();

                public TreeNode(byte[] data)
                {
                    X = data[0];
                    Y = data[1];
                    Z = data[2];
                    HasChildren = data[3] != 0;
                    Index = BitConverter.ToUInt16(data, 4);
                }
            }

            public Vector4 Unknown1;
            public Vector4 Unknown2;
            public List<byte[]> Unknowns = new List<byte[]>();
            public long ArrayOffset;
            public TreeNode RootNode;

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                base.Serialize(reader, header);
                reader.Position += 0x10;

                ArrayOffset = header.ReadArray(reader);
                int numItems = reader.ReadInt();
                reader.Position += 4;

                Unknown1 = new Vector4(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );
                Unknown2 = new Vector4(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );

                reader.Position = ArrayOffset;
                for (int i = 0; i < numItems; i++)
                {
                    Unknowns.Add(reader.ReadBytes(6));
                }
            }
        }

        public class hkRefCountedProperties : hkBaseClass
        {
            public List<hkBaseClass> Properties = new List<hkBaseClass>();

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                base.Serialize(reader, header);

                long arrayOffset = header.ReadArray(reader);
                int numItems = reader.ReadInt();

                reader.Position = arrayOffset;
                for (int i = 0; i < numItems; i++)
                {
                    Properties.Add(header.ReadObject(reader));
                }
            }
        }

        public class hknpShapeMassProperties : hkBaseClass
        {
            public Vector4 Unknown1;
            public Vector4 Unknown2;

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                base.Serialize(reader, header);
                reader.Position += 0x10;

                Unknown1 = new Vector4(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );
                Unknown2 = new Vector4(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );
            }
        }

        public class HavokPhysicsContainer : hkBaseClass
        {
            public List<hkBaseClass> Objects = new List<hkBaseClass>();

            public override void Serialize(NativeReader reader, HkxHeader header)
            {
                base.Serialize(reader, header);
                reader.Position += 0x10;

                long arrayOffset = header.ReadArray(reader);
                int numItems = reader.ReadInt();

                reader.Position = arrayOffset;
                for (int i = 0; i < numItems; i++)
                {
                    Objects.Add(header.ReadObject(reader));
                }
            }
        }

        //public class hkRootLevelContainer : hkBaseClass
        //{
        //}
    }

    public static partial class ReaderUtils
    {
        
    }

    public class HkxHeader
    {
        public uint StartOffset;
        public uint Magic1;
        public uint Magic2;
        public uint UserTag;
        public uint FileVersion;
        public uint LayoutRules;
        public uint NumSections;
        public uint ContentsSectionIndex;
        public uint ContentsSectionOffset;
        public uint ContentsClassNameSectionIndex;
        public uint ContentsClassNameSectionOffset;
        public String ContentsVersion;
        public uint Flags;
        public uint Padding;
        public int Bits;

        public Dictionary<long, long> ArrayOffsets = new Dictionary<long, long>();
        public Dictionary<long, Hkx.hkBaseClass> ObjectOffsets = new Dictionary<long, Hkx.hkBaseClass>();

        private List<long> stackOffsets = new List<long>();

        public bool IsValid() { return (Magic1 == 0x57e0e057 && Magic2 == 0x10c0c010); }

        public void Serialize(NativeReader reader, int bits)
        {
            Bits = bits;
            Magic1 = reader.ReadUInt();
            if (Magic1 != 0x57e0e057)
            {
                do
                {
                    reader.BaseStream.Seek(-3, SeekOrigin.Current);
                    Magic1 = reader.ReadUInt();
                }
                while (Magic1 != 0x57e0e057);
            }

            StartOffset = (uint)reader.BaseStream.Position - 4;
            Magic2 = reader.ReadUInt();
            if (Magic2 == 0x10c0c010)
            {
                UserTag = reader.ReadUInt();
                FileVersion = reader.ReadUInt();
                LayoutRules = reader.ReadUInt();
                NumSections = reader.ReadUInt();
                ContentsSectionIndex = reader.ReadUInt();
                ContentsSectionOffset = reader.ReadUInt();
                ContentsClassNameSectionIndex = reader.ReadUInt();
                ContentsClassNameSectionOffset = reader.ReadUInt();
                ContentsVersion = reader.ReadTerminatedString(0xff);
                Flags = reader.ReadUInt();
                Padding = reader.ReadUInt();
            }
        }

        public Hkx.hkBaseClass ReadObject(NativeReader reader)
        {
            reader.ReadInt();
            if (Bits == 64)
                reader.ReadInt();

            if (!ObjectOffsets.ContainsKey(reader.Position - (Bits == 64 ? 8 : 4)))
                return null;
            return ObjectOffsets[reader.Position - (Bits == 64 ? 8 : 4)];
        }

        public long ReadArray(NativeReader reader)
        {
            long arrayOffset = ArrayOffsets[reader.Position];
            reader.ReadInt();
            if (Bits == 64)
                reader.ReadInt();

            return arrayOffset;
        }

        public int BeginRelArrayRead(NativeReader reader)
        {
            ushort numEntries = reader.ReadUShort();
            ushort offset = reader.ReadUShort();

            stackOffsets.Add(reader.Position);
            reader.Position += (offset - 4);

            return numEntries;
        }

        public void EndRelArrayRead(NativeReader reader)
        {
            reader.Position = stackOffsets[stackOffsets.Count - 1];
            stackOffsets.RemoveAt(stackOffsets.Count - 1);
        }
    }

    class HkxInstance
    {
        public class HkxClassDescriptor
        {
            public uint Signature;
            public byte Version;
            public String Name;

            public HkxClassDescriptor()
            {
            }

            public void Serialize(NativeReader reader)
            {
                Signature = reader.ReadUInt();
                Version = reader.ReadByte();
                Name = reader.ReadNullTerminatedString();
            }
        }

        public HkxHeader Header { get; set; }
        public List<Hkx.hkBaseClass> Objects => objList.Values.ToList();

        public int LocalFixupSize;
        public int GlobalFixupSize;
        public int AbsoluteDataStart;

        int virtualFixupsOffset;
        public int endOffset;

        Dictionary<ulong, HkxClassDescriptor> Classes = new Dictionary<ulong, HkxClassDescriptor>();
        Dictionary<long, Hkx.hkBaseClass> objList = new Dictionary<long, Hkx.hkBaseClass>();

        public void Serialize(NativeReader reader, int bits)
        {
            Header = new HkxHeader();
            Header.Serialize(reader, bits);

            int ClassSectionOffset = 0;
            int ClassSectionSize = 0;
            int TypesSectionOffset = 0;
            int exportsOffset = 0;
            int importsOffset = 0;
            int CurrentSection = 0;

            while (CurrentSection < Header.NumSections)
            {
                String BlockType = reader.ReadTerminatedString(0xff);
                if (BlockType.Contains("__classnames__"))
                {
                    ClassSectionOffset = reader.ReadInt();
                    reader.ReadLong();
                    ClassSectionSize = reader.ReadInt();
                    reader.BaseStream.Seek(12, SeekOrigin.Current);
                    CurrentSection++;
                }
                else if (BlockType.Contains("__types__"))
                {
                    TypesSectionOffset = reader.ReadInt();
                    reader.BaseStream.Seek(24, SeekOrigin.Current);
                    CurrentSection++;
                }
                else if (BlockType.Contains("__data__"))
                {
                    AbsoluteDataStart = reader.ReadInt();
                    LocalFixupSize = reader.ReadInt();
                    GlobalFixupSize = reader.ReadInt();
                    virtualFixupsOffset = reader.ReadInt();
                    exportsOffset = reader.ReadInt();
                    importsOffset = reader.ReadInt();
                    endOffset = reader.ReadInt();
                    CurrentSection++;
                }
            }

            reader.Position = Header.StartOffset + ClassSectionOffset;
            while (reader.BaseStream.Position < (Header.StartOffset + ClassSectionOffset + ClassSectionSize))
            {
                ulong ClassOffset = (ulong)(reader.BaseStream.Position - (Header.StartOffset + ClassSectionOffset));
                HkxClassDescriptor Class = new HkxClassDescriptor();
                Class.Serialize(reader);

                Classes.Add(ClassOffset + 5, Class);
            }

            reader.Position = Header.StartOffset + AbsoluteDataStart + virtualFixupsOffset;

            int TotalSize = (int)Header.StartOffset + AbsoluteDataStart + endOffset;

            while (reader.BaseStream.Position < TotalSize)
            {
                long DataOffset = reader.ReadLong();
                int ClassType = reader.ReadInt();

                if (ClassType != -1 && reader.BaseStream.Position < TotalSize)
                {
                    HkxClassDescriptor CurrentClassDescriptor = Classes[(ulong)ClassType];
                    Type type = Type.GetType("LevelEditorPlugin.Resources.Hkx." + CurrentClassDescriptor.Name);

                    if (type != null)
                    {
                        Hkx.hkBaseClass BaseClass = (Hkx.hkBaseClass)Activator.CreateInstance(type);
                        BaseClass.Offset = Header.StartOffset + AbsoluteDataStart + DataOffset;
                        objList.Add(BaseClass.Offset, BaseClass);
                    }
                }
            }

            reader.Position = Header.StartOffset + AbsoluteDataStart + endOffset;
        }

        public void ReadObjects(NativeReader reader)
        {
            foreach (long offset in objList.Keys)
            {
                Hkx.hkBaseClass value = objList[offset];
                if (value != null)
                {
                    reader.Position = offset;
                    value.Serialize(reader, Header);
                }
            }
        }

        public Hkx.hkBaseClass GetObject(long offset)
        {
            if (!objList.ContainsKey(offset))
                return null;
            return objList[offset];
        }
    }

    public class HavokPhysicsData : Resource
    {
        public class HavokPhysicsDataModifiedResource : BaseModifiedResource
        {
            private Dictionary<int, Matrix> modifiedTransforms = new Dictionary<int, Matrix>();

            public HavokPhysicsDataModifiedResource(ResAssetEntry inEntry)
                : base(inEntry)
            {
            }

            public bool GetTransform(int index, out Matrix outTransform)
            {
                outTransform = Matrix.Identity;
                if (!modifiedTransforms.ContainsKey(index))
                    return false;

                outTransform = modifiedTransforms[index];
                return true;
            }

            public void SetTransform(int index, Matrix transform)
            {
                if (!modifiedTransforms.ContainsKey(index))
                    modifiedTransforms.Add(index, Matrix.Identity);
                modifiedTransforms[index] = transform;
            }

            public void ClearTransform(int index)
            {
                if (!modifiedTransforms.ContainsKey(index))
                    return;
                modifiedTransforms.Remove(index);
            }

            public override void SaveInternal(NativeWriter writer)
            {
                writer.Write(modifiedTransforms.Count);
                foreach (KeyValuePair<int, Matrix> kvp in modifiedTransforms)
                {
                    writer.Write(kvp.Key);

                    writer.Write(kvp.Value.M11);
                    writer.Write(kvp.Value.M12);
                    writer.Write(kvp.Value.M13);
                    writer.Write(kvp.Value.M14);

                    writer.Write(kvp.Value.M21);
                    writer.Write(kvp.Value.M22);
                    writer.Write(kvp.Value.M23);
                    writer.Write(kvp.Value.M24);

                    writer.Write(kvp.Value.M31);
                    writer.Write(kvp.Value.M32);
                    writer.Write(kvp.Value.M33);
                    writer.Write(kvp.Value.M34);

                    writer.Write(kvp.Value.M41);
                    writer.Write(kvp.Value.M42);
                    writer.Write(kvp.Value.M43);
                    writer.Write(kvp.Value.M44);
                }
            }

            public override void ReadInternal(NativeReader reader)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    int key = reader.ReadInt();
                    Matrix value = new Matrix(
                        reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                        reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                        reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                        reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()
                        );

                    modifiedTransforms.Add(key, value);
                }
            }
        }

        Hkx.hknpStaticCompoundShape RootShape => (inst64.Objects[0] as Hkx.HavokPhysicsContainer).Objects[0] as Hkx.hknpStaticCompoundShape;

        private int partCount;
        private List<Vec3> partTranslations = new List<Vec3>();
        private List<AxisAlignedBox> localAabbs = new List<AxisAlignedBox>();
        private List<byte> materialIndices = new List<byte>();
        private List<uint> materialFlagsAndIndices = new List<uint>();
        private List<ushort> unknownIndices = new List<ushort>();
        private byte materialCountUsed;
        private byte highestMaterialIndex;

        private HkxInstance inst32;
        private HkxInstance inst64;

        private byte[] resData;
        private ResAssetEntry resEntry;

        private HavokPhysicsDataModifiedResource modifiedData;

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource inModifiedData)
        {
            base.Read(reader, am, entry, inModifiedData);

            modifiedData = (inModifiedData != null)
                ? inModifiedData as HavokPhysicsDataModifiedResource
                : new HavokPhysicsDataModifiedResource(entry);

            resData = reader.ReadToEnd();
            resEntry = entry;

            reader.Position = 0;

            {
                partCount = reader.ReadInt();

                int partTranslationsCount = reader.ReadInt();
                reader.ReadLong();
                int localAabbsCount = reader.ReadInt();
                reader.ReadLong();
                int materialIndicesCount = reader.ReadInt();
                reader.ReadLong();
                int materialFlagsAndIndicesCount = reader.ReadInt();
                reader.ReadLong();

                int unknownCount = 0;
                if (ProfilesLibrary.DataVersion != 20141118 && ProfilesLibrary.DataVersion != 20141117)
                {
                    unknownCount = reader.ReadInt();
                    reader.ReadLong();
                }

                materialCountUsed = reader.ReadByte();
                highestMaterialIndex = reader.ReadByte();
                reader.ReadBytes(2);
                reader.Pad(16);

                for (int i = 0; i < partTranslationsCount; i++)
                    partTranslations.Add(reader.ReadVec3());
                reader.Pad(16);
                for (int i = 0; i < localAabbsCount; i++)
                    localAabbs.Add(reader.ReadAxisAlignedBox());
                reader.Pad(16);
                for (int i = 0; i < materialIndicesCount; i++)
                    materialIndices.Add(reader.ReadByte());
                reader.Pad(16);
                for (int i = 0; i < materialFlagsAndIndicesCount; i++)
                    materialFlagsAndIndices.Add(reader.ReadUInt());
                reader.Pad(16);

                if (ProfilesLibrary.DataVersion != 20141118 && ProfilesLibrary.DataVersion != 20141117)
                {
                    for (int i = 0; i < unknownCount; i++)
                        unknownIndices.Add(reader.ReadUShort());
                    reader.Pad(16);
                }
            }

            inst32 = new HkxInstance();
            inst32.Serialize(reader, 32);

            if (ProfilesLibrary.DataVersion != 20140225)
            {
                inst64 = new HkxInstance();
                inst64.Serialize(reader, 64);
            }

            int fixupSize32 = reader.ReadInt();
            int fixupSize64 = reader.ReadInt();

            long pos = reader.Position;
            long fileOffset = inst32.Header.StartOffset + inst32.AbsoluteDataStart;

            while (reader.Position < (pos + inst32.GlobalFixupSize))
            {
                int offset = reader.ReadInt();
                int objOffset = reader.ReadInt();

                if (offset != -1)
                    inst32.Header.ArrayOffsets.Add(fileOffset + offset, fileOffset + objOffset);
            }
            while (reader.Position < (pos + inst32.LocalFixupSize - inst32.GlobalFixupSize))
            {
                int offset = reader.ReadInt();
                int unknown = reader.ReadInt();
                int objOffset = reader.ReadInt();

                inst32.Header.ObjectOffsets.Add(fileOffset + offset, inst32.GetObject(fileOffset + objOffset));
            }

            reader.Position = (pos + fixupSize32);
            pos = reader.Position;

            fileOffset = inst64.Header.StartOffset + inst64.AbsoluteDataStart;

            while (reader.Position < (pos + inst64.GlobalFixupSize))
            {
                int offset = reader.ReadInt();
                int objOffset = reader.ReadInt();

                if (offset != -1)
                    inst64.Header.ArrayOffsets.Add(fileOffset + offset, fileOffset + objOffset);
            }
            pos = reader.Position;
            while (reader.Position < (pos + (inst64.LocalFixupSize - inst64.GlobalFixupSize)))
            {
                int offset = reader.ReadInt();
                int unknown = reader.ReadInt();
                int objOffset = reader.ReadInt();

                inst64.Header.ObjectOffsets.Add(fileOffset + offset, inst64.GetObject(fileOffset + objOffset));
            }

            //inst32.ReadObjects(reader);
            inst64.ReadObjects(reader);
        }

        public void UpdateData(int index, Matrix newTransform)
        {
            modifiedData.SetTransform(index, newTransform);
            modifiedData.UpdateData(this);
        }

        public void UndoData(int index, Matrix prevTransform)
        {
            // @todo: Figure out when to clear unmodified transforms, as this will continue to store transforms of meshes that
            //        were moved and then reverted

            modifiedData.SetTransform(index, prevTransform);
            modifiedData.UndoData(this);
        }

        public override byte[] SaveBytes()
        {
            MemoryStream ms = new MemoryStream();
            using (NativeWriter writer = new NativeWriter(ms))
            {
                writer.Write(resData);

                // write 32bit
                //{
                //    int index = 0;
                //    foreach (var instance in (inst32.Objects[0] as Hkx.hknpStaticCompoundShape).Instances)
                //    {
                //        Matrix transform;
                //        if (modifiedData.GetTransform(index, out transform))
                //        {
                //            var shape = instance.shape as Hkx.hknpShape;
                //            if (shape != null && (shape.unknown & 0x10) != 0)
                //            {
                //                transform = Matrix.Scaling(-1, 1, 1) * transform;
                //            }

                //            Vector3 scale;
                //            Vector3 translation;
                //            Quaternion rotation;
                //            Matrix rotationMatrix;

                //            transform.Decompose(out scale, out rotation, out translation);
                //            rotationMatrix = Matrix.RotationQuaternion(rotation);

                //            writer.Position = instance.DataOffset;

                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M11);
                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M12);
                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M13);
                //            writer.BaseStream.Position += 4; //writer.Write(instance.tmp1);

                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M21);
                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M22);
                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M23);
                //            writer.BaseStream.Position += 4; //writer.Write(instance.tmp2);

                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M31);
                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M32);
                //            writer.BaseStream.Position += 4; //writer.Write(rotationMatrix.M33);
                //            writer.BaseStream.Position += 4; //writer.Write(instance.tmp3);

                //            writer.Write(translation.X);
                //            writer.Write(translation.Y);
                //            writer.Write(translation.Z);
                //            writer.Write(instance.tmp4);

                //            writer.BaseStream.Position += 4; //writer.Write(scale.X);
                //            writer.BaseStream.Position += 4; //writer.Write(scale.Y);
                //            writer.BaseStream.Position += 4; //writer.Write(scale.Z);
                //        }
                //        index++;
                //    }
                //}

                // write 64bit
                {
                    foreach (hkBaseClass obj in (inst64.Objects[0] as Hkx.HavokPhysicsContainer).Objects)
                    {
                        // reset the static compound shape data tree, this is a hack and is sure to come with some
                        // performance implications, but it works for now.

                        hknpStaticCompoundShape staticCompoundShape = obj as Hkx.hknpStaticCompoundShape;
                        if (staticCompoundShape.ShapeData != null)
                        {
                            writer.Position = staticCompoundShape.ShapeData.ArrayOffset;
                            for (int i = 0; i < staticCompoundShape.ShapeData.Unknowns.Count; i++)
                            {
                                writer.Write(new byte[] { 0x00, 0x00, 0x00 });
                                writer.BaseStream.Position += 3;
                            }
                        }

                        int index = 0;
                        foreach (hknpStaticCompoundShape.hknpInstance instance in (obj as Hkx.hknpStaticCompoundShape).Instances)
                        {
                            Matrix transform;
                            if (modifiedData.GetTransform(index, out transform))
                            {
                                hknpShape shape = instance.shape as Hkx.hknpShape;
                                if (shape != null && (shape.unknown & 0x10) != 0)
                                {
                                    transform = Matrix.Scaling(-1, 1, 1) * transform;
                                }

                                Vector3 scale;
                                Vector3 translation;
                                Quaternion rotation;
                                Matrix rotationMatrix;

                                transform.Decompose(out scale, out rotation, out translation);
                                rotationMatrix = Matrix.RotationQuaternion(rotation);

                                writer.Position = instance.DataOffset;

                                writer.Write(rotationMatrix.M11);
                                writer.Write(rotationMatrix.M12);
                                writer.Write(rotationMatrix.M13);
                                writer.BaseStream.Position += 4;

                                writer.Write(rotationMatrix.M21);
                                writer.Write(rotationMatrix.M22);
                                writer.Write(rotationMatrix.M23);
                                writer.BaseStream.Position += 4;

                                writer.Write(rotationMatrix.M31);
                                writer.Write(rotationMatrix.M32);
                                writer.Write(rotationMatrix.M33);
                                writer.BaseStream.Position += 4;

                                writer.Write(translation.X);
                                writer.Write(translation.Y);
                                writer.Write(translation.Z);
                                writer.BaseStream.Position += 4;

                                writer.Write(scale.X);
                                writer.Write(scale.Y);
                                writer.Write(scale.Z);
                            }
                            index++;
                        }
                    }
                }
            }

            byte[] outBuffer = ms.ToArray();
            ms.Dispose();

            return outBuffer;
        }

        public Matrix GetTransform(int index)
        {
            Matrix outTransform;
            if (modifiedData.GetTransform(index, out outTransform))
                return outTransform;

            Matrix preScale = Matrix.Identity;
            hknpStaticCompoundShape.hknpInstance instance = RootShape.Instances[(int)index];
            hknpShape shape = instance.shape as Hkx.hknpShape;

            if (shape != null && (shape.unknown & 0x10) != 0)
            {
                preScale = Matrix.Scaling(-1, 1, 1);
            }

            return
                preScale *
                Matrix.Scaling(instance.Scale) *
                instance.Transform;
                //instance.Rotation * 
                //Matrix.Translation(instance.Translation);
        }

        public string GetPhysicsShapeType(int index)
        {
            hknpStaticCompoundShape.hknpInstance instance = RootShape.Instances[index];
            return instance.shape.GetType().Name;
        }

        public override ModifiedResource SaveModifiedResource()
        {
            return modifiedData;
        }
    }

    public class HavokPhysicsDataActionHandler : ICustomActionHandler
    {
        public HandlerUsage Usage => HandlerUsage.Modify;

        private class HavokPhysicsDataResource : EditorModResource
        {
            public static uint Hash => (uint)Frosty.Hash.Fnv1.HashString(typeof(HavokPhysicsDataActionHandler).FullName);
            public override ModResourceType Type => ModResourceType.Res;

            private uint resType;
            private ulong resRid;
            private byte[] resMeta;

            public HavokPhysicsDataResource(ResAssetEntry entry, FrostyModWriter.Manifest manifest)
                : base(entry)
            {
                ModifiedResource md = entry.ModifiedEntry.DataObject as ModifiedResource;
                byte[] data = md.Save();

                name = entry.Name.ToLower();
                sha1 = Utils.GenerateSha1(data);
                resourceIndex = manifest.Add(sha1, data);
                size = data.Length;
                handlerHash = (int)Hash;

                resType = entry.ResType;
                resRid = entry.ResRid;
                resMeta = (entry.ModifiedEntry.ResMeta != null) ? entry.ModifiedEntry.ResMeta : entry.ResMeta;
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);

                writer.Write(resType);
                writer.Write(resRid);
                writer.Write(resMeta.Length);
                writer.Write(resMeta);
            }
        }

        public void SaveToMod(FrostyModWriter writer, AssetEntry entry)
        {
            writer.AddResource(new HavokPhysicsDataResource(entry as ResAssetEntry, writer.ResourceManifest));
        }

        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {
            return new List<string>();
        }

        public object Load(object existing, byte[] newData)
        {
            HavokPhysicsData.HavokPhysicsDataModifiedResource newPhysicsData = ModifiedResource.Read(newData) as HavokPhysicsData.HavokPhysicsDataModifiedResource;
            return newPhysicsData;
        }

        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            HavokPhysicsData.HavokPhysicsDataModifiedResource physicsData = data as HavokPhysicsData.HavokPhysicsDataModifiedResource;
            HavokPhysicsData havokData = am.GetResAs<HavokPhysicsData>(am.GetResEntry((origEntry as ResAssetEntry).ResRid), physicsData);

            byte[] uncompressedData = havokData.SaveBytes();

            origEntry.OriginalSize = uncompressedData.Length;
            outData = Utils.CompressFile(uncompressedData);

            origEntry.Size = outData.Length;
            origEntry.Sha1 = Utils.GenerateSha1(outData);
        }
    }
}
