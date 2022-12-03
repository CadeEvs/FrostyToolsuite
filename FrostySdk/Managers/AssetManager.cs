using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using FrostySdk.Managers.Entries;
using FrostySdk.BaseProfile;

namespace FrostySdk.Managers
{
    public class AssetManagerImportResult
    {
        public bool InvalidatedDueToPatch { get; internal set; }
        public List<EbxAssetEntry> AddedAssets { get; internal set; }
        public List<EbxAssetEntry> ModifiedAssets { get; internal set; }
        public List<EbxAssetEntry> RemovedAssets { get; internal set; }
    }

    public interface ICustomAssetManager
    {
        bool ShouldInitializeOnStartup { get; }

        void Initialize(ILogger logger);
        AssetEntry GetAssetEntry(string key);
        Stream GetAsset(AssetEntry entry);
        void ModifyAsset(string key, byte[] data);
        IEnumerable<AssetEntry> EnumerateAssets(bool modifiedOnly);
        void OnCommand(string command, params object[] value);
    }

    public partial class AssetManager : ILoggable
    {
        #region -- Classes --
        internal class BinarySbDataHelper
        {
            Dictionary<string, byte[]> ebxDataFiles = new Dictionary<string, byte[]>();
            Dictionary<string, byte[]> resDataFiles = new Dictionary<string, byte[]>();
            Dictionary<string, byte[]> chunkDataFiles = new Dictionary<string, byte[]>();
            AssetManager am;

            public BinarySbDataHelper(AssetManager inParent)
            {
                am = inParent;
            }

            public void FilterAndAddBundleData(DbObject baseList, DbObject deltaList)
            {
                FilterBinaryBundleData(baseList, deltaList, "ebx", ebxDataFiles);
                FilterBinaryBundleData(baseList, deltaList, "res", resDataFiles);
                FilterBinaryBundleData(baseList, deltaList, "chunks", chunkDataFiles);
            }

            public void RemoveEbxData(string name) => ebxDataFiles.Remove(name);

            public void RemoveResData(string name) => resDataFiles.Remove(name);

            public void RemoveChunkData(string name) => chunkDataFiles.Remove(name);

            public void WriteToCache(AssetManager am)
            {
                int totalCount = ebxDataFiles.Count + resDataFiles.Count + chunkDataFiles.Count;
                if (totalCount == 0)
                    return;

                FileInfo fi = new FileInfo(am.m_fileSystem.CacheName + "_sbdata.cas");
                if (!Directory.Exists(fi.DirectoryName))
                    Directory.CreateDirectory(fi.DirectoryName);

                using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
                {
                    foreach (KeyValuePair<string, byte[]> kvp in ebxDataFiles)
                    {
                        EbxAssetEntry entry = am.m_ebxList[kvp.Key];
                        entry.ExtraData.DataOffset = writer.Position;

                        writer.Write(kvp.Value);
                    }
                    foreach (KeyValuePair<string, byte[]> kvp in resDataFiles)
                    {
                        ResAssetEntry entry = am.m_resList[kvp.Key];
                        entry.ExtraData.DataOffset = writer.Position;

                        writer.Write(kvp.Value);
                    }
                    foreach (KeyValuePair<string, byte[]> kvp in chunkDataFiles)
                    {
                        Guid chunkId = new Guid(kvp.Key);
                        ChunkAssetEntry entry = am.m_chunkList[chunkId];

                        entry.ExtraData.DataOffset = writer.Position;
                        writer.Write(kvp.Value);
                    }
                }

                ebxDataFiles.Clear();
                resDataFiles.Clear();
                chunkDataFiles.Clear();
            }

            private void FilterBinaryBundleData(DbObject baseList, DbObject deltaList, string listName, Dictionary<string, byte[]> dataFiles)
            {
                foreach (DbObject entry in deltaList.GetValue<DbObject>(listName))
                {
                    Sha1 sha1 = entry.GetValue<Sha1>("sha1");
                    string name = entry.GetValue<string>("name");
                    if (name == null)
                        name = entry.GetValue<Guid>("id").ToString();

                    if (dataFiles.ContainsKey(name))
                        continue;

                    bool bFound = false;
                    if (baseList != null)
                    {
                        foreach (DbObject baseEntry in baseList.GetValue<DbObject>(listName))
                        {
                            if (baseEntry.GetValue<Sha1>("sha1") == sha1)
                            {
                                entry.SetValue("size", baseEntry.GetValue<long>("size"));
                                entry.SetValue("originalSize", baseEntry.GetValue<long>("originalSize"));
                                entry.SetValue("offset", baseEntry.GetValue<long>("offset"));
                                entry.RemoveValue("data");

                                bFound = true;
                                break;
                            }
                        }
                    }

                    if (!bFound)
                    {
                        byte[] data = Utils.CompressFile(entry.GetValue<byte[]>("data"));
                        dataFiles.Add(name, data);

                        entry.SetValue("size", data.Length);
                        entry.AddValue("cache", true);
                        entry.RemoveValue("sb");
                    }
                }
            }
        }

        internal class BaseBundleInfo
        {
            public string Name;
            public string SbName;
            public long Offset;
            public long Size;
            public bool IsPatch;
        }

        internal interface IAssetLoader
        {
            void Load(AssetManager parent, BinarySbDataHelper helper);
        }
        #endregion

        private const ulong CacheMagic = 0x02005954534F5246;
        private const uint CacheVersion = 2;

        private FileSystemManager m_fileSystem;
        private ResourceManager m_resourceManager;
        private ILogger m_logger;

        private List<SuperBundleEntry> m_superBundles = new List<SuperBundleEntry>();
        private List<BundleEntry> m_bundles = new List<BundleEntry>();
        private Dictionary<string, EbxAssetEntry> m_ebxList = new Dictionary<string, EbxAssetEntry>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, ResAssetEntry> m_resList = new Dictionary<string, ResAssetEntry>();
        private Dictionary<Guid, ChunkAssetEntry> m_chunkList = new Dictionary<Guid, ChunkAssetEntry>();

        //private Dictionary<string, bool> ebxPaths = new Dictionary<string, bool>();
        private Dictionary<Guid, EbxAssetEntry> m_ebxGuidList = new Dictionary<Guid, EbxAssetEntry>();
        private Dictionary<ulong, ResAssetEntry> m_resRidList = new Dictionary<ulong, ResAssetEntry>();

        private Dictionary<string, ICustomAssetManager> m_customAssetManagers = new Dictionary<string, ICustomAssetManager>();

        public AssetManager(FileSystemManager inFileSystem, ResourceManager inResourceManager)
        {
            m_fileSystem = inFileSystem;
            m_resourceManager = inResourceManager;
        }

        public void RegisterCustomAssetManager(string type, Type managerType)
        {
            m_customAssetManagers.Add(type, (ICustomAssetManager)Activator.CreateInstance(managerType));
        }

        public void Initialize(bool additionalStartup = true, AssetManagerImportResult result = null)
        {
            DateTime startTime = DateTime.Now;
            List<EbxAssetEntry> prePatchCache = new List<EbxAssetEntry>();

            if (!ReadFromCache(out prePatchCache))
            {
                BinarySbDataHelper helper = new BinarySbDataHelper(this);

                // load in assets from specified loader
                IAssetLoader loader = (IAssetLoader)Activator.CreateInstance(ProfilesLibrary.AssetLoader);
                loader.Load(this, helper);

                // write any patched sb data to cache
                helper.WriteToCache(this);

                GC.Collect();

                if (!additionalStartup)
                {
                    WriteToCache();
                }
            }

            TimeSpan elapsedTime = DateTime.Now - startTime;
            WriteToLog("Loading complete", elapsedTime.ToString());

            if (additionalStartup)
            {
                // index those ebx
                DoEbxIndexing();

                // determine if bundle is a blueprint bundle or a shared bundle
                foreach (BundleEntry bundle in m_bundles)
                {
                    bundle.Type = BundleType.SharedBundle;
                    bundle.Blueprint = GetEbxEntry(bundle.Name.Remove(0, 6));
                    if (bundle.Blueprint == null)
                    {
                        // just try with the win32 in place
                        bundle.Blueprint = GetEbxEntry(bundle.Name);
                    }

                    if (bundle.Blueprint != null)
                    {
                        // is either sublevel or blueprint bundle
                        bundle.Type = BundleType.SubLevel;
                        if (TypeLibrary.IsSubClassOf(bundle.Blueprint.Type, "BlueprintBundle"))
                        {
                            bundle.Type = BundleType.BlueprintBundle;
                        }
                    }
                }

                // now initialize any custom asset managers
                foreach (ICustomAssetManager manager in m_customAssetManagers.Values)
                {
                    if (manager.ShouldInitializeOnStartup)
                    {
                        manager.Initialize(m_logger);
                    }
                }

                if (result != null)
                {
                    result.InvalidatedDueToPatch = prePatchCache != null;
                    if (prePatchCache != null)
                    {
                        WriteToLog("Processing patch results");

                        List<Guid> foundObjs = new List<Guid>();
                        List<EbxAssetEntry> modifiedObjs = new List<EbxAssetEntry>();
                        List<EbxAssetEntry> addedObjs = new List<EbxAssetEntry>();
                        List<EbxAssetEntry> removedObjs = new List<EbxAssetEntry>();

                        foreach (EbxAssetEntry ebx in prePatchCache)
                        {
                            EbxAssetEntry entry = GetEbxEntry(ebx.Guid);
                            if (entry != null)
                            {
                                // entry was found
                                foundObjs.Add(ebx.Guid);

                                // Fifa BinarySb layout doesnt store sha1s, so we cant check what files changed
                                if (entry.Sha1 != ebx.Sha1 && BaseBinarySb.GetMagic() == BaseBinarySb.Magic.Standard)
                                {
                                    // entry was modified
                                    modifiedObjs.Add(entry);
                                }
                            }
                            else
                            {
                                // entry was added
                                removedObjs.Add(new EbxAssetEntry()
                                {
                                    Name = ebx.Name,
                                    Type = ebx.Type,
                                    Guid = ebx.Guid,
                                });
                            }
                        }

                        foreach (EbxAssetEntry entry in m_ebxList.Values)
                        {
                            if (!foundObjs.Contains(entry.Guid))
                            {
                                // entry was added
                                addedObjs.Add(entry);
                            }
                        }

                        result.AddedAssets = addedObjs;
                        result.ModifiedAssets = modifiedObjs;
                        result.RemovedAssets = removedObjs;
                    }
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19,
                        ProfileVersion.Anthem,
                        ProfileVersion.Madden20,
                        ProfileVersion.Fifa20,
                        ProfileVersion.NeedForSpeedHeat,
                        ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                        ProfileVersion.Fifa21,
                        ProfileVersion.Madden22,
                        ProfileVersion.Fifa22,
                        ProfileVersion.Battlefield2042,
                        ProfileVersion.Madden23,
                        ProfileVersion.NeedForSpeedUnbound))
                {
                    // load class infos
                    WriteToLog("Loading type info");
                    TypeLibrary.Reflection.LoadClassInfoAssets(this);
                }
            }
        }

