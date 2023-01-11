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

        private FileSystemManager m_fs;
        private ResourceManager m_rm;
        private AssetManager m_am;
        private ILogger m_logger;

        private List<string> m_addedSuperBundles = new List<string>();

        private ConcurrentDictionary<int, ModBundleInfo> m_modifiedSuperBundles = new ConcurrentDictionary<int, ModBundleInfo>();
        private ConcurrentDictionary<int, ModBundleInfo> m_modifiedBundles = new ConcurrentDictionary<int, ModBundleInfo>();
        private ConcurrentDictionary<int, List<string>> m_addedBundles = new ConcurrentDictionary<int, List<string>>();

        private ConcurrentDictionary<string, EbxAssetEntry> m_modifiedEbx = new ConcurrentDictionary<string, EbxAssetEntry>();
        private ConcurrentDictionary<string, ResAssetEntry> m_modifiedRes = new ConcurrentDictionary<string, ResAssetEntry>();
        private ConcurrentDictionary<Guid, ChunkAssetEntry> m_modifiedChunks = new ConcurrentDictionary<Guid, ChunkAssetEntry>();
        private ConcurrentDictionary<string, DbObject> m_modifiedFs = new ConcurrentDictionary<string, DbObject>();

        private ConcurrentDictionary<Sha1, ArchiveInfo> m_archiveData = new ConcurrentDictionary<Sha1, ArchiveInfo>();
        private int m_numArchiveEntries = 0;
        private int m_numTasks;

        private CasDataInfo m_casData = new CasDataInfo();
        private static int s_chunksBundleHash = Fnv1.HashString("chunks"); // should not be used, only here to ignore old mods adding assets to it
        private Dictionary<int, Dictionary<int, Dictionary<uint, CatResourceEntry>>> m_resources = new Dictionary<int, Dictionary<int, Dictionary<uint, CatResourceEntry>>>();
        private string m_modDirName = "ModData";
        private string m_patchPath = "Patch";
        private bool m_hasPatchFolder = true;

        public ILogger Logger { get => m_logger; set => m_logger = value; }

        private void PrintLog()
        {
            using (NativeWriter writer = new NativeWriter(new FileStream("log.txt", FileMode.Create, FileAccess.Write)))
            {
                if (m_addedSuperBundles.Count > 0)
                {
                    writer.WriteLine("Added SuperBundles:");
                    foreach (string sb in m_addedSuperBundles)
                    {
                        writer.WriteLine($"  - {sb}");
                    }
                    writer.WriteLine(string.Empty);
                }

                if (m_modifiedSuperBundles.Count > 0)
                {
                    writer.WriteLine("Modified SuperBundles:");
                    foreach (var kv in m_modifiedSuperBundles)
                    {
                        writer.WriteLine($"  {m_am.GetSuperBundle(kv.Key).Name}:");

                        List<Guid> sortedChunks = new List<Guid>(kv.Value.Modify.Chunks);
                        sortedChunks.Sort();

                        if (kv.Value.Modify.Chunks.Count > 0)
                        {
                            writer.WriteLine($"    Modified Chunks:");
                            foreach (Guid chunkId in sortedChunks)
                            {
                                writer.WriteLine($"    - {chunkId}");
                            }
                        }

                        sortedChunks = new List<Guid>(kv.Value.Add.Chunks);
                        sortedChunks.Sort();

                        if (kv.Value.Add.Chunks.Count > 0)
                        {
                            writer.WriteLine($"    Added Chunks:");
                            foreach (Guid chunkId in sortedChunks)
                            {
                                writer.WriteLine($"    - {chunkId}");
                            }
                        }
                    }
                    writer.WriteLine(string.Empty);
                }

                if (m_modifiedBundles.Count > 0)
                {
                    Dictionary<int, string> bundleHashMap = new Dictionary<int, string>();
                    foreach (BundleEntry be in m_am.EnumerateBundles())
                    {
                        if (!bundleHashMap.ContainsKey(HashBundle(be)))
                            bundleHashMap.Add(HashBundle(be), be.Name);
                    }
                    writer.WriteLine("Modified Bundles:");
                    
                    List<int> bundles = m_modifiedBundles.Keys.ToList();
                    bundles.Sort();

                    foreach (int hash in bundles)
                    {
                        var kv = m_modifiedBundles[hash];
                        writer.WriteLine($"  {bundleHashMap[hash]}:");

                        if (kv.Modify.Ebx.Count > 0)
                        {
                            writer.WriteLine($"    Modified Ebx:");
                            List<string> sorted = new List<string>(kv.Modify.Ebx);
                            sorted.Sort();
                            foreach (string name in sorted)
                            {
                                writer.WriteLine($"    - {name}");
                            }
                        }
                        if (kv.Add.Ebx.Count > 0)
                        {
                            writer.WriteLine($"    Added Ebx:");
                            List<string> sorted = new List<string>(kv.Add.Ebx);
                            sorted.Sort();
                            foreach (string name in sorted)
                            {
                                writer.WriteLine($"    - {name}");
                            }
                        }

                        if (kv.Modify.Res.Count > 0)
                        {
                            writer.WriteLine($"    Modified Res:");
                            List<string> sorted = new List<string>(kv.Modify.Res);
                            sorted.Sort();
                            foreach (string name in sorted)
                            {
                                writer.WriteLine($"    - {name}");
                            }
                        }
                        if (kv.Add.Res.Count > 0)
                        {
                            writer.WriteLine($"    Added Res:");
                            List<string> sorted = new List<string>(kv.Add.Res);
                            sorted.Sort();
                            foreach (string name in sorted)
                            {
                                writer.WriteLine($"    - {name}");
                            }
                        }

                        if (kv.Modify.Chunks.Count > 0)
                        {
                            writer.WriteLine($"    Modified Chunks:");
                            List<Guid> sorted = new List<Guid>(kv.Modify.Chunks);
                            sorted.Sort();
                            foreach (Guid chunkId in sorted)
                            {
                                writer.WriteLine($"    - {chunkId}");
                            }
                        }
                        if (kv.Add.Chunks.Count > 0)
                        {
                            writer.WriteLine($"    Added Chunks:");
                            List<Guid> sorted = new List<Guid>(kv.Add.Chunks);
                            sorted.Sort();
                            foreach (Guid chunkId in sorted)
                            {
                                writer.WriteLine($"    - {chunkId}");
                            }
                        }
                    }
                    writer.WriteLine(string.Empty);
                }
            }
        }

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
            Parallel.ForEach(fmod.Resources, resource =>
            {
                // pull existing bundles from asset manager
                List<int> bundles = new List<int>();

                // get superbundles which have modified chunks
                List<int> superBundles = new List<int>();

                // get superbundles which have added chunks
                List<int> addedSuperBundles = new List<int>();

                if (resource.Type == ModResourceType.Bundle)
                {
                    BundleEntry bEntry = new BundleEntry();
                    resource.FillAssetEntry(bEntry);

                    m_addedBundles.TryAdd(bEntry.SuperBundleId, new List<string>());
                    m_addedBundles[bEntry.SuperBundleId].Add(bEntry.Name);

                }
                else if (resource.Type == ModResourceType.Ebx)
                {
                    if (resource.IsModified || !m_modifiedEbx.ContainsKey(resource.Name))
                    {
                        if (resource.HasHandler)
                        {
                            EbxAssetEntry entry = null;
                            HandlerExtraData extraData = null;
                            byte[] data = fmod.GetResourceData(resource);

                            if (m_modifiedEbx.ContainsKey(resource.Name))
                            {
                                entry = m_modifiedEbx[resource.Name];
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
                                var ebxEntry = m_am.GetEbxEntry(resource.Name);
                                foreach (int bid in ebxEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(m_am.GetBundleEntry(bid)));
                                }

                                entry.ExtraData = extraData;
                                m_modifiedEbx.TryAdd(resource.Name, entry);
                            }

                            // merge new and old data together
                            if (extraData != null)
                                extraData.Data = extraData.Handler.Load(extraData.Data, data);
                        }
                        else
                        {
                            if (m_modifiedEbx.ContainsKey(resource.Name))
                            {
                                EbxAssetEntry existingEntry = m_modifiedEbx[resource.Name];

                                if (existingEntry.ExtraData != null)
                                    return;
                                if (existingEntry.Sha1 == resource.Sha1)
                                    return;

                                m_archiveData[existingEntry.Sha1].RefCount--;
                                if (m_archiveData[existingEntry.Sha1].RefCount == 0)
                                    m_archiveData.TryRemove(existingEntry.Sha1, out _);

                                m_modifiedEbx.TryRemove(resource.Name, out _);
                                m_numArchiveEntries--;
                            }

                            EbxAssetEntry entry = new EbxAssetEntry();
                            resource.FillAssetEntry(entry);

                            byte[] data = fmod.GetResourceData(resource);
                            var ebxEntry = m_am.GetEbxEntry(resource.Name);

                            if (data == null)
                            {
                                data = NativeReader.ReadInStream(m_am.GetRawStream(ebxEntry));

                                entry.Sha1 = ebxEntry.Sha1;
                                entry.OriginalSize = ebxEntry.OriginalSize;
                            }
                            else if (ebxEntry != null)
                            {
                                // add in existing bundles
                                foreach (int bid in ebxEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(m_am.GetBundleEntry(bid)));
                                }
                            }

                            entry.Size = data.Length;

                            m_modifiedEbx.TryAdd(entry.Name, entry);
                            if (!m_archiveData.ContainsKey(entry.Sha1))
                                m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                            else
                                m_archiveData[entry.Sha1].RefCount++;
                            m_numArchiveEntries++;
                        }
                    }
                }
                else if (resource.Type == ModResourceType.Res)
                {
                    if (resource.IsModified || !m_modifiedRes.ContainsKey(resource.Name))
                    {
                        if (resource.HasHandler)
                        {
                            ResAssetEntry entry = null;
                            HandlerExtraData extraData = null;
                            byte[] data = fmod.GetResourceData(resource);

                            if (m_modifiedRes.ContainsKey(resource.Name))
                            {
                                entry = m_modifiedRes[resource.Name];
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
                                var resEntry = m_am.GetResEntry(resource.Name);
                                foreach (int bid in resEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(m_am.GetBundleEntry(bid)));
                                }

                                entry.ExtraData = extraData;
                                m_modifiedRes.TryAdd(resource.Name, entry);
                            }

                            // merge new and old data together
                            if (extraData != null)
                                extraData.Data = extraData.Handler.Load(extraData.Data, data);
                        }
                        else
                        {
                            if (m_modifiedRes.ContainsKey(resource.Name))
                            {
                                ResAssetEntry existingEntry = m_modifiedRes[resource.Name];

                                if (existingEntry.ExtraData != null)
                                    return;
                                if (existingEntry.Sha1 == resource.Sha1)
                                    return;

                                m_archiveData[existingEntry.Sha1].RefCount--;
                                if (m_archiveData[existingEntry.Sha1].RefCount == 0)
                                    m_archiveData.TryRemove(existingEntry.Sha1, out _);

                                m_modifiedRes.TryRemove(resource.Name, out _);
                                m_numArchiveEntries--;
                            }

                            ResAssetEntry entry = new ResAssetEntry();
                            resource.FillAssetEntry(entry);

                            byte[] data = fmod.GetResourceData(resource);
                            var resEntry = m_am.GetResEntry(resource.Name);

                            if (data == null)
                            {
                                data = NativeReader.ReadInStream(m_am.GetRawStream(resEntry));

                                entry.Sha1 = resEntry.Sha1;
                                entry.OriginalSize = resEntry.OriginalSize;
                                entry.ResMeta = resEntry.ResMeta;
                                entry.ResRid = resEntry.ResRid;
                                entry.ResType = resEntry.ResType;
                            }
                            else if (resEntry != null)
                            {
                                // add in existing bundles
                                foreach (int bid in resEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(m_am.GetBundleEntry(bid)));
                                }
                            }

                            entry.Size = data.Length;

                            m_modifiedRes.TryAdd(entry.Name, entry);
                            if (!m_archiveData.ContainsKey(entry.Sha1))
                                m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                            else
                                m_archiveData[entry.Sha1].RefCount++;
                            m_numArchiveEntries++;
                        }
                    }
                }
                else if (resource.Type == ModResourceType.Chunk)
                {
                    Guid guid = new Guid(resource.Name);
                    if (resource.IsModified || !m_modifiedChunks.ContainsKey(guid))
                    {
                        if (resource.HasHandler)
                        {
                            ChunkAssetEntry entry = null;
                            HandlerExtraData extraData = null;
                            byte[] data = fmod.GetResourceData(resource);

                            if (m_modifiedChunks.ContainsKey(guid))
                            {
                                entry = m_modifiedChunks[guid];
                                extraData = (HandlerExtraData)entry.ExtraData;
                            }
                            else
                            {
                                entry = new ChunkAssetEntry();
                                extraData = new HandlerExtraData();

                                entry.Id = guid;
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
                                var chunkEntry = m_am.GetChunkEntry(guid);
                                foreach (int bid in chunkEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(m_am.GetBundleEntry(bid)));
                                }

                                // add in existing superbundles
                                superBundles.AddRange(chunkEntry.SuperBundles);

                                entry.ExtraData = extraData;
                                m_modifiedChunks.TryAdd(guid, entry);
                            }

                            // merge new and old data together
                            extraData.Data = extraData.Handler.Load(extraData.Data, data);
                        }
                        else
                        {
                            if (m_modifiedChunks.ContainsKey(guid))
                            {
                                ChunkAssetEntry existingEntry = m_modifiedChunks[guid];
                                if (existingEntry.Sha1 == resource.Sha1)
                                    return;

                                m_archiveData[existingEntry.Sha1].RefCount--;
                                if (m_archiveData[existingEntry.Sha1].RefCount == 0)
                                    m_archiveData.TryRemove(existingEntry.Sha1, out _);

                                m_modifiedChunks.TryRemove(guid, out _);
                                m_numArchiveEntries--;
                            }

                            ChunkAssetEntry entry = new ChunkAssetEntry();
                            resource.FillAssetEntry(entry);

                            byte[] data = fmod.GetResourceData(resource);
                            var chunkEntry = m_am.GetChunkEntry(guid);

                            if (data == null)
                            {
                                data = NativeReader.ReadInStream(m_am.GetRawStream(chunkEntry));

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
                                    if (m_fs.GetManifestChunk(chunkEntry.Id) != null)
                                    {
                                        entry.TocChunkSpecialHack = true;
                                        if (chunkEntry.Bundles.Count == 0)
                                        {
                                            resource.ClearAddedBundles();
                                        }
                                    }
                                }
                            }
                            else if (chunkEntry != null)
                            {
                                // add in existing bundles
                                foreach (int bid in chunkEntry.Bundles)
                                {
                                    bundles.Add(HashBundle(m_am.GetBundleEntry(bid)));
                                }

                                // add in existing superbundles
                                superBundles.AddRange(chunkEntry.SuperBundles);
                            }
                            else if (!resource.IsAdded)
                            {
                                // old mods had their IsAdded flag overridden so we have to manually set the flag and add it to chunks superbundles
                                resource.IsAdded = true;
                                foreach (var superBundle in m_am.EnumerateSuperBundles())
                                {
                                    // StandardModExecutor.cs had hack with "chunks" and FifaModExecutor.cs had hack with globals.toc 
                                    if (superBundle.Name.Contains("chunks") || superBundle.Name.Equals("Win32/globals", StringComparison.OrdinalIgnoreCase) || superBundle.Name.Equals("<none>"))
                                    {
                                        addedSuperBundles.Add(m_am.GetSuperBundleId(superBundle));
                                    }
                                }
                            }

                            // add in added superbundles
                            addedSuperBundles.AddRange(entry.AddedSuperBundles);

                            entry.Size = data.Length;

                            m_modifiedChunks.TryAdd(guid, entry);
                            if (!m_archiveData.ContainsKey(entry.Sha1))
                                m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                            else
                                m_archiveData[entry.Sha1].RefCount++;
                            m_numArchiveEntries++;
                        }
                    }
                    else
                    {
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5))
                        {
                            var chunkEntry = m_am.GetChunkEntry(guid);
                            var entry = m_modifiedChunks[guid];

                            if (m_fs.GetManifestChunk(chunkEntry.Id) != null)
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

                    if (!m_modifiedFs.ContainsKey(resource.Name))
                    {
                        m_modifiedFs.TryAdd(resource.Name, null);
                    }

                    m_modifiedFs[resource.Name] = fileStub;
                }

                // modified bundle actions (these are pulled from the asset manager during applying)
                foreach (int bundleHash in bundles)
                {
                    // Skips bundle if the whitelist has more than one element and doesn't contain the bundle hash
                    if (App.WhitelistedBundles.Count != 0 && !App.WhitelistedBundles.Contains(bundleHash))
                    {
                        continue;
                    }

                    m_modifiedBundles.TryAdd(bundleHash, new ModBundleInfo() { Name = bundleHash });

                    ModBundleInfo modBundle = m_modifiedBundles[bundleHash];
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
                    if ((App.WhitelistedBundles.Count != 0 && !App.WhitelistedBundles.Contains(bundleHash)) || bundleHash == s_chunksBundleHash)
                    {
                        continue;
                    }

                    m_modifiedBundles.TryAdd(bundleHash, new ModBundleInfo() { Name = bundleHash });

                    ModBundleInfo modBundle = m_modifiedBundles[bundleHash];
                    switch (resource.Type)
                    {
                        case ModResourceType.Ebx: modBundle.Add.AddEbx(resource.Name); break;
                        case ModResourceType.Res: modBundle.Add.AddRes(resource.Name); break;
                        case ModResourceType.Chunk: modBundle.Add.AddChunk(new Guid(resource.Name)); break;
                    }
                }

                // modified superbundle actions (these are pulled from the asset manager during applying)
                foreach (int superBundleId in superBundles)
                {
                    m_modifiedSuperBundles.TryAdd(superBundleId, new ModBundleInfo() { Name = superBundleId });

                    ModBundleInfo modBundle = m_modifiedSuperBundles[superBundleId];

                    modBundle.Modify.AddChunk(new Guid(resource.Name));
                }

                // add superbundle actions (these are stored in the mod)
                foreach (int superBundleId in addedSuperBundles)
                {
                    m_modifiedSuperBundles.TryAdd(superBundleId, new ModBundleInfo() { Name = superBundleId });

                    ModBundleInfo modBundle = m_modifiedSuperBundles[superBundleId];

                    modBundle.Add.AddChunk(new Guid(resource.Name));
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

                if (bundle == s_chunksBundleHash)
                {
                    continue;
                }

                if (!m_modifiedBundles.ContainsKey(bundle))
                    m_modifiedBundles.TryAdd(bundle, new ModBundleInfo() { Name = bundle });

                ModBundleInfo modBundle = m_modifiedBundles[bundle];
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
                    m_addedSuperBundles.Add(name);
                }
                else if (resourceType == "bundle")
                {
                    string name = resource.GetValue<string>("name");
                    string superBundle = resource.GetValue<string>("sb");

                    int hash = Fnv1a.HashString(superBundle.ToLower());
                    if (!m_addedBundles.ContainsKey(hash))
                        m_addedBundles.TryAdd(hash, new List<string>());

                    m_addedBundles[hash].Add(name);
                }
                else if (resourceType == "ebx")
                {
                    string name = resource.GetValue<string>("name");

                    if (m_modifiedEbx.ContainsKey(name))
                    {
                        EbxAssetEntry existingEntry = m_modifiedEbx[name];
                        if (existingEntry.Sha1 == resource.GetValue<Sha1>("sha1"))
                            continue;

                        m_archiveData[existingEntry.Sha1].RefCount--;
                        if (m_archiveData[existingEntry.Sha1].RefCount == 0)
                            m_archiveData.TryRemove(existingEntry.Sha1, out _);

                        m_modifiedEbx.TryRemove(name, out _);
                        m_numArchiveEntries--;
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

                        using (NativeReader reader = new NativeReader(new FileStream(m_fs.ResolvePath(fileRef), FileMode.Open, FileAccess.Read)))
                        {
                            reader.Position = offset;
                            buffer = reader.ReadBytes((int)entry.Size);
                        }
                    }

                    entry.Sha1 = Utils.GenerateSha1(buffer);

                    m_modifiedEbx.TryAdd(entry.Name, entry);
                    if (!m_archiveData.ContainsKey(entry.Sha1))
                        m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = buffer, RefCount = 1 });
                    else
                        m_archiveData[entry.Sha1].RefCount++;
                    m_numArchiveEntries++;
                }
                else if (resourceType == "res")
                {
                    string name = resource.GetValue<string>("name");

                    if (m_modifiedRes.ContainsKey(name))
                    {
                        ResAssetEntry existingEntry = m_modifiedRes[name];
                        if (existingEntry.Sha1 == resource.GetValue<Sha1>("sha1"))
                            continue;

                        m_archiveData[existingEntry.Sha1].RefCount--;
                        if (m_archiveData[existingEntry.Sha1].RefCount == 0)
                            m_archiveData.TryRemove(existingEntry.Sha1, out _);

                        m_modifiedRes.TryRemove(name, out _);
                        m_numArchiveEntries--;
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

                        using (NativeReader reader = new NativeReader(new FileStream(m_fs.ResolvePath(fileRef), FileMode.Open, FileAccess.Read)))
                        {
                            reader.Position = offset;
                            buffer = reader.ReadBytes((int)entry.Size);
                        }
                    }

                    entry.Sha1 = Utils.GenerateSha1(buffer);

                    m_modifiedRes.TryAdd(entry.Name, entry);
                    if (!m_archiveData.ContainsKey(entry.Sha1))
                        m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = buffer, RefCount = 1 });
                    else
                        m_archiveData[entry.Sha1].RefCount++;
                    m_numArchiveEntries++;
                }
                else if (resourceType == "chunk")
                {
                    Guid chunkId = new Guid(resource.GetValue<string>("name"));
                    if (m_modifiedChunks.ContainsKey(chunkId))
                    {
                        ChunkAssetEntry existingEntry = m_modifiedChunks[chunkId];
                        if (existingEntry.Sha1 == resource.GetValue<Sha1>("sha1"))
                            continue;

                        m_archiveData[existingEntry.Sha1].RefCount--;
                        if (m_archiveData[existingEntry.Sha1].RefCount == 0)
                            m_archiveData.TryRemove(existingEntry.Sha1, out _);

                        m_modifiedChunks.TryRemove(chunkId, out _);
                        m_numArchiveEntries--;
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
                        H32 = resource.GetValue<int>("h32", 0)
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
                        using (NativeReader reader = new NativeReader(new FileStream(m_fs.ResolvePath(fileRef), FileMode.Open, FileAccess.Read)))
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

                    m_modifiedChunks.TryAdd(entry.Id, entry);
                    if (!m_archiveData.ContainsKey(entry.Sha1))
                        m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = buffer, RefCount = 1 });
                    else
                        m_archiveData[entry.Sha1].RefCount++;
                    m_numArchiveEntries++;

                    ChunkAssetEntry originalEntry = m_am.GetChunkEntry(entry.Id);
                    if (originalEntry != null)
                    {
                        // pull superbundles from am
                        foreach (int sbId in originalEntry.SuperBundles)
                        {
                            m_modifiedSuperBundles.TryAdd(sbId, new ModBundleInfo() { Name = sbId });

                            ModBundleInfo chunksBundle = m_modifiedSuperBundles[sbId];
                            chunksBundle.Modify.Chunks.Add(entry.Id);
                        }
                    }
                    else
                    {
                        // need to add it to some superbundle
                        foreach (var superBundle in m_am.EnumerateSuperBundles())
                        {
                            // StandardModExecutor.cs had hack with "chunks" and FifaModExecutor.cs had hack with globals.toc 
                            if (superBundle.Name.Contains("chunks") || superBundle.Name.Equals("Win32/globals", StringComparison.OrdinalIgnoreCase) || superBundle.Name.Equals("<none>"))
                            {
                                int sbId = m_am.GetSuperBundleId(superBundle);
                                m_modifiedSuperBundles.TryAdd(sbId, new ModBundleInfo() { Name = sbId });

                                ModBundleInfo chunksBundle = m_modifiedSuperBundles[sbId];
                                chunksBundle.Add.Chunks.Add(entry.Id);
                            }
                        }
                    }

                    if (ver < 2)
                    {
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
            m_modDirName = "ModData\\" + modPackName;
            cancelToken.ThrowIfCancellationRequested();

            App.Logger.Log("Launching");

            m_fs = inFs;
            Logger = inLogger;

            string modDataPath = m_fs.BasePath + m_modDirName + "\\";

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4,
                ProfileVersion.NeedForSpeed,
                ProfileVersion.PlantsVsZombiesGardenWarfare2,
                ProfileVersion.NeedForSpeedRivals))
            {
                // old fb3 games use an update folder
                m_patchPath = "Update\\Patch\\Data";
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5, ProfileVersion.NeedForSpeedUnbound))
            {
                // bfv doesnt have a patch directory
                m_patchPath = "Data";
                m_hasPatchFolder = false;
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
            if (!File.Exists(Path.Combine(modDataPath, m_patchPath, "mods.json")))
            {
                needsModding = true;
            }
            else
            {
                List<ModInfo> oldModInfoList = JsonConvert.DeserializeObject<List<ModInfo>>(File.ReadAllText(Path.Combine(modDataPath, m_patchPath, "mods.json")));
                List<ModInfo> currentModInfoList = GenerateModInfoList(modPaths, rootPath);

                // check if the mod data needs recreating
                // ie. mod change or patch
                if (!IsSamePatch(modDataPath + m_patchPath) || !oldModInfoList.SequenceEqual(currentModInfoList))
                {
                    needsModding = true;
                }
            }

            cancelToken.ThrowIfCancellationRequested();
            if (needsModding)
            {
                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Initializing Resources");

                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    foreach (string catalogName in m_fs.Catalogs)
                    {
                        Dictionary<int, Dictionary<uint, CatResourceEntry>> entries = LoadCatalog(m_fs, "native_data/" + catalogName + "/cas.cat", out int hash);
                        if (entries != null)
                            m_resources.Add(hash, entries);

                        entries = LoadCatalog(m_fs, "native_patch/" + catalogName + "/cas.cat", out hash);
                        if (entries != null)
                            m_resources.Add(hash, entries);
                    }
                }

                m_rm = new ResourceManager(m_fs);
                m_rm.SetLogger(m_logger);
                m_rm.Initialize();

                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Loading " + ProfilesLibrary.CacheName + ".cache");

                m_am = new AssetManager(m_fs, m_rm);
                m_am.SetLogger(m_logger);
                m_am.Initialize(additionalStartup: false);

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
                    Logger.Log($"Loading Mods ({mod.ModDetails?.Title ?? mod.Filename.Replace(".fbmod", "")})");
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
                assetEntries.AddRange(m_modifiedEbx.Values);
                assetEntries.AddRange(m_modifiedRes.Values);
                assetEntries.AddRange(m_modifiedChunks.Values);

                int currentResource = 0;
                Parallel.ForEach(assetEntries, entry =>
                {
                    if (entry.ExtraData is HandlerExtraData handlerExtaData)
                    {
                        handlerExtaData.Handler.Modify(entry, m_am, runtimeResources, handlerExtaData.Data, out byte[] data);

                        if (!m_archiveData.TryAdd(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 }))
                            m_archiveData[entry.Sha1].RefCount++;
                    }
                    ReportProgress(currentResource++, assetEntries.Count);
                    Logger.Log($"Applying Handlers ({currentResource}/{assetEntries.Count})");
                });

                // process any new resources added during custom handler modification
                ProcessModResources(runtimeResources);
