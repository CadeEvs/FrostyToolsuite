using System;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
public class ArrayHashAttribute : Attribute
{
    public uint Hash { get; set; }

    public ArrayHashAttribute(uint inHash)
    {
        Hash = inHash;
    }
}