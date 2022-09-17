namespace FrostySdk.Managers.Entries
{
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

        public string Name;
        public int SuperBundleId;
        public EbxAssetEntry Blueprint;
        public BundleType Type;
        public bool Added;
    }
}