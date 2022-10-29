using FrostySdk.Attributes;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;

#region -- Old ShaderBlocks --

// contains the old shaderblock classes used to retroactively fix
// projects containing these old shaderblocks

namespace MeshSetPlugin.Resources.Old
{
    public class ShaderBlockResource
    {
        public ulong Hash;
        public ulong ResourceId;

        public ShaderBlockResource(NativeReader reader)
        {
            Hash = reader.ReadULong();
        }

        public virtual Resources.ShaderBlockResource Convert()
        {
            return null;
        }
    }

    public class ShaderPersistentParamDbBlock : ShaderBlockResource
    {
        public List<ParameterEntry> Parameters = new List<ParameterEntry>();
        public ShaderPersistentParamDbBlock(NativeReader reader)
            : base(reader)
        {
            ulong offset = reader.ReadULong();
            int size = reader.ReadInt();

            reader.Pad(16);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Parameters.Add(new ParameterEntry(reader));
            }
        }

        public override Resources.ShaderBlockResource Convert()
        {
            Resources.ShaderPersistentParamDbBlock mpdb = new Resources.ShaderPersistentParamDbBlock
            {
                Hash = Hash,
                Parameters = Parameters,
                IsModified = true
            };

            return mpdb;
        }
    }

    public class MeshParamDbBlock : ShaderBlockResource
    {
        public Guid UnknownGuid;
        public int LodIndex;
        public List<ParameterEntry> Parameters = new List<ParameterEntry>();

        public MeshParamDbBlock(NativeReader reader)
            : base(reader)
        {
            ulong offset = reader.ReadULong();
            int size = reader.ReadInt();
            LodIndex = reader.ReadInt();
            UnknownGuid = reader.ReadGuid();

            reader.Pad(16);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Parameters.Add(new ParameterEntry(reader));
            }
        }

        public override Resources.ShaderBlockResource Convert()
        {
            Resources.MeshParamDbBlock mpdb = new Resources.MeshParamDbBlock
            {
                Hash = Hash,
                Parameters = Parameters,
                MeshAssetGuid = UnknownGuid,
                LodIndex = LodIndex,
                IsModified = true
            };

            return mpdb;
        }
    }
}

#endregion

namespace MeshSetPlugin.Resources
{
    public enum RvmLevelOfDetail
    {
        RvmLevelOfDetail_Low = 0,
        RvmLevelOfDetail_High = 1
    }
    public enum ShaderInstancingMethod
    {
        ShaderInstancingMethod_None = 0,
        ShaderInstancingMethod_ObjectTransform4x3Half = 1,
        ShaderInstancingMethod_ObjectTransform4x3InstanceData4x1Half = 2,
        ShaderInstancingMethod_ObjectTransform4x3InstanceData4x2Half = 3,
        ShaderInstancingMethod_WorldTransform4x3Float = 4,
        ShaderInstancingMethod_WorldTransform4x3FloatInstanceData4x2Half = 5,
        ShaderInstancingMethod_PrevWorldTransform4x3FloatInstanceData4x2Half = 6,
        ShaderInstancingMethod_ObjectTranslationScaleHalf = 7,
        ShaderInstancingMethod_ObjectTranslationScaleHalfInstanceData4x1Half = 8,
        ShaderInstancingMethod_ObjectTranslationScaleHalfInstanceData4x2Half = 9,
        ShaderInstancingMethod_PositionStream = 10,
        ShaderInstancingMethod_PositionTbnStream = 11,
        ShaderInstancingMethod_PrevPositionStream = 12,
        ShaderInstancingMethod_LinearMediaStreaming = 13,
        ShaderInstancingMethod_PositionStreamAux = 14,
        ShaderInstancingMethod_DxBuffer = 15,
        ShaderInstancingMethod_DxBufferInstanceData4x1Float = 16,
        ShaderInstancingMethod_DxBufferInstanceData4x2Float = 17,
        ShaderInstancingMethod_Manual = 18,
        ShaderInstancingMethodCount = 19
    }

