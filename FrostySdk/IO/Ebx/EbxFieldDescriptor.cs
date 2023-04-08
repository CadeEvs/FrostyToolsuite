using Frosty.Sdk.Sdk;

namespace Frosty.Sdk.IO.Ebx;

public struct EbxFieldDescriptor
{
    public string Name;
    public uint NameHash;
    public TypeFlags Flags;
    public ushort TypeDescriptorRef;
    public uint DataOffset;
    public uint SecondOffset;
}