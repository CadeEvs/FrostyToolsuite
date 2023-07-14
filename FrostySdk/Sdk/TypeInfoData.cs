using System;
using System.Text;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.IO;
using Frosty.Sdk.Sdk.TypeInfoDatas;
using Frosty.Sdk.Sdk.TypeInfos;

namespace Frosty.Sdk.Sdk;

internal class TypeInfoData
{
    protected string m_name = string.Empty;
    protected uint m_nameHash;
    protected TypeFlags m_flags;
    protected ushort m_size;
    protected Guid m_guid;
    protected string m_nameSpace = string.Empty;
    protected long p_arrayInfo;
    protected byte m_alignment;
    protected ushort m_fieldCount;
    protected uint m_signature;

    public static TypeInfoData ReadTypeInfoData(MemoryReader reader)
    {
        TypeInfoData retVal;
        string name = string.Empty;
        if (!ProfilesLibrary.HasStrippedTypeNames)
        {
            name = reader.ReadNullTerminatedString();
        }

        uint nameHash = 0;
        if (TypeInfo.Version > 4)
        {
            nameHash = reader.ReadUInt();
        }

        TypeFlags flags = reader.ReadUShort();

        switch (flags.GetTypeEnum())
        {
            case TypeFlags.TypeEnum.Struct:
                retVal = new StructInfoData();
                break;
            case TypeFlags.TypeEnum.Class:
                retVal = new ClassInfoData();
                break;
            case TypeFlags.TypeEnum.Array:
                retVal = new ArrayInfoData();
                break;
            case TypeFlags.TypeEnum.Enum:
                retVal = new EnumInfoData();
                break;
            case TypeFlags.TypeEnum.Function:
                retVal = new FunctionInfoData();
                break;
            case TypeFlags.TypeEnum.Delegate:
                retVal = new DelegateInfoData();
                break;
            case TypeFlags.TypeEnum.String:
            case TypeFlags.TypeEnum.CString:
            case TypeFlags.TypeEnum.FileRef:
            case TypeFlags.TypeEnum.Boolean:
            case TypeFlags.TypeEnum.Int8:
            case TypeFlags.TypeEnum.UInt8:
            case TypeFlags.TypeEnum.Int16:
            case TypeFlags.TypeEnum.UInt16:
            case TypeFlags.TypeEnum.Int32:
            case TypeFlags.TypeEnum.UInt32:
            case TypeFlags.TypeEnum.Int64:
            case TypeFlags.TypeEnum.UInt64:
            case TypeFlags.TypeEnum.Float32:
            case TypeFlags.TypeEnum.Float64:
            case TypeFlags.TypeEnum.Guid:
            case TypeFlags.TypeEnum.Sha1:
            case TypeFlags.TypeEnum.ResourceRef:
            case TypeFlags.TypeEnum.TypeRef:
            case TypeFlags.TypeEnum.BoxedValueRef:
                retVal = new PrimitiveInfoData();
                break;

            default:
                retVal = new TypeInfoData();
                Console.WriteLine($"Not implemented type: {flags.GetTypeEnum()}");
                break;
        }

        if (!ProfilesLibrary.HasStrippedTypeNames)
        {
            if (Strings.ClassHashes.TryGetValue(nameHash, out string? hash))
            {
                name = hash;
            }
            else if (Strings.StringHashes.TryGetValue(nameHash, out hash))
            {
                name = hash;
            }
        }

        retVal.m_name = name;
        retVal.m_nameHash = nameHash;
        retVal.m_flags = flags;

        retVal.Read(reader);

        return retVal;
    }

    public virtual void Read(MemoryReader reader)
    {
        m_size = reader.ReadUShort();

        if (TypeInfo.Version > 4)
        {
            m_guid = reader.ReadGuid();
        }

        long nameSpaceOffset = reader.ReadLong();
        long curPos = reader.Position;
        reader.Position = nameSpaceOffset;
        m_nameSpace = reader.ReadNullTerminatedString();
        reader.Position = curPos;

        if (TypeInfo.Version > 2)
        {
            p_arrayInfo = reader.ReadLong();
        }

        m_alignment = reader.ReadByte();
        m_fieldCount = TypeInfo.Version > 1 ? reader.ReadUShort() : reader.ReadByte();
        if (TypeInfo.Version > 5)
        {
            m_signature = reader.ReadUInt();
        }
    }

    public void SetGuid(Guid guid) => m_guid = guid;

    public string GetName() => m_name;

    public TypeFlags GetFlags() => m_flags;

    public virtual void CreateType(StringBuilder sb)
    {
        sb.AppendLine($"[{nameof(EbxTypeMetaAttribute)}({(ushort)m_flags}, {m_alignment}, {m_size}, \"{m_nameSpace}\")]");

        sb.AppendLine($"[{nameof(DisplayNameAttribute)}(\"{m_name}\")]");
        
        if (!m_guid.Equals(Guid.Empty))
        {
            sb.AppendLine($"[{nameof(GuidAttribute)}(\"{m_guid}\")]");
        }
        if (m_nameHash != 0)
        {
            sb.AppendLine($"[{nameof(NameHashAttribute)}({m_nameHash})]");
        }
        if (m_signature != 0)
        {
            sb.AppendLine($"[{nameof(SignatureAttribute)}({m_signature})]");
        }

        if (TypeInfo.TypeInfoMapping.TryGetValue(p_arrayInfo, out TypeInfo? value))
        {
            ArrayInfo arrayInfo = (value as ArrayInfo)!;
            arrayInfo.CreateType(sb);
        }
    }

    public string CleanUpName() => CleanUpString(m_name);

    public string CleanUpString(string name)
    {
        if (name == "char")
        {
            return "Char";
        }

        if (name.Contains("::"))
        {
            return name[(name.IndexOf("::", StringComparison.Ordinal) + 2)..];
        }
        return name.Replace(':', '_').Replace("<", "_").Replace(">", "_");
    }
}