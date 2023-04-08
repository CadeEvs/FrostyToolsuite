using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.Ebx;
using Frosty.Sdk.IO.Ebx;
using Frosty.Sdk.Sdk;

namespace Frosty.Sdk.IO;


public class EbxReader : DataStream
{
    public static EbxReader CreateProjectReader(Stream inStream)
    {
        return new EbxReader(inStream);
    }

    public static EbxReader CreateReader(Stream inStream)
    {
        return ProfilesLibrary.EbxVersion == 6 ? new EbxReaderRiff(inStream) : new EbxReader(inStream);
    }

    public Guid FileGuid => fileGuid;
    public virtual string RootType => GetType(m_typeResolver.ResolveType(instances[0].TypeDescriptorRef)).Name;
    public List<Guid> Dependencies => dependencies;
    public bool IsValid => isValid;

    public List<EbxFieldDescriptor> FieldTypes => m_fieldDescriptors;
    public List<EbxTypeDescriptor> ClassTypes => m_typeDescriptors;

    protected readonly List<EbxFieldDescriptor> m_fieldDescriptors = new();
    protected readonly List<EbxTypeDescriptor> m_typeDescriptors = new();
    protected List<EbxInstance> instances = new();
    protected List<EbxArray> arrays = new();
    protected List<EbxBoxedValue> boxedValues = new();
    internal List<EbxImportReference> imports = new();
    protected List<Guid> dependencies = new();
    protected List<object> objects = new();

    protected List<BoxedValueRef> boxedValueRefs = new();

    protected Guid fileGuid;
    protected long arraysOffset;
    internal long stringsOffset;
    protected long stringsAndDataLen;
    protected uint guidCount;
    protected ushort instanceCount;
    protected uint exportedCount;
    protected ushort uniqueClassCount;
    protected ushort classTypeCount;
    protected ushort fieldTypeCount;
    protected ushort typeNamesLen;
    protected uint stringsLen;
    protected uint arrayCount;
    protected uint dataLen;
    protected uint boxedValuesCount;
    protected long boxedValuesOffset;

    internal EbxVersion magic;
    protected bool isValid = false;

    internal EbxTypeResolver m_typeResolver;

    internal EbxReader(Stream inStream, bool passthru)
        : base(inStream)
    {
    }

    public EbxReader(Stream inStream)
        : base(inStream)
    {
        magic = (EbxVersion)ReadUInt32();
        if (magic != EbxVersion.Version2 && magic != EbxVersion.Version4)
        {
            throw new InvalidDataException("magic");
        }

        stringsOffset = ReadUInt32();
        stringsAndDataLen = ReadUInt32();
        guidCount = ReadUInt32();
        instanceCount = ReadUInt16();
        exportedCount = ReadUInt16();
        uniqueClassCount = ReadUInt16();
        classTypeCount = ReadUInt16();
        fieldTypeCount = ReadUInt16();
        typeNamesLen = ReadUInt16();

        stringsLen = ReadUInt32();
        arrayCount = ReadUInt32();
        dataLen = ReadUInt32();

        arraysOffset = stringsOffset + stringsLen + dataLen;

        fileGuid = ReadGuid();

        if (magic == EbxVersion.Version4)
        {
            boxedValuesCount = ReadUInt32();
            boxedValuesOffset = ReadUInt32();
            boxedValuesOffset += stringsOffset + stringsLen;
        }
        else
        {
            Pad(16);
        }

        for (int i = 0; i < guidCount; i++)
        {
            EbxImportReference import = new()
            {
                FileGuid = ReadGuid(),
                ClassGuid = ReadGuid()
            };

            imports.Add(import);
            if (!dependencies.Contains(import.FileGuid))
                dependencies.Add(import.FileGuid);
        }

        Dictionary<int, string> typeNames = new();

        long typeNamesOffset = Position;
        while (Position - typeNamesOffset < typeNamesLen)
        {
            string typeName = ReadNullTerminatedString();
            int hash = Utils.Utils.HashString(typeName);

            if (!typeNames.ContainsKey(hash))
                typeNames.Add(hash, typeName);
        }

        for (int i = 0; i < fieldTypeCount; i++)
        {
            EbxFieldDescriptor fieldDescriptor = new()
            {
                NameHash = ReadUInt32(),
                Flags = ReadUInt16(),
                TypeDescriptorRef = ReadUInt16(),
                DataOffset = ReadUInt32(),
                SecondOffset = ReadUInt32(),
            };

            fieldDescriptor.Name = typeNames.TryGetValue((int)fieldDescriptor.NameHash, out string? value) ? value : string.Empty;

            m_fieldDescriptors.Add(fieldDescriptor);
        }

        for (int i = 0; i < classTypeCount; i++)
        {
            EbxTypeDescriptor typeDescriptor = new()
            {
                NameHash = ReadUInt32(),
                FieldIndex = ReadInt32(),
                FieldCount = ReadByte(),
                Alignment = ReadByte(),
                Flags = ReadUInt16(),
                Size = ReadUInt16(),
                SecondSize = ReadUInt16()
            };

            typeDescriptor.Name = typeNames.TryGetValue((int)typeDescriptor.NameHash, out string? value) ? value : string.Empty;

            m_typeDescriptors.Add(typeDescriptor);
        }

        m_typeResolver = new EbxTypeResolver(m_typeDescriptors, m_fieldDescriptors);
        
        for (int i = 0; i < instanceCount; i++)
        {
            EbxInstance inst = new()
            {
                TypeDescriptorRef = ReadUInt16(),
                Count = ReadUInt16()
            };

            if (i < exportedCount)
            {
                inst.IsExported = true;
            }

            instances.Add(inst);
        }

        Pad(16);

        for (int i = 0; i < arrayCount; i++)
        {
            EbxArray array = new()
            {
                Offset = ReadUInt32(),
                Count = ReadUInt32(),
                TypeDescriptorRef = ReadInt32()
            };

            arrays.Add(array);
        }

        Pad(16);

        for (int i = 0; i < boxedValuesCount; i++)
        {
            EbxBoxedValue boxedValue = new()
            {
                Offset = ReadUInt32(),
                TypeDescriptorRef = ReadUInt16(),
                Type = ReadUInt16()
            };

            boxedValues.Add(boxedValue);
        }

        Position = stringsOffset + stringsLen;
        isValid = true;
    }

