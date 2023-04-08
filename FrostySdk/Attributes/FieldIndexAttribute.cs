using System;

namespace Frosty.Sdk.Attributes;

/// <summary>
/// Specifies the fields index, which may differ from its offset
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class FieldIndexAttribute : Attribute
{
    public int Index { get; set; }
    public FieldIndexAttribute(int inIndex)
    {
        Index = inIndex;
    }
}