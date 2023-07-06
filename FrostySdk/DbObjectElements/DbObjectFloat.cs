using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectFloat : DbObject
{
    private float m_value;

    protected internal DbObjectFloat(Type inType)
        : base(inType)
    {
    }

    public DbObjectFloat(float inValue)
        : base(Type.Float | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectFloat(string inName, float inValue)
        : base(Type.Float, inName)
    {
        m_value = inValue;
    }

    public override float AsFloat()
    {
        return m_value;
    }

    public override double AsDouble()
    {
        return m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteSingle(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadSingle();
    }
}