using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Library.Reflection
{
    public static class ReflectionUtils
    {
        public static void CallPrivateMethod(Type type, object instance, string function, params object[] parameters)
        {
            type.GetMethod(function, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(instance, parameters);
        }

        public static void SetPrivateField(Type type, object instance, string memberName, object newValue)
        {
            type.GetField(memberName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(instance, newValue);
        }

        public static object GetPrivateField(Type type, object instance, string memberName)
        {
            return type.GetField(memberName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);
        }

        public static string AddStringToList(string value)
        {
            int hash = Frosty.Hash.Fnv1.HashString(value);
            IDictionary<int, string> strings = (IDictionary<int, string>)typeof(FrostySdk.Utils).GetField("strings", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            if (!strings.ContainsKey(hash))
            {
                strings.Add(hash, value);
            }

            return value;
        }
    }
}
