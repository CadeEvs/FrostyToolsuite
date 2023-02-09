using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using Frosty.Hash;
using FrostySdk.Resources;
using Frosty.Core.Handlers;
using Frosty.Core.Mod;
using Frosty.Core.IO;
using Frosty.Controls;
using Frosty.Core.Windows;
using System.Windows;
using FrostySdk.Managers.Entries;

namespace Frosty.Core
{
    public sealed class FrostyProject
    {
        /*
          Project Versions:
            1 - Initial Version
            2 - (Unknown)
            3 - Assets can store multiple linked assets of CHUNK and RES type
            4 - (Unknown)
            5 - (Unknown)
            6 - New texture streaming changes (retroactively fixes old textures)
              - Stores mod details
            7 - Ebx now stored as objects rather than compressed byte streams
            8 - Chunk H32 now stored (retroactively calculate h32 for old projects)
            9 - Changed to a binary format, and added custom action handlers
            10 - TODO
            11 - Merging of defined res files (eg. ShaderBlockDepot)
            12 - Legacy files now use determinstic guids and added user data (retroactively fix old legacy files)
            13 - Merging of defined ebx files
            14 - Can duplicate blueprint bundles
            15 - Adds superbundle ids for toc chunks
        */

#if FROSTY_DEVELOPER_ADDTOBUNDLE
        private const uint FormatVersion = 14;
#else
        private const uint FormatVersion = 15;
#endif
        private const ulong Magic = 0x00005954534F5246;

        public string DisplayName
        {
            get
            {
                if (filename == "")
                    return "New Project.fbproject";

                FileInfo fi = new FileInfo(filename);
                return fi.Name;
            }
        }
        public string Filename 
        { 
            get => filename;
            set => filename = value;
        }
        public string Profile
        {
            get => gameProfile;
            set => gameProfile = value;
        }
        public bool RequiresNewProfile
        {
            get => requiresNewProfile;
            set => requiresNewProfile = value;
        }
        public bool IsDirty => App.AssetManager != null ? App.AssetManager.GetDirtyCount() != 0 || modSettings.IsDirty : false;

        public ModSettings ModSettings => modSettings;

        private string filename;
        private DateTime creationDate;
        private DateTime modifiedDate;
        public uint gameVersion;

        private string gameProfile;
        private bool requiresNewProfile;

        // mod export settings
        private ModSettings modSettings;

        public FrostyProject()
        {
            filename = "";
            creationDate = DateTime.Now;
            modifiedDate = DateTime.Now;
            gameVersion = 0;
            modSettings = new ModSettings { Author = Config.Get("ModAuthor", "") };
            modSettings.ClearDirtyFlag();
        }

        public bool Load(string inFilename)
        {
            filename = inFilename;

            ulong magic = 0;
            using (NativeReader reader = new NativeReader(new FileStream(inFilename, FileMode.Open, FileAccess.Read)))
            {
                magic = reader.ReadULong();
                if (magic == Magic)
                    return InternalLoad(reader);
            }
            return LegacyLoad(inFilename);
        }

