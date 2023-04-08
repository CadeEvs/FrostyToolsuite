using System;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.All, Inherited = false)]
public class NameHashAttribute : Attribute
{
    public uint Hash { get; }
    public NameHashAttribute(uint inHash) { Hash = inHash; }
}