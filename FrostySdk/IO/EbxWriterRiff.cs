using FrostySdk.Attributes;
using FrostySdk.Ebx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FrostySdk.IO
{

    internal class RiffEbxContainer
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

        private List<RelocPtr> relocPtrs = new List<RelocPtr>();

        public RiffEbxContainer()
        {
        }

        public void AddRelocPtr(string type, object obj)
        {
            relocPtrs.Add(new RelocPtr(type, obj));
        }

        public void AddRelocPtr(string type, object obj, NativeWriter writer)
        {
            RelocPtr ptr = new RelocPtr(type, obj);
            ptr.Offset = writer.Position;
            relocPtrs.Add(ptr);
            writer.Write(0xdeadbeefdeadbeef);
        }

        public void WriteRelocPtr(string type, object obj, NativeWriter writer)
        {
            RelocPtr ptr = FindRelocPtr(type, obj);
            ptr.Offset = writer.Position;
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

        public void AddRelativeOffset(string type, object data, NativeWriter writer)
        {
            RelocPtr ptr = FindRelocPtr(type, data);
            if (ptr != null)
            {
                ptr.DataOffset = writer.Position - ptr.Offset;
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

    public class EbxWriterRiff : EbxBaseWriter
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
        private List<uint> classSignatures = new List<uint>();
        private List<EbxField> fieldTypes = new List<EbxField>();
        private List<string> typeNames = new List<string>();
        private List<EbxImportReference> imports = new List<EbxImportReference>();

        private byte[] data = null;
        private List<EbxInstance> instances = new List<EbxInstance>();
        private List<EbxArray> arrays = new List<EbxArray>();
        private List<byte[]> arrayData = new List<byte[]>();
        private List<uint> arrayFieldOffsets = new List<uint>();
        private List<uint> pointerOffsets = new List<uint>();
        private List<uint> exportOffsets = new List<uint>();
        private List<uint> importOffsets = new List<uint>();
        private List<uint> resourceRefOffsets = new List<uint>();
        private RiffEbxContainer riffEbxContainer = new RiffEbxContainer();

        private ushort uniqueClassCount = 0;
        private uint exportedCount = 0;
        private uint emptyArrayCount = 0;
        private uint arraysOffset = 0;
        private uint boxedValuesOffset = 0;
        private uint stringsOffset = 0;

        internal EbxWriterRiff(Stream inStream, EbxWriteFlags inFlags = EbxWriteFlags.None, bool leaveOpen = false)
            : base(inStream, inFlags, leaveOpen)
        {
            flags = inFlags;
        }

        public override void WriteAsset(EbxAsset asset)
        {
            if (flags.HasFlag(EbxWriteFlags.DoNotSort))
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
            //objsToProcess.Reverse();
            foreach (object obj in objsToProcess)
            {
                ProcessClass(obj.GetType());
            }
            for (int i = 0; i < typesToProcess.Count; i++)
            {
                ProcessType(i);
            }

            ProcessData();

            Write((int)EbxVersion.Version6);
            Write(0x00); // total size - 8
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
                    writer.Write(data);

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

                    writer.Write(classGuids.Count);
                    foreach (Guid guid in classGuids)
                    {
                        writer.Write(guid);
                    }

                    // should always be equal to the class GUID count
                    writer.Write(classSignatures.Count);
                    foreach (uint sig in classSignatures)
                    {
                        writer.Write(sig);
                    }

                    writer.Write(exportedCount);
                    writer.Write(exportOffsets.Count);
                    foreach (uint off in exportOffsets)
                    {
                        writer.Write(off);
                    }

                    writer.Write(pointerOffsets.Count);
                    foreach (uint off in pointerOffsets)
                    {
                        writer.Write(off);
                    }

                    writer.Write(resourceRefOffsets.Count);
                    foreach (uint off in resourceRefOffsets)
                    {
                        writer.Write(off);
                    }

                    writer.Write(imports.Count);
                    foreach (EbxImportReference import in imports)
                    {
                        writer.Write(import.FileGuid);
                        writer.Write(import.ClassGuid);
                    }

                    writer.Write(importOffsets.Count); // should always be the same as the import count
                    foreach (uint off in importOffsets)
                    {
                        writer.Write(off);
                    }

                    // @todo: type info offsets / type info refs
                    writer.Write(0x00);

                    writer.Write(arraysOffset);
                    writer.Write(boxedValuesOffset);
                    writer.Write(stringsOffset);
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
                    writer.Write(arrays.Count);
                    writer.Write(boxedValues.Count);
                    foreach (EbxArray arr in arrays)
                    {
                        writer.Write(arr.Offset);
                        writer.Write(arr.Count);
                        writer.Write(0x00); // unknown, varies between assets
                        writer.Write((ushort)arr.Type);
                        writer.Write((short)arr.ClassRef);
                    }

                    foreach (EbxBoxedValue val in boxedValues)
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
                if (objsToProcess.Contains(obj))
                {
                    return new List<object>();
                }

                objsToProcess.Add(obj);
                objs.Add(obj);
            }

            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            List<object> retObjects = new List<object>();

            foreach (PropertyInfo pi in pis)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !flags.HasFlag(EbxWriteFlags.IncludeTransient))
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
                    else if (value.Type == PointerRefType.External && !imports.Contains(value.External))
                    {
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
                                else if (value.Type == PointerRefType.External && !imports.Contains(value.External))
                                {
                                    imports.Add(value.External);
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

        private ushort ProcessClass(Type objType, bool isBaseClass = false)
        {
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                ProcessClass(objType.BaseType, true);
            }

            int index = FindExistingClass(objType);
            if (index != -1)
            {
                return (ushort)index;
            }

            EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
            PropertyInfo[] allProps = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<PropertyInfo> pis = new List<PropertyInfo>();

            foreach (PropertyInfo pi in allProps)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() == null || flags.HasFlag(EbxWriteFlags.IncludeTransient))
                {
                    pis.Add(pi);
                }
            }

            // only child classes and classes that don't inherit should be added
            if (!isBaseClass)
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

                if (ebxType == EbxFieldType.Struct)
                {
                    Type structType = pi.PropertyType;
                    ProcessClass(structType, isBaseClass);
                }
                else if (ebxType == EbxFieldType.Array)
                {
                    ebxType = fta.ArrayType;

                    Type arrayType = pi.PropertyType;
                    if (FindExistingClass(arrayType) == -1)
                    {

                        if (!typesToProcess.Contains(arrayType))
                        {
                            arrayTypes.Add(fta);
                        }
                        if (ebxType == EbxFieldType.Struct)
                        {
                            Type structType = arrayType.GenericTypeArguments[0];
                            ProcessClass(structType, isBaseClass);
                        }
                    }
                }
            }

            return (ushort)index;
        }

        private void ProcessType(int index)
        {
            Type type = typesToProcess[index];
            EbxClassMetaAttribute cta = type.GetCustomAttribute<EbxClassMetaAttribute>();

            if (cta.Type == EbxFieldType.Array)
            {
                EbxFieldMetaAttribute ata = arrayTypes[0];
                arrayTypes.RemoveAt(0);

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
                    if (pi.GetCustomAttribute<IsTransientAttribute>() != null && !flags.HasFlag(EbxWriteFlags.IncludeTransient))
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
            ushort classRef = (ushort)typesToProcess.FindIndex((Type value) => value == propType);
            //if (classRef == 0xFFFF)
            //    classRef = 0;

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
                {
                    exportedObjs.Add(obj);
                }
                else
                {
                    otherObjs.Add(obj);
                }
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
                    {
                        uniqueTypes.Add(type);
                    }

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
                    {
                        writer.Write(guid.ExportedGuid);
                    }
                    exportOffsets.Add((uint)writer.Position);
                    writer.Write((ulong)classIdx);
                    if (classType.Alignment != 0x04)
                    {
                        writer.Write((ulong)0);
                    }

                    // both unknown but seem to always be the same (needs more testing)
                    writer.Write(2u);
                    writer.Write(45312u); // possibly flags?

                    WriteClass(sortedObjs[i], type, writer);
                    count++;
                }

                // Add final instance
                inst.Count = count;
                instances.Add(inst);

                writer.WritePadding(16);
                arraysOffset = (uint)writer.Position;
                // 32 bytes are used for an empty array(?)
                byte[] arrayPad = new byte[16];
                writer.Write(arrayPad);
                for (int i = 0; i < emptyArrayCount; ++i)
                {
                    // empty arrays reference the same offset
                    riffEbxContainer.AddRelativeOffset("EMPTY_ARRAY", $"arr_{i}", writer);
                }
                writer.Write(arrayPad);

                if (arrays.Count > 0)
                {
                    for (int i = 0; i < arrays.Count; i++)
                    {
                        EbxArray array = arrays[i];

                        EbxClass arrayClassType = classTypes[array.ClassRef];
                        // shift where the count is so that the array data is properly aligned
                        long dataPos = writer.Position + 4;
                        if (arrayClassType.Alignment != 4)
                        {
                            while (dataPos % arrayClassType.Alignment != 0)
                            {
                                writer.Write((byte)0);
                                dataPos = writer.Position;
                            }
                            writer.Position -= 0x4;
                        }
                        writer.Write(array.Count);

                        riffEbxContainer.AddRelativeOffset("ARRAY", $"arr_{i}", writer);
                        array.Offset = (uint)writer.Position;
                        writer.Write(arrayData[i]);

                        arrays[i] = array;
                    }
                    WritePadding(16);
                }

                boxedValuesOffset = (uint)writer.Position;
                foreach (byte[] valueData in boxedValueData)
                {
                    writer.Write(valueData);
                }

                stringsOffset = (uint)writer.Position;
                foreach (string str in strings)
                {
                    riffEbxContainer.AddRelativeOffset("STRING", str, writer);
                    writer.WriteNullTerminatedString(str);
                }

                for (int i = 0; i < sortedObjs.Count; ++i)
                {
                    writer.Position = exportOffsets[i];
                    riffEbxContainer.AddRelativeOffset("POINTER", $"ptr_{i}", writer);
                }

                riffEbxContainer.FixupRelocPtrs(writer);
            }

            data = dataStream.ToArray();
            uniqueClassCount = (ushort)uniqueTypes.Count;
        }

        private void WriteClass(object obj, Type objType, NativeWriter writer)
        {
            if (objType.BaseType.Namespace == "FrostySdk.Ebx")
            {
                WriteClass(obj, objType.BaseType, writer);
            }

            PropertyInfo[] pis = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            // properties need to be sorted by offset
            IEnumerable<PropertyInfo> sortedPis = pis.OrderBy(p =>
            {
                EbxFieldMetaAttribute fta = p.GetCustomAttribute<EbxFieldMetaAttribute>();
                return fta.Offset;
            });

            EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();

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
                    if (fta.Type == EbxFieldType.ResourceRef
                        || fta.Type == EbxFieldType.TypeRef
                        || fta.Type == EbxFieldType.FileRef
                        || fta.Type == EbxFieldType.BoxedValueRef
                        || fta.Type == EbxFieldType.UInt64
                        || fta.Type == EbxFieldType.Int64
                        || fta.Type == EbxFieldType.Float64)
                    {
                        writer.WritePadding(8);
                    }
                    else if (fta.Type == EbxFieldType.Array || fta.Type == EbxFieldType.Pointer)
                    {
                        writer.WritePadding(4);
                    }

                    switch (fta.Type)
                    {
                        case EbxFieldType.TypeRef: writer.Write((ulong)0); break;
                        case EbxFieldType.FileRef: writer.Write((ulong)0); break;
                        case EbxFieldType.CString: writer.Write(0); break;
                        case EbxFieldType.Pointer: writer.Write(0); break;

                        case EbxFieldType.Struct:
                            {
                                EbxClass structType = GetClass(objType);
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

                
                WriteField(pi.GetValue(obj), fta.Type, fta, cta.Alignment, writer, isReference);
            }

            writer.WritePadding(cta.Alignment);
        }

        private void WriteField(object obj, EbxFieldType ebxType, EbxFieldMetaAttribute fieldMeta, byte classAlignment, NativeWriter writer, bool isReference)
        {
            switch (ebxType)
            {
                case EbxFieldType.TypeRef:
                    {
                        string str = (TypeRef)obj;
                        AddString(str);
                        pointerOffsets.Add((uint)writer.Position);
                        riffEbxContainer.AddRelocPtr("STRING", str, writer);
                    }
                    break;

                case EbxFieldType.FileRef:
                    {
                        string str = (FileRef)obj;
                        AddString(str);
                        pointerOffsets.Add((uint)writer.Position);
                        riffEbxContainer.AddRelocPtr("STRING", str, writer);
                    }
                    break;

                case EbxFieldType.CString:
                    {
                        string str = (CString)obj;
                        AddString(str);
                        pointerOffsets.Add((uint)writer.Position);
                        riffEbxContainer.AddRelocPtr("STRING", str, writer);
                    }
                    break;

                case EbxFieldType.Pointer:
                    {
                        PointerRef pointer = (PointerRef)obj;
                        uint pointerIndex = 0;

                        if (pointer.Type == PointerRefType.External)
                        {
                            int importIdx = imports.FindIndex((EbxImportReference value) => value == pointer.External);
                            pointerIndex = (uint)(importIdx | 0x80000000);

                            if (isReference && !dependencies.Contains(imports[importIdx].FileGuid))
                            {
                                dependencies.Add(imports[importIdx].FileGuid);
                            }
                            importOffsets.Add((uint)writer.Position);
                            writer.Write((ulong)pointerIndex);
                        }
                        else if (pointer.Type == PointerRefType.Internal)
                        {
                            pointerOffsets.Add((uint)writer.Position);
                            riffEbxContainer.AddRelocPtr("POINTER", $"ptr_{sortedObjs.FindIndex((object value) => value == pointer.Internal)}", writer);
                        }

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
                        int arrayClassIdx = typesToProcess.FindIndex((Type item) => item == obj.GetType().GenericTypeArguments[0]);
                        int arrayIdx = 0;

                        // cast to IList to avoid having to invoke methods manually
                        IList arrayObj = (IList)obj;
                        int count = arrayObj.Count;

                        // array pointers need to be in the pointer list
                        pointerOffsets.Add((uint)writer.Position);

                        if (count != 0)
                        {
                            MemoryStream arrayStream = new MemoryStream();
                            using (NativeWriter arrayWriter = new NativeWriter(arrayStream))
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    object subValue = arrayObj[i];
                                    WriteField(subValue, fieldMeta.ArrayType, fieldMeta, classAlignment, arrayWriter, isReference);
                                }
                            }

                            arrayIdx = arrays.Count;
                            arrays.Add(
                                new EbxArray()
                                {
                                    Count = (uint)count,
                                    ClassRef = arrayClassIdx,
                                    Type = fieldMeta.ArrayFlags
                                });
                            arrayData.Add(arrayStream.ToArray());
                            riffEbxContainer.AddRelocPtr("ARRAY", $"arr_{arrayIdx}", writer);
                        }
                        else
                        {
                            riffEbxContainer.AddRelocPtr("EMPTY_ARRAY", $"arr_{emptyArrayCount++}", writer);
                        }
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
                case EbxFieldType.ResourceRef:
                    {
                        resourceRefOffsets.Add((uint)writer.Position);
                        writer.Write((ResourceRef)obj);
                    }
                    break;
                case EbxFieldType.BoxedValueRef:
                    {
                        /*BoxedValueRef value = (BoxedValueRef)obj;
                        int index = boxedValues.Count;

                        if (value.Type == EbxFieldType.Inherited)
                        {
                            index = -1;
                        }
                        else
                        {
                            boxedValueWriter.Write(0);
                            EbxBoxedValue boxedValue = new EbxBoxedValue() { Offset = (uint)boxedValueWriter.Position, Type = (ushort)value.Type };
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

                                WriteField(value.Value, EbxFieldType.Struct, 0, boxedValueWriter, isReference);
                            }
                            else
                            {
                                boxedValueWriter.Write(WriteBoxedValueRef(value));
                            }
                            boxedValues.Add(boxedValue);
                        }

                        writer.Write(index);
                        writer.Write((ulong)0);
                        writer.Write((uint)0);*/
                    }
                    {
                        BoxedValueRef value = (BoxedValueRef)obj;
                        int index = boxedValues.Count;

                        if (value.Type == EbxFieldType.Inherited)
                        {
                            index = -1;
                        }
                        else
                        {
                            EbxBoxedValue boxedValue = new EbxBoxedValue() { Offset = 0, Type = (ushort)value.Type };

                            boxedValues.Add(boxedValue);
                            boxedValueData.Add(WriteBoxedValueRef(value));
                        }

                        Write(index);
                        Write((ulong)0);
                        Write((uint)0);
                    }
                    break;

                default: throw new InvalidDataException($"Unhandled field type: {ebxType}");
            }
        }

        private int FindExistingClass(Type inType) => typesToProcess.FindIndex((Type value) => value == inType);

        private void AddTypeName(string inName)
        {
            if (!typeNames.Contains(inName))
            {
                typeNames.Add(inName);
            }
        }

        private int AddClass(string name, Type classType)
        {
            Guid classGuid = classType.GetCustomAttribute<GuidAttribute>().Guid;
            classGuids.Add(classGuid);

            EbxClass ebxClass = GetClass(classType);
            classTypes.Add(ebxClass);

            Guid tiGuid = classType.GetCustomAttribute<TypeInfoGuidAttribute>().Guid;
            classSignatures.Add(BitConverter.ToUInt32(tiGuid.ToByteArray(), 12));

            AddTypeName(name);
            typesToProcess.Add(classType);
            return classTypes.Count - 1;
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
