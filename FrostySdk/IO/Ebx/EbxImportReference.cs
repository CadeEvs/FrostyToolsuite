using System;

namespace Frosty.Sdk.IO.Ebx;

public struct EbxImportReference
{
    public Guid FileGuid;
    public Guid ClassGuid;

    public override string ToString() => FileGuid.ToString() + "/" + ClassGuid.ToString();

    public static bool operator ==(EbxImportReference a, EbxImportReference b) => a.Equals(b);

    public static bool operator !=(EbxImportReference a, EbxImportReference b) => !a.Equals(b);

    public override bool Equals(object? obj)
    {
        if (obj is EbxImportReference b)
        {
            return (FileGuid == b.FileGuid && ClassGuid == b.ClassGuid);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ FileGuid.GetHashCode();
            hash = (hash * 16777619) ^ ClassGuid.GetHashCode();
            return hash;
        }
    }
}