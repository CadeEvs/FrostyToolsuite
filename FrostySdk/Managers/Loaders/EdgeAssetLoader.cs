using FrostySdk.IO;
using System.IO;
using FrostySdk.Managers.Entries;

namespace FrostySdk.Managers
{
    public partial class AssetManager
    {
        internal class EdgeAssetLoader : IAssetLoader
        {
            public void Load(AssetManager parent, BinarySbDataHelper helper)
            {
                foreach (string superBundleName in parent.m_fileSystem.SuperBundles)
                {
                    DbObject toc = parent.ProcessTocChunks(string.Format("{0}.toc", superBundleName), helper, false);
                    if (toc == null)
                        continue;

                    parent.m_superBundles.Add(new SuperBundleEntry() { Name = superBundleName });
                    parent.WriteToLog("Loading data ({0})", superBundleName);
                    parent.ReportProgress(parent.m_superBundles.Count, parent.m_fileSystem.SuperBundleCount);

                    using (NativeReader sbReader = new NativeReader(new FileStream(parent.m_fileSystem.ResolvePath(string.Format("{0}.sb", superBundleName)), FileMode.Open, FileAccess.Read)))
                    {
                        DbObject bundles = toc.GetValue<DbObject>("bundles");
                        for (int i = 0; i < bundles.GetValue<DbObject>("names").Count; i++)
                        {
                            string bundleName = bundles.GetValue<DbObject>("names")[i] as string;
                            int offset = (int)bundles.GetValue<DbObject>("offsets")[i];
                            int size = (int)bundles.GetValue<DbObject>("sizes")[i];

                            // add new bundle entry
                            parent.m_bundles.Add(new BundleEntry() { Name = bundleName, SuperBundleId = parent.m_superBundles.Count - 1 });
                            int bundleId = parent.m_bundles.Count - 1;

                            DbObject sb = null;
                            using (DbReader reader = new DbReader(sbReader.CreateViewStream(offset, size), parent.m_fileSystem.CreateDeobfuscator()))
                                sb = reader.ReadDbObject();

                            // process assets
                            parent.ProcessBundleEbx(sb, bundleId, helper);
                            parent.ProcessBundleRes(sb, bundleId, helper);
                            parent.ProcessBundleChunks(sb, bundleId, helper);
                        }
                    }
                }
            }
        }
    }
}
