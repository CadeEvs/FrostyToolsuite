using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FrostySdk.Managers.Entries;

namespace Frosty.ModSupport
{
    public partial class FrostyModExecutor
    {
        private class ArchiveInfo
        {
            public byte[] Data;
            public int RefCount;
        }
        private class ModBundleInfo
        {
            public class ModBundleAction
            {
                public List<string> Ebx = new List<string>();
                public List<string> Res = new List<string>();
                public List<Guid> Chunks = new List<Guid>();

                public void AddEbx(string name)
                {
                    lock (Ebx)
                    {
                        if (!Ebx.Contains(name))
                        {
                            Ebx.Add(name);
                        }
                    }
                }
                public void AddRes(string name)
                {
                    lock (Res)
                    {
                        if (!Res.Contains(name))
                        {
                            Res.Add(name);
                        }
                    }
                }
                public void AddChunk(Guid guid)
                {
                    lock (Chunks)
                    {
                        if (!Chunks.Contains(guid))
                        {
                            Chunks.Add(guid);
                        }
                    }
                }
            }
            public int Name;
            public ModBundleAction Add = new ModBundleAction();
            public ModBundleAction Remove = new ModBundleAction();
            public ModBundleAction Modify = new ModBundleAction();
        }
        private class CasFileEntry
        {
            public ManifestFileInfo FileInfo;
            public ChunkAssetEntry Entry;
        }
        private class CasDataEntry
        {
            public string Catalog { get; }
            public bool HasEntries => dataRefs.Count != 0;

            private List<Sha1> dataRefs = new List<Sha1>();
            private Dictionary<Sha1, List<CasFileEntry>> fileInfos = new Dictionary<Sha1, List<CasFileEntry>>();

            public CasDataEntry(string inCatalog, params Sha1[] sha1)
            {
                Catalog = inCatalog;
                if (sha1.Length != 0)
                    dataRefs.AddRange(sha1);
            }

            public void Add(Sha1 sha1, ChunkAssetEntry entry = null, ManifestFileInfo file = null)
            {
                if (!dataRefs.Contains(sha1))
                {
                    dataRefs.Add(sha1);
                    fileInfos.Add(sha1, new List<CasFileEntry>());
                }

                if (entry != null || file != null)
                    fileInfos[sha1].Add(new CasFileEntry() { Entry = entry, FileInfo = file });
            }

            public bool Contains(Sha1 sha1) => dataRefs.Contains(sha1);

            public IEnumerable<Sha1> EnumerateDataRefs()
            {
                foreach (Sha1 sha1 in dataRefs)
                    yield return sha1;
            }

            public IEnumerable<CasFileEntry> EnumerateFileInfos(Sha1 sha1)
            {
                int index = dataRefs.IndexOf(sha1);
                if (index == -1)
                    yield break;
                if (index >= fileInfos.Count)
                    yield break;
                foreach (CasFileEntry fi in fileInfos[sha1])
                    yield return fi;
            }
        }
        private class CasDataInfo
        {
            private Dictionary<string, CasDataEntry> entries = new Dictionary<string, CasDataEntry>();
            public void Add(string catalog, Sha1 sha1, ChunkAssetEntry entry = null, ManifestFileInfo file = null)
            {
                if (!entries.ContainsKey(catalog))
                {
                    entries.Add(catalog, new CasDataEntry(catalog));
                }
                entries[catalog].Add(sha1, entry, file);
            }

            public IEnumerable<CasDataEntry> EnumerateEntries()
            {
                foreach (CasDataEntry entry in entries.Values)
                    yield return entry;
            }

            public int CountEntries()
            {
                return entries.Count;
            }
        }
        private class FrostySymLinkException : Exception
        {
            public override string Message => "One ore more symbolic links could not be created, please restart tool as Administrator and ensure your storage drive is formatted to NTFS (not exFAT).";
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        private class ModInfo
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string Category { get; set; }
            public string Link { get; set; }
            public string FileName { get; set; }


            public override bool Equals(object obj)
            {
                ModInfo modInfo = obj as ModInfo;

                if (this.Name == modInfo.Name
                    && this.Version == modInfo.Version
                    && this.Category == modInfo.Category
                    && this.FileName == modInfo.FileName)
                    return true;

                return false;
            }
        }

        private struct SymLinkStruct
        {
            public readonly string dest;
            public readonly string src;
            public readonly bool isFolder;

            public SymLinkStruct(string inDst, string inSrc, bool inFolder)
            {
                dest = inDst;
                src = inSrc;
                isFolder = inFolder;
            }
        };

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        private FileSystemManager fs;
        private ResourceManager rm;
        private AssetManager am;
        private ILogger logger;

        private List<string> addedSuperBundles = new List<string>();

        private ConcurrentDictionary<int, ModBundleInfo> modifiedBundles = new ConcurrentDictionary<int, ModBundleInfo>();
        private ConcurrentDictionary<int, List<string>> addedBundles = new ConcurrentDictionary<int, List<string>>();

        private ConcurrentDictionary<string, EbxAssetEntry> modifiedEbx = new ConcurrentDictionary<string, EbxAssetEntry>();
        private ConcurrentDictionary<string, ResAssetEntry> modifiedRes = new ConcurrentDictionary<string, ResAssetEntry>();
        private ConcurrentDictionary<Guid, ChunkAssetEntry> modifiedChunks = new ConcurrentDictionary<Guid, ChunkAssetEntry>();
        private ConcurrentDictionary<string, DbObject> modifiedFs = new ConcurrentDictionary<string, DbObject>();

        private ConcurrentDictionary<Sha1, ArchiveInfo> archiveData = new ConcurrentDictionary<Sha1, ArchiveInfo>();
        private int numArchiveEntries = 0;
        private int numTasks;

        private CasDataInfo casData = new CasDataInfo();
        private static int chunksBundleHash = Fnv1.HashString("chunks");
        private Dictionary<int, Dictionary<int, Dictionary<uint, CatResourceEntry>>> resources = new Dictionary<int, Dictionary<int, Dictionary<uint, CatResourceEntry>>>();
        private string modDirName = "ModData";

        public ILogger Logger { get => logger; set => logger = value; }

        private void ReportProgress(int current, int total) => Logger.Log("progress:" + current / (float)total * 100d);