    public class ShaderBlockHashException : Exception
    {
        public ShaderBlockHashException(int length)
            : base("A hashing exception has occurred. Length = " + length)
        {
        }
    }

    public class ShaderBlockResource
    {
        public int Index;
        public ulong Hash;
        public bool IsModified;

        public ShaderBlockResource()
        {
        }

        public virtual void Read(NativeReader reader, List<ShaderBlockResource> shaderBlockEntries)
        {
            Hash = reader.ReadULong();
            IsModified = false;
        }

        internal virtual void Save(NativeWriter writer, List<int> relocTable, out long startOffset)
        {
            startOffset = writer.Position;
            writer.Write(Hash);
            relocTable.Add((int)writer.Position);
        }

        public virtual void ChangeHash(uint value)
        {
            Hash ^= value;
        }

        public virtual void CalculateHash()
        {
        }
    }

    public class ShaderStaticParamDbBlock : ShaderBlockResource
    {
        public List<ShaderBlockResource> Resources = new List<ShaderBlockResource>();
        public ShaderStaticParamDbBlock()
        {
        }

        public override void Read(NativeReader reader, List<ShaderBlockResource> shaderBlockEntries)
        {
            base.Read(reader, shaderBlockEntries);

            long offset = reader.ReadLong();
            long size = reader.ReadLong();

            reader.Position = offset;

            for (long i = 0; i < size; i++)
            {
                int index = reader.ReadInt();
                Resources.Add(shaderBlockEntries[index]);
            }
        }

        internal override void Save(NativeWriter writer, List<int> relocTable, out long startOffset)
        {
            long offset = writer.Position;

            for (int i = 0; i < Resources.Count; i++)
            {
                writer.Write(Resources[i].Index);
            }

            writer.WritePadding(0x08);

            base.Save(writer, relocTable, out startOffset);

            writer.Write(offset);
            writer.Write((long)Resources.Count);
        }

        //public override void ChangeHash(uint value)
        //{
        //    base.ChangeHash(value);
        //    foreach (var res in Resources)
        //        res.ChangeHash(value);
        //}

        //public void ChangeHash(uint value, bool mesh = true)
        //{
        //    base.ChangeHash(value);
        //    foreach (var res in Resources)
        //    {
        //        if (res is ShaderStaticParamDbBlock staticBlock)
        //            staticBlock.ChangeHash(value, mesh);
        //        else if (mesh || !(res is MeshParamDbBlock))
        //            res.ChangeHash(value);
        //    }
        //}
    }

    public class ShaderPersistentParamDbBlock : ShaderBlockResource
    {
        public List<ParameterEntry> Parameters = new List<ParameterEntry>();
        public ShaderPersistentParamDbBlock()
        {
        }

        public override void Read(NativeReader reader, List<ShaderBlockResource> shaderBlockEntries)
        {
            base.Read(reader, shaderBlockEntries);

            long offset = reader.ReadLong();
            long size = reader.ReadLong();

            reader.Position = offset;
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Parameters.Add(new ParameterEntry(reader));
            }
        }

        public object GetParameterValue(string name)
        {
            object retVal = null;
            uint hash = (uint)Fnv1.HashString(name.ToLower());

            foreach (ParameterEntry param in Parameters)
            {
                if (param.NameHash == hash)
                {
                    retVal = param.GetValue();
                    break;
                }
            }

            return retVal;
        }

        public void SetParameterValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            uint hash = (uint)Fnv1.HashString(name.ToLower());
            foreach (ParameterEntry param in Parameters)
            {
                if (param.NameHash == hash)
                {
                    param.SetValue(value);
                    return;
                }
            }

