using Frosty.Hash;
using System;

namespace FrostySdk.Ebx
{
    public class TypeRef
    {
        public string Name => typeName;
        public Guid Guid => typeGuid;
        private Guid typeGuid;
        private readonly string typeName;

        public TypeRef()
        {
            typeName = "";
        }

        public TypeRef(string value)
        {
            typeName = value;
        }

        public TypeRef(Guid guid)
        {
            typeGuid = guid;
            typeName = TypeLibrary.Reflection.LookupType(guid);
            // type is likely using a signed GUID
            if (typeName == guid.ToString())
            {
                Type refType = TypeLibrary.GetType(guid);
                if (refType == null)
                {
                    throw new Exception($"Could not find a type with the GUID {guid}");
                }
                typeName = refType.Name;
            }
        }

        public Type GetReferencedType()
        {
            // should be a primitive type if the GUID is empty
            if (typeGuid == Guid.Empty)
            {
                Type refType = TypeLibrary.GetType(typeName);
                if (refType == null)
                {
                    throw new Exception($"Could not find the type {typeName}");
                }
                return refType;
            }
            else
            {

                Type refType = TypeLibrary.GetType((uint)Fnv1.HashString(typeName));
                if (refType == null)
                {
                    refType = TypeLibrary.GetType(typeGuid);
                    if (refType == null)
                    {
                        throw new Exception($"Could not find the type {typeName}");
                    }
                }
                return refType;
            }
        }

        public static implicit operator string(TypeRef value)
        {
            return value.typeGuid != Guid.Empty ? value.typeGuid.ToString().ToUpper() : value.typeName;
        }

        public static implicit operator TypeRef(string value) => new TypeRef(value);

        public static implicit operator TypeRef(Guid guid) => new TypeRef(guid);

        public bool IsNull() => string.IsNullOrEmpty(typeName);

        public override string ToString() => "TypeRef '" + (IsNull() ? "(null)" : typeName) + "'";
    }
}
