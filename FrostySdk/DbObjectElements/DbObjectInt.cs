using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectInt : DbObject
{
    private int m_value;

    protected internal DbObjectInt(Type inType)
        : base(inType)
    {
    }

    public DbObjectInt(int inValue)
        : base(Type.Int | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectInt(string inName, int inValue)
        : base(Type.Int, inName)
    {
        m_value = inValue;
    }

    public override int AsInt()
    {
        return m_value;
    }

    public override uint AsUInt()
    {
        return (uint)m_value;
    }

    public override long AsLong()
    {
        return m_value;
    }

    public override ulong AsULong()
    {
        return (uint)m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteInt32(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadInt32();
    }
}