using FrostySdk.IO;
using System.IO;

namespace FrostySdk.Managers
{
    public partial class AssetManager
    {
        internal class EdgeAssetLoader : IAssetLoader
        {
            public void Load(AssetManager parent, BinarySbDataHelper helper)
            {
                foreach (string superBundleName in parent.fs.SuperBundles)
                {
                    DbObject toc = parent.ProcessTocChunks(string.Format("{0}.toc", superBundleName), helper, false);
                    if (toc == null)
                        continue;

                    parent.WriteToLog("Loading Data ({0})", superBundleName);
                    using (NativeReader sbReader = new NativeReader(new FileStream(parent.fs.ResolvePath(string.Format("{0}.sb", superBundleName)), FileMode.Open, FileAccess.Read)))
                    {
                        DbObject bundles = toc.GetValue<DbObject>("bundles");
                        for (int i = 0; i < bundles.GetValue<DbObject>("names").Count; i++)
                        {
                            string bundleName = bundles.GetValue<DbObject>("names")[i] as string;
                            int offset = (int)bundles.GetValue<DbObject>("offsets")[i];
                            int size = (int)bundles.GetValue<DbObject>("sizes")[i];

                            // add new bundle entry
                            parent.bundles.Add(new BundleEntry() { Name = bundleName, SuperBundleId = parent.superBundles.Count - 1 });
                            int bundleId = parent.bundles.Count - 1;

                            DbObject sb = null;
                            using (DbReader reader = new DbReader(sbReader.CreateViewStream(offset, size), parent.fs.CreateDeobfuscator()))
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
