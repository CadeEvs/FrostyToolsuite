using FrostySdk.Managers;
using System;

namespace Frosty.Core.Attributes
{
    public enum CustomHandlerType
    {
        Ebx,
        Res,
        CustomAsset
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

        // Custom
        public string CustomAssetType { get; set; }

        public RegisterCustomHandlerAttribute(CustomHandlerType inType, Type inClassType, ResourceType resType = ResourceType.Invalid, string ebxType = "", string customType = "")
        {
            HandlerType = inType;
            HandlerClassType = inClassType;
            ResType = resType;
            EbxType = ebxType;
            CustomAssetType = customType;
        }
    }
}
