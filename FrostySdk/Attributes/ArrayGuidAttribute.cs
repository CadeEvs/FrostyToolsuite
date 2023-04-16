using System;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
public class ArrayGuidAttribute : Attribute
{
    public Guid Guid { get; }
    
    public ArrayGuidAttribute(string guid)
    {
        Guid = Guid.Parse(guid);
    }
}