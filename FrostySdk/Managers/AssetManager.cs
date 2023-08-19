using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.IO.Ebx;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Loaders;
using Frosty.Sdk.Managers.Patch;
using Frosty.Sdk.Resources;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers;

/// <summary>
/// Manages everything related to Assets from the game.
/// </summary>
public static class AssetManager
{
    public static bool IsInitialized { get; private set; }
    
    public static ILogger? Logger { get; set; }

    private static readonly Dictionary<int, int> s_superBundlesNameHashHashMap = new();
    private static readonly List<SuperBundleEntry> s_superBundles = new();
    
    private static readonly Dictionary<int, int> s_bundlesNameHashHashMap = new();
    private static readonly List<BundleEntry> s_bundles = new();
    
    private static readonly Dictionary<int, int> s_ebxNameHashHashMap = new();
    private static readonly Dictionary<Guid, int> s_ebxGuidHashMap = new();
    private static readonly List<EbxAssetEntry> s_ebxAssetEntries = new();
    
    private static readonly Dictionary<int, int> s_resNameHashHashMap = new();
    private static readonly Dictionary<ulong, int> s_resRidHashMap = new();
    private static readonly List<ResAssetEntry> s_resAssetEntries = new();
    
    private static readonly Dictionary<Guid, int> s_chunkGuidHashMap = new();
    private static readonly List<ChunkAssetEntry> s_chunkAssetEntries = new();

    /// <summary>
    /// Cache Versions:
    /// <para>1 - Initial Version</para>
    /// <para>2 - Nothing changed in the format just bumped up that the cache gets regenerated, bc bundled chunks did not always had their logical offset/size stored</para>
    /// <para>3 - Completely changed what needs to be stored</para>
    /// </summary>
    private const uint c_cacheVersion = 3;
    private const ulong c_cacheMagic = 0x02005954534F5246;

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
            Stopwatch timer = new();
            
            Logger?.Report("Sdk", "Loading FileInfos from catalogs");
            
            timer.Start();
            ResourceManager.LoadInstallChunks();
            timer.Stop();
            
            Logger?.Report("Sdk", $"Loaded FileInfos from catalogs in {timer.Elapsed.Seconds} seconds");
            
            IAssetLoader assetLoader = GetAssetLoader();
            
            Logger?.Report("Sdk", "Loading Assets from SuperBundles");
            
            timer.Restart();
            assetLoader.Load();
            timer.Stop();
            
            Logger?.Report("Sdk", $"Loaded Assets from SuperBundles in {timer.Elapsed.Seconds} seconds");
            
            ResourceManager.CLearInstallChunks();
            
            Logger?.Report("Sdk", "Indexing Ebx");
            
            timer.Restart();
            DoEbxIndexing();
            timer.Stop();
            
            Logger?.Report("Sdk", $"Indexed ebx in {timer.Elapsed}");
            
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
        
