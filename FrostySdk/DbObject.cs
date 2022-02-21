using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FrostySdk
{
    public class DbObject
    {
        public int Count => list?.Count ?? 0;

        internal IDictionary<string, object> hash;
        internal IList<object> list;

        public DbObject(bool bObject = true)
        {
            if (bObject) hash = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            else list = new List<object>();
        }

        public DbObject(object inVal)
        {
            if (inVal.GetType() == typeof(List<object>))
                list = (IList<object>)inVal;

            else if (inVal.GetType() == typeof(Dictionary<string, object>))
                hash = (IDictionary<string, object>)inVal;
        }

        public static DbObject CreateObject() => new DbObject(bObject: true);
        public static DbObject CreateList()   => new DbObject(bObject: false);

        #region -- Object functions --
        public T GetValue<T>(string name, T defaultValue = default(T))
        {
            if (hash == null || !hash.ContainsKey(name))
                return defaultValue;
            if (hash[name] is T)
                return (T)hash[name];
            return (T)Convert.ChangeType(hash[name], typeof(T));
        }

        public void SetValue(string name, object newValue)
        {
            if (newValue == null)
                return;

            if (!hash.ContainsKey(name))
            {
                AddValue(name, newValue);
                return;
            }
            hash[name] = newValue;
        }

        public void AddValue(string name, object value)
        {
            if (!hash.ContainsKey(name))
                hash.Add(name, SanitizeData(value));
        }

        public void RemoveValue(string name)
        {
            if (hash.ContainsKey(name))
                hash.Remove(name);
        }

        public bool HasValue(string name) => hash.ContainsKey(name);
        public IEnumerable<string> EnumerateKeys()
        {
            int count = hash?.Keys.Count ?? 0;
            for (int i = 0; i < count; i++)
                yield return hash.Keys.ElementAt(i);
        }

        public object GetRawValue(string name) => !hash.ContainsKey(name) ? null : hash[name];
        #endregion

        #region -- List functions --
        public IEnumerator GetEnumerator()
        {
            int count = list?.Count ?? 0;
            for (int i = 0; i < count; i++)
                yield return list[i];
        }

        public object this[int id] => id >= list.Count ? null : list[id];

        public void Add(object value) => list?.Add(SanitizeData(value));

        public void SetAt(int id, object value)
        {
            if (list == null || id >= list.Count)
                return;
            list[id] = SanitizeData(value);
        }

        public void Insert(int id, object value) => list.Insert(id, value);

        public void RemoveAt(int id)
        {
            if (list != null && id < list.Count)
                list.RemoveAt(id);
        }

        public int FindIndex(Predicate<object> match)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (match.Invoke(list[i]))
                    return i;
            }
            return -1;
        }

        public T Find<T>(Predicate<object> match)
        {
            if (list == null)
                return default;

            return (T)((List<object>)list).Find(match);
        }
        #endregion

        private object SanitizeData(object value)
        {
            if (value is uint)
            {
                uint tmp = (uint)value;
                value = (int)tmp;
            }
            else if (value is ulong)
            {
                ulong tmp = (ulong)value;
                value = (long)tmp;
            }

            return value;
        }
    }
}
