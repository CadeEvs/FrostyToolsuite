using System;

namespace Frosty.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterStartupActionAttribute : Attribute
    {
        public Type StartupActionType { get; private set; }

        public RegisterStartupActionAttribute(Type inActionType)
        {
            StartupActionType = inActionType;
        }
    }
}
