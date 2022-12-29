using FrostySdk.Attributes;
using FrostySdk.Ebx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FrostySdk.IO
{
    [Flags]
    public enum EbxWriteFlags
    {
        None = 0,
        IncludeTransient = 1,
        DoNotSort
    }

    public class EbxBaseWriter : NativeWriter
    {
        public static EbxBaseWriter CreateProjectWriter(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
        {
            return new EbxWriter(inStream, inFlags, leaveOpen);
        }

        public static EbxBaseWriter CreateWriter(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
        {
            if ((ProfilesLibrary.EbxVersion & 1) != 0)
            {
                return new EbxWriterV2(inStream, inFlags, leaveOpen);
            }
            else if (ProfilesLibrary.EbxVersion == 6)
            {
                return new EbxWriterRiff(inStream, inFlags, leaveOpen);
            }
            else
            {
                return new EbxWriter(inStream, inFlags, leaveOpen);
            }
        }

        protected EbxWriteFlags m_flags;
        protected List<string> m_strings = new List<string>();
        protected List<EbxBoxedValue> m_boxedValues = new List<EbxBoxedValue>();
        protected List<byte[]> m_boxedValueData = new List<byte[]>();
        protected NativeWriter m_boxedValueWriter = new NativeWriter(new MemoryStream());

        protected uint m_stringsLength = 0;

        internal EbxBaseWriter(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
            : base(inStream, leaveOpen)
        {
            m_flags = inFlags;
        }

        public virtual void WriteAsset(EbxAsset asset)
        {
        }

        protected virtual byte[] WriteBoxedValueRef(BoxedValueRef value)
        {
            // @todo: Does not at all handle boxed value arrays
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                object obj = value.Value;
                switch (value.Type)
                {
                    case EbxFieldType.TypeRef: writer.Write((ulong)AddString((TypeRef)obj)); break;
                    case EbxFieldType.FileRef: writer.Write((ulong)AddString((FileRef)obj)); break;
                    case EbxFieldType.CString: writer.Write(AddString((CString)obj)); break;
                    case EbxFieldType.Enum: writer.Write((int)obj); break;
                    case EbxFieldType.Float32: writer.Write((float)obj); break;
                    case EbxFieldType.Float64: writer.Write((double)obj); break;
                    case EbxFieldType.Boolean: writer.Write((byte)(((bool)obj) ? 0x01 : 0x00)); break;
                    case EbxFieldType.Int8: writer.Write((sbyte)obj); break;
                    case EbxFieldType.UInt8: writer.Write((byte)obj); break;
                    case EbxFieldType.Int16: writer.Write((short)obj); break;
                    case EbxFieldType.UInt16: writer.Write((ushort)obj); break;
                    case EbxFieldType.Int32: writer.Write((int)obj); break;
                    case EbxFieldType.UInt32: writer.Write((uint)obj); break;
                    case EbxFieldType.Int64: writer.Write((long)obj); break;
                    case EbxFieldType.UInt64: writer.Write((ulong)obj); break;
                    case EbxFieldType.Guid: writer.Write((Guid)obj); break;
                    case EbxFieldType.Sha1: writer.Write((Sha1)obj); break;
                    case EbxFieldType.String: writer.WriteFixedSizedString((string)obj, 32); break;
                    case EbxFieldType.ResourceRef: writer.Write((ResourceRef)obj); break;

                    default: throw new InvalidDataException($"Unhandled field type: {value.Type}");
                }

                return writer.ToByteArray();;
            }
        }

        protected virtual uint AddString(string stringToAdd)
        {
            if (stringToAdd == "")
                return 0xFFFFFFFF;

            uint offset = 0;
            if (m_strings.Contains(stringToAdd))
            {
                for (int i = 0; i < m_strings.Count; i++)
                {
                    if (m_strings[i] == stringToAdd)
                        break;
                    offset += (uint)(m_strings[i].Length + 1);
                }
            }
            else
            {
                offset = m_stringsLength;
                m_strings.Add(stringToAdd);
                m_stringsLength += (uint)(stringToAdd.Length + 1);
            }

            return offset;
        }
    }

    public class EbxWriter : EbxBaseWriter
    {
        public List<object> Objects => sortedObjs;
        public List<Guid> Dependencies => dependencies;

        private List<object> objsToProcess = new List<object>();
        private List<Type> typesToProcess = new List<Type>();
        private List<EbxFieldMetaAttribute> arrayTypes = new List<EbxFieldMetaAttribute>();

        private List<object> objs = new List<object>();
        private List<object> sortedObjs = new List<object>();
        private List<Guid> dependencies = new List<Guid>();

        private List<EbxClass> classTypes = new List<EbxClass>();
        private List<EbxField> fieldTypes = new List<EbxField>();
        private List<string> typeNames = new List<string>();
        private List<EbxImportReference> imports = new List<EbxImportReference>();
        
        private byte[] data = null;
        private List<EbxInstance> instances = new List<EbxInstance>();
        private List<EbxArray> arrays = new List<EbxArray>();
        private List<byte[]> arrayData = new List<byte[]>();
        
        private ushort uniqueClassCount = 0;
        private ushort exportedCount = 0;

        internal EbxWriter(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
            : base(inStream, inFlags, leaveOpen)
        {
            m_flags = inFlags;
        }

        public override void WriteAsset(EbxAsset asset)
        {
            if (m_flags.HasFlag(EbxWriteFlags.DoNotSort))
            {
                foreach (object obj in asset.Objects)
                    ExtractClass(obj.GetType(), obj);
                WriteEbx(asset.FileGuid);
            }
            else
            {
                List<object> writeObjs = new List<object>();
                foreach (object obj in asset.RootObjects)
                    writeObjs.Add(obj);
                WriteEbxObjects(writeObjs, asset.FileGuid);
            }
        }

        public void WriteEbxObject(object inObj, Guid fileGuid)
        {
            List<object> writeObjs = new List<object>() { inObj };
            WriteEbxObjects(writeObjs, fileGuid);
        }

        public void WriteEbxObjects(List<object> inObjects, Guid fileGuid)
        {
            List<object> subObjs = new List<object>();
            subObjs.AddRange(inObjects);

            while (subObjs.Count > 0)
            {
                object nxtObj = subObjs[0];
                subObjs.RemoveAt(0);

                // add all sub objects for extraction
                subObjs.AddRange(ExtractClass(nxtObj.GetType(), nxtObj));
            }

            WriteEbx(fileGuid);
        }

        private void WriteEbx(Guid fileGuid)
        {
            uint stringsOffset = 0;
            uint stringsAndDataLen = 0;
            uint boxedValueOffset = 0;
            ushort typeNamesLen = 0;
            uint dataLen = 0;

            //objsToProcess.Reverse();
            foreach (object obj in objsToProcess)
                ProcessClass(obj.GetType());
            for (int i = 0; i < typesToProcess.Count; i++)
                ProcessType(i);

            ProcessData();

            Write((int)(((ProfilesLibrary.EbxVersion & 4) != 0) ? EbxVersion.Version4 : EbxVersion.Version2));
            Write(0x00); // stringsOffset
            Write(0x00); // stringsAndDataLen
            Write(imports.Count);
            Write((ushort)instances.Count);
            Write(exportedCount);
            Write(uniqueClassCount);
            Write((ushort)classTypes.Count);
            Write((ushort)fieldTypes.Count);
            Write((ushort)0x00); // typeNamesLen
            Write(0x00); // stringsLen
            Write(arrays.Count);
            Write(0x00); // dataLen
            Write(fileGuid);

            if (((ProfilesLibrary.EbxVersion & 4) != 0))
            {
                Write(0xDEADBEEF);
                Write(0xDEADBEEF);
            }
            else
            {
                WritePadding(16);
            }

            foreach (EbxImportReference importRef in imports)
            {
                Write(importRef.FileGuid);
                Write(importRef.ClassGuid);
            }

            WritePadding(16);

            long offset = Position;
            for (int i = 0; i < typeNames.Count; i++)
                WriteNullTerminatedString(typeNames[i]);
            WritePadding(16);

            typeNamesLen = (ushort)(Position - offset);

            foreach (EbxField fieldType in fieldTypes)
            {
                ushort type = fieldType.Type;
                if (((ProfilesLibrary.EbxVersion & 4) != 0))
                    type <<= 1;

                Write(HashString(fieldType.Name));
                Write(type);
                Write(fieldType.ClassRef);
                Write(fieldType.DataOffset);
                Write(fieldType.SecondOffset);
            }

            foreach (EbxClass classType in classTypes)
            {
                ushort type = classType.Type;
                if (((ProfilesLibrary.EbxVersion & 4) != 0))
                    type <<= 1;

                Write(HashString(classType.Name));
                Write(classType.FieldIndex);
                Write((byte)classType.FieldCount);
                Write(classType.Alignment);
                Write(type);
                Write(classType.Size);
                Write(classType.SecondSize);
            }

            foreach (EbxInstance inst in instances)
            {
                Write(inst.ClassRef);
                Write(inst.Count);
            }
            WritePadding(16);

            long arraysOffset = Position;
            for (int i = 0; i < arrays.Count; i++)
            {
                Write(0);
                Write(0);
                Write(0);
            }
            WritePadding(16);

            long boxedValueRefOffset = Position;
            for (int i = 0; i < m_boxedValues.Count; i++)
            {
                Write(0);
                Write(0);
            }
            WritePadding(16);

            stringsOffset = (uint)Position;

            foreach (string str in m_strings)
                WriteNullTerminatedString(str);
            WritePadding(16);

            m_stringsLength = (uint)(Position - stringsOffset);

            offset = Position;
            Write(data);
            Write((byte)0x00);
            WritePadding(16);

            dataLen = (uint)(Position - offset);

            if (arrays.Count > 0)
            {
                offset = Position;
                for (int i = 0; i < arrays.Count; i++)
                {
                    //if (arrays[i].Count > 0)
                    {
                        EbxArray array = arrays[i];

                        Write(array.Count);

                        array.Offset = (uint)(Position - offset);

                        Write(arrayData[i]);
                        if (i != arrays.Count - 1)
                            Write(0x00);

                        WritePadding(16);
                        Position -= 4;

                        arrays[i] = array;
                    }
                }
                Position += 4;
                WritePadding(16);
            }

            boxedValueOffset = (uint)(Position - m_stringsLength - stringsOffset);
            if (m_boxedValueWriter.Position > 0)
            {
                Write(m_boxedValueWriter.ToByteArray());
                WritePadding(16);
                m_boxedValueWriter.Close();
            }
            stringsAndDataLen = (uint)(Position - stringsOffset);

            Position = 0x04;
            Write(stringsOffset);
            Write(stringsAndDataLen);

            Position = 0x1A;
            Write(typeNamesLen);
            Write(m_stringsLength);

            Position = 0x24;
            Write(dataLen);

            Position = arraysOffset;
            for (int i = 0; i < arrays.Count; i++)
            {
                Write(arrays[i].Offset);
                Write(arrays[i].Count);
                Write(arrays[i].ClassRef);
            }

            if (((ProfilesLibrary.EbxVersion & 4) != 0))
            {
                Position = 0x38;
                Write(m_boxedValues.Count);
                Write(boxedValueOffset);

                Position = boxedValueRefOffset;
                for (int i = 0; i < m_boxedValues.Count; i++)
                {
                    Write(m_boxedValues[i].Offset);
                    Write(m_boxedValues[i].ClassRef);
                    Write(m_boxedValues[i].Type);
                }
            }
        }

        private List<object> ExtractClass(Type type, object obj, bool add = true)
        {
            if (add)
            {
                if (objsToProcess.Contains(obj))
                    return new List<object>();

                objsToProcess.Add(obj);
                objs.Add(obj);
            }

            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            List<object> retObjects = new List<object>();

            foreach (PropertyInfo pi in pis)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                    continue;

                if (pi.PropertyType == typeof(PointerRef))
                {
                    PointerRef value = (PointerRef)pi.GetValue(obj);
                    if (value.Type == PointerRefType.Internal)
                        retObjects.Add(value.Internal);
                    //ExtractClass(value.Internal.GetType(), value.Internal);
                    else if (value.Type == PointerRefType.External)
                    {
                        if (!imports.Contains(value.External))
                            imports.Add(value.External);
                    }
                }
                else if (pi.PropertyType.Namespace == "FrostySdk.Ebx" && pi.PropertyType.BaseType != typeof(Enum))
                {
                    object structObj = pi.GetValue(obj);
                    retObjects.AddRange(ExtractClass(structObj.GetType(), structObj, false));
                }
                else if (pi.PropertyType.Name == "List`1")
                {
                    Type arrayType = pi.PropertyType;
                    int count = (int)arrayType.GetMethod("get_Count").Invoke(pi.GetValue(obj), null);

                    if (count > 0)
                    {
                        if (arrayType.GenericTypeArguments[0] == typeof(PointerRef))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                PointerRef value = (PointerRef)arrayType.GetMethod("get_Item").Invoke(pi.GetValue(obj), new object[] { i });
                                if (value.Type == PointerRefType.Internal)
                                    retObjects.Add(value.Internal);
                                //ExtractClass(value.Internal.GetType(), value.Internal);
                                else if (value.Type == PointerRefType.External)
                                {
                                    if (!imports.Contains(value.External))
                                        imports.Add(value.External);
                                }
                            }
                        }
                        else if (arrayType.GenericTypeArguments[0].Namespace == "FrostySdk.Ebx" && arrayType.GenericTypeArguments[0].BaseType != typeof(Enum))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                object value = arrayType.GetMethod("get_Item").Invoke(pi.GetValue(obj), new object[] { i });
                                retObjects.AddRange(ExtractClass(value.GetType(), value, false));
                            }
                        }
                    }
                }
            }

            if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
                retObjects.AddRange(ExtractClass(type.BaseType, obj, false));

            return retObjects;
        }

        private ushort ProcessClass(Type objType)
        {
            bool inherited = false;
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                ProcessClass(objType.BaseType);
                inherited = true;
            }

            int index = FindExistingClass(objType);
            if (index != -1)
                return (ushort)index;

            EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
            PropertyInfo[] allProps = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<PropertyInfo> pis = new List<PropertyInfo>();

            foreach (PropertyInfo pi in allProps)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                    continue;
                pis.Add(pi);
            }

            index = AddClass(objType.Name, fieldTypes.Count, (byte)(pis.Count + ((inherited) ? 1 : 0)),
                cta.Alignment, cta.Flags, cta.Size, 0, objType);

            // Inherited
            if (inherited)
                AddTypeName("$");

            if (objType.IsEnum)
            {
                //uint enumIndex = 0;
                string[] enumNames = objType.GetEnumNames();

                foreach (string enumName in enumNames)
                {
                    AddTypeName(enumName);
                    //enumIndex++;
                }
            }
            else
            {
                // Fields
                foreach (PropertyInfo pi in pis)
                {
                    EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                    EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);

                    if (ebxType == EbxFieldType.Struct)
                    {
                        Type structType = pi.PropertyType;
                        ProcessClass(structType);
                    }
                    else if (ebxType == EbxFieldType.Enum)
                    {
                        Type enumType = pi.PropertyType;
                        ProcessClass(enumType);
                    }
                    else if (ebxType == EbxFieldType.Array)
                    {
                        ebxType = (EbxFieldType)((fta.ArrayFlags >> 4) & 0x1F);

                        Type arrayType = pi.PropertyType;
                        if (FindExistingClass(arrayType) == -1)
                        {

                            if (!typesToProcess.Contains(arrayType))
                            {
                                arrayTypes.Add(fta);
                                AddClass("array", 0, 1, 4, fta.Flags, 4, 0, arrayType);
                            }

                            if (ebxType == EbxFieldType.Struct)
                            {
                                Type structType = arrayType.GenericTypeArguments[0];
                                ProcessClass(structType);
                            }
                            else if (ebxType == EbxFieldType.Enum)
                            {
                                Type enumType = arrayType.GenericTypeArguments[0];
                                ProcessClass(enumType);
                            }

                            AddTypeName("member");
                        }
                    }

                    AddTypeName(pi.Name);
                }
            }

            return (ushort)index;
        }

        private void ProcessType(int index)
        {
            Type type = typesToProcess[index];

            EbxClass classType = classTypes[index];
            classType.FieldIndex = fieldTypes.Count;

            if (classType.DebugType == EbxFieldType.Array)
            {
                EbxFieldMetaAttribute ata = arrayTypes[0];
                arrayTypes.RemoveAt(0);

                classType.FieldCount = 1;

                ushort arrayClassRef = (ushort)FindExistingClass(type.GenericTypeArguments[0]);
                if (arrayClassRef == 0xFFFF)
                    arrayClassRef = 0;

                AddField("member", ata.ArrayFlags, arrayClassRef, 0, 0);
            }
            else if (classType.DebugType == EbxFieldType.Enum)
            {
                string[] enumNames = type.GetEnumNames();
                Array enumValues = type.GetEnumValues();

                classType.FieldCount = (byte)enumNames.Length;

                for (int i = 0; i < enumNames.Length; i++)
                {
                    int enumValue = (int)enumValues.GetValue(i);
                    AddField(enumNames[i], 0, 0, (uint)enumValue, (uint)enumValue);
                }
            }
            else
            {
                PropertyInfo[] allProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                List<PropertyInfo> pis = new List<PropertyInfo>();

                foreach (PropertyInfo pi in allProps)
                {
                    // ignore transients if saving to project
                    if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                        continue;

                    // ignore instance guid
                    if (pi.Name.Equals("__InstanceGuid"))
                        continue;

                    pis.Add(pi);
                }

                classType.FieldCount = (byte)pis.Count;

                if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
                {
                    ushort classIndex = (ushort)FindExistingClass(type.BaseType);

                    classType.FieldCount++;

                    // set offset of inherited value to the greater of the class alignment or 8
                    AddField("$", 0, classIndex, (uint)(type.GetCustomAttribute<ForceAlignAttribute>() != null ? 16 : 8), 0);
                }

                foreach (PropertyInfo pi in pis)
                    ProcessField(pi);
            }

            classTypes[index] = classType;
        }

        private void ProcessField(PropertyInfo pi)
        {
            ushort classRef = 0;

            EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
            EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);

            Type propType = pi.PropertyType;
            classRef = (ushort)typesToProcess.FindIndex((Type value) => { return value == propType; });
            if (classRef == 0xFFFF)
                classRef = 0;

            AddField(pi.Name, fta.Flags, classRef, fta.Offset, 0);
        }

        private void ProcessData()
        {
            List<Type> uniqueTypes = new List<Type>();
            List<object> exportedObjs = new List<object>();
            List<object> otherObjs = new List<object>();

            for (int i = 0; i < objs.Count; i++)
            {
                dynamic obj = objs[i];
                AssetClassGuid guid = obj.GetInstanceGuid();
                if (guid.IsExported)
                    exportedObjs.Add(obj);
                else
                    otherObjs.Add(obj);
            }

            object root = exportedObjs[0];
            exportedObjs.RemoveAt(0);

            exportedObjs.Sort((dynamic a, dynamic b) =>
            {
                AssetClassGuid guidA = a.GetInstanceGuid();
                AssetClassGuid guidB = b.GetInstanceGuid();

                byte[] bA = guidA.ExportedGuid.ToByteArray();
                byte[] bB = guidB.ExportedGuid.ToByteArray();

                uint idA = (uint)(bA[0] << 24 | bA[1] << 16 | bA[2] << 8 | bA[3]);
                uint idB = (uint)(bB[0] << 24 | bB[1] << 16 | bB[2] << 8 | bB[3]);

                return idA.CompareTo(idB);
            });

            otherObjs.Sort((object a, object b) => a.GetType().Name.CompareTo(b.GetType().Name));

            sortedObjs.Add(root);
            sortedObjs.AddRange(exportedObjs);
            sortedObjs.AddRange(otherObjs);

            MemoryStream dataStream = new MemoryStream();
            using (NativeWriter writer = new NativeWriter(dataStream))
            {
                Type type = sortedObjs[0].GetType();
                int classIdx = FindExistingClass(type);
                EbxClass classType = classTypes[classIdx];

                EbxInstance inst = new EbxInstance()
                {
                    ClassRef = (ushort)classIdx,
                    Count = 0,
                    IsExported = true
                };

                ushort count = 0;
                exportedCount++;

                for (int i = 0; i < sortedObjs.Count; i++)
                {
                    AssetClassGuid guid = ((dynamic)sortedObjs[i]).GetInstanceGuid();

                    type = sortedObjs[i].GetType();
                    classIdx = FindExistingClass(type);
                    classType = classTypes[classIdx];

                    if (!uniqueTypes.Contains(type))
                        uniqueTypes.Add(type);

                    //instances.Add(new EbxInstance()
                    //{
                    //    ClassRef = (ushort)classIdx,
                    //    Count = 1,
                    //    IsExported = guid != Guid.Empty
                    //});
                    //exportedCount += (ushort)((instances[instances.Count - 1].IsExported) ? 1 : 0);

                    if (classIdx != inst.ClassRef || inst.IsExported && !guid.IsExported)
                    {
                        inst.Count = count;
                        instances.Add(inst);

                        inst = new EbxInstance
                        {
                            ClassRef = (ushort)classIdx,
                            IsExported = guid.IsExported
                        };
                        exportedCount += (ushort)((inst.IsExported) ? 1 : 0);

                        count = 0;
                    }

                    writer.WritePadding(classType.Alignment);

                    if (guid.IsExported)
                        writer.Write(guid.ExportedGuid);

                    if (classType.Alignment != 0x04)
                        writer.Write((ulong)0);

                    WriteClass(sortedObjs[i], type, writer);
                    count++;
                }

                // Add final instance
                inst.Count = count;
                instances.Add(inst);
            }

            data = dataStream.ToArray();
            uniqueClassCount = (ushort)uniqueTypes.Count;
        }

        private void WriteClass(object obj, Type objType, NativeWriter writer)
        {
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
                WriteClass(obj, objType.BaseType, writer);

            PropertyInfo[] pis = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            EbxClass classType = classTypes[FindExistingClass(objType)];

            foreach (PropertyInfo pi in pis)
            {
                // ignore transients if not saving to project
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                    continue;

                // ignore the instance guid
                if (pi.Name.Equals("__InstanceGuid"))
                    continue;

                EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                bool isReference = pi.GetCustomAttribute<IsReferenceAttribute>() != null;

                EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);
                WriteField(pi.GetValue(obj), ebxType, classType.Alignment, writer, isReference);
            }

            writer.WritePadding(classType.Alignment);
        }

        private void WriteField(object obj, EbxFieldType ebxType, byte classAlignment, NativeWriter writer, bool isReference)
        {
            if (ebxType == EbxFieldType.ResourceRef
                || ebxType == EbxFieldType.TypeRef
                || ebxType == EbxFieldType.FileRef
                || ebxType == EbxFieldType.BoxedValueRef
                || ebxType == EbxFieldType.UInt64
                || ebxType == EbxFieldType.Int64
                || ebxType == EbxFieldType.Float64)
            {
                writer.WritePadding(8);
            }
            else if (ebxType == EbxFieldType.Array || ebxType == EbxFieldType.Pointer)
                writer.WritePadding(4);

            switch (ebxType)
            {
                case EbxFieldType.TypeRef: writer.Write((ulong)AddString((TypeRef)obj)); break;
                case EbxFieldType.FileRef: writer.Write((ulong)AddString((FileRef)obj)); break;
                case EbxFieldType.CString: writer.Write(AddString((CString)obj)); break;

                case EbxFieldType.Pointer:
                    {
                        PointerRef pointer = (PointerRef)obj;
                        uint pointerIndex = 0;

                        if (pointer.Type == PointerRefType.External)
                        {
                            int importIdx = imports.FindIndex((EbxImportReference value) => value == pointer.External);
                            pointerIndex = (uint)(importIdx | 0x80000000);

                            if (isReference && !dependencies.Contains(imports[importIdx].FileGuid))
                                dependencies.Add(imports[importIdx].FileGuid);
                        }
                        else if (pointer.Type == PointerRefType.Internal)
                            pointerIndex = (uint)(sortedObjs.FindIndex((object value) => value == pointer.Internal) + 1);

                        writer.Write(pointerIndex);
                    }
                    break;

                case EbxFieldType.Struct:
                    {
                        object structValue = obj;
                        Type structType = structValue.GetType();

                        EbxClass structClassType = classTypes[FindExistingClass(structType)];
                        writer.WritePadding(structClassType.Alignment);

                        WriteClass(structValue, structType, writer);
                    }
                    break;

                case EbxFieldType.Array:
                    {
                        int arrayClassIdx = typesToProcess.FindIndex((Type item) => item == obj.GetType());
                        int arrayIdx = 0;

                        EbxClass arrayClassType = classTypes[arrayClassIdx];
                        EbxField arrayFieldType = fieldTypes[arrayClassType.FieldIndex];

                        ebxType = arrayFieldType.DebugType;

                        Type arrayType = obj.GetType();
                        int count = (int)arrayType.GetMethod("get_Count").Invoke(obj, null);

                        if (arrays.Count == 0)
                        {
                            arrays.Add(
                                new EbxArray()
                                {
                                    Count = 0,
                                    ClassRef = arrayClassIdx
                                });
                            arrayData.Add(new byte[] { });
                        }

                        MemoryStream arrayStream = new MemoryStream();
                        using (NativeWriter arrayWriter = new NativeWriter(arrayStream))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                object subValue = arrayType.GetMethod("get_Item").Invoke(obj, new object[] { i });
                                Type subValueType = subValue.GetType();

                                WriteField(subValue, ebxType, classAlignment, arrayWriter, isReference);
                            }
                        }

                        if (count != 0)
                        {
                            arrayIdx = arrays.Count;
                            arrays.Add(
                                new EbxArray()
                                {
                                    Count = (uint)count,
                                    ClassRef = arrayClassIdx
                                });
                            arrayData.Add(arrayStream.ToArray());
                        }
                        writer.Write(arrayIdx);
                    }
                    break;

                case EbxFieldType.Enum: writer.Write((int)obj); break;
                case EbxFieldType.Float32: writer.Write((float)obj); break;
                case EbxFieldType.Float64: writer.Write((double)obj); break;
                case EbxFieldType.Boolean: writer.Write((byte)(((bool)obj) ? 0x01 : 0x00)); break;
                case EbxFieldType.Int8: writer.Write((sbyte)obj); break;
                case EbxFieldType.UInt8: writer.Write((byte)obj); break;
                case EbxFieldType.Int16: writer.Write((short)obj); break;
                case EbxFieldType.UInt16: writer.Write((ushort)obj); break;
                case EbxFieldType.Int32: writer.Write((int)obj); break;
                case EbxFieldType.UInt32: writer.Write((uint)obj); break;
                case EbxFieldType.Int64: writer.Write((long)obj); break;
                case EbxFieldType.UInt64: writer.Write((ulong)obj); break;
                case EbxFieldType.Guid: writer.Write((Guid)obj); break;
                case EbxFieldType.Sha1: writer.Write((Sha1)obj); break;
                case EbxFieldType.String: writer.WriteFixedSizedString((string)obj, 32); break;
                case EbxFieldType.ResourceRef: writer.Write((ResourceRef)obj); break;
                case EbxFieldType.BoxedValueRef:
                    {
                        BoxedValueRef value = (BoxedValueRef)obj;
                        int index = m_boxedValues.Count;

                        if (value.Type == EbxFieldType.Inherited)
                        {
                            index = -1;
                        }
                        else
                        {
                            m_boxedValueWriter.Write(0);
                            EbxBoxedValue boxedValue = new EbxBoxedValue() { Offset = (uint)m_boxedValueWriter.Position, Type = (ushort)value.Type };
                            if (value.Type == EbxFieldType.Enum)
                            {
                                boxedValue.ClassRef = ProcessClass(value.Value.GetType());
                            }

                            if (value.Type == EbxFieldType.Struct)
                            {
                                int count = typesToProcess.Count;
                                boxedValue.ClassRef = ProcessClass(value.Value.GetType());
                                if (count != typesToProcess.Count)
                                    ProcessType((int)boxedValue.ClassRef);

                                WriteField(value.Value, EbxFieldType.Struct, 0, m_boxedValueWriter, isReference);
                            }
                            else
                            {
                                m_boxedValueWriter.Write(WriteBoxedValueRef(value));
                            }
                            m_boxedValues.Add(boxedValue);
                        }

                        writer.Write(index);
                        writer.Write((ulong)0);
                        writer.Write((uint)0);
                    }
                    break;


                default: throw new InvalidDataException("Error");
            }
        }

        private int FindExistingClass(Type inType)
        {
            return classTypes.FindIndex((EbxClass value) => { return value.Name == inType.Name; });
        }

        private void AddTypeName(string inName)
        {
            if (typeNames.Contains(inName))
                return;
            typeNames.Add(inName);
        }

        private int AddClass(string name, int fieldIndex, byte fieldCount, byte alignment, ushort type, ushort size, ushort secondSize, Type classType)
        {
            classTypes.Add(new EbxClass()
            {
                Name = name,
                FieldIndex = fieldIndex,
                FieldCount = fieldCount,
                Alignment = alignment,
                Type = type,
                Size = size,
                SecondSize = secondSize
            });

            AddTypeName(name);
            typesToProcess.Add(classType);

            return (classTypes.Count - 1);
        }

        private void AddField(string name, ushort type, ushort classRef, uint dataOffset, uint secondOffset)
        {
            fieldTypes.Add(new EbxField()
            {
                Name = name,
                Type = type,
                ClassRef = classRef,
                DataOffset = dataOffset,
                SecondOffset = secondOffset
            });
            AddTypeName(name);
        }

        private int HashString(string strToHash)
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

    public class EbxWriterV2 : EbxBaseWriter
    {
        public List<object> Objects => sortedObjs;
        public List<Guid> Dependencies => dependencies;

        private List<object> objsToProcess = new List<object>();
        private List<Type> typesToProcess = new List<Type>();
        private List<EbxFieldMetaAttribute> arrayTypes = new List<EbxFieldMetaAttribute>();

        private List<object> objs = new List<object>();
        private List<object> sortedObjs = new List<object>();
        private List<Guid> dependencies = new List<Guid>();

        private List<EbxClass> classTypes = new List<EbxClass>();
        private List<Guid> classGuids = new List<Guid>();
        private List<EbxField> fieldTypes = new List<EbxField>();
        private List<string> typeNames = new List<string>();
        private List<EbxImportReference> imports = new List<EbxImportReference>();
        private List<string> strings = new List<string>();
        private byte[] data = null;
        private List<EbxInstance> instances = new List<EbxInstance>();
        private List<EbxArray> arrays = new List<EbxArray>();
        private List<byte[]> arrayData = new List<byte[]>();

        private ushort uniqueClassCount = 0;
        private ushort exportedCount = 0;

        internal EbxWriterV2(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
            : base(inStream, inFlags, leaveOpen)
        {
        }

        public override void WriteAsset(EbxAsset asset)
        {
            if (m_flags.HasFlag(EbxWriteFlags.DoNotSort))
            {
                foreach (object obj in asset.Objects)
                    ExtractClass(obj.GetType(), obj);
                WriteEbx(asset.FileGuid);
            }
            else
            {
                List<object> writeObjs = new List<object>();
                foreach (object obj in asset.RootObjects)
                    writeObjs.Add(obj);
                WriteEbxObjects(writeObjs, asset.FileGuid);
            }
        }

        public void WriteEbxObject(object inObj, Guid fileGuid)
        {
            List<object> writeObjs = new List<object>() { inObj };
            WriteEbxObjects(writeObjs, fileGuid);
        }

        public void WriteEbxObjects(List<object> inObjects, Guid fileGuid)
        {
            List<object> subObjs = new List<object>();
            subObjs.AddRange(inObjects);

            while (subObjs.Count > 0)
            {
                object nxtObj = subObjs[0];
                subObjs.RemoveAt(0);

                // add all sub objects for extraction
                subObjs.AddRange(ExtractClass(nxtObj.GetType(), nxtObj));
            }

            WriteEbx(fileGuid);
        }

        private void WriteEbx(Guid fileGuid)
        {
            uint stringsOffset = 0;
            uint stringsAndDataLen = 0;
            uint boxedValueOffset = 0;
            ushort typeNamesLen = 0;
            uint dataLen = 0;

            //objsToProcess.Reverse();
            foreach (object obj in objsToProcess)
                ProcessClass(obj.GetType());
            for (int i = 0; i < typesToProcess.Count; i++)
                ProcessType(i);

            ProcessData();

            Write((int)((ProfilesLibrary.EbxVersion == 4) ? EbxVersion.Version4 : EbxVersion.Version2));
            Write(0x00); // stringsOffset
            Write(0x00); // stringsAndDataLen
            Write(imports.Count);
            Write((ushort)instances.Count);
            Write(exportedCount);
            Write(uniqueClassCount);
            Write((ushort)classGuids.Count);
            Write((ushort)0x00);
            Write((ushort)0x00); // typeNamesLen
            Write(0x00); // stringsLen
            Write(arrays.Count);
            Write(0x00); // dataLen
            Write(fileGuid);

            if (ProfilesLibrary.EbxVersion == 4)
            {
                Write(0xDEADBEEF);
                Write(0xDEADBEEF);
            }
            else
            {
                WritePadding(16);
            }

            foreach (EbxImportReference importRef in imports)
            {
                Write(importRef.FileGuid);
                Write(importRef.ClassGuid);
            }

            //WritePadding(16);

            long offset = Position;
            //for (int i = 0; i < typeNames.Count; i++)
            //    WriteNullTerminatedString(typeNames[i]);
            //WritePadding(16);

            typeNamesLen = (ushort)(Position - offset);

            //foreach (EbxField fieldType in fieldTypes)
            //{
            //    ushort type = fieldType.Type;
            //    if (ProfilesLibrary.EbxVersion == 4)
            //        type <<= 1;

            //    Write(HashString(fieldType.Name));
            //    Write(type);
            //    Write(fieldType.ClassRef);
            //    Write(fieldType.DataOffset);
            //    Write(fieldType.SecondOffset);
            //}

            //foreach (EbxClass classType in classTypes)
            //{
            //    ushort type = classType.Type;
            //    if (ProfilesLibrary.EbxVersion == 4)
            //        type <<= 1;

            //    Write(HashString(classType.Name));
            //    Write(classType.FieldIndex);
            //    Write(classType.FieldCount);
            //    Write(classType.Alignment);
            //    Write(type);
            //    Write(classType.Size);
            //    Write(classType.SecondSize);
            //}

            foreach (Guid guid in classGuids)
            {
                Write(guid);
            }

            foreach (EbxInstance inst in instances)
            {
                Write(inst.ClassRef);
                Write(inst.Count);
            }
            WritePadding(16);

            long arraysOffset = Position;
            for (int i = 0; i < arrays.Count; i++)
            {
                Write(0);
                Write(0);
                Write(0);
            }
            WritePadding(16);

            long boxedValueRefOffset = Position;
            for (int i = 0; i < m_boxedValues.Count; i++)
            {
                Write(0);
                Write(0);
            }
            WritePadding(16);

            stringsOffset = (uint)Position;

            foreach (string str in strings)
                WriteNullTerminatedString(str);
            WritePadding(16);

            m_stringsLength = (uint)(Position - stringsOffset);

            offset = Position;
            Write(data);
            Write((byte)0x00);
            WritePadding(16);

            dataLen = (uint)(Position - offset);

            if (arrays.Count > 0)
            {
                offset = Position;
                for (int i = 0; i < arrays.Count; i++)
                {
                    //if (arrays[i].Count > 0)
                    {
                        EbxArray array = arrays[i];

                        Write(array.Count);

                        array.Offset = (uint)(Position - offset);

                        Write(arrayData[i]);
                        if (i != arrays.Count - 1)
                            Write(0x00);

                        WritePadding(16);
                        Position -= 4;

                        arrays[i] = array;
                    }
                }
                Position += 4;
                WritePadding(16);
            }

            
            boxedValueOffset = (uint)(Position - m_stringsLength - stringsOffset);
            if (m_boxedValueData.Count > 0)
            {
                for (int i = 0; i < m_boxedValueData.Count; i++)
                {
                    var boxedValue = m_boxedValues[i];

                    Write(0);
                    boxedValue.Offset = (uint)(Position - boxedValueOffset);
                    Write(m_boxedValueData[i]);

                    m_boxedValues[i] = boxedValue;
                }
                WritePadding(16);
            }
            stringsAndDataLen = (uint)(Position - stringsOffset);

            Position = 0x04;
            Write(stringsOffset);
            Write(stringsAndDataLen);

            Position = 0x1A;
            //Write(typeNamesLen);
            Write((ushort)0x00);
            Write(m_stringsLength);

            Position = 0x24;
            Write(dataLen);

            Position = arraysOffset;
            for (int i = 0; i < arrays.Count; i++)
            {
                Write(arrays[i].Offset);
                Write(arrays[i].Count);
                Write(arrays[i].ClassRef);
            }

            if (ProfilesLibrary.EbxVersion == 4)
            {
                Position = 0x38;
                Write(m_boxedValueData.Count);
                Write(boxedValueOffset);

                Position = boxedValueRefOffset;
                for (int i = 0; i < m_boxedValues.Count; i++)
                {
                    Write(m_boxedValues[i].Offset);
                    Write(m_boxedValues[i].ClassRef);
                    Write(m_boxedValues[i].Type);
                }
            }
        }

        private List<object> ExtractClass(Type type, object obj, bool add = true)
        {
            if (add)
            {
                if (objsToProcess.Contains(obj))
                    return new List<object>();

                objsToProcess.Add(obj);
                objs.Add(obj);
            }

            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            List<object> retObjects = new List<object>();

            foreach (PropertyInfo pi in pis)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                    continue;

                if (pi.PropertyType == typeof(PointerRef))
                {
                    PointerRef value = (PointerRef)pi.GetValue(obj);
                    if (value.Type == PointerRefType.Internal)
                        retObjects.Add(value.Internal);
                    //ExtractClass(value.Internal.GetType(), value.Internal);
                    else if (value.Type == PointerRefType.External)
                    {
                        if (!imports.Contains(value.External))
                            imports.Add(value.External);
                    }
                }
                else if (pi.PropertyType.Namespace == "FrostySdk.Ebx" && pi.PropertyType.BaseType != typeof(Enum))
                {
                    object structObj = pi.GetValue(obj);
                    retObjects.AddRange(ExtractClass(structObj.GetType(), structObj, false));
                }
                else if (pi.PropertyType.Name == "List`1")
                {
                    Type arrayType = pi.PropertyType;
                    int count = (int)arrayType.GetMethod("get_Count").Invoke(pi.GetValue(obj), null);

                    if (count > 0)
                    {
                        if (arrayType.GenericTypeArguments[0] == typeof(PointerRef))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                PointerRef value = (PointerRef)arrayType.GetMethod("get_Item").Invoke(pi.GetValue(obj), new object[] { i });
                                if (value.Type == PointerRefType.Internal)
                                    retObjects.Add(value.Internal);
                                //ExtractClass(value.Internal.GetType(), value.Internal);
                                else if (value.Type == PointerRefType.External)
                                {
                                    if (!imports.Contains(value.External))
                                        imports.Add(value.External);
                                }
                            }
                        }
                        else if (arrayType.GenericTypeArguments[0].Namespace == "FrostySdk.Ebx" && arrayType.GenericTypeArguments[0].BaseType != typeof(Enum))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                object value = arrayType.GetMethod("get_Item").Invoke(pi.GetValue(obj), new object[] { i });
                                retObjects.AddRange(ExtractClass(value.GetType(), value, false));
                            }
                        }
                    }
                }
            }

            if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
                retObjects.AddRange(ExtractClass(type.BaseType, obj, false));

            return retObjects;
        }

        private ushort ProcessClass(Type objType)
        {
            bool inherited = false;
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                ProcessClass(objType.BaseType);
                inherited = true;
            }

            int index = FindExistingClass(objType);
            if (index != -1)
                return (ushort)index;

            EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
            PropertyInfo[] allProps = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<PropertyInfo> pis = new List<PropertyInfo>();

            foreach (PropertyInfo pi in allProps)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                    continue;
                pis.Add(pi);
            }

            index = AddClass(objType.Name, fieldTypes.Count, (byte)(pis.Count + ((inherited) ? 1 : 0)),
                cta.Alignment, cta.Flags, cta.Size, 0, objType);

            // Inherited
            if (inherited)
                AddTypeName("$");

            if (objType.IsEnum)
            {
                //uint enumIndex = 0;
                string[] enumNames = objType.GetEnumNames();

                foreach (string enumName in enumNames)
                {
                    AddTypeName(enumName);
                    //enumIndex++;
                }
            }
            else
            {
                // Fields
                foreach (PropertyInfo pi in pis)
                {
                    EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                    EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);

                    if (ebxType == EbxFieldType.Struct)
                    {
                        Type structType = pi.PropertyType;
                        ProcessClass(structType);
                    }
                    else if (ebxType == EbxFieldType.Enum)
                    {
                        Type enumType = pi.PropertyType;
                        ProcessClass(enumType);
                    }
                    else if (ebxType == EbxFieldType.Array)
                    {
                        ebxType = (EbxFieldType)((fta.ArrayFlags >> 4) & 0x1F);

                        Type arrayType = pi.PropertyType;
                        if (FindExistingClass(arrayType) == -1)
                        {

                            if (!typesToProcess.Contains(arrayType))
                            {
                                arrayTypes.Add(fta);
                                //AddClass("array", 0, 1, 4, fta.Flags, 4, 0, arrayType);
                                AddClass(pi, arrayType);
                            }

                            if (ebxType == EbxFieldType.Struct)
                            {
                                Type structType = arrayType.GenericTypeArguments[0];
                                ProcessClass(structType);
                            }
                            else if (ebxType == EbxFieldType.Enum)
                            {
                                Type enumType = arrayType.GenericTypeArguments[0];
                                ProcessClass(enumType);
                            }

                            AddTypeName("member");
                        }
                    }

                    AddTypeName(pi.Name);
                }
            }

            return (ushort)index;
        }

        private void ProcessType(int index)
        {
            Type type = typesToProcess[index];

            EbxClass classType = classTypes[index];
            //classType.FieldIndex = fieldTypes.Count;

            if (classType.DebugType == EbxFieldType.Array)
            {
                EbxFieldMetaAttribute ata = arrayTypes[0];
                arrayTypes.RemoveAt(0);

                //classType.FieldCount = 1;

                ushort arrayClassRef = (ushort)FindExistingClass(type.GenericTypeArguments[0]);
                if (arrayClassRef == 0xFFFF)
                    arrayClassRef = 0;

                AddField("member", ata.ArrayFlags, arrayClassRef, 0, 0);
            }
            else if (classType.DebugType == EbxFieldType.Enum)
            {
                string[] enumNames = type.GetEnumNames();
                Array enumValues = type.GetEnumValues();

                //classType.FieldCount = (byte)enumNames.Length;

                for (int i = 0; i < enumNames.Length; i++)
                {
                    int enumValue = (int)enumValues.GetValue(i);
                    AddField(enumNames[i], 0, 0, (uint)enumValue, (uint)enumValue);
                }
            }
            else
            {
                PropertyInfo[] allProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                List<PropertyInfo> pis = new List<PropertyInfo>();

                foreach (PropertyInfo pi in allProps)
                {
                    // ignore transients if saving to project
                    if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                        continue;

                    // ignore instance guid
                    if (pi.Name.Equals("__InstanceGuid"))
                        continue;

                    pis.Add(pi);
                }

                //classType.FieldCount = (byte)pis.Count;

                if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
                {
                    ushort classIndex = (ushort)FindExistingClass(type.BaseType);

                    //classType.FieldCount++;

                    // this seems to break V4 ebx, so lets only do it for V2
                    if (ProfilesLibrary.EbxVersion == 2)
                    {
                        // set offset of inherited value to the greater of the class alignment or 8
                        AddField("$", 0, classIndex, (uint)(classType.Alignment < 8 ? 8 : classType.Alignment), 0);
                    }
                    else
                    {
                        AddField("$", 0, classIndex, 8, 0);
                    }
                }

                foreach (PropertyInfo pi in pis)
                    ProcessField(pi);
            }

            //classTypes[index] = classType;
        }

        private void ProcessField(PropertyInfo pi)
        {
            ushort classRef = 0;

            EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
            EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);

            Type propType = pi.PropertyType;
            classRef = (ushort)typesToProcess.FindIndex((Type value) => value == propType);
            if (classRef == 0xFFFF)
                classRef = 0;

            AddField(pi.Name, fta.Flags, classRef, fta.Offset, 0);
        }

        private void ProcessData()
        {
            List<Type> uniqueTypes = new List<Type>();
            List<object> exportedObjs = new List<object>();
            List<object> otherObjs = new List<object>();

            for (int i = 0; i < objs.Count; i++)
            {
                dynamic obj = objs[i];
                AssetClassGuid guid = obj.GetInstanceGuid();
                if (guid.IsExported)
                    exportedObjs.Add(obj);
                else
                    otherObjs.Add(obj);
            }

            object root = exportedObjs[0];
            exportedObjs.RemoveAt(0);

            exportedObjs.Sort((dynamic a, dynamic b) =>
            {
                AssetClassGuid guidA = a.GetInstanceGuid();
                AssetClassGuid guidB = b.GetInstanceGuid();

                byte[] bA = guidA.ExportedGuid.ToByteArray();
                byte[] bB = guidB.ExportedGuid.ToByteArray();

                uint idA = (uint)(bA[0] << 24 | bA[1] << 16 | bA[2] << 8 | bA[3]);
                uint idB = (uint)(bB[0] << 24 | bB[1] << 16 | bB[2] << 8 | bB[3]);

                return idA.CompareTo(idB);
            });

            otherObjs.Sort((object a, object b) => a.GetType().Name.CompareTo(b.GetType().Name));

            sortedObjs.Add(root);
            sortedObjs.AddRange(exportedObjs);
            sortedObjs.AddRange(otherObjs);

            MemoryStream dataStream = new MemoryStream();
            using (NativeWriter writer = new NativeWriter(dataStream))
            {
                Type type = sortedObjs[0].GetType();
                int classIdx = FindExistingClass(type);
                EbxClass classType = classTypes[classIdx];

                EbxInstance inst = new EbxInstance()
                {
                    ClassRef = (ushort)classIdx,
                    Count = 0,
                    IsExported = true
                };

                ushort count = 0;
                exportedCount++;

                for (int i = 0; i < sortedObjs.Count; i++)
                {
                    AssetClassGuid guid = ((dynamic)sortedObjs[i]).GetInstanceGuid();

                    type = sortedObjs[i].GetType();
                    classIdx = FindExistingClass(type);
                    classType = classTypes[classIdx];

                    if (!uniqueTypes.Contains(type))
                        uniqueTypes.Add(type);

                    //instances.Add(new EbxInstance()
                    //{
                    //    ClassRef = (ushort)classIdx,
                    //    Count = 1,
                    //    IsExported = guid != Guid.Empty
                    //});
                    //exportedCount += (ushort)((instances[instances.Count - 1].IsExported) ? 1 : 0);

                    if (classIdx != inst.ClassRef || inst.IsExported && !guid.IsExported)
                    {
                        inst.Count = count;
                        instances.Add(inst);

                        inst = new EbxInstance
                        {
                            ClassRef = (ushort)classIdx,
                            IsExported = guid.IsExported
                        };
                        exportedCount += (ushort)((inst.IsExported) ? 1 : 0);

                        count = 0;
                    }

                    writer.WritePadding(classType.Alignment);

                    if (guid.IsExported)
                        writer.Write(guid.ExportedGuid);

                    if (classType.Alignment != 0x04)
                        writer.Write((ulong)0);

                    WriteClass(sortedObjs[i], type, writer);
                    count++;
                }

                // Add final instance
                inst.Count = count;
                instances.Add(inst);
            }

            data = dataStream.ToArray();
            uniqueClassCount = (ushort)uniqueTypes.Count;
        }

        private void WriteClass(object obj, Type objType, NativeWriter writer)
        {
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
                WriteClass(obj, objType.BaseType, writer);

            PropertyInfo[] pis = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            EbxClass classType = classTypes[FindExistingClass(objType)];

            for (int i = 0; i < classType.FieldCount; i++)
            {
                EbxField field = GetField(classType, classType.FieldIndex + i);
                if (field.DebugType == EbxFieldType.Inherited)
                    continue;

                PropertyInfo pi = null;
                foreach (PropertyInfo curPi in pis)
                {
                    var attr = curPi.GetCustomAttribute<HashAttribute>();
                    if (attr != null)
                    {
                        if ((uint)attr.Hash == field.NameHash)
                        {
                            pi = curPi;
                            break;
                        }
                    }
                }

                if (pi == null)
                {
                    if (field.DebugType == EbxFieldType.ResourceRef
                        || field.DebugType == EbxFieldType.TypeRef
                        || field.DebugType == EbxFieldType.FileRef
                        || field.DebugType == EbxFieldType.BoxedValueRef
                        || field.DebugType == EbxFieldType.UInt64
                        || field.DebugType == EbxFieldType.Int64
                        || field.DebugType == EbxFieldType.Float64)
                    {
                        writer.WritePadding(8);
                    }
                    else if (field.DebugType == EbxFieldType.Array || field.DebugType == EbxFieldType.Pointer)
                        writer.WritePadding(4);

                    switch (field.DebugType)
                    {
                        case EbxFieldType.TypeRef: writer.Write((ulong)0); break;
                        case EbxFieldType.FileRef: writer.Write((ulong)0); break;
                        case EbxFieldType.CString: writer.Write(0); break;
                        case EbxFieldType.Pointer: writer.Write(0); break;

                        case EbxFieldType.Struct:
                            {
                                EbxClass structType = EbxReaderV2.std.GetClass(classType.Index + (short)field.ClassRef).Value;
                                writer.WritePadding(structType.Alignment);
                                writer.Write(new byte[structType.Size]);
                            }
                            break;

                        case EbxFieldType.Array: writer.Write(0); break;
                        case EbxFieldType.Enum: writer.Write((int)0); break;
                        case EbxFieldType.Float32: writer.Write((float)0); break;
                        case EbxFieldType.Float64: writer.Write((double)0); break;
                        case EbxFieldType.Boolean: writer.Write((byte)0); break;
                        case EbxFieldType.Int8: writer.Write((sbyte)0); break;
                        case EbxFieldType.UInt8: writer.Write((byte)0); break;
                        case EbxFieldType.Int16: writer.Write((short)0); break;
                        case EbxFieldType.UInt16: writer.Write((ushort)0); break;
                        case EbxFieldType.Int32: writer.Write((int)0); break;
                        case EbxFieldType.UInt32: writer.Write((uint)0); break;
                        case EbxFieldType.Int64: writer.Write((long)0); break;
                        case EbxFieldType.UInt64: writer.Write((ulong)0); break;
                        case EbxFieldType.Guid: writer.Write(Guid.Empty); break;
                        case EbxFieldType.Sha1: writer.Write(Sha1.Zero); break;
                        case EbxFieldType.String: writer.WriteFixedSizedString("", 32); break;
                        case EbxFieldType.ResourceRef: writer.Write((ulong)0); break;
                        case EbxFieldType.BoxedValueRef: writer.Write(Guid.Empty); break;
                    }

                    continue;
                }

                EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                bool isReference = pi.GetCustomAttribute<IsReferenceAttribute>() != null;

                EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);
                WriteField(pi.GetValue(obj), ebxType, classType.Alignment, writer, isReference);
            }

            //foreach (PropertyInfo pi in pis)
            //{
            //    // ignore transients if not saving to project
            //    if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !flags.HasFlag(EbxWriteFlags.IncludeTransient))
            //        continue;

            //    // ignore the instance guid
            //    if (pi.Name.Equals("__InstanceGuid"))
            //        continue;

            //    EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
            //    bool isReference = pi.GetCustomAttribute<IsReferenceAttribute>() != null;

            //    EbxFieldType ebxType = (EbxFieldType)((fta.Flags >> 4) & 0x1F);
            //    WriteField(pi.GetValue(obj), ebxType, classType.Alignment, writer, isReference);
            //}

            writer.WritePadding(classType.Alignment);
        }

        private void WriteField(object obj, EbxFieldType ebxType, byte classAlignment, NativeWriter writer, bool isReference)
        {
            if (ebxType == EbxFieldType.ResourceRef
                || ebxType == EbxFieldType.TypeRef
                || ebxType == EbxFieldType.FileRef
                || ebxType == EbxFieldType.BoxedValueRef
                || ebxType == EbxFieldType.UInt64
                || ebxType == EbxFieldType.Int64
                || ebxType == EbxFieldType.Float64)
            {
                writer.WritePadding(8);
            }
            else if (ebxType == EbxFieldType.Array || ebxType == EbxFieldType.Pointer)
                writer.WritePadding(4);

            switch (ebxType)
            {
                case EbxFieldType.TypeRef: writer.Write((ulong)AddString((TypeRef)obj)); break;
                case EbxFieldType.FileRef: writer.Write((ulong)AddString((FileRef)obj)); break;
                case EbxFieldType.CString: writer.Write(AddString((CString)obj)); break;

                case EbxFieldType.Pointer:
                    {
                        PointerRef pointer = (PointerRef)obj;
                        uint pointerIndex = 0;

                        if (pointer.Type == PointerRefType.External)
                        {
                            int importIdx = imports.FindIndex((EbxImportReference value) => value == pointer.External);
                            pointerIndex = (uint)(importIdx | 0x80000000);

                            if (isReference && !dependencies.Contains(imports[importIdx].FileGuid))
                                dependencies.Add(imports[importIdx].FileGuid);
                        }
                        else if (pointer.Type == PointerRefType.Internal)
                            pointerIndex = (uint)(sortedObjs.FindIndex((object value) => value == pointer.Internal) + 1);

                        writer.Write(pointerIndex);
                    }
                    break;

                case EbxFieldType.Struct:
                    {
                        object structValue = obj;
                        Type structType = structValue.GetType();

                        EbxClass structClassType = classTypes[FindExistingClass(structType)];
                        writer.WritePadding(structClassType.Alignment);

                        WriteClass(structValue, structType, writer);
                    }
                    break;

                case EbxFieldType.Array:
                    {
                        int arrayClassIdx = typesToProcess.FindIndex((Type item) => item == obj.GetType());
                        int arrayIdx = 0;

                        EbxClass arrayClassType = classTypes[arrayClassIdx];
                        EbxField arrayFieldType = GetField(arrayClassType, arrayClassType.FieldIndex);// fieldTypes[arrayClassType.FieldIndex];

                        ebxType = arrayFieldType.DebugType;

                        Type arrayType = obj.GetType();
                        int count = (int)arrayType.GetMethod("get_Count").Invoke(obj, null);

                        //if (arrays.Count == 0)
                        //{
                        //    arrays.Add(
                        //        new EbxArray()
                        //        {
                        //            Count = 0,
                        //            ClassRef = arrayClassIdx
                        //        });
                        //    arrayData.Add(new byte[] { });
                        //}

                        MemoryStream arrayStream = new MemoryStream();
                        using (NativeWriter arrayWriter = new NativeWriter(arrayStream))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                object subValue = arrayType.GetMethod("get_Item").Invoke(obj, new object[] { i });
                                Type subValueType = subValue.GetType();

                                WriteField(subValue, ebxType, classAlignment, arrayWriter, isReference);
                            }
                        }

                        //if (count != 0)
                        {
                            arrayIdx = arrays.Count;
                            arrays.Add(
                                new EbxArray()
                                {
                                    Count = (uint)count,
                                    ClassRef = arrayClassIdx
                                });
                            arrayData.Add(arrayStream.ToArray());
                        }
                        writer.Write(arrayIdx);
                    }
                    break;

                case EbxFieldType.Enum: writer.Write((int)obj); break;
                case EbxFieldType.Float32: writer.Write((float)obj); break;
                case EbxFieldType.Float64: writer.Write((double)obj); break;
                case EbxFieldType.Boolean: writer.Write((byte)(((bool)obj) ? 0x01 : 0x00)); break;
                case EbxFieldType.Int8: writer.Write((sbyte)obj); break;
                case EbxFieldType.UInt8: writer.Write((byte)obj); break;
                case EbxFieldType.Int16: writer.Write((short)obj); break;
                case EbxFieldType.UInt16: writer.Write((ushort)obj); break;
                case EbxFieldType.Int32: writer.Write((int)obj); break;
                case EbxFieldType.UInt32: writer.Write((uint)obj); break;
                case EbxFieldType.Int64: writer.Write((long)obj); break;
                case EbxFieldType.UInt64: writer.Write((ulong)obj); break;
                case EbxFieldType.Guid: writer.Write((Guid)obj); break;
                case EbxFieldType.Sha1: writer.Write((Sha1)obj); break;
                case EbxFieldType.String: writer.WriteFixedSizedString((string)obj, 32); break;
                case EbxFieldType.ResourceRef: writer.Write((ResourceRef)obj); break;
                case EbxFieldType.BoxedValueRef:
                    {
                        BoxedValueRef value = (BoxedValueRef)obj;
                        int index = m_boxedValues.Count;

                        if (value.Type == EbxFieldType.Inherited)
                        {
                            index = -1;
                        }
                        else
                        {
                            EbxBoxedValue boxedValue = new EbxBoxedValue() { Offset = 0, Type = (ushort)value.Type };

                            m_boxedValues.Add(boxedValue);
                            m_boxedValueData.Add(WriteBoxedValueRef(value));
                        }

                        Write(index);
                        Write((ulong)0);
                        Write((uint)0);
                    }
                    break;


                default: throw new InvalidDataException("Error");
            }
        }

        private int FindExistingClass(Type inType) => typesToProcess.FindIndex((Type value) => value == inType);

        private void AddTypeName(string inName)
        {
            if (!typeNames.Contains(inName))
                typeNames.Add(inName);
        }

        private int AddClass(PropertyInfo pi, Type classType)
        {
            EbxClass ebxClass = GetClass(pi.GetCustomAttribute<GuidAttribute>().Guid);
            classTypes.Add(ebxClass);
            typesToProcess.Add(classType);
            classGuids.Add(pi.GetCustomAttribute<GuidAttribute>().Guid);
            return (classTypes.Count - 1);
        }

        private int AddClass(string name, int fieldIndex, byte fieldCount, byte alignment, ushort type, ushort size, ushort secondSize, Type classType)
        {
            //classTypes.Add(new EbxClass()
            //{
            //    Name = name,
            //    FieldIndex = fieldIndex,
            //    FieldCount = fieldCount,
            //    Alignment = alignment,
            //    Type = type,
            //    Size = size,
            //    SecondSize = secondSize
            //});
            EbxClass ebxClass = GetClass(classType);
            classTypes.Add(ebxClass);
            classGuids.Add(EbxReaderV2.std.GetGuid(ebxClass).Value);

            //for (int i = 0; i < ebxClass.FieldCount; i++)
            //{
            //    fieldTypes.Add(GetField(ebxClass, ebxClass.FieldIndex + i));
            //}

            AddTypeName(name);
            typesToProcess.Add(classType);

            return (classTypes.Count - 1);
        }

        private void AddField(string name, ushort type, ushort classRef, uint dataOffset, uint secondOffset)
        {
            //fieldTypes.Add(new EbxField()
            //{
            //    Name = name,
            //    Type = type,
            //    ClassRef = classRef,
            //    DataOffset = dataOffset,
            //    SecondOffset = secondOffset
            //});
            AddTypeName(name);
        }

        private int HashString(string strToHash)
        {
            int hash = 5381;
            for (int i = 0; i < strToHash.Length; i++)
            {
                byte B = (byte)strToHash[i];
                hash = (hash * 33) ^ B;
            }

            return hash;
        }

        private new uint AddString(string stringToAdd)
        {
            if (stringToAdd == "")
                return 0xFFFFFFFF;

            uint offset = 0;
            if (strings.Contains(stringToAdd))
            {
                for (int i = 0; i < strings.Count; i++)
                {
                    if (strings[i] == stringToAdd)
                        break;
                    offset += (uint)(strings[i].Length + 1);
                }
            }
            else
            {
                offset = m_stringsLength;
                strings.Add(stringToAdd);
                m_stringsLength += (uint)(stringToAdd.Length + 1);
            }

            return offset;
        }

        internal EbxClass GetClass(Type objType)
        {
            EbxClass? classType = null;
            foreach (TypeInfoGuidAttribute attr in objType.GetCustomAttributes<TypeInfoGuidAttribute>())
            {
                if (classType == null)
                    classType = EbxReaderV2.std.GetClass(attr.Guid);
                break;
            }

            return classType.Value;
        }

        internal EbxClass GetClass(Guid guid) => EbxReaderV2.std.GetClass(guid).Value;

        internal EbxField GetField(EbxClass classType, int index) => EbxReaderV2.std.GetField(index).Value;
    }
}