        public void SetLogger(ILogger inLogger) => m_logger = inLogger;

        public void ClearLogger() => m_logger = null;

        public void DoEbxIndexing()
        {
            if (m_ebxGuidList.Count > 0)
            {
                return;
            }

            List<EbxAssetEntry> ebxToRemove = new List<EbxAssetEntry>();
            int assetCount = m_ebxList.Count;
            int count = 0;

            DateTime startTime = DateTime.Now;
            foreach (EbxAssetEntry entry in m_ebxList.Values)
            {
#if ENABLE_LCU
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden20) && entry.ExtraData.CasPath.StartsWith("LCU/"))
                    Console.WriteLine(entry.Name);
#endif

                bool patched = false;
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
                {
                    if (entry.ExtraData.CasPath.StartsWith("native_patch"))
                    {
                        patched = true;
                    }
                }

                Stream stream = GetEbxStream(entry);
                int nameHash = Fnv1.HashString(entry.Name.ToLower());

                if (stream != null)
                {
                    using (EbxReader reader = EbxReader.CreateReader(stream, m_fileSystem, patched))
                    {
                        entry.Type = reader.RootType;
                        entry.Guid = reader.FileGuid;

                        // now grab the actual asset name
                        reader.Position = reader.stringsOffset;
                        string name = reader.ReadNullTerminatedString();
                        int newNameHash = Fnv1.HashString(name.ToLower());

                        // only if the lower case one matches
                        if (newNameHash == nameHash)
                        {
                            entry.Name = name;
                        }

                        foreach (EbxImportReference import in reader.imports)
                        {
                            if (!entry.ContainsDependency(import.FileGuid))
                            {
                                entry.DependentAssets.Add(import.FileGuid);
                            }
                        }

                        if (m_ebxGuidList.ContainsKey(entry.Guid))
                        {
                            //logger.Log("Existing asset found with same guid '{0}'", entry.Guid);
                            continue;
                        }
                        m_ebxGuidList.Add(entry.Guid, entry);
                    }
                }
                else
                {
                    // Mark as encrypted or remove (Both are unloadable types)
                    if (m_resourceManager.IsEncrypted(entry.Sha1))
                    {
                        entry.Type = "EncryptedAsset";
                    }
                    else
                    {
                        ebxToRemove.Add(entry);
                    }
                }

