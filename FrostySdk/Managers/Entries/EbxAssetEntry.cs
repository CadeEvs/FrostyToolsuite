using System;
using System.Collections.Generic;

namespace FrostySdk.Managers.Entries
{
    public class EbxAssetEntry : AssetEntry
    {
        public override string Name
        {
            get
            {
                // TODO: @techdebt find better method to move blueprint bundles to sub-folder, this will most likely break writing.
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042) &&
                    (base.Name.Contains("cd_") || base.Name.Contains("md_")))
                {
                    return $"win32/{base.Name}";
                }

                return base.Name;
            }
        }
        
        public Guid Guid;
        public List<Guid> DependentAssets = new List<Guid>();
        public override string AssetType => "ebx";

        public bool ContainsDependency(Guid guid)
        {
            return HasModifiedData ? ModifiedEntry.DependentAssets.Contains(guid) : DependentAssets.Contains(guid);
        }

        public IEnumerable<Guid> EnumerateDependencies()
        {
            if (HasModifiedData)
            {
                foreach (Guid guid in ModifiedEntry.DependentAssets)
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
}