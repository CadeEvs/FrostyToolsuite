using System;

namespace Frosty.Sdk.Attributes;

/// <summary>
/// Specifies that this property is hidden from the property grid
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IsHiddenAttribute : Attribute
{
}