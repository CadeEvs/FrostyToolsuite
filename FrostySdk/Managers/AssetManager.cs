using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.IO.Ebx;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Loaders;
using Frosty.Sdk.Managers.Patch;
using Frosty.Sdk.Resources;
using FileInfo = Frosty.Sdk.Managers.Infos.FileInfo;

namespace Frosty.Sdk.Managers;

/// <summary>
/// Manages everything related to Assets from the game.
/// </summary>
public static class AssetManager
{
    public static bool IsInitialized;

    private static readonly Dictionary<int, int> s_superBundlesNameHashHashMap = new();
    private static readonly List<SuperBundleEntry> s_superBundles = new();
    
    private static readonly Dictionary<int, int> s_bundlesNameHashHashMap = new();
    private static readonly List<BundleEntry> s_bundles = new();
    
    private static readonly Dictionary<int, int> s_ebxNameHashHashMap = new();
    private static readonly Dictionary<Guid, int> s_ebxGuidHashMap = new();
    private static readonly ObservableCollection<EbxAssetEntry> s_ebxAssetEntries = new();
    
    private static readonly Dictionary<int, int> s_resNameHashHashMap = new();
    private static readonly Dictionary<ulong, int> s_resRidHashMap = new();
    private static readonly ObservableCollection<ResAssetEntry> s_resAssetEntries = new();
    
    private static readonly Dictionary<Guid, int> s_chunkGuidHashMap = new();
    private static readonly ObservableCollection<ChunkAssetEntry> s_chunkAssetEntries = new();

    private const ulong c_cacheMagic = 0x02005954534F5246; 
    private const uint c_cacheVersion = 4;

    /// <summary>
    /// Parses the games SuperBundles and creates lookups for all Assets, Bundles and SuperBundles.
    /// </summary>
    /// <param name="patchResult">The <see cref="PatchResult"/> that all the changes will get added to, if it is not null and a previous cache exists.</param>
    /// <returns>False if the initialization failed.</returns>
    public static bool Initialize(PatchResult? patchResult = null)
    {
        if (IsInitialized)
        {
            return true;
        }
        
        if (!FileSystemManager.IsInitialized || !ResourceManager.IsInitialized)
        {
            return false;
        }

        if (!ReadCache(out List<EbxAssetEntry> prePatchEbx, out List<ResAssetEntry> prePatchRes,
                out List<ChunkAssetEntry> prePatchChunks))
        {
            IAssetLoader assetLoader = GetAssetLoader(ProfilesLibrary.AssetLoader);
            assetLoader.Load();
            
            DoEbxIndexing();
            
            WriteCache();

            if (prePatchEbx.Count > 0 || prePatchRes.Count > 0 || prePatchChunks.Count > 0)
            {
                if (patchResult != null)
                {
                    // modified/added ebx
                    foreach (EbxAssetEntry ebxAssetEntry in s_ebxAssetEntries)
                    {
                        EbxAssetEntry? prePatch = prePatchEbx.Find(e =>
                            e.Name.Equals(ebxAssetEntry.Name, StringComparison.OrdinalIgnoreCase));
                        if (prePatch != null)
                        {
                            if (prePatch.Sha1 != ebxAssetEntry.Sha1)
                            {
                                patchResult.Modified.Ebx.Add(ebxAssetEntry.Name);
                                prePatchEbx.Remove(prePatch);
                            }
                        }
                        else
                        {
                            patchResult.Added.Ebx.Add(ebxAssetEntry.Name);
                        }
                    }
                    
                    // modified/added res
                    foreach (ResAssetEntry resAssetEntry in s_resAssetEntries)
                    {
                        ResAssetEntry? prePatch = prePatchRes.Find(e =>
                            e.Name.Equals(resAssetEntry.Name, StringComparison.OrdinalIgnoreCase));
                        if (prePatch != null)
                        {
                            if (prePatch.Sha1 != resAssetEntry.Sha1)
                            {
                                patchResult.Modified.Res.Add(resAssetEntry.Name);
                                prePatchRes.Remove(prePatch);
                            }
                        }
                        else
                        {
                            patchResult.Added.Res.Add(resAssetEntry.Name);
                        }
                    }
                    
                    // modified/added chunks
                    foreach (ChunkAssetEntry chunkAssetEntry in s_chunkAssetEntries)
                    {
                        ChunkAssetEntry? prePatch = prePatchChunks.Find(e =>
                            e.Id.Equals(chunkAssetEntry.Id));
                        if (prePatch != null)
                        {
                            if (prePatch.Sha1 != chunkAssetEntry.Sha1)
                            {
                                patchResult.Modified.Chunks.Add(chunkAssetEntry.Id);
                                prePatchChunks.Remove(prePatch);
                            }
                        }
                        else
                        {
                            patchResult.Added.Chunks.Add(chunkAssetEntry.Id);
                        }
                    }

                    // removed ebx
                    foreach (EbxAssetEntry ebxAssetEntry in prePatchEbx)
                    {
                        patchResult.Removed.Ebx.Add(ebxAssetEntry.Name);
                    }
                    
                    // removed res
                    foreach (ResAssetEntry resAssetEntry in prePatchRes)
                    {
                        patchResult.Removed.Res.Add(resAssetEntry.Name);
                    }
                    
                    // removed chunks
                    foreach (ChunkAssetEntry chunkAssetEntry in prePatchChunks)
                    {
                        patchResult.Removed.Chunks.Add(chunkAssetEntry.Id);
                    }
                }
            
                prePatchEbx.Clear();
                prePatchRes.Clear();
                prePatchChunks.Clear();
            }
        }
        
        IsInitialized = true;
        return true;
    }

