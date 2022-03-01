using System;
using System.Collections.Generic;
using FrostySdk.Ebx;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using Frosty.Core.Windows;

namespace Frosty.Core.Viewport
{
    public class MeshVariationMaterial
    {
        public Guid MaterialGuid { get; private set; }
        public Guid MaterialVariationAssetGuid { get; private set; }
        public Guid MaterialVariationClassGuid { get; private set; }
        public object TextureParameters;

        public MeshVariationMaterial(dynamic ebxEntry)
        {
            MaterialGuid = ((PointerRef)ebxEntry.Material).External.ClassGuid;
            PointerRef MaterialVariation = (PointerRef)ebxEntry.MaterialVariation;
            MaterialVariationAssetGuid = MaterialVariation.External.FileGuid;
            MaterialVariationClassGuid = MaterialVariation.External.ClassGuid;

            // @hack
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield1 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield5 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20
             && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedHeat)
            {
                TextureParameters = ebxEntry.TextureParameters;
            }
        }
    }

    public class MeshVariation
    {
        public Guid MeshGuid { get; private set; }
        public uint AssetNameHash { get; private set; }
        public List<MeshVariationMaterial> Materials { get; } = new List<MeshVariationMaterial>();
        public List<Tuple<EbxImportReference, int>> DbLocations { get; private set; } = new List<Tuple<EbxImportReference, int>>();

        public MeshVariation(dynamic ebxEntry)
        {
            MeshGuid = ((PointerRef)ebxEntry.Mesh).External.FileGuid;

            try
            {
                AssetNameHash = ebxEntry.VariationAssetNameHash;
            }
            catch (Exception)
            {
                AssetNameHash = 1;
            }
            foreach (dynamic material in ebxEntry.Materials)
            {
                Materials.Add(new MeshVariationMaterial(material));
            }
        }

        public MeshVariationMaterial GetMaterial(Guid materialGuid)
        {
            foreach (MeshVariationMaterial material in Materials)
            {
                if (material.MaterialGuid == materialGuid)
                    return material;
            }
            return null;
        }

        public void AddVariationDb(Guid fileGuid, Guid classGuid, int idx)
        {
            DbLocations.Add(new Tuple<EbxImportReference, int>(new EbxImportReference() { FileGuid = fileGuid, ClassGuid = classGuid }, idx));
        }
    }

    public class MeshVariationDbEntry
    {
        public const int ROOT_VARIATION = 0;

        public Guid Guid { get; private set; }
        public Dictionary<uint, MeshVariation> Variations { get; private set; } = new Dictionary<uint, MeshVariation>();

        public MeshVariationDbEntry(Guid inGuid)
        {
            Guid = inGuid;
        }

        public void Add(Guid fileGuid, Guid classGuid, int idx, dynamic obj)
        {
            uint assetNameHash = obj.VariationAssetNameHash;
            if (!Variations.ContainsKey(assetNameHash))
                Variations.Add(assetNameHash, new MeshVariation(obj));
            Variations[assetNameHash].AddVariationDb(fileGuid, classGuid, idx);
        }

        public MeshVariation GetVariation(uint hash) => !Variations.ContainsKey(hash) ? null : Variations[hash];

        public bool ContainsVariation(uint hash) => Variations.ContainsKey(hash);
    }

    public static class MeshVariationDb
    {
        public static bool IsLoaded { get; set; }
        public static Dictionary<Guid, MeshVariationDbEntry> Entries { get => entries; set { entries = value; } }

        private static Dictionary<Guid, MeshVariationDbEntry> entries = new Dictionary<Guid, MeshVariationDbEntry>();

        public static void LoadVariations(FrostyTaskWindow task)
        {
            bool RenderPerformanceLoadingEnabled = Config.Get<bool>("RenderPerformanceLoadingEnabled", true);

            if (!RenderPerformanceLoadingEnabled)
            {
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20
             || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat)
                {
                    IsLoaded = true;
                    return;
                }

                uint totalCount = App.AssetManager.GetEbxCount("MeshVariationDatabase");
                uint index = 0;

                entries.Clear();
                task.Update("Loading variation databases");

                foreach (EbxAssetEntry ebx in App.AssetManager.EnumerateEbx("MeshVariationDatabase"))
                {
                    uint progress = (uint)((index / (float)totalCount) * 100);
                    task.Update(progress: progress);

                    EbxAsset asset = App.AssetManager.GetEbx(ebx);
                    {
                        dynamic db = asset.RootObject;
                        int j = 0;

                        foreach (dynamic v in db.Entries)
                        {
                            try
                            {
                                Guid meshGuid = ((PointerRef)v.Mesh).External.FileGuid;
                                if (!entries.ContainsKey(meshGuid))
                                    entries.Add(meshGuid, new MeshVariationDbEntry(meshGuid));
                                entries[meshGuid].Add(ebx.Guid, asset.RootInstanceGuid, j, v);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.ToString());
                                System.Diagnostics.Debugger.Break();
                            }

                            j++;
                        }
                    }
                    index++;
                }

                IsLoaded = true;
            }
            else
            {
                foreach (EbxAssetEntry ebx in App.AssetManager.EnumerateEbx("MeshVariationDatabase"))
                {
                    if (ebx.ContainsDependency(App.SelectedAsset.Guid))
                    {
                        EbxAsset asset = App.AssetManager.GetEbx(ebx);
                        {
                            dynamic db = asset.RootObject;
                            int j = 0;

                            foreach (dynamic v in db.Entries)
                            {
                                try
                                {
                                    Guid meshGuid = ((PointerRef)v.Mesh).External.FileGuid;
                                    if (!entries.ContainsKey(meshGuid))
                                        entries.Add(meshGuid, new MeshVariationDbEntry(meshGuid));
                                    entries[meshGuid].Add(ebx.Guid, asset.RootInstanceGuid, j, v);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    System.Diagnostics.Debugger.Break();
                                }

                                j++;
                            }
                        }
                    }
                }
            }
        }
        public static void LoadVariations(EbxAssetEntry ebxEntry)
        {
            foreach (EbxAssetEntry ebx in App.AssetManager.EnumerateEbx("MeshVariationDatabase"))
            {
                if (ebx.ContainsDependency(ebxEntry.Guid))
                {
                    EbxAsset asset = App.AssetManager.GetEbx(ebx);
                    {
                        dynamic db = asset.RootObject;
                        int j = 0;

                        foreach (dynamic v in db.Entries)
                        {
                            try
                            {
                                Guid meshGuid = ((PointerRef)v.Mesh).External.FileGuid;
                                if (!entries.ContainsKey(meshGuid))
                                    entries.Add(meshGuid, new MeshVariationDbEntry(meshGuid));
                                entries[meshGuid].Add(ebx.Guid, asset.RootInstanceGuid, j, v);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.ToString());
                                System.Diagnostics.Debugger.Break();
                            }

                            j++;
                        }
                    }
                }
            }
        }

        public static MeshVariationDbEntry GetVariations(Guid meshGuid)
        {
            if (entries.Count == 0 || !entries.ContainsKey(meshGuid))
                return null;
            return entries[meshGuid];
        }

        public static MeshVariationDbEntry GetVariations(string name)
        {
            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(name);
            return entry == null ? null : GetVariations(entry.Guid);
        }

        public static IEnumerable<MeshVariation> FindVariations(uint hash)
        {
            foreach (MeshVariationDbEntry entry in entries.Values)
            {
                if (entry.ContainsVariation(hash))
                    yield return entry.GetVariation(hash);
            }
        }
    }
}