    public T ReadAsset<T>() where T : EbxAsset, new()
    {
        T asset = new();
        InternalReadObjects();

        asset.fileGuid = fileGuid;
        asset.objects = objects;
        asset.dependencies = dependencies;
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

    protected virtual void InternalReadObjects()
    {
        foreach (EbxInstance inst in instances)
        {
            EbxTypeDescriptor typeDescriptor = m_typeResolver.ResolveType(inst.TypeDescriptorRef);
            for (int i = 0; i < inst.Count; i++)
            {
                objects.Add(CreateObject(typeDescriptor));
            }
        }

        int typeId = 0;
        int index = 0;

        foreach (EbxInstance inst in instances)
        {
            EbxTypeDescriptor typeDescriptor = m_typeResolver.ResolveType(inst.TypeDescriptorRef);
            for (int i = 0; i < inst.Count; i++)
            {
                Pad(typeDescriptor.GetAlignment());

                Guid instanceGuid = Guid.Empty;
                if (inst.IsExported)
                {
                    instanceGuid = ReadGuid();
                }

                if (typeDescriptor.Alignment != 0x04)
                {
                    Position += 8;
                }

                dynamic obj = objects[typeId++];
                obj.SetInstanceGuid(new AssetClassGuid(instanceGuid, index++));

                ReadClass(typeDescriptor, obj, Position - 8);
            }
        }
    }

    protected virtual void ReadClass(EbxTypeDescriptor classType, object? obj, long startOffset)
    {
        if (obj == null)
        {
            Position += classType.Size;
            Pad(classType.GetAlignment());
            return;
        }
        Type objType = obj.GetType();

        for (int j = 0; j < classType.GetFieldCount(); j++)
        {
            EbxFieldDescriptor fieldType = m_typeResolver.ResolveField(classType.FieldIndex + j);
            PropertyInfo? fieldProp = GetProperty(objType, fieldType);

            Position = startOffset + fieldType.DataOffset;

            if (fieldType.Flags.GetTypeEnum() == TypeFlags.TypeEnum.Inherited)
            {
                // read super class first
                ReadClass(m_typeResolver.ResolveType(classType, fieldType.TypeDescriptorRef), obj, startOffset);
            }
            else
            {
                if (fieldType.Flags.GetTypeEnum() == TypeFlags.TypeEnum.Array)
                {
                    EbxTypeDescriptor arrayType = m_typeResolver.ResolveType(classType, fieldType.TypeDescriptorRef);

                    int index = ReadInt32();
                    EbxArray array = arrays[index];

                    long arrayPos = Position;
                    Position = arraysOffset + array.Offset;

                    for (int i = 0; i < array.Count; i++)
                    {
                        EbxFieldDescriptor arrayField = m_typeResolver.ResolveField(arrayType.FieldIndex);
                        object value = ReadField(arrayType, arrayField.Flags.GetTypeEnum(), arrayField.TypeDescriptorRef);

                        try
                        {
                            fieldProp?.GetValue(obj)?.GetType().GetMethod("Add")?.Invoke(fieldProp.GetValue(obj), new[] { value });
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    Position = arrayPos;
                }
                else
                {
                    object value = ReadField(classType, fieldType.Flags.GetTypeEnum(), fieldType.TypeDescriptorRef);

                    try
                    {
                        fieldProp?.SetValue(obj, value);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        Pad(classType.GetAlignment());
    }

    protected object ReadField(EbxTypeDescriptor? parentClass, TypeFlags.TypeEnum fieldType, ushort fieldClassRef)
    {
        switch (fieldType)
        {
            case TypeFlags.TypeEnum.Boolean:
                return ReadBoolean();
            case TypeFlags.TypeEnum.Int8:
                return (sbyte)ReadByte();
            case TypeFlags.TypeEnum.UInt8:
                return ReadByte();
            case TypeFlags.TypeEnum.Int16:
                return ReadInt16();
            case TypeFlags.TypeEnum.UInt16:
                return ReadUInt16();
            case TypeFlags.TypeEnum.Int32:
                return ReadInt32();
            case TypeFlags.TypeEnum.UInt32:
                return ReadUInt32();
            case TypeFlags.TypeEnum.Int64:
                return ReadInt64();
            case TypeFlags.TypeEnum.UInt64:
                return ReadUInt64();
            case TypeFlags.TypeEnum.Float32:
                return ReadSingle();
            case TypeFlags.TypeEnum.Float64:
                return ReadDouble();
            case TypeFlags.TypeEnum.Guid:
                return ReadGuid();
            case TypeFlags.TypeEnum.ResourceRef:
                return ReadResourceRef();
            case TypeFlags.TypeEnum.Sha1:
                return ReadSha1();
            case TypeFlags.TypeEnum.String:
                return ReadFixedSizedString(32);
            case TypeFlags.TypeEnum.CString:
                return ReadCString(ReadUInt32());
            case TypeFlags.TypeEnum.FileRef:
                return ReadFileRef();
            case TypeFlags.TypeEnum.Delegate:
            case TypeFlags.TypeEnum.TypeRef:
                return ReadTypeRef();
            case TypeFlags.TypeEnum.BoxedValueRef:
                return ReadBoxedValueRef();
            case TypeFlags.TypeEnum.Struct:
                EbxTypeDescriptor structType = parentClass.HasValue ? m_typeResolver.ResolveType(parentClass.Value, fieldClassRef) : m_typeResolver.ResolveType(fieldClassRef);
                Pad(structType.GetAlignment());
                object structObj = CreateObject(structType);
                ReadClass(structType, structObj, Position);
                return structObj;
            case TypeFlags.TypeEnum.Enum:
                return ReadInt32();
            case TypeFlags.TypeEnum.Class:
                return ReadPointerRef();
            case TypeFlags.TypeEnum.DbObject:
                throw new InvalidDataException("DbObject");
            default:
                throw new InvalidDataException("Unknown");
        }
    }

    protected virtual PropertyInfo? GetProperty(Type objType, EbxFieldDescriptor field)
    {
        return objType.GetProperties().FirstOrDefault((pi) => pi.GetCustomAttribute<NameHashAttribute>()?.Hash == field.NameHash);
    }

    protected virtual object CreateObject(EbxTypeDescriptor typeDescriptor) => TypeLibrary.CreateObject(typeDescriptor.NameHash)!;

    protected virtual Type GetType(EbxTypeDescriptor classType) => TypeLibrary.GetType(classType.NameHash)!;

    protected Type GetTypeFromEbxField(EbxFieldDescriptor fieldType)
    {
        switch (fieldType.Flags.GetTypeEnum())
        {
            case TypeFlags.TypeEnum.Struct: return GetType(m_typeResolver.ResolveType(fieldType.TypeDescriptorRef));
            case TypeFlags.TypeEnum.String: return typeof(string);
            case TypeFlags.TypeEnum.Int8: return typeof(sbyte);
            case TypeFlags.TypeEnum.UInt8: return typeof(byte);
            case TypeFlags.TypeEnum.Boolean: return typeof(bool);
            case TypeFlags.TypeEnum.UInt16: return typeof(ushort);
            case TypeFlags.TypeEnum.Int16: return typeof(short);
            case TypeFlags.TypeEnum.UInt32: return typeof(uint);
            case TypeFlags.TypeEnum.Int32: return typeof(int);
            case TypeFlags.TypeEnum.UInt64: return typeof(ulong);
            case TypeFlags.TypeEnum.Int64: return typeof(long);
            case TypeFlags.TypeEnum.Float32: return typeof(float);
            case TypeFlags.TypeEnum.Float64: return typeof(double);
            case TypeFlags.TypeEnum.Class: return typeof(PointerRef);
            case TypeFlags.TypeEnum.Guid: return typeof(Guid);
            case TypeFlags.TypeEnum.Sha1: return typeof(Sha1);
            case TypeFlags.TypeEnum.CString: return typeof(CString);
            case TypeFlags.TypeEnum.ResourceRef: return typeof(ResourceRef);
            case TypeFlags.TypeEnum.FileRef: return typeof(FileRef);
            case TypeFlags.TypeEnum.TypeRef: return typeof(TypeRef);
            case TypeFlags.TypeEnum.BoxedValueRef: return typeof(ulong);
            case TypeFlags.TypeEnum.Array:
                EbxTypeDescriptor arrayType = m_typeDescriptors[fieldType.TypeDescriptorRef];
                return typeof(List<>).MakeGenericType(GetTypeFromEbxField(m_fieldDescriptors[arrayType.FieldIndex]));
            case TypeFlags.TypeEnum.Enum:
                return GetType(m_typeResolver.ResolveType(fieldType.TypeDescriptorRef));

            default:
                throw new NotImplementedException();
        }
    }

    protected virtual string ReadString(uint offset)
    {
        if (offset == 0xFFFFFFFF)
            return string.Empty;

        long pos = Position;
        Position = stringsOffset + offset;

        string retStr = ReadNullTerminatedString();
        Position = pos;

        return retStr;
    }

    protected CString ReadCString(uint offset) => new(ReadString(offset));

    protected ResourceRef ReadResourceRef() => new(ReadUInt64());

    protected FileRef ReadFileRef()
    {
        uint index = ReadUInt32();
        Position += 4;

        return new FileRef(ReadString(index));
    }

    protected virtual PointerRef ReadPointerRef()
    {
        uint index = ReadUInt32();
        
        if ((index >> 0x1F) == 1)
        {
            EbxImportReference import = imports[(int)(index & 0x7FFFFFFF)];

            return new PointerRef(import);
        }

        if (index == 0)
        {
            return new PointerRef();
        }

        return new PointerRef(objects[(int)(index - 1)]);
    }

    protected virtual TypeRef ReadTypeRef()
    {
        string str = ReadString(ReadUInt32());
        Position += 4;

        if (string.IsNullOrEmpty(str))
            return new TypeRef();

        if (Guid.TryParse(str, out Guid guid))
        {
            if (guid != Guid.Empty)
            {
                return new TypeRef(guid);
            }
        }

        return new TypeRef(str);
    }

    protected virtual BoxedValueRef ReadBoxedValueRef()
    {
        int index = ReadInt32();
        Position += 12;

        if (index == -1)
            return new BoxedValueRef();

        EbxBoxedValue boxedValue = boxedValues[index];
        TypeFlags.TypeEnum subType = TypeFlags.TypeEnum.Inherited;

        long pos = Position;
        Position = boxedValuesOffset + boxedValue.Offset;

        object value;
        if ((TypeFlags.TypeEnum)boxedValue.Type == TypeFlags.TypeEnum.Array)
        {
            EbxTypeDescriptor arrayType = m_typeResolver.ResolveType(boxedValue.TypeDescriptorRef);
            EbxFieldDescriptor arrayField = m_typeResolver.ResolveField(arrayType.FieldIndex);

            value = Activator.CreateInstance(typeof(List<>).MakeGenericType(GetTypeFromEbxField(arrayField)))!;
            index = ReadInt32();
            EbxArray array = arrays[index];

            long arrayPos = Position;
            Position = arraysOffset + array.Offset;

            for (int i = 0; i < array.Count; i++)
            {
                object subValue = ReadField(arrayType, arrayField.Flags.GetTypeEnum(), arrayField.TypeDescriptorRef);
                value.GetType()?.GetMethod("Add")?.Invoke(value, new object[] { subValue });
            }

            subType = arrayField.Flags.GetTypeEnum();
            Position = arrayPos;
        }
        else
        {
            value = ReadField(null, (TypeFlags.TypeEnum)boxedValue.Type, boxedValue.TypeDescriptorRef);
            if ((TypeFlags.TypeEnum)boxedValue.Type == TypeFlags.TypeEnum.Enum)
            {
                object tmpValue = value;
                EbxTypeDescriptor enumClass = m_typeResolver.ResolveType(boxedValue.TypeDescriptorRef);
                value = Enum.Parse(GetType(enumClass), tmpValue.ToString()!);
            }
        }
        Position = pos;

        return new BoxedValueRef(value, (TypeFlags.TypeEnum)boxedValue.Type, subType);
    }
}