            Parameters.Add(new ParameterEntry(name, value));
        }

        internal override void Save(NativeWriter writer, List<int> relocTable, out long startOffset)
        {
            long offset = writer.Position;

            writer.Write(Parameters.Count);
            foreach (var param in Parameters)
            {
                writer.Write(param.ToBytes());
            }

            long size = (writer.Position - offset);
            writer.WritePadding(0x08);

            base.Save(writer, relocTable, out startOffset);

            writer.Write(offset);
            writer.Write(size);
        }
    }

    public class MeshParamDbBlock : ShaderBlockResource
    {
        public Guid MeshAssetGuid;
        public int LodIndex;
        public List<ParameterEntry> Parameters = new List<ParameterEntry>();

        public MeshParamDbBlock()
        {
        }

        public override void Read(NativeReader reader, List<ShaderBlockResource> shaderBlockEntries)
        {
            base.Read(reader, shaderBlockEntries);

            long offset = reader.ReadLong();
            int size = reader.ReadInt();
            LodIndex = reader.ReadInt();
            MeshAssetGuid = reader.ReadGuid();

            reader.Position = offset;
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Parameters.Add(new ParameterEntry(reader));
            }
        }

        public object GetParameterValue(string name)
        {
            object retVal = null;
            uint hash = (uint)Fnv1.HashString(name.ToLower());

            foreach (ParameterEntry param in Parameters)
            {
                if (param.NameHash == hash)
                {
                    retVal = param.GetValue();
                    break;
                }
            }

            return retVal;
        }

        public void SetParameterValue(string name, object value)
        {
            uint hash = (uint)Fnv1.HashString(name.ToLower());
            foreach (ParameterEntry param in Parameters)
            {
                if (param.NameHash == hash)
                {
                    param.SetValue(value);
                    return;
                }
            }

            Parameters.Add(new ParameterEntry(name, value));
        }

        internal override void Save(NativeWriter writer, List<int> relocTable, out long startOffset)
        {
            long offset = writer.Position;

            writer.Write(Parameters.Count);
            foreach (var param in Parameters)
            {
                writer.Write(param.ToBytes());
            }

            int size = (int)(writer.Position - offset);
            writer.WritePadding(0x08);

            base.Save(writer, relocTable, out startOffset);

            writer.Write(offset);
            writer.Write(size);
            writer.Write(LodIndex);
            writer.Write(MeshAssetGuid);
        }
    }

    public class ShaderBlockMeshVariationEntry : ShaderBlockResource
    {
        public List<Guid> RvmShaderRefGuids = new List<Guid>();
        public List<int> RvmShaderRefInts = new List<int>();

        public ShaderBlockMeshVariationEntry()
        {
        }

        public override void Read(NativeReader reader, List<ShaderBlockResource> shaderBlockEntries)
        {
            base.Read(reader, shaderBlockEntries);

            long offset = reader.ReadLong();
            long count = reader.ReadLong();

            reader.Position = offset;

            for (long i = 0; i < count; i++)
            {
                RvmShaderRefGuids.Add(reader.ReadGuid());
                RvmShaderRefInts.Add(reader.ReadInt());
            }
        }

        internal override void Save(NativeWriter writer, List<int> relocTable, out long startOffset)
        {
            long offset = writer.Position;

            for (int i = 0; i < RvmShaderRefGuids.Count; i++)
            {
                writer.Write(RvmShaderRefGuids[i]);
                writer.Write(RvmShaderRefInts[i]);
            }

            writer.WritePadding(0x08);

            base.Save(writer, relocTable, out startOffset);

            writer.Write(offset);
            writer.Write((long)RvmShaderRefGuids.Count);
        }

        public void SetHash(uint meshNameHash, uint variationNameHash, int entryIndex)
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(meshNameHash);         // uint32   NameHash of meshset (nameHash in the ebx seems to do nothing)
                writer.Write(variationNameHash);    // uint32   VariationAssetNameHash (the same like in the MeshVariationDatabase)
                writer.Write(entryIndex);           // int32    LodIndex

                ulong hash = CityHash.Hash64(writer.ToByteArray());

                if (hash != Hash)
                {
                    Hash = hash;
                    IsModified = true;
                }
            }
        }
    }

    public class ShaderBlockEntry : ShaderStaticParamDbBlock
    {
        public ShaderBlockEntry()
        {
        }

        public override void Read(NativeReader reader, List<ShaderBlockResource> shaderBlockEntries)
        {
            base.Read(reader, shaderBlockEntries);
        }

        public MeshParamDbBlock GetMeshParams(int sectionIndex)
        {
            ShaderStaticParamDbBlock sectionBlock = GetSection(sectionIndex);
            if (sectionBlock == null)
            {
                return null;
            }

            return sectionBlock.Resources[0] as MeshParamDbBlock;
        }

        public ShaderPersistentParamDbBlock GetTextureParams(int sectionIndex)
        {
            ShaderStaticParamDbBlock sectionBlock = GetSection(sectionIndex);

            ShaderStaticParamDbBlock paramsBlock = sectionBlock?.Resources[1] as ShaderStaticParamDbBlock;
            return paramsBlock?.Resources[1] as ShaderPersistentParamDbBlock;
        }

        public ShaderPersistentParamDbBlock GetParams(int sectionIndex)
        {
            ShaderStaticParamDbBlock sectionBlock = GetSection(sectionIndex);

            ShaderStaticParamDbBlock paramsBlock = sectionBlock?.Resources[1] as ShaderStaticParamDbBlock;
            return paramsBlock?.Resources[0] as ShaderPersistentParamDbBlock;
        }

        private ShaderStaticParamDbBlock GetSection(int sectionIndex)
        {
            if (sectionIndex >= Resources.Count)
            {
                return null;
            }

            return Resources[sectionIndex] as ShaderStaticParamDbBlock;
        }

        public void SetHash(uint meshNameHash, uint variationNameHash, int entryIndex)
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(meshNameHash);         // uint32   NameHash of meshset (nameHash in the ebx seems to do nothing)
                writer.Write(variationNameHash);    // uint32   VariationAssetNameHash (the same like in the MeshVariationDatabase)
                writer.Write(entryIndex);           // int32    LodIndex

                ulong hash = CityHash.Hash64(writer.ToByteArray());

                if (hash != Hash)
                {
                    Hash = hash;
                    IsModified = true;
                }
            }
        }
    }

    public class ParameterEntry
    {
        public uint NameHash;
        public uint TypeHash;
        public ushort Used;
        public byte[] Value;

        private ulong parameterHash;

        public ParameterEntry(string name, object value)
        {
            NameHash = (uint)Fnv1.HashString(name.ToLower());
            Type objType = value.GetType();
            Type actualType = null;

            if (value is bool)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("Boolean"));
            }
            else if (value is uint)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("Uint32"));
            }
            else if (value is float)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("Float32"));
            }
            else if (value is long)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("Int64"));
            }
            else if (value is float[])
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("Vec"));
            }
            else if (value is Guid)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("ITexture"));
            }
            else if (value is PrimitiveType)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("PrimitiveType"));
            }
            else if (value is ShaderInstancingMethod)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("ShaderInstancingMethod"));
            }
            else if (value is RvmLevelOfDetail)
            {
                actualType = TypeLibrary.GetType((uint)Fnv1.HashString("RvmLevelOfDetail"));
            }
            else
            {
                actualType = objType;
            }

            TypeHash = (uint)Fnv1.HashString(actualType.Name);
            Used = 1;

            parameterHash = CalculateHash(name, actualType);

            SetValue(value);
        }

        public ParameterEntry(NativeReader reader)
        {
            parameterHash = reader.ReadULong();
            TypeHash = reader.ReadUInt();
            Used = reader.ReadUShort();
            NameHash = (uint)((uint)reader.ReadUShort() << 16 | (((parameterHash >> 48) & 0xFFFF)));

            int size = reader.ReadInt();
            if (TypeHash == 0xad0abfd3 /* ITexture */)
            {
                size = 16;
            }

            Value = reader.ReadBytes(size);
        }

        public object GetValue()
        {
            object retVal = null;
            switch (TypeHash)
            {
                case 0x9638b221: retVal = BitConverter.ToBoolean(Value, 0); break;
                case 0x0d1cfa1b: retVal = Value[0]; break;
                case 0xb0bc3c22: retVal = BitConverter.ToUInt32(Value, 0); break;
                case 0x7f39a7b4: retVal = BitConverter.ToSingle(Value, 0); break;
                case 0xcc971f4: retVal = BitConverter.ToInt64(Value, 0); break;
                case 0xad0abfd3: retVal = new Guid(Value); break;
                case 0x0b87fa95:
                    {
                        float[] f = new float[4];
                        f[0] = BitConverter.ToSingle(Value, 0);
                        f[1] = BitConverter.ToSingle(Value, 4);
                        f[2] = BitConverter.ToSingle(Value, 8);
                        f[3] = BitConverter.ToSingle(Value, 12);
                        retVal = f;
                    }
                    break;
                case 0x3AD97822: retVal = (RvmLevelOfDetail)BitConverter.ToInt32(Value, 0); break;
                case 0x963FC9FC: retVal = (PrimitiveType)BitConverter.ToInt32(Value, 0); break;
                case 0x85EA841F: retVal = (ShaderInstancingMethod)BitConverter.ToInt32(Value, 0); break;
            }
            return retVal;
        }

        public void SetValue(object value)
        {
            switch (TypeHash)
            {
                case 0x9638b221: Value = new byte[1] { (byte)(((bool)value) ? 1 : 0) }; break;
                case 0x0d1cfa1b: Value = new byte[1] { (byte)value }; break;
                case 0xb0bc3c22: Value = BitConverter.GetBytes((uint)value); break;
                case 0x7f39a7b4: Value = BitConverter.GetBytes((float)value); break;
                case 0xcc971f4: Value = BitConverter.GetBytes((long)value); break;
                case 0x0b87fa95:
                    {
                        using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                        {
                            float[] f = (float[])value;
                            writer.Write(f[0]);
                            writer.Write(f[1]);
                            writer.Write(f[2]);
                            writer.Write(f[3]);
                            Value = writer.ToByteArray();
                        }
                    }
                    break;
                case 0xad0abfd3: Value = ((Guid)value).ToByteArray(); break;
                case 0x3AD97822:
                case 0x963FC9FC:
                case 0x85EA841F:
                default: Value = BitConverter.GetBytes((int)value); break;
            }
        }

        public byte[] ToBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(parameterHash);
                writer.Write(TypeHash);
                writer.Write(Used);
                writer.Write((ushort)(NameHash >> 16));
                writer.Write((TypeHash == 0xad0abfd3) ? 1 : Value.Length);
                writer.Write(Value);

                return writer.ToByteArray();
            }
        }

        private ulong CalculateHash(string name, Type type)
        {
            string typeName = type.Name;
            string typeModule = type.GetCustomAttribute<EbxClassMetaAttribute>().Namespace;

            byte[] buffer = null;
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(0x01);
                writer.Write(name.Length);
                writer.Write(typeName.Length);
                writer.Write(typeModule.Length);
                writer.WriteFixedSizedString(name, name.Length);
                writer.WriteFixedSizedString(typeName, typeName.Length);
                writer.WriteFixedSizedString(typeModule, typeModule.Length);
                buffer = writer.ToByteArray();
            }

            return ((CityHash.Hash64(buffer) & 0xFFFFFFFFFFFF) | ((ulong)Fnv1.HashString(name.ToLower()) << 48));
        }
    }

    public class ShaderBlockDepot : Resource
    {
        public int ResourceCount => sbResources.Count;
        public int Count => sbEntries.Count;

        private List<ShaderBlockEntry> sbEntries = new List<ShaderBlockEntry>();
        private List<ShaderBlockResource> sbResources = new List<ShaderBlockResource>();

        public ShaderBlockDepot()
        {
        }

        public ShaderBlockDepot(byte[] inMeta)
        {
            resMeta = inMeta;
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);

            ModifiedShaderBlockDepot msbd = modifiedData as ModifiedShaderBlockDepot;

            int count = BitConverter.ToInt32(resMeta, 0x0c);
            List<long> offsets = new List<long>();

            for (int i = 0; i < count; i++)
            {
                offsets.Add(reader.ReadLong());

                long type = reader.ReadLong();
                ShaderBlockResource bde = null;

                switch (type)
                {
                    case 0: bde = new ShaderBlockEntry(); sbEntries.Add(bde as ShaderBlockEntry); break;
                    case 1: bde = new ShaderPersistentParamDbBlock(); break;
                    case 2: bde = new ShaderStaticParamDbBlock(); break;
                    case 3: bde = new MeshParamDbBlock(); break;
                    case 4: bde = new ShaderBlockMeshVariationEntry(); break;
                }

                sbResources.Add(bde);
            }

            // load in modifications
            for (int i = 0; i < offsets.Count; i++)
            {
                reader.Position = offsets[i];
                sbResources[i].Read(reader, sbResources);

                if (msbd != null && msbd.ContainsHash(sbResources[i].Hash))
                {
                    sbResources[i] = msbd.GetResource(sbResources[i].Hash);
                }

                sbResources[i].Index = i;
            }
        }

        public override byte[] SaveBytes()
        {
            return ToBytes();
        }

        public override ModifiedResource SaveModifiedResource()
        {
            ModifiedShaderBlockDepot msbd = new ModifiedShaderBlockDepot();
            foreach (var resource in sbResources)
            {
                if (resource.IsModified)
                {
                    msbd.AddResource(resource.Hash, resource);
                }
            }
            return msbd;
        }

        public ShaderBlockResource GetResource(int index)
        {
            if (index >= sbResources.Count)
            {
                return null;
            }

            return sbResources[index];
        }

        public bool ReplaceResource(ShaderBlockResource newResource)
        {
            for (int i = 0; i < sbResources.Count; i++)
            {
                if (sbResources[i].Hash == newResource.Hash)
                {
                    newResource.Index = sbResources[i].Index;
                    sbResources[i] = newResource;
                    return true;
                }
            }
            return false;
        }

        public ShaderBlockEntry GetSectionEntry(int lodIndex)
        {
            if (lodIndex >= sbEntries.Count)
            {
                return null;
            }

            return sbEntries[lodIndex];
        }

        public byte[] ToBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                for (int i = 0; i < sbResources.Count; i++)
                {
                    writer.Write((long)0);
                    writer.Write((long)0);
                }

                List<long> offsets = new List<long>();
                List<int> relocTable = new List<int>();

                for (int i = 0; i < sbResources.Count; i++)
                {
                    sbResources[i].Save(writer, relocTable, out long offset);
                    offsets.Add(offset);

                    relocTable.Add(i * 0x10);
                }
                writer.WritePadding(0x10);

                writer.Position = 0;
                for (int i = 0; i < offsets.Count; i++)
                {
                    writer.Write(offsets[i]);
                    if (sbResources[i] is ShaderBlockEntry)
                    {
                        writer.Write((long)0);
                    }
                    else if (sbResources[i] is ShaderPersistentParamDbBlock)
                    {
                        writer.Write((long)1);
                    }
                    else if (sbResources[i] is ShaderStaticParamDbBlock)
                    {
                        writer.Write((long)2);
                    }
                    else if (sbResources[i] is MeshParamDbBlock)
                    {
                        writer.Write((long)3);
                    }
                    else if (sbResources[i] is ShaderBlockMeshVariationEntry)
                    {
                        writer.Write((long)4);
                    }
                }

                unsafe
                {
                    // update the res meta
                    fixed (byte* ptr = &resMeta[0])
                    {

                        if (*(ushort*)(ptr + 0) != 0x000A)
                        {
                            throw new Exception("version number");// maybe version number
                        }

                        if (*(ushort*)(ptr + 2) != 0x5B06)
                        {
                            throw new Exception("unk");
                        }

                        *(uint*)(ptr + 4) = (uint)writer.Length;
                        *(uint*)(ptr + 8) = (uint)(relocTable.Count * 4);
                        *(uint*)(ptr + 12) = (uint)sbResources.Count;
                    }
                }

                writer.Position = writer.Length;
                for (int i = 0; i < relocTable.Count; i++)
                {
                    writer.Write(relocTable[i]);
                }

                return writer.ToByteArray();
            }
        }
    }

    public class ModifiedShaderBlockDepot : ModifiedResource
    {
        private List<ulong> hashes = new List<ulong>();
        private List<ShaderBlockResource> resources = new List<ShaderBlockResource>();

        public ModifiedShaderBlockDepot()
        {
        }

        public void Merge(ModifiedShaderBlockDepot newMsbd)
        {
            for (int i = 0; i < hashes.Count; i++)
            {
                if (newMsbd.ContainsHash(hashes[i]))
                {
                    resources[i] = newMsbd.GetResource(hashes[i]);
                }
            }
            for (int i = 0; i < newMsbd.hashes.Count; i++)
            {
                if (!ContainsHash(newMsbd.hashes[i]))
                {
                    AddResource(newMsbd.hashes[i], newMsbd.resources[i]);
                }
            }
        }

        internal bool ContainsHash(ulong hash)
        {
            return hashes.Contains(hash);
        }

        internal ShaderBlockResource GetResource(ulong hash)
        {
            return resources[hashes.IndexOf(hash)];
        }

        internal void AddResource(ulong hash, ShaderBlockResource resource)
        {
            int index = hashes.IndexOf(hash);
            if (index == -1)
            {
                hashes.Add(hash);
                resources.Add(null);
                index = resources.Count - 1;
            }

            resources[index] = resource;
        }

        public override void SaveInternal(NativeWriter writer)
        {
            base.SaveInternal(writer);

            writer.Write(hashes.Count);
            writer.WritePadding(0x10);

            for (int i = 0; i < hashes.Count; i++)
            {
                writer.Write((long)0);
                writer.Write((long)0);
            }

            List<long> offsets = new List<long>();
            List<int> relocTable = new List<int>();

            for (int i = 0; i < hashes.Count; i++)
            {
                resources[i].Save(writer, relocTable, out long offset);
                offsets.Add(offset);
            }

            writer.Position = 0x10;
            for (int i = 0; i < hashes.Count; i++)
            {
                writer.Write(offsets[i]);
                if (resources[i] is ShaderBlockEntry)
                {
                    writer.Write((long)0);
                }
                else if (resources[i] is ShaderPersistentParamDbBlock)
                {
                    writer.Write((long)1);
                }
                else if (resources[i] is ShaderStaticParamDbBlock)
                {
                    writer.Write((long)2);
                }
                else if (resources[i] is MeshParamDbBlock)
                {
                    writer.Write((long)3);
                }
                else if (resources[i] is ShaderBlockMeshVariationEntry)
                {
                    writer.Write((long)4);
                }
            }
        }

        public override void ReadInternal(NativeReader reader)
        {
            int count = reader.ReadInt();
            reader.Pad(0x10);

            List<long> offsets = new List<long>();
            for (int i = 0; i < count; i++)
            {
                offsets.Add(reader.ReadLong());

                long type = reader.ReadLong();
                ShaderBlockResource bde = null;

                switch (type)
                {
                    case 0: bde = new ShaderBlockEntry(); break;
                    case 1: bde = new ShaderPersistentParamDbBlock(); break;
                    case 2: bde = new ShaderStaticParamDbBlock(); break;
                    case 3: bde = new MeshParamDbBlock(); break;
                    case 4: bde = new ShaderBlockMeshVariationEntry(); break;
                }

                resources.Add(bde);
            }

            for (int i = 0; i < resources.Count; i++)
            {
                reader.Position = offsets[i];
                resources[i].Read(reader, null);
                hashes.Add(resources[i].Hash);
            }
        }
    }
}
