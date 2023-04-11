using System;

namespace Frosty.Sdk.Attributes;


[AttributeUsage(AttributeTargets.Assembly)]
public class SdkVersionAttribute : Attribute
{
    private uint Head;

    public SdkVersionAttribute(uint inHead)
    {
        Head = inHead;
    }
}