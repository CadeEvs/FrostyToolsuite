using System;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum, Inherited = false)]
public class ArrayHashAttribute : Attribute
{
    public uint Hash { get; set; }

    public ArrayHashAttribute(uint inHash)
    {
        Hash = inHash;
    }
}