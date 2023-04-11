using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Frosty.Sdk;

public class InvalidDbTypeException : Exception
{
    public InvalidDbTypeException(bool isObject)
     :base(isObject ? "DbObject is List type not Object type." : "DbObject is Object type not List type.")
    {
    }
}

public partial class DbObject
{
    public int Count => m_list?.Count ?? 0;

    internal readonly Dictionary<string, object>? m_hash;
    internal readonly List<object>? m_list;

    public DbObject(bool bObject = true, int capacity = 0)
    {
        if (bObject)
        {
            m_hash = new Dictionary<string, object>(capacity, StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            m_list = new List<object>(capacity);
        }
    }

    public DbObject(object inVal)
    {
        if (inVal is List<object> list)
        {
            m_list = list;
        }

        else if (inVal is Dictionary<string, object> hash)
        {
            m_hash = hash;
        }
    }

    public static DbObject CreateObject(int capacity = 0) => new(bObject: true, capacity);
    public static DbObject CreateList(int capacity = 0)   => new(bObject: false, capacity);

    public DbType GetDbType()
    {
        if (m_hash != null)
            return DbType.Object;
        else if (m_list != null)
            return DbType.List;
        return DbType.Invalid;
    }

    #region -- Object functions --
    public object this[string key]
    {
        get
        {
            if (m_hash == null)
            {
                throw new InvalidDbTypeException(true);
            }

            return m_hash[key];
        }
        set
        {
            if (m_hash == null)
            {
                throw new InvalidDbTypeException(true);
            }

            if (!m_hash.ContainsKey(key))
            {
                m_hash.Add(key, SanitizeData(value));
            }
            else
            {
                m_hash[key] = SanitizeData(value);
            }
        }
    }

    public bool ContainsKey(string key)
    {
        if (m_hash == null)
        {
            throw new InvalidDbTypeException(true);
        }

        return m_hash.ContainsKey(key);
    }

    public void AddValue(string name, object value)
    {
        if (m_hash == null)
        {
            throw new InvalidDbTypeException(true);
        }

        m_hash.TryAdd(name, SanitizeData(value));
    }

    public void RemoveValue(string name)
    {
        if (m_hash == null)
        {
            throw new InvalidDbTypeException(true);
        }

        m_hash.Remove(name);
    }
    
    public IEnumerable<string> EnumerateKeys()
    {
        if (m_hash == null)
        {
            throw new InvalidDbTypeException(true);
        }
        
        int count = m_hash.Keys.Count;
        for (int i = 0; i < count; i++)
            yield return m_hash.Keys.ElementAt(i);
    }

    #endregion

    #region -- List functions --
    public IEnumerator GetEnumerator()
    {
        if (m_list == null)
        {
            throw new InvalidDbTypeException(false);
        }
        
        int count = m_list.Count;
        for (int i = 0; i < count; i++)
            yield return m_list[i];
    }

    public object this[int id] => m_list == null ? throw new InvalidDbTypeException(false) : m_list[id];

    public void Add(object value) => m_list?.Add(SanitizeData(value));

    public void SetAt(int id, object value)
    {
        if (m_list == null || id >= m_list.Count)
        {
            return;
        }

        m_list[id] = SanitizeData(value);
    }

    public void Insert(int id, object value)
    {
        if (m_list == null)
        {
            throw new InvalidDbTypeException(false);
        }
        m_list.Insert(id, value);
    }

    public void RemoveAt(int id)
    {
        if (m_list == null)
        {
            throw new InvalidDbTypeException(false);
        }
        m_list.RemoveAt(id);
    }

    public int FindIndex(Predicate<object> match)
    {
        if (m_list == null)
        {
            throw new InvalidDbTypeException(false);
        }
        for (int i = 0; i < m_list.Count; i++)
        {
            if (match.Invoke(m_list[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public T? Find<T>(Predicate<object> match)
    {
        if (m_list == null)
        {
            throw new InvalidDbTypeException(false);
        }

        return (T?)((List<object>)m_list).Find(match);
    }
    #endregion

    private object SanitizeData(object value)
    {
        // these are here so there arent any crashes when converting values that are added by frosty
        if (value is byte b)
        {
            value = (sbyte)b;
        }
        else if (value is ushort us)
        {
            value = (short)us;
        }
        
        // only those are actually needed
        else if (value is uint u)
        {
            value = (int)u;
        }
        else if (value is ulong ul)
        {
            value = (long)ul;
        }

        return value;
    }
}