        public void Save(string overrideFilename = "", bool updateDirtyState = true)
        {
            string actualFilename = filename;
            if (!string.IsNullOrEmpty(overrideFilename))
                actualFilename = overrideFilename;

            modifiedDate = DateTime.Now;
            gameVersion = App.FileSystemManager.Head;

            FileInfo fi = new FileInfo(actualFilename);
            if (!fi.Directory.Exists)
            {
                // create output directory
                Directory.CreateDirectory(fi.DirectoryName);
            }

            // save to temporary file first
            string tempFilename = fi.FullName + ".tmp";

            using (NativeWriter writer = new NativeWriter(new FileStream(tempFilename, FileMode.Create)))
            {
                writer.Write(Magic);
                writer.Write(FormatVersion);
                writer.WriteNullTerminatedString(ProfilesLibrary.ProfileName);
                writer.Write(creationDate.Ticks);
                writer.Write(modifiedDate.Ticks);
                writer.Write(gameVersion);

                writer.WriteNullTerminatedString(modSettings.Title);
                writer.WriteNullTerminatedString(modSettings.Author);
                writer.WriteNullTerminatedString(modSettings.Category);
                writer.WriteNullTerminatedString(modSettings.Version);
                writer.WriteNullTerminatedString(modSettings.Description);

                if (modSettings.Icon != null && modSettings.Icon.Length != 0)
                {
                    writer.Write(modSettings.Icon.Length);
                    writer.Write(modSettings.Icon);
                }
                else
                {
                    writer.Write(0);
                }

                for (int i = 0; i < 4; i++)
                {
                    byte[] buf = modSettings.GetScreenshot(i);
                    if (buf != null && buf.Length != 0)
                    {
                        writer.Write(buf.Length);
                        writer.Write(buf);
                    }
                    else
                    {
                        writer.Write(0);
                    }
                }

                // -----------------------------------------------------------------------------
                // added data
                // -----------------------------------------------------------------------------

                // @todo: superbundles
                writer.Write(0);

                // bundles
                long sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                int count = 0;
                foreach (BundleEntry entry in App.AssetManager.EnumerateBundles(modifiedOnly: true))
                {
                    if (entry.Added)
                    {
                        writer.WriteNullTerminatedString(entry.Name);
                        writer.WriteNullTerminatedString(App.AssetManager.GetSuperBundle(entry.SuperBundleId).Name);
                        writer.Write((int)entry.Type);
                        count++;
                    }
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // ebx
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 0;
                foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(modifiedOnly: true))
                {
                    if (entry.IsAdded)
                    {
                        writer.WriteNullTerminatedString(entry.Name);
                        writer.Write(entry.Guid);
                        count++;
                    }
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // res
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 0;
                foreach (ResAssetEntry entry in App.AssetManager.EnumerateRes(modifiedOnly: true))
                {
                    if (entry.IsAdded)
                    {
                        writer.WriteNullTerminatedString(entry.Name);
                        writer.Write(entry.ResRid);
                        writer.Write(entry.ResType);
                        writer.Write(entry.ResMeta);
                        count++;
                    }
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // chunks
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 0;
                foreach (ChunkAssetEntry entry in App.AssetManager.EnumerateChunks(modifiedOnly: true))
                {
                    if (entry.IsAdded)
                    {
                        writer.Write(entry.Id);
                        writer.Write(entry.H32);
                        count++;
                    }
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // -----------------------------------------------------------------------------
                // modified data
                // -----------------------------------------------------------------------------

                // ebx
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 0;
                foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(modifiedOnly: true, includeLinked: true))
                {
                    writer.WriteNullTerminatedString(entry.Name);
                    SaveLinkedAssets(entry, writer);

                    // bundles the asset has been added to
                    writer.Write(entry.AddedBundles.Count);
                    foreach (int bid in entry.AddedBundles)
                        writer.WriteNullTerminatedString(App.AssetManager.GetBundleEntry(bid).Name);

                    // if the asset has been modified
                    writer.Write(entry.HasModifiedData);
                    if (entry.HasModifiedData)
                    {
                        // mark asset as only transient modified
                        writer.Write(entry.ModifiedEntry.IsTransientModified);
                        writer.WriteNullTerminatedString(entry.ModifiedEntry.UserData);

                        ModifiedResource modifiedResource = entry.ModifiedEntry.DataObject as ModifiedResource;
                        byte[] buf = null;
                        bool bCustomHandler = modifiedResource != null;

                        if (bCustomHandler)
                        {
                            // asset is using a custom handler
                            buf = modifiedResource.Save();
                        }
                        else
                        {
                            // asset is using just regular data
                            EbxAsset asset = entry.ModifiedEntry.DataObject as EbxAsset;
                            using (EbxBaseWriter ebxWriter = EbxBaseWriter.CreateProjectWriter(new MemoryStream(), EbxWriteFlags.IncludeTransient))
                            {
                                ebxWriter.WriteAsset(asset);
                                buf = ebxWriter.ToByteArray();
                            }
                        }

                        writer.Write(bCustomHandler);
                        writer.Write(buf.Length);
                        writer.Write(buf);
                    }

                    if (updateDirtyState)
                        entry.IsDirty = false;

                    count++;
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // res
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 0;
                foreach (ResAssetEntry entry in App.AssetManager.EnumerateRes(modifiedOnly: true))
                {
                    writer.WriteNullTerminatedString(entry.Name);
                    SaveLinkedAssets(entry, writer);

                    // bundles the asset has been added to
                    writer.Write(entry.AddedBundles.Count);
                    foreach (int bid in entry.AddedBundles)
                        writer.WriteNullTerminatedString(App.AssetManager.GetBundleEntry(bid).Name);

                    // if the asset has been modified
                    writer.Write(entry.HasModifiedData);
                    if (entry.HasModifiedData)
                    {
                        writer.Write(entry.ModifiedEntry.Sha1);
                        writer.Write(entry.ModifiedEntry.OriginalSize);
                        if (entry.ModifiedEntry.ResMeta != null)
                        {
                            writer.Write(entry.ModifiedEntry.ResMeta.Length);
                            writer.Write(entry.ModifiedEntry.ResMeta);
                        }
                        else
                        {
                            // no res meta
                            writer.Write(0);
                        }
                        writer.WriteNullTerminatedString(entry.ModifiedEntry.UserData);

                        byte[] buffer = entry.ModifiedEntry.Data;
                        if (entry.ModifiedEntry.DataObject != null)
                        {
                            ModifiedResource md = entry.ModifiedEntry.DataObject as ModifiedResource;
                            buffer = md.Save();
                        }

                        writer.Write(buffer.Length);
                        writer.Write(buffer);
                    }

                    if (updateDirtyState)
                        entry.IsDirty = false;

                    count++;
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // chunks
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 0;
                foreach (ChunkAssetEntry entry in App.AssetManager.EnumerateChunks(modifiedOnly: true))
                {
                    writer.Write(entry.Id);

                    // bundles the asset has been added to
                    writer.Write(entry.AddedBundles.Count);
                    foreach (int bid in entry.AddedBundles)
                        writer.WriteNullTerminatedString(App.AssetManager.GetBundleEntry(bid).Name);

                    // superbundles the asset has been added to
                    writer.Write(entry.AddedSuperBundles.Count);
                    foreach (int sbid in entry.AddedSuperBundles)
                        writer.WriteNullTerminatedString(App.AssetManager.GetSuperBundle(sbid).Name);

                    // if the asset has been modified
                    writer.Write(entry.HasModifiedData);
                    if (entry.HasModifiedData)
                    {
                        writer.Write(entry.ModifiedEntry.Sha1);
                        writer.Write(entry.ModifiedEntry.LogicalOffset);
                        writer.Write(entry.ModifiedEntry.LogicalSize);
                        writer.Write(entry.ModifiedEntry.RangeStart);
                        writer.Write(entry.ModifiedEntry.RangeEnd);
                        writer.Write(entry.ModifiedEntry.FirstMip);
                        writer.Write(entry.ModifiedEntry.H32);
                        writer.Write(entry.ModifiedEntry.AddToChunkBundle);
                        writer.WriteNullTerminatedString(entry.ModifiedEntry.UserData);

                        writer.Write(entry.ModifiedEntry.Data.Length);
                        writer.Write(entry.ModifiedEntry.Data);
                    }

                    if (updateDirtyState)
                        entry.IsDirty = false;

                    count++;
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                // custom actions
                sizePosition = writer.Position;
                writer.Write(0xDEADBEEF);

                count = 1;
                ICustomAssetCustomActionHandler legacyHandler = new LegacyCustomActionHandler();
                legacyHandler.SaveToProject(writer);

                foreach (string type in App.PluginManager.CustomAssetHandlers)
                {
                    ICustomAssetCustomActionHandler customHandler = App.PluginManager.GetCustomAssetHandler(type);
                    customHandler.SaveToProject(writer);
                    count++;
                }

                writer.Position = sizePosition;
                writer.Write(count);
                writer.Position = writer.Length;

                if (updateDirtyState)
                    modSettings.ClearDirtyFlag();
            }

            if (File.Exists(tempFilename))
            {
                bool isValid = false;

                // check project file to ensure it saved correctly
                using (FileStream fs = new FileStream(tempFilename, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length > 0)
                    {
                        isValid = true;
                    }
                }

                if (isValid)
                {
                    // replace existing project
                    File.Delete(fi.FullName);
                    File.Move(tempFilename, fi.FullName);
                }
            }
        }

        public ModSettings GetModSettings()
        {
            return modSettings;
        }

        public void WriteToMod(string filename, ModSettings overrideSettings)
        {
            using (FrostyModWriter writer = new FrostyModWriter(new FileStream(filename, FileMode.Create), overrideSettings))
                writer.WriteProject(this);
        }

        public static void SaveLinkedAssets(AssetEntry entry, NativeWriter writer)
        {
            writer.Write(entry.LinkedAssets.Count);
            foreach (AssetEntry linkedEntry in entry.LinkedAssets)
            {
                writer.WriteNullTerminatedString(linkedEntry.AssetType);
                if (linkedEntry is ChunkAssetEntry assetEntry)
                    writer.Write(assetEntry.Id);
                else
                    writer.WriteNullTerminatedString(linkedEntry.Name);
            }
        }

        public static List<AssetEntry> LoadLinkedAssets(NativeReader reader)
        {
            int numItems = reader.ReadInt();
            List<AssetEntry> linkedEntries = new List<AssetEntry>();

            for (int i = 0; i < numItems; i++)
            {
                string type = reader.ReadNullTerminatedString();
                if (type == "ebx")
                {
                    string name = reader.ReadNullTerminatedString();
                    EbxAssetEntry ebxEntry = App.AssetManager.GetEbxEntry(name);
                    if (ebxEntry != null)
                        linkedEntries.Add(ebxEntry);
                }
                else if (type == "res")
                {
                    string name = reader.ReadNullTerminatedString();
                    ResAssetEntry resEntry = App.AssetManager.GetResEntry(name);
                    if (resEntry != null)
                        linkedEntries.Add(resEntry);
                }
                else if (type == "chunk")
                {
                    Guid id = reader.ReadGuid();
                    ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(id);
                    if (chunkEntry != null)
                        linkedEntries.Add(chunkEntry);
                }
                else
                {
                    string name = reader.ReadNullTerminatedString();
                    AssetEntry customEntry = App.AssetManager.GetCustomAssetEntry(type, name);
                    if (customEntry != null)
                        linkedEntries.Add(customEntry);
                }
            }

            return linkedEntries;
        }

        public static void LoadLinkedAssets(DbObject asset, AssetEntry entry, uint version)
        {
            if (version == 2)
            {
                // old projects can only store one linked asset
                string linkedAssetType = asset.GetValue<string>("linkedAssetType");
                if (linkedAssetType == "res")
                {
                    string name = asset.GetValue<string>("linkedAssetId");
                    entry.LinkedAssets.Add(App.AssetManager.GetResEntry(name));
                }
                else if (linkedAssetType == "chunk")
                {
                    Guid id = asset.GetValue<Guid>("linkedAssetId");
                    entry.LinkedAssets.Add(App.AssetManager.GetChunkEntry(id));
                }
            }
            else
            {
                foreach (DbObject linkedAsset in asset.GetValue<DbObject>("linkedAssets"))
                {
                    string type = linkedAsset.GetValue<string>("type");
                    if (type == "ebx")
                    {
                        string name = linkedAsset.GetValue<string>("id");
                        EbxAssetEntry ebxEntry = App.AssetManager.GetEbxEntry(name);
                        if (ebxEntry != null)
                            entry.LinkedAssets.Add(ebxEntry);
                    }
                    else if (type == "res")
                    {
                        string name = linkedAsset.GetValue<string>("id");
                        ResAssetEntry resEntry = App.AssetManager.GetResEntry(name);
                        if (resEntry != null)
                            entry.LinkedAssets.Add(resEntry);
                    }
                    else if (type == "chunk")
                    {
                        Guid id = linkedAsset.GetValue<Guid>("id");
                        ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(id);
                        if (chunkEntry != null)
                            entry.LinkedAssets.Add(chunkEntry);
                    }
                    else
                    {
                        string name = linkedAsset.GetValue<string>("id");
                        AssetEntry customEntry = App.AssetManager.GetCustomAssetEntry(type, name);
                        if (customEntry != null)
                            entry.LinkedAssets.Add(customEntry);
                    }
                }
            }
        }

        private bool InternalLoad(NativeReader reader)
        {
            uint version = reader.ReadUInt();
            if (version > FormatVersion)
                return false;

            // 9 is the first version to support the new format, any earlier versions stored
            // here are invalid

            if (version < 9)
                return false;

            gameProfile = reader.ReadNullTerminatedString();
            // load profile if one isn't already loaded
            if (!ProfilesLibrary.HasLoadedProfile)
            {
                App.ClearProfileData();

                requiresNewProfile = true;
                App.LoadProfile(gameProfile);
            }
            else
            {
                // only load project if it's using the same profile as the one loaded
                if (gameProfile.ToLower() != ProfilesLibrary.ProfileName.ToLower())
                    return false;
                
                // if it is the same project, reset and revert all of the current assets
                App.AssetManager.Reset();
            }

            FrostyTaskWindow.Show("Loading Project", "", (task) =>
            {
                task.Update(filename);

                Dictionary<int, AssetEntry> h32map = new Dictionary<int, AssetEntry>();

                creationDate = new DateTime(reader.ReadLong());
                modifiedDate = new DateTime(reader.ReadLong());
                gameVersion = reader.ReadUInt();

                modSettings.Title = reader.ReadNullTerminatedString();
                modSettings.Author = reader.ReadNullTerminatedString();
                modSettings.Category = reader.ReadNullTerminatedString();
                modSettings.Version = reader.ReadNullTerminatedString();
                modSettings.Description = reader.ReadNullTerminatedString();

                int size = reader.ReadInt();
                if (size > 0)
                    modSettings.Icon = reader.ReadBytes(size);

                for (int i = 0; i < 4; i++)
                {
                    size = reader.ReadInt();
                    if (size > 0)
                        modSettings.SetScreenshot(i, reader.ReadBytes(size));
                }

                modSettings.ClearDirtyFlag();

                // -----------------------------------------------------------------------------
                // added data
                // -----------------------------------------------------------------------------

                // superbundles
                int numItems = reader.ReadInt();

                // bundles
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    string name = reader.ReadNullTerminatedString();
                    string sbName = reader.ReadNullTerminatedString();
                    BundleType type = (BundleType)reader.ReadInt();

                    App.AssetManager.AddBundle(name, type, App.AssetManager.GetSuperBundleId(sbName));
                }

                // ebx
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    EbxAssetEntry entry = new EbxAssetEntry
                    {
                        Name = reader.ReadNullTerminatedString(),
                        Guid = reader.ReadGuid()
                    };
                    App.AssetManager.AddEbx(entry);
                }

                // res
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    ResAssetEntry entry = new ResAssetEntry
                    {
                        Name = reader.ReadNullTerminatedString(),
                        ResRid = reader.ReadULong(),
                        ResType = reader.ReadUInt(),
                        ResMeta = reader.ReadBytes(0x10)
                    };
                    App.AssetManager.AddRes(entry);
                }

                // chunks
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    ChunkAssetEntry newEntry = new ChunkAssetEntry
                    {
                        Id = reader.ReadGuid(),
                        H32 = reader.ReadInt()
                    };
                    App.AssetManager.AddChunk(newEntry);
                }

                // -----------------------------------------------------------------------------
                // modified data
                // -----------------------------------------------------------------------------

                // ebx
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    string name = reader.ReadNullTerminatedString();
                    List<AssetEntry> linkedEntries = LoadLinkedAssets(reader);
                    List<int> bundles = new List<int>();

                    if (version >= 13)
                    {
                        int length = reader.ReadInt();
                        for (int j = 0; j < length; j++)
                        {
                            string bundleName = reader.ReadNullTerminatedString();
                            int bid = App.AssetManager.GetBundleId(bundleName);
                            if (bid != -1)
                                bundles.Add(bid);
                        }
                    }

                    bool isModified = reader.ReadBoolean();

                    bool isTransientModified = false;
                    string userData = "";
                    byte[] data = null;
                    bool modifiedResource = false;

                    if (isModified)
                    {
                        isTransientModified = reader.ReadBoolean();
                        if (version >= 12)
                            userData = reader.ReadNullTerminatedString();

                        if (version < 13)
                        {
                            int length = reader.ReadInt();
                            for (int j = 0; j < length; j++)
                            {
                                string bundleName = reader.ReadNullTerminatedString();
                                int bid = App.AssetManager.GetBundleId(bundleName);
                                if (bid != -1)
                                    bundles.Add(bid);
                            }
                        }

                        if (version >= 13)
                            modifiedResource = reader.ReadBoolean();
                        data = reader.ReadBytes(reader.ReadInt());
                    }

                    EbxAssetEntry entry = App.AssetManager.GetEbxEntry(name);
                    if (entry != null)
                    {
                        entry.LinkedAssets.AddRange(linkedEntries);
                        entry.AddedBundles.AddRange(bundles);

                        if (isModified)
                        {
                            entry.ModifiedEntry = new ModifiedAssetEntry
                            {
                                IsTransientModified = isTransientModified,
                                UserData = userData
                            };

                            if (modifiedResource)
                            {
                                // store as modified resource data object
                                entry.ModifiedEntry.DataObject = ModifiedResource.Read(data);
                            }
                            else
                            {
                                if (!entry.IsAdded && App.PluginManager.GetCustomHandler(entry.Type) != null)
                                {
                                    // @todo: throw some kind of error
                                }

                                // store as a regular ebx
                                using (EbxReader ebxReader = EbxReader.CreateProjectReader(new MemoryStream(data)))
                                {
                                    EbxAsset asset = ebxReader.ReadAsset<EbxAsset>();
                                    entry.ModifiedEntry.DataObject = asset;

                                    if (entry.IsAdded)
                                        entry.Type = asset.RootObject.GetType().Name;
                                    entry.ModifiedEntry.DependentAssets.AddRange(asset.Dependencies);
                                }
                            }

                            entry.OnModified();
                        }

                        int hash = Fnv1.HashString(entry.Name);
                        if (!h32map.ContainsKey(hash))
                            h32map.Add(hash, entry);
                    }
                }

                // res
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    string name = reader.ReadNullTerminatedString();
                    List<AssetEntry> linkedEntries = LoadLinkedAssets(reader);
                    List<int> bundles = new List<int>();

                    if (version >= 13)
                    {
                        int length = reader.ReadInt();
                        for (int j = 0; j < length; j++)
                        {
                            string bundleName = reader.ReadNullTerminatedString();
                            int bid = App.AssetManager.GetBundleId(bundleName);
                            if (bid != -1)
                                bundles.Add(bid);
                        }
                    }

                    bool isModified = reader.ReadBoolean();

                    Sha1 sha1 = Sha1.Zero;
                    long originalSize = 0;
                    byte[] resMeta = null;
                    byte[] data = null;
                    string userData = "";

                    if (isModified)
                    {
                        sha1 = reader.ReadSha1();
                        originalSize = reader.ReadLong();

                        int length = reader.ReadInt();
                        if (length > 0)
                            resMeta = reader.ReadBytes(length);

                        if (version >= 12)
                            userData = reader.ReadNullTerminatedString();

                        if (version < 13)
                        {
                            length = reader.ReadInt();
                            for (int j = 0; j < length; j++)
                            {
                                string bundleName = reader.ReadNullTerminatedString();
                                int bid = App.AssetManager.GetBundleId(bundleName);
                                if (bid != -1)
                                    bundles.Add(bid);
                            }
                        }

                        data = reader.ReadBytes(reader.ReadInt());
                    }

                    ResAssetEntry entry = App.AssetManager.GetResEntry(name);
                    if (entry != null)
                    {
                        entry.LinkedAssets.AddRange(linkedEntries);
                        entry.AddedBundles.AddRange(bundles);

                        if (isModified)
                        {
                            entry.ModifiedEntry = new ModifiedAssetEntry
                            {
                                Sha1 = sha1,
                                OriginalSize = originalSize,
                                ResMeta = resMeta,
                                UserData = userData
                            };

                            if (sha1 == Sha1.Zero)
                            {
                                // store as modified resource data object
                                entry.ModifiedEntry.DataObject = ModifiedResource.Read(data);
                            }
                            else
                            {
                                if (!entry.IsAdded && App.PluginManager.GetCustomHandler((ResourceType)entry.ResType) != null)
                                {
                                    // @todo: throw some kind of error here
                                }

                                // store as normal data
                                entry.ModifiedEntry.Data = data;
                            }

                            entry.OnModified();
                        }

                        int hash = Fnv1.HashString(entry.Name);
                        if (!h32map.ContainsKey(hash))
                            h32map.Add(hash, entry);
                    }
                }

                // chunks
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    Guid id = reader.ReadGuid();
                    List<int> bundles = new List<int>();
                    List<int> superBundles = new List<int>();

                    if (version >= 13)
                    {
                        int length = reader.ReadInt();
                        for (int j = 0; j < length; j++)
                        {
                            string bundleName = reader.ReadNullTerminatedString();
                            int bid = App.AssetManager.GetBundleId(bundleName);
                            if (bid != -1)
                                bundles.Add(bid);
                        }
                    }

                    if (version > 13)
                    {
                        int length = reader.ReadInt();
                        for (int j = 0; j < length; j++)
                        {
                            string superBundleName = reader.ReadNullTerminatedString();
                            int sbid = App.AssetManager.GetSuperBundleId(superBundleName);
                            if (sbid != -1)
                            {
                                superBundles.Add(sbid);
                            }
                        }
                    }

                    bool isModified = true;
                    if (version >= 13)
                        isModified = reader.ReadBoolean();

                    Sha1 sha1 = Sha1.Zero;
                    uint logicalOffset = 0;
                    uint logicalSize = 0;
                    uint rangeStart = 0;
                    uint rangeEnd = 0;
                    int firstMip = -1;
                    int h32 = 0;
                    bool addToChunkBundles = false;
                    string userData = "";
                    byte[] data = null;

                    if (isModified)
                    {
                        sha1 = reader.ReadSha1();
                        logicalOffset = reader.ReadUInt();
                        logicalSize = reader.ReadUInt();
                        rangeStart = reader.ReadUInt();
                        rangeEnd = reader.ReadUInt();
                        firstMip = reader.ReadInt();
                        h32 = reader.ReadInt();
                        addToChunkBundles = reader.ReadBoolean();
                        if (version >= 12)
                            userData = reader.ReadNullTerminatedString();

                        if (version < 13)
                        {
                            int length = reader.ReadInt();
                            for (int j = 0; j < length; j++)
                            {
                                string bundleName = reader.ReadNullTerminatedString();
                                int bid = App.AssetManager.GetBundleId(bundleName);
                                if (bid != -1)
                                    bundles.Add(bid);
                            }
                        }

                        data = reader.ReadBytes(reader.ReadInt());
                    }

                    ChunkAssetEntry entry = App.AssetManager.GetChunkEntry(id);

                    if (entry == null && isModified)
                    {
                        // hack: since chunks are not modified by FrostEd patches, instead a new one
                        // is added when something that uses a chunk is modified. If an existing chunk
                        // from a project is missing, a new one is created, and its linked resource
                        // is used to fill in the bundles (this may fail if a chunk is not meant to be
                        // in any bundles)

                        ChunkAssetEntry newEntry = new ChunkAssetEntry
                        {
                            Id = id,
                            H32 = h32
                        };
                        App.AssetManager.AddChunk(newEntry);

                        if (h32map.ContainsKey(newEntry.H32))
                        {
                            foreach (int bundleId in h32map[newEntry.H32].Bundles)
                                newEntry.AddToBundle(bundleId);
                        }
                        entry = newEntry;
                    }

                    if (entry != null)
                    {
                        entry.AddedBundles.AddRange(bundles);
                        entry.AddedSuperBundles.AddRange(superBundles);
                        if (isModified)
                        {
                            entry.ModifiedEntry = new ModifiedAssetEntry
                            {
                                Sha1 = sha1,
                                LogicalOffset = logicalOffset,
                                LogicalSize = logicalSize,
                                RangeStart = rangeStart,
                                RangeEnd = rangeEnd,
                                FirstMip = firstMip,
                                H32 = h32,
                                AddToChunkBundle = addToChunkBundles,
                                UserData = userData,
                                Data = data
                            };
                            entry.OnModified();
                        }
                    }
                }

                // custom actions
                numItems = reader.ReadInt();
                for (int i = 0; i < numItems; i++)
                {
                    string typeString = reader.ReadNullTerminatedString();

                    ICustomAssetCustomActionHandler actionHandler = new LegacyCustomActionHandler();
                    actionHandler.LoadFromProject(version, reader, typeString);

                    foreach (var test in App.PluginManager.CustomAssetHandlers)
                    {
                        actionHandler = App.PluginManager.GetCustomAssetHandler(test);
                        actionHandler.LoadFromProject(version, reader, typeString);
                    }

                    // @hack: fixes an issue where v11 projects incorrectly wrote a null custom handler
                    if (version < 12)
                        break;
                }
            });

