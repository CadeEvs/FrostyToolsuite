using FrostySdk.Attributes;
using FrostySdk.Ebx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FrostySdk.IO
{

    public class EbxReaderRiff : EbxReaderV2
    {
        private List<uint> dataOffsets = new List<uint>();
        private List<uint> pointerOffsets = new List<uint>();
        private List<uint> resourceRefOffsets = new List<uint>();
        private List<uint> importOffsets = new List<uint>();
        private List<uint> typeInfoOffsets = new List<uint>();

        private long dataStartOffset;

        public EbxReaderRiff(Stream InStream, FileSystemManager fs, bool inPatched)
            : base(InStream, true)
        {
            if (std == null)
            {
                std = new EbxSharedTypeDescriptors(fs, "SharedTypeDescriptors.ebx");
                if (fs.HasFileInMemoryFs("SharedTypeDescriptors_patch.ebx"))
                {
                    patchStd = new EbxSharedTypeDescriptors(fs, "SharedTypeDescriptors_patch.ebx");
                }
            }

            Position = 0;
            patched = inPatched;

            // RIFF
            magic = (EbxVersion)ReadUInt();
            if (magic != EbxVersion.Version6)
                throw new InvalidDataException("Ebx not in RIFF format."); // RIFX and LIST
            uint size = ReadUInt();

            // EBX
            uint ebxHeader = ReadUInt(Endian.Big);
            if (ebxHeader != (uint)RiffEbxSection.EBX && ebxHeader != (uint)RiffEbxSection.EBXS) // EBXS ????
                throw new InvalidDataException("Not valid EBX/EBXS.");

            // EBXD
            if (ReadUInt(Endian.Big) != (uint)RiffEbxSection.EBXD)
            {
                throw new InvalidDataException("Not valid EBXD chunk.");
            }

            uint ebxdSize = ReadUInt();

            long ebxdOffset = Position;

            Pad(16);

            dataStartOffset = Position;

            Position = ebxdOffset + ebxdSize;

            Pad(2);

            // EFIX
            if (ReadUInt(Endian.Big) != (uint)RiffEbxSection.EFIX)
                throw new InvalidDataException("Not valid EFIX chunk.");
            uint efixSize = ReadUInt();

            long efixOffset = Position;

            fileGuid = ReadGuid();
            uint classGuidCount = ReadUInt();

            for (int i = 0; i < classGuidCount; i++)
            {
                classGuids.Add(ReadGuid());
            }

            uint signatureCount = ReadUInt();

            for (int i = 0; i < signatureCount; i++)
            {
                byte[] signature = ReadBytes(4);
                Guid classGuid = classGuids[i];
                byte[] classGuidBytes = classGuid.ToByteArray();

                byte[] typeInfoGuidByteArray = new byte[16];
                Array.Copy(classGuidBytes, 4, typeInfoGuidByteArray, 0, 12);
                Array.Copy(signature, 0, typeInfoGuidByteArray, 12, 4);

                Guid typeInfoGuid = new Guid(typeInfoGuidByteArray);
                classGuids[i] = typeInfoGuid;
            }

            exportedCount = ReadUInt();
            uint dataOffsetCount = ReadUInt();

            for (int i = 0; i < dataOffsetCount; i++)
            {
                uint offset = ReadUInt();
                dataOffsets.Add(offset);

                long curPos = Position;
                Position = dataStartOffset + offset;

                instances.Add
                (
                    new EbxInstance
                    {
                        ClassRef = ReadUShort(),
                        Count = 1,
                        IsExported = (i < exportedCount)
                    }
                );

                Position = curPos;
            }

            uint pointerOffsetCount = ReadUInt();
            for (int i = 0; i < pointerOffsetCount; i++)
            {
                pointerOffsets.Add(ReadUInt());
            }

            uint resourceRefOffsetCount = ReadUInt();
            for (int i = 0; i < resourceRefOffsetCount; i++)
            {
                resourceRefOffsets.Add(ReadUInt());
            }

            uint importReferenceCount = ReadUInt();
            for (int i = 0; i < importReferenceCount; i++)
            {
                imports.Add
                (
                    new EbxImportReference
                    {
                        FileGuid = ReadGuid(),
                        ClassGuid = ReadGuid()
                    }
                );
            }

            uint importOffsetCount = ReadUInt();
            for (int i = 0; i < importOffsetCount; i++)
            {
                importOffsets.Add(ReadUInt());
            }

            uint typeInfoOffsetCount = ReadUInt();
            for (int i = 0; i < typeInfoOffsetCount; i++)
            {
                typeInfoOffsets.Add(ReadUInt());
            }

            arraysOffset = ReadUInt();
            boxedValuesOffset = ReadUInt();
            stringsOffset = ReadUInt() + dataStartOffset;

            if (Position != efixOffset + efixSize)
            {
                Position = efixOffset + efixSize;
            }

            // EBXX
            if (ReadUInt(Endian.Big) != (uint)RiffEbxSection.EBXX)
                throw new InvalidDataException("Not valid EBXX chunk.");
            uint ebxxSize = ReadUInt();
            arrayCount = ReadUInt();
            boxedValuesCount = ReadUInt();

            for (int i = 0; i < arrayCount; i++)
            {
                uint offset = ReadUInt();
                uint count = ReadUInt();
                Position += 6;
                ushort classRef = ReadUShort();

                arrays.Add
                (
                    new EbxArray
                    {
                        Offset = offset,
                        Count = count,
                        ClassRef = classRef
                    }
                );
            }

            for (var i = 0; i < boxedValuesCount; i++)
            {
                uint offset = ReadUInt();
                uint count = ReadUInt();
                uint hash = ReadUInt();
                ushort type = ReadUShort();
                ushort classRef = ReadUShort();

                boxedValues.Add
                (
                    new EbxBoxedValue
                    {
                        Offset = offset,
                        Type = type,
                        ClassRef = classRef
                    }
                );
            }

            Position = dataStartOffset;
        }

        internal override void InternalReadObjects()
        {
            EbxClass c = std.GetClass(classGuids[0]).Value;
            foreach (EbxInstance inst in instances)
            {
                Type objType = TypeLibrary.GetType(classGuids[inst.ClassRef]);
                for (int i = 0; i < inst.Count; i++)
                {
                    objects.Add(TypeLibrary.CreateObject(objType));
                    refCounts.Add(0);
                }
            }

            int typeId = 0;
            int index = 0;

            for (int i = 0; i < instances.Count; i++)
            {
                EbxInstance inst = instances[i];

                for (int j = 0; j < inst.Count; j++)
                {
                    dynamic obj = objects[typeId++];
                    Type objType = obj.GetType();
                    EbxClass classType = GetClass(objType);

                    Pad(classType.Alignment);

                    Guid instanceGuid = Guid.Empty;
                    if (inst.IsExported)
                    {
                        instanceGuid = ReadGuid();
                    }

                    long classPos = Position;

                    obj.SetInstanceGuid(new AssetClassGuid(instanceGuid, index++));

                    ReadClass(classType, obj, Position);

                    Position = classPos + classType.Size;
                }
            }
        }

        internal override EbxClass GetClass(EbxClass? classType, int index)
        {
            EbxClass? newClassType = null;
            Guid? guid = null;

            if (!classType.HasValue)
            {
                guid = classGuids[index];
                newClassType = patchStd?.GetClass(guid.Value);
                if (!newClassType.HasValue)
                {
                    newClassType = std.GetClass(guid.Value);
                }
            }
            else
            {
                guid = std.GetGuid(index);

                if (classType.Value.SecondSize == 1)
                {
                    guid = patchStd.GetGuid(index);
                    newClassType = patchStd.GetClass(index);

                    if (newClassType == null)
                    {
                        newClassType = std.GetClass(guid.Value);
                    }
                }
                else
                {
                    newClassType = std.GetClass(index);
                }
            }

            if (newClassType.HasValue)
            {
                TypeLibrary.AddType(newClassType.Value.Name, guid);
            }

            return newClassType.Value;
        }

        internal override object ReadClass(EbxClass classType, object obj, long startOffset)
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

                    Position = fieldType.DataOffset + startOffset;

                    if (fieldType.DebugCategory == EbxFieldCategory.ArrayType)
                    {
                        long arrayPos = Position;
                        int arrayOffset = ReadInt();
                        Position += arrayOffset - 8;

                        int count = ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            object value = ReadField(classType, fieldType.DebugType, fieldType.ClassRef, (attr != null));
                            if (fieldProp != null)
                            {
                                try { fieldProp.GetValue(obj).GetType().GetMethod("Add").Invoke(fieldProp.GetValue(obj), new object[] { value }); }
                                catch (Exception) { }
                            }
                            if (fieldType.DebugType == EbxFieldType.Pointer || fieldType.DebugType == EbxFieldType.CString)
                            {
                                Pad(8);
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

            Position = startOffset + classType.Size;

            Pad(classType.Alignment);

            return null;
        }

        internal override object ReadClass(EbxClassMetaAttribute classMeta, object obj, Type objType, long startOffset)
        {
            if (obj == null)
            {
                Position += classMeta.Size;
                while (Position % classMeta.Alignment != 0)
                    Position++;
                return null;
            }

            if (objType.BaseType != typeof(object))
            {
                ReadClass(objType.BaseType.GetCustomAttribute<EbxClassMetaAttribute>(), obj, objType.BaseType, startOffset);
            }

            PropertyInfo[] pis = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo pi in pis)
            {
                if (pi.GetCustomAttribute<IsTransientAttribute>() != null)
                    continue;

                IsReferenceAttribute attr = pi.GetCustomAttribute<IsReferenceAttribute>();
                EbxFieldMetaAttribute fieldMeta = pi.GetCustomAttribute<EbxFieldMetaAttribute>();

                Position = startOffset + fieldMeta.Offset;
                if (fieldMeta.Type == EbxFieldType.Array)
                {
                    int arrayOffset = ReadInt();


                    EbxArray array = arrays.Find(a => a.Offset == Position - 4 - dataStartOffset + arrayOffset);

                    long arrayPos = Position;
                    Position = array.Offset + dataStartOffset;

                    // @temp: since constructors can add elements to an array, need to clear
                    //        before attempting to read in new elements

                    pi?.GetValue(obj).GetType().GetMethod("Clear").Invoke(pi.GetValue(obj), new object[] { });

                    // read in array elements
                    for (int i = 0; i < array.Count; i++)
                    {
                        object value = ReadField(fieldMeta.ArrayType, fieldMeta.BaseType, (attr != null));
                        pi?.GetValue(obj).GetType().GetMethod("Add").Invoke(pi.GetValue(obj), new object[] { value });
                    }
                    Position = arrayPos;
                }
                else
                {
                    object value = ReadField(fieldMeta.Type, pi.PropertyType, (attr != null));
                    pi?.SetValue(obj, value);
                }
            }

            while (Position - startOffset != classMeta.Size)
                Position++;

            return null;
        }

        internal bool IsArray(ushort fieldType)
        {
            return (fieldType & 0xF) == 4;
        }

        internal override string ReadString(uint offset)
        {
            if (offset == uint.MaxValue)
            {
                return string.Empty;
            }

            long pos = Position;
            Position += offset - 4;

            string retStr = ReadNullTerminatedString();
            Position = pos;

            return retStr;
        }

        internal override TypeRef ReadTypeRef()
        {
            uint type = ReadUInt();
            Position += 4;
            if (type == 0)
            {
                return new TypeRef();
            }

            // (always?) primitive type
            if ((type & 0x80000000) != 0)
            {
                type &= ~0x80000000;
                EbxFieldType valuetype = (EbxFieldType)((type >> 5) & 0x1F);
                return new TypeRef(valuetype.ToString());
            }
            else
            {
                int classRef = (int)(type >> 2);
                int unkVal = (int)(type & 3);
                Guid? classGuid = classGuids[classRef];
                if (classGuid.HasValue)
                {
                    return new TypeRef(classGuid.Value);
                }
                else
                {
                    return new TypeRef();
                }
            }
        }

        internal override PointerRef ReadPointerRef(bool dontRefCount)
        {
            int index = ReadInt();

            if (index == 0)
            {
                return new PointerRef();
            }
            else if ((index & 1) == 1)
            {
                return new PointerRef(imports[index >> 1]);
            }
            else
            {
                long pointerDataOffset = Position - 4 + index - dataStartOffset;
                int dataOffset = dataOffsets.IndexOf((uint)pointerDataOffset);

                if (!dontRefCount)
                {
                    refCounts[dataOffset]++;
                }

                return new PointerRef(objects[dataOffset]);
            }
        }

        internal override BoxedValueRef ReadBoxedValueRef()
        {
            uint type = ReadUInt();
            Position += 4;
            long valueOffset = ReadLong();
            long curPos = Position;
            try
            {
                if (type == 0)
                {
                    return new BoxedValueRef();
                }

                if ((type & 0x80000000) != 0)
                {
                    type &= ~0x80000000;
                }

                long offsetToLookup = Position - 8 + valueOffset - dataStartOffset;

                int boxedValueIndex = boxedValues.FindIndex(boxVal => boxVal.Offset == offsetToLookup);
                if (boxedValueIndex == -1)
                {
                    return new BoxedValueRef();
                }

                EbxBoxedValue boxedValue = boxedValues[boxedValueIndex];
                EbxFieldType subType = EbxFieldType.Inherited;
                EbxFieldType boxedValuetype = (EbxFieldType)((boxedValue.Type >> 5) & 0x1F);
                EbxFieldCategory boxedValuecategory = (EbxFieldCategory)((boxedValue.Type >> 1) & 0xF);

                // used when there isn't a type mask (0x80000000)
                int classRef = (int)(type >> 2);
                int unkVal = (int)(type & 3);

                Position = offsetToLookup + dataStartOffset;
                object value = null;

                if (boxedValuecategory == EbxFieldCategory.ArrayType)
                {
                    int arrValueOffset = ReadInt();
                    Position += arrValueOffset - 8;
                    uint arrayCount = ReadUInt();

                    int sizeOfStruct = 0;

                    if (boxedValue.ClassRef != ushort.MaxValue)
                    {
                        EbxClass arrayType = GetClass(null, boxedValue.ClassRef);
                        sizeOfStruct = arrayType.Size;

                        EbxField arrayField = GetField(arrayType, arrayType.FieldIndex);
                        value = Activator.CreateInstance(typeof(List<>).MakeGenericType(GetTypeFromEbxField(arrayField)));

                        long startingPosition = Position;

                        for (var i = 0; i < arrayCount; i++)
                        {
                            // If array of structs, align to the next struct size. Not all structs take up their size (because why would they??)
                            if (sizeOfStruct > 0)
                                Position = startingPosition + (sizeOfStruct * i);

                            object subValue = ReadField(arrayType, arrayField.DebugType, arrayField.ClassRef, false);
                            value.GetType().GetMethod("Add").Invoke(value, new object[] { subValue });
                        }
                        subType = arrayField.DebugType;
                    }
                    else
                    {
                        subType = boxedValuetype;
                    }
                }
                else
                {
                    value = ReadField(null, boxedValuetype, boxedValue.ClassRef);
                    Type objType = value.GetType();
                    EbxClassMetaAttribute cta = objType.GetCustomAttribute<EbxClassMetaAttribute>();
                    if (boxedValuetype == EbxFieldType.Enum)
                    {
                        object tmpValue = value;
                        EbxClass enumClass = GetClass(null, boxedValue.ClassRef);
                        value = Enum.Parse(GetType(enumClass), tmpValue.ToString());
                    }
                }
                
                return new BoxedValueRef(value, boxedValuetype, subType, boxedValuecategory);
            }
            finally
            {
                Position = curPos;
            }
        }
    }
}
