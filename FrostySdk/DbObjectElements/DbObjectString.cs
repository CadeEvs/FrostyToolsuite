using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectString : DbObject
{
    private string m_value;

    protected internal DbObjectString(Type inType)
        : base(inType)
    {
        m_value = string.Empty;
    }

    public DbObjectString(string inValue)
        : base(Type.String | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectString(string inName, string inValue)
        : base(Type.String, inName)
    {
        m_value = inValue;
    }

    public override string AsString()
    {
        return m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteSizedString(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadSizedString();
    }
}