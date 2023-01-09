using FrostySdk.Managers.Entries;

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
                parent.m_superBundles.Add(new SuperBundleEntry { Name = "<none>" });

                foreach (DbObject bundle in parent.m_fileSystem.EnumerateBundles())
                {
                    BundleEntry be = new BundleEntry { Name = bundle.GetValue<string>("name"), SuperBundleId = 0 };
                    parent.m_bundles.Add(be);

                    if (bundle == null)
                        continue;

                    // process assets
                    parent.ProcessBundleEbx(bundle, parent.m_bundles.Count - 1, helper);
                    parent.ProcessBundleRes(bundle, parent.m_bundles.Count - 1, helper);
                    parent.ProcessBundleChunks(bundle, parent.m_bundles.Count - 1, helper);
                }

                foreach (ChunkAssetEntry entry in parent.m_fileSystem.ProcessManifestChunks())
                {
                    if (!parent.m_chunkList.ContainsKey(entry.Id))
                    {
                        parent.m_chunkList.Add(entry.Id, entry);
                    }
                    else
                    {
                        parent.m_chunkList[entry.Id].SuperBundles.AddRange(entry.SuperBundles);
                    }
                }
            }
        }
    }
}
