using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectBool : DbObject
{
    private bool m_value;

    protected internal DbObjectBool(Type inType)
        : base(inType)
    {
    }

    public DbObjectBool(bool inValue)
        : base(Type.Boolean | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectBool(string inName, bool inValue)
        : base(Type.Boolean, inName)
    {
        m_value = inValue;
    }

    public override bool AsBoolean()
    {
        return m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteByte((byte)(m_value ? 1 : 0));
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadByte() != 0;
    }
}