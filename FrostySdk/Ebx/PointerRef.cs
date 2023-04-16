using System;
using Frosty.Sdk.IO.Ebx;

namespace Frosty.Sdk.Ebx;

public struct PointerRef
{
    public EbxImportReference External { get; }
    public object? Internal { get; }
    public PointerRefType Type { get; }

    public PointerRef(EbxImportReference externalRef)
    {
        External = externalRef;
        Internal = null;
        Type = PointerRefType.External;
    }

    public PointerRef(Guid guid)
    {
        External = new EbxImportReference { FileGuid = guid, ClassGuid = Guid.Empty };
        Internal = null;
        Type = (guid != Guid.Empty) ? PointerRefType.External : PointerRefType.Null;
    }

    public PointerRef(object internalRef)
    {
        External = new EbxImportReference();
        Internal = internalRef;
        Type = PointerRefType.Internal;
    }

    public static bool operator ==(PointerRef a, object b) => a.Equals(b);

    public static bool operator !=(PointerRef a, object b) => !a.Equals(b);

    public override bool Equals(object? obj)
    {  
        if (obj is not PointerRef b)
        {
            return false;
        }

        return Equals(b);
    }

    public bool Equals(PointerRef b)
    {
        return Type == b.Type && Internal == b.Internal && External == b.External;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            if (Type == PointerRefType.Internal)
            {
                hash = (hash * 16777619) ^ Internal!.GetHashCode();
            }
            else if (Type == PointerRefType.External)
            {
                hash = (hash * 16777619) ^ External.GetHashCode();
            }

            return hash;
        }
    }
}