    #region -- GetEntry --

    #region -- SuperBundle --

    /// <summary>
    /// Gets the <see cref="SuperBundleEntry"/> by name.
    /// </summary>
    /// <param name="name">The name of the SuperBundle.</param>
    /// <returns>The <see cref="SuperBundleEntry"/> or null if it doesn't exist.</returns>
    public static SuperBundleEntry? GetSuperBundleEntry(string name)
    {
        int nameHash = Utils.Utils.HashString(name, true);
        return s_superBundlesNameHashHashMap.TryGetValue(nameHash, out int value) ? s_superBundles[value] : null;
    }
    
    /// <summary>
    /// Gets the <see cref="SuperBundleEntry"/> by Id.
    /// </summary>
    /// <param name="id">The Id of the SuperBundle.</param>
    /// <returns>The <see cref="SuperBundleEntry"/> or null if it doesn't exist.</returns>
    public static SuperBundleEntry? GetSuperBundleEntry(int id)
    {
        return s_superBundles.Count > id ? s_superBundles[id] : null;
    }

    #endregion

    #region -- Bundle --

    /// <summary>
    /// Gets the <see cref="BundleEntry"/> by name.
    /// </summary>
    /// <param name="name">The name of the Bundle.</param>
    /// <returns>The <see cref="BundleEntry"/> or null if it doesn't exist.</returns>
    public static BundleEntry? GetBundleEntry(string name)
    {
        int nameHash = Utils.Utils.HashString(name, true);
        return s_bundlesNameHashHashMap.TryGetValue(nameHash, out int value) ? s_bundles[value] : null;
    }

    /// <summary>
    /// Gets the <see cref="BundleEntry"/> by name.
    /// </summary>
    /// <param name="id">The Id of the Bundle.</param>
    /// <returns>The <see cref="BundleEntry"/> or null if it doesn't exist.</returns>
    public static BundleEntry? GetBundleEntry(int id)
    {
        return s_bundles.Count > id ? s_bundles[id] : null;
    }

    #endregion
    
    #region -- AssetEntry --

    #region -- Ebx --

    /// <summary>
    /// Gets the <see cref="EbxAssetEntry"/> by name.
    /// </summary>
    /// <param name="name">The name of the Ebx.</param>
    /// <returns>The <see cref="EbxAssetEntry"/> or null if it doesn't exist.</returns>
    public static EbxAssetEntry? GetEbxAssetEntry(string name)
    {
        int nameHash = Utils.Utils.HashString(name, true);
        return s_ebxNameHashHashMap.TryGetValue(nameHash, out int value) ? s_ebxAssetEntries[value] : null;
    }
    
    /// <summary>
    /// Gets the <see cref="EbxAssetEntry"/> by <see cref="Guid"/>.
    /// </summary>
    /// <param name="guid">The <see cref="Guid"/> of the Ebx.</param>
    /// <returns>The <see cref="EbxAssetEntry"/> or null if it doesn't exist.</returns>
    public static EbxAssetEntry? GetEbxAssetEntry(Guid guid)
    {
        return s_ebxGuidHashMap.TryGetValue(guid, out int value) ? s_ebxAssetEntries[value] : null;
    }

