using System;

namespace Frosty.Sdk.Attributes;

/// <summary>
/// Specifies the category this property will be displayed under if the class is a top level class of the property grid
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CategoryAttribute : Attribute
{
    public string Name { get; set; }
    public CategoryAttribute(string name)
    {
        Name = name;
    }
}