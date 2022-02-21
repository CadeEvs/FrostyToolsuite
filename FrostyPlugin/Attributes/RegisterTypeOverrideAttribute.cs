using System;

namespace Frosty.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class RegisterTypeOverrideAttribute : Attribute
    {
        public string LookupName { get; set; }
        public Type EditorType { get; set; }
        public RegisterTypeOverrideAttribute(string lookupName, Type type)
        {
            LookupName = lookupName;
            EditorType = type;
        }
    }
}