        private Dictionary<int, Dictionary<uint, CatResourceEntry>> LoadCatalog(FileSystemManager fs, string filename, out int catFileHash)
        {
            catFileHash = 0;
            string fullPath = fs.ResolvePath(filename);
            if (!File.Exists(fullPath))
                return null;

            catFileHash = Fnv1.HashString(fullPath.ToLower());
            Dictionary<int, Dictionary<uint, CatResourceEntry>> resources = new Dictionary<int, Dictionary<uint, CatResourceEntry>>();
            using (CatReader reader = new CatReader(new FileStream(fullPath, FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
            {
                for (int i = 0; i < reader.ResourceCount; i++)
                {
                    CatResourceEntry entry = reader.ReadResourceEntry();
                    if (!resources.ContainsKey(entry.ArchiveIndex))
                        resources.Add(entry.ArchiveIndex, new Dictionary<uint, CatResourceEntry>());
                    resources[entry.ArchiveIndex].Add(entry.Offset, entry);
                }
            }

            return resources;
        }

        private int HashBundle(BundleEntry bentry)
        {
            int hash = Fnv1.HashString(bentry.Name.ToLower());

            if (bentry.Name.Length == 8 && int.TryParse(bentry.Name, System.Globalization.NumberStyles.HexNumber, null, out int tmp))
                hash = tmp;

            return hash;
        }
        private void ProcessModResources(IResourceContainer fmod)
        {
            // Bundle whitelist may not contain the chunk bundle. This adds it to prevent issues
            if (App.WhitelistedBundles.Count != 0 && !App.WhitelistedBundles.Contains(chunksBundleHash))
            {
                App.WhitelistedBundles.Add(chunksBundleHash);
            }

            Parallel.ForEach(fmod.Resources, resource =>
            {
                // pull existing bundles from asset manager
                List<int> bundles = new List<int>();

                if (resource.Type == ModResourceType.Bundle)
                {
                    BundleEntry bEntry = new BundleEntry();
                    resource.FillAssetEntry(bEntry);

                    addedBundles.TryAdd(bEntry.SuperBundleId, new List<string>());
                    addedBundles[bEntry.SuperBundleId].Add(bEntry.Name);

                }
                else if (resource.Type == ModResourceType.Ebx)
                {
                    if (resource.IsModified || !modifiedEbx.ContainsKey(resource.Name))
                    {
                        if (resource.HasHandler)
                        {
                            EbxAssetEntry entry = null;
                            HandlerExtraData extraData = null;
                            byte[] data = fmod.GetResourceData(resource);

                            if (modifiedEbx.ContainsKey(resource.Name))
                            {
                                entry = modifiedEbx[resource.Name];
                                extraData = (HandlerExtraData)entry.ExtraData;
                            }
                            else
                            {
                                entry = new EbxAssetEntry();
                                extraData = new HandlerExtraData();

                                resource.FillAssetEntry(entry);
                                // the rest of the chunk will be populated via the handler

                                ICustomActionHandler handler = App.PluginManager.GetCustomHandler((uint)resource.Handler);
                                if (handler != null)
                                    extraData.Handler = handler;

                                // add in existing bundles
                                var ebxEntry = am.GetEbxEntry(resource.Name);
                                foreach (int bid in ebxEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(am.GetBundleEntry(bid)));
                                }

                                entry.ExtraData = extraData;
                                modifiedEbx.TryAdd(resource.Name, entry);
                            }

                            // merge new and old data together
                            if (extraData != null)
                                extraData.Data = extraData.Handler.Load(extraData.Data, data);
                        }
                        else
                        {
                            if (modifiedEbx.ContainsKey(resource.Name))
                            {
                                EbxAssetEntry existingEntry = modifiedEbx[resource.Name];

                                if (existingEntry.ExtraData != null)
                                    return;
                                if (existingEntry.Sha1 == resource.Sha1)
                                    return;

                                archiveData[existingEntry.Sha1].RefCount--;
                                if (archiveData[existingEntry.Sha1].RefCount == 0)
                                    archiveData.TryRemove(existingEntry.Sha1, out _);

                                modifiedEbx.TryRemove(resource.Name, out _);
                                numArchiveEntries--;
                            }

                            EbxAssetEntry entry = new EbxAssetEntry();
                            resource.FillAssetEntry(entry);

                            byte[] data = fmod.GetResourceData(resource);
                            var ebxEntry = am.GetEbxEntry(resource.Name);

                            if (data == null)
                            {
                                data = NativeReader.ReadInStream(am.GetRawStream(ebxEntry));

                                entry.Sha1 = ebxEntry.Sha1;
                                entry.OriginalSize = ebxEntry.OriginalSize;
                            }

                            if (ebxEntry != null)
                            {
                                // add in existing bundles
                                foreach (int bid in ebxEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(am.GetBundleEntry(bid)));
                                }
                            }

                            entry.Size = data.Length;

                            modifiedEbx.TryAdd(entry.Name, entry);
                            if (!archiveData.ContainsKey(entry.Sha1))
                                archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                            else
                                archiveData[entry.Sha1].RefCount++;
                            numArchiveEntries++;
                        }
                    }
                }
                else if (resource.Type == ModResourceType.Res)
                {
                    if (resource.IsModified || !modifiedRes.ContainsKey(resource.Name))
                    {
                        if (resource.HasHandler)
                        {
                            ResAssetEntry entry = null;
                            HandlerExtraData extraData = null;
                            byte[] data = fmod.GetResourceData(resource);

                            if (modifiedRes.ContainsKey(resource.Name))
                            {
                                entry = modifiedRes[resource.Name];
                                extraData = (HandlerExtraData)entry.ExtraData;
                            }
                            else
                            {
                                entry = new ResAssetEntry();
                                extraData = new HandlerExtraData();

                                resource.FillAssetEntry(entry);
                                // the rest of the chunk will be populated via the handler

                                ICustomActionHandler handler = App.PluginManager.GetCustomHandler((ResourceType)entry.ResType);
                                if (handler != null)
                                    extraData.Handler = handler;

                                // add in existing bundles
                                var resEntry = am.GetResEntry(resource.Name);
                                foreach (int bid in resEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(am.GetBundleEntry(bid)));
                                }

                                entry.ExtraData = extraData;
                                modifiedRes.TryAdd(resource.Name, entry);
                            }

                            // merge new and old data together
                            if (extraData != null)
                                extraData.Data = extraData.Handler.Load(extraData.Data, data);
                        }
                        else
                        {
                            if (modifiedRes.ContainsKey(resource.Name))
                            {
                                ResAssetEntry existingEntry = modifiedRes[resource.Name];

                                if (existingEntry.ExtraData != null)
                                    return;
                                if (existingEntry.Sha1 == resource.Sha1)
                                    return;

                                archiveData[existingEntry.Sha1].RefCount--;
                                if (archiveData[existingEntry.Sha1].RefCount == 0)
                                    archiveData.TryRemove(existingEntry.Sha1, out _);

                                modifiedRes.TryRemove(resource.Name, out _);
                                numArchiveEntries--;
                            }

                            ResAssetEntry entry = new ResAssetEntry();
                            resource.FillAssetEntry(entry);

                            byte[] data = fmod.GetResourceData(resource);
                            var resEntry = am.GetResEntry(resource.Name);

                            if (data == null)
                            {
                                data = NativeReader.ReadInStream(am.GetRawStream(resEntry));

                                entry.Sha1 = resEntry.Sha1;
                                entry.OriginalSize = resEntry.OriginalSize;
                                entry.ResMeta = resEntry.ResMeta;
                                entry.ResRid = resEntry.ResRid;
                                entry.ResType = resEntry.ResType;
                            }

                            if (resEntry != null)
                            {
                                // add in existing bundles
                                foreach (int bid in resEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(am.GetBundleEntry(bid)));
                                }
                            }

                            entry.Size = data.Length;

                            modifiedRes.TryAdd(entry.Name, entry);
                            if (!archiveData.ContainsKey(entry.Sha1))
                                archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                            else
                                archiveData[entry.Sha1].RefCount++;
                            numArchiveEntries++;
                        }
                    }
                }
                else if (resource.Type == ModResourceType.Chunk)
                {
                    Guid guid = new Guid(resource.Name);
                    if (resource.IsModified || !modifiedChunks.ContainsKey(guid))
                    {
                        if (resource.HasHandler)
                        {
                            ChunkAssetEntry entry = null;
                            HandlerExtraData extraData = null;
                            byte[] data = fmod.GetResourceData(resource);

                            if (modifiedChunks.ContainsKey(guid))
                            {
                                entry = modifiedChunks[guid];
                                extraData = (HandlerExtraData)entry.ExtraData;
                            }
                            else
                            {
                                entry = new ChunkAssetEntry();
                                extraData = new HandlerExtraData();

                                entry.Id = guid;
                                entry.IsTocChunk = resource.IsTocChunk;
                                // the rest of the chunk will be populated via the handler

                                if ((uint)resource.Handler == 0xBD9BFB65)
                                {
                                    // hack to ensure handler for legacy assets is set properly
                                    extraData.Handler = new Frosty.Core.Handlers.LegacyCustomActionHandler();
                                }
                                else
                                {
                                    ICustomActionHandler handler = App.PluginManager.GetCustomHandler((uint)resource.Handler);
                                    if (handler != null)
                                        extraData.Handler = handler;
                                }

                                // add in existing bundles
                                var chunkEntry = am.GetChunkEntry(guid);
                                bundles.Add(chunksBundleHash);
                                foreach (int bid in chunkEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(am.GetBundleEntry(bid)));
                                }

                                entry.ExtraData = extraData;
                                modifiedChunks.TryAdd(guid, entry);
                            }

                            // merge new and old data together
                            extraData.Data = extraData.Handler.Load(extraData.Data, data);
                        }
                        else
                        {
                            if (modifiedChunks.ContainsKey(guid))
                            {
                                ChunkAssetEntry existingEntry = modifiedChunks[guid];
                                if (existingEntry.Sha1 == resource.Sha1)
                                    return;

                                archiveData[existingEntry.Sha1].RefCount--;
                                if (archiveData[existingEntry.Sha1].RefCount == 0)
                                    archiveData.TryRemove(existingEntry.Sha1, out _);

                                modifiedChunks.TryRemove(guid, out _);
                                numArchiveEntries--;
                            }

                            ChunkAssetEntry entry = new ChunkAssetEntry();
                            resource.FillAssetEntry(entry);

                            byte[] data = fmod.GetResourceData(resource);
                            var chunkEntry = am.GetChunkEntry(guid);

                            if (data == null)
                            {
                                data = NativeReader.ReadInStream(am.GetRawStream(chunkEntry));

                                entry.Sha1 = (chunkEntry.Sha1 == Sha1.Zero) ? Utils.GenerateSha1(data) : chunkEntry.Sha1;
                                entry.OriginalSize = chunkEntry.OriginalSize;
                                entry.LogicalSize = chunkEntry.LogicalSize;
                                entry.LogicalOffset = chunkEntry.LogicalOffset;

                                // probably not needed at all
                                if (chunkEntry.RangeStart != 0 && chunkEntry.RangeEnd != 0)
                                {
                                    entry.RangeStart = chunkEntry.RangeStart;
                                    entry.RangeEnd = chunkEntry.RangeEnd;
                                }

                                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5))
                                {
                                    if (fs.GetManifestChunk(chunkEntry.Id) != null)
                                    {
                                        entry.TocChunkSpecialHack = true;
                                        if (chunkEntry.Bundles.Count == 0)
                                        {
                                            resource.ClearAddedBundles();
                                        }
                                    }
                                }
                            }

                            if (chunkEntry != null)
                            {
                                // add in existing bundles
                                bundles.Add(chunksBundleHash);
                                foreach (int bid in chunkEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(am.GetBundleEntry(bid)));
                                }
                            }

                            entry.Size = data.Length;

                            modifiedChunks.TryAdd(guid, entry);
                            if (!archiveData.ContainsKey(entry.Sha1))
                                archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                            else
                                archiveData[entry.Sha1].RefCount++;
                            numArchiveEntries++;
                        }
                    }
                    else
                    {
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5))
                        {
                            var chunkEntry = am.GetChunkEntry(guid);
                            var entry = modifiedChunks[guid];

                            if (fs.GetManifestChunk(chunkEntry.Id) != null)
                            {
                                entry.TocChunkSpecialHack = true;
                                if (chunkEntry.Bundles.Count == 0)
                                    resource.ClearAddedBundles();
                            }
                        }
                    }
                }
                else if (resource.Type == ModResourceType.FsFile)
                {
                    DbObject fileStub;
                    using (DbReader reader = new DbReader(new MemoryStream(fmod.GetResourceData(resource)), null))
                    {
                        fileStub = reader.ReadDbObject();
                    }

                    if (!modifiedFs.ContainsKey(resource.Name))
                    {
                        modifiedFs.TryAdd(resource.Name, null);
                    }

                    modifiedFs[resource.Name] = fileStub;
                }

                // modified bundle actions (these are pulled from the asset manager during applying)
                foreach (int bundleHash in bundles)
                {
                    // Skips bundle if the whitelist has more than one element and doesn't contain the bundle hash
                    if (App.WhitelistedBundles.Count != 0 && !App.WhitelistedBundles.Contains(bundleHash))
                    {
                        continue;
                    }

                    modifiedBundles.TryAdd(bundleHash, new ModBundleInfo() { Name = bundleHash });

                    ModBundleInfo modBundle = modifiedBundles[bundleHash];
                    switch (resource.Type)
                    {
                        case ModResourceType.Ebx: modBundle.Modify.AddEbx(resource.Name); break;
                        case ModResourceType.Res: modBundle.Modify.AddRes(resource.Name); break;
                        case ModResourceType.Chunk: modBundle.Modify.AddChunk(new Guid(resource.Name)); break;
                    }
                }

                // add bundle actions (these are stored in the mod)
                foreach (int bundleHash in resource.AddedBundles)
                {
                    // Skips bundle if the whitelist has more than one element and doesn't contain the bundle hash
                    if (App.WhitelistedBundles.Count != 0 && !App.WhitelistedBundles.Contains(bundleHash))
                    {
                        continue;
                    }

                    modifiedBundles.TryAdd(bundleHash, new ModBundleInfo() { Name = bundleHash });

                    ModBundleInfo modBundle = modifiedBundles[bundleHash];
                    switch (resource.Type)
                    {
                        case ModResourceType.Ebx: modBundle.Add.AddEbx(resource.Name); break;
                        case ModResourceType.Res: modBundle.Add.AddRes(resource.Name); break;
                        case ModResourceType.Chunk: modBundle.Add.AddChunk(new Guid(resource.Name)); break;
                    }
                }
            });
        }

        private void ProcessLegacyModResources(string modPath)
        {
            DbObject mod = null;
            using (DbReader reader = new DbReader(new FileStream(modPath, FileMode.Open, FileAccess.Read), null))
                mod = reader.ReadDbObject();

            string magic = mod.GetValue<string>("magic");
            int ver = int.Parse(magic.Replace("FBMODV", ""));

            // obtain bundles to modify
            DbObject resourceList = mod.GetValue<DbObject>("resources");
            foreach (DbObject action in mod.GetValue<DbObject>("actions"))
            {
                int bundle = Fnv1.HashString(action.GetValue<string>("bundle").ToLower());
                string actionType = action.GetValue<string>("type");
                int resourceId = action.GetValue<int>("resourceId");

                if (!modifiedBundles.ContainsKey(bundle))
                    modifiedBundles.TryAdd(bundle, new ModBundleInfo() { Name = bundle });

                ModBundleInfo modBundle = modifiedBundles[bundle];
                DbObject resource = resourceList[resourceId] as DbObject;

                string resName = resource.GetValue<string>("name");
                string resType = resource.GetValue<string>("type");

                if (actionType == "modify")
                {
                    switch (resType)
                    {
                        case "ebx": modBundle.Modify.Ebx.Add(resName); break;
                        case "res": modBundle.Modify.Res.Add(resName); break;
                        case "chunk": modBundle.Modify.Chunks.Add(new Guid(resName)); break;
                    }
                }
                else if (actionType == "add")
                {
                    switch (resType)
                    {
                        case "ebx": modBundle.Add.Ebx.Add(resName); break;
                        case "res": modBundle.Add.Res.Add(resName); break;
                        case "chunk": modBundle.Add.Chunks.Add(new Guid(resName)); break;
                    }
                }
                else if (actionType == "remove")
                {
                    switch (resType)
                    {
                        case "ebx": modBundle.Remove.Ebx.Add(resName); break;
                        case "res": modBundle.Remove.Res.Add(resName); break;
                        case "chunk": modBundle.Remove.Chunks.Add(new Guid(resName)); break;
                    }
                }
            }

            // obtain resources to modify
            foreach (DbObject resource in resourceList)
            {
                string resourceType = resource.GetValue<string>("type");
                if (resourceType == "superbundle")
                {
                    string name = resource.GetValue<string>("name");
                    addedSuperBundles.Add(name);
                }
                else if (resourceType == "bundle")
                {
                    string name = resource.GetValue<string>("name");
                    string superBundle = resource.GetValue<string>("sb");

                    int hash = Fnv1a.HashString(superBundle.ToLower());
                    if (!addedBundles.ContainsKey(hash))
                        addedBundles.TryAdd(hash, new List<string>());

                    addedBundles[hash].Add(name);
                }
                else if (resourceType == "ebx")
                {
                    string name = resource.GetValue<string>("name");

                    if (modifiedEbx.ContainsKey(name))
                    {
                        EbxAssetEntry existingEntry = modifiedEbx[name];
                        if (existingEntry.Sha1 == resource.GetValue<Sha1>("sha1"))
                            continue;

                        archiveData[existingEntry.Sha1].RefCount--;
                        if (archiveData[existingEntry.Sha1].RefCount == 0)
                            archiveData.TryRemove(existingEntry.Sha1, out _);

                        modifiedEbx.TryRemove(name, out _);
                        numArchiveEntries--;
                    }

                    EbxAssetEntry entry = new EbxAssetEntry
                    {
                        Name = name,
                        OriginalSize = resource.GetValue<long>("uncompressedSize"),
                        Size = resource.GetValue<long>("compressedSize")
                    };

                    byte[] buffer = null;
                    if (resource.HasValue("archiveIndex"))
                    {
                        entry.IsInline = resource.GetValue<bool>("shouldInline");
                        buffer = GetResourceData(modPath, resource.GetValue<int>("archiveIndex"), resource.GetValue<long>("archiveOffset"), (int)entry.Size);
                    }
                    else
                    {
                        ManifestFileRef fileRef = resource.GetValue<int>("file");
                        long offset = resource.GetValue<int>("offset");

                        using (NativeReader reader = new NativeReader(new FileStream(fs.ResolvePath(fileRef), FileMode.Open, FileAccess.Read)))
                        {
                            reader.Position = offset;
                            buffer = reader.ReadBytes((int)entry.Size);
                        }
                    }

                    entry.Sha1 = Utils.GenerateSha1(buffer);

                    modifiedEbx.TryAdd(entry.Name, entry);
                    if (!archiveData.ContainsKey(entry.Sha1))
                        archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = buffer, RefCount = 1 });
                    else
                        archiveData[entry.Sha1].RefCount++;
                    numArchiveEntries++;
                }
                else if (resourceType == "res")
                {
                    string name = resource.GetValue<string>("name");

                    if (modifiedRes.ContainsKey(name))
                    {
                        ResAssetEntry existingEntry = modifiedRes[name];
                        if (existingEntry.Sha1 == resource.GetValue<Sha1>("sha1"))
                            continue;

                        archiveData[existingEntry.Sha1].RefCount--;
                        if (archiveData[existingEntry.Sha1].RefCount == 0)
                            archiveData.TryRemove(existingEntry.Sha1, out _);

                        modifiedRes.TryRemove(name, out _);
                        numArchiveEntries--;
                    }

                    ResAssetEntry entry = new ResAssetEntry
                    {
                        Name = name,
                        OriginalSize = resource.GetValue<long>("uncompressedSize"),
                        Size = resource.GetValue<long>("compressedSize"),
                        ResRid = (ulong)resource.GetValue<long>("resRid"),
                        ResType = (uint)resource.GetValue<int>("resType"),
                        ResMeta = resource.GetValue<byte[]>("resMeta")
                    };

                    byte[] buffer = null;
                    if (resource.HasValue("archiveIndex"))
                    {
                        entry.IsInline = resource.GetValue<bool>("shouldInline");
                        buffer = GetResourceData(modPath, resource.GetValue<int>("archiveIndex"), resource.GetValue<long>("archiveOffset"), (int)entry.Size);
                    }
                    else
                    {
                        ManifestFileRef fileRef = resource.GetValue<int>("file");
                        long offset = resource.GetValue<int>("offset");

                        using (NativeReader reader = new NativeReader(new FileStream(fs.ResolvePath(fileRef), FileMode.Open, FileAccess.Read)))
                        {
                            reader.Position = offset;
                            buffer = reader.ReadBytes((int)entry.Size);
                        }
                    }

                    entry.Sha1 = Utils.GenerateSha1(buffer);

                    modifiedRes.TryAdd(entry.Name, entry);
                    if (!archiveData.ContainsKey(entry.Sha1))
                        archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = buffer, RefCount = 1 });
                    else
                        archiveData[entry.Sha1].RefCount++;
                    numArchiveEntries++;
                }
                else if (resourceType == "chunk")
                {
                    Guid chunkId = new Guid(resource.GetValue<string>("name"));
                    if (modifiedChunks.ContainsKey(chunkId))
                    {
                        ChunkAssetEntry existingEntry = modifiedChunks[chunkId];
                        if (existingEntry.Sha1 == resource.GetValue<Sha1>("sha1"))
                            continue;

                        archiveData[existingEntry.Sha1].RefCount--;
                        if (archiveData[existingEntry.Sha1].RefCount == 0)
                            archiveData.TryRemove(existingEntry.Sha1, out _);

                        modifiedChunks.TryRemove(chunkId, out _);
                        numArchiveEntries--;
                    }

                    ChunkAssetEntry entry = new ChunkAssetEntry
                    {
                        Id = chunkId,
                        Size = resource.GetValue<long>("compressedSize"),
                        LogicalOffset = resource.GetValue<uint>("logicalOffset"),
                        LogicalSize = resource.GetValue<uint>("logicalSize"),
                        RangeStart = resource.GetValue<uint>("rangeStart"),
                        RangeEnd = resource.GetValue<uint>("rangeEnd"),
                        FirstMip = resource.GetValue<int>("firstMip", -1),
                        H32 = resource.GetValue<int>("h32", 0),
                        IsTocChunk = resource.GetValue<bool>("tocChunk")
                    };

                    byte[] buffer = null;
                    if (resource.HasValue("archiveIndex"))
                    {
                        // obtain data from archive
                        entry.IsInline = resource.GetValue<bool>("shouldInline", false);
                        buffer = GetResourceData(modPath, resource.GetValue<int>("archiveIndex"), resource.GetValue<long>("archiveOffset"), (int)entry.Size);
                    }
                    else
                    {
                        ManifestFileRef fileRef = resource.GetValue<int>("file");
                        long offset = resource.GetValue<int>("offset");

                        // obtain data from cas file location
                        using (NativeReader reader = new NativeReader(new FileStream(fs.ResolvePath(fileRef), FileMode.Open, FileAccess.Read)))
                        {
                            reader.Position = offset;
                            buffer = reader.ReadBytes((int)entry.Size);
                        }

                        if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5))
                        {
                            if (entry.LogicalOffset != 0)
                            {
                                // calculate range values from cas data
                                using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
                                {
                                    int totalSize = 0;
                                    while (totalSize != entry.LogicalOffset)
                                    {
                                        int uncompressedSize = reader.ReadInt(Endian.Big);
                                        ushort compressCode = reader.ReadUShort(Endian.Big);
                                        ushort blockSize = reader.ReadUShort(Endian.Big);

                                        totalSize += uncompressedSize;
                                        if (totalSize > entry.LogicalOffset)
                                        {
                                            reader.Position -= 8;
                                            break;
                                        }

                                        reader.Position += blockSize;
                                    }

                                    entry.RangeStart = (uint)reader.Position;
                                    entry.RangeEnd = (uint)buffer.Length;
                                }
                            }
                        }
                    }

                    entry.Sha1 = Utils.GenerateSha1(buffer);

                    modifiedChunks.TryAdd(entry.Id, entry);
                    if (!archiveData.ContainsKey(entry.Sha1))
                        archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = buffer, RefCount = 1 });
                    else
                        archiveData[entry.Sha1].RefCount++;
                    numArchiveEntries++;

                    if (ver < 2)
                    {
                        // previous mod format versions had no action listed for toc chunk changes
                        // so now have to manually add an action for it.
                        if (!modifiedBundles.ContainsKey(chunksBundleHash))
                            modifiedBundles.TryAdd(chunksBundleHash, new ModBundleInfo() { Name = chunksBundleHash });
                        ModBundleInfo chunksBundle = modifiedBundles[chunksBundleHash];
                        chunksBundle.Modify.Chunks.Add(entry.Id);

                        // new code requires first mip to be set to modify range values, however
                        // old mods didnt modify this. So lets force it, hopefully not too many
                        // issues result from this.
                        entry.FirstMip = 0;
                    }

                    if (entry.FirstMip == -1 && entry.RangeEnd != 0)
                        entry.FirstMip = 0;
                }
            }
        }

        public int Run(FileSystemManager inFs, CancellationToken cancelToken, ILogger inLogger, string rootPath, string modPackName, string additionalArgs, params string[] modPaths)
        {
            modDirName = "ModData\\" + modPackName;
            cancelToken.ThrowIfCancellationRequested();

            App.Logger.Log("Launching");

            fs = inFs;
            Logger = inLogger;

            string modDataPath = fs.BasePath + modDirName + "\\";
            string patchPath = "Patch";
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4,
                ProfileVersion.NeedForSpeed,
                ProfileVersion.PlantsVsZombiesGardenWarfare2,
                ProfileVersion.NeedForSpeedRivals))
            {
                patchPath = "Update\\Patch\\Data";
            }

            // bfn and bfv dont have a patch directory
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.Battlefield5))
            {
                patchPath = "Data";
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden20))
            {
                string lcuPath = Environment.ExpandEnvironmentVariables(@"%ProgramData%\Frostbite\Madden NFL 20");
                if (Directory.Exists(lcuPath))
                {
                    DirectoryInfo di = new DirectoryInfo(lcuPath);
                    FrostyMessageBox.Show("Frosty has detected the presence of a Live Content Update. This must be removed and the game taken offline for mods to take affect.\r\n\r\nThe location of this update is:\r\n\r\n" + di.FullName, "Frosty Editor");
                    return -1;
                }
            }

            // check for already running process
            if (!Config.Get<bool>("DisableLaunchProcessCheck", false))
            {
                Process[] processes = Process.GetProcesses();
                string processName = ProfilesLibrary.ProfileName;
                foreach (Process process in processes)
                {
                    if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        FrostyMessageBox.Show(string.Format("Unable to launch process as there is already a running process with process Id {0}", process.Id), "Frosty Toolsuite");
                        return -1;
                    }
                }
            }

            cancelToken.ThrowIfCancellationRequested();

            cancelToken.ThrowIfCancellationRequested();
            Logger.Log("Loading Mods");

            bool needsModding = false;
            if (!File.Exists(Path.Combine(modDataPath, patchPath, "mods.json")))
                needsModding = true;
            else
            {
                List<ModInfo> oldModInfoList = JsonConvert.DeserializeObject<List<ModInfo>>(File.ReadAllText(Path.Combine(modDataPath, patchPath, "mods.json")));
                List<ModInfo> currentModInfoList = GenerateModInfoList(modPaths, rootPath);

                // check if the mod data needs recreating
                // ie. mod change or patch
                if (!IsSamePatch(modDataPath + patchPath) || !oldModInfoList.SequenceEqual(currentModInfoList))
                    needsModding = true;
            }

            cancelToken.ThrowIfCancellationRequested();
            if (needsModding)
            {
                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Initializing Resources");

                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    foreach (string catalogName in fs.Catalogs)
                    {
                        Dictionary<int, Dictionary<uint, CatResourceEntry>> entries = LoadCatalog(fs, "native_data/" + catalogName + "/cas.cat", out int hash);
                        if (entries != null)
                            resources.Add(hash, entries);

                        entries = LoadCatalog(fs, "native_patch/" + catalogName + "/cas.cat", out hash);
                        if (entries != null)
                            resources.Add(hash, entries);
                    }
                }

                rm = new ResourceManager(fs);
                rm.SetLogger(logger);
                rm.Initialize();

                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Loading " + ProfilesLibrary.CacheName + ".cache");

                am = new AssetManager(fs, rm);
                am.SetLogger(logger);
                am.Initialize(additionalStartup: false);

                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Loading Mods");
                App.Logger.Log("Loading Mods");

                // Get Full Modlist
                List<FrostyMod> modList = new List<FrostyMod>();
                foreach (string path in modPaths)
                {
                    FileInfo fi = new FileInfo(Path.Combine(rootPath, path));

                    if (fi.Extension == ".fbmod")
                        modList.Add(new FrostyMod(fi.FullName));

                    else if (fi.Extension == ".fbcollection")
                    {
                        foreach (FrostyMod mod in new FrostyModCollection(fi.FullName).Mods)
                            modList.Add(mod);
                    }
                }

                // Load Mod Resources
                int currentMod = 0;
                foreach (FrostyMod mod in modList)
                {
                    Logger.Log($"Loading Mods ({mod.ModDetails.Title})");
                    if (mod.NewFormat)
                    {
                        ProcessModResources(mod);
                    }
                    else
                    {
                        ProcessLegacyModResources(mod.Path);
                    }
                    ReportProgress(currentMod++, modList.Count);
                }

                Logger.Log("Applying Handlers");
                App.Logger.Log("Applying Handlers");

                // apply handlers
                RuntimeResources runtimeResources = new RuntimeResources();

                List<AssetEntry> assetEntries = new List<AssetEntry>();
                assetEntries.AddRange(modifiedEbx.Values);
                assetEntries.AddRange(modifiedRes.Values);
                assetEntries.AddRange(modifiedChunks.Values);

                int currentResource = 0;
                Parallel.ForEach(assetEntries, entry =>
                {
                    if (entry.ExtraData is HandlerExtraData handlerExtaData)
                    {
                        handlerExtaData.Handler.Modify(entry, am, runtimeResources, handlerExtaData.Data, out byte[] data);

                        if (!archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 }))
                            archiveData[entry.Sha1].RefCount++;
                    }
                    ReportProgress(currentResource++, assetEntries.Count);
                    Logger.Log($"Applying Handlers ({currentResource}/{assetEntries.Count})");
                });

                // process any new resources added during custom handler modification
                ProcessModResources(runtimeResources);

                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Cleaning Up ModData");
                App.Logger.Log("Cleaning Up ModData");

                List<SymLinkStruct> cmdArgs = new List<SymLinkStruct>();
                bool newInstallation = false;

                fs.ResetManifest();
                if (!DeleteSelectFiles(modDataPath + patchPath))
                {
                    if (!Directory.Exists(modDataPath))
                    {
                        newInstallation = true;
                        Logger.Log("Creating ModData");

                        // create mod path
                        Directory.CreateDirectory(modDataPath);

                        if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.StarWarsSquadrons))
                        {
                            if (!Directory.Exists(modDataPath + "Data"))
                                Directory.CreateDirectory(modDataPath + "Data");
                            cmdArgs.Add(new SymLinkStruct(modDataPath + "Data/Win32", fs.BasePath + "Data/Win32", true));
                        }
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5)) //bfv doesnt have a patch directory so we need to rebuild the data folder structure instead
                        {
                            if (!Directory.Exists(modDataPath + "Data"))
                                Directory.CreateDirectory(modDataPath + "Data");

                            foreach (string casFilename in Directory.EnumerateFiles(fs.BasePath + patchPath, "*.cas", SearchOption.AllDirectories))
                            {
                                FileInfo casFi = new FileInfo(casFilename);
                                string destPath = casFi.Directory.FullName.ToLower().Replace("\\" + patchPath.ToLower(), "\\" + modDirName.ToLower() + "\\" + patchPath.ToLower());
                                string tempPath = Path.Combine(destPath, casFi.Name);

                                if (!Directory.Exists(destPath))
                                    Directory.CreateDirectory(destPath);

                                cmdArgs.Add(new SymLinkStruct(tempPath, casFi.FullName, false));
                            }
                        }
                        else
                        {
                            // data path
                            cmdArgs.Add(new SymLinkStruct(modDataPath + "Data", fs.BasePath + "Data", true));
                        }

                        if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                            ProfileVersion.Battlefield4,
                            ProfileVersion.NeedForSpeed,
                            ProfileVersion.PlantsVsZombiesGardenWarfare2,
                            ProfileVersion.NeedForSpeedRivals))
                        {
                            // create update dir if it does not exist
                            if (!Directory.Exists(modDataPath + "Update"))
                                Directory.CreateDirectory(modDataPath + "Update");

                            // update paths
                            foreach (string path in Directory.EnumerateDirectories(fs.BasePath + "Update"))
                            {
                                DirectoryInfo di = new DirectoryInfo(path);

                                // ignore the patch directory
                                if (di.Name.ToLower() != "patch")
                                    cmdArgs.Add(new SymLinkStruct(modDataPath + "Update/" + di.Name, di.FullName, true));
                            }
                        }
                        else if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17))
                        {
                            // update path
                            cmdArgs.Add(new SymLinkStruct(modDataPath + "Update", fs.BasePath + "Update", true));
                        }

                        if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19,
                            ProfileVersion.Madden20,
                            ProfileVersion.Fifa20,
                            ProfileVersion.Anthem,
                            ProfileVersion.NeedForSpeedHeat,
                            ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                            ProfileVersion.Fifa21))
                        {
                            foreach (string casFilename in Directory.EnumerateFiles(fs.BasePath + patchPath, "*.cas", SearchOption.AllDirectories))
                            {
                                FileInfo casFi = new FileInfo(casFilename);
                                string destPath = casFi.Directory.FullName.ToLower().Replace("\\" + patchPath.ToLower(), "\\" + modDirName.ToLower() + "\\" + patchPath.ToLower());
                                string tempPath = Path.Combine(destPath, casFi.Name);

                                if (!Directory.Exists(destPath))
                                    Directory.CreateDirectory(destPath);

                                cmdArgs.Add(new SymLinkStruct(tempPath, casFi.FullName, false));
                            }
                        }
                    }
                }

                // add cas files to link
                foreach (string catalog in fs.Catalogs)
                {
                    string path = fs.ResolvePath("native_patch/" + catalog + "/cas.cat");
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5)) //again, no patch directory. fun.
                    {
                        path = fs.ResolvePath("native_data/" + catalog + "/cas.cat");
                    }
                    if (!File.Exists(path))
                        continue;

                    FileInfo catInfo = new FileInfo(path);
                    string destPath = catInfo.Directory.FullName.Replace("\\" + patchPath.ToLower(), "\\" + modDirName.ToLower() + "\\" + patchPath.ToLower());

                    if (!Directory.Exists(destPath))
                        Directory.CreateDirectory(destPath);

                    foreach (FileInfo fi in catInfo.Directory.GetFiles())
                    {
                        string tempPath = Path.Combine(destPath, fi.Name);
                        if (fi.Extension == ".cas")
                        {
                            if (File.Exists(tempPath))
                                continue;
                            cmdArgs.Add(new SymLinkStruct(tempPath, fi.FullName, false));
                        }
                        else if (fi.Extension == ".cat")
                            fi.CopyTo(tempPath, false);
                    }
                }

                if (cmdArgs.Count > 0)
                {
                    string reason = "New patch detected.";
                    if (newInstallation)
                        reason = "New installation detected.";

                    FrostyMessageBox.Show(reason + "\r\n\r\nShortly you will be prompted for elevated privileges, this is required to create symbolic links between the original data and the new modified data. Please ensure that you accept this to avoid any issues.", "Frosty Toolsuite");
                    if (!RunSymbolicLinkProcess(cmdArgs))
                    {
                        Directory.Delete(modDataPath, true);
                        throw new FrostySymLinkException();
                    }
                }

                // set max threads to processor amount (stop hitching)
                ThreadPool.GetMaxThreads(out int workerThreads, out int completionPortThreads);
                ThreadPool.SetMaxThreads(Config.Get("ApplyingThreadCount", Environment.ProcessorCount), completionPortThreads);

                // modify tocs and sbs
                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Applying Mods");
                App.Logger.Log("Applying Mods");

                cmdArgs.Clear();

                // nfs heat and pvzbfn
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.NeedForSpeedHeat, ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.Fifa21))
                {
                    CasBundleAction.CasFiles.Clear();
                    foreach (string catalog in fs.Catalogs)
                    {
                        int casIndex = 1;
                        string path = fs.BasePath + "Patch\\" + catalog + "\\cas_" + casIndex.ToString("D2") + ".cas";

                        while (File.Exists(path))
                        {
                            casIndex++;
                            path = fs.BasePath + "Patch\\" + catalog + "\\cas_" + casIndex.ToString("D2") + ".cas";
                        }

                        CasBundleAction.CasFiles.Add(catalog, casIndex);
                    }

                    List<CasBundleAction> actions = new List<CasBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    // @todo: Added bundles

                    int totalTasks = 0;
                    foreach (SuperBundleInfo superBundle in fs.EnumerateSuperBundleInfos())
                    {
                        if (fs.ResolvePath(superBundle.Name + ".toc") == "")
                            continue;

                        CasBundleAction action = new CasBundleAction(superBundle, doneEvent, this);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        numTasks++;
                        totalTasks++;
                    }

                    while (numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        ReportProgress(totalTasks - numTasks, totalTasks);
                        Thread.Sleep(1);
                    }

                    foreach (CasBundleAction completedAction in actions)
                    {
                        if (completedAction.HasErrored)
                        {
                            // if any of the threads caused an exception, throw it to the global handler
                            // as the game data is now in an inconsistent state
                            throw completedAction.Exception;
                        }

                        string srcPath = fs.ResolvePath(completedAction.SuperBundleInfo.Name + ".toc");
                        FileInfo sbFi = new FileInfo(modDataPath + patchPath + "/" + completedAction.SuperBundleInfo.Name + ".toc");
                        if (!sbFi.Exists)
                        {
                            Directory.CreateDirectory(sbFi.DirectoryName);
                            cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                        }

                        foreach (string catalog in completedAction.SuperBundleInfo.SplitSuperBundles)
                        {
                            string name = completedAction.SuperBundleInfo.Name.Replace("win32", catalog);
                            srcPath = fs.ResolvePath(name + ".toc");
                            sbFi = new FileInfo(modDataPath + patchPath + "/" + name + ".toc");
                            if (!sbFi.Exists)
                            {
                                Directory.CreateDirectory(sbFi.DirectoryName);
                                cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                            }
                        }

                        if (completedAction.HasSb)
                        {
                            srcPath = fs.ResolvePath(completedAction.SuperBundleInfo.Name + ".sb");
                            sbFi = new FileInfo(modDataPath + patchPath + "/" + completedAction.SuperBundleInfo.Name + ".sb");
                            if (!sbFi.Exists)
                            {
                                Directory.CreateDirectory(sbFi.DirectoryName);
                                cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                            }

                            foreach (string catalog in completedAction.SuperBundleInfo.SplitSuperBundles)
                            {
                                string name = completedAction.SuperBundleInfo.Name.Replace("win32", catalog);
                                srcPath = fs.ResolvePath(name + ".sb");
                                sbFi = new FileInfo(modDataPath + patchPath + "/" + name + ".sb");
                                if (!sbFi.Exists)
                                {
                                    Directory.CreateDirectory(sbFi.DirectoryName);
                                    cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                                }
                            }
                        }
                    }
                }



                // fifa19-20 and madden20
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.Madden20))
                {
                    DbObject layout = null;
                    using (DbReader reader = new DbReader(new FileStream(fs.BasePath + patchPath + "/layout.toc", FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
                        layout = reader.ReadDbObject();

                    FifaBundleAction.CasFileCount = fs.CasFileCount;
                    List<FifaBundleAction> actions = new List<FifaBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    // @todo: Added bundles

                    int totalTasks = 0;
                    foreach (CatalogInfo ci in fs.EnumerateCatalogInfos())
                    {
                        FifaBundleAction action = new FifaBundleAction(ci, doneEvent, this, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        numTasks++;
                        totalTasks++;
                    }

                    while (numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        ReportProgress(totalTasks - numTasks, totalTasks);
                        Thread.Sleep(1);
                    }

                    foreach (FifaBundleAction completedAction in actions)
                    {
                        if (completedAction.HasErrored)
                        {
                            // if any of the threads caused an exception, throw it to the global handler
                            // as the game data is now in an inconsistent state
                            throw completedAction.Exception;
                        }

                        if (completedAction.CasFiles.Count > 0)
                        {
                            DbObject installManifest = layout.GetValue<DbObject>("installManifest");
                            foreach (DbObject installChunk in installManifest.GetValue<DbObject>("installChunks"))
                            {
                                if (completedAction.CatalogInfo.Name.Equals("win32/" + installChunk.GetValue<string>("name")))
                                {
                                    foreach (int idx in completedAction.CasFiles.Keys)
                                    {
                                        DbObject casFileEntry = DbObject.CreateObject();
                                        casFileEntry.SetValue("id", idx);
                                        casFileEntry.SetValue("path", completedAction.CasFiles[idx]);
                                        installChunk.GetValue<DbObject>("files").Add(casFileEntry);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // write out layout.toc with additional cas entries where required
                    using (DbWriter writer = new DbWriter(new FileStream(modDataPath + patchPath + "/layout.toc", FileMode.Create), true))
                        writer.Write(layout);
                }

                // swbf2, bfv, and sws
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    List<ManifestBundleAction> actions = new List<ManifestBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    if (addedBundles.Count != 0)
                    {
                        int hash = Fnv1a.HashString("<none>");
                        foreach (string bundleName in addedBundles[hash])
                            fs.AddManifestBundle(new ManifestBundleInfo() { hash = Fnv1.HashString(bundleName) });
                    }

                    Dictionary<string, List<ModBundleInfo>> tasks = new Dictionary<string, List<ModBundleInfo>>();
                    foreach (ModBundleInfo bundle in modifiedBundles.Values)
                    {
                        if (bundle.Name.Equals(chunksBundleHash))
                            continue;

                        ManifestBundleInfo manifestBundle = fs.GetManifestBundle(bundle.Name);
                        string catalog = fs.GetCatalog(manifestBundle.files[0].file);

                        if (!tasks.ContainsKey(catalog))
                            tasks.Add(catalog, new List<ModBundleInfo>());

                        tasks[catalog].Add(bundle);
                    }

                    int totalTasks = 0;
                    foreach (List<ModBundleInfo> task in tasks.Values)
                    {
                        ManifestBundleAction action = new ManifestBundleAction(task, doneEvent, this, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        numTasks++;
                        totalTasks++;
                    }

                    while (numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        ReportProgress(totalTasks - numTasks, totalTasks);
                        Thread.Sleep(1);
                    }

                    foreach (ManifestBundleAction completedAction in actions)
                    {
                        if (completedAction.Exception != null)
                        {
                            // if any of the threads caused an exception, throw it to the global handler
                            // as the game data is now in an inconsistent state
                            throw completedAction.Exception;
                        }

                        if (completedAction.DataRefs.Count > 0)
                        {
                            // add bundle data to archive
                            for (int i = 0; i < completedAction.BundleRefs.Count; i++)
                            {
                                if (!archiveData.ContainsKey(completedAction.BundleRefs[i]))
                                    archiveData.TryAdd(completedAction.BundleRefs[i], new ArchiveInfo() { Data = completedAction.BundleBuffers[i] });
                            }

                            // add refs to be added to cas (and manifest)
                            for (int i = 0; i < completedAction.DataRefs.Count; i++)
                                casData.Add(fs.GetCatalog(completedAction.FileInfos[i].FileInfo.file), completedAction.DataRefs[i], completedAction.FileInfos[i].Entry, completedAction.FileInfos[i].FileInfo);
                        }
                    }

                    // now process manifest chunk changes
                    if (modifiedBundles.ContainsKey(chunksBundleHash))
                    {
                        foreach (Guid id in modifiedBundles[chunksBundleHash].Modify.Chunks)
                        {
                            ChunkAssetEntry entry = modifiedChunks[id];
                            ManifestChunkInfo ci = fs.GetManifestChunk(entry.Id);

                            if (ci != null)
                            {
                                if (entry.TocChunkSpecialHack)
                                {
                                    // change to using the first catalog
                                    ci.file.file = new ManifestFileRef(0, false, 0);
                                }

                                casData.Add(fs.GetCatalog(ci.file.file), entry.Sha1, entry, ci.file);
                            }
                        }
                        foreach (Guid id in modifiedBundles[chunksBundleHash].Add.Chunks)
                        {
                            ChunkAssetEntry entry = modifiedChunks[id];

                            ManifestChunkInfo ci = new ManifestChunkInfo
                            {
                                guid = entry.Id,
                                file = new ManifestFileInfo { file = new ManifestFileRef(0, false, 0), isChunk = true }
                            };
                            fs.AddManifestChunk(ci);

                            casData.Add(fs.GetCatalog(ci.file.file), entry.Sha1, entry, ci.file);
                        }
                    }
                }

                // remaining games
                else
                {
                    List<SuperBundleAction> actions = new List<SuperBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    int totalTasks = 0;
                    foreach (string superBundle in fs.SuperBundles)
                    {
                        if (fs.ResolvePath(superBundle + ".toc") == "")
                            continue;

                        SuperBundleAction action = new SuperBundleAction(superBundle, doneEvent, this, modDirName + "/" + patchPath, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        numTasks++;
                        totalTasks++;
                    }

                    foreach (string superBundle in addedSuperBundles)
                    {
                        SuperBundleAction action = new SuperBundleAction(superBundle, doneEvent, this, modDirName + "/" + patchPath, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        numTasks++;
                        totalTasks++;
                    }

                    while (numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        logger.Log("progress:" + ((totalTasks - numTasks) / (double)totalTasks) * 100.0d);
                        Thread.Sleep(1);
                    }

                    foreach (SuperBundleAction completedAction in actions)
                    {
                        if (completedAction.HasErrored)
                        {
                            // if any of the threads caused an exception, throw it to the global handler
                            // as the game data is now in an inconsistent state
                            throw completedAction.Exception;
                        }

                        if (!completedAction.TocModified)
                        {
                            string srcPath = fs.ResolvePath(completedAction.SuperBundle + ".toc");
                            FileInfo sbFi = new FileInfo(modDataPath + patchPath + "/" + completedAction.SuperBundle + ".toc");

                            if (!Directory.Exists(sbFi.DirectoryName))
                                Directory.CreateDirectory(sbFi.DirectoryName);

                            cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                        }

                        if (!completedAction.SbModified)
                        {
                            string srcPath = fs.ResolvePath(completedAction.SuperBundle + ".sb");
                            FileInfo sbFi = new FileInfo(modDataPath + patchPath + "/" + completedAction.SuperBundle + ".sb");

                            if (!Directory.Exists(sbFi.DirectoryName))
                                Directory.CreateDirectory(sbFi.DirectoryName);

                            cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                        }

                        if (completedAction.CasRefs.Count != 0)
                        {
                            string catalogPath = fs.GetCatalogFromSuperBundle(completedAction.SuperBundle);
                            for (int i = 0; i < completedAction.CasRefs.Count; i++)
                                casData.Add(catalogPath, completedAction.CasRefs[i]);
                        }
                    }
                }

                cancelToken.ThrowIfCancellationRequested();
                if (cmdArgs.Count > 0)
                {
                    RunSymbolicLinkProcess(cmdArgs);
                }

                // reset threadpool
                ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);

                Logger.Log("Writing Archive Data");
                App.Logger.Log("Writing Archive Data");

                int totalEntries = casData.CountEntries();
                int currentEntry = 0;
                if (totalEntries > 0)
                {
                    ReportProgress(currentEntry, totalEntries);
                }

                // write out cas and modify cats
                foreach (CasDataEntry entry in casData.EnumerateEntries())
                {
                    if (!entry.HasEntries)
                        continue;

                    cancelToken.ThrowIfCancellationRequested();
                    if (!File.Exists(modDataPath + patchPath + "\\" + entry.Catalog + "\\cas.cat"))
                    {
                        if (!File.Exists(fs.BasePath + "data\\" + entry.Catalog + "\\cas.cat"))
                            continue;

                        using (NativeReader reader = new NativeReader(new FileStream(fs.BasePath + "data\\" + entry.Catalog + "\\cas.cat", FileMode.Open, FileAccess.Read)))
                        {
                            FileInfo fi = new FileInfo(modDataPath + patchPath + "\\" + entry.Catalog + "\\cas.cat");
                            if (!fi.Directory.Exists)
                                Directory.CreateDirectory(fi.Directory.FullName);

                            using (NativeWriter writer = new NativeWriter(new FileStream(modDataPath + patchPath + "\\" + entry.Catalog + "\\cas.cat", FileMode.Create)))
                            {
                                writer.Write(reader.ReadBytes(0x23C));
                                writer.Write(0x00);
                                writer.Write(0x00);
                                if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda,
                                    ProfileVersion.Fifa17,
                                    ProfileVersion.Fifa18,
                                    ProfileVersion.StarWarsBattlefrontII,
                                    ProfileVersion.NeedForSpeedPayback,
                                    ProfileVersion.Madden19,
                                    ProfileVersion.Battlefield5,
                                    ProfileVersion.StarWarsSquadrons))
                                {
                                    writer.Write(0x00);
                                    writer.Write(0x00);
                                    writer.Write(-1);
                                    writer.Write(-1);
                                }
                            }
                        }
                    }

                    WriteArchiveData(modDataPath + patchPath + "\\" + entry.Catalog, entry);

                    ReportProgress(currentEntry++, totalEntries);
                }

                cancelToken.ThrowIfCancellationRequested();

                Logger.Log("Writing Manifest");
                App.Logger.Log("Writing Manifest");

                // finally copy in the left over patch data
                if (modifiedFs.Count > 0)
                {
                    fs.WriteInitFs(fs.BasePath + patchPath + "/initfs_win32", modDataPath + patchPath + "/initfs_win32", modifiedFs);
                }
                else
                {
                    CopyFileIfRequired(fs.BasePath + patchPath + "/initfs_win32", modDataPath + patchPath + "/initfs_win32");
                }


                if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                    ProfileVersion.Battlefield4,
                    ProfileVersion.NeedForSpeed,
                    ProfileVersion.PlantsVsZombiesGardenWarfare2,
                    ProfileVersion.NeedForSpeedRivals))
                {
                    // modify layout.toc for any new superbundles added
                    DbObject layout = null;
                    using (DbReader reader = new DbReader(new FileStream(fs.BasePath + patchPath + "/layout.toc", FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
                        layout = reader.ReadDbObject();

                    foreach (string path in Directory.EnumerateFiles(modDataPath + patchPath, "*.sb", SearchOption.AllDirectories))
                    {
                        // remove path, and extension and replace \ with /
                        string sbName = path.Replace(modDataPath + patchPath + "\\", "").Replace("\\", "/").Replace(".sb", "");
                        foreach (DbObject entry in layout.GetValue<DbObject>("superBundles"))
                        {
                            if (entry.GetValue<string>("name").Equals(sbName, StringComparison.OrdinalIgnoreCase))
                            {
                                entry.RemoveValue("same");
                                entry.SetValue("delta", true);
                            }
                        }
                    }

                    using (DbWriter writer = new DbWriter(new FileStream(modDataPath + patchPath + "/layout.toc", FileMode.Create), true))
                        writer.Write(layout);
                }

                else if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.Madden20))
                {
                    DbObject layout = null;
                    using (DbReader reader = new DbReader(new FileStream(fs.ResolvePath("layout.toc"), FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
                        layout = reader.ReadDbObject();

                    // write out new manifest
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                    {
                        DbObject manifest = layout.GetValue<DbObject>("manifest");
                        ManifestFileRef fileRef = (ManifestFileRef)manifest.GetValue<int>("file");

                        byte[] tmpBuf = fs.WriteManifest();
                        string catalog = fs.GetCatalog(fileRef);

                        // find the next available cas
                        int casIndex = 1;
                        while (File.Exists(modDataPath + patchPath + "/" + (string.Format("{0}\\cas_{1}.cas", catalog, casIndex.ToString("D2")))))
                            casIndex++;

                        Sha1 sha1 = Utils.GenerateSha1(tmpBuf);

                        archiveData.TryAdd(sha1, new ArchiveInfo() { Data = tmpBuf });
                        WriteArchiveData(modDataPath + patchPath + "/" + catalog, new CasDataEntry("", sha1));

                        manifest.SetValue("size", tmpBuf.Length);
                        manifest.SetValue("offset", 0);
                        manifest.SetValue("sha1", sha1);
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5)) //more patch directory shenanigans
                            manifest.SetValue("file", (int)new ManifestFileRef(fileRef.CatalogIndex, false, casIndex));
                        else
                            manifest.SetValue("file", (int)new ManifestFileRef(fileRef.CatalogIndex, true, casIndex));
                    }

                    // add any new superbundles
                    if (addedSuperBundles.Count > 0)
                    {
                        foreach (string superBundle in addedSuperBundles)
                        {
                            DbObject sbobj = new DbObject();
                            sbobj.SetValue("name", superBundle);
                            layout.GetValue<DbObject>("superBundles").Add(sbobj);

                            DbObject chunk = (DbObject)layout.GetValue<DbObject>("installManifest").GetValue<DbObject>("installChunks")[1];
                            chunk.GetValue<DbObject>("superbundles").Add(superBundle);
                        }
                    }

                    string layoutLocation = modDataPath + patchPath + "/layout.toc";
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                        layoutLocation = modDataPath + "Data/layout.toc";

                    using (DbWriter writer = new DbWriter(new FileStream(layoutLocation, FileMode.Create), true))
                        writer.Write(layout);
                }

                // fifa17, dai, bf4, nfs, nfsr, gw2
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                    ProfileVersion.DragonAgeInquisition,
                    ProfileVersion.Battlefield4,
                    ProfileVersion.NeedForSpeed,
                    ProfileVersion.NeedForSpeedRivals,
                    ProfileVersion.PlantsVsZombiesGardenWarfare2))
                {
                    // copy additional files
                    CopyFileIfRequired(fs.BasePath + patchPath + "/../package.mft", modDataPath + patchPath + "/../package.mft");
                }

                // swbf2, bfv, sws
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    // copy from old data to new data
                    CopyFileIfRequired(fs.BasePath + "Data/chunkmanifest", modDataPath + "Data/chunkmanifest");
                    if (modifiedFs.Count > 0)
                    {
                        fs.WriteInitFs(fs.BasePath + "Data/initfs_Win32", modDataPath + "Data/initfs_Win32", modifiedFs);
                    }
                    else
                    {
                        CopyFileIfRequired(fs.BasePath + "Data/initfs_Win32", modDataPath + "Data/initfs_Win32");
                    }
                }

                // create the frosty mod list file
                File.WriteAllText(Path.Combine(modDataPath, patchPath, "mods.json"), JsonConvert.SerializeObject(GenerateModInfoList(modPaths, rootPath), Formatting.Indented));
            }

            cancelToken.ThrowIfCancellationRequested();

            // DAI and NFS dont require bcrypt
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4,
                ProfileVersion.NeedForSpeed,
                ProfileVersion.NeedForSpeedRivals))
            {
                // delete old useless bcrypt
                if (File.Exists(fs.BasePath + "bcrypt.dll"))
                    File.Delete(fs.BasePath + "bcrypt.dll");

                // copy over new CryptBase
                CopyFileIfRequired("ThirdParty/CryptBase.dll", fs.BasePath + "CryptBase.dll");
            }
            CopyFileIfRequired(fs.BasePath + "user.cfg", modDataPath + "user.cfg");

            // FIFA games require a fifaconfig workaround
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                ProfileVersion.Fifa18,
                ProfileVersion.Fifa19,
                ProfileVersion.Fifa20))
            {
                FileInfo fi = new FileInfo(fs.BasePath + "FIFASetup\\fifaconfig_orig.exe");
                if (!fi.Exists)
                {
                    fi = new FileInfo(fs.BasePath + "FIFASetup\\fifaconfig.exe");
                    fi.MoveTo(fi.FullName.Replace(".exe", "_orig.exe"));
                }

                CopyFileIfRequired("thirdparty/fifaconfig.exe", fs.BasePath + "FIFASetup\\fifaconfig.exe");
            }

            // launch the game (redirecting to the modPath directory)
            Logger.Log("Launching Game");

            try
            {
                ExecuteProcess($"{fs.BasePath + ProfilesLibrary.ProfileName}.exe", $"-dataPath \"{modDataPath.Trim('\\')}\" {additionalArgs}");
            }
            catch (Exception ex)
            {
                App.Logger.Log("Error Launching Game: " + ex);
            }

            App.Logger.Log("Done");

            GC.Collect();
            return 0;
        }

        private List<ModInfo> GenerateModInfoList(string[] modPaths, string rootPath)
        {
            List<ModInfo> modInfoList = new List<ModInfo>();

            foreach (string path in modPaths)
            {
                FileInfo fi = new FileInfo(Path.Combine(rootPath, path));
                FrostyMod fmod = new FrostyMod(fi.FullName);
                ModInfo modInfo;

                if (fmod.NewFormat)
                {
                    modInfo = new ModInfo
                    {
                        Name = fmod.ModDetails.Title,
                        Version = fmod.ModDetails.Version,
                        Category = fmod.ModDetails.Category,
                        Link = fmod.ModDetails.Link,
                        FileName = path
                    };
                }
                else
                {
                    FrostyModCollection fcollection = new FrostyModCollection(fi.FullName);
                    if (fcollection.IsValid)
                    {
                        modInfo = new ModInfo
                        {
                            Name = fcollection.ModDetails.Title,
                            Version = fcollection.ModDetails.Version,
                            Category = fcollection.ModDetails.Category,
                            Link = fcollection.ModDetails.Link,
                            FileName = path
                        };
                    }
                    else
                    {
                        DbObject mod = null;
                        using (DbReader reader = new DbReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read), null))
                            mod = reader.ReadDbObject();

                        modInfo = new ModInfo
                        {
                            Name = mod.GetValue<string>("title"),
                            Version = mod.GetValue<string>("version"),
                            Category = mod.GetValue<string>("category"),
                            FileName = path
                        };
                    }

                }
                modInfoList.Add(modInfo);
            }
            return modInfoList;
        }

        private void WriteArchiveData(string catalog, CasDataEntry casDataEntry)
        {
            List<int> casEntries = new List<int>();
            int casIndex = 1;
            int totalSize = 0;

            // find the next available cas
            while (File.Exists(string.Format("{0}\\cas_{1}.cas", catalog, casIndex.ToString("D2"))))
                casIndex++;

            // write data to cas
            Stream currentCasStream = null;
            foreach (Sha1 sha1 in casDataEntry.EnumerateDataRefs())
            {
                ArchiveInfo info = archiveData[sha1];

                int casMaxBytes = 536870912;
                switch (Config.Get("MaxCasFileSize", "512MB"))
                {
                    case "1GB": casMaxBytes = 1073741824; break;
                    case "512MB": casMaxBytes = 536870912; break;
                    case "256MB": casMaxBytes = 268435456; break;
                }

                // if cas exceeds max size, create a new one (incrementing index)
                if (currentCasStream == null || ((totalSize + info.Data.Length) > casMaxBytes))
                {
                    if (currentCasStream != null)
                    {
                        currentCasStream.Dispose();
                        casIndex++;
                    }

                    FileInfo casFi = new FileInfo(string.Format("{0}\\cas_{1}.cas", catalog, casIndex.ToString("D2")));
                    Directory.CreateDirectory(casFi.DirectoryName);

                    currentCasStream = new FileStream(casFi.FullName, FileMode.Create, FileAccess.Write);
                    totalSize = 0;
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4, ProfileVersion.NeedForSpeed, ProfileVersion.NeedForSpeedRivals))
                {
                    byte[] tmpBuf = new byte[0x20];
                    using (NativeWriter tmpWriter = new NativeWriter(new MemoryStream(tmpBuf)))
                    {
                        tmpWriter.Write(0xF00FCEFA);
                        tmpWriter.Write(sha1);
                        tmpWriter.Write((long)info.Data.Length);
                    }
                    currentCasStream.Write(tmpBuf, 0, 0x20);
                    totalSize += 0x20;
                }

                foreach (CasFileEntry casEntry in casDataEntry.EnumerateFileInfos(sha1))
                {
                    if (casEntry.Entry != null && casEntry.Entry.RangeStart != 0 && !casEntry.FileInfo.isChunk)
                    {
                        casEntry.FileInfo.offset = (uint)(currentCasStream.Position + casEntry.Entry.RangeStart);
                        casEntry.FileInfo.size = (casEntry.Entry.RangeEnd - casEntry.Entry.RangeStart);
                    }
                    else
                    {
                        casEntry.FileInfo.offset = (uint)currentCasStream.Position;
                        casEntry.FileInfo.size = info.Data.Length;
                    }
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
                    {
                        casEntry.FileInfo.file = new ManifestFileRef(casEntry.FileInfo.file.CatalogIndex, false, casIndex);
                    }
                    else
                    {
                        casEntry.FileInfo.file = new ManifestFileRef(casEntry.FileInfo.file.CatalogIndex, true, casIndex);
                    }
                }

                currentCasStream.Write(info.Data, 0, info.Data.Length);

                casEntries.Add(casIndex);
                totalSize += info.Data.Length;
            }
            currentCasStream.Dispose();

            FileInfo fi = new FileInfo(string.Format("{0}\\cas.cat", catalog));
            List<CatResourceEntry> entries = new List<CatResourceEntry>();
            List<CatPatchEntry> patchEntries = new List<CatPatchEntry>();
            List<CatResourceEntry> encEntries = new List<CatResourceEntry>();
            byte[] header = null;

            // read in original cat
            using (CatReader reader = new CatReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
            {
                for (int i = 0; i < reader.ResourceCount; i++)
                    entries.Add(reader.ReadResourceEntry());

                if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda,
                    ProfileVersion.Fifa17,
                    ProfileVersion.Fifa18,
                    ProfileVersion.NeedForSpeedPayback,
                    ProfileVersion.Madden19))
                {
                    for (int i = 0; i < reader.EncryptedCount; i++)
                        encEntries.Add(reader.ReadEncryptedEntry());
                }

                for (int i = 0; i < reader.PatchCount; i++)
                    patchEntries.Add(reader.ReadPatchEntry());

                reader.Position = 0;
                header = reader.ReadBytes(0x22C);
            }

            // write out new modified cat
            using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
            {
                int numEntries = 0;
                int numPatchEntries = 0;

                MemoryStream ms = new MemoryStream();
                using (NativeWriter tmpWriter = new NativeWriter(ms))
                {
                    // unmodified entries
                    foreach (CatResourceEntry entry in entries)
                    {
                        //if (casDataEntry.Contains(entry.Sha1))
                        //    continue;

                        tmpWriter.Write(entry.Sha1);
                        tmpWriter.Write(entry.Offset);
                        tmpWriter.Write(entry.Size);
                        if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                            ProfileVersion.Battlefield4,
                            ProfileVersion.NeedForSpeed,
                            ProfileVersion.NeedForSpeedRivals))
                        {
                            tmpWriter.Write(entry.LogicalOffset);
                        }
                        tmpWriter.Write(entry.ArchiveIndex);
                        numEntries++;
                    }

                    int offset = 0;
                    int index = 0;

                    // new entries
                    foreach (Sha1 sha1 in casDataEntry.EnumerateDataRefs())
                    {
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                            ProfileVersion.Battlefield4,
                            ProfileVersion.NeedForSpeed,
                            ProfileVersion.NeedForSpeedRivals))
                        {
                            offset += 0x20;
                        }

                        ArchiveInfo info = archiveData[sha1];

                        tmpWriter.Write(sha1);
                        tmpWriter.Write(offset);
                        tmpWriter.Write(info.Data.Length);
                        if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                            ProfileVersion.Battlefield4,
                            ProfileVersion.NeedForSpeed,
                            ProfileVersion.NeedForSpeedRivals))
                        {
                            tmpWriter.Write(0x00);
                        }
                        tmpWriter.Write(casEntries[index++]);

                        offset += info.Data.Length;
                        numEntries++;
                    }

                    if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda,
                        ProfileVersion.Fifa17,
                        ProfileVersion.Fifa18,
                        ProfileVersion.NeedForSpeedPayback,
                        ProfileVersion.Madden19))
                    {
                        // encrypted entries
                        foreach (CatResourceEntry entry in encEntries)
                        {
                            tmpWriter.Write(entry.Sha1);
                            tmpWriter.Write(entry.Offset);
                            tmpWriter.Write(entry.EncryptedSize);
                            tmpWriter.Write(entry.LogicalOffset);
                            tmpWriter.Write(entry.ArchiveIndex);
                            tmpWriter.Write(entry.Unknown);
                            tmpWriter.WriteFixedSizedString(entry.KeyId, entry.KeyId.Length);
                            tmpWriter.Write(entry.UnknownData);
                        }
                    }

                    // unmodified patch entries
                    foreach (CatPatchEntry entry in patchEntries)
                    {
                        //if (casDataEntry.Contains(entry.Sha1))
                        //    continue;

                        tmpWriter.Write(entry.Sha1);
                        tmpWriter.Write(entry.BaseSha1);
                        tmpWriter.Write(entry.DeltaSha1);
                        numPatchEntries++;
                    }

                    // write it to file
                    if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                        ProfileVersion.Battlefield4,
                        ProfileVersion.NeedForSpeed,
                        ProfileVersion.NeedForSpeedRivals))
                    {
                        writer.Write(header);
                    }
                    writer.WriteFixedSizedString("NyanNyanNyanNyan", 16);
                    if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                        ProfileVersion.Battlefield4,
                        ProfileVersion.NeedForSpeed,
                        ProfileVersion.NeedForSpeedRivals))
                    {
                        writer.Write(numEntries);
                        writer.Write(numPatchEntries);
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda,
                            ProfileVersion.Fifa17,
                            ProfileVersion.Fifa18,
                            ProfileVersion.StarWarsBattlefrontII,
                            ProfileVersion.NeedForSpeedPayback,
                            ProfileVersion.Madden19,
                            ProfileVersion.Battlefield5,
                            ProfileVersion.StarWarsSquadrons))
                        {
                            writer.Write(encEntries.Count);
                            writer.Write(0x00);
                            writer.Write(-1);
                            writer.Write(-1);
                        }
                    }
                    writer.Write(ms.ToArray());
                }
            }
        }

        private bool DeleteSelectFiles(string modPath)
        {
            DirectoryInfo di = new DirectoryInfo(modPath);
            if (di.Exists)
            {
                RecursiveDeleteFiles(modPath);
                foreach (string catalog in fs.Catalogs)
                {
                    string basePatchCatalog = fs.ResolvePath("native_patch/" + catalog);
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5)) //woo patch folder
                        basePatchCatalog = fs.ResolvePath("native_data/" + catalog);
                    string modDataCatalog = $"{modPath}/{catalog}";

                    if (Directory.Exists(modDataCatalog))
                    {
                        foreach (string filename in Directory.EnumerateFiles(modDataCatalog))
                        {
                            FileInfo fi = new FileInfo(filename);

                            // delete if cas does not exist in base patch OR is not a symbolic link
                            if (!File.Exists(basePatchCatalog + "/" + fi.Name) || (fi.Attributes & FileAttributes.ReparsePoint) == 0)
                            {
                                File.Delete(fi.FullName);
                            }
                        }
                    }
                }

                return true;
            }
            return false;
        }

        private bool IsSamePatch(string modPath)
        {
            string baseLayoutPath = fs.ResolvePath("native_patch/layout.toc");
            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
            {
                baseLayoutPath = fs.ResolvePath("native_data/layout.toc");
            }

            string modLayoutPath = modPath + "/layout.toc";
            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
            {
                modLayoutPath = Directory.GetParent(modPath).FullName + "/Data/layout.toc";
            }

            if (!File.Exists(baseLayoutPath))
                return false;
            if (!File.Exists(modLayoutPath))
                return false;

            DbObject patchLayout = null;
            using (DbReader reader = new DbReader(new FileStream(baseLayoutPath, FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
                patchLayout = reader.ReadDbObject();

            DbObject modLayout = null;
            using (DbReader reader = new DbReader(new FileStream(modLayoutPath, FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
                modLayout = reader.ReadDbObject();

            int patchHead = patchLayout.GetValue<int>("head");
            int modHead = modLayout.GetValue<int>("head");

            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
            {
                if (patchHead == 0xBDFB3 && modHead != patchHead)
                {
                    // SWBF2 new layout requires completely rebuilding ModData from scratch
                    Directory.Delete(modPath + "../", true);
                    return false;
                }
            }

            if (modHead != patchHead)
            {
                Directory.Delete(modPath + "../", true);
                return false;
            }

            return true;
        }

        private void RecursiveDeleteFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            DirectoryInfo[] paths = di.GetDirectories();
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo fi in files)
            {
                // delete all cat/toc/sb files and the initfs_win32 and mods file
                if (fi.Extension == ".cat" || fi.Extension == ".toc" || fi.Extension == ".sb" || fi.Name.ToLower() == "mods.txt" || fi.Name.ToLower() == "mods.json")
                {
                    // dont delete layout.toc
                    if (fi.Name.ToLower() == "layout.toc")
                        continue;

                    fi.Delete();
                }
            }

            foreach (DirectoryInfo subDi in paths)
            {
                string tempPath = Path.Combine(path, subDi.Name);
                RecursiveDeleteFiles(tempPath);
            }
        }

        private bool RunSymbolicLinkProcess(List<SymLinkStruct> cmdArgs)
        {
            using (TextWriter writer = new StreamWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\run.bat", FileMode.Create)))
            {
                foreach (SymLinkStruct arg in cmdArgs)
                    writer.WriteLine("mklink" + ((arg.isFolder) ? "/D " : " ") + "\"" + arg.dest + "\" \"" + arg.src + "\"");
            }

            // create data and update symbolic links
            ExecuteProcess("cmd.exe", "/C \"" + AppDomain.CurrentDomain.BaseDirectory + "\\run.bat\"", true, true);

            // delete batch
            File.Delete("run.bat");

            // validate
            foreach (SymLinkStruct arg in cmdArgs)
            {
                if ((arg.isFolder && !Directory.Exists(arg.dest)) || (!arg.isFolder && !File.Exists(arg.dest)))
                    return false;
            }

            return true;
        }

        public static void ExecuteProcess(string processName, string args, bool waitForExit = false, bool asAdmin = false, Dictionary<string, string> env = null)
        {
            using (Process process = new Process())
            {
                FileInfo fi = new FileInfo(processName);

                process.StartInfo.FileName = processName;
                process.StartInfo.WorkingDirectory = fi.DirectoryName;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;

                if (env != null)
                {
                    foreach (string key in env.Keys)
                    {
                        process.StartInfo.EnvironmentVariables[key] = env[key];
                    }
                }

                if (asAdmin)
                {
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Verb = "runas";
                }

                process.Start();

                if (waitForExit)
                    process.WaitForExit();
            }
        }

        private byte[] GetResourceData(string modFilename, int archiveIndex, long offset, int size)
        {
            string archiveFilename = modFilename.Replace(".fbmod", "_" + archiveIndex.ToString("D2") + ".archive");
            if (!File.Exists(archiveFilename))
                return null;

            using (NativeReader reader = new NativeReader(new FileStream(archiveFilename, FileMode.Open, FileAccess.Read)))
            {
                reader.Position = offset;
                return reader.ReadBytes(size);
            }
        }

        private void CopyFileIfRequired(string source, string dest)
        {
            FileInfo baseFi = new FileInfo(source);
            FileInfo modFi = new FileInfo(dest);
            if (baseFi.Exists)
            {
                // copy file if it doesn't exist, or recently modified
                if (!modFi.Exists || (modFi.Exists && baseFi.LastWriteTimeUtc > modFi.LastWriteTimeUtc || baseFi.Length != modFi.Length))
                    File.Copy(baseFi.FullName, modFi.FullName, true);
            }
        }
    }
}