            return true;
        }

        private bool LegacyLoad(string inFilename)
        {
            Dictionary<int, AssetEntry> h32map = new Dictionary<int, AssetEntry>();

            DbObject project = null;
            using (DbReader reader = new DbReader(new FileStream(inFilename, FileMode.Open, FileAccess.Read), null))
                project = reader.ReadDbObject();

            uint version = project.GetValue<uint>("version");
            if (version > FormatVersion)
                return false;

            string gameProfile = project.GetValue<string>("gameProfile", ProfilesLibrary.ProfileName);
            if (gameProfile.ToLower() != ProfilesLibrary.ProfileName.ToLower())
                return false;

            creationDate = new DateTime(project.GetValue<long>("creationDate"));
            modifiedDate = new DateTime(project.GetValue<long>("modifiedDate"));
            gameVersion = project.GetValue<uint>("gameVersion");

            DbObject modObj = project.GetValue<DbObject>("modSettings");
            if (modObj != null)
            {
                modSettings.Title = modObj.GetValue("title", "");
                modSettings.Author = modObj.GetValue("author", "");
                modSettings.Category = modObj.GetValue("category", "");
                modSettings.Version = modObj.GetValue("version", "");
                modSettings.Description = modObj.GetValue("description", "");
                modSettings.Icon = modObj.GetValue<byte[]>("icon");
                modSettings.SetScreenshot(0, modObj.GetValue<byte[]>("screenshot1"));
                modSettings.SetScreenshot(1, modObj.GetValue<byte[]>("screenshot2"));
                modSettings.SetScreenshot(2, modObj.GetValue<byte[]>("screenshot3"));
                modSettings.SetScreenshot(3, modObj.GetValue<byte[]>("screenshot4"));
                modSettings.ClearDirtyFlag();
            }

            // load added assets first
            DbObject addedObjs = project.GetValue<DbObject>("added");
            if (addedObjs != null)
            {
                foreach (DbObject sbObj in addedObjs.GetValue<DbObject>("superbundles"))
                    App.AssetManager.AddSuperBundle(sbObj.GetValue<string>("name"));

                foreach (DbObject bObj in addedObjs.GetValue<DbObject>("bundles"))
                {
                    App.AssetManager.AddBundle(
                        bObj.GetValue<string>("name"),
                        (BundleType)bObj.GetValue<int>("type"),
                        App.AssetManager.GetSuperBundleId(bObj.GetValue<string>("superbundle"))
                        );
                }

                foreach (DbObject ebx in addedObjs.GetValue<DbObject>("ebx"))
                {
                    EbxAssetEntry newEntry = new EbxAssetEntry
                    {
                        Name = ebx.GetValue<string>("name"),
                        Guid = ebx.GetValue<Guid>("guid"),
                        Type = ebx.GetValue<string>("type", "UnknownAsset")
                    };
                    App.AssetManager.AddEbx(newEntry);
                }

                foreach (DbObject res in addedObjs.GetValue<DbObject>("res"))
                {
                    ResAssetEntry newEntry = new ResAssetEntry
                    {
                        Name = res.GetValue<string>("name"),
                        ResRid = (ulong)res.GetValue<long>("resRid"),
                        ResType = (uint)res.GetValue<int>("resType")
                    };
                    App.AssetManager.AddRes(newEntry);
                }

                foreach (DbObject chunk in addedObjs.GetValue<DbObject>("chunks"))
                {
                    ChunkAssetEntry newEntry = new ChunkAssetEntry
                    {
                        Id = chunk.GetValue<Guid>("id"),
                        H32 = chunk.GetValue<int>("H32")
                    };
                    App.AssetManager.AddChunk(newEntry);
                }
            }

            if (version < 6)
            {
                // prior to v6. Only chunks could be added
                foreach (DbObject chunk in project.GetValue<DbObject>("chunks"))
                {
                    if (chunk.GetValue<bool>("added"))
                    {
                        ChunkAssetEntry newEntry = new ChunkAssetEntry {Id = chunk.GetValue<Guid>("id")};
                        App.AssetManager.AddChunk(newEntry);
                    }
                }
            }

            DbObject modifiedObjs = project.GetValue<DbObject>("modified") ?? project;

            foreach (DbObject res in modifiedObjs.GetValue<DbObject>("res"))
            {
                ResAssetEntry entry = App.AssetManager.GetResEntry(res.GetValue<string>("name"));
                if (entry == null)
                {
                    // not sure what to do in this scenario
                }

                if (entry != null)
                {
                    LoadLinkedAssets(res, entry, version);
                    if (res.HasValue("data"))
                    {
                        entry.ModifiedEntry = new ModifiedAssetEntry
                        {
                            Sha1 = res.GetValue<Sha1>("sha1"),
                            OriginalSize = res.GetValue<long>("originalSize"),
                            Data = res.GetValue<byte[]>("data"),
                            ResMeta = res.GetValue<byte[]>("meta")
                        };
                    }

                    if (res.HasValue("bundles"))
                    {
                        DbObject bundles = res.GetValue<DbObject>("bundles");
                        foreach (string bundle in bundles)
                            entry.AddedBundles.Add(App.AssetManager.GetBundleId(bundle));
                    }

                    int hash = Fnv1.HashString(entry.Name);
                    if (!h32map.ContainsKey(hash))
                        h32map.Add(hash, entry);
                }
            }
            foreach (DbObject ebx in modifiedObjs.GetValue<DbObject>("ebx"))
            {
                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(ebx.GetValue<string>("name"));
                if (entry == null)
                {
                    // not sure what to do in this scenario
                }

                if (entry != null)
                {
                    LoadLinkedAssets(ebx, entry, version);
                    if (ebx.HasValue("data"))
                    {
                        entry.ModifiedEntry = new ModifiedAssetEntry();
                        byte[] data = ebx.GetValue<byte[]>("data");

                        if (version < 7)
                        {
                            // old ebx stored as compressed byte stream
                            using (CasReader reader = new CasReader(new MemoryStream(data)))
                                data = reader.Read();
                        }

                        using (EbxReader reader = EbxReader.CreateReader(new MemoryStream(data)))
                        {
                            EbxAsset asset = reader.ReadAsset<EbxAsset>();
                            entry.ModifiedEntry.DataObject = asset;
                        }

                        if (ebx.HasValue("transient"))
                            entry.ModifiedEntry.IsTransientModified = true;
                    }

                    if (ebx.HasValue("bundles"))
                    {
                        DbObject bundles = ebx.GetValue<DbObject>("bundles");
                        foreach (string bundle in bundles)
                            entry.AddedBundles.Add(App.AssetManager.GetBundleId(bundle));
                    }

                    int hash = Fnv1.HashString(entry.Name);
                    if (!h32map.ContainsKey(hash))
                        h32map.Add(hash, entry);
                }
            }
            foreach (DbObject chunk in modifiedObjs.GetValue<DbObject>("chunks"))
            {
                Guid id = chunk.GetValue<Guid>("id");
                ChunkAssetEntry entry = App.AssetManager.GetChunkEntry(id);

                if (entry == null)
                {
                    // hack: since chunks are not modified by FrostEd patches, instead a new one
                    // is added when something that uses a chunk is modified. If an existing chunk
                    // from a project is missing, a new one is created, and its linked resource
                    // is used to fill in the bundles (this may fail if a chunk is not meant to be
                    // in any bundles)

                    ChunkAssetEntry newEntry = new ChunkAssetEntry
                    {
                        Id = chunk.GetValue<Guid>("id"),
                        H32 = chunk.GetValue<int>("H32")
                    };
                    App.AssetManager.AddChunk(newEntry);

                    if (h32map.ContainsKey(newEntry.H32))
                    {
                        foreach (int bundleId in h32map[newEntry.H32].Bundles)
                            newEntry.AddToBundle(bundleId);
                    }
                    entry = newEntry;
                }

                if (chunk.HasValue("data"))
                {
                    entry.ModifiedEntry = new ModifiedAssetEntry
                    {
                        Sha1 = chunk.GetValue<Sha1>("sha1"),
                        Data = chunk.GetValue<byte[]>("data"),
                        LogicalOffset = chunk.GetValue<uint>("logicalOffset"),
                        LogicalSize = chunk.GetValue<uint>("logicalSize"),
                        RangeStart = chunk.GetValue<uint>("rangeStart"),
                        RangeEnd = chunk.GetValue<uint>("rangeEnd"),
                        FirstMip = chunk.GetValue<int>("firstMip", -1),
                        H32 = chunk.GetValue<int>("h32", 0),
                        AddToChunkBundle = chunk.GetValue<bool>("addToChunkBundle", true)
                    };
                }
                else
                {
                    entry.FirstMip = chunk.GetValue<int>("firstMip", -1);
                    entry.H32 = chunk.GetValue<int>("h32", 0);
                }

                if (chunk.HasValue("bundles"))
                {
                    DbObject bundles = chunk.GetValue<DbObject>("bundles");
                    foreach (string bundle in bundles)
                        entry.AddedBundles.Add(App.AssetManager.GetBundleId(bundle));
                }
            }

            if (version < 8)
            {
                foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(modifiedOnly: true))
                {
                    foreach (AssetEntry linkedEntry in entry.LinkedAssets)
                    {
                        if (linkedEntry is ChunkAssetEntry)
                            linkedEntry.ModifiedEntry.H32 = Fnv1.HashString(entry.Name.ToLower());
                    }
                }
                foreach (ResAssetEntry entry in App.AssetManager.EnumerateRes(modifiedOnly: true))
                {
                    foreach (AssetEntry linkedEntry in entry.LinkedAssets)
                    {
                        if (linkedEntry is ChunkAssetEntry)
                            linkedEntry.ModifiedEntry.H32 = Fnv1.HashString(entry.Name.ToLower());
                    }
                }
            }

            return true;
        }
    }
}