        Logger?.Report("Sdk", "Finished initializing");
        
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
        return Stream.Null;
    }

    public static Stream GetRawAsset(AssetEntry entry)
    {
        return Stream.Null;
    }

    #endregion
    
    public static IEnumerable<SuperBundleEntry> EnumerateSuperBundles()
    {
        for (int i = 0; i < s_superBundles.Count; i++)
        {
            yield return s_superBundles[i];
        }
    }

    public static IEnumerable<BundleEntry> EnumerateBundles(BundleType type = BundleType.None, int superBundleId = -1)
    {
        foreach (BundleEntry entry in s_bundles)
        {
            if (type != BundleType.None && entry.Type != type)
            {
                continue;
            }

            if (superBundleId != -1 && entry.SuperBundleId != superBundleId)
            {
                continue;
            }
            
            yield return entry;
        }
    }
    
    public static IEnumerable<EbxAssetEntry> EnumerateEbxAssetEntries()
    {
        foreach (EbxAssetEntry entry in s_ebxAssetEntries)
        {
            yield return entry;
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

    internal static int AddBundle(string name, int superBundleId)
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

    private static IAssetLoader GetAssetLoader()
    {
        switch (FileSystemManager.BundleFormat)
        {
            case BundleFormat.DbObject:
                return new DbObjectAssetLoader();
            case BundleFormat.Manifest2019:
                return new Manifest2019AssetLoader();
            case BundleFormat.Kelvin:
                return new KelvinAssetLoader();
            case BundleFormat.SuperBundleManifest:
                return new ManifestAssetLoader();
            default:
                throw new ArgumentException("Not valid AssetLoader.");
        }
    }

    #region -- AddingAssets --

    internal static void AddEbx(EbxAssetEntry entry, int bundleId)
    {
        int nameHash = Utils.Utils.HashString(entry.Name, true);
        if (s_ebxNameHashHashMap.TryGetValue(nameHash, out int index))
        {
            EbxAssetEntry existing = s_ebxAssetEntries[index];
            
            existing.FileInfos.UnionWith(entry.FileInfos);

            existing.Bundles.Add(bundleId);
        }
        else
        {
            entry.Bundles.Add(bundleId);
            s_ebxNameHashHashMap.Add(nameHash, s_ebxAssetEntries.Count);
            s_ebxAssetEntries.Add(entry);
        }
    }

    internal static void AddRes(ResAssetEntry entry, int bundleId)
    {
        int nameHash = Utils.Utils.HashString(entry.Name, true);
        if (s_resNameHashHashMap.TryGetValue(nameHash, out int index))
        {
            ResAssetEntry existing = s_resAssetEntries[index];
            
            existing.FileInfos.UnionWith(entry.FileInfos);

            existing.Bundles.Add(bundleId);
        }
        else
        {
            entry.Bundles.Add(bundleId);
            if (entry.ResRid != 0)
            {
                s_resRidHashMap.Add(entry.ResRid, s_resAssetEntries.Count);
            }
            s_resNameHashHashMap.Add(nameHash, s_resAssetEntries.Count);
            s_resAssetEntries.Add(entry);
        }
    }

    internal static void AddChunk(ChunkAssetEntry entry, int bundleId)
    {
        if (s_chunkGuidHashMap.TryGetValue(entry.Id, out int index))
        {
            ChunkAssetEntry existing = s_chunkAssetEntries[index];
            
            existing.FileInfos.UnionWith(entry.FileInfos);

            existing.Bundles.Add(bundleId);
            
            if (existing.LogicalSize == 0)
            {
                // this chunk was first added as a superbundle chunk, so add logical offset/size and sha1
                existing.Sha1 = existing.Sha1;
                existing.LogicalOffset = existing.LogicalOffset;
                existing.LogicalSize = existing.LogicalSize;
                existing.OriginalSize = entry.OriginalSize;
            }
        }
        else
        {
            entry.Bundles.Add(bundleId);
            s_chunkGuidHashMap.Add(entry.Id, s_chunkAssetEntries.Count);
            s_chunkAssetEntries.Add(entry);
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
        if (s_chunkGuidHashMap.TryGetValue(entry.Id, out int index))
        {
            ChunkAssetEntry existing = s_chunkAssetEntries[index];
            
            // add existing Bundles
            entry.Bundles.UnionWith(existing.Bundles);
            
            entry.FileInfos.UnionWith(existing.FileInfos);

            // add logicalOffset/Size, since those are only stored in bundles
            entry.LogicalOffset = existing.LogicalOffset;
            entry.LogicalSize = existing.LogicalSize;
            entry.OriginalSize = existing.OriginalSize;

            // merge SuperBundles
            entry.SuperBundles.UnionWith(existing.SuperBundles);

            s_chunkAssetEntries[s_chunkGuidHashMap[entry.Id]] = entry;
        }
        else
        {
            s_chunkGuidHashMap.Add(entry.Id, s_chunkAssetEntries.Count);
            s_chunkAssetEntries.Add(entry);
        }
    }

    #endregion

    #region -- Cache --

    private static void DoEbxIndexing()
    {
        // TODO: implement GetAsset()
        return;
        
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
                if (FileSystemManager.BundleFormat == BundleFormat.SuperBundleManifest)
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

            Logger?.Report("Sdk", "Loading ebx from cache");
            int ebxCount = stream.ReadInt32();
            for (int i = 0; i < ebxCount; i++)
            {
                Logger?.Report(i / (double)ebxCount);
                string name = stream.ReadNullTerminatedString();

                EbxAssetEntry entry = new(name, stream.ReadSha1(), stream.ReadInt64(), stream.ReadInt64())
                {
                    Guid = stream.ReadGuid(),
                    Type = stream.ReadNullTerminatedString()
                };

                int numFileInfos = stream.ReadInt32();
                for (int j = 0; j < numFileInfos; j++)
                {
                    entry.FileInfos.Add(IFileInfo.Deserialize(stream));
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
                    // TODO: remove this when the ebx indexing is working
                    if (entry.Guid != Guid.Empty)
                    {
                        s_ebxGuidHashMap.Add(entry.Guid, s_ebxAssetEntries.Count);
                    }
                    s_ebxNameHashHashMap.Add(Utils.Utils.HashString(name, true), s_ebxAssetEntries.Count);
                    s_ebxAssetEntries.Add(entry);
                }
            }

            Logger?.Report("Sdk", "Loading res from cache");
            int resCount = stream.ReadInt32();
            for (int i = 0; i < resCount; i++)
            {
                Logger?.Report(i / (double)resCount);
                string name = stream.ReadNullTerminatedString();

                ResAssetEntry entry = new(name, stream.ReadSha1(), stream.ReadInt64(), stream.ReadInt64(),
                    stream.ReadUInt64(), stream.ReadUInt32(), stream.ReadBytes(stream.ReadInt32()));

                int numFileInfos = stream.ReadInt32();
                for (int j = 0; j < numFileInfos; j++)
                {
                    entry.FileInfos.Add(IFileInfo.Deserialize(stream));
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
            
            Logger?.Report("Sdk", "Loading chunks from cache");
            int chunkCount = stream.ReadInt32();
            for (int i = 0; i < chunkCount; i++)
            {
                Logger?.Report(i / (double)chunkCount);
                ChunkAssetEntry entry = new(stream.ReadGuid(), stream.ReadSha1(), stream.ReadInt64(),
                    stream.ReadUInt32(), stream.ReadUInt32());

                int numFileInfos = stream.ReadInt32();
                for (int j = 0; j < numFileInfos; j++)
                {
                    entry.FileInfos.Add(IFileInfo.Deserialize(stream));
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
        FileInfo fi = new($"{FileSystemManager.CacheName}.cache");
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
                
                stream.WriteInt32(entry.FileInfos.Count);
                foreach (IFileInfo fileInfo in entry.FileInfos)
                {
                    IFileInfo.Serialize(stream, fileInfo);
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
                
                stream.WriteInt32(entry.FileInfos.Count);
                foreach (IFileInfo fileInfo in entry.FileInfos)
                {
                    IFileInfo.Serialize(stream, fileInfo);
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
                
                stream.WriteInt32(entry.FileInfos.Count);
                foreach (IFileInfo fileInfo in entry.FileInfos)
                {
                    IFileInfo.Serialize(stream, fileInfo);
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