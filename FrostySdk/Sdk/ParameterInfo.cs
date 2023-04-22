using Frosty.Sdk.IO;

namespace Frosty.Sdk.Sdk;

internal class ParameterInfo
{
    public string GetName() => m_name;
    public TypeInfo GetTypeInfo() => TypeInfo.TypeInfoMapping[p_typeInfo];
    public byte GetParameterType() => m_type;

    private string m_name = string.Empty;
    private long p_typeInfo;
    private byte m_type;
    private long p_defaultValue;

    public void Read(MemoryReader reader)
    {
        m_name = reader.ReadNullTerminatedString();
        p_typeInfo = reader.ReadLong();
        m_type = reader.ReadByte();
        p_defaultValue = reader.ReadLong();
    }

    public void ProcessDefaultValue()
    {

    }
}