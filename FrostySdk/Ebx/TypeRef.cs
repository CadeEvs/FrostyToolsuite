using System;

namespace Frosty.Sdk.Ebx;

public class TypeRef
{
    public string Name => m_typeName;
    public Guid Guid => m_typeGuid;
    private readonly Guid m_typeGuid;
    private readonly string m_typeName;

    public TypeRef()
    {
        m_typeName = string.Empty;
    }

    public TypeRef(string value)
    {
        m_typeName = value;
    }

    public TypeRef(Guid guid)
    {
        m_typeGuid = guid;
        m_typeName = TypeLibrary.GetType(guid)?.Name ?? m_typeGuid.ToString();
    }

    public Type GetReferencedType()
    {
        // should be a primitive type if the GUID is empty
        if (m_typeGuid == Guid.Empty)
        {
            Type? refType = TypeLibrary.GetType(m_typeName);
            if (refType == null)
            {
                throw new Exception($"Could not find the type {m_typeName}");
            }
            return refType;
        }
        else
        {

            Type? refType = TypeLibrary.GetType(m_typeGuid);
            if (refType == null)
            {
                throw new Exception($"Could not find the type {m_typeName}");
            }
            return refType;
        }
    }

    public static implicit operator string(TypeRef value)
    {
        return value.m_typeGuid != Guid.Empty ? value.m_typeGuid.ToString().ToUpper() : value.m_typeName;
    }

    public static implicit operator TypeRef(string value) => new(value);

    public static implicit operator TypeRef(Guid guid) => new(guid);

    public bool IsNull() => string.IsNullOrEmpty(m_typeName);

    public override string ToString() => $"TypeRef '{(IsNull() ? "(null)" : m_typeName)}'";
}