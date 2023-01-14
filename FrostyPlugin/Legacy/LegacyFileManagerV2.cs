using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core.Legacy
{
    internal class UnkStruct
    {
        public string Name;
        public int StartIndex;
        public int Length;
    }

    public sealed class LegacyFileManagerV2 : ICustomAssetManager
    {
        /* list of all legacy entries indexed by hash */
        private Dictionary<int, LegacyFileEntry> legacyEntries = new Dictionary<int, LegacyFileEntry>();

        /* 
         * list of cached legacy chunks when running in cache mode (this ensures that the same chunk
         * is not extracted over and over again)
         */
        private Dictionary<Guid, byte[]> cachedChunks = new Dictionary<Guid, byte[]>();
        private bool cacheMode = false;
        
        public bool ShouldInitializeOnStartup => true;

        /* Load in legacy files */
        public void Initialize(ILogger logger)
        {
            logger.Log("Loading legacy files");
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx("ChunkFileCollector"))
            {
                EbxAsset asset = App.AssetManager.GetEbx(entry);
                if (asset == null)
                    continue;

                dynamic obj = asset.RootObject;
                dynamic manifest = obj.Manifest;

                ChunkAssetEntry chunk = App.AssetManager.GetChunkEntry(manifest.ChunkId);
                if (chunk == null)
                    continue;

                Stream chunkStream = App.AssetManager.GetChunk(chunk);
                if (chunkStream == null)
                    continue;

                using (NativeReader reader = new NativeReader(chunkStream))
                {
                    int count1 = reader.ReadInt();
                    long offset1 = reader.ReadLong();

                    int numEntries = reader.ReadInt();
                    long dataOffset = reader.ReadLong();

                    int count2 = reader.ReadInt();
                    long offset2 = reader.ReadLong();

                    int chunkCount = reader.ReadInt();
                    long chunkOffset = reader.ReadLong();

                    reader.Position = offset1;
                    List<string> paths = new List<string>();
                    for (int i = 0; i < count1; i++)
                    {
                        long strOffset = reader.ReadLong();
                        long curPos = reader.Position;
                        reader.Position = strOffset;
                        string name = reader.ReadNullTerminatedString();
                        paths.Add(name);
                        reader.Position = curPos;
                    }

                    reader.Position = dataOffset;
                    for (int i = 0; i < numEntries; i++)
                    {
                        long strOffset = reader.ReadLong();
                        long curPos = reader.Position;
                        reader.Position = strOffset;
                        string name = reader.ReadNullTerminatedString();
                        reader.Position = curPos;

                        int hash = Fnv1.HashString(name);
                        LegacyFileEntry legacyEntry = null;

                        if (!legacyEntries.ContainsKey(hash))
                        {
                            legacyEntry = new LegacyFileEntry { Name = name };
                            legacyEntries.Add(hash, legacyEntry);
                        }
                        else
                        {
                            legacyEntry = legacyEntries[hash];
                        }

                        LegacyFileEntry.ChunkCollectorInstance inst = new LegacyFileEntry.ChunkCollectorInstance
                        {
                            CompressedOffset = reader.ReadLong(),
                            CompressedSize = reader.ReadLong(),
                            Offset = reader.ReadLong(),
                            Size = reader.ReadLong(),
                            ChunkId = reader.ReadGuid(),
                            Entry = entry
                        };
                        legacyEntry.CollectorInstances.Add(inst);
                    }

                    reader.Position = offset2;
                    List<UnkStruct> unkStructs = new List<UnkStruct>();
                    for (int i = 0; i < count2; i++)
                    {
                        long strOffset = reader.ReadLong();
                        long curPos = reader.Position;
                        reader.Position = strOffset;
                        string name = reader.ReadNullTerminatedString();
                        reader.Position = curPos;

                        unkStructs.Add(new UnkStruct()
                        {
                            Name = name,
                            StartIndex = reader.ReadInt(),
                            Length = reader.ReadInt()
                        });
                    }

                    reader.Position = chunkOffset;
                    for (int i = 0; i < chunkCount; i++)
                    {
                        long chunkSize = reader.ReadLong();
                        Guid chunkId = reader.ReadGuid();

                        if (cacheMode)
                        {
                            if (!cachedChunks.ContainsKey(chunkId))
                            {
                                using (Stream chkStream = App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(chunkId)))
                                {
                                    if (chkStream != null)
                                    {
                                        cachedChunks.Add(chunkId, ((MemoryStream)chunkStream).ToArray());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /* Enable/Disable cache mode */
        public void SetCacheModeEnabled(bool enabled)
        {
            cacheMode = enabled;
        }

        /* Flush any loaded chunks */
        public void FlushCache()
        {
            cachedChunks.Clear();
        }

        /* Enumerate through all legacy files */
        public IEnumerable<AssetEntry> EnumerateAssets(bool modifiedOnly)
        {
            foreach (LegacyFileEntry entry in legacyEntries.Values)
            {
                if (modifiedOnly && !entry.IsModified)
                    continue;
                yield return entry;
            }
        }

        /* Obtain legacy file data */
        public AssetEntry GetAssetEntry(string key)
        {
            int hash = Fnv1.HashString(key);
            if (legacyEntries.ContainsKey(hash))
                return legacyEntries[hash];
            return null;
        }

        /* Obtain legacy file data */
        public Stream GetAsset(AssetEntry entry)
        {
            LegacyFileEntry lfe = entry as LegacyFileEntry;
            Stream chunkStream = GetChunkStream(lfe);
            if (chunkStream == null)
                return null;

            using (NativeReader reader = new NativeReader(chunkStream))
            {
                LegacyFileEntry.ChunkCollectorInstance inst = (lfe.IsModified)
                    ? lfe.CollectorInstances[0].ModifiedEntry
                    : lfe.CollectorInstances[0];

                reader.Position = inst.Offset;
                return new MemoryStream(reader.ReadBytes((int)inst.Size));
            }
        }

        public void ModifyAsset(string key, byte[] data)
        {
            int hash = Fnv1.HashString(key);
            if (!legacyEntries.ContainsKey(hash))
                return;

            LegacyFileEntry lfe = legacyEntries[hash];

            MemoryStream ms = new MemoryStream();
            using (NativeWriter writer = new NativeWriter(ms, true))
                writer.Write(data);

            App.AssetManager.RevertAsset(lfe);

            ChunkAssetEntry orig = App.AssetManager.GetChunkEntry(lfe.ChunkId);

            Guid guid = App.AssetManager.AddChunk(data, GenerateDeterministicGuid(lfe));
            foreach (LegacyFileEntry.ChunkCollectorInstance inst in lfe.CollectorInstances)
            {
                // add new chunk
                inst.ModifiedEntry = new LegacyFileEntry.ChunkCollectorInstance();
                ChunkAssetEntry assetChunkEntry = App.AssetManager.GetChunkEntry(guid);

                inst.ModifiedEntry.ChunkId = guid;
                inst.ModifiedEntry.Offset = 0;
                inst.ModifiedEntry.CompressedOffset = 0;
                inst.ModifiedEntry.Size = data.Length;
                inst.ModifiedEntry.CompressedSize = assetChunkEntry.ModifiedEntry.Data.Length;

                // @temp
                foreach (int sbId in orig.SuperBundles)
                {
                    assetChunkEntry.AddToSuperBundle(sbId);
                }
                assetChunkEntry.ModifiedEntry.UserData = "legacy;" + lfe.Name;

                // link to main ebx
                lfe.LinkAsset(assetChunkEntry);
                inst.Entry.LinkAsset(lfe);
            }

            lfe.IsDirty = true;
            ms.Dispose();
        }

        /* Obtain data from cache or directly from chunk */
        private Stream GetChunkStream(LegacyFileEntry lfe)
        {
            if (cacheMode)
            {
                if (!cachedChunks.ContainsKey(lfe.ChunkId))
                {
                    using (Stream chunkStream = App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(lfe.ChunkId)))
                    {
                        if (chunkStream == null)
                            return null;
                        cachedChunks.Add(lfe.ChunkId, ((MemoryStream)chunkStream).ToArray());
                    }
                }
                return new MemoryStream(cachedChunks[lfe.ChunkId]);
            }

            return App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(lfe.ChunkId));
        }

        public void OnCommand(string command, params object[] value)
        {
            switch (command)
            {
                case "SetCacheModeEnabled": SetCacheModeEnabled((bool)value[0]); break;
                case "FlushCache": FlushCache(); break;
            }
        }

        /// <summary>
        /// Will return a unique but determinstic guid based on the filename of the asset. This will
        /// ensure to minimize random conflicts between unassociated objects
        /// </summary>
        public static Guid GenerateDeterministicGuid(LegacyFileEntry lfe)
        {
            ulong nameHash = Murmur2.HashString64(lfe.Filename, 0x4864);
            ulong dirHash = Murmur2.HashString64(lfe.Path, 0x4864);
            int index = 1;

            Guid guid = Guid.Empty;
            do
            {
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    writer.Write(dirHash);
                    writer.Write(((nameHash ^ (ulong)index)));

                    byte[] b = writer.ToByteArray();
                    b[15] = 0x01;

                    guid = new Guid(b);
                }
                index++;
            }
            while (App.AssetManager.GetChunkEntry(guid) != null);

            return guid;
        }
    }
}
