using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectLong : DbObject
{
    private long m_value;

    protected internal DbObjectLong(Type inType)
        : base(inType)
    {
    }

    public DbObjectLong(long inValue)
        : base(Type.Long | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectLong(string inName, long inValue)
        : base(Type.Long, inName)
    {
        m_value = inValue;
    }

    public override int AsInt()
    {
        return (int)m_value;
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
        return (ulong)m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteInt64(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadInt64();
    }
}