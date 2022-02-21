using FrostySdk.Managers;
using System;

namespace Frosty.Core.Attributes
{
    public enum CustomHandlerType
    {
        Ebx,
        Res
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomHandlerAttribute : Attribute
    {
        public CustomHandlerType HandlerType { get; set; }
        public Type HandlerClassType { get; set; }

        // Ebx
        public string EbxType { get; set; }

        // Res
        public ResourceType ResType { get; set; }

        public RegisterCustomHandlerAttribute(CustomHandlerType inType, Type inClassType, ResourceType resType = ResourceType.Invalid, string ebxType = "")
        {
            HandlerType = inType;
            HandlerClassType = inClassType;
            ResType = resType;
            EbxType = ebxType;
        }
    }
}
