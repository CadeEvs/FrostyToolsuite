using System;
using System.Collections.Generic;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.Managers.Entries;

public class EbxAssetEntry : AssetEntry
{
    public override string Name
    {
        get
        {
            // TODO: @techdebt find better method to move blueprint bundles to sub-folder, this will most likely break writing.
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042) &&
                (base.Name.StartsWith("cd_") || base.Name.StartsWith("md_") &! base.Name.Contains("win32/")))
            {
                return $"win32/{base.Name}";
            }

            return base.Name;
        }
    }
        
    public Guid Guid;
    public readonly List<Guid> DependentAssets = new();
    public override string AssetType => "ebx";

    public EbxAssetEntry(string inName, Sha1 inSha1, long inSize, long inOriginalSize)
        : base(inSha1, inSize, inOriginalSize)
    {
        base.Name = inName;
    }

    public bool ContainsDependency(Guid guid)
    {
        return HasModifiedData ? ModifiedEntry!.DependentAssets.Contains(guid) : DependentAssets.Contains(guid);
    }

    public IEnumerable<Guid> EnumerateDependencies()
    {
        if (HasModifiedData)
        {
            foreach (Guid guid in ModifiedEntry!.DependentAssets)
            {
                yield return guid;
            }
        }
        else
        {
            foreach (Guid guid in DependentAssets)
            {
                yield return guid;
            }
        }
    }
}