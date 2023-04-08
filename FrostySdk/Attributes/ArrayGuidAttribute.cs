using System;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, Inherited = false)]
public class ArrayGuidAttribute : Attribute
{
    public Guid Guid { get; }
    
    public ArrayGuidAttribute(Guid guid)
    {
        Guid = guid;
    }
}