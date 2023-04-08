using System;

namespace Frosty.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Struct)]
public class SignatureAttribute : Attribute
{
    public uint Signature { get; }

    public SignatureAttribute(uint inSignature)
    {
        Signature = inSignature;
    }
}