    #endregion
    
    #region -- Res --

    /// <summary>
    /// Gets the <see cref="ResAssetEntry"/> by name.
    /// </summary>
    /// <param name="name">The name of the Res.</param>
    /// <returns>The <see cref="ResAssetEntry"/> or null if it doesn't exist.</returns>
    public static ResAssetEntry? GetResAssetEntry(string name)
    {
        int nameHash = Utils.Utils.HashString(name, true);
        return s_resNameHashHashMap.TryGetValue(nameHash, out int value) ? s_resAssetEntries[value] : null;
    }
    
    /// <summary>
    /// Gets the <see cref="ResAssetEntry"/> by Rid.
    /// </summary>
    /// <param name="resRid">The Rid of the Res.</param>
    /// <returns>The <see cref="ResAssetEntry"/> or null if it doesn't exist.</returns>
    public static ResAssetEntry? GetResAssetEntry(ulong resRid)
    {
        return s_resRidHashMap.TryGetValue(resRid, out int value) ? s_resAssetEntries[value] : null;
    }

    #endregion

    #region -- Chunk --
    
    /// <summary>
    /// Gets the <see cref="ChunkAssetEntry"/> by Id.
    /// </summary>
    /// <param name="chunkId">The Id of the Res.</param>
    /// <returns>The <see cref="ChunkAssetEntry"/> or null if it doesn't exist.</returns>
    public static ChunkAssetEntry? GetChunkAssetEntry(Guid chunkId)
    {
        return s_chunkGuidHashMap.TryGetValue(chunkId, out int value) ? s_chunkAssetEntries[value] : null;
    }

    #endregion

#endregion

    #endregion

    #region -- GetAsset --
    
    public static EbxAsset GetEbx(EbxAssetEntry entry)
    {
        using (EbxReader reader = EbxReader.CreateReader(GetAsset(entry)))
        {
            return reader.ReadAsset<EbxAsset>();
        }
    }

    public static T GetResAs<T>(ResAssetEntry entry)
        where T : Resource, new()
    {
        using (DataStream stream = new(GetAsset(entry)))
        {
            T retVal = new T();
            retVal.Set(entry);
            retVal.Deserialize(stream);
            
            return retVal;
        }
    }

    public static Stream GetAsset(AssetEntry entry)
    {
        switch (entry.Location)
        {
            case AssetDataLocation.Cas:
                return entry.BaseSha1 != Sha1.Zero ? ResourceManager.GetResourceData(entry.BaseSha1, entry.DeltaSha1) : ResourceManager.GetResourceData(entry.Sha1);
            case AssetDataLocation.CasNonIndexed:
                return ResourceManager.GetResourceData(entry.FileInfo);
            default:
                throw new Exception();
        }
    }

    public static Stream GetRawAsset(AssetEntry entry)
    {
        switch (entry.Location)
        {
            case AssetDataLocation.Cas:
                return entry.BaseSha1 != Sha1.Zero ? ResourceManager.GetRawResourceData(entry.BaseSha1, entry.DeltaSha1) : ResourceManager.GetResourceData(entry.Sha1);
            case AssetDataLocation.CasNonIndexed:
                return ResourceManager.GetRawResourceData(entry.FileInfo);
            default:
                throw new Exception();
        }
    }

    #endregion

    public static IEnumerable<SuperBundleEntry> EnumerateSuperBundles() => s_superBundles;
    
    public static IEnumerable<SuperBundleEntry> EnumerateSuperBundlesYield()
    {
        for (int i = 0; i < s_superBundles.Count; i++)
        {
            yield return s_superBundles[i];
        }
    }

    /// <summary>
    /// Adds SuperBundle to the AssetManager.
    /// </summary>
    /// <param name="name">The name of the SuperBundle.</param>
    /// <returns>The Id of the SuperBundle.</returns>
    internal static int AddSuperBundle(string name)
    {
        int hash = Utils.Utils.HashString(name, true);
        if (s_superBundlesNameHashHashMap.TryGetValue(hash, out int id))
        {
            return id;
        }
        id = s_superBundles.Count;
        s_superBundlesNameHashHashMap.Add(hash, id);
        s_superBundles.Add(new SuperBundleEntry(name));
        return id;
    }