#if FROSTY_DEVELOPER
                PrintLog();
#endif

                cancelToken.ThrowIfCancellationRequested();
                Logger.Log("Cleaning Up ModData");
                App.Logger.Log("Cleaning Up ModData");

                List<SymLinkStruct> cmdArgs = new List<SymLinkStruct>();
                bool newInstallation = false;

                m_fs.ResetManifest();
                if (!DeleteSelectFiles(modDataPath + m_patchPath))
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
                            cmdArgs.Add(new SymLinkStruct(modDataPath + "Data/Win32", m_fs.BasePath + "Data/Win32", true));
                        }
                        else if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5, ProfileVersion.NeedForSpeedUnbound))
                        {
                            // bfv and unbound dont have a patch directory so we need to rebuild the data folder structure instead
                            if (!Directory.Exists(modDataPath + "Data"))
                                Directory.CreateDirectory(modDataPath + "Data");

                            foreach (string casFilename in Directory.EnumerateFiles(m_fs.BasePath + m_patchPath, "*.cas", SearchOption.AllDirectories))
                            {
                                FileInfo casFi = new FileInfo(casFilename);
                                string destPath = casFi.Directory.FullName.ToLower().Replace("\\" + m_patchPath.ToLower(), "\\" + m_modDirName.ToLower() + "\\" + m_patchPath.ToLower());
                                string tempPath = Path.Combine(destPath, casFi.Name);

                                if (!Directory.Exists(destPath))
                                    Directory.CreateDirectory(destPath);

                                cmdArgs.Add(new SymLinkStruct(tempPath, casFi.FullName, false));
                            }
                        }
                        else
                        {
                            // data path
                            cmdArgs.Add(new SymLinkStruct(modDataPath + "Data", m_fs.BasePath + "Data", true));
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
                            foreach (string path in Directory.EnumerateDirectories(m_fs.BasePath + "Update"))
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
                            cmdArgs.Add(new SymLinkStruct(modDataPath + "Update", m_fs.BasePath + "Update", true));
                        }

                        if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Anthem,
                            ProfileVersion.Madden20, ProfileVersion.Fifa20,
                            ProfileVersion.NeedForSpeedHeat, ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                            ProfileVersion.Fifa21, ProfileVersion.Madden22,
                            ProfileVersion.Fifa22, ProfileVersion.Madden23))
                        {
                            foreach (string casFilename in Directory.EnumerateFiles(m_fs.BasePath + m_patchPath, "*.cas", SearchOption.AllDirectories))
                            {
                                FileInfo casFi = new FileInfo(casFilename);
                                string destPath = casFi.Directory.FullName.ToLower().Replace("\\" + m_patchPath.ToLower(), "\\" + m_modDirName.ToLower() + "\\" + m_patchPath.ToLower());
                                string tempPath = Path.Combine(destPath, casFi.Name);

                                if (!Directory.Exists(destPath))
                                    Directory.CreateDirectory(destPath);

                                cmdArgs.Add(new SymLinkStruct(tempPath, casFi.FullName, false));
                            }
                        }
                    }
                }

                // add cas files to link
                foreach (string catalog in m_fs.Catalogs)
                {
                    string path = m_fs.ResolvePath("native_patch/" + catalog + "/cas.cat");
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5)) //again, no patch directory. fun.
                    {
                        path = m_fs.ResolvePath("native_data/" + catalog + "/cas.cat");
                    }
                    if (!File.Exists(path))
                        continue;

                    FileInfo catInfo = new FileInfo(path);
                    string destPath = catInfo.Directory.FullName.Replace("\\" + m_patchPath.ToLower(), "\\" + m_modDirName.ToLower() + "\\" + m_patchPath.ToLower());

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
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.NeedForSpeedHeat,
                    ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.Fifa21,
                    ProfileVersion.Madden22, ProfileVersion.Fifa22,
                    ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound))
                {
                    CasBundleAction.CasFiles.Clear();
                    foreach (string catalog in m_fs.Catalogs)
                    {
                        int casIndex = 1;
                        string path = m_fs.BasePath + m_patchPath + "\\" + catalog + "\\cas_" + casIndex.ToString("D2") + ".cas";

                        while (File.Exists(path))
                        {
                            casIndex++;
                            path = m_fs.BasePath + m_patchPath + "\\" + catalog + "\\cas_" + casIndex.ToString("D2") + ".cas";
                        }

                        CasBundleAction.CasFiles.Add(catalog, casIndex);
                    }

                    List<CasBundleAction> actions = new List<CasBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    // @todo: Added bundles

                    int totalTasks = 0;
                    foreach (SuperBundleInfo superBundle in m_fs.EnumerateSuperBundleInfos())
                    {
                        if (m_fs.ResolvePath(superBundle.Name + ".toc") == "")
                            continue;

                        CasBundleAction action = new CasBundleAction(superBundle, doneEvent, this);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        m_numTasks++;
                        totalTasks++;
                    }

                    while (m_numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        ReportProgress(totalTasks - m_numTasks, totalTasks);
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

                        string srcPath = m_fs.ResolvePath($"native_patch/{completedAction.SuperBundleInfo.Name}.toc");
                        if (!m_hasPatchFolder)
                        {
                            srcPath = m_fs.ResolvePath($"native_data/{completedAction.SuperBundleInfo.Name}.toc");
                        }
                        FileInfo sbFi = new FileInfo(modDataPath + m_patchPath + "/" + completedAction.SuperBundleInfo.Name + ".toc");
                        if (!sbFi.Exists && srcPath != string.Empty)
                        {
                            Directory.CreateDirectory(sbFi.DirectoryName);
                            cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                        }

                        foreach (string catalog in completedAction.SuperBundleInfo.SplitSuperBundles)
                        {
                            string name = completedAction.SuperBundleInfo.Name.Replace("win32", catalog);
                            srcPath = m_fs.ResolvePath($"native_patch/{name}.toc");
                            if (!m_hasPatchFolder)
                            {
                                srcPath = m_fs.ResolvePath($"native_data/{name}.toc");
                            }
                            sbFi = new FileInfo(modDataPath + m_patchPath + "/" + name + ".toc");
                            if (!sbFi.Exists && srcPath != string.Empty)
                            {
                                Directory.CreateDirectory(sbFi.DirectoryName);
                                cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                            }
                        }

                        if (completedAction.HasSb)
                        {
                            srcPath = m_fs.ResolvePath($"native_patch/{completedAction.SuperBundleInfo.Name}.sb");
                            if (!m_hasPatchFolder)
                            {
                                srcPath = m_fs.ResolvePath($"native_data/{completedAction.SuperBundleInfo.Name}.sb");
                            }
                            sbFi = new FileInfo(modDataPath + m_patchPath + "/" + completedAction.SuperBundleInfo.Name + ".sb");
                            if (!sbFi.Exists && srcPath != string.Empty)
                            {
                                Directory.CreateDirectory(sbFi.DirectoryName);
                                cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                            }

                            foreach (string catalog in completedAction.SuperBundleInfo.SplitSuperBundles)
                            {
                                string name = completedAction.SuperBundleInfo.Name.Replace("win32", catalog);
                                srcPath = m_fs.ResolvePath($"native_patch/{name}.sb");
                                if (!m_hasPatchFolder)
                                {
                                    srcPath = m_fs.ResolvePath($"native_data/{name}.sb");
                                }
                                sbFi = new FileInfo(modDataPath + m_patchPath + "/" + name + ".sb");
                                if (!sbFi.Exists && srcPath != string.Empty)
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
                    using (DbReader reader = new DbReader(new FileStream(m_fs.BasePath + m_patchPath + "/layout.toc", FileMode.Open, FileAccess.Read), m_fs.CreateDeobfuscator()))
                        layout = reader.ReadDbObject();

                    FifaBundleAction.CasFileCount = m_fs.CasFileCount;
                    List<FifaBundleAction> actions = new List<FifaBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    // @todo: Added bundles

                    int totalTasks = 0;
                    foreach (CatalogInfo ci in m_fs.EnumerateCatalogInfos())
                    {
                        FifaBundleAction action = new FifaBundleAction(ci, doneEvent, this, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        m_numTasks++;
                        totalTasks++;
                    }

                    while (m_numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        ReportProgress(totalTasks - m_numTasks, totalTasks);
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
                    using (DbWriter writer = new DbWriter(new FileStream(modDataPath + m_patchPath + "/layout.toc", FileMode.Create), true))
                        writer.Write(layout);
                }

                // swbf2, bfv, and sws
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    List<ManifestBundleAction> actions = new List<ManifestBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    if (m_addedBundles.Count != 0)
                    {
                        int hash = Fnv1a.HashString("<none>");
                        foreach (string bundleName in m_addedBundles[hash])
                            m_fs.AddManifestBundle(new ManifestBundleInfo() { hash = Fnv1.HashString(bundleName) });
                    }

                    Dictionary<string, List<ModBundleInfo>> tasks = new Dictionary<string, List<ModBundleInfo>>();
                    foreach (ModBundleInfo bundle in m_modifiedBundles.Values)
                    {
                        ManifestBundleInfo manifestBundle = m_fs.GetManifestBundle(bundle.Name);
                        string catalog = m_fs.GetCatalog(manifestBundle.files[0].file);

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
                        m_numTasks++;
                        totalTasks++;
                    }

                    while (m_numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        ReportProgress(totalTasks - m_numTasks, totalTasks);
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
                                if (!m_archiveData.ContainsKey(completedAction.BundleRefs[i]))
                                    m_archiveData.TryAdd(completedAction.BundleRefs[i], new ArchiveInfo() { Data = completedAction.BundleBuffers[i] });
                            }

                            // add refs to be added to cas (and manifest)
                            for (int i = 0; i < completedAction.DataRefs.Count; i++)
                                m_casData.Add(m_fs.GetCatalog(completedAction.FileInfos[i].FileInfo.file), completedAction.DataRefs[i], completedAction.FileInfos[i].Entry, completedAction.FileInfos[i].FileInfo);
                        }
                    }

                    // now process manifest chunk changes
                    if (m_modifiedSuperBundles.ContainsKey(0))
                    {
                        foreach (Guid id in m_modifiedSuperBundles[0].Modify.Chunks)
                        {
                            ChunkAssetEntry entry = m_modifiedChunks[id];
                            ManifestChunkInfo ci = m_fs.GetManifestChunk(entry.Id);

                            if (ci != null)
                            {
                                if (entry.TocChunkSpecialHack)
                                {
                                    // change to using the first catalog
                                    ci.file.file = new ManifestFileRef(0, false, 0);
                                }

                                m_casData.Add(m_fs.GetCatalog(ci.file.file), entry.Sha1, entry, ci.file);
                            }
                        }
                        foreach (Guid id in m_modifiedSuperBundles[0].Add.Chunks)
                        {
                            ChunkAssetEntry entry = m_modifiedChunks[id];

                            ManifestChunkInfo ci = new ManifestChunkInfo
                            {
                                guid = entry.Id,
                                file = new ManifestFileInfo { file = new ManifestFileRef(0, false, 0), isChunk = true }
                            };
                            m_fs.AddManifestChunk(ci);

                            m_casData.Add(m_fs.GetCatalog(ci.file.file), entry.Sha1, entry, ci.file);
                        }
                    }
                }

                // remaining games
                else
                {
                    List<SuperBundleAction> actions = new List<SuperBundleAction>();
                    ManualResetEvent doneEvent = new ManualResetEvent(false);

                    int totalTasks = 0;
                    foreach (string superBundle in m_fs.SuperBundles)
                    {
                        if (m_fs.ResolvePath(superBundle + ".toc") == "")
                            continue;

                        SuperBundleAction action = new SuperBundleAction(superBundle, doneEvent, this, m_modDirName + "/" + m_patchPath, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        m_numTasks++;
                        totalTasks++;
                    }

                    foreach (string superBundle in m_addedSuperBundles)
                    {
                        SuperBundleAction action = new SuperBundleAction(superBundle, doneEvent, this, m_modDirName + "/" + m_patchPath, cancelToken);
                        ThreadPool.QueueUserWorkItem(action.ThreadPoolCallback, null);
                        actions.Add(action);
                        m_numTasks++;
                        totalTasks++;
                    }

                    while (m_numTasks != 0)
                    {
                        // show progress
                        cancelToken.ThrowIfCancellationRequested();
                        m_logger.Log("progress:" + ((totalTasks - m_numTasks) / (double)totalTasks) * 100.0d);
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
                            string srcPath = m_fs.ResolvePath(completedAction.SuperBundle + ".toc");
                            FileInfo sbFi = new FileInfo(modDataPath + m_patchPath + "/" + completedAction.SuperBundle + ".toc");

                            if (!Directory.Exists(sbFi.DirectoryName))
                                Directory.CreateDirectory(sbFi.DirectoryName);

                            cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                        }

                        if (!completedAction.SbModified)
                        {
                            string srcPath = m_fs.ResolvePath(completedAction.SuperBundle + ".sb");
                            FileInfo sbFi = new FileInfo(modDataPath + m_patchPath + "/" + completedAction.SuperBundle + ".sb");

                            if (!Directory.Exists(sbFi.DirectoryName))
                                Directory.CreateDirectory(sbFi.DirectoryName);

                            cmdArgs.Add(new SymLinkStruct(sbFi.FullName, srcPath, false));
                        }

                        if (completedAction.CasRefs.Count != 0)
                        {
                            string catalogPath = m_fs.GetCatalogFromSuperBundle(completedAction.SuperBundle);
                            for (int i = 0; i < completedAction.CasRefs.Count; i++)
                                m_casData.Add(catalogPath, completedAction.CasRefs[i]);
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

                int totalEntries = m_casData.CountEntries();
                int currentEntry = 0;
                if (totalEntries > 0)
                {
                    ReportProgress(currentEntry, totalEntries);
                }

                // write out cas and modify cats
                foreach (CasDataEntry entry in m_casData.EnumerateEntries())
                {
                    if (!entry.HasEntries)
                        continue;

                    cancelToken.ThrowIfCancellationRequested();
                    if (!File.Exists(modDataPath + m_patchPath + "\\" + entry.Catalog + "\\cas.cat"))
                    {
                        if (!File.Exists(m_fs.BasePath + "data\\" + entry.Catalog + "\\cas.cat"))
                            continue;

                        using (NativeReader reader = new NativeReader(new FileStream(m_fs.BasePath + "data\\" + entry.Catalog + "\\cas.cat", FileMode.Open, FileAccess.Read)))
                        {
                            FileInfo fi = new FileInfo(modDataPath + m_patchPath + "\\" + entry.Catalog + "\\cas.cat");
                            if (!fi.Directory.Exists)
                                Directory.CreateDirectory(fi.Directory.FullName);

                            using (NativeWriter writer = new NativeWriter(new FileStream(modDataPath + m_patchPath + "\\" + entry.Catalog + "\\cas.cat", FileMode.Create)))
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

                    WriteArchiveData(modDataPath + m_patchPath + "\\" + entry.Catalog, entry);

                    ReportProgress(currentEntry++, totalEntries);
                }

                cancelToken.ThrowIfCancellationRequested();

                Logger.Log("Writing Manifest");
                App.Logger.Log("Writing Manifest");

                // finally copy in the left over patch data
                if (m_modifiedFs.Count > 0)
                {
                    m_fs.WriteInitFs(m_fs.BasePath + m_patchPath + "/initfs_win32", modDataPath + m_patchPath + "/initfs_win32", m_modifiedFs);
                }
                else
                {
                    CopyFileIfRequired(m_fs.BasePath + m_patchPath + "/initfs_win32", modDataPath + m_patchPath + "/initfs_win32");
                }


                if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                    ProfileVersion.Battlefield4,
                    ProfileVersion.NeedForSpeed,
                    ProfileVersion.PlantsVsZombiesGardenWarfare2,
                    ProfileVersion.NeedForSpeedRivals))
                {
                    // modify layout.toc for any new superbundles added
                    DbObject layout = null;
                    using (DbReader reader = new DbReader(new FileStream(m_fs.BasePath + m_patchPath + "/layout.toc", FileMode.Open, FileAccess.Read), m_fs.CreateDeobfuscator()))
                        layout = reader.ReadDbObject();

                    foreach (string path in Directory.EnumerateFiles(modDataPath + m_patchPath, "*.sb", SearchOption.AllDirectories))
                    {
                        // remove path, and extension and replace \ with /
                        string sbName = path.Replace(modDataPath + m_patchPath + "\\", "").Replace("\\", "/").Replace(".sb", "");
                        foreach (DbObject entry in layout.GetValue<DbObject>("superBundles"))
                        {
                            if (entry.GetValue<string>("name").Equals(sbName, StringComparison.OrdinalIgnoreCase))
                            {
                                entry.RemoveValue("same");
                                entry.SetValue("delta", true);
                            }
                        }
                    }

                    using (DbWriter writer = new DbWriter(new FileStream(modDataPath + m_patchPath + "/layout.toc", FileMode.Create), true))
                        writer.Write(layout);
                }

                else if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.Madden20))
                {
                    DbObject layout = null;
                    using (DbReader reader = new DbReader(new FileStream(m_fs.ResolvePath("layout.toc"), FileMode.Open, FileAccess.Read), m_fs.CreateDeobfuscator()))
                        layout = reader.ReadDbObject();

                    // write out new manifest
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                    {
                        DbObject manifest = layout.GetValue<DbObject>("manifest");
                        ManifestFileRef fileRef = (ManifestFileRef)manifest.GetValue<int>("file");

                        byte[] tmpBuf = m_fs.WriteManifest();
                        string catalog = m_fs.GetCatalog(fileRef);

                        // find the next available cas
                        int casIndex = 1;
                        while (File.Exists(modDataPath + m_patchPath + "/" + (string.Format("{0}\\cas_{1}.cas", catalog, casIndex.ToString("D2")))))
                            casIndex++;

                        Sha1 sha1 = Utils.GenerateSha1(tmpBuf);

                        m_archiveData.TryAdd(sha1, new ArchiveInfo() { Data = tmpBuf });
                        WriteArchiveData(modDataPath + m_patchPath + "/" + catalog, new CasDataEntry("", sha1));

                        manifest.SetValue("size", tmpBuf.Length);
                        manifest.SetValue("offset", 0);
                        manifest.SetValue("sha1", sha1);
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5)) //more patch directory shenanigans
                            manifest.SetValue("file", (int)new ManifestFileRef(fileRef.CatalogIndex, false, casIndex));
                        else
                            manifest.SetValue("file", (int)new ManifestFileRef(fileRef.CatalogIndex, true, casIndex));
                    }

                    // add any new superbundles
                    if (m_addedSuperBundles.Count > 0)
                    {
                        foreach (string superBundle in m_addedSuperBundles)
                        {
                            DbObject sbobj = new DbObject();
                            sbobj.SetValue("name", superBundle);
                            layout.GetValue<DbObject>("superBundles").Add(sbobj);

                            DbObject chunk = (DbObject)layout.GetValue<DbObject>("installManifest").GetValue<DbObject>("installChunks")[1];
                            chunk.GetValue<DbObject>("superbundles").Add(superBundle);
                        }
                    }

                    string layoutLocation = modDataPath + m_patchPath + "/layout.toc";
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
                    CopyFileIfRequired(m_fs.BasePath + m_patchPath + "/../package.mft", modDataPath + m_patchPath + "/../package.mft");
                }

                // swbf2, bfv, sws
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    // copy from old data to new data
                    CopyFileIfRequired(m_fs.BasePath + "Data/chunkmanifest", modDataPath + "Data/chunkmanifest");
                    if (m_modifiedFs.Count > 0)
                    {
                        m_fs.WriteInitFs(m_fs.BasePath + "Data/initfs_Win32", modDataPath + "Data/initfs_Win32", m_modifiedFs);
                    }
                    else
                    {
                        CopyFileIfRequired(m_fs.BasePath + "Data/initfs_Win32", modDataPath + "Data/initfs_Win32");
                    }
                }

                // create the frosty mod list file
                File.WriteAllText(Path.Combine(modDataPath, m_patchPath, "mods.json"), JsonConvert.SerializeObject(GenerateModInfoList(modPaths, rootPath), Formatting.Indented));
            }

            cancelToken.ThrowIfCancellationRequested();

            // DAI and NFS dont require bcrypt
            if (!ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition,
                ProfileVersion.Battlefield4,
                ProfileVersion.NeedForSpeed,
                ProfileVersion.NeedForSpeedRivals))
            {
                // delete old useless bcrypt
                if (File.Exists(m_fs.BasePath + "bcrypt.dll"))
                    File.Delete(m_fs.BasePath + "bcrypt.dll");

                // copy over new CryptBase
                CopyFileIfRequired("ThirdParty/CryptBase.dll", m_fs.BasePath + "CryptBase.dll");
            }
            CopyFileIfRequired(m_fs.BasePath + "user.cfg", modDataPath + "user.cfg");

            // FIFA games require a fifaconfig workaround
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                ProfileVersion.Fifa18,
                ProfileVersion.Fifa19,
                ProfileVersion.Fifa20))
            {
                FileInfo fi = new FileInfo(m_fs.BasePath + "FIFASetup\\fifaconfig_orig.exe");
                if (!fi.Exists)
                {
                    fi = new FileInfo(m_fs.BasePath + "FIFASetup\\fifaconfig.exe");
                    fi.MoveTo(fi.FullName.Replace(".exe", "_orig.exe"));
                }

                CopyFileIfRequired("thirdparty/fifaconfig.exe", m_fs.BasePath + "FIFASetup\\fifaconfig.exe");
            }

            // launch the game (redirecting to the modPath directory)
            Logger.Log("Launching Game");

            try
            {
                //KillEADesktop();
                //ModifyInstallerData($"-dataPath \"{modDataPath.Trim('\\')}\" {additionalArgs}");
                ExecuteProcess($"{m_fs.BasePath + ProfilesLibrary.ProfileName}.exe", $"-dataPath \"{modDataPath.Trim('\\')}\" {additionalArgs}");
                //WaitForGame();
                //CleanUpInstalledData();
            }
            catch (Exception ex)
            {
                App.Logger.Log("Error Launching Game: " + ex);
            }

            App.Logger.Log("Done");

            GC.Collect();
            return 0;
        }

        private void KillEADesktop()
        {
            foreach (Process process in Process.GetProcessesByName("EADesktop"))
            {
                try
                {
                    string processFilename = process?.MainModule?.ModuleName;
                    if (string.IsNullOrEmpty(processFilename))
                    {
                        continue;
                    }

                    FileInfo fi = new FileInfo(processFilename);
                    if (process.ProcessName.IndexOf("EADesktop.exe", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        break;
                    }
                }
                catch
                {
                }
            }
        }

        private void WaitForGame()
        {
            DateTime time = DateTime.UtcNow;
            TimeSpan maxTime = new TimeSpan(0, 0, 30);

            while (true)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    try
                    {
                        string processFilename = process?.MainModule?.ModuleName;
                        if (string.IsNullOrEmpty(processFilename))
                        {
                            continue;
                        }

                        FileInfo fi = new FileInfo(processFilename);
                        if (process.ProcessName.IndexOf(ProfilesLibrary.ProfileName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
                if (DateTime.UtcNow > time + maxTime)
                {
                    break;
                }
            }
        }

        private void ModifyInstallerData(string parameters)
        {
            // clean up previous modification if it was not cleaned up already
            if (File.Exists(m_fs.BasePath + "__Installer/installerdata.xml.orig"))
            {
                if (File.Exists(m_fs.BasePath + "__Installer/installerdata.xml"))
                {
                    File.Delete(m_fs.BasePath + "__Installer/installerdata.xml");
                }
                File.Copy(m_fs.BasePath + "__Installer/installerdata.xml.orig", m_fs.BasePath + "__Installer/installerdata.xml");
            }
            else
            {
                if (File.Exists(m_fs.BasePath + "__Installer/installerdata.xml"))
                {
                    File.Copy(m_fs.BasePath + "__Installer/installerdata.xml", m_fs.BasePath + "__Installer/installerdata.xml.orig");
                }
            }

            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(m_fs.BasePath + "__Installer/installerdata.xml");
            System.Xml.Linq.XElement parametersElem = doc.Root.Element("runtime")?.Elements("launcher").ToArray()[1].Element("parameters");
            parametersElem.SetValue(parameters);
            doc.Save(m_fs.BasePath + "__Installer/installerdata.xml");
        }

        private void CleanUpInstalledData()
        {
            if (File.Exists(m_fs.BasePath + "__Installer/installerdata.xml.orig") && File.Exists(m_fs.BasePath + "__Installer/installerdata.xml"))
            {
                File.Delete(m_fs.BasePath + "__Installer/installerdata.xml");
                File.Move(m_fs.BasePath + "__Installer/installerdata.xml.orig", m_fs.BasePath + "__Installer/installerdata.xml");
            }
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
                ArchiveInfo info = m_archiveData[sha1];

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
            using (CatReader reader = new CatReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read), m_fs.CreateDeobfuscator()))
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

                        ArchiveInfo info = m_archiveData[sha1];

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
                foreach (string catalog in m_fs.Catalogs)
                {
                    string basePatchCatalog = m_fs.ResolvePath("native_patch/" + catalog);
                    if (!m_hasPatchFolder)
                    {
                        basePatchCatalog = m_fs.ResolvePath("native_data/" + catalog);
                    }

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
            string baseLayoutPath = m_fs.ResolvePath("native_patch/layout.toc");
            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
            {
                baseLayoutPath = m_fs.ResolvePath("native_data/layout.toc");
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
            using (DbReader reader = new DbReader(new FileStream(baseLayoutPath, FileMode.Open, FileAccess.Read), m_fs.CreateDeobfuscator()))
                patchLayout = reader.ReadDbObject();

            DbObject modLayout = null;
            using (DbReader reader = new DbReader(new FileStream(modLayoutPath, FileMode.Open, FileAccess.Read), m_fs.CreateDeobfuscator()))
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

        public static void ExecuteProcess(string processName, string args = "", bool waitForExit = false, bool asAdmin = false, Dictionary<string, string> env = null)
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
