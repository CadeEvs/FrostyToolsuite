using System;
using Frosty.Sdk.Sdk;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EbxArrayMetaAttribute : Attribute
{
    public TypeFlags Flags { get; set; }
    
    public EbxArrayMetaAttribute(ushort flags)
    {
        Flags = flags;
    }
}