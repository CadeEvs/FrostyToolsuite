using FrostySdk.Attributes;
using FrostySdk.Ebx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FrostySdk.IO
{

    public class EbxWriterRiff : EbxBaseWriter
    {
        public List<object> Objects => m_sortedObjs;
        public List<Guid> Dependencies => m_dependencies;

        private List<object> m_objsToProcess = new List<object>();
        private List<Type> m_typesToProcess = new List<Type>();
        private List<EbxFieldMetaAttribute> m_arrayTypes = new List<EbxFieldMetaAttribute>();

        private List<object> m_objs = new List<object>();
        private List<object> m_sortedObjs = new List<object>();
        private List<Guid> m_dependencies = new List<Guid>();

        private List<EbxClass> m_classTypes = new List<EbxClass>();
        private List<Guid> m_classGuids = new List<Guid>();
        private List<uint> m_classSignatures = new List<uint>();
        private List<EbxField> m_fieldTypes = new List<EbxField>();
        private List<string> m_typeNames = new List<string>();
        private List<EbxImportReference> m_imports = new List<EbxImportReference>();

        private byte[] m_data = null;
        private List<EbxInstance> m_instances = new List<EbxInstance>();
        private List<EbxArray> m_arrays = new List<EbxArray>();
        private List<byte[]> m_arrayData = new List<byte[]>();
        private List<uint> m_typeInfoOffsets = new List<uint>();
        private List<uint> m_arrayFieldOffsets = new List<uint>();
        private List<uint> m_pointerOffsets = new List<uint>();
        private List<uint> m_exportOffsets = new List<uint>();
        private List<uint> m_importOffsets = new List<uint>();
        private List<uint> m_resourceRefOffsets = new List<uint>();

        private ushort m_uniqueClassCount = 0;
        private uint m_exportedCount = 0;
        private uint m_arraysOffset = 0;
        private uint m_boxedValuesOffset = 0;
        private uint m_stringsOffset = 0;

        internal EbxWriterRiff(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
            : base(inStream, inFlags, leaveOpen)
        {
            m_flags = inFlags;
        }

        public override void WriteAsset(EbxAsset asset)
        {
            if (m_flags.HasFlag(EbxWriteFlags.DoNotSort))
            {
                foreach (object obj in asset.Objects)
                {
                    ExtractClass(obj.GetType(), obj);
                }
                WriteEbx(asset.FileGuid);
            }
            else
            {
                List<object> writeObjs = new List<object>();
                foreach (object obj in asset.RootObjects)
                {
                    writeObjs.Add(obj);
                }
                WriteEbxObjects(writeObjs, asset.FileGuid);
            }
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
            // do 2 passes through the EBX objects
            // this puts them in the right order for processing
            foreach (object obj in m_objsToProcess)
            {
                Type objType = obj.GetType();
                if (FindExistingClass(objType) == -1)
                {
                    AddClass(objType.Name, objType);
                }
            }
            foreach (object obj in m_objsToProcess)
            {
                ProcessClass(obj.GetType(), obj);
            }
            for (int i = 0; i < m_typesToProcess.Count; i++)
            {
                ProcessType(i);
            }

            ProcessData();

            Write((int)EbxVersion.Version6);
            Write(0x00); // total size - 8

            // @todo: sometimes this should be EBXS, don't know when though
            Write((int)RiffEbxSection.EBX, Endian.Big);

            // write EBXD section
            {
                MemoryStream stream = new MemoryStream();
                using (NativeWriter writer = new NativeWriter(stream))
                {
                    uint ebxdSize = 0;
                    writer.Write((int)RiffEbxSection.EBXD, Endian.Big);
                    writer.Write(0x00);
                    long ebxdSizePos = Position;
                    writer.WritePadding(16);
                    writer.Write(0x00); // extra 4 bytes to align the EBX data
                    writer.Write(m_data);

                    ebxdSize = (uint)(writer.Position - 8);
                    writer.Position = 0x4;
                    writer.Write(ebxdSize);
                }
                Write(stream.ToArray());
            }

            // write EFIX section
            {
                WritePadding(2);
                MemoryStream stream = new MemoryStream();
                using (NativeWriter writer = new NativeWriter(stream))
                {
                    uint efixSize = 0;
                    writer.Write((int)RiffEbxSection.EFIX, Endian.Big);
                    writer.Write(0x00);
                    writer.Write(fileGuid);

                    writer.Write(m_classGuids.Count);
                    foreach (Guid guid in m_classGuids)
                    {
                        writer.Write(guid);
                    }

                    writer.Write(m_classSignatures.Count);
                    foreach (uint sig in m_classSignatures)
                    {
                        writer.Write(sig);
                    }

                    writer.Write(m_exportedCount);
                    m_exportOffsets.Sort((uint a, uint b) => a.CompareTo(b));
                    writer.Write(m_exportOffsets.Count);
                    foreach (uint off in m_exportOffsets)
                    {
                        writer.Write(off);
                    }

                    m_pointerOffsets.Sort((uint a, uint b) => a.CompareTo(b));
                    writer.Write(m_pointerOffsets.Count);
                    foreach (uint off in m_pointerOffsets)
                    {
                        writer.Write(off);
                    }

                    m_resourceRefOffsets.Sort((uint a, uint b) => a.CompareTo(b));
                    writer.Write(m_resourceRefOffsets.Count);
                    foreach (uint off in m_resourceRefOffsets)
                    {
                        writer.Write(off);
                    }

                    writer.Write(m_imports.Count);
                    foreach (EbxImportReference import in m_imports)
                    {
                        writer.Write(import.FileGuid);
                        writer.Write(import.ClassGuid);
                    }

                    m_importOffsets.Sort((uint a, uint b) => a.CompareTo(b));
                    writer.Write(m_importOffsets.Count);
                    foreach (uint off in m_importOffsets)
                    {
                        writer.Write(off);
                    }

                    m_typeInfoOffsets.Sort((uint a, uint b) => a.CompareTo(b));
                    writer.Write(m_typeInfoOffsets.Count);
                    foreach (uint off in m_typeInfoOffsets)
                    {
                        writer.Write(off);
                    }

                    writer.Write(m_arraysOffset);
                    writer.Write(m_boxedValuesOffset);
                    writer.Write(m_stringsOffset);
                    writer.Write(0x00); // seems to always be zero but counts toward the size of this section

                    efixSize = (uint)(writer.Position - 8);
                    writer.Position = 0x4;
                    writer.Write(efixSize);
                }
                Write(stream.ToArray());
            }

            // write EBXX section
            {
                WritePadding(2);
                MemoryStream stream = new MemoryStream();
                using (NativeWriter writer = new NativeWriter(stream))
                {
                    uint ebxxSize = 0;
                    writer.Write((int)RiffEbxSection.EBXX, Endian.Big);
                    writer.Write(0x00);

                    // discard empty arrays and boxed values
                    m_arrays.RemoveAll(a => a.Count == 0);
                    m_boxedValues.RemoveAll(a => a.Offset == 0);

                    writer.Write(m_arrays.Count);
                    writer.Write(m_boxedValues.Count);

                    m_arrays.Sort((EbxArray a, EbxArray b) => a.Offset.CompareTo(b.Offset));
                    foreach (EbxArray arr in m_arrays)
                    {
                        writer.Write(arr.Offset);
                        writer.Write(arr.Count);
                        writer.Write(0x00); // unknown, varies between assets
                        writer.Write((ushort)arr.Type);
                        writer.Write((short)arr.ClassRef);
                    }

                    m_boxedValues.Sort((EbxBoxedValue a, EbxBoxedValue b) => a.Offset.CompareTo(b.Offset));
                    foreach (EbxBoxedValue val in m_boxedValues)
                    {
                        writer.Write(val.Offset);
                        writer.Write(1);
                        writer.Write(0x00); // unknown, varies between assets
                        writer.Write(val.Type);
                        writer.Write((short)val.ClassRef);
                    }

                    ebxxSize = (uint)(writer.Position - 8);
                    writer.Position = 0x4;
                    writer.Write(ebxxSize);
                }
                Write(stream.ToArray());
            }

            // @todo: write REFL section
            {
                /*uint reflSize = 0;
                WritePadding(2);
                Write((int)RiffEbxSection.REFL, Endian.Big);
                long sizeOffset = Position;
                Write(0x00);

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
                }*/
            }

            uint dataLen = (uint)(Position - 8);
            Position = 0x4;
            Write(dataLen);
        }

        private List<object> ExtractClass(Type type, object obj, bool add = true)
        {
            if (add)
            {
                if (m_objsToProcess.Contains(obj))
                {
                    return new List<object>();
                }

                m_objsToProcess.Add(obj);
                m_objs.Add(obj);
            }

            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            List<object> retObjects = new List<object>();

            foreach (PropertyInfo pi in pis)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                {
                    continue;
                }

                if (pi.PropertyType == typeof(PointerRef))
                {
                    PointerRef value = (PointerRef)pi.GetValue(obj);
                    if (value.Type == PointerRefType.Internal)
                    {
                        retObjects.Add(value.Internal);
                    }
                    else if (value.Type == PointerRefType.External && !m_imports.Contains(value.External))
                    {
                        m_imports.Add(value.External);
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
                    IList arrayObj = (IList)pi.GetValue(obj);
                    int count = arrayObj.Count;

                    if (count > 0)
                    {
                        if (arrayType.GenericTypeArguments[0] == typeof(PointerRef))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                PointerRef value = (PointerRef)arrayObj[i];
                                if (value.Type == PointerRefType.Internal)
                                {
                                    retObjects.Add(value.Internal);
                                }
                                else if (value.Type == PointerRefType.External && !m_imports.Contains(value.External))
                                {
                                    m_imports.Add(value.External);
                                }
                            }
                        }
                        else if (arrayType.GenericTypeArguments[0].Namespace == "FrostySdk.Ebx" && arrayType.GenericTypeArguments[0].BaseType != typeof(Enum))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                object value = arrayObj[i];
                                retObjects.AddRange(ExtractClass(value.GetType(), value, false));
                            }
                        }
                    }
                }
            }

            if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
            {
                retObjects.AddRange(ExtractClass(type.BaseType, obj, false));
            }

            return retObjects;
        }

        private ushort ProcessClass(Type objType, object obj, bool shouldAddType = true, bool isBaseType = false)
        {
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                ProcessClass(objType.BaseType, obj, shouldAddType: false, isBaseType: true);
            }

            int index = FindExistingClass(objType);
            if (index != -1)
            {
                shouldAddType = false;
                //return (ushort)index;
            }

            EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
            PropertyInfo[] allProps = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<PropertyInfo> pis = new List<PropertyInfo>();

            foreach (PropertyInfo pi in allProps)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() == null || m_flags.HasFlag(EbxWriteFlags.IncludeTransient))
                {
                    pis.Add(pi);
                }
            }

            // only child classes and classes that don't inherit should be added
            if (!isBaseType && shouldAddType)
            {
                index = AddClass(objType.Name, objType);
            }
            
            // don't process enums
            if (objType.IsEnum)
            {
                return (ushort)index;
            }

            // Fields
            foreach (PropertyInfo pi in pis)
            {
                EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                EbxFieldType ebxType = fta.Type;

                // only certain types need to be processed to be referenced in the EBX data
                // this seems to only include structs/classes used by arrays and boxed values

                switch (ebxType)
                {
                    case EbxFieldType.Struct:
                        {
                            Type structType = pi.PropertyType;
                            // exclude struct properties from being processed unless they contain arrays
                            ProcessClass(structType, pi.GetValue(obj), shouldAddType: false, isBaseType: false);
                        }
                        break;

                    case EbxFieldType.Array:
                        {
                            if (fta.ArrayType != EbxFieldType.Struct && fta.ArrayType != EbxFieldType.Enum)
                            {
                                break;
                            }

                            Type arrayElemType = pi.PropertyType.GenericTypeArguments[0];
                            bool addType = true;
                            if (FindExistingClass(arrayElemType) != -1)
                            {
                                addType = false;
                            }

                            IList arrayObj = (IList)pi.GetValue(obj);

                            // empty arrays shouldn't have their types processed
                            if (arrayObj.Count == 0)
                            {
                                break;
                            }

                            if (fta.ArrayType == EbxFieldType.Struct)
                            {
                                // need to iterate through elements in case some elements contain more types to process than others
                                foreach (object elemObj in arrayObj)
                                {
                                    ProcessClass(arrayElemType, elemObj);
                                }
                            }
                            else if (fta.ArrayType == EbxFieldType.Enum && addType)
                            {
                                
                                AddClass(arrayElemType.Name, arrayElemType);
                            }

                            if (addType)
                            {
                                m_arrayTypes.Add(fta);
                            }

                            // owner struct should only be added if it contains an array
                            // it always seems to be added after the first array encountered
                            if (!shouldAddType && !isBaseType && FindExistingClass(objType) == -1)
                            {
                                index = AddClass(objType.Name, objType);
                            }
                        }
                        break;

                    case EbxFieldType.BoxedValueRef:
                        {
                            BoxedValueRef boxedValueRef = (BoxedValueRef)pi.GetValue(obj);
                            if (boxedValueRef.Value == null)
                            {
                                break;
                            }

                            if (boxedValueRef.Type != EbxFieldType.Struct && boxedValueRef.Type != EbxFieldType.Array && boxedValueRef.Type != EbxFieldType.Enum)
                            {
                                break;
                            }

                            Type valueRefType = boxedValueRef.Value.GetType();
                            if (boxedValueRef.Type == EbxFieldType.Array)
                            {
                                valueRefType = valueRefType.GenericTypeArguments[0];
                            }

                            if (FindExistingClass(valueRefType) != -1)
                            {
                                break;
                            }

                            if (boxedValueRef.Type == EbxFieldType.Array)
                            {
                                IList arrayObj = (IList)boxedValueRef.Value;

                                // empty arrays shouldn't have their types processed
                                if (arrayObj.Count == 0)
                                {
                                    break;
                                }

                                if (boxedValueRef.ArrayType == EbxFieldType.Struct)
                                {
                                    // need to iterate through elements in case some elements contain more types to process than others
                                    foreach (object elemObj in arrayObj)
                                    {
                                        ProcessClass(valueRefType, elemObj);
                                    }
                                }
                            }
                            else if (boxedValueRef.Type == EbxFieldType.Struct)
                            {
                                ProcessClass(valueRefType, boxedValueRef.Value);
                            }
                            else
                            {
                                AddClass(valueRefType.Name, valueRefType);
                            }
                        }
                        break;

                    default: break;
                }
            }

            return (ushort)index;
        }

        private void ProcessType(int index)
        {
            Type type = m_typesToProcess[index];
            EbxClassMetaAttribute cta = type.GetCustomAttribute<EbxClassMetaAttribute>();

            if (cta.Type == EbxFieldType.Array)
            {
                EbxFieldMetaAttribute ata = m_arrayTypes[0];
                m_arrayTypes.RemoveAt(0);

                ushort arrayClassRef = (ushort)FindExistingClass(type.GenericTypeArguments[0]);
                //if (arrayClassRef == 0xFFFF)
                //    arrayClassRef = 0;

                AddField("member", ata.ArrayFlags, arrayClassRef, 0, 0);
            }
            else if (cta.Type == EbxFieldType.Enum)
            {
                string[] enumNames = type.GetEnumNames();
                Array enumValues = type.GetEnumValues();

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

                if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
                {
                    ushort classIndex = (ushort)FindExistingClass(type.BaseType);
                    AddField("$", 0, classIndex, 8, 0);
                }

                foreach (PropertyInfo pi in pis)
                {
                    ProcessField(pi);
                }
            }
        }

        private void ProcessField(PropertyInfo pi)
        {
            EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();

            Type propType = pi.PropertyType;
            ushort classRef = (ushort)FindExistingClass(propType);

            AddField(pi.Name, fta.Flags, classRef, fta.Offset, 0);
        }

        private void ProcessData()
        {
            List<Type> uniqueTypes = new List<Type>();
            List<object> exportedObjs = new List<object>();
            List<object> otherObjs = new List<object>();

            for (int i = 0; i < m_objs.Count; i++)
            {
                dynamic obj = m_objs[i];
                AssetClassGuid guid = obj.GetInstanceGuid();
                if (guid.IsExported)
                {
                    exportedObjs.Add(obj);
                }
                else
                {
                    otherObjs.Add(obj);
                }
            }

            m_exportedCount = (uint)exportedObjs.Count;
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

            //otherObjs.Sort((object a, object b) => a.GetType().Name.CompareTo(b.GetType().Name));

            m_sortedObjs.Add(root);
            m_sortedObjs.AddRange(exportedObjs);
            m_sortedObjs.AddRange(otherObjs);

            m_imports.Sort((EbxImportReference a, EbxImportReference b) =>
            {
                byte[] bA = a.FileGuid.ToByteArray();
                byte[] bB = b.FileGuid.ToByteArray();

                uint idA = (uint)(bA[0] << 24 | bA[1] << 16 | bA[2] << 8 | bA[3]);
                uint idB = (uint)(bB[0] << 24 | bB[1] << 16 | bB[2] << 8 | bB[3]);

                // collision usually means imports are coming from the same file
                // in that case, compare the class/instance GUIDs instead
                if (idA != idB)
                {
                    return idA.CompareTo(idB);
                }

                bA = a.ClassGuid.ToByteArray();
                bB = b.ClassGuid.ToByteArray();

                idA = (uint)(bA[0] << 24 | bA[1] << 16 | bA[2] << 8 | bA[3]);
                idB = (uint)(bB[0] << 24 | bB[1] << 16 | bB[2] << 8 | bB[3]);

                return idA.CompareTo(idB);
            });

            MemoryStream dataStream = new MemoryStream();
            using (NativeWriter writer = new NativeWriter(dataStream))
            {
                Type type = m_sortedObjs[0].GetType();
                int classIdx = FindExistingClass(type);
                EbxClass classType = m_classTypes[classIdx];

                EbxInstance inst = new EbxInstance()
                {
                    ClassRef = (ushort)classIdx,
                    Count = 0,
                    IsExported = true
                };

                ushort count = 0;

                for (int i = 0; i < m_sortedObjs.Count; i++)
                {
                    AssetClassGuid guid = ((dynamic)m_sortedObjs[i]).GetInstanceGuid();

                    type = m_sortedObjs[i].GetType();
                    classIdx = FindExistingClass(type);
                    classType = m_classTypes[classIdx];

                    if (!uniqueTypes.Contains(type))
                    {
                        uniqueTypes.Add(type);
                    }

                    if (classIdx != inst.ClassRef || inst.IsExported && !guid.IsExported)
                    {
                        inst.Count = count;
                        m_instances.Add(inst);

                        inst = new EbxInstance
                        {
                            ClassRef = (ushort)classIdx,
                            IsExported = guid.IsExported
                        };
                        count = 0;
                    }

                    writer.WritePadding(classType.Alignment);

                    if (guid.IsExported)
                    {
                        writer.Write(guid.ExportedGuid);
                    }
                    m_exportOffsets.Add((uint)writer.Position);
                    long classStartOffset = writer.Position;

                    writer.Write((ulong)classIdx);
                    if (classType.Alignment != 0x04)
                    {
                        writer.Write((ulong)0);
                    }

                    writer.Write(2u); // seems to always be the same (needs more testing)
                    // flags?
                    if (guid.IsExported)
                    {
                        writer.Write(45312u);
                    }
                    else
                    {
                        writer.Write(40960u);
                    }

                    WriteClass(m_sortedObjs[i], type, classStartOffset, writer);
                    count++;
                }

                // Add final instance
                inst.Count = count;
                m_instances.Add(inst);

                writer.WritePadding(16);
                m_arraysOffset = (uint)writer.Position;
                // 32 bytes are used for an empty array(?)
                byte[] arrayPad = new byte[32];
                writer.Write(arrayPad);

                if (m_arrays.Count > 0)
                {
                    for (int i = 0; i < m_arrays.Count; i++)
                    {
                        EbxArray array = m_arrays[i];
                        // skip empty arrays, these are handled during relocation/fixup
                        if (array.Count == 0)
                        {
                            continue;
                        }

                        // need to manually get alignment if it doesn't use a class ref
                        byte alignment;
                        if (array.ClassRef == -1)
                        {
                            EbxFieldType arrayType = (EbxFieldType)((array.Type >> 5) & 0x1F);
                            switch (arrayType)
                            {
                                case EbxFieldType.Enum:
                                case EbxFieldType.TypeRef:
                                case EbxFieldType.String:
                                case EbxFieldType.Boolean:
                                case EbxFieldType.Float32:
                                case EbxFieldType.Int8:
                                case EbxFieldType.UInt8:
                                case EbxFieldType.Int16:
                                case EbxFieldType.UInt16:
                                case EbxFieldType.Int32:
                                case EbxFieldType.UInt32:
                                    alignment = 4; break;

                                case EbxFieldType.Float64:
                                case EbxFieldType.Int64:
                                case EbxFieldType.UInt64:
                                case EbxFieldType.CString:
                                case EbxFieldType.FileRef:
                                case EbxFieldType.Delegate:
                                case EbxFieldType.Pointer:
                                case EbxFieldType.ResourceRef:
                                case EbxFieldType.BoxedValueRef:
                                    alignment = 8; break;
                                //case EbxFieldType.Guid: alignment = 16; break;

                                default: alignment = 4; break;
                            }
                        }
                        else
                        {
                            EbxClass arrayClass = m_classTypes[array.ClassRef];
                            switch (arrayClass.DebugCategory)
                            {
                                case EbxFieldCategory.EnumType:
                                    alignment = 4; break;
                                case EbxFieldCategory.Pointer:
                                case EbxFieldCategory.ArrayType:
                                case EbxFieldCategory.DelegateType:
                                    alignment = 8; break;
                                default: alignment = arrayClass.Alignment; break;
                            }
                        }
                        writer.WritePadding(alignment);
                        // shift where the count is so that the array data is properly aligned
                        long dataPos = writer.Position + 4;
                        if (alignment != 4)
                        {
                            while (dataPos % alignment != 0)
                            {
                                writer.Write((byte)0);
                                dataPos = writer.Position;
                            }
                            writer.Position -= 0x4;
                        }
                        writer.Write(array.Count);

                        array.Offset = (uint)writer.Position;
                        writer.Write(m_arrayData[i]);

                        m_arrays[i] = array;
                    }
                    writer.WritePadding(16);
                }

                m_boxedValuesOffset = (uint)writer.Position;
                for (int i = 0; i < m_boxedValues.Count; i++)
                {
                    EbxBoxedValue boxedValue = m_boxedValues[i];
                    // null boxed values have an offset of zero and aren't written
                    if (m_boxedValueData[i] == null)
                    {
                        continue;
                    }

                    byte alignment;
                    if (boxedValue.ClassRef == 0xFFFF)
                    {
                        EbxFieldType boxedValType = (EbxFieldType)((boxedValue.Type >> 5) & 0x1F);
                        switch (boxedValType)
                        {
                            case EbxFieldType.Boolean:
                                alignment = 1; break;
                            case EbxFieldType.Int8:
                            case EbxFieldType.UInt8:
                            case EbxFieldType.Int16:
                            case EbxFieldType.UInt16:
                                alignment = 2; break;
                            case EbxFieldType.Enum:
                            case EbxFieldType.TypeRef:
                            case EbxFieldType.String:
                            case EbxFieldType.Float32:
                            case EbxFieldType.Int32:
                            case EbxFieldType.UInt32:
                                alignment = 4; break;

                            case EbxFieldType.Float64:
                            case EbxFieldType.Int64:
                            case EbxFieldType.UInt64:
                            case EbxFieldType.CString:
                            case EbxFieldType.FileRef:
                            case EbxFieldType.Delegate:
                            case EbxFieldType.Pointer:
                            case EbxFieldType.ResourceRef:
                            case EbxFieldType.BoxedValueRef:
                                alignment = 8; break;
                            //case EbxFieldType.Guid: alignment = 16; break;

                            default: alignment = 4; break;
                        }
                    }
                    else
                    {
                        EbxClass boxedValClass = m_classTypes[boxedValue.ClassRef];
                        switch (boxedValClass.DebugCategory)
                        {
                            case EbxFieldCategory.EnumType:
                                alignment = 4; break;
                            case EbxFieldCategory.Pointer:
                            case EbxFieldCategory.ArrayType:
                            case EbxFieldCategory.DelegateType:
                                alignment = 8; break;
                            default: alignment = boxedValClass.Alignment; break;
                        }
                    }
                    writer.WritePadding(alignment);
                    boxedValue.Offset = (uint)writer.Position;
                    m_boxedValues[i] = boxedValue;

                    writer.Write(m_boxedValueData[i]);
                }

                m_stringsOffset = (uint)writer.Position;
                foreach (string str in m_strings)
                {
                    writer.WriteNullTerminatedString(str);
                }

                FixupPointers(writer);
            }

            m_data = dataStream.ToArray();
            m_uniqueClassCount = (ushort)uniqueTypes.Count;
        }

        private void WriteClass(object obj, Type objType, long startOffset, NativeWriter writer, bool writeClassBytes = true)
        {
            EbxClass classType = GetClass(objType);
            //EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();

            if (writeClassBytes)
            {
                int diff = (int)(writer.Position - startOffset);
                writer.Write(new byte[classType.Size - diff]);
            }

            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                WriteClass(obj, objType.BaseType, startOffset, writer, false);
            }

            PropertyInfo[] pis = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            // sort properties by field index for better accuracy
            IEnumerable<PropertyInfo> sortedPis = pis.OrderBy(p =>
            {
                FieldIndexAttribute fia = p.GetCustomAttribute<FieldIndexAttribute>();
                return fia.Index;
            });

            foreach (PropertyInfo pi in sortedPis)
            {
                EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                bool isReference = pi.GetCustomAttribute<IsReferenceAttribute>() != null;
                bool isTransient = pi.GetCustomAttribute<IsTransientAttribute>() != null;

                if (fta.Type == EbxFieldType.Inherited || isTransient)
                {
                    continue;
                }

                // we shouldn't have a null property, but handle it anyway
                if (pi == null)
                {
                    switch (fta.Type)
                    {
                        case EbxFieldType.TypeRef:
                        case EbxFieldType.FileRef:
                        case EbxFieldType.CString:
                        case EbxFieldType.Array:
                        case EbxFieldType.Pointer: writer.Write((ulong)0); break;

                        case EbxFieldType.Struct:
                            {
                                EbxClassMetaAttribute meta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
                                writer.WritePadding(meta.Alignment);
                                writer.Write(new byte[meta.Size]);
                            }
                            break;

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
                        case EbxFieldType.BoxedValueRef:
                            {
                                writer.Write((uint)0);
                                writer.Write((uint)0);
                                writer.Write((ulong)0);
                            }
                            break;
                    }

                    continue;
                }

                writer.Position = startOffset + fta.Offset;
                WriteField(pi.GetValue(obj), fta.Type, fta, writer, isReference);
            }

            writer.Position = startOffset + classType.Size;
            writer.WritePadding(classType.Alignment);
        }

        private void WriteField(object obj, EbxFieldType ebxType, EbxFieldMetaAttribute fieldMeta, NativeWriter writer, bool isReference)
        {
            switch (ebxType)
            {
                case EbxFieldType.TypeRef:
                    {
                        TypeRef typeRefObj = (TypeRef)obj;
                        WriteTypeRef(typeRefObj, false, writer);
                    }
                    break;

                case EbxFieldType.FileRef:
                    {
                        string str = (FileRef)obj;
                        writer.Write((long)AddString(str));
                    }
                    break;

                case EbxFieldType.CString:
                    {
                        string str = (CString)obj;
                        writer.Write((long)AddString(str));
                    }
                    break;

                case EbxFieldType.Pointer:
                    {
                        PointerRef pointer = (PointerRef)obj;
                        WritePointer(pointer, isReference, writer);
                    }
                    break;

                case EbxFieldType.Struct:
                    {
                        object structValue = obj;
                        Type structType = structValue.GetType();
                        EbxClassMetaAttribute cta = structType.GetCustomAttribute<EbxClassMetaAttribute>();

                        writer.WritePadding(cta.Alignment);
                        WriteClass(structValue, structType, writer.Position, writer);
                    }
                    break;

                case EbxFieldType.Array:
                    {
                        WriteArray(obj, fieldMeta, isReference, writer);
                    }
                    break;

                case EbxFieldType.Enum:        writer.Write((int)obj); break;
                case EbxFieldType.Float32:     writer.Write((float)obj); break;
                case EbxFieldType.Float64:     writer.Write((double)obj); break;
                case EbxFieldType.Boolean:     writer.Write((byte)(((bool)obj) ? 0x01 : 0x00)); break;
                case EbxFieldType.Int8:        writer.Write((sbyte)obj); break;
                case EbxFieldType.UInt8:       writer.Write((byte)obj); break;
                case EbxFieldType.Int16:       writer.Write((short)obj); break;
                case EbxFieldType.UInt16:      writer.Write((ushort)obj); break;
                case EbxFieldType.Int32:       writer.Write((int)obj); break;
                case EbxFieldType.UInt32:      writer.Write((uint)obj); break;
                case EbxFieldType.Int64:       writer.Write((long)obj); break;
                case EbxFieldType.UInt64:      writer.Write((ulong)obj); break;
                case EbxFieldType.Guid:        writer.Write((Guid)obj); break;
                case EbxFieldType.Sha1:        writer.Write((Sha1)obj); break;
                case EbxFieldType.String:      writer.WriteFixedSizedString((string)obj, 32); break;
                case EbxFieldType.ResourceRef: writer.Write((ResourceRef)obj); break;

                case EbxFieldType.BoxedValueRef:
                    {
                        BoxedValueRef value = (BoxedValueRef)obj;
                        TypeRef typeRef = new TypeRef(value.TypeString);
                        (ushort, ushort) tiPair = WriteTypeRef(typeRef, true, writer);

                        int index = m_boxedValues.Count;
                        EbxBoxedValue boxedValue = new EbxBoxedValue()
                        {
                            Offset = 0,
                            Type = tiPair.Item1,
                            ClassRef = tiPair.Item2
                        };

                        m_boxedValues.Add(boxedValue);
                        if (value.Value != null)
                        {
                            m_boxedValueData.Add(WriteBoxedValueRef(value));
                        }
                        else
                        {
                            m_boxedValueData.Add(null);
                        }

                        writer.Write((ulong)index);
                    }
                    break;

                default: throw new InvalidDataException($"Unhandled field type: {ebxType}");
            }
        }

        private (ushort, ushort) WriteTypeRef(TypeRef typeRef, bool addSignature, NativeWriter writer)
        {
            if (typeRef.IsNull() || typeRef.Name == "0" || typeRef.Name == "Inherited")
            {
                writer.Write(0x00);
                writer.Write(0x00);
                return (0, 0);
            }

            Type typeRefType = typeRef.GetReferencedType();
            int typeIdx = FindExistingClass(typeRefType);
            EbxClassMetaAttribute cta = typeRefType.GetCustomAttribute<EbxClassMetaAttribute>();

            EbxFieldType type = EbxFieldType.Inherited;
            EbxFieldCategory category = EbxFieldCategory.None;
            if (cta != null)
            {
                type = cta.Type;
                category = (EbxFieldCategory)(cta.Flags & 0xF);
            }
            else if (typeRefType.Name.StartsWith("Delegate"))
            {
                type = EbxFieldType.Delegate;
                category = EbxFieldCategory.DelegateType;
            }

            (ushort, ushort) tiPair;
            uint typeFlags = (uint)type << 5;
            typeFlags |= (uint)category << 1;
            typeFlags |= 1;

            tiPair.Item1 = (ushort)typeFlags;
            if (category == EbxFieldCategory.PrimitiveType)
            {
                typeFlags |= 0x80000000;
                tiPair.Item2 = (ushort)typeIdx;
            }
            else
            {
                if (typeIdx == -1)
                {
                    // boxed value type refs shouldn't end up here, as they're already handled when processing classes
                    typeIdx = AddClass(typeRefType.Name, typeRefType, addSignature);
                }
                typeFlags = (uint)(typeIdx << 2);
                typeFlags |= 2;
                // boxed value info in the EBXX section needs the class index
                // the type ref just sets it to zero
                tiPair.Item2 = (ushort)typeIdx;
                typeIdx = 0;
            }
            writer.Write(typeFlags);
            writer.Write(typeIdx);
            return tiPair;
        }

        private void WritePointer(PointerRef pointer, bool isReference, NativeWriter writer)
        {
            if (pointer.Type == PointerRefType.External)
            {
                int importIdx = m_imports.FindIndex((EbxImportReference value) => value == pointer.External);

                if (isReference && !m_dependencies.Contains(m_imports[importIdx].FileGuid))
                {
                    m_dependencies.Add(m_imports[importIdx].FileGuid);
                }
                // the import list in the EBX counts both GUIDs of each import separately
                // what we want is the index to the class GUID
                writer.Write((ulong)(importIdx * 2 + 1));
            }
            else if (pointer.Type == PointerRefType.Internal)
            {
                ulong objIdx = (ulong)m_sortedObjs.FindIndex((object value) => value == pointer.Internal);
                writer.Write(objIdx);
            }
            else if (pointer.Type == PointerRefType.Null)
            {
                writer.Write((ulong)0);
            }
        }

        private void WriteArray(object obj, EbxFieldMetaAttribute fieldMeta, bool isReference, NativeWriter writer)
        {
            int arrayClassIdx = FindExistingClass(obj.GetType().GenericTypeArguments[0]);
            int arrayIdx = 0;

            // cast to IList to avoid having to invoke methods manually
            IList arrayObj = (IList)obj;
            int count = arrayObj.Count;

            if (count != 0)
            {
                MemoryStream arrayStream = new MemoryStream();
                using (NativeWriter arrayWriter = new NativeWriter(arrayStream))
                {
                    for (int i = 0; i < count; i++)
                    {
                        object subValue = arrayObj[i];
                        WriteField(subValue, fieldMeta.ArrayType, fieldMeta, arrayWriter, isReference);
                    }
                }

                m_arrayData.Add(arrayStream.ToArray());
            }
            else
            {
                m_arrayData.Add(null);
            }

            arrayIdx = m_arrays.Count;
            ushort arrayTypeFlags = (ushort)((uint)fieldMeta.ArrayType << 5);
            arrayTypeFlags |= (ushort)((uint)EbxFieldCategory.ArrayType << 1);
            m_arrays.Add(
                new EbxArray()
                {
                    Count = (uint)count,
                    ClassRef = arrayClassIdx,
                    Type = arrayTypeFlags
                });
            writer.Write((ulong)arrayIdx);
        }

        protected override byte[] WriteBoxedValueRef(BoxedValueRef value)
        {
            // @todo: Does not at all handle boxed value arrays
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                object obj = value.Value;
                switch (value.Type)
                {
                    case EbxFieldType.TypeRef:
                        {
                            TypeRef typeRefObj = (TypeRef)obj;
                            WriteTypeRef(typeRefObj, false, writer);
                        }
                        break;

                    case EbxFieldType.FileRef:
                        {
                            string str = (FileRef)obj;
                            writer.Write((long)AddString(str));
                        }
                        break;

                    case EbxFieldType.CString:
                        {
                            string str = (CString)obj;
                            writer.Write((long)AddString(str));
                        }
                        break;

                    case EbxFieldType.Pointer:
                        {
                            PointerRef pointer = (PointerRef)obj;
                            WritePointer(pointer, false, writer);
                        }
                        break;

                    case EbxFieldType.Struct:
                        {
                            object structValue = obj;
                            Type structType = structValue.GetType();
                            EbxClassMetaAttribute cta = structType.GetCustomAttribute<EbxClassMetaAttribute>();

                            writer.WritePadding(cta.Alignment);
                            WriteClass(structValue, structType, writer.Position, writer);
                        }
                        break;

                    case EbxFieldType.Enum:        writer.Write((int)obj); break;
                    case EbxFieldType.Float32:     writer.Write((float)obj); break;
                    case EbxFieldType.Float64:     writer.Write((double)obj); break;
                    case EbxFieldType.Boolean:     writer.Write((byte)(((bool)obj) ? 0x01 : 0x00)); break;
                    case EbxFieldType.Int8:        writer.Write((sbyte)obj); break;
                    case EbxFieldType.UInt8:       writer.Write((byte)obj); break;
                    case EbxFieldType.Int16:       writer.Write((short)obj); break;
                    case EbxFieldType.UInt16:      writer.Write((ushort)obj); break;
                    case EbxFieldType.Int32:       writer.Write((int)obj); break;
                    case EbxFieldType.UInt32:      writer.Write((uint)obj); break;
                    case EbxFieldType.Int64:       writer.Write((long)obj); break;
                    case EbxFieldType.UInt64:      writer.Write((ulong)obj); break;
                    case EbxFieldType.Guid:        writer.Write((Guid)obj); break;
                    case EbxFieldType.Sha1:        writer.Write((Sha1)obj); break;
                    case EbxFieldType.String:      writer.WriteFixedSizedString((string)obj, 32); break;
                    case EbxFieldType.ResourceRef: writer.Write((ResourceRef)obj); break;

                    default: throw new InvalidDataException($"Unhandled field type: {value.Type}");
                }

                return writer.ToByteArray();
            }
        }

        protected override uint AddString(string stringToAdd)
        {
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

        private void FixupPointers(NativeWriter writer)
        {
            for (int i = 0; i < m_sortedObjs.Count; i++)
            {
                Type type = m_sortedObjs[i].GetType();
                int classIdx = FindExistingClass(type);
                EbxClass classType = m_classTypes[classIdx];

                writer.Position = m_exportOffsets[i];

                writer.Position += sizeof(long);
                if (classType.Alignment != 0x04)
                {
                    writer.Position += sizeof(long);
                }

                writer.Position += sizeof(int);
                writer.Position += sizeof(int);

                FixupClass(m_sortedObjs[i], type, m_exportOffsets[i], writer);
            }
        }

        private void FixupClass(object obj, Type objType, long startOffset, NativeWriter writer)
        {
            // sweep through the EBX data after writing it to patch pointers/offsets

            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                FixupClass(obj, objType.BaseType, startOffset, writer);
            }

            PropertyInfo[] pis = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            // sort properties by field index for better accuracy
            IEnumerable<PropertyInfo> sortedPis = pis.OrderBy(p =>
            {
                FieldIndexAttribute fia = p.GetCustomAttribute<FieldIndexAttribute>();
                return fia.Index;
            });

            EbxClass classType = GetClass(objType);

            foreach (PropertyInfo pi in sortedPis)
            {
                EbxFieldMetaAttribute fta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();
                bool isReference = pi.GetCustomAttribute<IsReferenceAttribute>() != null;
                bool isTransient = pi.GetCustomAttribute<IsTransientAttribute>() != null;

                if (fta.Type == EbxFieldType.Inherited || isTransient)
                {
                    continue;
                }

                // we shouldn't have a null property, but handle it anyway
                if (pi == null)
                {
                    switch (fta.Type)
                    {
                        case EbxFieldType.TypeRef:
                        case EbxFieldType.FileRef:
                        case EbxFieldType.CString:
                        case EbxFieldType.Array:
                        case EbxFieldType.Pointer: writer.Position += 8; break;

                        case EbxFieldType.Struct:
                            {
                                EbxClassMetaAttribute meta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
                                while (writer.Position != meta.Alignment)
                                {
                                    ++writer.Position;
                                }
                                writer.Position += meta.Size;
                            }
                            break;

                        case EbxFieldType.Enum: writer.Position += 4; break;
                        case EbxFieldType.Float32: writer.Position += 4; break;
                        case EbxFieldType.Float64: writer.Position += 8; break;
                        case EbxFieldType.Boolean: writer.Position += 1; break;
                        case EbxFieldType.Int8: writer.Position += 1; break;
                        case EbxFieldType.UInt8: writer.Position += 1; break;
                        case EbxFieldType.Int16: writer.Position += 2; break;
                        case EbxFieldType.UInt16: writer.Position += 2; break;
                        case EbxFieldType.Int32: writer.Position += 4; break;
                        case EbxFieldType.UInt32: writer.Position += 4; break;
                        case EbxFieldType.Int64: writer.Position += 8; break;
                        case EbxFieldType.UInt64: writer.Position += 8; break;
                        case EbxFieldType.Guid: writer.Position += 16; break;
                        case EbxFieldType.Sha1: writer.Position += 20; break;
                        case EbxFieldType.String: writer.Position += 32; break;
                        case EbxFieldType.ResourceRef: writer.Position += 8; break;
                        case EbxFieldType.BoxedValueRef: writer.Position += 16; break;
                    }

                    continue;
                }

                writer.Position = startOffset + fta.Offset;

                FixupField(pi.GetValue(obj), fta.Type, writer);
            }

            writer.Position = startOffset + classType.Size;
            while (writer.Position % classType.Alignment != 0)
            {
                ++writer.Position;
            }
        }

        private void FixupField(object obj, EbxFieldType ebxType, NativeWriter writer, EbxFieldType fieldArrayType = EbxFieldType.Inherited)
        {
            switch (ebxType)
            {
                case EbxFieldType.TypeRef:
                    {
                        FixupTypeRef(writer.Position, writer);
                    }
                    break;

                case EbxFieldType.FileRef:
                case EbxFieldType.CString:
                    {
                        FixupPointer(writer.Position, m_stringsOffset, writer);
                    }
                    break;

                case EbxFieldType.Pointer:
                    {
                        PointerRef pointer = (PointerRef)obj;

                        if (pointer.Type == PointerRefType.External)
                        {
                            m_importOffsets.Add((uint)writer.Position);
                            writer.Position += sizeof(long);
                        }
                        else if (pointer.Type == PointerRefType.Internal)
                        {
                            FixupInternalRef(writer.Position, writer);
                        }
                        else if (pointer.Type == PointerRefType.Null)
                        {
                            writer.Position += sizeof(long);
                        }
                    }
                    break;

                case EbxFieldType.Struct:
                    {
                        object structValue = obj;
                        Type structType = structValue.GetType();
                        EbxClassMetaAttribute cta = structType.GetCustomAttribute<EbxClassMetaAttribute>();

                        while (writer.Position % cta.Alignment != 0)
                        {
                            ++writer.Position;
                        }

                        FixupClass(structValue, structType, writer.Position, writer);
                    }
                    break;

                case EbxFieldType.Array:
                    {
                        FixupArray(obj, writer.Position, writer);
                    }
                    break;

                case EbxFieldType.ResourceRef:
                    {
                        FixupResourceRef(writer.Position, writer);
                    }
                    break;
                case EbxFieldType.BoxedValueRef:
                    {
                        FixupTypeRef(writer.Position, writer);
                        FixupBoxedValue(obj, writer.Position, writer);
                    }
                    break;

                case EbxFieldType.Enum:    writer.Position += sizeof(int); break;
                case EbxFieldType.Float32: writer.Position += sizeof(float); break;
                case EbxFieldType.Float64: writer.Position += sizeof(double); break;
                case EbxFieldType.Boolean: writer.Position += sizeof(bool); break;
                case EbxFieldType.Int8:    writer.Position += sizeof(byte); break;
                case EbxFieldType.UInt8:   writer.Position += sizeof(byte); break;
                case EbxFieldType.Int16:   writer.Position += sizeof(short); break;
                case EbxFieldType.UInt16:  writer.Position += sizeof(short); break;
                case EbxFieldType.Int32:   writer.Position += sizeof(int); break;
                case EbxFieldType.UInt32:  writer.Position += sizeof(int); break;
                case EbxFieldType.Int64:   writer.Position += sizeof(long); break;
                case EbxFieldType.UInt64:  writer.Position += sizeof(long); break;
                case EbxFieldType.Guid:    writer.Position += 16; break;
                case EbxFieldType.Sha1:    writer.Position += 20; break;
                case EbxFieldType.String:  writer.Position += 32; break;

                default: throw new InvalidDataException($"Unhandled field type: {ebxType}");
            }
        }

        private void FixupPointer(long fieldOffset, long sectionOffset, NativeWriter writer)
        {
            m_pointerOffsets.Add((uint)fieldOffset);

            byte[] offsetBytes = new byte[sizeof(long)];
            writer.BaseStream.Read(offsetBytes, 0, sizeof(long));
            writer.Position = fieldOffset;

            long offset = sectionOffset + BitConverter.ToInt64(offsetBytes, 0);
            offset -= writer.Position;
            // file pointers only use 32 bits, but runtime pointers need 64 bits when being patched
            // so just cast down and zero out the other 32 bits
            writer.Write((int)offset);
            writer.Write(0);
        }

        private void FixupInternalRef(long fieldOffset, NativeWriter writer)
        {
            m_pointerOffsets.Add((uint)fieldOffset);

            byte[] refIdxBytes = new byte[sizeof(long)];
            writer.BaseStream.Read(refIdxBytes, 0, sizeof(long));
            writer.Position = fieldOffset;

            int refIdx = (int)BitConverter.ToInt64(refIdxBytes, 0);
            
            long offset = m_exportOffsets[refIdx];
            offset -= writer.Position;
            // file pointers only use 32 bits, but runtime pointers need 64 bits when being patched
            // so just cast down and zero out the other 32 bits
            writer.Write((int)offset);
            writer.Write(0);
        }

        private void FixupArray(object obj, long fieldOffset, NativeWriter writer)
        {
            // array pointers need to be in the pointer list
            m_pointerOffsets.Add((uint)fieldOffset);

            byte[] arrayIdxBytes = new byte[sizeof(long)];
            writer.BaseStream.Read(arrayIdxBytes, 0, sizeof(long));
            writer.Position = fieldOffset;

            int arrayIdx = (int)BitConverter.ToInt64(arrayIdxBytes, 0);
            EbxArray array = m_arrays[arrayIdx];

            if (array.Count == 0)
            {
                // arrays with zero elements always point to an empty value 16 bytes into the array section
                long offset = m_arraysOffset + 0x10;
                offset -= writer.Position;
                writer.Write((int)offset);
                writer.Write(0);
            }
            else
            {
                long offset = array.Offset;
                offset -= writer.Position;
                // file pointers only use 32 bits, but runtime pointers need 64 bits when being patched
                // so just cast down and zero out the other 32 bits
                writer.Write((int)offset);
                writer.Write(0);

                long oldPos = writer.Position;
                writer.Position = array.Offset;

                IList arrayObj = (IList)obj;
                for (int i = 0; i < array.Count; i++)
                {
                    object subValue = arrayObj[i];
                    EbxFieldType arrayType = (EbxFieldType)((array.Type >> 5) & 0x1F);
                    FixupField(subValue, arrayType, writer);
                }

                writer.Position = oldPos;
            }            
        }

        private void FixupTypeRef(long fieldOffset, NativeWriter writer)
        {
            byte[] typeBytes = new byte[sizeof(int)];
            writer.BaseStream.Read(typeBytes, 0, sizeof(int));
            byte[] classRefBytes = new byte[sizeof(int)];
            writer.BaseStream.Read(classRefBytes, 0, sizeof(int));

            uint type = BitConverter.ToUInt32(typeBytes, 0);
            int classRef = BitConverter.ToInt32(classRefBytes, 0);

            if (type == 0 && classRef == 0)
            {
                return;
            }

            m_typeInfoOffsets.Add((uint)fieldOffset);
        }

        private void FixupResourceRef(long fieldOffset, NativeWriter writer)
        {
            byte[] refBytes = new byte[sizeof(long)];
            writer.BaseStream.Read(refBytes, 0, sizeof(long));

            ulong resourceRef = BitConverter.ToUInt64(refBytes, 0);

            if (resourceRef == 0)
            {
                return;
            }

            m_resourceRefOffsets.Add((uint)fieldOffset);
        }

        private void FixupBoxedValue(object obj, long fieldOffset, NativeWriter writer)
        {
            BoxedValueRef value = (BoxedValueRef)obj;

            byte[] boxedValIdxBytes = new byte[sizeof(long)];
            writer.BaseStream.Read(boxedValIdxBytes, 0, sizeof(long));
            writer.Position = fieldOffset;

            int boxedValIdx = (int)BitConverter.ToInt64(boxedValIdxBytes, 0);
            // null boxed values always have an offset of zero
            if (m_boxedValueData[boxedValIdx] == null)
            {
                writer.Write((ulong)0);
                return;
            }

            // boxed value pointers need to be in the pointer list
            m_pointerOffsets.Add((uint)fieldOffset);
            EbxBoxedValue boxedVal = m_boxedValues[boxedValIdx];

            long offset = boxedVal.Offset;
            offset -= writer.Position;
            // file pointers only use 32 bits, but runtime pointers need 64 bits when being patched
            // so just cast down and zero out the other 32 bits
            writer.Write((int)offset);
            writer.Write(0);

            long oldPos = writer.Position;
            writer.Position = boxedVal.Offset;

            EbxFieldType valueType = (EbxFieldType)((boxedVal.Type >> 5) & 0x1F);
            FixupField(value.Value, valueType, writer);

            writer.Position = oldPos;
        }

        private int FindExistingClass(Type inType) => m_typesToProcess.FindIndex((Type value) => value == inType);

        private void AddTypeName(string inName)
        {
            if (!m_typeNames.Contains(inName))
            {
                m_typeNames.Add(inName);
            }
        }

        private int AddClass(string name, Type classType, bool addSignature = true)
        {
            Guid classGuid = classType.GetCustomAttribute<GuidAttribute>().Guid;
            m_classGuids.Add(classGuid);

            EbxClass ebxClass = GetClass(classType);
            m_classTypes.Add(ebxClass);

            if (addSignature)
            {
                Guid tiGuid = classType.GetCustomAttribute<TypeInfoGuidAttribute>().Guid;
                m_classSignatures.Add(BitConverter.ToUInt32(tiGuid.ToByteArray(), 12));
            }

            AddTypeName(name);
            m_typesToProcess.Add(classType);
            return m_classTypes.Count - 1;
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

        internal EbxClass GetClass(Type objType)
        {
            EbxClass? classType = null;
            foreach (TypeInfoGuidAttribute attr in objType.GetCustomAttributes<TypeInfoGuidAttribute>())
            {
                if (classType == null)
                    classType = GetClass(attr.Guid);
                break;
            }

            if (!classType.HasValue)
            {
                if (objType.Name.StartsWith("Delegate"))
                {
                    classType = new EbxClass()
                    {
                        Type = 456,
                        Name = objType.GetCustomAttribute<DisplayNameAttribute>().Name
                    };
                }
                else
                {
                    EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
                    if (cta == null)
                    {
                        throw new InvalidDataException($"Unhandled type: {objType.Name}");
                    }
                    classType = new EbxClass()
                    {
                        Type = cta.Flags,
                        Name = objType.Name,
                        Alignment = cta.Alignment,
                        Size = cta.Size
                    };
                }
            }
            return classType.Value;
        }

        internal EbxClass GetClass(Guid guid)
        {
            if (EbxReaderV2.patchStd != null)
            {
                EbxClass? ebxClass = EbxReaderV2.patchStd.GetClass(guid);
                if (ebxClass.HasValue)
                {
                    return ebxClass.Value;
                }
            }
            return EbxReaderV2.std.GetClass(guid).Value;
        }

        internal EbxField GetField(EbxClass classType, int index)
        {
            if (EbxReaderV2.patchStd != null)
            {
                EbxField? ebxClass = EbxReaderV2.patchStd.GetField(index);
                if (ebxClass.HasValue)
                {
                    return ebxClass.Value;
                }
            }
            return EbxReaderV2.std.GetField(index).Value;
        }

    }

}
