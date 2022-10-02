using FrostySdk.Attributes;
using FrostySdk.Ebx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FrostySdk.IO
{

    public class EbxReaderV2 : EbxReader
    {
        public override string RootType
        {
            get
            {
                Type type = TypeLibrary.GetType(classGuids[instances[0].ClassRef]);
                return type != null ? type.Name : "";
            }
        }
        internal List<Guid> classGuids = new List<Guid>();

        internal static EbxSharedTypeDescriptors std = null;
        internal static EbxSharedTypeDescriptors patchStd = null;
        internal static bool patched = false;

        internal EbxReaderV2(Stream inStream, bool passthru)
            : base(inStream, passthru)
        {
        }

        public EbxReaderV2(Stream InStream, FileSystemManager fs, bool inPatched)
            : base(InStream, true)
        {
            if (std == null)
            {
                std = new EbxSharedTypeDescriptors(fs, "SharedTypeDescriptors.ebx");
                if (fs.HasFileInMemoryFs("SharedTypeDescriptors_patch.ebx"))
                    patchStd = new EbxSharedTypeDescriptors(fs, "SharedTypeDescriptors_patch.ebx");
            }

            patched = inPatched;
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

            boxedValuesCount = ReadUInt();
            boxedValuesOffset = ReadUInt();
            boxedValuesOffset += stringsOffset + stringsLen;

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
                Guid guid = ReadGuid();
                classGuids.Add(guid);
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

        internal override void InternalReadObjects()
        {
            List<Type> types = new List<Type>();
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

            foreach (EbxInstance inst in instances)
            {
                for (int i = 0; i < inst.Count; i++)
                {
                    dynamic obj = objects[typeId++];
                    Type objType = obj.GetType();
                    EbxClass classType = GetClass(objType);

                    while (Position % classType.Alignment != 0)
                        Position++;

                    Guid instanceGuid = Guid.Empty;
                    if (inst.IsExported)
                        instanceGuid = ReadGuid();

                    if (classType.Alignment != 0x04)
                        Position += 8;

                    obj.SetInstanceGuid(new AssetClassGuid(instanceGuid, index++));
                    ReadClass(classType, obj, Position - 8);
                }
            }
        }

        internal EbxClass GetClass(Type objType)
        {
            EbxClass? classType = null;
            foreach (TypeInfoGuidAttribute attr in objType.GetCustomAttributes<TypeInfoGuidAttribute>())
            {
                if (classGuids.Contains(attr.Guid))
                {
                    if (patched && patchStd != null)
                        classType = patchStd.GetClass(attr.Guid);
                    if (classType == null)
                        classType = std.GetClass(attr.Guid);
                    break;
                }
            }
            return classType.Value;
        }

        internal override PropertyInfo GetProperty(Type objType, EbxField field)
        {
            if (field.NameHash == 0xb95a6ae7)
            {
                return null;
            }

            foreach (PropertyInfo pi in objType.GetProperties())
            {
                HashAttribute attr = pi.GetCustomAttribute<HashAttribute>();
                if (attr == null)
                    continue;

                int hash = attr.Hash;
                if (hash == (int)field.NameHash)
                    return pi;
            }
            return null;
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
                    newClassType = std.GetClass(guid.Value);
            }
            else
            {
                int idx = (short)index + ((classType.HasValue) ? classType.Value.Index : 0);
                guid = std.GetGuid(idx);

                if (classType.Value.SecondSize == 1)
                {
                    guid = patchStd.GetGuid(idx);
                    newClassType = patchStd.GetClass(idx);

                    if (newClassType == null)
                        newClassType = std.GetClass(guid.Value);
                }
                else
                    newClassType = std.GetClass(idx);
            }

            if (newClassType.HasValue)
                TypeLibrary.AddType(newClassType.Value.Name, guid);

            return newClassType.Value;
        }

        internal override EbxField GetField(EbxClass classType, int index)
        {
            return classType.SecondSize == 1 ? patchStd.GetField(index).Value : std.GetField(index).Value;
        }

        internal override object CreateObject(EbxClass classType)
        {
            return TypeLibrary.CreateObject(classType.SecondSize == 1 ? patchStd.GetGuid(classType).Value : std.GetGuid(classType).Value);
        }

        internal override Type GetType(EbxClass classType)
        {
            return TypeLibrary.GetType(classType.SecondSize == 1 ? patchStd.GetGuid(classType).Value : std.GetGuid(classType).Value);
        }

        internal virtual object ReadClass(EbxClassMetaAttribute classMeta, object obj, Type objType, long startOffset)
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
                    int index = ReadInt();
                    EbxArray array = arrays[index];

                    long arrayPos = Position;
                    Position = arraysOffset + array.Offset;

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

        internal object ReadField(EbxFieldType type, Type baseType, bool dontRefCount = false)
        {
            switch (type)
            {
                case EbxFieldType.Boolean: return ReadBoolean();
                case EbxFieldType.Int8: return (sbyte)ReadByte();
                case EbxFieldType.UInt8: return ReadByte();
                case EbxFieldType.Int16: return ReadShort();
                case EbxFieldType.UInt16: return ReadUShort();
                case EbxFieldType.Int32: return ReadInt();
                case EbxFieldType.UInt32: return ReadUInt();
                case EbxFieldType.Int64: return ReadLong();
                case EbxFieldType.UInt64: return ReadULong();
                case EbxFieldType.Float32: return ReadFloat();
                case EbxFieldType.Float64: return ReadDouble();
                case EbxFieldType.Guid: return ReadGuid();
                case EbxFieldType.ResourceRef: return ReadResourceRef();
                case EbxFieldType.Sha1: return ReadSha1();
                case EbxFieldType.String: return ReadSizedString(32);
                case EbxFieldType.CString: return ReadCString(ReadUInt());
                case EbxFieldType.FileRef: return ReadFileRef();
                case EbxFieldType.TypeRef: return ReadTypeRef();
                case EbxFieldType.BoxedValueRef: return ReadBoxedValueRef();
                case EbxFieldType.Struct:
                    object structObj = TypeLibrary.CreateObject(baseType);
                    EbxClassMetaAttribute classMeta = structObj.GetType().GetCustomAttribute<EbxClassMetaAttribute>();
                    Pad(classMeta.Alignment);
                    ReadClass(classMeta, structObj, structObj.GetType(), Position);
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
    }
}
