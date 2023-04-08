using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.Managers.Entries;

public class ResAssetEntry : AssetEntry
{
    public override string Type => ((ResourceType)ResType).ToString();
    public override string AssetType => "res";
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

    public ulong ResRid;
    public uint ResType;
    public byte[] ResMeta;

    public ResAssetEntry(string inName, Sha1 inSha1, long inSize, long inOriginalSize, ulong inResRid, uint inResType, byte[] inResMeta)
        : base(inSha1, inSize, inOriginalSize)
    {
        base.Name = inName;
        ResRid = inResRid;
        ResType = inResType;
        ResMeta = inResMeta;
    }
}