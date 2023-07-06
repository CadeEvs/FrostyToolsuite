using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectSha1 : DbObject
{
    private Sha1 m_value;

    protected internal DbObjectSha1(Type inType)
        : base(inType)
    {
    }

    public DbObjectSha1(Sha1 inValue)
        : base(Type.Sha1 | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectSha1(string inName, Sha1 inValue)
        : base(Type.Sha1, inName)
    {
        m_value = inValue;
    }

    public override Sha1 AsSha1()
    {
        return m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.WriteSha1(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        m_value = stream.ReadSha1();
    }
}