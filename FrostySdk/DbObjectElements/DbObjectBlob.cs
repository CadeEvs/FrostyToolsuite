using System;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectBlob : DbObject
{
    private byte[] m_value;

    protected internal DbObjectBlob(Type inType)
        : base(inType)
    {
        m_value = Array.Empty<byte>();
    }

    public DbObjectBlob(byte[] inValue)
        : base(Type.Blob | Type.Anonymous)
    {
        m_value = inValue;
    }
    
    public DbObjectBlob(string inName, byte[] inValue)
        : base(Type.Blob, inName)
    {
        m_value = inValue;
    }

    public override byte[] AsBlob()
    {
        return m_value;
    }

    protected override void InternalSerialize(DataStream stream)
    {
        stream.Write7BitEncodedInt32(m_value.Length);
        stream.Write(m_value);
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        int length = stream.Read7BitEncodedInt32();
        m_value = new byte[length];
        stream.ReadExactly(m_value);
    }
}