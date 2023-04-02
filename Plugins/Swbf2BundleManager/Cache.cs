using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using RootInstanceEntriesPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BundleManager
{
    public static class Cache
    {
        public static bool IsLoaded { get; private set; } = false;
        private static int BM_Version = 3;
        private static AssetManager AM = App.AssetManager;
        private static Stopwatch stopWatch = new Stopwatch();

        public static Dictionary<int, List<int>> BundleParents { get; private set; } = new Dictionary<int, List<int>>();
        public static Dictionary<EbxAssetEntry, AssetData> UnmodifiedAssetData { get; private set; } = new Dictionary<EbxAssetEntry, AssetData>();
        private static Dictionary<EbxAssetEntry, List<Guid>> NetRegReferences = new Dictionary<EbxAssetEntry, List<Guid>>();
        public static List<string> NetRegReferenceTypes { get; private set; } = new List<string>();
        public static Dictionary<EbxAssetEntry, Dictionary<UInt32, MeshVariOriginalData>> MeshVariationEntries { get; private set; } = new Dictionary<EbxAssetEntry, Dictionary<UInt32, MeshVariOriginalData>>();
        public static Dictionary<EbxAssetEntry, Dictionary<EbxAssetEntry, ResAssetEntry>> ObjectVariationPairs { get; private set; } = new Dictionary<EbxAssetEntry, Dictionary<EbxAssetEntry, ResAssetEntry>>();

        private static int stageIdx = 1;
        private static Dictionary<int, int> gameStageCounts = new Dictionary<int, int>()
        {
            {(int)ProfileVersion.StarWarsBattlefrontII, 7 },
            {(int)ProfileVersion.StarWarsBattlefront, 4},
            {(int)ProfileVersion.Battlefield5, 4},
            {(int)ProfileVersion.Battlefield1, 4},
        };

        public static bool LoadCache(FrostyTaskWindow task)
        {
            if (IsLoaded)
                return true;
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII && RootInstanceEbxEntryDb.IsLoaded != true && RootInstanceEbxEntryDb.ReadCache(task) == false)
            {
                var result = FrostyMessageBox.Show(string.Format("Root Instance Plugin Cache required. Would you like to generate one?"), "Bundle Manager", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) RootInstanceEbxEntryDb.LoadEbxRootInstanceEntries(task);
                else
                    return false;
            }

            if (!ReadingCache(task))
            {
                if (AM.GetModifiedCount() == 0)
                {
                    var result = FrostyMessageBox.Show(string.Format("New Bundle Manager Cache required. Would you like to generate one?"), "Bundle Manager", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes) { CreateCache(task); }
                    else
                        return false;
                }
                else
                {
                    FrostyMessageBox.Show(string.Format("New Bundle Manager Cache required. Please start a new project file without any modifications to generate one"), "Bundle Manager");
                    return false;
                }
            }
            return true;
        }


        public static void CreateCache(FrostyTaskWindow task)
        {
            stopWatch.Start();


            CreateCacheEnumerateSharedBundles(task);
            CreateCacheEnumerateSublevelBundles(task);
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                CreateCacheSWBF2CampaignSearch(task);
                CreateCacheSWBF2VurSearch(task);
            }
            FindAllNetRegObjectReferences(task);
            CreateCacheMeshVariationDatabase(task);
            LogAssetData(task);
            ExportingCache(App.FileSystemManager.CacheName);
            stopWatch.Stop();
            App.Logger.Log(string.Format("Bundle Manager Cache generated in {0} seconds", stopWatch.Elapsed));
            stopWatch.Reset();
        }

        #region Bundle Parents
        public static object forLock = new object();

        private static void LogUpdate(FrostyTaskWindow task, string update)
        {
            string fullUpdate = String.Format("{0}/{1}: {2}", stageIdx++, gameStageCounts[ProfilesLibrary.DataVersion], update);
            App.Logger.Log(fullUpdate);
            task.Update(fullUpdate, 0);
        }
        private static void CreateCacheEnumerateSharedBundles(FrostyTaskWindow task) //Works out the order in which shared bundles are loaded using a primitive technique 
        {
            LogUpdate(task, "Caching Shared Bundle Inheritance");
            int forCount = AM.EnumerateBundles(BundleType.SharedBundle).ToList().Count;
            int forIdx = 0;
            Parallel.ForEach(AM.EnumerateBundles(BundleType.SharedBundle), bEntry => 
            {
                int bunId = App.AssetManager.GetBundleId(bEntry);
                List<BundleEntry> ParallelBundles = new List<BundleEntry>();
                List<BundleEntry> DependantBundles = new List<BundleEntry>();
                //List<BundleEntry> UnrelatedBundles = new List<BundleEntry>();
                foreach (EbxAssetEntry parEntry in App.AssetManager.EnumerateEbx().Where(o => o.IsInBundle(bunId)))
                {
                    foreach (Guid refGuid in parEntry.DependentAssets)
                    {
                        EbxAssetEntry refEntry = App.AssetManager.GetEbxEntry(refGuid);

                        //Temp fix for BFV
                        if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 & refEntry == null) continue;

                        if (refEntry.IsInBundle(bunId))
                        {
                            foreach (int otherBundleId in refEntry.Bundles)
                            {
                                BundleEntry otherBundleEntry = App.AssetManager.GetBundleEntry(otherBundleId);
                                if (!ParallelBundles.Contains(otherBundleEntry) && otherBundleEntry.Type == BundleType.SharedBundle)
                                {
                                    ParallelBundles.Add(otherBundleEntry);
                                }
                            }
                        }
                        else
                        {
                            foreach (int otherBundleId in refEntry.Bundles)
                            {
                                BundleEntry otherBundleEntry = App.AssetManager.GetBundleEntry(otherBundleId);
                                if (!DependantBundles.Contains(otherBundleEntry) && otherBundleEntry.Type == BundleType.SharedBundle)
                                {
                                    DependantBundles.Add(otherBundleEntry);
                                }
                            }
                        }
                    }
                }
                foreach (BundleEntry otherBundleEntry in ParallelBundles)
                {
                    if (DependantBundles.Contains(otherBundleEntry))
                    {
                        //UnrelatedBundles.Add(otherBundleEntry);
                        DependantBundles.Remove(otherBundleEntry);
                    }
                }
                if (DependantBundles.Count > 0)
                {
                    List<int> bunParents = new List<int>();
                    foreach (BundleEntry otherBundleEntry in DependantBundles)
                    {
                        bunParents.Add(AM.GetBundleId(otherBundleEntry));
                    }
                    lock (forLock)
                    {
                        BundleParents.Add(AM.GetBundleId(bEntry), bunParents);
                    }
                }
                task.Update(progress:(float)forIdx++ / forCount * 100);
            });
            //foreach (BundleEntry bEntry in AM.EnumerateBundles(BundleType.SharedBundle))
            //{
            //    task.Update(String.Format("{0}/{1}: {2}", stageIdx, gameStageCounts[ProfilesLibrary.DataVersion], "Caching: " + bEntry.Name), ((enumIdx++ / (double)enumCount) * 100.0d));
            //    int Bid = App.AssetManager.GetBundleId(bEntry);
            //    List<BundleEntry> ParallelBundles = new List<BundleEntry>();
            //    List<BundleEntry> DependantBundles = new List<BundleEntry>();
            //    List<BundleEntry> UnrelatedBundles = new List<BundleEntry>();
            //    foreach (EbxAssetEntry parEntry in App.AssetManager.EnumerateEbx())
            //    {
            //        if (parEntry.IsInBundle(Bid))
            //        {
            //            foreach (Guid refGuid in parEntry.DependentAssets)
            //            {
            //                EbxAssetEntry refEntry = App.AssetManager.GetEbxEntry(refGuid);

            //                //Temp fix for BFV
            //                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 & refEntry == null) continue;

            //                if (refEntry.IsInBundle(Bid))
            //                {
            //                    foreach (int otherBundleId in refEntry.Bundles)
            //                    {
            //                        BundleEntry otherBundleEntry = App.AssetManager.GetBundleEntry(otherBundleId);
            //                        if (!ParallelBundles.Contains(otherBundleEntry) & otherBundleEntry.Type == BundleType.SharedBundle)
            //                        {
            //                            ParallelBundles.Add(otherBundleEntry);
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    foreach (int otherBundleId in refEntry.Bundles)
            //                    {
            //                        BundleEntry otherBundleEntry = App.AssetManager.GetBundleEntry(otherBundleId);
            //                        if (!DependantBundles.Contains(otherBundleEntry) & otherBundleEntry.Type == BundleType.SharedBundle)
            //                        {
            //                            DependantBundles.Add(otherBundleEntry);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    foreach (BundleEntry otherBundleEntry in ParallelBundles)
            //    {
            //        if (DependantBundles.Contains(otherBundleEntry))
            //        {
            //            UnrelatedBundles.Add(otherBundleEntry);
            //            DependantBundles.Remove(otherBundleEntry);
            //        }
            //    }
            //    if (DependantBundles.Count > 0)
            //    {
            //        List<int> bunParents = new List<int>();
            //        foreach (BundleEntry otherBundleEntry in DependantBundles)
            //        {
            //            bunParents.Add(AM.GetBundleId(otherBundleEntry));
            //        }
            //        BundleParents.Add(AM.GetBundleId(bEntry), bunParents);
            //    }
            //}
        }

        private static void CreateCacheEnumerateSublevelBundles(FrostyTaskWindow task) //Uses description files and level data to find the order in which sublevel bundles are loaded.
        {
            LogUpdate(task, "Caching Sublevel Bundle Inheritance");
            int forCount = AM.EnumerateEbx(type: "LevelDescriptionAsset").ToList().Count;
            int forIdx = 0;
            Parallel.ForEach(AM.EnumerateEbx(type: "LevelDescriptionAsset"), refEntry => 
            {
                if (refEntry.IsAdded)
                    return;
                dynamic refRoot = AM.GetEbx(refEntry, true).RootObject;
                string LevelName = BunNameCorrection(refRoot.LevelName);
                List<int> Parents = new List<int>();
                foreach (dynamic obj in refRoot.Bundles)
                {
                    string BunName = "win32/" + BunNameCorrection(obj.Name);
                    int bunId = AM.GetBundleId(BunName);
                    if (bunId == -1)
                    {
                        bunId = AM.GetBundleId(((uint)Utils.HashString(BunName)).ToString("x"));
                        if (bunId == -1)
                        {
                            App.Logger.Log("Error: Could not find bundle " + BunName + " or " + ((uint)Utils.HashString(BunName)).ToString("x"));
                            continue;
                        }
                    }
                    Parents.Add(bunId);
                }
                CreateCacheSearchLevelData(LevelName, Parents);
                task.Update(progress: (float)forIdx++ / forCount * 100);
            });

            //foreach (EbxAssetEntry refEntry in App.AssetManager.EnumerateEbx(type: "LevelDescriptionAsset"))
            //{
            //    if (refEntry.IsAdded)
            //        continue;
            //    dynamic refRoot = AM.GetEbx(refEntry, true).RootObject;
            //    string LevelName = BunNameCorrection(refRoot.LevelName);
            //    task.Update(String.Format("{0}/{1}: {2}", stageIdx, gameStageCounts[ProfilesLibrary.DataVersion], "Caching Level: " + LevelName), ((forIdx++ / (double)forCount) * 100.0d) + 0.0d);
            //    List<int> Parents = new List<int>();
            //    foreach (dynamic obj in refRoot.Bundles)
            //    {
            //        string BunName = "win32/" + BunNameCorrection(obj.Name);
            //        int BundID = AM.GetBundleId(BunName);
            //        if (BundID == -1)
            //        {
            //            BundID = AM.GetBundleId(((uint)Utils.HashString(BunName)).ToString("x"));
            //            if (BundID == -1)
            //            {
            //                App.Logger.Log("Error: Could not find bundle " + BunName + " or " + ((uint)Utils.HashString(BunName)).ToString("x"));
            //                continue;
            //            }
            //        }
            //        Parents.Add(BundID);
            //    }
            //    CreateCacheSearchLevelData(LevelName, Parents);
            //}
        }

        private static void CreateCacheSearchLevelData(string LevelName, List<int> Parents)
        {
            lock (forLock)
            {
                BundleParents.Add(AM.GetBundleId("win32/" + LevelName), Parents);
            }
            List<int> newParents = new List<int> { AM.GetBundleId("win32/" + LevelName) };
            EbxAssetEntry refEntry = AM.GetEbxEntry(LevelName);
            if (refEntry == null)
            {
                App.Logger.Log("Could not find LevelData: " + refEntry);
                return;
            }
            EbxAsset refAsset = AM.GetEbx(refEntry);
            foreach (dynamic obj in refAsset.Objects)
            {
                if (obj.GetType().Name == "SubWorldReferenceObjectData")
                {
                    CreateCacheSearchLevelData(LevelName = BunNameCorrection(obj.BundleName), newParents);
                }
            }
        }

        private static void CreateCacheSWBF2CampaignSearch(FrostyTaskWindow task)
        {
            LogUpdate(task, "Caching Campaign Sublevel Bundle Inheritance");
            void CampaignSearch(string Name, List<int> parents)
            {
                EbxAssetEntry refEntry = App.AssetManager.GetEbxEntry(Name);
                EbxAsset refAsset = App.AssetManager.GetEbx(refEntry);
                Parallel.ForEach(refAsset.Objects, obj => 
                {
                    if (obj.GetType().Name == "SubWorldReferenceObjectData")
                        CreateCacheSearchLevelData(BunNameCorrection(((dynamic)obj).BundleName), parents);
                });
            }
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                int forCount = AM.EnumerateEbx(type: "DetachedSetBlueprint").ToList().Count;
                int forIdx = 0;
                Parallel.ForEach(AM.EnumerateEbx(type: "DetachedSetBlueprint"), refEntry => 
                {
                    CampaignSearch(refEntry.Name, refEntry.Bundles.ToList());
                    task.Update(progress: (float)forIdx++ / forCount * 100);
                });
            }
        }

        private static void CreateCacheSWBF2VurSearch(FrostyTaskWindow task)
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                LogUpdate(task, "Caching Blueprint Bundle Inheritance");
                int forCount = AM.EnumerateEbx(type: "VisualUnlockRootAsset").ToList().Count;
                int forIdx = 0;
                Parallel.ForEach(AM.EnumerateEbx(type: "VisualUnlockRootAsset"), refEntry => 
                {
                    if (refEntry.IsAdded)
                        return;
                    dynamic refRoot = App.AssetManager.GetEbx(refEntry, true).RootObject;
                    lock (forLock)
                    {
                        foreach (dynamic obj in refRoot.SkinInfos)
                        {
                            if (obj.ThirdPersonBundle.Parents.Count > 0)
                            {
                                BundleParents.Add(AM.GetBundleId("win32/" + obj.ThirdPersonBundle.Name), new List<int> { AM.GetBundleId("win32/" + obj.ThirdPersonBundle.Parents[0].Name) });
                            }
                            if (obj.FirstPersonBundle.Parents.Count > 0)
                            {
                                BundleParents.Add(AM.GetBundleId("win32/" + obj.FirstPersonBundle.Name), new List<int> { AM.GetBundleId("win32/" + obj.FirstPersonBundle.Parents[0].Name) });
                            }
                        }
                    }
                    task.Update(progress: (forIdx++ / (double)forCount) * 100.0d);
                });
                //foreach (EbxAssetEntry refEntry in App.AssetManager.EnumerateEbx(type: "VisualUnlockRootAsset"))
                //{
                //    if (refEntry.IsAdded)
                //        continue;
                //    dynamic refRoot = App.AssetManager.GetEbx(refEntry, true).RootObject;
                //    foreach (dynamic obj in refRoot.SkinInfos)
                //    {
                //        if (obj.ThirdPersonBundle.Parents.Count > 0)
                //        {
                //            BundleParents.Add(AM.GetBundleId("win32/" + obj.ThirdPersonBundle.Name), new List<int> { AM.GetBundleId("win32/" + obj.ThirdPersonBundle.Parents[0].Name) });
                //        }
                //        if (obj.FirstPersonBundle.Parents.Count > 0)
                //        {
                //            BundleParents.Add(AM.GetBundleId("win32/" + obj.FirstPersonBundle.Name), new List<int> { AM.GetBundleId("win32/" + obj.FirstPersonBundle.Parents[0].Name) });
                //        }
                //    }
                //    task.Update(progress: (forIdx++ / (double)forCount) * 100.0d);
                //}
                List<int> SharedBundles = new List<int>()
                {
                    AM.GetBundleId("win32/default_settings"),
                    AM.GetBundleId("win32/gameplay/bundles/sharedbundles/common/weapons/sharedbundleweapons_common"),
                    AM.GetBundleId("win32/gameplay/wrgameconfiguration"),
                    AM.GetBundleId("win32/gameplay/bundles/sharedbundles/frontend+mp/characters/sharedbundlecharacters_frontend+mp"),
                    AM.GetBundleId("win32/gameplay/bundles/sharedbundles/common/vehicles/sharedbundlevehiclescockpits"),
                };
                foreach (BundleEntry bunEntry in AM.EnumerateBundles(type: BundleType.BlueprintBundle))
                {
                    if (!(ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII) && bunEntry.Name.Contains("/SP/")))
                    {
                        int bunID = AM.GetBundleId(bunEntry);
                        if (!BundleParents.ContainsKey(bunID))
                            BundleParents.Add(bunID, SharedBundles);
                        else
                            BundleParents[bunID].AddRange(SharedBundles);
                    }
                }
            }
        }

        public static string BunNameCorrection(string Name)
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefront || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield1)
                return Name.ToLower();
            else
                return Name;
        }

        #endregion

        #region NetworkRegistries
        private static void FindAllNetRegObjectReferences(FrostyTaskWindow task)
        {
            LogUpdate(task, "Caching Network Registry References");
            int forCount = AM.EnumerateEbx(type: "NetworkRegistryAsset").ToList().Count;
            int ForIdx = 0;

            Parallel.ForEach(AM.EnumerateEbx(type: "NetworkRegistryAsset"), parEntry => 
            {
                if (parEntry.IsAdded)
                    return;

                EbxAsset parAsset = AM.GetEbx(parEntry, true);
                dynamic parRoot = parAsset.RootObject;
                
                lock (forLock)
                {
                    foreach (dynamic pr in parRoot.Objects)
                    {
                        if (pr.Type != PointerRefType.External)
                            continue;
                        EbxAssetEntry refEntry = AM.GetEbxEntry(pr.External.FileGuid);
                        if (refEntry == null)
                            continue;
                        if (!NetRegReferences.ContainsKey(refEntry))
                            NetRegReferences.Add(refEntry, new List<Guid> { });
                        if (!NetRegReferences[refEntry].Contains(pr.External.ClassGuid))
                            NetRegReferences[refEntry].Add(pr.External.ClassGuid);
                    }
                }
                task.Update(progress: (ForIdx++ / (double)forCount) * 100.0d);
            });
            //foreach (EbxAssetEntry parEntry in AM.EnumerateEbx(type: "NetworkRegistryAsset"))
            //{
            //    if (parEntry.IsAdded)
            //        continue;

            //    EbxAsset parAsset = AM.GetEbx(parEntry, true);
            //    dynamic parRoot = parAsset.RootObject;

            //    foreach(dynamic pr in parRoot.Objects)
            //    {
            //        if (pr.Type != PointerRefType.External)
            //            continue;
            //        EbxAssetEntry refEntry = AM.GetEbxEntry(pr.External.FileGuid);
            //        if (refEntry == null)
            //            continue;
            //        if (!NetRegReferences.ContainsKey(refEntry))
            //            NetRegReferences.Add(refEntry, new List<Guid> { });
            //        if (!NetRegReferences[refEntry].Contains(pr.External.ClassGuid))
            //            NetRegReferences[refEntry].Add(pr.External.ClassGuid);
            //    }
            //    task.Update(progress: (ForIdx++ / (double)forCount) * 100.0d);
            //}
        }
        #endregion

        #region MVDB

        private static void CreateCacheMeshVariationDatabase(FrostyTaskWindow task)
        {
            LogUpdate(task, "Caching MeshVariationDatabases");
            uint forCount = AM.GetEbxCount("MeshVariationDatabase");
            uint forIdx = 0;
            Dictionary<uint, EbxAssetEntry> varHashes = AM.EnumerateEbx(type: "ObjectVariation").ToList().ToDictionary(o => (uint)Utils.HashString(o.Name.ToLower()), o => o);

            Parallel.ForEach(AM.EnumerateEbx(type: "MeshVariationDatabase"), parEntry =>
            {
                EbxAsset parAsset = App.AssetManager.GetEbx(parEntry, true);
                dynamic parRoot = parAsset.RootObject;
                int j = 0;
                lock (forLock)
                {
                    foreach (dynamic mvdbEntry in parRoot.Entries)
                    {
                        if (mvdbEntry.Mesh.Type == PointerRefType.External && AM.GetEbxEntry(mvdbEntry.Mesh.External.FileGuid) != null)
                        {
                            EbxAssetEntry refEntry = AM.GetEbxEntry(mvdbEntry.Mesh.External.FileGuid);
                            if (!MeshVariationEntries.ContainsKey(refEntry))
                                MeshVariationEntries.Add(refEntry, new Dictionary<UInt32, MeshVariOriginalData>());

                            if (!MeshVariationEntries[refEntry].ContainsKey(mvdbEntry.VariationAssetNameHash))
                            {
                                MeshVariOriginalData mvData = new MeshVariOriginalData();
                                mvData.BM_MeshVariationDatabaseEntry = new BM_MeshVariationDatabaseEntry(mvdbEntry);
                                mvData.dbLocations = new Dictionary<EbxAssetEntry, int>() { { parEntry, j } };
                                mvData.refGuids = new List<Guid>();

                                foreach (BM_MeshVariationDatabaseMaterial material in mvData.BM_MeshVariationDatabaseEntry.Materials)
                                {
                                    foreach (BM_TextureShaderParameter texParam in material.TextureParameters)
                                    {
                                        if (texParam.Value.Type == PointerRefType.External && texParam.Value.External.FileGuid != null && AM.GetEbxEntry(texParam.Value.External.FileGuid) != null && !mvData.refGuids.Contains(texParam.Value.External.FileGuid))
                                            mvData.refGuids.Add(texParam.Value.External.FileGuid);
                                    }
                                }

                                MeshVariationEntries[refEntry].Add(mvData.BM_MeshVariationDatabaseEntry.VariationAssetNameHash, mvData);

                                if (mvData.BM_MeshVariationDatabaseEntry.VariationAssetNameHash != 0)
                                {
                                    EbxAssetEntry varEntry = varHashes[mvData.BM_MeshVariationDatabaseEntry.VariationAssetNameHash];

                                    if (varEntry != null)
                                    {
                                        string resName = String.Format("{0}/{1}_{2}/shaderblocks_variation/blocks", varEntry.Name.ToLower(), refEntry.DisplayName.ToLower(), (uint)Utils.HashString(refEntry.Name.ToLower()));
                                        ResAssetEntry resEntry = AM.GetResEntry(resName);
                                        if (resEntry == null)
                                            throw new Exception("Could not find shaderblockdepot for variation: " + resName);
                                        if (!ObjectVariationPairs.ContainsKey(varEntry))
                                            ObjectVariationPairs.Add(varEntry, new Dictionary<EbxAssetEntry, ResAssetEntry>() { { refEntry, resEntry } });

                                        if (!ObjectVariationPairs[varEntry].ContainsKey(refEntry))
                                            ObjectVariationPairs[varEntry].Add(refEntry, resEntry);
                                    }
                                }
                            }
                            else
                                MeshVariationEntries[refEntry][mvdbEntry.VariationAssetNameHash].dbLocations.Add(parEntry, j);
                        }
                        j++;
                    }
                    task.Update(progress: (float)forIdx++ / forCount * 100);
                }
            });


            //foreach (EbxAssetEntry ebx in App.AssetManager.EnumerateEbx("MeshVariationDatabase"))
            //{
            //    task.Update(progress: (uint)((forIdx / (float)forCount) * 100));
            //    if (ebx.IsAdded)
            //        continue;
            //    EbxAsset asset = App.AssetManager.GetEbx(ebx, true);
            //    {
            //        dynamic db = asset.RootObject;
            //        int j = 0;

            //        foreach (dynamic mvdbEntry in db.Entries)
            //        {
            //            if (mvdbEntry.Mesh.Type == PointerRefType.External && AM.GetEbxEntry(mvdbEntry.Mesh.External.FileGuid) != null)
            //            {
            //                EbxAssetEntry refEntry = AM.GetEbxEntry(mvdbEntry.Mesh.External.FileGuid);
            //                if (!MeshVariationEntries.ContainsKey(refEntry))
            //                    MeshVariationEntries.Add(refEntry, new Dictionary<UInt32, MeshVariOriginalData>());

            //                if (!MeshVariationEntries[refEntry].ContainsKey(mvdbEntry.VariationAssetNameHash))
            //                {
            //                    MeshVariOriginalData mvData = new MeshVariOriginalData();
            //                    mvData.BM_MeshVariationDatabaseEntry = new BM_MeshVariationDatabaseEntry(mvdbEntry);
            //                    mvData.dbLocations = new Dictionary<EbxAssetEntry, int>() { { ebx, j } };
            //                    mvData.refGuids = new List<Guid>();

            //                    foreach (BM_MeshVariationDatabaseMaterial material in mvData.BM_MeshVariationDatabaseEntry.Materials)
            //                    {
            //                        foreach (BM_TextureShaderParameter texParam in material.TextureParameters)
            //                        {
            //                            if (texParam.Value.Type == PointerRefType.External && texParam.Value.External.FileGuid != null && AM.GetEbxEntry(texParam.Value.External.FileGuid) != null && !mvData.refGuids.Contains(texParam.Value.External.FileGuid))
            //                                mvData.refGuids.Add(texParam.Value.External.FileGuid);
            //                        }
            //                    }

            //                    MeshVariationEntries[refEntry].Add(mvData.BM_MeshVariationDatabaseEntry.VariationAssetNameHash, mvData);

            //                    if (mvData.BM_MeshVariationDatabaseEntry.VariationAssetNameHash != 0)
            //                    {
            //                        EbxAssetEntry varEntry = varHashes[mvData.BM_MeshVariationDatabaseEntry.VariationAssetNameHash];

            //                        if (varEntry != null)
            //                        {
            //                            string resName = String.Format("{0}/{1}_{2}/shaderblocks_variation/blocks", varEntry.Name.ToLower(), refEntry.DisplayName.ToLower(), (uint)Utils.HashString(refEntry.Name.ToLower()));
            //                            ResAssetEntry resEntry = AM.GetResEntry(resName);
            //                            if (resEntry == null)
            //                                throw new Exception("Could not find shaderblockdepot for variation: " + resName);
            //                            if (!ObjectVariationPairs.ContainsKey(varEntry))
            //                                ObjectVariationPairs.Add(varEntry, new Dictionary<EbxAssetEntry, ResAssetEntry>() { { refEntry, resEntry } });

            //                            if (!ObjectVariationPairs[varEntry].ContainsKey(refEntry))
            //                                ObjectVariationPairs[varEntry].Add(refEntry, resEntry);
            //                        }
            //                    }
            //                }
            //                else
            //                    MeshVariationEntries[refEntry][mvdbEntry.VariationAssetNameHash].dbLocations.Add(ebx, j);
            //            }
            //            j++;
            //        }
            //    }
            //    forIdx++;
            //}
        }

        #endregion

        #region Asset Logging

        public static void LogAssetData(FrostyTaskWindow task)
        {

            LogUpdate(task, "Caching Asset Data");
            int forCount = AM.EnumerateEbx().ToList().Count;
            int forIdx = 0;

            Dictionary<string, AssetLogger> loggerExtensions = new Dictionary<string, AssetLogger>();
            loggerExtensions.Add("null", new AssetLogger());
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(AssetLogger)))
                {
                    var extension = (AssetLogger)Activator.CreateInstance(type);
                    loggerExtensions.Add(extension.AssetType, extension);
                }
            }

            Parallel.ForEach(AM.EnumerateEbx(), parEntry => 
            {
                if (parEntry.IsAdded || parEntry.Type == "NetworkRegistryAsset" || parEntry.Type == "MeshVariationDatabase")
                    return;

                string key = "null";
                foreach (string typekey in loggerExtensions.Keys)
                {
                    if (TypeLibrary.IsSubClassOf(parEntry.Type, typekey))
                    {
                        key = typekey;
                        break;
                    }
                }
                EbxAsset parAsset = App.AssetManager.GetEbx(parEntry, true);
                AssetData parData = loggerExtensions[key].GetAssetData(parEntry, parAsset);

                lock (forLock)
                {
                    if (NetRegReferences.ContainsKey(parEntry))
                    {
                        parData.Objects = new List<EbxImportReference>();
                        foreach (Guid guid in NetRegReferences[parEntry])
                        {
                            parData.Objects.Add(new EbxImportReference() { FileGuid = parEntry.Guid, ClassGuid = guid });
                            dynamic obj = parAsset.GetObject(guid);
                            if (!NetRegReferenceTypes.Contains(obj.GetType().Name))
                                NetRegReferenceTypes.Add(obj.GetType().Name);
                        }
                    }

                    if (key != "null" || parData.EbxReferences.Count > 0 || parData.Res.Count > 0 || parData.Chunks.Count > 0 || (parData.Objects != null && parData.Objects.Count > 0))
                        UnmodifiedAssetData.Add(parEntry, parData);

                    task.Update(progress: (float)forIdx++ / forCount * 100);
                }
            });

            //foreach (EbxAssetEntry parEntry in AM.EnumerateEbx())
            //{
            //    task.Update(String.Format("{0}/{1}: {2}", stageIdx, gameStageCounts[ProfilesLibrary.DataVersion], parEntry.DisplayName), ((forIdx++ / (double)forCount) * 100.0d) + 0.0d);
            //    if (parEntry.IsAdded || parEntry.Type == "NetworkRegistryAsset" || parEntry.Type == "MeshVariationDatabase")
            //        continue;

            //    //if (UnmodifiedAssetData.Count > 100)
            //    //    break;

            //    string key = "null";
            //    foreach (string typekey in loggerExtensions.Keys)
            //    {
            //        if (TypeLibrary.IsSubClassOf(parEntry.Type, typekey))
            //        {
            //            key = typekey;
            //            break;
            //        }
            //    }
            //    EbxAsset parAsset = App.AssetManager.GetEbx(parEntry, true);
            //    AssetData parData = loggerExtensions[key].GetAssetData(parEntry, parAsset);
            //    if (NetRegReferences.ContainsKey(parEntry))
            //    {
            //        parData.Objects = new List<EbxImportReference>();
            //        foreach (Guid guid in NetRegReferences[parEntry])
            //        {
            //            parData.Objects.Add(new EbxImportReference() { FileGuid = parEntry.Guid, ClassGuid = guid});
            //            dynamic obj = parAsset.GetObject(guid);
            //            if (!NetRegReferenceTypes.Contains(obj.GetType().Name))
            //                NetRegReferenceTypes.Add(obj.GetType().Name);
            //        }
            //    }

            //    if (key != "null" || parData.EbxReferences.Count > 0 || parData.Res.Count > 0 || parData.Chunks.Count > 0 || (parData.Objects != null && parData.Objects.Count > 0))
            //        UnmodifiedAssetData.Add(parEntry, parData);

            //}
        }

        #endregion

        private static void ExportingCache(string name)
        {
            List<ResAssetEntry> notLoggedRes = AM.EnumerateRes().ToList();
            foreach (AssetData data in UnmodifiedAssetData.Values)
                foreach (ResAssetEntry resEntry in data.Res)
                    if (notLoggedRes.Contains(resEntry))
                        notLoggedRes.Remove(resEntry);
            foreach (Dictionary<EbxAssetEntry, ResAssetEntry> pair in ObjectVariationPairs.Values)
                foreach (ResAssetEntry resEntry in pair.Values)
                    if (notLoggedRes.Contains(resEntry))
                        notLoggedRes.Remove(resEntry);

            using (NativeWriter writer = new NativeWriter(new FileStream($"{name}_bundlemanager_cachevisual.txt", FileMode.Create)))
            {
                writer.WriteLine("Cache Generated With Bundle Manager Version: " + BM_Version.ToString());
                writer.WriteLine("Logged Bundles:");
                foreach (int BunID in BundleParents.Keys)
                {
                    writer.WriteLine(String.Format("{0}{1} ({2} parents):", new string('\t', 1), AM.GetBundleEntry(BunID).Name, BundleParents[BunID].Count));
                    foreach (int bunParID in BundleParents[BunID])
                        writer.WriteLine(String.Format("{0}{1}", new string('\t', 2), AM.GetBundleEntry(bunParID).Name));
                }

                writer.WriteLine("Network Registry Types:");
                foreach (string NetRegType in NetRegReferenceTypes)
                    writer.WriteLine(String.Format("{0}{1}", new string('\t', 1), NetRegType));

                writer.WriteLine("MeshVariationDatabase Entries:");
                foreach (EbxAssetEntry mvdbEntry in MeshVariationEntries.Keys)
                {
                    writer.WriteLine(String.Format("{0}{1} ({2} variations)", new string('\t', 1), AM.GetEbxEntry(mvdbEntry.Guid).Name, MeshVariationEntries[mvdbEntry].Count));
                    foreach (UInt32 var in MeshVariationEntries[mvdbEntry].Keys)
                    {
                        //dynamic mvdbObj = MeshVariationEntries[mvdbEntry][var].BM_MeshVariationDatabaseEntry;
                        BM_MeshVariationDatabaseEntry mvdbObj = MeshVariationEntries[mvdbEntry][var].BM_MeshVariationDatabaseEntry;
                        if (mvdbObj.Materials.Count > 0 && var != 0 && AM.GetEbxEntry(mvdbObj.Materials[0].MaterialVariation.External.FileGuid) != null)
                            writer.WriteLine(String.Format("{0}VariationAssetNameHash: {1} ({2})", new string('\t', 2), var, AM.GetEbxEntry(mvdbObj.Materials[0].MaterialVariation.External.FileGuid).Name));
                        else
                            writer.WriteLine(String.Format("{0}VariationAssetNameHash: {1}", new string('\t', 2), var));
                        writer.WriteLine(String.Format("{0}Materials ({1}):", new string('\t', 3), mvdbObj.Materials.Count));
                        int idx = 0;
                        foreach (BM_MeshVariationDatabaseMaterial mvMat in mvdbObj.Materials)
                        {
                            writer.WriteLine(String.Format("{0}Material {1}:", new string('\t', 4), idx++));
                            writer.WriteLine(String.Format("{0}MaterialGuid: {1}", new string('\t', 5), mvMat.Material.External.ClassGuid));
                            writer.WriteLine(String.Format("{0}MaterialVariationAssetGuid: {1}", new string('\t', 5), mvMat.MaterialVariation.External.FileGuid));
                            writer.WriteLine(String.Format("{0}MaterialVariationClassGuid: {1}", new string('\t', 5), mvMat.MaterialVariation.External.ClassGuid));
                            writer.WriteLine(String.Format("{0}SurfaceShaderId: {1}", new string('\t', 5), mvMat.SurfaceShaderId));
                            writer.WriteLine(String.Format("{0}SurfaceShaderGuid: {1}", new string('\t', 5), mvMat.SurfaceShaderGuid));
                            writer.WriteLine(String.Format("{0}MaterialId: {1}", new string('\t', 5), mvMat.MaterialId));

                            writer.WriteLine(String.Format("{0}Texture Parameters ({1}):", new string('\t', 5), mvMat.TextureParameters.Count));
                            int idx2 = 0;
                            foreach (BM_TextureShaderParameter texParam in mvMat.TextureParameters)
                            {
                                writer.WriteLine(String.Format("{0}Texture Parameter {1}:", new string('\t', 6), idx2++));
                                writer.WriteLine(String.Format("{0}ParameterName: {1}", new string('\t', 7), texParam.ParameterName));
                                if (AM.GetEbxEntry(texParam.Value.External.FileGuid) != null)
                                    writer.WriteLine(String.Format("{0}Value: {1}", new string('\t', 7), AM.GetEbxEntry(texParam.Value.External.FileGuid).Name));
                                else
                                    writer.WriteLine(String.Format("{0}Value: {1}", new string('\t', 7), "Null Reference"));
                            }
                        }

                        writer.WriteLine(String.Format("{0}DB Locations ({1}):", new string('\t', 3), MeshVariationEntries[mvdbEntry][var].dbLocations.Count));
                        foreach (EbxAssetEntry refEntry in MeshVariationEntries[mvdbEntry][var].dbLocations.Keys)
                            writer.WriteLine(String.Format("{0}{1}[{2}]:", new string('\t', 4), refEntry.Name, MeshVariationEntries[mvdbEntry][var].dbLocations[refEntry]));

                        writer.WriteLine(String.Format("{0}Ebx References ({1}):", new string('\t', 3), MeshVariationEntries[mvdbEntry][var].refGuids.Count));
                        foreach (Guid guid in MeshVariationEntries[mvdbEntry][var].refGuids)
                            writer.WriteLine(String.Format("{0}{1}:", new string('\t', 4), AM.GetEbxEntry(guid).Name));
                    }
                }

                writer.WriteLine("ObjectVariation Pairs:");
                foreach (EbxAssetEntry refEntry in ObjectVariationPairs.Keys)
                    writer.WriteLine(String.Format("{0}{1} - {2} - {3}", new string('\t', 1), refEntry.Name, String.Join(",\n\t\t", ObjectVariationPairs[refEntry].Select(o => o.Key.Name)), String.Join(",\n\t\t", ObjectVariationPairs[refEntry].Select(o => o.Value.Name))));

                writer.WriteLine("Asset Metadata:");
                foreach (EbxAssetEntry parEntry in UnmodifiedAssetData.Keys)
                {
                    writer.WriteLine(String.Format("{0}{1}", new string('\t', 1), parEntry.Name));

                    //ebx
                    if (UnmodifiedAssetData[parEntry].EbxReferences.Count > 0)
                        writer.WriteLine(String.Format("{0}Ebx References ({1}):", new string('\t', 2), UnmodifiedAssetData[parEntry].EbxReferences.Count));
                    foreach (EbxAssetEntry refEntry in UnmodifiedAssetData[parEntry].EbxReferences)
                        writer.WriteLine(String.Format("{0}{1}", new string('\t', 3), refEntry.Name));

                    //res
                    if (UnmodifiedAssetData[parEntry].Res.Count > 0)
                        writer.WriteLine(String.Format("{0}Res References ({1}):", new string('\t', 2), UnmodifiedAssetData[parEntry].Res.Count));
                    foreach (ResAssetEntry resEntry in UnmodifiedAssetData[parEntry].Res)
                        writer.WriteLine(String.Format("{0}{1}", new string('\t', 3), resEntry.Name));

                    //chunk
                    if (UnmodifiedAssetData[parEntry].Chunks.Count > 0)
                        writer.WriteLine(String.Format("{0}Chunk References ({1}):", new string('\t', 2), UnmodifiedAssetData[parEntry].Chunks.Count));
                    foreach (ChunkAssetEntry chunkEntry in UnmodifiedAssetData[parEntry].Chunks)
                        writer.WriteLine(String.Format("{0}{1}", new string('\t', 3), chunkEntry.Name));

                    //Network Registry Objects
                    if (UnmodifiedAssetData[parEntry].Objects != null)
                    {
                        if (UnmodifiedAssetData[parEntry].Objects.Count > 0)
                            writer.WriteLine(String.Format("{0}Network Registry References ({1}):", new string('\t', 2), UnmodifiedAssetData[parEntry].Objects.Count));
                        foreach (EbxImportReference netregRef in UnmodifiedAssetData[parEntry].Objects)
                            writer.WriteLine(String.Format("{0}{1}", new string('\t', 3), netregRef.ClassGuid));
                    }
                }

                writer.WriteLine("Res with no found ebx");
                foreach(ResAssetEntry resEntry in notLoggedRes)
                    writer.WriteLine(String.Format("{0}{1}", new string('\t', 1), resEntry.Name));
            }

            using (NativeWriter writer = new NativeWriter(new FileStream($"{name}_bundlemanager.cache", FileMode.Create)))
            {
                writer.WriteNullTerminatedString("MopMagicMopMagic"); //Magic
                writer.Write(BM_Version);
                writer.Write(BundleParents.Keys.Count);
                writer.Write(NetRegReferenceTypes.Count);
                writer.Write(MeshVariationEntries.Count);
                writer.Write(UnmodifiedAssetData.Count);
                writer.Write(ObjectVariationPairs.Count);


                foreach (int BunID in BundleParents.Keys)
                {
                    writer.Write(BunID);
                    writer.Write(BundleParents[BunID].Count);
                    foreach (int BunID2 in BundleParents[BunID])
                        writer.Write(BunID2);
                }

                foreach (string NetRegType in NetRegReferenceTypes)
                    writer.WriteNullTerminatedString(NetRegType);

                foreach (EbxAssetEntry mvdbEntry in MeshVariationEntries.Keys)
                {
                    writer.Write(mvdbEntry.Guid);
                    writer.Write(((dynamic)MeshVariationEntries[mvdbEntry].First().Value.BM_MeshVariationDatabaseEntry).Mesh.External.ClassGuid);
                    writer.Write(MeshVariationEntries[mvdbEntry].Keys.Count);
                    foreach (UInt32 var in MeshVariationEntries[mvdbEntry].Keys)
                    {
                        BM_MeshVariationDatabaseEntry mvdbObj = MeshVariationEntries[mvdbEntry][var].BM_MeshVariationDatabaseEntry;
                        writer.Write(var);
                        writer.Write(mvdbObj.Materials.Count);
                        foreach (BM_MeshVariationDatabaseMaterial mvMat in mvdbObj.Materials)
                        {
                            writer.Write(mvMat.Material.External.ClassGuid);
                            if (var != 0)
                            {
                                writer.Write(mvMat.MaterialVariation.External.FileGuid);
                                writer.Write(mvMat.MaterialVariation.External.ClassGuid);
                            }
                            writer.Write(mvMat.SurfaceShaderId);
                            writer.Write(mvMat.SurfaceShaderGuid);
                            writer.Write(mvMat.MaterialId);
                            writer.Write(mvMat.TextureParameters.Count);
                            foreach (BM_TextureShaderParameter texParam in mvMat.TextureParameters)
                            {
                                writer.WriteNullTerminatedString(texParam.ParameterName);
                                writer.Write(texParam.Value.External.FileGuid);
                                writer.Write(texParam.Value.External.ClassGuid);
                            }
                        }

                        writer.Write(MeshVariationEntries[mvdbEntry][var].dbLocations.Keys.Count);
                        foreach (EbxAssetEntry refEntry in MeshVariationEntries[mvdbEntry][var].dbLocations.Keys)
                        {
                            writer.Write(refEntry.Guid);
                            writer.Write(MeshVariationEntries[mvdbEntry][var].dbLocations[refEntry]);
                        }

                        writer.Write(MeshVariationEntries[mvdbEntry][var].refGuids.Count);
                        foreach (Guid guid in MeshVariationEntries[mvdbEntry][var].refGuids)
                            writer.Write(guid);
                    }
                }

                foreach (EbxAssetEntry parEntry in UnmodifiedAssetData.Keys)
                {
                    writer.Write(parEntry.Guid);
                    AssetData parData = UnmodifiedAssetData[parEntry];
                    writer.Write(parData.EbxReferences.Count);
                    foreach (EbxAssetEntry refEntry in parData.EbxReferences)
                        writer.Write(refEntry.Guid);
                    writer.Write(parData.Res.Count);
                    foreach (ResAssetEntry refEntry in parData.Res)
                        writer.Write(refEntry.ResRid);
                    writer.Write(parData.Chunks.Count);
                    foreach (ChunkAssetEntry refEntry in parData.Chunks)
                        writer.Write(refEntry.Id);
                    if (parData.Objects != null)
                    {
                        writer.Write(parData.Objects.Count);
                        foreach (EbxImportReference reference in parData.Objects)
                            writer.Write(reference.ClassGuid);
                    }
                    else
                        writer.Write(0);
                }

                foreach (KeyValuePair<EbxAssetEntry, Dictionary<EbxAssetEntry, ResAssetEntry>> pair in ObjectVariationPairs)
                {
                    writer.Write(pair.Key.Guid);
                    writer.Write(pair.Value.Count);
                    foreach (KeyValuePair<EbxAssetEntry, ResAssetEntry> pair2 in pair.Value)
                    {
                        writer.Write(pair2.Key.Guid);
                        writer.Write(pair2.Value.ResRid);
                    }
                }

                BundleParents.Clear();
                NetRegReferenceTypes.Clear();
                MeshVariationEntries.Clear();
                UnmodifiedAssetData.Clear();
                ObjectVariationPairs.Clear();

            }
        }

        public static bool ReadingCache(FrostyTaskWindow task)
        {
            if (!File.Exists($"{App.FileSystemManager.CacheName}_bundlemanager.cache"))
                return false;
            //stopWatch.Start();
            using (NativeReader reader = new NativeReader(new FileStream($"{App.FileSystemManager.CacheName}_bundlemanager.cache", FileMode.Open, FileAccess.Read)))
            {
                task.Update("loading Cache", 00.0d);
                if (reader.ReadNullTerminatedString() != "MopMagicMopMagic" || reader.ReadInt() != BM_Version)
                    return false;

                int bunParCount = reader.ReadInt();
                int netRegTypesCount = reader.ReadInt();
                int mvdbEntriesCount = reader.ReadInt();
                int unmodAssetDataCount = reader.ReadInt();
                int objectvariationCount = reader.ReadInt();

                for (int i = 0; i < bunParCount; i++)
                {
                    int bunID = reader.ReadInt();
                    List<int> bunParents = new List<int>();
                    int bunCount = reader.ReadInt();
                    for (int i2 = 0; i2 < bunCount; i2++) { bunParents.Add(reader.ReadInt()); }
                    BundleParents.Add(bunID, bunParents);
                }

                for (int i = 0; i < netRegTypesCount; i++) { NetRegReferenceTypes.Add(reader.ReadNullTerminatedString()); }


                for (int i = 0; i < mvdbEntriesCount; i++)
                {
                    EbxAssetEntry mvdbEntry = AM.GetEbxEntry(reader.ReadGuid());
                    MeshVariationEntries.Add(mvdbEntry, new Dictionary<uint, MeshVariOriginalData>());

                    Guid meshClassGuid = reader.ReadGuid();
                    int varCount = reader.ReadInt();

                    for (int i2 = 0; i2 < varCount; i2++)
                    {
                        BM_MeshVariationDatabaseEntry mv_Entry = new BM_MeshVariationDatabaseEntry(reader, new PointerRef(new EbxImportReference() { FileGuid = mvdbEntry.Guid, ClassGuid = meshClassGuid}));
                        MeshVariOriginalData mvdbData = new MeshVariOriginalData() { BM_MeshVariationDatabaseEntry = mv_Entry, dbLocations = new Dictionary<EbxAssetEntry, int>(), refGuids = new List<Guid>() };

                        int dbCount = reader.ReadInt();
                        for (int i3 = 0; i3 < dbCount; i3++)
                            mvdbData.dbLocations.Add(AM.GetEbxEntry(reader.ReadGuid()), reader.ReadInt());

                        int refCount = reader.ReadInt();
                        for (int i3 = 0; i3 < refCount; i3++)
                            mvdbData.refGuids.Add(reader.ReadGuid());

                        MeshVariationEntries[mvdbEntry].Add(mv_Entry.VariationAssetNameHash, mvdbData);

                    }

                    ////task.Update(progress: 30.0d + 30.0d * (i/ (double)mvdbEntriesCount));
                    //EbxAssetEntry mvdbEntry = AM.GetEbxEntry(reader.ReadGuid());
                    //MeshVariationEntries.Add(mvdbEntry, new Dictionary<uint, MeshVariOriginalData>());


                    //Guid meshClassGuid = reader.ReadGuid();
                    //int varCount = reader.ReadInt();

                    //for (int i2 = 0; i2 < varCount; i2++)
                    //{
                    //    dynamic mvdbObject = TypeLibrary.CreateObject("MeshVariationDatabaseEntry");
                    //    mvdbObject.Mesh = new PointerRef(new EbxImportReference() { FileGuid = mvdbEntry.Guid, ClassGuid = meshClassGuid });
                    //    UInt32 var = reader.ReadUInt();
                    //    int matCount = reader.ReadInt();

                    //    for (int i3 = 0; i3 < matCount; i3++)
                    //    {
                    //        dynamic mvdbMaterial = TypeLibrary.CreateObject("MeshVariationDatabaseMaterial");
                    //        mvdbMaterial.Material = new PointerRef(new EbxImportReference() { FileGuid = mvdbEntry.Guid, ClassGuid = reader.ReadGuid() });
                    //        if (var != 0)
                    //            mvdbMaterial.MaterialVariation = new PointerRef(new EbxImportReference() { FileGuid = reader.ReadGuid(), ClassGuid = reader.ReadGuid() });
                    //        mvdbMaterial.SurfaceShaderId = reader.ReadUInt();
                    //        mvdbMaterial.SurfaceShaderGuid = reader.ReadGuid();
                    //        mvdbMaterial.MaterialId = reader.ReadLong();

                    //        int texParamCount = reader.ReadInt();
                    //        for (int i4 = 0; i4 < texParamCount; i4++)
                    //        {
                    //            dynamic mvdbTexParam = TypeLibrary.CreateObject("TextureShaderParameter");
                    //            mvdbTexParam.ParameterName = reader.ReadNullTerminatedString();
                    //            mvdbTexParam.Value = new PointerRef(new EbxImportReference() { FileGuid = reader.ReadGuid(), ClassGuid = reader.ReadGuid()});
                    //            mvdbMaterial.TextureParameters.Add(mvdbTexParam);
                    //        }
                    //        mvdbObject.Materials.Add(mvdbMaterial);
                    //    }

                    //    MeshVariOriginalData mvdbData = new MeshVariOriginalData() { MeshVariationDatabaseEntry = mvdbObject, dbLocations = new Dictionary<EbxAssetEntry, int>(), refGuids = new List<Guid>() };

                    //    int dbCount = reader.ReadInt();
                    //    for (int i3 = 0; i3 < dbCount; i3++)
                    //        mvdbData.dbLocations.Add(AM.GetEbxEntry(reader.ReadGuid()), reader.ReadInt());

                    //    int refCount = reader.ReadInt();
                    //    for (int i3 = 0; i3 < refCount; i3++)
                    //        mvdbData.refGuids.Add(reader.ReadGuid());


                    //    MeshVariationEntries[mvdbEntry].Add(var, mvdbData);

                    //}
                }

                for (int i = 0; i < unmodAssetDataCount; i++)
                {
                    EbxAssetEntry parEntry = AM.GetEbxEntry(reader.ReadGuid());
                    AssetData parData = new AssetData { EbxReferences = new List<EbxAssetEntry>(), Chunks = new List<ChunkAssetEntry>(), Res = new List<ResAssetEntry>() };
                    int count = reader.ReadInt();
                    for (int i2 = 0; i2 < count; i2++)
                        parData.EbxReferences.Add(AM.GetEbxEntry(reader.ReadGuid()));
                    count = reader.ReadInt();
                    for (int i2 = 0; i2 < count; i2++)
                        parData.Res.Add(AM.GetResEntry(reader.ReadULong()));
                    count = reader.ReadInt();
                    for (int i2 = 0; i2 < count; i2++)
                        parData.Chunks.Add(AM.GetChunkEntry(reader.ReadGuid()));
                    count = reader.ReadInt();
                    if (count != 0)
                    {
                        parData.Objects = new List<EbxImportReference>();
                        for (int i2 = 0; i2 < count; i2++)
                            parData.Objects.Add(new EbxImportReference() { FileGuid = parEntry.Guid, ClassGuid = reader.ReadGuid() });
                    }
                    UnmodifiedAssetData.Add(parEntry, parData);
                }

                for (int i = 0; i < objectvariationCount; i++)
                {
                    EbxAssetEntry varEntry = AM.GetEbxEntry(reader.ReadGuid());
                    int varMeshCount = reader.ReadInt();
                    Dictionary<EbxAssetEntry, ResAssetEntry> meshVars = new Dictionary<EbxAssetEntry, ResAssetEntry>();
                    for (int i2 = 0; i2 < varMeshCount; i2++)
                    {
                        EbxAssetEntry varMeshEntry = AM.GetEbxEntry(reader.ReadGuid());
                        ResAssetEntry resAssetEntry = AM.GetResEntry(reader.ReadULong());
                        meshVars.Add(varMeshEntry, resAssetEntry);
                    }
                    ObjectVariationPairs.Add(varEntry, meshVars);
                }

                task.Update("", 00.0d);
            }

            //stopWatch.Stop();
            //App.Logger.Log(string.Format("Bundle Manager Cache read in {0} seconds", stopWatch.Elapsed));

            //ExportingCache(App.FileSystem.CacheName + "_temp");

            IsLoaded = true;
            return true;
        }


    }
}