                // SWBF2/BFV/SWS
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    // need to work out bundle here (as bundles are hashed names only)
                    if (TypeLibrary.IsSubClassOf(entry.Type, "BlueprintBundle") || TypeLibrary.IsSubClassOf(entry.Type, "SubWorldData"))
                    {
                        BundleEntry be = m_bundles[entry.Bundles[0]];

                        be.Name = entry.Name;
                        if (!be.Name.StartsWith("win32/", StringComparison.OrdinalIgnoreCase))
                        {
                            be.Name = "win32/" + be.Name;
                        }
                        be.Blueprint = entry;
                    }
                    else if ((ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5) && TypeLibrary.IsSubClassOf(entry.Type, "UIItemDescriptionAsset")) ||
                        ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII) && TypeLibrary.IsSubClassOf(entry.Type, "UIMetaDataAsset") ||
                        ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsSquadrons) && TypeLibrary.IsSubClassOf(entry.Type, "UIMetaDataAsset"))
                    {
                        string bundleName = "win32/" + entry.Name.ToLower() + "_bundle";
                        int h = Fnv1.HashString(bundleName);
                        BundleEntry be = m_bundles.Find((BundleEntry a) => a.Name.Equals(h.ToString("x8")));

                        if (be != null)
                        {
                            be.Name = bundleName;
                        }
                    }
                }

                count++;
                WriteToLog("Initial load - Indexing data ({0}%)", (int)((count / (double)assetCount) * 100.0));
                WriteToLog("progress:{0}", ((count / (double)assetCount) * 100.0d));
            }

            foreach (EbxAssetEntry entry in ebxToRemove)
            {
                m_ebxList.Remove(entry.Name);
            }
            ebxToRemove.Clear();

            WriteToCache();
            WriteToLog("Initial load - Indexing complete");
        }

        public uint GetModifiedCount()
        {
            uint modifiedEbx = (uint)m_ebxList.Values.Count((EbxAssetEntry entry) => entry.IsModified);
            uint modifiedRes = (uint)m_resList.Values.Count((ResAssetEntry entry) => entry.IsModified);
            uint modifiedChunks = (uint)m_chunkList.Values.Count((ChunkAssetEntry entry) => entry.IsModified);
            uint modifiedCustom = 0;
            foreach (ICustomAssetManager mgr in m_customAssetManagers.Values)
            {
                modifiedCustom += (uint)mgr.EnumerateAssets(modifiedOnly: true).Count();
            }

            return modifiedEbx + modifiedRes + modifiedChunks + modifiedCustom;
        }

        public uint GetDirtyCount()
        {
            uint dirtyEbx = (uint)m_ebxList.Values.Count((EbxAssetEntry entry) => entry.IsDirty);
            uint dirtyRes = (uint)m_resList.Values.Count((ResAssetEntry entry) => entry.IsDirty);
            uint dirtyChunks = (uint)m_chunkList.Values.Count((ChunkAssetEntry entry) => entry.IsDirty);
            uint dirtyCustom = 0;
            foreach (ICustomAssetManager mgr in m_customAssetManagers.Values)
            {
                dirtyCustom += (uint)mgr.EnumerateAssets(modifiedOnly: true).Count((AssetEntry a) => a.IsDirty);
            }

            return dirtyEbx + dirtyRes + dirtyChunks + dirtyCustom;
        }

        public uint GetEbxCount(string ebxType)
        {
            return (uint)m_ebxList.Values.Count((EbxAssetEntry entry) => (entry.Type != null && entry.Type.Equals(ebxType)));
        }

        public uint GetEbxCount()
        {
            return (uint)m_ebxList.Count;
        }

        public uint GetResCount(uint resType)
        {
            return (uint)m_resList.Values.Count((ResAssetEntry entry) => entry.ResType == resType);
        }

        public void Reset()
        {
            // clear modifications
            List<EbxAssetEntry> ebxValues = m_ebxList.Values.ToList();
            List<ResAssetEntry> resValues = m_resList.Values.ToList();
            List<ChunkAssetEntry> chunkValues = m_chunkList.Values.ToList();

            foreach (EbxAssetEntry entry in ebxValues)
            {
                RevertAsset(entry, suppressOnModify: false);
            }
            foreach (ResAssetEntry entry in resValues)
            {
                RevertAsset(entry, suppressOnModify: false);
            }
            foreach (ChunkAssetEntry entry in chunkValues)
            {
                RevertAsset(entry, suppressOnModify: false);
            }

            foreach (ICustomAssetManager mgr in m_customAssetManagers.Values)
            {
                foreach (AssetEntry entry in mgr.EnumerateAssets(modifiedOnly: true))
                {
                    RevertAsset(entry, suppressOnModify: false);
                }
            }
        }

        public void RevertAsset(AssetEntry entry, bool dataOnly = false, bool suppressOnModify = true)
        {
            if (!entry.IsModified)
            {
                return;
            }

            foreach (AssetEntry linkedEntry in entry.LinkedAssets)
            {
                RevertAsset(linkedEntry, dataOnly, suppressOnModify);
            }

            // clear modified data
            entry.ClearModifications();

            if (!dataOnly)
            {
                // revert the entire asset
                entry.LinkedAssets.Clear();
                entry.AddedBundles.Clear();
                entry.RemBundles.Clear();

                if (entry.IsAdded)
                {
                    if (entry is EbxAssetEntry ebxEntry)
                    {
                        m_ebxGuidList.Remove(ebxEntry.Guid);
                        m_ebxList.Remove(ebxEntry.Name);
                    }
                    else if (entry is ResAssetEntry resEntry)
                    {
                        m_resRidList.Remove(resEntry.ResRid);
                        m_resList.Remove(resEntry.Name);
                    }
                    else if (entry is ChunkAssetEntry chunkEntry)
                    {
                        m_chunkList.Remove(chunkEntry.Id);
                    }
                }

                entry.IsDirty = false;
                if (!entry.IsAdded && !suppressOnModify)
                {
                    entry.OnModified();
                }
            }
        }

        #region -- Add Functions (Project Only) --
        /// <summary>
        /// Adds a new chunk (Used only by project loading)
        /// </summary>
        public void AddChunk(ChunkAssetEntry entry)
        {
            entry.IsAdded = true;
            m_chunkList.Add(entry.Id, entry);
        }
        public void AddRes(ResAssetEntry entry)
        {
            entry.IsAdded = true;
            m_resList.Add(entry.Name.ToLower(), entry);
            m_resRidList.Add(entry.ResRid, entry);
        }
        public void AddEbx(EbxAssetEntry entry)
        {
            entry.IsAdded = true;
            m_ebxList.Add(entry.Name.ToLower(), entry);
            m_ebxGuidList.Add(entry.Guid, entry);
        }
        #endregion

        #region -- Add Functions --
        /// <summary>
        /// Adds a new bundle to the manager
        /// </summary>
        public BundleEntry AddBundle(string name, BundleType type, int sbIndex)
        {
            int bindex = m_bundles.FindIndex((BundleEntry be) => be.Name == name);
            if (bindex != -1)
            {
                return m_bundles[bindex];
            }

            BundleEntry bentry = new BundleEntry
            {
                Name = name,
                SuperBundleId = sbIndex,
                Type = type,
                Added = true
            };
            m_bundles.Add(bentry);

            return bentry;
        }

        /// <summary>
        /// Adds a new superbundle to the manager
        /// </summary>
        public SuperBundleEntry AddSuperBundle(string name)
        {
            int sbindex = m_superBundles.FindIndex((SuperBundleEntry sbe) => sbe.Name.Equals(name));
            if (sbindex != -1)
            {
                return m_superBundles[sbindex];
            }

            SuperBundleEntry sbentry = new SuperBundleEntry
            {
                Name = name,
                Added = true
            };
            m_superBundles.Add(sbentry);

            return sbentry;
        }

        /// <summary>
        /// Adds a new EBX to the manager
        /// </summary>
        public EbxAssetEntry AddEbx(string name, EbxAsset asset, params int[] bundles)
        {
            string keyName = name.ToLower();
            if (m_ebxList.ContainsKey(keyName))
            {
                return m_ebxList[keyName];
            }

            EbxAssetEntry entry = new EbxAssetEntry
            {
                Name = name,
                Guid = asset.FileGuid,
                Type = asset.RootObject.GetType().Name,
                ModifiedEntry = new ModifiedAssetEntry()
            };

            byte[] buffer = null;
            using (EbxWriter writer = new EbxWriter(new MemoryStream()))
            {
                writer.WriteAsset(asset);
                buffer = writer.ToByteArray();
            }

            entry.AddedBundles.AddRange(bundles);
            entry.ModifiedEntry.DataObject = asset;
            entry.ModifiedEntry.OriginalSize = 0;
            entry.ModifiedEntry.Sha1 = Sha1.Zero;
            entry.ModifiedEntry.IsInline = false;
            entry.IsDirty = true;
            entry.IsAdded = true;

            m_ebxList.Add(keyName, entry);
            m_ebxGuidList.Add(entry.Guid, entry);

            return entry;
        }

        /// <summary>
        /// Adds a new resource to the manager
        /// </summary>
        public ResAssetEntry AddRes(string name, ResourceType resType, byte[] resMeta, byte[] buffer, params int[] bundles)
        {
            name = name.ToLower();
            if (m_resList.ContainsKey(name))
            {
                return m_resList[name];
            }

            ResAssetEntry entry = new ResAssetEntry
            {
                Name = name,
                ResRid = Utils.GenerateResourceId(),
                ResType = (uint)resType,
                ResMeta = resMeta,
                IsAdded = true,
                IsDirty = true
            };

            while (m_resRidList.ContainsKey(entry.ResRid))
            {
                entry.ResRid = Utils.GenerateResourceId();
            }

            entry.AddedBundles.AddRange(bundles);
            entry.ModifiedEntry = new ModifiedAssetEntry
            {
                Data = Utils.CompressFile(buffer),
                OriginalSize = buffer.Length,
                IsInline = false,
                ResMeta = entry.ResMeta
            };

            entry.ModifiedEntry.Sha1 = GenerateSha1(entry.ModifiedEntry.Data);

            m_resList.Add(entry.Name, entry);
            m_resRidList.Add(entry.ResRid, entry);

            return entry;
        }

        /// <summary>
        /// Adds a new chunk to the manager
        /// </summary>
        public Guid AddChunk(byte[] buffer, Guid? overrideGuid = null, Texture texture = null, params int[] bundles)
        {
            ChunkAssetEntry entry = new ChunkAssetEntry { IsAdded = true, IsDirty = true };
            CompressionType compressType = (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18)) ? CompressionType.Oodle : CompressionType.Default;

            entry.ModifiedEntry = new ModifiedAssetEntry
            {
                Data = (texture != null) ? Utils.CompressTexture(buffer, texture, compressType) : Utils.CompressFile(buffer, compressionOverride: compressType),
                LogicalSize = (uint)buffer.Length,
                FirstMip = -1
            };
            entry.ModifiedEntry.Sha1 = GenerateSha1(entry.ModifiedEntry.Data);

            entry.AddedBundles.AddRange(bundles);
            if (texture != null)
            {
                entry.ModifiedEntry.LogicalOffset = texture.LogicalOffset;
                entry.ModifiedEntry.LogicalSize = texture.LogicalSize;
                entry.ModifiedEntry.RangeStart = texture.RangeStart;
                entry.ModifiedEntry.RangeEnd = texture.RangeEnd;
                entry.ModifiedEntry.FirstMip = texture.FirstMip;
            }


            if (overrideGuid.HasValue)
            {
                entry.Id = overrideGuid.Value;
            }
            else
            {
                // @todo: Deterministic guid to reduce conflicts
                byte[] guidBuf = Guid.NewGuid().ToByteArray();
                guidBuf[15] |= 1;

                entry.Id = new Guid(guidBuf);
            }

            m_chunkList.Add(entry.Id, entry);
            return entry.Id;
        }
        #endregion

        #region -- Modify Functions --
        /// <summary>
        /// Modifies chunk based on ID
        /// </summary>
        public bool ModifyChunk(Guid chunkId, byte[] buffer, Texture texture = null)
        {
            if (!m_chunkList.ContainsKey(chunkId))
            {
                return false;
            }

            ChunkAssetEntry entry = m_chunkList[chunkId];
            CompressionType compressType = (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Fifa20)) ? CompressionType.Oodle : CompressionType.Default;
            if ((ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19)) && texture != null)
            {
                compressType = CompressionType.Oodle;
            }

            if (entry.ModifiedEntry == null)
            {
                entry.ModifiedEntry = new ModifiedAssetEntry();
            }

            entry.ModifiedEntry.Data = (texture != null)
                ? Utils.CompressTexture(buffer, texture: texture, compressionOverride: compressType)
                : Utils.CompressFile(buffer, compressionOverride: compressType);

            entry.ModifiedEntry.Sha1 = GenerateSha1(entry.ModifiedEntry.Data);
            entry.ModifiedEntry.LogicalSize = (uint)buffer.Length;

            if (texture != null)
            {
                entry.ModifiedEntry.LogicalOffset = texture.LogicalOffset;
                entry.ModifiedEntry.LogicalSize = texture.LogicalSize;
                entry.ModifiedEntry.RangeStart = texture.RangeStart;
                entry.ModifiedEntry.RangeEnd = (uint)entry.ModifiedEntry.Data.Length;
                entry.ModifiedEntry.FirstMip = texture.FirstMip;
            }

            entry.IsDirty = true;

            return true;
        }

        /// <summary>
        /// Modify resource based on resource id
        /// </summary>
        public void ModifyRes(ulong resRid, byte[] buffer, byte[] meta = null)
        {
            if (!m_resRidList.ContainsKey(resRid))
            {
                return;
            }

            ResAssetEntry entry = m_resRidList[resRid];
            CompressionType compressType = (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Fifa20, ProfileVersion.Fifa21)) ? CompressionType.Oodle : CompressionType.Default;

            if (entry.ModifiedEntry == null)
            {
                entry.ModifiedEntry = new ModifiedAssetEntry();
            }

            entry.ModifiedEntry.Data = Utils.CompressFile(buffer, resType: (ResourceType)entry.ResType, compressionOverride: compressType);
            entry.ModifiedEntry.OriginalSize = buffer.Length;
            entry.ModifiedEntry.Sha1 = GenerateSha1(entry.ModifiedEntry.Data);

            if (meta != null)
            {
                entry.ModifiedEntry.ResMeta = meta;
            }

            entry.IsDirty = true;
        }
        public void ModifyRes(ulong resRid, Resource resource)
        {
            if (!m_resRidList.ContainsKey(resRid))
            {
                return;
            }

            object modifiedResource = resource.SaveModifiedResource();
            if (modifiedResource != null && !m_resRidList[resRid].IsAdded)
            {
                ResAssetEntry entry = m_resRidList[resRid];
                if (entry.ModifiedEntry == null)
                    entry.ModifiedEntry = new ModifiedAssetEntry();

                entry.ModifiedEntry.DataObject = modifiedResource;
                entry.ModifiedEntry.ResMeta = resource.ResourceMeta;
                entry.IsDirty = true;
            }
            else
            {
                ModifyRes(resRid, resource.SaveBytes(), resource.ResourceMeta);
            }
        }

        /// <summary>
        /// Modify resource based on name
        /// </summary>
        public void ModifyRes(string resName, byte[] buffer, byte[] meta = null)
        {
            if (!m_resList.ContainsKey(resName))
            {
                return;
            }

            ResAssetEntry entry = m_resList[resName];
            CompressionType compressType = (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18) ? CompressionType.Oodle : CompressionType.Default;

            if (entry.ModifiedEntry == null)
            {
                entry.ModifiedEntry = new ModifiedAssetEntry();
            }

            entry.ModifiedEntry.Data = Utils.CompressFile(buffer, resType: (ResourceType)entry.ResType, compressionOverride: compressType);
            entry.ModifiedEntry.OriginalSize = buffer.Length;
            entry.ModifiedEntry.Sha1 = GenerateSha1(entry.ModifiedEntry.Data);

            if (meta != null)
            {
                entry.ModifiedEntry.ResMeta = meta;
            }

            entry.IsDirty = true;
        }
        public void ModifyRes(string resName, Resource resource)
        {
            if (!m_resList.ContainsKey(resName))
            {
                return;
            }

            object modifiedResource = resource.SaveModifiedResource();
            if (modifiedResource != null && !m_resList[resName].IsAdded)
            {
                ResAssetEntry entry = m_resList[resName];
                if (entry.ModifiedEntry == null)
                    entry.ModifiedEntry = new ModifiedAssetEntry();

                entry.ModifiedEntry.DataObject = modifiedResource;
                entry.ModifiedEntry.ResMeta = resource.ResourceMeta;
                entry.IsDirty = true;
            }
            else
            {
                ModifyRes(resName, resource.SaveBytes(), resource.ResourceMeta);
            }
        }

        /// <summary>
        /// Modify EBX based on name
        /// </summary>
        public void ModifyEbx(string name, EbxAsset asset)
        {
            name = name.ToLower();
            if (!m_ebxList.ContainsKey(name))
            {
                return;
            }

            EbxAssetEntry entry = m_ebxList[name];

            if (entry.ModifiedEntry == null)
            {
                entry.ModifiedEntry = new ModifiedAssetEntry();
            }

            object modifiedResource = asset.SaveModifiedResource();
            entry.ModifiedEntry.DataObject = modifiedResource ?? asset;
            entry.ModifiedEntry.OriginalSize = 0;
            entry.ModifiedEntry.Sha1 = Sha1.Zero;
            entry.ModifiedEntry.IsTransientModified = asset.TransientEdit;
            entry.ModifiedEntry.DependentAssets.Clear();
            entry.ModifiedEntry.DependentAssets.AddRange(asset.Dependencies);
            entry.IsDirty = true;
        }

        /// <summary>
        /// Modify an asset that exists in a custom manager
        /// </summary>
        public void ModifyCustomAsset(string type, string name, byte[] data)
        {
            if (!m_customAssetManagers.ContainsKey(type))
            {
                return;
            }
            m_customAssetManagers[type].ModifyAsset(name, data);
        }
        #endregion

        #region -- Enumeration Functions --
        public IEnumerable<SuperBundleEntry> EnumerateSuperBundles(bool modifiedOnly = false)
        {
            foreach (SuperBundleEntry sbentry in m_superBundles)
            {
                if (modifiedOnly && !sbentry.Added)
                {
                    continue;
                }
                yield return sbentry;
            }
        }

        public IEnumerable<BundleEntry> EnumerateBundles(BundleType type = BundleType.None, bool modifiedOnly = false)
        {
            foreach (BundleEntry bentry in m_bundles)
            {
                if (type != BundleType.None && bentry.Type != type)
                {
                    continue;
                }

                if (modifiedOnly && !bentry.Added)
                {
                    continue;
                }
                yield return bentry;
            }
        }

        public IEnumerable<EbxAssetEntry> EnumerateEbx(BundleEntry bentry)
        {
            int bindex = m_bundles.IndexOf(bentry);
            return EnumerateEbx("", false, false, true, bindex);
        }

        public IEnumerable<EbxAssetEntry> EnumerateEbx(string type = "", bool modifiedOnly = false, bool includeLinked = false, bool includeHidden = true, string bundleSubPath = "")
        {
            List<int> bundleIndices = new List<int>();
            if (bundleSubPath != "")
            {
                bundleSubPath = bundleSubPath.ToLower();
                for (int i = 0; i < m_bundles.Count; i++)
                {
                    if (m_bundles[i].Name.Equals(bundleSubPath) || m_bundles[i].Name.StartsWith(bundleSubPath + "/"))
                        bundleIndices.Add(i);
                }
            }

            return EnumerateEbx(type, modifiedOnly, includeLinked, includeHidden, bundleIndices.ToArray());
        }

        protected IEnumerable<EbxAssetEntry> EnumerateEbx(string type, bool modifiedOnly, bool includeLinked, bool includeHidden, params int[] bundles)
        {
            foreach (EbxAssetEntry entry in m_ebxList.Values)
            {
                if (modifiedOnly)
                {
                    if (!entry.IsModified)
                        continue;
                    if (entry.IsIndirectlyModified && !includeLinked && !entry.IsDirectlyModified)
                        continue;
                }

                if (type != "" && (entry.Type == null || !TypeLibrary.IsSubClassOf(entry.Type, type)))
                    continue;

                if (bundles.Length != 0)
                {
                    bool bFound = false;
                    foreach (int bindex in bundles)
                    {
                        if (entry.Bundles.Contains(bindex))
                        {
                            bFound = true;
                            break;
                        }
                        else if (entry.AddedBundles.Contains(bindex))
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (!bFound)
                        continue;
                }

                yield return entry;
            }
        }

        public IEnumerable<ResAssetEntry> EnumerateRes(BundleEntry bentry)
        {
            int bindex = m_bundles.IndexOf(bentry);
            if (bindex == -1)
                yield break;
            foreach (ResAssetEntry entry in EnumerateRes(0, false, bindex))
                yield return entry;
        }

        public IEnumerable<ResAssetEntry> EnumerateRes(uint resType = 0, bool modifiedOnly = false, string bundleSubPath = "")
        {
            List<int> bundleIndices = new List<int>();
            if (bundleSubPath != "")
            {
                bundleSubPath = bundleSubPath.ToLower();
                for (int i = 0; i < m_bundles.Count; i++)
                {
                    if (m_bundles[i].Name.Equals(bundleSubPath) || m_bundles[i].Name.StartsWith(bundleSubPath + "/"))
                        bundleIndices.Add(i);
                }
                if (bundleIndices.Count == 0)
                    yield break;
            }

            foreach (ResAssetEntry entry in EnumerateRes(resType, modifiedOnly, bundleIndices.ToArray()))
                yield return entry;
        }

        protected IEnumerable<ResAssetEntry> EnumerateRes(uint resType, bool modifiedOnly, params int[] bundles)
        {
            foreach (ResAssetEntry entry in m_resList.Values)
            {
                if (modifiedOnly && !entry.IsDirectlyModified)
                    continue;
                if (resType != 0 && entry.ResType != resType)
                    continue;
                if (bundles.Length != 0)
                {
                    bool bFound = false;
                    foreach (int bindex in bundles)
                    {
                        if (entry.Bundles.Contains(bindex))
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (!bFound)
                        continue;
                }

                yield return entry;
            }
        }

        public IEnumerable<ChunkAssetEntry> EnumerateChunks(BundleEntry bentry)
        {
            int bindex = m_bundles.IndexOf(bentry);
            if (bindex == -1)
                yield break;
            foreach (ChunkAssetEntry entry in m_chunkList.Values)
            {
                if (entry.Bundles.Contains(bindex))
                    yield return entry;
            }
        }

        public IEnumerable<ChunkAssetEntry> EnumerateChunks(bool modifiedOnly = false)
        {
            foreach (ChunkAssetEntry entry in m_chunkList.Values)
            {
                if (modifiedOnly && !entry.IsDirectlyModified)
                    continue;
                yield return entry;
            }
        }

        public IEnumerable<AssetEntry> EnumerateCustomAssets(string type, bool modifiedOnly = false)
        {
            if (!m_customAssetManagers.ContainsKey(type))
                yield break;
            foreach (AssetEntry entry in m_customAssetManagers[type].EnumerateAssets(modifiedOnly))
                yield return entry;
        }
        #endregion

        #region -- Get Functions --
        public int GetSuperBundleId(SuperBundleEntry sbentry)
            => m_superBundles.FindIndex((SuperBundleEntry sbe) => sbe.Name.Equals(sbentry.Name));

        public int GetSuperBundleId(string sbname)
            => m_superBundles.FindIndex((SuperBundleEntry sbe) => sbe.Name.Equals(sbname, StringComparison.OrdinalIgnoreCase));

        public SuperBundleEntry GetSuperBundle(int id)
            => id >= m_superBundles.Count ? null : m_superBundles[id];

        public int GetBundleId(BundleEntry bentry)
            => m_bundles.FindIndex((BundleEntry be) => be.Name.Equals(bentry.Name));

        public int GetBundleId(string name)
            => m_bundles.FindIndex((BundleEntry be) => be.Name.Equals(name));

        public BundleEntry GetBundleEntry(int bundleId)
            => bundleId >= m_bundles.Count ? null : m_bundles[bundleId];

        public AssetEntry GetCustomAssetEntry(string type, string key)
            => !m_customAssetManagers.ContainsKey(type) ? null : m_customAssetManagers[type].GetAssetEntry(key);

        public T GetCustomAssetEntry<T>(string type, string key) where T : AssetEntry
            => (T)GetCustomAssetEntry(type, key);

        public EbxAssetEntry GetEbxEntry(Guid ebxGuid)
            => !m_ebxGuidList.ContainsKey(ebxGuid) ? null : m_ebxGuidList[ebxGuid];

        public EbxAsset GetEbx(string name, bool getUnmodifiedData = false) => GetEbx(GetEbxEntry(name), getUnmodifiedData);

        public EbxAssetEntry GetEbxEntry(string name)
        {
            name = name.ToLower();
            return !m_ebxList.ContainsKey(name) ? null : m_ebxList[name];
        }

        public ResAssetEntry GetResEntry(ulong resRid) => !m_resRidList.ContainsKey(resRid) ? null : m_resRidList[resRid];

        public ResAssetEntry GetResEntry(string name)
        {
            name = name.ToLower();
            return !m_resList.ContainsKey(name) ? null : m_resList[name];
        }

        public ChunkAssetEntry GetChunkEntry(Guid id) => !m_chunkList.ContainsKey(id) ? null : m_chunkList[id];

        public Stream GetCustomAsset(string type, AssetEntry entry) => !m_customAssetManagers.ContainsKey(type) ? null : m_customAssetManagers[type].GetAsset(entry);

        public T GetEbxAs<T>(EbxAssetEntry entry) where T : EbxAsset, new()
        {
            // return modified data as a data object
            ModifiedResource modifiedResource = null;
            if (entry.ModifiedEntry?.DataObject != null)
                modifiedResource = entry.ModifiedEntry.DataObject as ModifiedResource;

            Stream ebxStream = GetAsset(entry);
            if (ebxStream == null)
                return null;

            bool patched = false;
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem,
                    ProfileVersion.Fifa20,
                    ProfileVersion.PlantsVsZombiesBattleforNeighborville))
            {
                if (entry.ExtraData.CasPath.StartsWith("native_patch"))
                    patched = true;
            }

            using (EbxReader reader = EbxReader.CreateReader(GetAsset(entry), m_fileSystem, patched))
            {
                T asset = reader.ReadAsset<T>();
                if (modifiedResource != null)
                    asset.ApplyModifiedResource(modifiedResource);
                return asset;
            }
        }

        public EbxAsset GetEbx(EbxAssetEntry entry, bool getUnmodifiedData = false)
        {
            // return modified data as a data object
            if ((entry.ModifiedEntry?.DataObject as EbxAsset) != null && !getUnmodifiedData)
            {
                return entry.ModifiedEntry.DataObject as EbxAsset;
            }

            Stream ebxStream = GetAsset(entry);
            if (ebxStream == null)
            {
                return null;
            }

            bool patched = false;
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem,
                    ProfileVersion.Fifa20,
                    ProfileVersion.PlantsVsZombiesBattleforNeighborville))
            {
                if (entry.ExtraData.CasPath.StartsWith("native_patch"))
                {
                    patched = true;
                }
            }

            using (EbxReader reader = EbxReader.CreateReader(GetAsset(entry), m_fileSystem, patched))
            {
                return reader.ReadAsset<EbxAsset>();
            }
        }

        /// <summary>
        /// legacy purposes only
        /// </summary>
        public Stream GetEbxStream(EbxAssetEntry entry)
        {
            if (entry.IsModified)
            {
                using (EbxWriter writer = new EbxWriter(new MemoryStream(), EbxWriteFlags.None, true))
                {
                    writer.WriteAsset(entry.ModifiedEntry.DataObject as EbxAsset);
                    writer.Position = 0;

                    return writer.BaseStream;
                }
            }

            return GetAsset(entry);
        }

        public Stream GetRes(ResAssetEntry entry) => GetAsset(entry);

        public T GetResAs<T>(ResAssetEntry entry, ModifiedResource modifiedData = null) where T : Resource, new()
        {
            Stream stream = GetAsset(entry);
            if (stream == null)
            {
                return default;
            }

            using (NativeReader reader = new NativeReader(stream))
            {
                if (modifiedData == null)
                {
                    if (entry.ModifiedEntry?.DataObject != null)
                    {
                        modifiedData = entry.ModifiedEntry.DataObject as ModifiedResource;
                    }
                }

                T retVal = new T();
                retVal.Read(reader, this, entry, modifiedData);
                return retVal;
            }
        }

        public Stream GetChunk(ChunkAssetEntry entry) => GetAsset(entry);

        public Stream GetRawStream(AssetEntry entry)
        {
            switch (entry.Location)
            {
                case AssetDataLocation.Cas:
                    return (entry.ExtraData != null)
                        ? null
                        : m_resourceManager.GetRawResourceData(entry.Sha1);

                case AssetDataLocation.SuperBundle:
                    return m_resourceManager.GetRawResourceData(((entry.ExtraData.IsPatch) ? "native_patch/" : "native_data/") + m_superBundles[entry.ExtraData.SuperBundleId].Name + ".sb", entry.ExtraData.DataOffset, entry.Size);

                case AssetDataLocation.Cache:
                    return m_resourceManager.GetRawResourceData(entry.ExtraData.DataOffset, entry.Size);

                case AssetDataLocation.CasNonIndexed:
                    return m_resourceManager.GetRawResourceData(entry.ExtraData.CasPath, entry.ExtraData.DataOffset, entry.Size);
            }

            return null;
        }

        private Stream GetAsset(AssetEntry entry)
        {
            // return modified data
            if (entry.ModifiedEntry != null && entry.ModifiedEntry.Data != null)
                return m_resourceManager.GetResourceData(entry.ModifiedEntry.Data);

            // otherwise, find and return original data
            switch (entry.Location)
            {
                case AssetDataLocation.Cas:
                    return (entry.ExtraData != null)
                        ? m_resourceManager.GetResourceData(entry.ExtraData.BaseSha1, entry.ExtraData.DeltaSha1)
                        : m_resourceManager.GetResourceData(entry.Sha1);

                case AssetDataLocation.SuperBundle:
                    return m_resourceManager.GetResourceData(((entry.ExtraData.IsPatch) ? "native_patch/" : "native_data/") + m_superBundles[entry.ExtraData.SuperBundleId].Name + ".sb", entry.ExtraData.DataOffset, entry.Size);

                case AssetDataLocation.Cache:
                    return m_resourceManager.GetResourceData(entry.ExtraData.DataOffset, entry.Size);

                case AssetDataLocation.CasNonIndexed:
                    return m_resourceManager.GetResourceData(entry.ExtraData.CasPath, entry.ExtraData.DataOffset, entry.Size);
            }

            return null;
        }

        #endregion

        #region -- Process Functions --
        private void ProcessBundleEbx(DbObject sb, int bundleId, BinarySbDataHelper helper)
        {
            if (sb.GetValue<DbObject>("ebx") == null)
                return;

            foreach (DbObject ebx in sb.GetValue<DbObject>("ebx"))
            {
                EbxAssetEntry entry = AddEbx(ebx);
                if (entry.Sha1 != ebx.GetValue<Sha1>("sha1") && ebx.GetValue<int>("casPatchType") != 0)
                {
                    entry.Sha1 = ebx.GetValue<Sha1>("sha1");
                    entry.Size = ebx.GetValue<long>("size");
                    entry.OriginalSize = ebx.GetValue<long>("originalSize");
                    entry.IsInline = ebx.HasValue("idata");
                }

                if (ebx.GetValue<bool>("cache") && entry.Location != AssetDataLocation.Cache)
                    helper.RemoveEbxData(entry.Name);

                // Add to bundle
                entry.Bundles.Add(bundleId);
            }
        }

        private void ProcessBundleRes(DbObject sb, int bundleId, BinarySbDataHelper helper)
        {
            if (sb.GetValue<DbObject>("res") == null)
                return;

            foreach (DbObject res in sb.GetValue<DbObject>("res"))
            {
                ResourceType resType = (ResourceType)res.GetValue<long>("resType");
                if (ProfilesLibrary.IsResTypeIgnored(resType))
                {
                    continue;
                }

                ResAssetEntry entry = AddRes(res);
                if (entry.Sha1 != res.GetValue<Sha1>("sha1") && res.GetValue<int>("casPatchType") != 0)
                {
                    // Remove old resrid ref
                    m_resRidList.Remove(entry.ResRid);

                    // Update asset
                    entry.Sha1 = res.GetValue<Sha1>("sha1");
                    entry.Size = res.GetValue<long>("size");
                    entry.ResRid = (ulong)res.GetValue<long>("resRid");
                    entry.ResMeta = res.GetValue<byte[]>("resMeta");
                    entry.IsInline = res.HasValue("idata");
                    entry.OriginalSize = res.GetValue<long>("originalSize");

                    // Add new resrid ref
                    m_resRidList.Add(entry.ResRid, entry);
                }

                if (res.GetValue<bool>("cache") && entry.Location != AssetDataLocation.Cache)
                    helper.RemoveResData(entry.Name);

                // Add to bundle
                entry.Bundles.Add(bundleId);
            }
        }

        private void ProcessBundleChunks(DbObject sb, int bundleId, BinarySbDataHelper helper)
        {
            if (sb.GetValue<DbObject>("chunks") == null)
                return;

            List<Guid> sorted = new List<Guid>();
            foreach (DbObject chunk in sb.GetValue<DbObject>("chunks"))
            {
                sorted.Add(chunk.GetValue<Guid>("id"));
            }

            sorted = sorted.OrderBy(guid => guid.ToHex()).ToList();
            foreach (DbObject chunk in sb.GetValue<DbObject>("chunks"))
            {
                ChunkAssetEntry entry = AddChunk(chunk/*, chunkMeta*/);
                if (entry.H32 == 0)
                {
                    DbObject chunkMeta = sb.GetValue<DbObject>("chunkMeta")[sorted.IndexOf(entry.Id)] as DbObject;
                    entry.H32 = chunkMeta.GetValue<int>("h32");
                    entry.FirstMip = chunkMeta.GetValue<DbObject>("meta").GetValue<int>("firstMip", -1);
                }

                if (chunk.GetValue<bool>("cache") && entry.Location != AssetDataLocation.Cache)
                    helper.RemoveChunkData(entry.Id.ToString());

                if (entry.Size == 0)
                {
                    entry.Size = chunk.GetValue<long>("size");
                    entry.LogicalOffset = chunk.GetValue<uint>("logicalOffset");
                    entry.LogicalSize = chunk.GetValue<uint>("logicalSize");
                    entry.RangeStart = chunk.GetValue<uint>("rangeStart");
                    entry.RangeEnd = chunk.GetValue<uint>("rangeEnd");
                    entry.BundledSize = chunk.GetValue<uint>("bundledSize");
                    entry.IsInline = chunk.HasValue("idata");
                }

                // Add to bundle
                entry.Bundles.Add(bundleId);
            }
        }

        private DbObject ProcessTocChunks(string superBundleName, BinarySbDataHelper helper, bool isBase = false)
        {
            string filename = m_fileSystem.ResolvePath(superBundleName);
            if (filename == "")
                return null;

            DbObject toc = null;
            using (DbReader reader = new DbReader(new FileStream(filename, FileMode.Open, FileAccess.Read), m_fileSystem.CreateDeobfuscator()))
                toc = reader.ReadDbObject();

            if (isBase && !(ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare2 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals ||
                ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare))
            {
                // dont process base toc chunks unless the patch one only 
                // stores the delta listing

                return toc;
            }

            if (toc.GetValue<DbObject>("chunks") != null)
            {
                foreach (DbObject chunk in toc.GetValue<DbObject>("chunks"))
                {
                    Guid chunkId = chunk.GetValue<Guid>("id");
                    ChunkAssetEntry entry = null;

                    if (m_chunkList.ContainsKey(chunkId))
                    {
                        entry = m_chunkList[chunkId];
                        m_chunkList.Remove(chunkId);

                        // remove cache data (if exists)
                        helper.RemoveChunkData(entry.Id.ToString());
                    }
                    else
                    {
                        entry = new ChunkAssetEntry();
                    }

                    entry.Id = chunk.GetValue<Guid>("id");
                    entry.Sha1 = chunk.GetValue<Sha1>("sha1");
                    entry.FirstMip = -1;

                    if (chunk.GetValue<long>("size") != 0)
                    {
                        entry.Location = AssetDataLocation.SuperBundle;
                        entry.Size = chunk.GetValue<long>("size");

                        entry.ExtraData = new AssetExtraData
                        {
                            DataOffset = chunk.GetValue<long>("offset"),
                            SuperBundleId = m_superBundles.Count - 1,
                            IsPatch = superBundleName.StartsWith("native_patch")
                        };
                    }
                    m_chunkList.Add(entry.Id, entry);
                }
            }

            return toc;
        }

        private EbxAssetEntry AddEbx(DbObject ebx)
        {
            string name = ebx.GetValue<string>("name").ToLower();

            if (m_ebxList.ContainsKey(name))
                return m_ebxList[name];

            EbxAssetEntry entry = new EbxAssetEntry
            {
                Name = name,
                Sha1 = ebx.GetValue<Sha1>("sha1"),
                Size = ebx.GetValue<long>("size"),
                OriginalSize = ebx.GetValue<long>("originalSize"),
                IsInline = ebx.HasValue("idata"),
                Location = AssetDataLocation.Cas
            };

            entry.BaseSha1 = m_resourceManager.GetBaseSha1(entry.Sha1);

            if (ebx.HasValue("cas"))
            {
                entry.Location = AssetDataLocation.CasNonIndexed;

                entry.ExtraData = new AssetExtraData
                {
                    DataOffset = ebx.GetValue<long>("offset"),
                    CasPath = (ebx.HasValue("catalog"))
                        ? m_fileSystem.GetFilePath(ebx.GetValue<int>("catalog"), ebx.GetValue<int>("cas"), ebx.HasValue("patch"))
                        : m_fileSystem.GetFilePath(ebx.GetValue<int>("cas"))
                };
            }
            else if (ebx.GetValue<bool>("sb"))
            {
                entry.Location = AssetDataLocation.SuperBundle;

                entry.ExtraData = new AssetExtraData
                {
                    DataOffset = ebx.GetValue<long>("offset"),
                    SuperBundleId = m_superBundles.Count - 1
                };
            }
            else if (ebx.GetValue<bool>("cache"))
            {
                entry.Location = AssetDataLocation.Cache;

                entry.ExtraData = new AssetExtraData { DataOffset = 0xdeadbeef };
            }
            else if (ebx.GetValue<int>("casPatchType") == 2)
            {
                entry.ExtraData = new AssetExtraData
                {
                    BaseSha1 = ebx.GetValue<Sha1>("baseSha1"),
                    DeltaSha1 = ebx.GetValue<Sha1>("deltaSha1")
                };
            }

            //if (entry.Path != "")
            //{
            //    string[] pathSegments = entry.Path.Split('/');
            //    string currentPath = "";

            //    for (int i = 0; i < pathSegments.Length; i++)
            //    {
            //        currentPath += pathSegments[i] + "/";
            //        if (!ebxPaths.ContainsKey(currentPath))
            //            ebxPaths.Add(currentPath, false);
            //        if (i < pathSegments.Length - 1)
            //            ebxPaths[currentPath] = true;
            //    }
            //}

            m_ebxList.Add(name, entry);
            return entry;
        }

        private ResAssetEntry AddRes(DbObject res)
        {
            string name = res.GetValue<string>("name");

            if (m_resList.ContainsKey(name))
                return m_resList[name];

            ResAssetEntry entry = new ResAssetEntry
            {
                Name = name,
                Sha1 = res.GetValue<Sha1>("sha1"),
                Size = res.GetValue<long>("size"),
                OriginalSize = res.GetValue<long>("originalSize"),
                ResRid = (ulong)res.GetValue<long>("resRid"),
                ResType = (uint)res.GetValue<long>("resType"),
                ResMeta = res.GetValue<byte[]>("resMeta"),
                IsInline = res.HasValue("idata"),
                Location = AssetDataLocation.Cas
            };

            entry.BaseSha1 = m_resourceManager.GetBaseSha1(entry.Sha1);

            if (res.HasValue("cas"))
            {
                entry.Location = AssetDataLocation.CasNonIndexed;

                entry.ExtraData = new AssetExtraData
                {
                    DataOffset = res.GetValue<long>("offset"),
                    CasPath = (res.HasValue("catalog"))
                        ? m_fileSystem.GetFilePath(res.GetValue<int>("catalog"), res.GetValue<int>("cas"), res.HasValue("patch"))
                        : m_fileSystem.GetFilePath(res.GetValue<int>("cas"))
                };
            }
            else if (res.GetValue<bool>("sb"))
            {
                entry.Location = AssetDataLocation.SuperBundle;

                entry.ExtraData = new AssetExtraData
                {
                    DataOffset = res.GetValue<long>("offset"),
                    SuperBundleId = m_superBundles.Count - 1
                };
            }
            else if (res.GetValue<bool>("cache"))
            {
                entry.Location = AssetDataLocation.Cache;

                entry.ExtraData = new AssetExtraData { DataOffset = 0xdeadbeef };
            }
            else if (res.GetValue<int>("casPatchType") == 2)
            {
                entry.ExtraData = new AssetExtraData
                {
                    BaseSha1 = res.GetValue<Sha1>("baseSha1"),
                    DeltaSha1 = res.GetValue<Sha1>("deltaSha1")
                };
            }

            m_resList.Add(name, entry);
            if (entry.ResRid != 0)
                m_resRidList.Add(entry.ResRid, entry);

            return entry;
        }

        private ChunkAssetEntry AddChunk(DbObject chunk/*, DbObject chunkMeta*/)
        {
            Guid chunkId = chunk.GetValue<Guid>("id");

            if (m_chunkList.ContainsKey(chunkId))
            {
                ChunkAssetEntry existingChunk = m_chunkList[chunkId];
                existingChunk.Sha1 = chunk.GetValue<Sha1>("sha1");
                existingChunk.LogicalOffset = chunk.GetValue<uint>("logicalOffset");
                existingChunk.LogicalSize = chunk.GetValue<uint>("logicalSize");
                existingChunk.RangeStart = chunk.GetValue<uint>("rangeStart");
                existingChunk.RangeEnd = chunk.GetValue<uint>("rangeEnd");
                existingChunk.BundledSize = chunk.GetValue<uint>("bundledSize");
                return existingChunk;
            }

            ChunkAssetEntry entry = new ChunkAssetEntry
            {
                Id = chunkId,
                Sha1 = chunk.GetValue<Sha1>("sha1"),
                Size = chunk.GetValue<long>("size"),
                LogicalOffset = chunk.GetValue<uint>("logicalOffset"),
                LogicalSize = chunk.GetValue<uint>("logicalSize"),
                RangeStart = chunk.GetValue<uint>("rangeStart"),
                RangeEnd = chunk.GetValue<uint>("rangeEnd"),
                BundledSize = chunk.GetValue<uint>("bundledSize"),
                IsInline = chunk.HasValue("idata"),
                Location = AssetDataLocation.Cas
            };

            //entry.OriginalSize = chunk.GetValue<long>("originalSize");
            //entry.H32 = chunkMeta.GetValue<int>("h32");
            //entry.FirstMip = chunkMeta.GetValue<DbObject>("meta").GetValue<int>("firstMip");

            if (chunk.HasValue("cas"))
            {
                entry.Location = AssetDataLocation.CasNonIndexed;

                entry.ExtraData = new AssetExtraData
                {
                    DataOffset = chunk.GetValue<long>("offset"),
                    CasPath = (chunk.HasValue("catalog"))
                        ? m_fileSystem.GetFilePath(chunk.GetValue<int>("catalog"), chunk.GetValue<int>("cas"), chunk.HasValue("patch"))
                        : m_fileSystem.GetFilePath(chunk.GetValue<int>("cas"))
                };
            }
            else if (chunk.GetValue<bool>("sb"))
            {
                entry.Location = AssetDataLocation.SuperBundle;

                entry.ExtraData = new AssetExtraData
                {
                    DataOffset = chunk.GetValue<long>("offset"),
                    SuperBundleId = m_superBundles.Count - 1
                };
            }
            else if (chunk.GetValue<bool>("cache"))
            {
                entry.Location = AssetDataLocation.Cache;

                entry.ExtraData = new AssetExtraData { DataOffset = 0xdeadbeef };
            }

            m_chunkList.Add(chunkId, entry);
            return entry;
        }
        #endregion

        #region -- Misc Functions -- 
        public void SendManagerCommand(string type, string command, params object[] value)
        {
            if (m_customAssetManagers.ContainsKey(type))
                m_customAssetManagers[type].OnCommand(command, value);
        }

        private bool ReadFromCache(out List<EbxAssetEntry> prePatchCache)
        {
            prePatchCache = null;
            if (!File.Exists(m_fileSystem.CacheName + ".cache"))
                return false;

            WriteToLog("Loading data (" + m_fileSystem.CacheName + ".cache)");
            bool bIsPatched = false;

            using (NativeReader reader = new NativeReader(new FileStream(m_fileSystem.CacheName + ".cache", FileMode.Open, FileAccess.Read)))
            {
                ulong magic = reader.ReadULong();
                if (magic != CacheMagic)
                    return false;

                uint version = reader.ReadUInt();
                if (version != CacheVersion)
                    return false;

                int profileNameHash = reader.ReadInt();
                if (profileNameHash != Fnv1.HashString(ProfilesLibrary.ProfileName))
                    return false;

                uint head = reader.ReadUInt();
                if (head != m_fileSystem.Head)
                {
                    bIsPatched = true;
                    prePatchCache = new List<EbxAssetEntry>();
                }

                int count = reader.ReadInt();

                // SWBF2/BFV
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    m_superBundles.Add(new SuperBundleEntry() { Name = "<none>" });
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        SuperBundleEntry sbentry = new SuperBundleEntry { Name = reader.ReadNullTerminatedString() };
                        m_superBundles.Add(sbentry);
                    }
                }

                count = reader.ReadInt();
                if (count == 0)
                    return false;

                // bundles
                for (int i = 0; i < count; i++)
                {
                    BundleEntry bentry = new BundleEntry
                    {
                        Name = reader.ReadNullTerminatedString(),
                        SuperBundleId = reader.ReadInt()
                    };

                    // SWBF2: patch weapon bundles with incorrect start
                    if (bentry.Name.StartsWith("win32/Win32"))
                        bentry.Name = bentry.Name.Remove(0, 6);

                    if (!bIsPatched)
                        m_bundles.Add(bentry);
                }

                // ebx
                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    EbxAssetEntry entry = new EbxAssetEntry
                    {
                        Name = reader.ReadNullTerminatedString(),
                        Sha1 = reader.ReadSha1()
                    };
                    entry.BaseSha1 = m_resourceManager.GetBaseSha1(entry.Sha1);
                    entry.Size = reader.ReadLong();
                    entry.OriginalSize = reader.ReadLong();
                    entry.Location = (AssetDataLocation)reader.ReadInt();
                    entry.IsInline = reader.ReadBoolean();

                    entry.Type = reader.ReadNullTerminatedString();
                    Guid ebxGuid = reader.ReadGuid();

                    bool hasExtraData = reader.ReadBoolean();
                    if (hasExtraData)
                    {
                        entry.ExtraData = new AssetExtraData
                        {
                            BaseSha1 = reader.ReadSha1(),
                            DeltaSha1 = reader.ReadSha1(),
                            DataOffset = reader.ReadLong(),
                            SuperBundleId = reader.ReadInt(),
                            IsPatch = reader.ReadBoolean(),
                            CasPath = reader.ReadNullTerminatedString()
                        };
                    }

                    int subCount = reader.ReadInt();
                    for (int j = 0; j < subCount; j++)
                        entry.Bundles.Add(reader.ReadInt());

                    subCount = reader.ReadInt();
                    for (int j = 0; j < subCount; j++)
                        entry.DependentAssets.Add(reader.ReadGuid());

                    if (bIsPatched)
                    {
                        entry.Guid = ebxGuid;
                        prePatchCache.Add(entry);
                    }
                    else
                    {
                        if (ebxGuid != Guid.Empty)
                        {
                            entry.Guid = ebxGuid;
                            if (m_ebxGuidList.ContainsKey(entry.Guid))
                                continue;
                            m_ebxGuidList.Add(ebxGuid, entry);
                        }
                        m_ebxList.Add(entry.Name, entry);
                    }
                }

                // res
                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    ResAssetEntry entry = new ResAssetEntry
                    {
                        Name = reader.ReadNullTerminatedString(),
                        Sha1 = reader.ReadSha1(),
                        //BaseSha1 = rm.GetBaseSha1(entry.Sha1);
                        Size = reader.ReadLong(),
                        OriginalSize = reader.ReadLong(),
                        Location = (AssetDataLocation)reader.ReadInt(),
                        IsInline = reader.ReadBoolean(),
                        ResRid = reader.ReadULong(),
                        ResType = reader.ReadUInt(),
                        ResMeta = reader.ReadBytes(reader.ReadInt())
                    };

                    entry.BaseSha1 = m_resourceManager.GetBaseSha1(entry.Sha1);

                    bool hasExtraData = reader.ReadBoolean();
                    if (hasExtraData)
                    {
                        entry.ExtraData = new AssetExtraData
                        {
                            BaseSha1 = reader.ReadSha1(),
                            DeltaSha1 = reader.ReadSha1(),
                            DataOffset = reader.ReadLong(),
                            SuperBundleId = reader.ReadInt(),
                            IsPatch = reader.ReadBoolean(),
                            CasPath = reader.ReadNullTerminatedString()
                        };
                    }

                    int subCount = reader.ReadInt();
                    for (int j = 0; j < subCount; j++)
                        entry.Bundles.Add(reader.ReadInt());

                    if (!bIsPatched)
                    {
                        m_resList.Add(entry.Name, entry);
                        if (entry.ResRid != 0)
                            m_resRidList.Add(entry.ResRid, entry);
                    }
                }

                // chunk
                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    ChunkAssetEntry entry = new ChunkAssetEntry
                    {
                        Id = reader.ReadGuid(),
                        Sha1 = reader.ReadSha1(),
                        Size = reader.ReadLong(),
                        Location = (AssetDataLocation)reader.ReadInt(),
                        IsInline = reader.ReadBoolean(),
                        BundledSize = reader.ReadUInt(),
                        RangeStart = reader.ReadUInt(),
                        RangeEnd = reader.ReadUInt(),
                        LogicalOffset = reader.ReadUInt(),
                        LogicalSize = reader.ReadUInt()
                    };

                    entry.BaseSha1 = m_resourceManager.GetBaseSha1(entry.Sha1);

                    // these two values are actually read wrong from the initial scan, this is because
                    // the chunk meta is not stored in the same layout as the chunks (which the inital
                    // scan assumes) so these cannot be relied on to be correct, so it is best to
                    // ignore them and recalculate them whenever required.

                    // LinkAsset will recalculate the H32 (hash of owning asset), and texture import will
                    // recalculate the FirstMip, if the FirstMip is required outside of texture importing
                    // then it is best to get it directly from the texture resource.

                    entry.H32 = reader.ReadInt();
                    entry.FirstMip = reader.ReadInt();

                    //entry.H32 = 0;
                    //entry.FirstMip = -1;

                    bool hasExtraData = reader.ReadBoolean();
                    if (hasExtraData)
                    {
                        entry.ExtraData = new AssetExtraData
                        {
                            BaseSha1 = reader.ReadSha1(),
                            DeltaSha1 = reader.ReadSha1(),
                            DataOffset = reader.ReadLong(),
                            SuperBundleId = reader.ReadInt(),
                            IsPatch = reader.ReadBoolean(),
                            CasPath = reader.ReadNullTerminatedString()
                        };
                    }

                    int subCount = reader.ReadInt();
                    for (int j = 0; j < subCount; j++)
                        entry.Bundles.Add(reader.ReadInt());

                    if (!bIsPatched)
                        m_chunkList.Add(entry.Id, entry);
                }
            }

            return !bIsPatched;
        }

        private void WriteToCache()
        {
            FileInfo fi = new FileInfo(m_fileSystem.CacheName + ".cache");
            if (!Directory.Exists(fi.DirectoryName))
                Directory.CreateDirectory(fi.DirectoryName);

            using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
            {
                writer.Write(CacheMagic);
                writer.Write(CacheVersion);
                writer.Write(Fnv1.HashString(ProfilesLibrary.ProfileName));
                writer.Write(m_fileSystem.Head);

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    writer.Write(0);
                }
                else
                {
                    writer.Write(m_superBundles.Count);
                    foreach (SuperBundleEntry sbentry in m_superBundles)
                        writer.WriteNullTerminatedString(sbentry.Name);
                }

                writer.Write(m_bundles.Count);
                foreach (BundleEntry bentry in m_bundles)
                {
                    writer.WriteNullTerminatedString(bentry.Name);
                    writer.Write(bentry.SuperBundleId);
                }

                writer.Write(m_ebxList.Values.Count);
                foreach (EbxAssetEntry entry in m_ebxList.Values)
                {
                    writer.WriteNullTerminatedString(entry.Name);
                    writer.Write(entry.Sha1);
                    writer.Write(entry.Size);
                    writer.Write(entry.OriginalSize);
                    writer.Write((int)entry.Location);
                    writer.Write(entry.IsInline);

                    writer.WriteNullTerminatedString(entry.Type ?? "");
                    writer.Write(entry.Guid);

                    writer.Write(entry.ExtraData != null);
                    if (entry.ExtraData != null)
                    {
                        writer.Write(entry.ExtraData.BaseSha1);
                        writer.Write(entry.ExtraData.DeltaSha1);
                        writer.Write(entry.ExtraData.DataOffset);
                        writer.Write(entry.ExtraData.SuperBundleId);
                        writer.Write(entry.ExtraData.IsPatch);
                        writer.WriteNullTerminatedString(entry.ExtraData.CasPath);
                    }

                    writer.Write(entry.Bundles.Count);
                    foreach (int bentry in entry.Bundles)
                        writer.Write(bentry);

                    writer.Write(entry.DependentAssets.Count);
                    foreach (Guid guid in entry.EnumerateDependencies())
                        writer.Write(guid);
                }

                writer.Write(m_resList.Values.Count);
                foreach (ResAssetEntry entry in m_resList.Values)
                {
                    writer.WriteNullTerminatedString(entry.Name);
                    writer.Write(entry.Sha1);
                    writer.Write(entry.Size);
                    writer.Write(entry.OriginalSize);
                    writer.Write((int)entry.Location);
                    writer.Write(entry.IsInline);

                    writer.Write(entry.ResRid);
                    writer.Write(entry.ResType);
                    writer.Write(entry.ResMeta.Length);
                    writer.Write(entry.ResMeta);

                    writer.Write(entry.ExtraData != null);
                    if (entry.ExtraData != null)
                    {
                        writer.Write(entry.ExtraData.BaseSha1);
                        writer.Write(entry.ExtraData.DeltaSha1);
                        writer.Write(entry.ExtraData.DataOffset);
                        writer.Write(entry.ExtraData.SuperBundleId);
                        writer.Write(entry.ExtraData.IsPatch);
                        writer.WriteNullTerminatedString(entry.ExtraData.CasPath);
                    }

                    writer.Write(entry.Bundles.Count);
                    foreach (int bentry in entry.Bundles)
                        writer.Write(bentry);
                }

                writer.Write(m_chunkList.Count);
                foreach (ChunkAssetEntry entry in m_chunkList.Values)
                {
                    writer.Write(entry.Id);
                    writer.Write(entry.Sha1);
                    writer.Write(entry.Size);
                    writer.Write((int)entry.Location);
                    writer.Write(entry.IsInline);

                    writer.Write(entry.BundledSize);
                    writer.Write(entry.RangeStart);
                    writer.Write(entry.RangeEnd);
                    writer.Write(entry.LogicalOffset);
                    writer.Write(entry.LogicalSize);
                    writer.Write(entry.H32);
                    writer.Write(entry.FirstMip);

                    writer.Write(entry.ExtraData != null);
                    if (entry.ExtraData != null)
                    {
                        writer.Write(entry.ExtraData.BaseSha1);
                        writer.Write(entry.ExtraData.DeltaSha1);
                        writer.Write(entry.ExtraData.DataOffset);
                        writer.Write(entry.ExtraData.SuperBundleId);
                        writer.Write(entry.ExtraData.IsPatch);
                        writer.WriteNullTerminatedString(entry.ExtraData.CasPath);
                    }

                    writer.Write(entry.Bundles.Count);
                    foreach (int bentry in entry.Bundles)
                        writer.Write(bentry);
                }
            }
        }

        private void WriteToLog(string text, params object[] vars) => m_logger?.Log(text, vars);

        private static Sha1 GenerateSha1(byte[] buffer)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                Sha1 newSha1 = new Sha1(sha1.ComputeHash(buffer));
                return newSha1;
            }
        }

        public static string GetLoaderName(string name)
        {
            Dictionary<string, Type> loaderTypes = new Dictionary<string, Type>()
            {
                { "LegacyAssetLoader", typeof(LegacyAssetLoader) },
                { "ManifestAssetLoader", typeof(ManifestAssetLoader) },
                { "StandardAssetLoader", typeof(StandardAssetLoader) },
                { "FifaAssetLoader", typeof(FifaAssetLoader) },
                { "EdgeAssetLoader", typeof(EdgeAssetLoader) },
                { "AnthemAssetLoader", typeof(AnthemAssetLoader) },
                { "CasAssetLoader", typeof(CasAssetLoader) }
            };

            return loaderTypes[name].Name;
        }
        #endregion
    }
}
