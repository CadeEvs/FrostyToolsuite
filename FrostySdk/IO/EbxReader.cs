using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FrostySdk.IO
{
    public enum PointerRefType : byte
    {
        Null = 0,
        Internal = 1,
        External = 2
    }

    public enum EbxFieldType : byte
    {
        Inherited = 0x00,
        DbObject = 0x01,
        Struct = 0x02,
        Pointer = 0x03,
        Array = 0x04,
        FixedArray = 0x05,
        String = 0x06,
        CString = 0x07,
        Enum = 0x08,
        FileRef = 0x09,
        Boolean = 0x0A,
        Int8 = 0x0B,
        UInt8 = 0x0C,
        Int16 = 0x0D,
        UInt16 = 0x0E,
        Int32 = 0x0F,
        UInt32 = 0x10,
        Int64 = 0x12,
        UInt64 = 0x11,
        Float32 = 0x13,
        Float64 = 0x14,
        Guid = 0x15,
        Sha1 = 0x16,
        ResourceRef = 0x17,
        Function = 0x18,
        TypeRef = 0x19,
        BoxedValueRef = 0x1A,
        Interface = 0x1B,
        Delegate = 0x1C
    }

    public enum EbxFieldCategory : byte
    {
        None = 0,
        Pointer = 1,
        Struct = 2,
        PrimitiveType = 3,
        ArrayType = 4,
        EnumType = 5,
        FunctionType = 6,
        InterfaceType = 7,
        DelegateType = 8
    }

    internal enum EbxVersion
    {
        Version2 = 0x0FB2D1CE,
        Version4 = 0x0FB4D1CE,
        Version6 = 0x46464952 // RIFF LE
    }

    internal enum RiffEbxSection
    {
        EBX  = 0x45425800, // BE
        EBXS = 0x45425853, // BE
        EBXD = 0x45425844, // BE
        EFIX = 0x45464958, // BE
        EBXX = 0x45425858, // BE
        REFL = 0x5245464C  // BE
    }

    public struct EbxField
    {
        public string Name;
        public uint NameHash;
        public ushort Type;
        public ushort ClassRef;
        public uint DataOffset;
        public uint SecondOffset;

        public EbxFieldType DebugType => (EbxFieldType)((Type >> 4) & 0x1F);
        public EbxFieldCategory DebugCategory => (EbxFieldCategory)(Type & 0xF);
    }

    public struct EbxClass
    {
        public string Name;
        public uint NameHash;
        public int FieldIndex;
        public ushort FieldCount;
        public byte Alignment;
        public ushort Type;
        public ushort Size;
        public ushort SecondSize;
        public string Namespace;
        public int Index;

        public EbxFieldType DebugType => (EbxFieldType)((Type >> 4) & 0x1F);
        public EbxFieldCategory DebugCategory => (EbxFieldCategory)(Type & 0xF);
    }

    public struct EbxInstance
    {
        public ushort ClassRef;
        public ushort Count;
        public bool IsExported;
    }

    public struct EbxArray
    {
        public int ClassRef;
        public uint Offset;
        public uint Count;
        public ushort Type;
    }

    public struct EbxBoxedValue
    {
        public uint Offset;
        public ushort ClassRef;
        public ushort Type;
    }

    public struct EbxImportReference
    {
        public Guid FileGuid;
        public Guid ClassGuid;

        public override string ToString() => FileGuid.ToString() + "/" + ClassGuid.ToString();

        public static bool operator ==(EbxImportReference A, EbxImportReference B) => A.Equals(B);

        public static bool operator !=(EbxImportReference A, EbxImportReference B) => !A.Equals(B);

        public override bool Equals(object obj)
        {
            if (obj is EbxImportReference b)
            {
                return (FileGuid == b.FileGuid && ClassGuid == b.ClassGuid);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int Hash = (int)2166136261;
                Hash = (Hash * 16777619) ^ FileGuid.GetHashCode();
                Hash = (Hash * 16777619) ^ ClassGuid.GetHashCode();
                return Hash;
            }
        }
    }

    [Serializable]
    public class EbxAsset
    {
        public Guid FileGuid => fileGuid;
        public Guid RootInstanceGuid
        {
            get
            {
                AssetClassGuid guid = ((dynamic)RootObject).GetInstanceGuid();
                return guid.ExportedGuid;
            }
        }

        public IEnumerable<Guid> Dependencies
        {
            get
            {
                for (int i = 0; i < dependencies.Count; i++)
                    yield return dependencies[i];
            }
        }
        public IEnumerable<object> RootObjects
        {
            get
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    if (refCounts[i] == 0 || i == 0)
                        yield return objects[i];
                }
            }
        }
        public IEnumerable<object> Objects
        {
            get
            {
                for (int i = 0; i < objects.Count; i++)
                    yield return objects[i];
            }
        }
        public IEnumerable<object> ExportedObjects
        {
            get
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    dynamic obj = objects[i];
                    AssetClassGuid guid = obj.GetInstanceGuid();
                    if (guid.IsExported)
                        yield return obj;
                }
            }
        }
        public object RootObject => objects[0];
        public bool IsValid => objects.Count != 0;
        public bool TransientEdit { get; set; }

        internal Guid fileGuid;
        internal List<object> objects;
        internal List<Guid> dependencies;
        internal List<int> refCounts;

        public EbxAsset()
        {
        }

        public EbxAsset(params object[] rootObjects)
        {
            fileGuid = Guid.NewGuid();

            objects = new List<object>();
            refCounts = new List<int>();
            dependencies = new List<Guid>();

            foreach (dynamic obj in rootObjects)
            {
                obj.SetInstanceGuid(new AssetClassGuid(Guid.NewGuid(), objects.Count));
                objects.Add(obj);
                refCounts.Add(0);
            }
        }

        /// <summary>
        /// Invoked when loading of the ebx asset has completed, to allow for any custom handling
        /// </summary>
        public virtual void OnLoadComplete()
        {
        }

        /// <summary>
        /// Saves the resource as a specialized ModifiedResource object
        /// </summary>
        public virtual ModifiedResource SaveModifiedResource()
        {
            return null;
        }

        /// <summary>
        /// Apply the modified resource data onto the loaded ebx data
        /// </summary>
        public virtual void ApplyModifiedResource(ModifiedResource modifiedResource)
        {
        }

        public dynamic GetObject(Guid guid)
        {
            foreach (dynamic obj in ExportedObjects)
            {
                if (obj.GetInstanceGuid() == guid)
                    return obj;
            }
            return null;
        }

        public bool AddDependency(Guid guid)
        {
            if (dependencies.Contains(guid))
                return false;
            dependencies.Add(guid);
            return true;
        }

        public void SetFileGuid(Guid guid) => fileGuid = guid;

        public void AddObject(dynamic obj, bool root = false)
        {
            AssetClassGuid guid = obj.GetInstanceGuid();
            if (guid.InternalId == -1)
            {
                // make sure internal id is set before adding
                guid = new AssetClassGuid(guid.ExportedGuid, objects.Count);
                obj.SetInstanceGuid(guid);
            }

            refCounts.Add(1);
            objects.Add(obj);
        }

        public void AddRootObject(dynamic obj)
        {
            Type t = obj.GetType();
            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(objects, t, fileGuid), objects.Count);
            obj.SetInstanceGuid(guid);

            if (objects.Contains(obj))
            {
                int index = objects.IndexOf(obj);
                refCounts[index] = 0;
            }
            else
            {
                refCounts.Add(0);
                objects.Add(obj);
            }
        }

        public void RemoveObject(object obj)
        {
            int idx = objects.IndexOf(obj);
            if (idx == -1)
                return;

            refCounts[idx]--;
            if (refCounts[idx] <= 0)
            {
                refCounts.RemoveAt(idx);
                objects.RemoveAt(idx);
            }
        }

        public void Update()
        {
            dependencies.Clear();

            Dictionary<object, int> mapping = new Dictionary<object, int>();
            List<int> newRefCnts = new List<int>();

            for (int i = 0; i < objects.Count; i++)
            {
                mapping.Add(objects[i], i);
                newRefCnts.Add(0);
            }

            List<int> nonRootObjs = new List<int>();
            List<Tuple<PropertyInfo, object>> refProps = new List<Tuple<PropertyInfo, object>>();
            List<Tuple<object, Guid>> externalProps = new List<Tuple<object, Guid>>();
            List<object> objsToProcess = new List<object>();
            int z = 0;

            // count refs for all pointers
            objsToProcess.AddRange(RootObjects);
            nonRootObjs.Add(0);

            while (objsToProcess.Count > 0)
            {
                int j = mapping[objsToProcess[0]];
                if (refCounts[j] == 0 && !nonRootObjs.Contains(j))
                    nonRootObjs.Add(j);

                CountRefs(objsToProcess[0], objsToProcess[0], ref newRefCnts, ref mapping, ref refProps, ref externalProps, ref objsToProcess);
                objsToProcess.RemoveAt(0);
            }

            //foreach (object obj in objects)
            //{
            //    if (refCounts[z] != 0)
            //        nonRootObjs.Add(z);
            //    CountRefs(obj, z, ref newRefCnts, ref mapping, ref refProps, ref externalProps);
            //    z++;
            //}

            // now remove non referenced, non root objects
            //nonRootObjs.Sort((int a, int b) => { return -a.CompareTo(b); });
            //foreach (int idx in nonRootObjs)
            //{
            //    if (newRefCnts[idx] == 0)
            //    {
            //        newRefCnts.RemoveAt(idx);
            //        objects.RemoveAt(idx);
            //    }
            //}

            for (int i = newRefCnts.Count - 1; i >= 0; i--)
            {
                if (nonRootObjs.Contains(i))
                    continue;

                if (newRefCnts[i] == 0)
                {
                    newRefCnts.RemoveAt(i);
                    objects.RemoveAt(i);
                }
            }

            foreach (var externalProp in externalProps)
            {
                if (objects.Contains(externalProp.Item1))
                {
                    if (!dependencies.Contains(externalProp.Item2))
                        dependencies.Add(externalProp.Item2);
                }
            }

            // check for invalid references, and clear them
            foreach (Tuple<PropertyInfo, object> refProp in refProps)
            {
                Type pType = refProp.Item1.PropertyType;
                if (pType == PointerType)
                {
                    PointerRef pr = (PointerRef)refProp.Item1.GetValue(refProp.Item2);
                    if (!objects.Contains(pr.Internal))
                    {
                        refProp.Item1.SetValue(refProp.Item2, new PointerRef());
                    }
                }
                else
                {
                    System.Collections.IList list = (System.Collections.IList)refProp.Item1.GetValue(refProp.Item2);
                    int count = list.Count;
                    bool requiresChange = false;

                    for (int i = 0; i < count; i++)
                    {
                        PointerRef pr = (PointerRef)list[i];
                        if (pr.Type == PointerRefType.Internal)
                        {
                            if (!objects.Contains(pr.Internal))
                            {
                                list[i] = new PointerRef();
                                requiresChange = true;
                            }
                        }
                    }

                    if (requiresChange)
                        refProp.Item1.SetValue(refProp.Item2, list);
                }
            }

            refCounts = newRefCnts;
        }

        private static Type PointerType = typeof(PointerRef);
        private static Type ValueType = typeof(ValueType);
        private static Type BoxedValueType = typeof(BoxedValueRef);

        //private void CountRefs(object obj, int objIndex, ref List<int> newRefCnts, ref Dictionary<object, int> mapping, ref List<Tuple<PropertyInfo, object>> refProps, ref List<Tuple<object, Guid>> externalProps)
        private void CountRefs(object obj, object classObj, ref List<int> newRefCnts, ref Dictionary<object, int> mapping, ref List<Tuple<PropertyInfo, object>> refProps, ref List<Tuple<object, Guid>> externalProps, ref List<object> objsToProcess)
        {
            PropertyInfo[] pis = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo pi in pis)
            {
                if (pi.PropertyType.IsPrimitive)
                    continue;
                if (pi.PropertyType.IsEnum)
                    continue;

                Type pType = pi.PropertyType;

                // Pointers
                if (pType == PointerType)
                {
                    bool isRef = pi.GetCustomAttribute<IsReferenceAttribute>() != null;

                    PointerRef pr = (PointerRef)pi.GetValue(obj);
                    if (pr.Type == PointerRefType.Internal)
                    {
                        if (isRef)
                        {
                            // collect reference for later checking
                            refProps.Add(new Tuple<PropertyInfo, object>(pi, obj));
                        }
                        else
                        {
                            // increase ref count
                            int idx = mapping[pr.Internal];
                            if (newRefCnts[idx] == 0)
                            {
                                newRefCnts[idx]++;
                                objsToProcess.Add(pr.Internal);
                            }
                        }
                    }
                    else if (pr.Type == PointerRefType.External)
                    {
                        externalProps.Add(new Tuple<object, Guid>(classObj, pr.External.FileGuid));
                    }
                }

                // Arrays
                else if (pType.GenericTypeArguments.Length != 0)
                {
                    Type arrayType = pType.GenericTypeArguments[0];

                    System.Collections.IList list = (System.Collections.IList)pi.GetValue(obj);
                    int count = list.Count;

                    if (count > 0)
                    {
                        // Pointer Array
                        if (arrayType == PointerType)
                        {
                            bool isRef = pi.GetCustomAttribute<IsReferenceAttribute>() != null;
                            if (isRef)
                                refProps.Add(new Tuple<PropertyInfo, object>(pi, obj));

                            for (int i = 0; i < count; i++)
                            {
                                List<PointerRef> plist = (List<PointerRef>)list;
                                PointerRef pr = plist[i];

                                if (pr.Type == PointerRefType.Internal && !isRef)
                                {
                                    int idx = mapping[pr.Internal];
                                    if (newRefCnts[idx] == 0)
                                    {
                                        newRefCnts[idx]++;
                                        objsToProcess.Add(pr.Internal);
                                    }
                                }
                                else if (pr.Type == PointerRefType.External)
                                {
                                    externalProps.Add(new Tuple<object, Guid>(classObj, pr.External.FileGuid));
                                }
                            }
                        }

                        // Structure Array
                        else if (arrayType != ValueType)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                CountRefs(list[i], classObj, ref newRefCnts, ref mapping, ref refProps, ref externalProps, ref objsToProcess);
                            }
                        }
                    }
                }

                else if (pType == BoxedValueType)
                {
                    BoxedValueRef boxedValue = pi.GetValue(obj) as BoxedValueRef;
                    if (boxedValue.Value != null)
                        CountRefs(boxedValue.Value, classObj, ref newRefCnts, ref mapping, ref refProps, ref externalProps, ref objsToProcess);
                }

                // Structures
                else if (pType != ValueType)
                {
                    CountRefs(pi.GetValue(obj), classObj, ref newRefCnts, ref mapping, ref refProps, ref externalProps, ref objsToProcess);
                }
            }
        }
    }

    public class EbxReader : NativeReader
    {
        public static EbxReader CreateProjectReader(Stream inStream)
        {
            return new EbxReader(inStream);
        }

        public static EbxReader CreateReader(Stream inStream, FileSystemManager fs = null, bool patched = false)
        {
            return ProfilesLibrary.EbxVersion == 6 ? new EbxReaderRiff(inStream, fs, patched) : (ProfilesLibrary.EbxVersion & 1) != 0 ? new EbxReaderV2(inStream, fs, patched) : new EbxReader(inStream);
        }

        public Guid FileGuid => fileGuid;
        public virtual string RootType => classTypes[instances[0].ClassRef].Name;
        public List<Guid> Dependencies => dependencies;
        public bool IsValid => isValid;

        public List<EbxField> FieldTypes => fieldTypes;
        public List<EbxClass> ClassTypes => classTypes;

        internal List<EbxField> fieldTypes = new List<EbxField>();
        private List<EbxClass> classTypes = new List<EbxClass>();
        internal List<EbxInstance> instances = new List<EbxInstance>();
        internal List<EbxArray> arrays = new List<EbxArray>();
        internal List<EbxBoxedValue> boxedValues = new List<EbxBoxedValue>();
        internal List<EbxImportReference> imports = new List<EbxImportReference>();
        internal List<Guid> dependencies = new List<Guid>();
        internal List<object> objects = new List<object>();
        internal List<int> refCounts = new List<int>();

        protected List<BoxedValueRef> boxedValueRefs = new List<BoxedValueRef>();

        internal Guid fileGuid;
        internal long arraysOffset;
        internal long stringsOffset;
        internal long stringsAndDataLen;
        internal uint guidCount;
        internal ushort instanceCount;
        internal uint exportedCount;
        internal ushort uniqueClassCount;
        internal ushort classTypeCount;
        internal ushort fieldTypeCount;
        internal ushort typeNamesLen;
        internal uint stringsLen;
        internal uint arrayCount;
        internal uint dataLen;
        internal uint boxedValuesCount;
        internal long boxedValuesOffset;

        internal EbxVersion magic;
        internal bool isValid = false;

        internal byte[] boxedValueBuffer;

        internal EbxReader(Stream inStream, bool passthru)
            : base(inStream)
        {
        }

        internal EbxReader(Stream InStream)
            : base(InStream)
        {
            magic = (EbxVersion)ReadUInt();
            if (magic != EbxVersion.Version2 && magic != EbxVersion.Version4)
                return;

            stringsOffset = ReadUInt();
            stringsAndDataLen = ReadUInt();
            guidCount = ReadUInt();
            instanceCount = ReadUShort();
            exportedCount = ReadUShort();
            uniqueClassCount = ReadUShort();
            classTypeCount = ReadUShort();
            fieldTypeCount = ReadUShort();
            typeNamesLen = ReadUShort();

            stringsLen = ReadUInt();
            arrayCount = ReadUInt();
            dataLen = ReadUInt();

            arraysOffset = stringsOffset + stringsLen + dataLen;

            fileGuid = ReadGuid();

            if (magic == EbxVersion.Version4)
            {
                boxedValuesCount = ReadUInt();
                boxedValuesOffset = ReadUInt();
                boxedValuesOffset += stringsOffset + stringsLen;
            }
            else
            {
                while (Position % 16 != 0)
                    Position++;
            }

            for (int i = 0; i < guidCount; i++)
            {
                EbxImportReference import = new EbxImportReference
                {
                    FileGuid = ReadGuid(),
                    ClassGuid = ReadGuid()
                };

                imports.Add(import);
                if (!dependencies.Contains(import.FileGuid))
                    dependencies.Add(import.FileGuid);
            }

            Dictionary<int, string> typeNames = new Dictionary<int, string>();

            long typeNamesOffset = Position;
            while (Position - typeNamesOffset < typeNamesLen)
            {
                string typeName = ReadNullTerminatedString();
                int hash = HashString(typeName);

                if (!typeNames.ContainsKey(hash))
                    typeNames.Add(hash, typeName);
            }

            for (int i = 0; i < fieldTypeCount; i++)
            {
                EbxField fieldType = new EbxField();

                int hash = ReadInt();
                fieldType.Type = (magic == EbxVersion.Version2) ? (ushort)(ReadUShort()) : (ushort)(ReadUShort() >> 1);
                fieldType.ClassRef = ReadUShort();
                fieldType.DataOffset = ReadUInt();
                fieldType.SecondOffset = ReadUInt();
                fieldType.Name = typeNames[hash];

                fieldTypes.Add(fieldType);
            }

            for (int i = 0; i < classTypeCount; i++)
            {
                EbxClass classType = new EbxClass();

                int hash = ReadInt();
                classType.FieldIndex = ReadInt();
                classType.FieldCount = ReadByte();
                classType.Alignment = ReadByte();
                classType.Type = (magic == EbxVersion.Version2) ? (ushort)(ReadUShort()) : (ushort)(ReadUShort() >> 1);
                classType.Size = ReadUShort();
                classType.SecondSize = ReadUShort();
                classType.Name = typeNames[hash];
                classTypes.Add(classType);
            }

            uint tmpExportedCount = exportedCount;
            for (int i = 0; i < instanceCount; i++)
            {
                EbxInstance inst = new EbxInstance
                {
                    ClassRef = ReadUShort(),
                    Count = ReadUShort()
                };

                if (tmpExportedCount != 0)
                {
                    inst.IsExported = true;
                    tmpExportedCount--;
                }

                instances.Add(inst);
            }

            while (Position % 16 != 0)
                Position++;

            for (int i = 0; i < arrayCount; i++)
            {
                EbxArray array = new EbxArray
                {
                    Offset = ReadUInt(),
                    Count = ReadUInt(),
                    ClassRef = ReadInt()
                };

                arrays.Add(array);
            }

            Pad(16);

            for (int i = 0; i < boxedValuesCount; i++)
            {
                EbxBoxedValue boxedValue = new EbxBoxedValue
                {
                    Offset = ReadUInt(),
                    ClassRef = ReadUShort(),
                    Type = ReadUShort()
                };

                boxedValues.Add(boxedValue);
            }

            Position = stringsOffset + stringsLen;
            isValid = true;
        }

        public T ReadAsset<T>() where T : EbxAsset, new()
        {
            T asset = new T();
            InternalReadObjects();

            asset.fileGuid = fileGuid;
            asset.objects = objects;
            asset.dependencies = dependencies;
            asset.refCounts = refCounts;
            asset.OnLoadComplete();

            return asset;
        }

        public dynamic ReadObject()
        {
            InternalReadObjects();
            return objects[0];
        }

        public List<object> ReadObjects()
        {
            InternalReadObjects();
            return objects;
        }

        public List<object> GetUnreferencedObjects()
        {
            List<object> unrefObjs = new List<object>
            {
                objects[0] // always add root
            };

            // iterate through the restof the objects
            for (int i = 1; i < objects.Count; i++)
            {
                if (refCounts[i] == 0)
                    unrefObjs.Add(objects[i]);
            }

            return unrefObjs;
        }

        internal virtual void InternalReadObjects()
        {
            List<Type> types = new List<Type>();
            foreach (EbxInstance inst in instances)
            {
                EbxClass classType = GetClass(null, inst.ClassRef);
                for (int i = 0; i < inst.Count; i++)
                {
                    Type type = ParseClass(classType);
                    objects.Add(TypeLibrary.CreateObject(type));
                    refCounts.Add(0);
                }
            }

            int typeId = 0;
            int index = 0;

            foreach (EbxInstance inst in instances)
            {
                EbxClass classType = GetClass(null, inst.ClassRef);
                for (int i = 0; i < inst.Count; i++)
                {
                    while (Position % classType.Alignment != 0)
                        Position++;

                    Guid instanceGuid = Guid.Empty;
                    if (inst.IsExported)
                        instanceGuid = ReadGuid();

                    if (classType.Alignment != 0x04)
                        Position += 8;

                    dynamic obj = objects[typeId++];
                    obj.SetInstanceGuid(new AssetClassGuid(instanceGuid, index++));

                    ReadClass(classType, obj, Position - 8);
                }
            }

            //while (boxedValuesOffset % 16 != 0)
            //    boxedValuesOffset++;

            // finally read in any boxed value refs
            //foreach (BoxedValueRef boxedValueRef in boxedValueRefs)
            //{
            //    if (boxedValueRef != -1)
            //    {
            //        Position = boxedValuesOffset + (8 * boxedValueRef);
            //        boxedValueRef.SetData(ReadBytes(8));
            //    }
            //}

            if (boxedValuesCount > 0)
            {
                Position = boxedValuesOffset;
                boxedValueBuffer = ReadToEnd();
            }
        }

        internal virtual object ReadClass(EbxClass classType, object obj, long startOffset)
        {
            if (obj == null)
            {
                Position += classType.Size;
                Pad(classType.Alignment);
                return null;
            }
            Type objType = obj.GetType();

            for (int j = 0; j < classType.FieldCount; j++)
            {
                EbxField fieldType = GetField(classType, classType.FieldIndex + j);
                PropertyInfo fieldProp = GetProperty(objType, fieldType);

                IsReferenceAttribute attr = (fieldProp != null)
                    ? fieldProp.GetCustomAttribute<IsReferenceAttribute>()
                    : null;

                if (fieldType.DebugType == EbxFieldType.Inherited)
                {
                    ReadClass(GetClass(classType, fieldType.ClassRef), obj, startOffset);
                }
                else
                {
                    if (fieldType.DebugType == EbxFieldType.ResourceRef
                        || fieldType.DebugType == EbxFieldType.TypeRef
                        || fieldType.DebugType == EbxFieldType.FileRef
                        || fieldType.DebugType == EbxFieldType.BoxedValueRef
                        || fieldType.DebugType == EbxFieldType.UInt64
                        || fieldType.DebugType == EbxFieldType.Int64
                        || fieldType.DebugType == EbxFieldType.Float64)
                    {
                        // Structure alignment
                        Pad(8);
                    }
                    else if (fieldType.DebugType == EbxFieldType.Array
                        || fieldType.DebugType == EbxFieldType.Pointer)
                    {
                        Pad(4);
                    }

                    // @temp
                    //if (fieldType.DebugType != EbxFieldType.Struct)
                    //{
                    //    if ((Position - startOffset) != fieldType.DataOffset)
                    //        Console.WriteLine("Offset misalignment: " + fieldType.DebugType);
                    //}

                    if (fieldType.DebugType == EbxFieldType.Array)
                    {
                        EbxClass arrayType = GetClass(classType, fieldType.ClassRef);

                        int index = ReadInt();
                        EbxArray array = arrays[index];

                        long arrayPos = Position;
                        Position = arraysOffset + array.Offset;

                        for (int i = 0; i < array.Count; i++)
                        {
                            var arrayField = GetField(arrayType, arrayType.FieldIndex);
                            object value = ReadField(arrayType, arrayField.DebugType, arrayField.ClassRef, (attr != null));
                            if (fieldProp != null)
                            {
                                try { fieldProp.GetValue(obj).GetType().GetMethod("Add").Invoke(fieldProp.GetValue(obj), new object[] { value }); }
                                catch (Exception) { }
                            }
                        }
                        Position = arrayPos;
                    }
                    else
                    {
                        object value = ReadField(classType, fieldType.DebugType, fieldType.ClassRef, (attr != null));
                        if (fieldProp != null)
                        {
                            try { fieldProp.SetValue(obj, value); }
                            catch (Exception) { }
                        }
                    }
                }
            }

            Pad(classType.Alignment);

            return null;
        }

        internal virtual PropertyInfo GetProperty(Type objType, EbxField field) => objType.GetProperty(field.Name);

        internal virtual EbxClass GetClass(EbxClass? parentClass, int index) => classTypes[index];

        internal virtual EbxField GetField(EbxClass classType, int index) => fieldTypes[index];

        internal virtual object CreateObject(EbxClass classType) => TypeLibrary.CreateObject(classType.Name);

        internal virtual Type GetType(EbxClass classType) => TypeLibrary.GetType(classType.Name);

        internal object ReadField(EbxClass? parentClass, EbxFieldType fieldType, ushort fieldClassRef, bool dontRefCount = false)
        {
            switch (fieldType)
            {
                case EbxFieldType.Boolean:
                    return ReadBoolean();
                case EbxFieldType.Int8:
                    return (sbyte)ReadByte();
                case EbxFieldType.UInt8:
                    return ReadByte();
                case EbxFieldType.Int16:
                    return ReadShort();
                case EbxFieldType.UInt16:
                    return ReadUShort();
                case EbxFieldType.Int32:
                    return ReadInt();
                case EbxFieldType.UInt32:
                    return ReadUInt();
                case EbxFieldType.Int64:
                    return ReadLong();
                case EbxFieldType.UInt64: 
                    return ReadULong();
                case EbxFieldType.Float32:
                    return ReadFloat();
                case EbxFieldType.Float64:
                    return ReadDouble();
                case EbxFieldType.Guid:
                    return ReadGuid();
                case EbxFieldType.ResourceRef:
                    return ReadResourceRef();
                case EbxFieldType.Sha1:
                    return ReadSha1();
                case EbxFieldType.String:
                    return ReadSizedString(32);
                case EbxFieldType.CString:
                    return ReadCString(ReadUInt());
                case EbxFieldType.FileRef:
                    return ReadFileRef();
                case EbxFieldType.Delegate:
                case EbxFieldType.TypeRef:
                    return ReadTypeRef();
                case EbxFieldType.BoxedValueRef:
                    return ReadBoxedValueRef();
                case EbxFieldType.Struct:
                    EbxClass structType = GetClass(parentClass, fieldClassRef);
                    Pad(structType.Alignment);
                    object structObj = CreateObject(structType);
                    ReadClass(structType, structObj, Position);
                    return structObj;
                case EbxFieldType.Enum:
                    return ReadInt();
                case EbxFieldType.Pointer:
                    return ReadPointerRef(dontRefCount);
                case EbxFieldType.DbObject:
                    throw new InvalidDataException("DbObject");
                default:
                    throw new InvalidDataException("Unknown");
            }
        }

        internal Type ParseClass(EbxClass classType)
        {
            Type existingType = TypeLibrary.AddType(classType.Name);
            if (existingType != null)
                return existingType;

            List<FieldType> fields = new List<FieldType>();
            Type parent = null;

            for (int j = 0; j < classType.FieldCount; j++)
            {
                EbxField fieldType = fieldTypes[classType.FieldIndex + j];
                if (fieldType.DebugType == EbxFieldType.Inherited)
                    parent = ParseClass(classTypes[fieldType.ClassRef]);
                else
                {
                    Type newType = GetTypeFromEbxField(fieldType);
                    fields.Add(new FieldType(fieldType.Name, newType, null, fieldType,
                        (fieldType.DebugType == EbxFieldType.Array)
                            ? (EbxField?)fieldTypes[classTypes[fieldType.ClassRef].FieldIndex]
                            : null));
                }
            }

            return classType.DebugType == EbxFieldType.Struct ? TypeLibrary.FinalizeStruct(classType.Name, fields, classType) : TypeLibrary.FinalizeClass(classType.Name, fields, parent, classType);
        }

        internal Type GetTypeFromEbxField(EbxField fieldType)
        {
            switch (fieldType.DebugType)
            {
                case EbxFieldType.Struct: return ParseClass(GetClass(null, fieldType.ClassRef));
                case EbxFieldType.String: return typeof(string);
                case EbxFieldType.Int8: return typeof(sbyte);
                case EbxFieldType.UInt8: return typeof(byte);
                case EbxFieldType.Boolean: return typeof(bool);
                case EbxFieldType.UInt16: return typeof(ushort);
                case EbxFieldType.Int16: return typeof(short);
                case EbxFieldType.UInt32: return typeof(uint);
                case EbxFieldType.Int32: return typeof(int);
                case EbxFieldType.UInt64: return typeof(ulong);
                case EbxFieldType.Int64: return typeof(long);
                case EbxFieldType.Float32: return typeof(float);
                case EbxFieldType.Float64: return typeof(double);
                case EbxFieldType.Pointer: return typeof(PointerRef);
                case EbxFieldType.Guid: return typeof(Guid);
                case EbxFieldType.Sha1: return typeof(Sha1);
                case EbxFieldType.CString: return typeof(CString);
                case EbxFieldType.ResourceRef: return typeof(ResourceRef);
                case EbxFieldType.FileRef: return typeof(FileRef);
                case EbxFieldType.TypeRef: return typeof(TypeRef);
                case EbxFieldType.BoxedValueRef: return typeof(ulong);
                case EbxFieldType.Array:
                    EbxClass arrayType = classTypes[fieldType.ClassRef];
                    return typeof(List<>).MakeGenericType(GetTypeFromEbxField(fieldTypes[arrayType.FieldIndex]));
                case EbxFieldType.Enum:
                    EbxClass enumType = classTypes[fieldType.ClassRef];
                    List<Tuple<string, int>> enumValues = new List<Tuple<string, int>>();
                    for (int i = 0; i < enumType.FieldCount; i++)
                    {
                        enumValues.Add(new Tuple<string, int>(fieldTypes[enumType.FieldIndex + i].Name, (int)fieldTypes[enumType.FieldIndex + i].DataOffset));
                    }
                    return TypeLibrary.AddEnum(enumType.Name, enumValues, enumType);

                case EbxFieldType.DbObject: return null;
            }
            return null;
        }

        internal virtual string ReadString(uint offset)
        {
            if (offset == 0xFFFFFFFF)
                return "";

            long pos = Position;
            Position = stringsOffset + offset;

            string retStr = ReadNullTerminatedString();
            Position = pos;

            return retStr;
        }

        internal CString ReadCString(uint offset) => new CString(ReadString(offset));

        internal ResourceRef ReadResourceRef() => new ResourceRef(ReadULong());

        internal FileRef ReadFileRef()
        {
            uint index = ReadUInt();
            Position += 4;

            return new FileRef(ReadString(index));
        }

        internal virtual PointerRef ReadPointerRef(bool dontRefCount)
        {
            uint index = ReadUInt();
            if ((index >> 0x1F) == 1)
            {
                EbxImportReference import = imports[(int)(index & 0x7FFFFFFF)];

                return new PointerRef(import);
            }
            else if (index == 0)
            {
                return new PointerRef();
            }
            else
            {
                if (!dontRefCount)
                {
                    refCounts[(int)(index - 1)]++;
                }

                return new PointerRef(objects[(int)(index - 1)]);
            }
        }

        internal virtual TypeRef ReadTypeRef()
        {
            string str = ReadString(ReadUInt());
            Position += 4;

            if (str == "")
                return new TypeRef();

            Guid guid = Guid.Empty;
            if (Guid.TryParse(str, out guid))
            {
                if (guid != Guid.Empty)
                    return new TypeRef(guid);
            }

            return new TypeRef(str);
        }

        internal virtual BoxedValueRef ReadBoxedValueRef()
        {
            int index = ReadInt();
            Position += 12;

            if (index == -1)
                return new BoxedValueRef();

            EbxBoxedValue boxedValue = boxedValues[index];
            EbxFieldType subType = EbxFieldType.Inherited;

            long pos = Position;
            Position = boxedValuesOffset + boxedValue.Offset;

            object value = null;
            if ((EbxFieldType)boxedValue.Type == EbxFieldType.Array)
            {
                EbxClass arrayType = GetClass(null, boxedValue.ClassRef);
                EbxField arrayField = GetField(arrayType, arrayType.FieldIndex);

                value = Activator.CreateInstance(typeof(List<>).MakeGenericType(GetTypeFromEbxField(arrayField)));

                index = ReadInt();
                EbxArray array = arrays[index];

                long arrayPos = Position;
                Position = arraysOffset + array.Offset;

                for (int i = 0; i < array.Count; i++)
                {
                    object subValue = ReadField(arrayType, arrayField.DebugType, arrayField.ClassRef, false);
                    value.GetType().GetMethod("Add").Invoke(value, new object[] { subValue });
                }

                subType = arrayField.DebugType;
                Position = arrayPos;
            }
            else
            {
                value = ReadField(null, (EbxFieldType)boxedValue.Type, boxedValue.ClassRef);
                if ((EbxFieldType)boxedValue.Type == EbxFieldType.Enum)
                {
                    object tmpValue = value;
                    EbxClass enumClass = GetClass(null, boxedValue.ClassRef);
                    value = Enum.Parse(GetType(enumClass), tmpValue.ToString());
                }
            }
            Position = pos;

            return new BoxedValueRef(value, (EbxFieldType)boxedValue.Type, subType);
        }

        internal int HashString(string strToHash)
        {
            int hash = 5381;
            for (int i = 0; i < strToHash.Length; i++)
            {
                byte B = (byte)strToHash[i];
                hash = (hash * 33) ^ B;
            }

            return hash;
        }
    }
}
