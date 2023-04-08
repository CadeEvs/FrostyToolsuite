using System;

namespace Frosty.Sdk.Attributes;

/// <summary>
/// Specifies that this property should not be saved
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IsTransientAttribute : Attribute
{
}