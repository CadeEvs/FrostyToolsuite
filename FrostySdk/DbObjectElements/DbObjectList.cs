using System;
using System.Collections;
using System.Collections.Generic;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.DbObjectElements;

public class DbObjectList : DbObject, IEnumerable<DbObject>
{
    public int Count => m_items.Count;
    
    private readonly List<DbObject> m_items;

    protected internal DbObjectList(Type inType)
        : base(inType)
    {
        m_items = new List<DbObject>();
    }

    protected internal  DbObjectList(int inCapacity)
        : base(Type.List | Type.Anonymous)
    {
        m_items = new List<DbObject>(inCapacity);
    }
    
    protected internal  DbObjectList(string inName, int inCapacity)
        : base(Type.List, inName)
    {
        m_items = new List<DbObject>(inCapacity);
    }

    public override bool IsList() => true;

    public override DbObjectList AsList()
    {
        return this;
    }
    
    public void Add(DbObjectDict value)
    {
        m_items.Add(value);
    }

    public void Add(DbObjectList value)
    {
        m_items.Add(value);
    }
    
    public void Add(bool value)
    {
        m_items.Add(new DbObjectBool(value));
    }

    public void Add(string value)
    {
        m_items.Add(new DbObjectString(value));
    }

    public void Add(int value)
    {
        m_items.Add(new DbObjectInt(value));
    }

    public void Add(uint value)
    {
        m_items.Add(new DbObjectInt((int)value));
    }

    public void Add(long value)
    {
        m_items.Add(new DbObjectLong(value));
    }

    public void Add(ulong value)
    {
        m_items.Add(new DbObjectLong((long)value));
    }

    public void Add(float value)
    {
        m_items.Add(new DbObjectFloat(value));
    }

    public void Add(double value)
    {
        m_items.Add(new DbObjectDouble(value));
    }

    public void Add(Guid value)
    {
        m_items.Add(new DbObjectGuid(value));
    }

    public void Add(Sha1 value)
    {
        m_items.Add(new DbObjectSha1(value));
    }

    public void Add(byte[] value)
    {
        m_items.Add(new DbObjectBlob(value));
    }

    public DbObject this[int index] => m_items[index];

    protected override void InternalSerialize(DataStream stream)
    {
        long curPos = stream.Position;
        
        foreach (DbObject value in m_items)
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
            
            m_items.Add(obj);
        }
    }
    
    private class DbObjectListEnum : IEnumerator<DbObject>
    {
        private List<DbObject> m_items;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        private int m_position = -1;

        public DbObjectListEnum(List<DbObject> inItems)
        {
            m_items = inItems;
        }

        public bool MoveNext()
        {
            m_position++;
            return m_position < m_items.Count;
        }

        public void Reset()
        {
            m_position = -1;
        }

        object IEnumerator.Current => Current;

        public DbObject Current
        {
            get
            {
                try
                {
                    return m_items[m_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void Dispose()
        {
        }
    }

    public IEnumerator<DbObject> GetEnumerator()
    {
        return new DbObjectListEnum(m_items);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
