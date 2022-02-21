using System;

namespace DAIMod
{
    public class ModConfigElementsList
    {
        public virtual void AddIntElement(string id, string name, int defValue, int minValue, int maxValue) { throw new NotImplementedException(); }
        public virtual void AddFloatElement(string id, string name, float defValue, float minValue, float maxValue) { throw new NotImplementedException(); }
        public virtual void AddBoolElement(string id, string name, bool defValue) { throw new NotImplementedException(); }
        public virtual void AddEnumElement(string id, string name, Type enumType, int defValue) { throw new NotImplementedException(); }
        public virtual void AddStringElement(string id, string name, string defValue) { throw new NotImplementedException(); }
    }

    public interface ModScript
    {
        void ConstructUI(ModConfigElementsList ConfigElementsList);
        void RunScript();
    }

    public interface IScripting
    {
        object GetConfigParam(string name);
        void LogLn(string line);
        byte[] GetResourceData(int index);
        void SetResourceData(int index, byte[] data);
        void SetResourceEnabled(int index, bool enabled);
    }

    public class Scripting
    {
        static IScripting scriptInterface;

        public static object GetConfigParam(string name) => scriptInterface != null ? scriptInterface.GetConfigParam(name) : 0;

        public static void LogLn(string line) => scriptInterface?.LogLn(line);

        public static byte[] GetResourceData(int index) => scriptInterface?.GetResourceData(index);

        public static void SetResourceData(int index, byte[] data) => scriptInterface?.SetResourceData(index, data);

        public static void SetResourceEnabled(int index, bool enabled) => scriptInterface?.SetResourceEnabled(index, enabled);

        public static void SetScriptInterface(IScripting inScriptInterface) => scriptInterface = inScriptInterface;
    }
}
