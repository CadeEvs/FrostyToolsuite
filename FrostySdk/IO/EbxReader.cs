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

    public Guid FileGuid => m_fileGuid;
    public virtual string RootType => GetType(m_typeResolver.ResolveType(m_instances[0].TypeDescriptorRef)).Name;
    public HashSet<Guid> Dependencies => m_dependencies;
    public EbxImportReference[] Imports => m_imports;
    public bool IsValid => m_isValid;

    protected readonly EbxFieldDescriptor[] m_fieldDescriptors;
    protected readonly EbxTypeDescriptor[] m_typeDescriptors;
    protected readonly EbxInstance[] m_instances;
    protected readonly EbxArray[] m_arrays;
    protected readonly EbxBoxedValue[] m_boxedValues;
    protected readonly EbxImportReference[] m_imports;
    protected HashSet<Guid> m_dependencies = new();
    protected List<object> m_objects = new();

    protected Guid m_fileGuid;
    protected long m_arraysOffset;
    internal long m_stringsOffset;
    protected long m_boxedValuesOffset;

    internal EbxVersion m_magic;
    protected bool m_isValid;

    private readonly EbxTypeResolver m_typeResolver;

    public EbxReader(Stream inStream)
        : base(inStream)
    {
        m_magic = (EbxVersion)ReadUInt32();
        if (m_magic != EbxVersion.Version2 && m_magic != EbxVersion.Version4)
        {
            throw new InvalidDataException("magic");
        }

        m_stringsOffset = ReadUInt32();
        uint stringsAndDataLen = ReadUInt32();
        uint importCount = ReadUInt32();
        ushort instanceCount = ReadUInt16();
        ushort exportedCount = ReadUInt16();
        ushort uniqueTypeCount = ReadUInt16();
        ushort typeDescriptorCount = ReadUInt16();
        ushort fieldDescriptorCount = ReadUInt16();
        ushort typeNamesLen = ReadUInt16();

        uint stringsLen = ReadUInt32();
        uint arrayCount = ReadUInt32();
        uint dataLen = ReadUInt32();

        m_arraysOffset = m_stringsOffset + stringsLen + dataLen;

        m_fileGuid = ReadGuid();

        uint boxedValuesCount = 0;
        if (m_magic == EbxVersion.Version4)
        {
            boxedValuesCount = ReadUInt32();
            m_boxedValuesOffset = ReadUInt32();
            m_boxedValuesOffset += m_stringsOffset + stringsLen;
        }
        else
        {
            Pad(16);
        }

        m_imports = new EbxImportReference[importCount]; 
        for (int i = 0; i < importCount; i++)
        {
            EbxImportReference import = new()
            {
                FileGuid = ReadGuid(),
                ClassGuid = ReadGuid()
            };

            m_imports[i] = (import);
            m_dependencies.Add(import.FileGuid);
        }

        Dictionary<int, string> typeNames = new();

        long typeNamesOffset = Position;
        while (Position - typeNamesOffset < typeNamesLen)
        {
            string typeName = ReadNullTerminatedString();
            int hash = Utils.Utils.HashString(typeName);

            typeNames.TryAdd(hash, typeName);
        }

        m_fieldDescriptors = new EbxFieldDescriptor[fieldDescriptorCount];
        for (int i = 0; i < fieldDescriptorCount; i++)
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

            m_fieldDescriptors[i] = fieldDescriptor;
        }

        m_typeDescriptors = new EbxTypeDescriptor[typeDescriptorCount];
        for (int i = 0; i < typeDescriptorCount; i++)
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

            m_typeDescriptors[i] = typeDescriptor;
        }

        m_typeResolver = new EbxTypeResolver(m_typeDescriptors, m_fieldDescriptors);

        m_instances = new EbxInstance[instanceCount];
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

            m_instances[i] = inst;
        }

        Pad(16);

        m_arrays = new EbxArray[arrayCount];
        for (int i = 0; i < arrayCount; i++)
        {
            m_arrays[i] = new EbxArray
            {
                Offset = ReadUInt32(),
                Count = ReadUInt32(),
                TypeDescriptorRef = ReadInt32()
            };
        }

        Pad(16);

        m_boxedValues = new EbxBoxedValue[boxedValuesCount];
        for (int i = 0; i < boxedValuesCount; i++)
        {
            m_boxedValues[i] = new EbxBoxedValue
            {
                Offset = ReadUInt32(),
                TypeDescriptorRef = ReadUInt16(),
                Type = ReadUInt16()
            };
        }

        Position = m_stringsOffset + stringsLen;
        m_isValid = true;
    }

    public T ReadAsset<T>() where T : EbxAsset, new()
    {
        T asset = new();
        InternalReadObjects();

        asset.fileGuid = m_fileGuid;
        asset.objects = m_objects;
        asset.dependencies = m_dependencies;
        asset.OnLoadComplete();

        return asset;
    }

    public dynamic ReadObject()
    {
        InternalReadObjects();
        return m_objects[0];
    }

    public List<object> ReadObjects()
    {
        InternalReadObjects();
        return m_objects;
    }

    protected virtual void InternalReadObjects()
    {
        foreach (EbxInstance inst in m_instances)
        {
            EbxTypeDescriptor typeDescriptor = m_typeResolver.ResolveType(inst.TypeDescriptorRef);
            for (int i = 0; i < inst.Count; i++)
            {
                m_objects.Add(CreateObject(typeDescriptor));
            }
        }

        int typeId = 0;
        int index = 0;

        foreach (EbxInstance inst in m_instances)
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

                dynamic obj = m_objects[typeId++];
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
                    EbxArray array = m_arrays[index];

                    long arrayPos = Position;
                    Position = m_arraysOffset + array.Offset;

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
        {
            return string.Empty;
        }

        long pos = Position;
        Position = m_stringsOffset + offset;

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
            EbxImportReference import = m_imports[(int)(index & 0x7FFFFFFF)];

            return new PointerRef(import);
        }

        if (index == 0)
        {
            return new PointerRef();
        }
        
        

        return new PointerRef(m_objects[(int)(index - 1)]);
    }

    protected virtual TypeRef ReadTypeRef()
    {
        string str = ReadString(ReadUInt32());
        Position += 4;

        if (string.IsNullOrEmpty(str))
        {
            return new TypeRef();
        }

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
        {
            return new BoxedValueRef();
        }

        EbxBoxedValue boxedValue = m_boxedValues[index];
        TypeFlags.TypeEnum subType = TypeFlags.TypeEnum.Inherited;

        long pos = Position;
        Position = m_boxedValuesOffset + boxedValue.Offset;

        object value;
        if ((TypeFlags.TypeEnum)boxedValue.Type == TypeFlags.TypeEnum.Array)
        {
            EbxTypeDescriptor arrayType = m_typeResolver.ResolveType(boxedValue.TypeDescriptorRef);
            EbxFieldDescriptor arrayField = m_typeResolver.ResolveField(arrayType.FieldIndex);

            value = Activator.CreateInstance(typeof(List<>).MakeGenericType(GetTypeFromEbxField(arrayField)))!;
            index = ReadInt32();
            EbxArray array = m_arrays[index];

            long arrayPos = Position;
            Position = m_arraysOffset + array.Offset;

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