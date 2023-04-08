using System;

namespace Frosty.Sdk.Attributes;


/// <summary>
/// Specifies that this property is read only
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IsReadOnlyAttribute : Attribute
{
}