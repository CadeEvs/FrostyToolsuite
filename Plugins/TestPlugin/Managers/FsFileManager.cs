using Frosty.Core;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.Managers
{
    public class FsFileEntry : AssetEntry
    {
        public override string AssetType => "fs";

        public override string Type
        {
            get
            {
                int lastPeriodIndex = Name.LastIndexOf('.');
                return lastPeriodIndex == -1 ? "" : Name.Substring(lastPeriodIndex + 1).ToUpper();
            }
            set { }
        }

        public override string Filename
        {
            get
            {
                int lastPeriodIndex = base.Filename.LastIndexOf('.');
                return lastPeriodIndex == -1 ? base.Filename : base.Filename.Substring(0, lastPeriodIndex);
            }
        }
    }

    public class FsFileManager : ICustomAssetManager
    {
        private Dictionary<int, FsFileEntry> entries = new Dictionary<int, FsFileEntry>();

        public IEnumerable<AssetEntry> EnumerateAssets(bool modifiedOnly)
        {
            foreach (AssetEntry entry in entries.Values)
            {
                if (modifiedOnly && !entry.IsModified)
                    continue;
                yield return entry;
            }
        }

        public Stream GetAsset(AssetEntry entry)
        {
            if (entry != null)
            {
                byte[] buf = App.FileSystem.GetFileFromMemoryFs(entry.Name);

                return new MemoryStream(buf);
            }
            else
                return null;
        }

        public AssetEntry GetAssetEntry(string key)
        {
            return entries.ContainsKey(Fnv1.HashString(key)) ? entries[Fnv1.HashString(key)] : null;
        }

        public void Initialize(ILogger logger)
        {
            logger.Log("Loading fs files");

            uint totalCount = App.FileSystem.GetFsCount();
            uint index = 0;
            foreach (string fsFileName in App.FileSystem.EnumerateFilesInMemoryFs())
            {
                uint progress = (uint)((index / (float)totalCount) * 100);
                logger.Log("progress:" + progress);

                int hash = Fnv1.HashString(fsFileName);

                FsFileEntry entry = new FsFileEntry() { Name = fsFileName };
                entries.Add(hash, entry);
            }
        }

        public void ModifyAsset(string key, byte[] data)
        {
            int hash = Fnv1.HashString(key);
            if (!entries.ContainsKey(hash))
                return;

            FsFileEntry entry = entries[hash];

            if (entry.ModifiedEntry == null)
                entry.ModifiedEntry = new ModifiedAssetEntry();

            using (DbReader reader = new DbReader(new MemoryStream(data), null))
                entry.ModifiedEntry.DataObject = reader.ReadDbObject();
            entry.IsDirty = true;
        }

        public void OnCommand(string command, params object[] value)
        {
            throw new NotImplementedException();
        }
    }
}
