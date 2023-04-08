using System;
using Frosty.Sdk.Sdk;

namespace Frosty.Sdk.Attributes;

/// <summary>
/// Mandatory attribute for all Ebx based fields
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EbxFieldMetaAttribute : Attribute
{
    public TypeFlags Flags { get; set; }
    public uint Offset { get; set; }
    public Type? BaseType { get; set; }

    public EbxFieldMetaAttribute(TypeFlags flags, uint offset, Type? baseType)
    {
        Flags = flags;
        Offset = offset;
        BaseType = baseType;
    }

    public EbxFieldMetaAttribute(TypeFlags.TypeEnum type, string baseType = "")
    {
        BaseType = typeof(object);
        if (baseType != "")
            BaseType = TypeLibrary.GetType(baseType);
        Flags = new TypeFlags(type);
    }
}