    /// <summary>
    /// Processes Assets from Bundle.
    /// </summary>
    /// <param name="bundle">The Bundle to process.</param>
    /// <param name="superBundleId">The Id of the SuperBundle the Bundle is in.</param>
    /// <param name="bundleName">The name of the Bundle.</param>
    internal static void ProcessBundle(DbObject bundle, int superBundleId, string bundleName)
    {
        int bundleId = AddBundle(bundleName, superBundleId);
        if (bundle.ContainsKey("ebx"))
        {
            AddEbx(bundle["ebx"].As<DbObject>(), bundleId);
        }
        if (bundle.ContainsKey("res"))
        {
            AddRes(bundle["res"].As<DbObject>(), bundleId);
        }
        if (bundle.ContainsKey("chunks"))
        {
            AddChunk(bundle["chunks"].As<DbObject>(), bundleId);
        }
    }

    /// <summary>
    /// Adds Chunk contained in the toc of a SuperBundle to the AssetManager.
    /// This will override any location of where an already processed chunk was stored,
    /// so that TextureChunks which are stored in Bundles have the correct data.
    /// </summary>
    /// <param name="entry">The <see cref="ChunkAssetEntry"/> of the Chunk.</param>
    internal static void AddSuperBundleChunk(ChunkAssetEntry entry)
    {
        if (s_chunkGuidHashMap.ContainsKey(entry.Id))
        {
            ChunkAssetEntry existing = s_chunkAssetEntries[s_chunkGuidHashMap[entry.Id]];
            
            // add existing Bundles
            entry.Bundles.UnionWith(existing.Bundles);

            // add logicalOffset/Size, since those are only stored in bundles
            entry.LogicalOffset = existing.LogicalOffset;
            entry.LogicalSize = existing.LogicalSize;

            // merge SuperBundles
            foreach (int existingSuperBundle in existing.SuperBundles.Where(existingSuperBundle => !entry.SuperBundles.Contains(existingSuperBundle)))
            {
                entry.SuperBundles.Add(existingSuperBundle);
            }

            s_chunkAssetEntries[s_chunkGuidHashMap[entry.Id]] = entry;
        }
        else
        {
            s_chunkGuidHashMap.Add(entry.Id, s_chunkAssetEntries.Count);
            s_chunkAssetEntries.Add(entry);
        }
    }

    private static int AddBundle(string name, int superBundleId)
    {
        int hash = Utils.Utils.HashString(name, true);
        if (s_bundlesNameHashHashMap.TryGetValue(hash, out int bundleId))
        {
            return bundleId;
        }
        s_bundlesNameHashHashMap.Add(hash, s_bundles.Count);
        s_bundles.Add(new BundleEntry(name, superBundleId));
        return s_bundles.Count - 1;
    }

    private static IAssetLoader GetAssetLoader(string name)
    {
        switch (name)
        {
            case "Standard":
                return new StandardAssetLoader();
            case "Cas":
                return new CasAssetLoader();
            case "Fifa":
                return new FifaAssetLoader();
            case "Manifest":
                return new ManifestAssetLoader();
            default:
                throw new ArgumentException("Not valid AssetLoader.");
        }
    }

    #region -- AddingAssets --

    /// <summary>
    /// Adds all ebx of a bundle to the AssetManager.
    /// </summary>
    /// <param name="ebxList">The <see cref="DbObject"/> list of the ebx from the bundle.</param>
    /// <param name="bundleId">The id of the bundle.</param>
    private static void AddEbx(DbObject ebxList, int bundleId)
    {
        foreach (DbObject ebx in ebxList)
        {
            string name = ebx["name"].As<string>();
            int nameHash = Utils.Utils.HashString(name, true);
            EbxAssetEntry entry;
            if (!s_ebxNameHashHashMap.ContainsKey(nameHash))
            {
                entry = new EbxAssetEntry(name, ebx["sha1"].As<Sha1>(), ebx["size"].As<long>(),
                    ebx["originalSize"].As<long>());
                
                if (ebx.ContainsKey("casPatchType") && ebx["casPatchType"].As<int>() == 2)
                {
                    entry.BaseSha1 = ebx["baseSha1"].As<Sha1>();
                    entry.DeltaSha1 = ebx["deltaSha1"].As<Sha1>();
                }

                if (ebx.ContainsKey("path"))
                {
                    entry.Location = AssetDataLocation.CasNonIndexed;
                    entry.FileInfo = new FileInfo(ebx["path"].As<string>(), ebx["offset"].As<long>(),
                        ebx["size"].As<long>());
                }
                
                s_ebxNameHashHashMap.Add(nameHash, s_ebxAssetEntries.Count);
                s_ebxAssetEntries.Add(entry);
            }
            else
            {
                entry = s_ebxAssetEntries[s_ebxNameHashHashMap[nameHash]];
            }

            // TODO: figure out how exactly to handle split super bundles since they seem to contain the same assets
            if (!entry.Bundles.Contains(bundleId))
            {
                entry.Bundles.Add(bundleId);
            }
        }
    }

