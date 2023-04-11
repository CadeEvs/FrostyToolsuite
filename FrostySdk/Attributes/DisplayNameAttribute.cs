using System;

namespace Frosty.Sdk.Attributes;

/// <summary>
/// Overrides the display name of the property/class in the Property Grid
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Delegate | AttributeTargets.Enum | AttributeTargets.Method, Inherited = false)]
public class DisplayNameAttribute : Attribute
{
    public string Name { get; set; }
    
    public DisplayNameAttribute(string name)
    {
        Name = name;
    }
}