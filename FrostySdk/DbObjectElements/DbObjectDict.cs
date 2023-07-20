using System;
using System.Collections.Generic;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectDict : DbObject
{
    private readonly Dictionary<string, DbObject> m_items;

    protected internal DbObjectDict(Type inType)
        : base(inType)
    {
        m_items = new Dictionary<string, DbObject>();
    }

    protected internal  DbObjectDict(int inCapacity)
        : base(Type.Dict | Type.Anonymous)
    {
        m_items = new Dictionary<string, DbObject>(inCapacity);
    }
    
    protected internal DbObjectDict(string inName, int inCapacity)
        : base(Type.Dict, inName)
    {
        m_items = new Dictionary<string, DbObject>(inCapacity);
    }

    public override bool IsDict() => true;

    public override DbObjectDict AsDict()
    {
        return this;
    }

    public DbObjectDict AsDict(string name)
    {
        return m_items[name].AsDict();
    }

    public DbObjectDict? AsDict(string name, DbObjectDict? defaultValue)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsDict() : defaultValue;
    }

    public DbObjectList AsList(string name)
    {
        return m_items[name].AsList();
    }

    public DbObjectList? AsList(string name, DbObjectList? defaultValue)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsList() : defaultValue;
    }
    
    public bool AsBoolean(string name, bool defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsBoolean() : defaultValue;
    }

    public string AsString(string name, string defaultValue = "")
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsString() : defaultValue;
    }

    public int AsInt(string name, int defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsInt() : defaultValue;
    }

    public uint AsUInt(string name, uint defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsUInt() : defaultValue;
    }

    public long AsLong(string name, long defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsLong() : defaultValue;
    }

    public ulong AsULong(string name, ulong defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsULong() : defaultValue;
    }

    public float AsFloat(string name, float defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsFloat() : defaultValue;
    }

    public double AsDouble(string name, double defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsDouble() : defaultValue;
    }

    public Guid AsGuid(string name, Guid defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsGuid() : defaultValue;
    }

    public Sha1 AsSha1(string name, Sha1 defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsSha1() : defaultValue;
    }

    public byte[] AsBlob(string name, byte[]? defaultValue = default)
    {
        return m_items.TryGetValue(name, out DbObject? item) ? item.AsBlob() : defaultValue ?? Array.Empty<byte>();
    }

    public bool ContainsKey(string name) => m_items.ContainsKey(name);

    public void Add(string name, DbObjectDict value)
    {
        m_items.Add(name, value);
    }

    public void Add(string name, DbObjectList value)
    {
        m_items.Add(name, value);
    }
    
    public void Add(string name, bool value)
    {
        m_items.Add(name, new DbObjectBool(name, value));
    }

    public void Add(string name, string value)
    {
        m_items.Add(name, new DbObjectString(name, value));
    }

    public void Add(string name, int value)
    {
        m_items.Add(name, new DbObjectInt(name, value));
    }

    public void Add(string name, uint value)
    {
        m_items.Add(name, new DbObjectInt(name, (int)value));
    }

    public void Add(string name, long value)
    {
        m_items.Add(name, new DbObjectLong(name, value));
    }

    public void Add(string name, ulong value)
    {
        m_items.Add(name, new DbObjectLong(name, (long)value));
    }

    public void Add(string name, float value)
    {
        m_items.Add(name, new DbObjectFloat(name, value));
    }

    public void Add(string name, double value)
    {
        m_items.Add(name, new DbObjectDouble(name, value));
    }

    public void Add(string name, Guid value)
    {
        m_items.Add(name, new DbObjectGuid(name, value));
    }

    public void Add(string name, Sha1 value)
    {
        m_items.Add(name, new DbObjectSha1(name, value));
    }

    public void Add(string name, byte[] value)
    {
        m_items.Add(name, new DbObjectBlob(name, value));
    }
    
    protected override void InternalSerialize(DataStream stream)
    {
        long curPos = stream.Position;

        foreach (DbObject value in m_items.Values)
        {
            Serialize(stream, value);
        }
        
        // write terminator
        stream.WriteByte((byte)Type.Null);

        long size = stream.Position - curPos;
        stream.Position = curPos;
        stream.Write7BitEncodedInt64(size);
        stream.Position = curPos + size;
    }

    protected override void InternalDeserialize(DataStream stream)
    {
        stream.Read7BitEncodedInt64();
        while (true)
        {
            DbObject? obj = Deserialize(stream);

            if (obj is null)
            {
                break;
            }
            
            m_items.Add(obj.Name, obj);
        }
    }
}