    /// <summary>
    /// Adds all res of a bundle to the AssetManager.
    /// </summary>
    /// <param name="resList">The <see cref="DbObject"/> list of the res from the bundle.</param>
    /// <param name="bundleId">The id of the bundle.</param>
    private static void AddRes(DbObject resList, int bundleId)
    {
        foreach (DbObject res in resList)
        {
            string name = (string)res["name"];
            int nameHash = Utils.Utils.HashString(name, true);
            ResAssetEntry entry;
            if (!s_resNameHashHashMap.ContainsKey(nameHash))
            {
                entry = new ResAssetEntry(name, res["sha1"].As<Sha1>(), res["size"].As<long>(),
                    res["originalSize"].As<long>(), res["resRid"].As<ulong>(), res["resType"].As<uint>(),
                    res["resMeta"].As<byte[]>());
                
                if (res.ContainsKey("casPatchType") && res["casPatchType"].As<int>() == 2)
                {
                    entry.BaseSha1 = res["baseSha1"].As<Sha1>();
                    entry.DeltaSha1 = res["deltaSha1"].As<Sha1>();
                }
                
                if (res.ContainsKey("path"))
                {
                    entry.Location = AssetDataLocation.CasNonIndexed;
                    entry.FileInfo = new FileInfo(res["path"].As<string>(), res["offset"].As<long>(),
                        res["size"].As<long>());
                }
                
                if (entry.ResRid != 0)
                {
                    s_resRidHashMap.Add(entry.ResRid, s_resAssetEntries.Count);
                }
                s_resNameHashHashMap.Add(nameHash, s_resAssetEntries.Count);
                s_resAssetEntries.Add(entry);
            }
            else
            {
                entry = s_resAssetEntries[s_resNameHashHashMap[nameHash]];
            }
            
            if (!entry.Bundles.Contains(bundleId))
            {
                entry.Bundles.Add(bundleId);
            }
        }
    }

    /// <summary>
    /// Adds all chunks of a bundle to the AssetManager.
    /// </summary>
    /// <param name="chunkList">The <see cref="DbObject"/> list of the chunks from the bundle.</param>
    /// <param name="bundleId">The id of the bundle.</param>
    private static void AddChunk(DbObject chunkList, int bundleId)
    {
        foreach (DbObject chunk in chunkList)
        {
            Guid chunkId = chunk["id"].As<Guid>();
            ChunkAssetEntry entry;
            if (!s_chunkGuidHashMap.ContainsKey(chunkId))
            {
                // some layouts store rangeStart/End or bundledSize, but we ignore those since we can recalculate them from the logicalOffset/Size
                entry = new ChunkAssetEntry(chunkId, chunk["sha1"].As<Sha1>(), chunk["size"].As<long>(),
                    chunk["logicalOffset"].As<uint>(), chunk["logicalSize"].As<uint>());
                
                if (chunk.ContainsKey("path"))
                {
                    entry.Location = AssetDataLocation.CasNonIndexed;
                    entry.FileInfo = new FileInfo(chunk["path"].As<string>(), chunk["offset"].As<long>(),
                        chunk["size"].As<long>());
                }
                
                s_chunkGuidHashMap.Add(chunkId, s_chunkAssetEntries.Count);
                s_chunkAssetEntries.Add(entry);
            }
            else
            {
                entry = s_chunkAssetEntries[s_chunkGuidHashMap[chunkId]];
            }
            
            if (!entry.Bundles.Contains(bundleId))
            {
                entry.Bundles.Add(bundleId);
            }
        }
    }

    #endregion

    #region -- Cache --

