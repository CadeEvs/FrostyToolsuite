using Frosty.Core;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BundleManager
{
    internal class BundleManagerPrerequisites
    {
        private int Version = 0;
        public Dictionary<EbxAssetEntry, List<BundleEntry>> assetsAddedToBundles = new Dictionary<EbxAssetEntry, List<BundleEntry>>();

        public void FindBundleEdits()
        {
            assetsAddedToBundles.Clear();
            foreach (EbxAssetEntry parEntry in App.AssetManager.EnumerateEbx())
            {
                if (!parEntry.IsModified || parEntry.IsAdded || parEntry.AddedBundles.Count == 0)
                    continue;
                assetsAddedToBundles.Add(parEntry, parEntry.AddedBundles.Select(o => App.AssetManager.GetBundleEntry(o)).ToList());
            }
        }

        public void WriteToFile(string file)
        {
            using (NativeWriter writer = new NativeWriter(new FileStream(file, FileMode.Create)))
            {
                writer.WriteNullTerminatedString("MopMagicMopMagic"); //Magic
                writer.Write(Version);
                writer.Write(assetsAddedToBundles.Count);
                foreach(KeyValuePair<EbxAssetEntry, List<BundleEntry>> assetPairs in assetsAddedToBundles)
                {
                    writer.WriteNullTerminatedString(assetPairs.Key.Name);
                    writer.Write(assetPairs.Value.Count);
                    foreach(BundleEntry entry in assetPairs.Value)
                    {
                        writer.WriteNullTerminatedString(entry.Name);
                    }
                }
            }
        }

        public void ReadFile(string file)
        {
            using (NativeReader reader = new NativeReader(new FileStream(file, FileMode.Open, FileAccess.Read)))
            {
                if (reader.ReadNullTerminatedString() != "MopMagicMopMagic" || reader.ReadInt() > Version)
                {
                    App.Logger.Log(string.Format($"Could not read prerequisite file {file}. Your BM may be out of date"));
                    return;
                }
                int pairsCount = reader.ReadInt();
                for (int i = 0; i < pairsCount; i++)
                {
                    EbxAssetEntry parEntry = App.AssetManager.GetEbxEntry(reader.ReadNullTerminatedString());
                    if (!assetsAddedToBundles.ContainsKey(parEntry))
                        assetsAddedToBundles.Add(parEntry, new List<BundleEntry>());

                    int bundlesCount = reader.ReadInt();
                    for (int y = 0; y < bundlesCount; y++)
                    {
                        int bunId = App.AssetManager.GetBundleId(reader.ReadNullTerminatedString());
                        if (bunId == -1)
                            continue;
                        BundleEntry bEntry = App.AssetManager.GetBundleEntry(bunId);
                        if (bEntry == null)
                            continue;
                        if (!assetsAddedToBundles[parEntry].Contains(bEntry))
                            assetsAddedToBundles[parEntry].Add(bEntry);
                    }

                }
            }
        }
    }
}
