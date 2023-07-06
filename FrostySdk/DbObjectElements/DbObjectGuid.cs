using System;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectGuid : DbObject
{
    private Guid m_value;

    protected internal DbObjectGuid(Type inType)
        : base(inType)
    {
    }

    public DbObjectGuid(Guid inValue)
        : base(Type.Guid | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectGuid(string inName, Guid inValue)
        : base(Type.Guid, inName)
    {
        m_value = inValue;
    }

    public override Guid AsGuid()
    {
        return m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteGuid(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadGuid();
    }
}