    private static void DoEbxIndexing()
    {
        if (s_ebxGuidHashMap.Count > 0)
        {
            return;
        }

        for (int i = 0; i < s_ebxAssetEntries.Count; i++)
        {
            EbxAssetEntry entry = s_ebxAssetEntries[i];
            
            using (EbxReader reader = EbxReader.CreateReader(GetAsset(entry)))
            {
                entry.Type = reader.RootType;
                entry.Guid = reader.FileGuid;

                // now grab the actual asset name
                reader.Position = reader.m_stringsOffset;
                string name = reader.ReadNullTerminatedString();
                int newNameHash = Utils.Utils.HashString(name, true);

                // only if the lower case one matches
                if (newNameHash == Utils.Utils.HashString(entry.Name, true))
                {
                    entry.Name = name;
                }

                foreach (EbxImportReference import in reader.Imports)
                {
                    entry.DependentAssets.Add(import.FileGuid);
                }
                
                s_ebxGuidHashMap.Add(entry.Guid, i);
                
                // Manifest AssetLoader has stripped the bundle names, so we need to figure them out
                if (ProfilesLibrary.AssetLoader.Equals("Manifest"))
                {
                    // need to work out bundle here (as bundles are hashed names only)
                    if (TypeLibrary.IsSubClassOf(entry.Type, "BlueprintBundle") ||
                        TypeLibrary.IsSubClassOf(entry.Type, "SubWorldData"))
                    {
                        BundleEntry be = s_bundles[entry.Bundles.First()];

                        be.Name = entry.Name;
                        if (!be.Name.StartsWith("win32/", StringComparison.OrdinalIgnoreCase))
                        {
                            be.Name = "win32/" + entry.Name;
                        }
                        be.Blueprint = entry;
                        
                    }
                    else if (TypeLibrary.IsSubClassOf(entry.Type, "UIItemDescriptionAsset") ||
                             TypeLibrary.IsSubClassOf(entry.Type, "UIMetaDataAsset"))
                    {
                        string bundleName = "win32/" + entry.Name.ToLower() + "_bundle";
                        int h = Utils.Utils.HashString(bundleName);
                        BundleEntry? be = s_bundles.Find(a => a.Name.Equals(h.ToString("x8")));

                        if (be != null)
                        {
                            be.Name = bundleName;
                        }
                    }
                }
            }
        }
    }

