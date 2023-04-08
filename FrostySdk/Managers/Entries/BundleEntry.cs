namespace Frosty.Sdk.Managers.Entries;

public enum BundleType
{
    None = -1,
    SubLevel,
    BlueprintBundle,
    SharedBundle
}
public class BundleEntry
{
    public string DisplayName => Name;

    public string Name = string.Empty;
    public int SuperBundleId;
    public EbxAssetEntry? Blueprint;
    public BundleType Type;
    public bool Added;
}