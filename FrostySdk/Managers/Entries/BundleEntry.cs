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
    /// <summary>
    /// The name of this <see cref="BundleEntry"/>.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The Id of the SuperBundle that contains this <see cref="BundleEntry"/>.
    /// </summary>
    public int SuperBundleId { get; }
    
    /// <summary>
    /// The <see cref="EbxAssetEntry"/> of the Blueprint if this <see cref="BundleEntry"/> is a <see cref="BundleType.BlueprintBundle"/>.
    /// </summary>
    public EbxAssetEntry? Blueprint { get; internal set; }
    
    /// <summary>
    /// The <see cref="BundleType"/> of this <see cref="BundleEntry"/>.
    /// </summary>
    public BundleType Type { get; internal set; }

    public BundleEntry(string inName, int inSuperBundleId)
    {
        Name = inName;
        SuperBundleId = inSuperBundleId;
    }
}