    private static bool ReadCache(out List<EbxAssetEntry> prePatchEbx, out List<ResAssetEntry> prePatchRes, out List<ChunkAssetEntry> prePatchChunks)
    {
        prePatchEbx = new List<EbxAssetEntry>();
        prePatchRes = new List<ResAssetEntry>();
        prePatchChunks = new List<ChunkAssetEntry>();
        
        if (!File.Exists($"{FileSystemManager.CacheName}.cache"))
        {
            return false;
        }
        
        bool isPatched = false;

        using (DataStream stream = new(new FileStream($"{FileSystemManager.CacheName}.cache", FileMode.Open, FileAccess.Read)))
        {
            ulong magic = stream.ReadUInt64();
            if (magic != c_cacheMagic)
            {
                return false;
            }

            uint version = stream.ReadUInt32();
            if (version != c_cacheVersion)
            {
                return false;
            }

            int profileNameHash = stream.ReadInt32();
            if (profileNameHash != Utils.Utils.HashString(ProfilesLibrary.ProfileName, true))
            {
                return false;
            }

            uint head = stream.ReadUInt32();
            if (head != FileSystemManager.Head)
            {
                isPatched = true;
            }

            int superBundleCount = stream.ReadInt32();
            for (int i = 0; i < superBundleCount; i++)
            {
                string name = stream.ReadNullTerminatedString();
                if (!isPatched)
                {
                    AddSuperBundle(name);
                }
            }

            int bundleCount = stream.ReadInt32();
            for (int i = 0; i < bundleCount; i++)
            {
                string name = stream.ReadNullTerminatedString();
                int superBundleId = stream.ReadInt32();

                if (!isPatched)
                {
                    AddBundle(name, superBundleId);
                }
            }

            int ebxCount = stream.ReadInt32();
            for (int i = 0; i < ebxCount; i++)
            {
                string name = stream.ReadNullTerminatedString();

                EbxAssetEntry entry = new(name, stream.ReadSha1(), stream.ReadInt64(), stream.ReadInt64())
                {
                    Guid = stream.ReadGuid(),
                    Type = stream.ReadNullTerminatedString(),
                    Location = (AssetDataLocation)stream.ReadByte()
                };

                switch (entry.Location)
                {
                    case AssetDataLocation.Cas:
                        entry.BaseSha1= stream.ReadSha1();
                        entry.DeltaSha1= stream.ReadSha1();
                        break;
                    case AssetDataLocation.CasNonIndexed:
                        entry.FileInfo.Path = stream.ReadNullTerminatedString();
                        entry.FileInfo.Offset = stream.ReadInt64();
                        entry.FileInfo.Size = stream.ReadInt64();
                        break;
                }

                int numBundles = stream.ReadInt32();
                for (int j = 0; j < numBundles; j++)
                {
                    entry.Bundles.Add(stream.ReadInt32());
                }

                if (isPatched)
                {
                    prePatchEbx.Add(entry);
                }
                else
                {
                    s_ebxGuidHashMap.Add(entry.Guid, s_ebxAssetEntries.Count);
                    s_ebxNameHashHashMap.Add(Utils.Utils.HashString(name, true), s_ebxAssetEntries.Count);
                    s_ebxAssetEntries.Add(entry);
                }
            }
            
            int resCount = stream.ReadInt32();
            for (int i = 0; i < resCount; i++)
            {
                string name = stream.ReadNullTerminatedString();

                ResAssetEntry entry = new(name, stream.ReadSha1(), stream.ReadInt64(), stream.ReadInt64(),
                    stream.ReadUInt64(), stream.ReadUInt32(), stream.ReadBytes(stream.ReadInt32()))
                {
                    Location = (AssetDataLocation)stream.ReadByte()
                };

                switch (entry.Location)
                {
                    case AssetDataLocation.Cas:
                        entry.BaseSha1= stream.ReadSha1();
                        entry.DeltaSha1= stream.ReadSha1();
                        break;
                    case AssetDataLocation.CasNonIndexed:
                        entry.FileInfo.Path = stream.ReadNullTerminatedString();
                        entry.FileInfo.Offset = stream.ReadInt64();
                        entry.FileInfo.Size = stream.ReadInt64();
                        break;
                }
                
                int numBundles = stream.ReadInt32();
                for (int j = 0; j < numBundles; j++)
                {
                    entry.Bundles.Add(stream.ReadInt32());
                }

                if (isPatched)
                {
                    prePatchRes.Add(entry);
                }
                else
                {
                    if (entry.ResRid != 0)
                    {
                        s_resRidHashMap.Add(entry.ResRid, s_resAssetEntries.Count);
                    }
                    s_resNameHashHashMap.Add(Utils.Utils.HashString(name, true), s_resAssetEntries.Count);
                    s_resAssetEntries.Add(entry);
                }
            }
            
            int chunkCount = stream.ReadInt32();
            for (int i = 0; i < chunkCount; i++)
            {
                ChunkAssetEntry entry = new(stream.ReadGuid(), stream.ReadSha1(), stream.ReadInt64(),
                    stream.ReadUInt32(), stream.ReadUInt32())
                {
                    Location = (AssetDataLocation)stream.ReadByte()
                };

                switch (entry.Location)
                {
                    case AssetDataLocation.Cas:
                        entry.BaseSha1= stream.ReadSha1();
                        entry.DeltaSha1= stream.ReadSha1();
                        break;
                    case AssetDataLocation.CasNonIndexed:
                        entry.FileInfo.Path = stream.ReadNullTerminatedString();
                        entry.FileInfo.Offset = stream.ReadInt64();
                        entry.FileInfo.Size = stream.ReadInt64();
                        break;
                }
                
                int numSuperBundles = stream.ReadInt32();
                for (int j = 0; j < numSuperBundles; j++)
                {
                    entry.SuperBundles.Add(stream.ReadInt32());
                }
                
                int numBundles = stream.ReadInt32();
                for (int j = 0; j < numBundles; j++)
                {
                    entry.Bundles.Add(stream.ReadInt32());
                }
                
                if (isPatched)
                {
                    prePatchChunks.Add(entry);
                }
                else
                {
                    s_chunkGuidHashMap.Add(entry.Id, s_chunkAssetEntries.Count);
                    s_chunkAssetEntries.Add(entry);
                }
            }
        }

        return !isPatched;
    }

