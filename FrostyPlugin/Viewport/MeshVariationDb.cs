using System;
using System.Collections.Generic;
using FrostySdk.Ebx;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using Frosty.Core.Windows;
using System.IO;
using System.Linq;
using FrostySdk.Managers.Entries;

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
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield1, ProfileVersion.Anthem,
                ProfileVersion.Battlefield5, ProfileVersion.Fifa20,
                ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.NeedForSpeedHeat,
                ProfileVersion.Fifa21, ProfileVersion.Madden22,
                ProfileVersion.Fifa22, ProfileVersion.Battlefield2042,
                ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                TextureParameters = ebxEntry.TextureParameters;
            }
        }
        public MeshVariationMaterial(Guid input1, Guid input2, Guid input3, List<dynamic> input4)
        {
            MaterialGuid = input1;
            MaterialVariationAssetGuid = input2;
            MaterialVariationClassGuid = input3;
            TextureParameters = input4;
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
        public MeshVariation(uint hash, Guid guid, List<Tuple<EbxImportReference, int>> tups, List<MeshVariationMaterial> materials)
        {
            AssetNameHash = hash;
            MeshGuid = guid;
            DbLocations = tups;
            Materials = materials;
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
        public static Dictionary<Guid, MeshVariationDbEntry> ModifiedEntries { get => modifiedentries; set { modifiedentries = value; } }

        private static Dictionary<Guid, MeshVariationDbEntry> modifiedentries = new Dictionary<Guid, MeshVariationDbEntry>();

        public static void LoadVariations(FrostyTaskWindow task)
        {
            int mvdbVersion = 1;


            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19 , ProfileVersion.Madden20,
                ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                ProfileVersion.NeedForSpeedHeat,
                ProfileVersion.Fifa21, ProfileVersion.Madden22,
                ProfileVersion.Fifa22, ProfileVersion.Battlefield2042,
                ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                IsLoaded = true;
                return;
            }

            uint totalCount = App.AssetManager.GetEbxCount("MeshVariationDatabase");
            uint index = 0;

            entries.Clear();
            task.Update("Loading variation databases");

            string cache = System.AppDomain.CurrentDomain.BaseDirectory + @"\Caches\" + ProfilesLibrary.CacheName + "_mvdb.cache";
            bool generateMVDB = true;

            if (File.Exists(cache))
            {
                using (NativeReader reader = new NativeReader(new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + @"\Caches\" + ProfilesLibrary.CacheName + "_mvdb.cache", FileMode.Open)))
                {
                    if (reader.ReadInt() == mvdbVersion)
                    {
                        generateMVDB = false;
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Guid guid = reader.ReadGuid();
                            MeshVariationDbEntry mvEntry = new MeshVariationDbEntry(guid);
                            int varCount = reader.ReadInt();
                            for (int j = 0; j < varCount; j++)
                            {
                                uint var = reader.ReadUInt();
                                uint nameHash = reader.ReadUInt();
                                Guid meshGuid = reader.ReadGuid();

                                List<Tuple<EbxImportReference, int>> tuples = new List<Tuple<EbxImportReference, int>>();
                                int dbCount = reader.ReadInt();
                                for (int i2 = 0; i2 < dbCount; i2++)
                                {
                                    EbxImportReference ebxRef = new EbxImportReference() { FileGuid = reader.ReadGuid(), ClassGuid = reader.ReadGuid() };
                                    int tup2 = reader.ReadInt();
                                    tuples.Add(new Tuple<EbxImportReference, int>(ebxRef, tup2));
                                }
                                List<MeshVariationMaterial> materials = new List<MeshVariationMaterial>();
                                int matCount = reader.ReadInt();
                                for (int i2 = 0; i2 < matCount; i2++)
                                {
                                    Guid MaterialGuid = reader.ReadGuid();
                                    Guid MaterialVariationAssetGuid = reader.ReadGuid();
                                    Guid MaterialVariationClassGuid = reader.ReadGuid();

                                    int texParamCount = reader.ReadInt();
                                    List<dynamic> texParams = new List<dynamic>();
                                    for (int i3 = 0; i3 < texParamCount; i3++)
                                    {
                                        PointerRef pr = new PointerRef(new EbxImportReference() { FileGuid = reader.ReadGuid(), ClassGuid = reader.ReadGuid() });
                                        CString cst = reader.ReadNullTerminatedString();
                                        dynamic texParam = TypeLibrary.CreateObject("TextureShaderParameter");
                                        texParam.Value = pr;
                                        texParam.ParameterName = cst;
                                        texParams.Add(texParam);
                                    }

                                    materials.Add(new MeshVariationMaterial(MaterialGuid, MaterialVariationAssetGuid, MaterialVariationClassGuid, texParams));
                                }
                                mvEntry.Variations.Add(var, new MeshVariation(nameHash, meshGuid, tuples, materials));
                            }
                            entries.Add(guid, mvEntry);
                        }
                    }
                    
                }
            }
            if (generateMVDB)
            {
                foreach (EbxAssetEntry ebx in App.AssetManager.EnumerateEbx("MeshVariationDatabase"))
                {
                    uint progress = (uint)((index / (float)totalCount) * 100);
                    task.Update(progress: progress);
                    if (ebx.IsAdded)
                        continue;
                    EbxAsset asset = App.AssetManager.GetEbx(ebx, true);
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

                using (NativeWriter writer = new NativeWriter(new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + @"\Caches\" + ProfilesLibrary.CacheName + "_mvdb.cache", FileMode.Create)))
                {
                    writer.Write(mvdbVersion);
                    writer.Write(entries.Count);
                    foreach (MeshVariationDbEntry mvdbEntry in entries.Values)
                    {
                        writer.Write(mvdbEntry.Guid);
                        writer.Write(mvdbEntry.Variations.Count);
                        foreach (uint var in mvdbEntry.Variations.Keys)
                        {
                            writer.Write(var);
                            writer.Write(mvdbEntry.Variations[var].AssetNameHash);
                            writer.Write(mvdbEntry.Variations[var].MeshGuid);
                            writer.Write(mvdbEntry.Variations[var].DbLocations.Count);
                            foreach (Tuple<EbxImportReference, int> tup in mvdbEntry.Variations[var].DbLocations)
                            {
                                writer.Write(tup.Item1.FileGuid);
                                writer.Write(tup.Item1.ClassGuid);
                                writer.Write(tup.Item2);
                            }
                            writer.Write(mvdbEntry.Variations[var].Materials.Count);
                            foreach (MeshVariationMaterial mvMat in mvdbEntry.Variations[var].Materials)
                            {
                                writer.Write(mvMat.MaterialGuid);
                                writer.Write(mvMat.MaterialVariationAssetGuid);
                                writer.Write(mvMat.MaterialVariationClassGuid);
                                dynamic texParams = mvMat.TextureParameters;
                                if (texParams == null)
                                {
                                    writer.Write(0);
                                    continue;
                                }
                                writer.Write(texParams.Count);
                                foreach (dynamic texParam in texParams)
                                {
                                    writer.Write(texParam.Value.External.FileGuid);
                                    writer.Write(texParam.Value.External.ClassGuid);
                                    writer.WriteNullTerminatedString(texParam.ParameterName);
                                }
                            }
                        }

                    }
                }
            }
            IsLoaded = true;
        }

        public static void LoadModifiedVariations()
        {

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Madden20,
                ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                ProfileVersion.NeedForSpeedHeat,
                ProfileVersion.Fifa21, ProfileVersion.Madden22,
                ProfileVersion.Fifa22, ProfileVersion.Battlefield2042,
                ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                IsLoaded = true;
                return;
            }

            modifiedentries.Clear();

            foreach (EbxAssetEntry ebx in App.AssetManager.EnumerateEbx("MeshVariationDatabase").ToList().Where(o => o.IsModified == true))
            {
                App.Logger.Log(ebx.Name);
                EbxAsset asset = App.AssetManager.GetEbx(ebx);
                {
                    dynamic db = asset.RootObject;
                    int j = 0;

                    foreach (dynamic v in db.Entries)
                    {
                        try
                        {
                            Guid meshGuid = ((PointerRef)v.Mesh).External.FileGuid;
                            if (!modifiedentries.ContainsKey(meshGuid))
                                modifiedentries.Add(meshGuid, new MeshVariationDbEntry(meshGuid));
                            modifiedentries[meshGuid].Add(ebx.Guid, asset.RootInstanceGuid, j, v);
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

        public static MeshVariationDbEntry GetVariations(Guid meshGuid)
        {
            if ((entries.Count == 0 & modifiedentries.Count == 0) || (!entries.ContainsKey(meshGuid) & !modifiedentries.ContainsKey(meshGuid)))
                return null;
            if (!modifiedentries.ContainsKey(meshGuid))
                return entries[meshGuid];
            else
            {
                MeshVariationDbEntry modifiedDBEntry = modifiedentries[meshGuid];
                if (!entries.ContainsKey(meshGuid))
                    return modifiedDBEntry;

                MeshVariationDbEntry originalDBEntry = entries[meshGuid];
                foreach(uint key in originalDBEntry.Variations.Keys)
                {
                    if (!modifiedDBEntry.Variations.ContainsKey(key))
                        modifiedDBEntry.Variations.Add(key, originalDBEntry.Variations[key]);
                }
                return modifiedDBEntry;
            }
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
