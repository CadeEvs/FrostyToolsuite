using Frosty.Core.IO;
using Frosty.Core.Legacy;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Handlers
{
    public sealed class LegacyCustomActionHandler : ICustomAssetCustomActionHandler
    {
        public HandlerUsage Usage => HandlerUsage.Merge;

        private class ModLegacyFileEntry
        {
            public int Hash { get; set; }
            public Guid ChunkId { get; set; }
            public long Offset { get; set; }
            public long CompressedOffset { get; set; }
            public long CompressedSize { get; set; }
            public long Size { get; set; }
        }

        public static uint Hash => 0xBD9BFB65;

        private class LegacyResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Chunk;
            public LegacyResource(string inName, string ebxName, byte[] data, IEnumerable<int> bundles, FrostyModWriter.Manifest manifest)
            {
                name = inName;
                sha1 = Utils.GenerateSha1(data);
                resourceIndex = manifest.Add(data);
                size = data.Length;
                handlerHash = (int)Hash;
                userData = "legacy;Collector (" + ebxName + ")";
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);

                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
            }
        }

        public void SaveToMod(FrostyModWriter writer)
        {
            Dictionary<EbxAssetEntry, List<Tuple<int, LegacyFileEntry.ChunkCollectorInstance>>> manifests = new Dictionary<EbxAssetEntry, List<Tuple<int, LegacyFileEntry.ChunkCollectorInstance>>>();
            foreach (LegacyFileEntry lfe in App.AssetManager.EnumerateCustomAssets("legacy", modifiedOnly: true))
            {
                foreach (LegacyFileEntry.ChunkCollectorInstance inst in lfe.CollectorInstances)
                {
                    if (!manifests.ContainsKey(inst.Entry))
                        manifests.Add(inst.Entry, new List<Tuple<int, LegacyFileEntry.ChunkCollectorInstance>>());
                    manifests[inst.Entry].Add(new Tuple<int, LegacyFileEntry.ChunkCollectorInstance>(lfe.NameHash, inst.ModifiedEntry));
                }
            }

            foreach (EbxAssetEntry entry in manifests.Keys)
            {
                dynamic obj = App.AssetManager.GetEbx(entry).RootObject;
                dynamic manifest = obj.Manifest;

                ChunkAssetEntry collectorChunkEntry = App.AssetManager.GetChunkEntry(manifest.ChunkId);

                MemoryStream ms = new MemoryStream();
                using (NativeWriter chunkWriter = new NativeWriter(ms))
                {
                    foreach (Tuple<int, LegacyFileEntry.ChunkCollectorInstance> inst in manifests[entry])
                    {
                        chunkWriter.Write(inst.Item1);
                        chunkWriter.Write(inst.Item2.ChunkId);
                        chunkWriter.Write(inst.Item2.Offset);
                        chunkWriter.Write(inst.Item2.CompressedOffset);
                        chunkWriter.Write(inst.Item2.CompressedSize);
                        chunkWriter.Write(inst.Item2.Size);
                    }

                    writer.AddResource(new LegacyResource(collectorChunkEntry.Name, entry.Name, ms.ToArray(), collectorChunkEntry.EnumerateBundles(), writer.ResourceManifest));
                }
            }
        }

        public bool SaveToProject(NativeWriter writer)
        {
            writer.WriteNullTerminatedString("legacy");

            long sizePosition = writer.Position;
            writer.Write(0xDEADBEEF);

            int count = 0;
            foreach (LegacyFileEntry lfe in App.AssetManager.EnumerateCustomAssets("legacy", modifiedOnly: true))
            {
                LegacyFileEntry.ChunkCollectorInstance inst = lfe.CollectorInstances[0].ModifiedEntry;
                writer.WriteNullTerminatedString(lfe.Name);
                FrostyProject.SaveLinkedAssets(lfe, writer);

                writer.Write(lfe.ChunkId);
                writer.Write(inst.Offset);
                writer.Write(inst.CompressedOffset);
                writer.Write(inst.CompressedSize);
                writer.Write(inst.Size);

                count++;
            }

            writer.Position = sizePosition;
            writer.Write(count);
            writer.Position = writer.Length;
            return true;
        }

        public void LoadFromProject(DbObject project)
        {
            uint version = project.GetValue<uint>("version");
            DbObject modifiedObj = project.GetValue<DbObject>("modified");

            if (!modifiedObj.HasValue("legacy"))
                return;

            foreach (DbObject legacyObj in modifiedObj.GetValue<DbObject>("legacy"))
            {
                LegacyFileEntry entry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", legacyObj.GetValue<string>("name"));
                if (entry != null)
                {
                    FrostyProject.LoadLinkedAssets(legacyObj, entry, version);
                    foreach (LegacyFileEntry.ChunkCollectorInstance inst in entry.CollectorInstances)
                    {
                        inst.ModifiedEntry = new LegacyFileEntry.ChunkCollectorInstance
                        {
                            ChunkId = legacyObj.GetValue<Guid>("chunkId"),
                            Offset = legacyObj.GetValue<long>("offset"),
                            CompressedOffset = legacyObj.GetValue<long>("compressedOffset"),
                            CompressedSize = legacyObj.GetValue<long>("compressedSize"),
                            Size = legacyObj.GetValue<long>("size")
                        };
                    }
                }
            }
        }

        public void LoadFromProject(uint version, NativeReader reader, string type)
        {
            if (type != "legacy")
                return;

            int numItems = reader.ReadInt();
            for (int i = 0; i < numItems; i++)
            {
                string name = reader.ReadNullTerminatedString();
                List<AssetEntry> linkedEntries = FrostyProject.LoadLinkedAssets(reader);
                Guid chunkId = reader.ReadGuid();
                long offset = reader.ReadLong();
                long compressedOffset = reader.ReadLong();
                long compressedSize = reader.ReadLong();
                long size = reader.ReadLong();

                LegacyFileEntry entry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", name);

                if (version < 12)
                {
                    // retroactively change guid to a determinstic guid
                    ChunkAssetEntry oldEntry = App.AssetManager.GetChunkEntry(chunkId);
                    Stream stream = App.AssetManager.GetChunk(oldEntry);

                    chunkId = LegacyFileManager.GenerateDeterministicGuid(entry);

                    // remove old chunk
                    App.AssetManager.RevertAsset(oldEntry);
                    App.AssetManager.AddChunk(NativeReader.ReadInStream(stream), chunkId);

                    // and add new chunk
                    ChunkAssetEntry newEntry = App.AssetManager.GetChunkEntry(chunkId);
                    newEntry.IsDirty = false;
                    newEntry.ModifiedEntry.UserData = "legacy;" + entry.Name;
                    newEntry.AddedSuperBundles.AddRange(oldEntry.SuperBundles);

                    linkedEntries.Clear();
                    entry.LinkAsset(newEntry);
                }

                if (entry != null)
                {
                    entry.LinkedAssets.AddRange(linkedEntries);
                    foreach (LegacyFileEntry.ChunkCollectorInstance inst in entry.CollectorInstances)
                    {
                        inst.ModifiedEntry = new LegacyFileEntry.ChunkCollectorInstance
                        {
                            ChunkId = chunkId,
                            Offset = offset,
                            CompressedOffset = compressedOffset,
                            CompressedSize = compressedSize,
                            Size = size
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Handles the loading and merging of the custom data
        /// </summary>
        public object Load(object existing, byte[] newData)
        {
            List<ModLegacyFileEntry> entries = (List<ModLegacyFileEntry>)existing ?? new List<ModLegacyFileEntry>();

            using (NativeReader reader = new NativeReader(new MemoryStream(newData)))
            {
                while (reader.Position < reader.Length)
                {
                    int hash = reader.ReadInt();

                    int idx = entries.FindIndex((ModLegacyFileEntry a) => a.Hash == hash);
                    if (idx != -1)
                        entries.RemoveAt(idx);

                    ModLegacyFileEntry newEntry = new ModLegacyFileEntry
                    {
                        Hash = hash,
                        ChunkId = reader.ReadGuid(),
                        Offset = reader.ReadLong(),
                        CompressedOffset = reader.ReadLong(),
                        CompressedSize = reader.ReadLong(),
                        Size = reader.ReadLong()
                    };
                    entries.Add(newEntry);
                }
            }
            return entries;
        }

        /// <summary>
        /// Handles the actual modification of the base data with the custom data
        /// </summary>
        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            ChunkAssetEntry chunkEntry = origEntry as ChunkAssetEntry;
            List<ModLegacyFileEntry> entries = (List<ModLegacyFileEntry>)data;
            using (NativeReader reader = new NativeReader(am.GetChunk(am.GetChunkEntry(chunkEntry.Id))))
            {
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                        ProfileVersion.Fifa18,
                        ProfileVersion.Madden19,
                        ProfileVersion.Fifa19,
                        ProfileVersion.Madden20,
                        ProfileVersion.Fifa20,
                        ProfileVersion.PlantsVsZombiesBattleforNeighborville))
                    {
                        WriteV1(writer, reader, entries);
                    }
                    else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21, ProfileVersion.Madden22, ProfileVersion.Fifa22,
                        ProfileVersion.Madden23, ProfileVersion.Fifa23))
                    {
                        WriteV2(writer, reader, entries);
                    }

                    outData = Utils.CompressFile(writer.ToByteArray());

                    chunkEntry.Sha1 = Utils.GenerateSha1(outData);
                    chunkEntry.Size = outData.Length;
                }
            }
        }

        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {
            return new List<string>();
        }

        private void WriteV1(NativeWriter writer, NativeReader reader, List<ModLegacyFileEntry> entries)
        {
            int numEntries = reader.ReadInt();
            writer.Write(numEntries);
            long dataOffset = reader.ReadLong();
            writer.Write(dataOffset);
            writer.Write(reader.ReadBytes((int)(dataOffset - 12)));

            for (int i = 0; i < numEntries; i++)
            {
                long strOffset = reader.ReadLong();
                long curPos = reader.Position;
                reader.Position = strOffset;
                string str = reader.ReadNullTerminatedString();
                int hash = Fnv1.HashString(str);
                reader.Position = curPos;

                int idx = entries.FindIndex((ModLegacyFileEntry a) => a.Hash == hash);
                if (idx != -1)
                {
                    ModLegacyFileEntry inst = entries[idx];
                    writer.Write(strOffset);
                    writer.Write(inst.CompressedOffset);
                    writer.Write(inst.CompressedSize);
                    writer.Write(inst.Offset);
                    writer.Write(inst.Size);
                    writer.Write(inst.ChunkId);
                    reader.Position += 0x30;
                }
                else
                {
                    writer.Write(strOffset);
                    writer.Write(reader.ReadBytes(0x30));
                }
            }
        }

        private void WriteV2(NativeWriter writer, NativeReader reader, List<ModLegacyFileEntry> entries)
        {
            writer.Write(reader.ReadInt());
            writer.Write(reader.ReadLong());

            int numEntries = reader.ReadInt();
            writer.Write(numEntries);
            long dataOffset = reader.ReadLong();
            writer.Write(dataOffset);

            writer.Write(reader.ReadInt());
            writer.Write(reader.ReadLong());

            int numChunks = reader.ReadInt();
            writer.Write(numChunks + entries.Count);
            long chunkOffset = reader.ReadLong();
            writer.Write(chunkOffset);

            writer.Write(reader.ReadBytes((int)(dataOffset - reader.Position)));

            for (int i = 0; i < numEntries; i++)
            {
                long strOffset = reader.ReadLong();
                long curPos = reader.Position;
                reader.Position = strOffset;
                string str = reader.ReadNullTerminatedString();
                int hash = Fnv1.HashString(str);
                reader.Position = curPos;

                int idx = entries.FindIndex((ModLegacyFileEntry a) => a.Hash == hash);
                if (idx != -1)
                {
                    ModLegacyFileEntry inst = entries[idx];
                    writer.Write(strOffset);
                    writer.Write(inst.CompressedOffset);
                    writer.Write(inst.CompressedSize);
                    writer.Write(inst.Offset);
                    writer.Write(inst.Size);
                    writer.Write(inst.ChunkId);
                    reader.Position += 0x30;
                }
                else
                {
                    writer.Write(strOffset);
                    writer.Write(reader.ReadBytes(0x30));
                }
            }

            //writer.Write(reader.ReadBytes((int)(chunkOffset - reader.Position)));

            //for (int i = 0; i < numChunks; i++)
            //{
            //    writer.Write(reader.ReadLong());
            //    writer.Write(reader.ReadGuid());
            //}
            //foreach (var entry in entries)
            //{
            //    writer.Write(entry.Size);
            //    writer.Write(entry.ChunkId);
            //}
            writer.Write(reader.ReadToEnd());
        }
    }
}