    private static void WriteCache()
    {
        System.IO.FileInfo fi = new($"{FileSystemManager.CacheName}.cache");
        Directory.CreateDirectory(fi.DirectoryName!);

        using (DataStream stream = new(new FileStream(fi.FullName, FileMode.Create, FileAccess.Write)))
        {
            stream.WriteUInt64(c_cacheMagic);
            stream.WriteUInt32(c_cacheVersion);
            
            stream.WriteInt32(Utils.Utils.HashString(ProfilesLibrary.ProfileName, true));
            stream.WriteUInt32(FileSystemManager.Head);
            
            stream.WriteInt32(s_superBundles.Count);
            foreach (SuperBundleEntry superBundleEntry in s_superBundles)
            {
                stream.WriteNullTerminatedString(superBundleEntry.Name);
            }
            
            stream.WriteInt32(s_bundles.Count);
            foreach (BundleEntry bundleEntry in s_bundles)
            {
                stream.WriteNullTerminatedString(bundleEntry.Name);
                stream.WriteInt32(bundleEntry.SuperBundleId);
            }
            
            stream.WriteInt32(s_ebxAssetEntries.Count);
            foreach (EbxAssetEntry entry in s_ebxAssetEntries)
            {
                stream.WriteNullTerminatedString(entry.Name);
                
                stream.WriteSha1(entry.Sha1);
                stream.WriteInt64(entry.Size);
                stream.WriteInt64(entry.OriginalSize);

                stream.WriteGuid(entry.Guid);
                stream.WriteNullTerminatedString(entry.Type);
                stream.WriteByte((byte)entry.Location);

                switch (entry.Location)
                {
                    case AssetDataLocation.Cas:
                        stream.WriteSha1(entry.BaseSha1);
                        stream.WriteSha1(entry.DeltaSha1);
                        break;
                    case AssetDataLocation.CasNonIndexed:
                        stream.WriteNullTerminatedString(entry.FileInfo.Path);
                        stream.WriteInt64(entry.FileInfo.Offset);
                        stream.WriteInt64(entry.FileInfo.Size);
                        break;
                }
                
                stream.WriteInt32(entry.Bundles.Count);
                foreach (int bundleId in entry.Bundles)
                {
                    stream.WriteInt32(bundleId);
                }
            }
            
            stream.WriteInt32(s_resAssetEntries.Count);
            foreach (ResAssetEntry entry in s_resAssetEntries)
            {
                stream.WriteNullTerminatedString(entry.Name);
                
                stream.WriteSha1(entry.Sha1);
                stream.WriteInt64(entry.Size);
                stream.WriteInt64(entry.OriginalSize);
                
                stream.WriteUInt64(entry.ResRid);
                stream.WriteUInt32(entry.ResType);
                stream.WriteInt32(entry.ResMeta.Length);
                stream.Write(entry.ResMeta, 0, entry.ResMeta.Length);

                stream.WriteByte((byte)entry.Location);

                switch (entry.Location)
                {
                    case AssetDataLocation.Cas:
                        stream.WriteSha1(entry.BaseSha1);
                        stream.WriteSha1(entry.DeltaSha1);
                        break;
                    case AssetDataLocation.CasNonIndexed:
                        stream.WriteNullTerminatedString(entry.FileInfo.Path);
                        stream.WriteInt64(entry.FileInfo.Offset);
                        stream.WriteInt64(entry.FileInfo.Size);
                        break;
                }
                
                stream.WriteInt32(entry.Bundles.Count);
                foreach (int bundleId in entry.Bundles)
                {
                    stream.WriteInt32(bundleId);
                }
            }
            
            stream.WriteInt32(s_chunkAssetEntries.Count);
            foreach (ChunkAssetEntry entry in s_chunkAssetEntries)
            {
                stream.WriteGuid(entry.Id);
                
                stream.WriteSha1(entry.Sha1);
                stream.WriteInt64(entry.Size);
                
                stream.WriteUInt32(entry.LogicalOffset);
                stream.WriteUInt32(entry.LogicalSize);

                stream.WriteByte((byte)entry.Location);

                switch (entry.Location)
                {
                    case AssetDataLocation.Cas:
                        stream.WriteSha1(entry.BaseSha1);
                        stream.WriteSha1(entry.DeltaSha1);
                        break;
                    case AssetDataLocation.CasNonIndexed:
                        stream.WriteNullTerminatedString(entry.FileInfo.Path);
                        stream.WriteInt64(entry.FileInfo.Offset);
                        stream.WriteInt64(entry.FileInfo.Size);
                        break;
                }
                
                stream.WriteInt32(entry.SuperBundles.Count);
                foreach (int superBundleId in entry.SuperBundles)
                {
                    stream.WriteInt32(superBundleId);
                }
                
                stream.WriteInt32(entry.Bundles.Count);
                foreach (int bundleId in entry.Bundles)
                {
                    stream.WriteInt32(bundleId);
                }
            }
        }
    }

    #endregion
}