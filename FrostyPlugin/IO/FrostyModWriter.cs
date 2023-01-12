using Frosty.Core.Handlers;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.IO
{
    public sealed class FrostyModWriter : NativeWriter
    {
        public class Manifest
        {
            public int Count => objects.Count;

            private Dictionary<Sha1, int> sha1Entries = new Dictionary<Sha1, int>();
            private List<object> objects = new List<object>();

            public Manifest()
            {
            }

            public int Add(byte[] data)
            {
                objects.Add(data);
                return objects.Count - 1;
            }

            public int Add(Sha1 sha1, byte[] data)
            {
                if (sha1Entries.ContainsKey(sha1))
                    return sha1Entries[sha1];

                objects.Add(data);
                sha1Entries.Add(sha1, objects.Count - 1);

                return objects.Count - 1;
            }

            public void Write(NativeWriter writer)
            {
                long dataOffset = writer.Position + (objects.Count * 16);
                long currentOffset = 0;

                foreach (object obj in objects)
                {
                    // write offset
                    writer.Write(currentOffset);

                    long dataSize = 0;
                    long pos = writer.Position;
                    writer.Position = dataOffset + currentOffset;

                    byte[] data = (byte[])obj;
                    writer.Write(data);
                    dataSize = data.Length;

                    writer.Position = pos;

                    // write size
                    writer.Write(dataSize);
                    currentOffset += dataSize;
                }
            }
        }

        private sealed class EmbeddedResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Embedded;
            public EmbeddedResource(string inName, byte[] data, Manifest manifest)
            {
                name = inName;
                if (data != null)
                {
                    resourceIndex = manifest.Add(data);
                    size = data.Length;
                }
            }
        }

        private sealed class BundleResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Bundle;
            private int superBundleName;

            public BundleResource(BundleEntry entry, Manifest manifest)
            {
                name = entry.Name.ToLower();
                superBundleName = Fnv1a.HashString(App.AssetManager.GetSuperBundle(entry.SuperBundleId).Name.ToLower());
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);
                writer.WriteNullTerminatedString(name);
                writer.Write(superBundleName);
            }
        }

        private sealed class EbxResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Ebx;
            public EbxResource(EbxAssetEntry entry, Manifest manifest)
                : base(entry)
            {
                CompressionType compressType = (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18) ? CompressionType.ZStd : CompressionType.Default;
                byte[] data = null;

                name = entry.Name.ToLower();
                if (entry.HasModifiedData)
                {
                    userData = entry.ModifiedEntry.UserData;
                    EbxWriteFlags flags = EbxWriteFlags.None;
                    if (ProfilesLibrary.EbxVersion == 6)
                    {
                        flags |= EbxWriteFlags.DoNotSort;
                    }

                    using (EbxBaseWriter ebxWriter = EbxBaseWriter.CreateWriter(new MemoryStream(), flags))
                    {
                        ebxWriter.WriteAsset(entry.ModifiedEntry.DataObject as EbxAsset);

                        size = ebxWriter.Length;
                        data = Utils.CompressFile(ebxWriter.ToByteArray(), compressionOverride: compressType);

                        resourceIndex = manifest.Add(data);
                        sha1 = Utils.GenerateSha1(data);
                    }
                }
            }
        }

        private sealed class ResResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Res;

            private uint resType;
            private ulong resRid;
            private byte[] resMeta;

            public ResResource(ResAssetEntry entry, Manifest manifest)
                : base(entry)
            {
                name = entry.Name.ToLower();
                if (entry.HasModifiedData)
                {
                    sha1 = entry.ModifiedEntry.Sha1;
                    resourceIndex = manifest.Add(entry.ModifiedEntry.Sha1, entry.ModifiedEntry.Data);
                    size = entry.ModifiedEntry.OriginalSize;

                    resType = entry.ResType;
                    resRid = entry.ResRid;
                    resMeta = (entry.ModifiedEntry.ResMeta != null) ? entry.ModifiedEntry.ResMeta : entry.ResMeta;
                    userData = entry.ModifiedEntry.UserData;

                    flags |= (byte)((entry.IsInline) ? 1 : 0);
                }
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);

                writer.Write(resType);
                writer.Write(resRid);
                writer.Write((resMeta != null) ? resMeta.Length : 0);
                if (resMeta != null)
                    writer.Write(resMeta);
            }
        }

        private sealed class ChunkResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Chunk;

            private uint rangeStart;
            private uint rangeEnd;
            private uint logicalOffset;
            private uint logicalSize;
            private int h32;
            private int firstMip;
            private List<int> superBundlesToAdd = new List<int>();

            public ChunkResource(ChunkAssetEntry entry, Manifest manifest)
                : base(entry)
            {
                name = entry.Id.ToString();
                superBundlesToAdd.AddRange(entry.AddedSuperBundles);
                if (entry.HasModifiedData)
                {
                    sha1 = entry.ModifiedEntry.Sha1;
                    resourceIndex = manifest.Add(entry.ModifiedEntry.Sha1, entry.ModifiedEntry.Data);
                    size = entry.ModifiedEntry.OriginalSize;

                    rangeStart = entry.ModifiedEntry.RangeStart;
                    rangeEnd = entry.ModifiedEntry.RangeEnd;
                    logicalOffset = entry.ModifiedEntry.LogicalOffset;
                    logicalSize = entry.ModifiedEntry.LogicalSize;
                    h32 = entry.ModifiedEntry.H32;
                    firstMip = entry.ModifiedEntry.FirstMip;
                    userData = entry.ModifiedEntry.UserData;

                    flags |= (byte)((entry.IsInline) ? 1 : 0);
                }
                else
                {
                    // special chunks bundle
                    if (App.FileSystemManager.GetManifestChunk(new Guid(name)) != null)
                    {
                        // not required?
                        flags |= 0x04;
                    }

                    h32 = entry.H32;
                    firstMip = entry.FirstMip;

                    // calculate RangeStart/End for texture chunks added to bundles, where they are not
                    // stored in the bundle (ie. Manifest layout)
                    if (firstMip != -1 && entry.LogicalOffset != 0 && entry.RangeStart == 0 && entry.RangeEnd == 0)
                    {
                        byte[] data = NativeReader.ReadInStream(App.AssetManager.GetRawStream(entry));

                        using (NativeReader reader = new NativeReader(new MemoryStream(data)))
                        {
                            long logicalOffset = entry.LogicalOffset;
                            uint size = 0;

                            while (true)
                            {
                                int decompressedSize = reader.ReadInt(Endian.Big);
                                ushort compressionType = reader.ReadUShort();
                                int bufferSize = reader.ReadUShort(Endian.Big);

                                int flags = ((compressionType & 0xFF00) >> 8);

                                if ((flags & 0x0F) != 0)
                                {
                                    bufferSize = ((flags & 0x0F) << 0x10) + bufferSize;
                                }
                                if ((decompressedSize & 0xFF000000) != 0)
                                {
                                    decompressedSize &= 0x00FFFFFF;
                                }

                                logicalOffset -= decompressedSize;
                                if (logicalOffset < 0)
                                {
                                    break;
                                }

                                compressionType = (ushort)(compressionType & 0x7F);
                                if (compressionType == 0x00)
                                {
                                    bufferSize = decompressedSize;
                                }

                                size += (uint)(bufferSize + 8);
                                reader.Position += bufferSize;
                            }

                            rangeStart = size;
                            rangeEnd = (uint)data.Length;
                        }
                    }
                }
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);

                writer.Write(rangeStart);
                writer.Write(rangeEnd);
                writer.Write(logicalOffset);
                writer.Write(logicalSize);
                writer.Write(h32);
                writer.Write(firstMip);
                writer.Write(superBundlesToAdd.Count);
                foreach (int sbId in superBundlesToAdd)
                {
                    writer.Write(sbId);
                }
            }
        }

        public Manifest ResourceManifest => manifest;

        private ModSettings overrideSettings;
        private Manifest manifest = new Manifest();
        private List<BaseModResource> resources = new List<BaseModResource>();

        public FrostyModWriter(Stream inStream, ModSettings inOverrideSettings = null)
            : base(inStream)
        {
            overrideSettings = inOverrideSettings;
        }

        public void WriteProject(FrostyProject project)
        {
            Write(FrostyMod.Magic);
            Write(FrostyMod.Version);
            Write(0xDEADBEEFDEADBEEF);
            Write(0xDEADBEEF);
            Write(ProfilesLibrary.ProfileName);
            Write(App.FileSystemManager.Head);

            ModSettings settings = overrideSettings ?? project.ModSettings;

            WriteNullTerminatedString(settings.Title);
            WriteNullTerminatedString(settings.Author);
            WriteNullTerminatedString(settings.Category);
            WriteNullTerminatedString(settings.Version);
            WriteNullTerminatedString(settings.Description);
            WriteNullTerminatedString(settings.Link);

            AddResource(new EmbeddedResource("Icon", settings.Icon, manifest));
            for (int i = 0; i < 4; i++)
                AddResource(new EmbeddedResource("Screenshot" + i.ToString(), settings.GetScreenshot(i), manifest));

            // @todo: superbundles

            // added bundles
            foreach (BundleEntry bentry in App.AssetManager.EnumerateBundles(modifiedOnly: true))
            {
                if (!bentry.Added)
                    continue;

                AddResource(new BundleResource(bentry, manifest));
            }

            // ebx
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(modifiedOnly: true))
            {
                if (entry.IsDirectlyModified)
                {
                    // ignore transient only edits (such as id fields)
                    if (entry.HasModifiedData && entry.ModifiedEntry.IsTransientModified)
                        continue;

                    if (entry.HasModifiedData)
                    {
                        ICustomActionHandler actionHandler = App.PluginManager.GetCustomHandler(entry.Type);
                        if (actionHandler != null && !entry.IsAdded)
                        {
                            // use custom action handler to save asset to mod
                            actionHandler.SaveToMod(this, entry);
                        }
                        else
                        {
                            // save as regular asset
                            AddResource(new EbxResource(entry, manifest));
                        }
                    }
                    else
                    {
                        AddResource(new EbxResource(entry, manifest));
                    }
                }
            }

            // res
            foreach (ResAssetEntry entry in App.AssetManager.EnumerateRes(modifiedOnly: true))
            {
                if (entry.IsDirectlyModified)
                {
                    if (entry.HasModifiedData)
                    {
                        ICustomActionHandler actionHandler = App.PluginManager.GetCustomHandler((ResourceType)entry.ResType);
                        if (actionHandler != null && !entry.IsAdded)
                        {
                            // use custom action handler to save resource to mod
                            actionHandler.SaveToMod(this, entry);
                        }
                        else
                        {
                            // save as regular resource
                            AddResource(new ResResource(entry, manifest));
                        }
                    }
                    else
                    {
                        AddResource(new ResResource(entry, manifest));
                    }
                }
            }

            // chunks
            foreach (ChunkAssetEntry entry in App.AssetManager.EnumerateChunks(modifiedOnly: true))
            {
                // chunks dont require custom handlers (except for the special case of Legacy). As chunks should never be modified
                // directly, but instead as part of an ebx or res modification

                if (entry.IsDirectlyModified)
                    AddResource(new ChunkResource(entry, manifest));
            }

            // legacy custom action handler
            ICustomAssetCustomActionHandler tmpHandler = new LegacyCustomActionHandler();
            tmpHandler.SaveToMod(this);

            // custom assets
            foreach (string type in App.PluginManager.CustomAssetHandlers)
            {
                ICustomAssetCustomActionHandler customHandler = App.PluginManager.GetCustomAssetHandler(type);
                customHandler.SaveToMod(this);
            }

            // write resources
            Write(resources.Count);
            foreach (EditorModResource resource in resources)
                resource.Write(this);

            // write manifest + data
            long manifestOffset = Position;
            manifest.Write(this);

            // finally update manifest offset in header
            Position = 12;
            Write(manifestOffset);
            Write(manifest.Count);
        }

        public void AddResource(BaseModResource resource)
        {
            resources.Add(resource);
        }
    }
}
