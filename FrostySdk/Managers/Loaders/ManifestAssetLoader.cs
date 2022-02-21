namespace FrostySdk.Managers
{
    public partial class AssetManager
    {
        internal class ManifestAssetLoader : IAssetLoader
        {
            public void Load(AssetManager parent, BinarySbDataHelper helper)
            {
                // SWBF2 reads all data from a manifest
                parent.WriteToLog("Loading data from manifest");

                // @todo: Get proper superbundle names
                parent.superBundles.Add(new SuperBundleEntry { Name = "<none>" });

                foreach (DbObject bundle in parent.fs.EnumerateBundles())
                {
                    BundleEntry be = new BundleEntry { Name = bundle.GetValue<string>("name"), SuperBundleId = 0 };
                    parent.bundles.Add(be);

                    if (bundle == null)
                        continue;

                    // process assets
                    parent.ProcessBundleEbx(bundle, parent.bundles.Count - 1, helper);
                    parent.ProcessBundleRes(bundle, parent.bundles.Count - 1, helper);
                    parent.ProcessBundleChunks(bundle, parent.bundles.Count - 1, helper);
                }

                foreach (ChunkAssetEntry entry in parent.fs.ProcessManifestChunks())
                {
                    if (!parent.chunkList.ContainsKey(entry.Id))
                        parent.chunkList.Add(entry.Id, entry);
                }
            }
        }
    }
}
