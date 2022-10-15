using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers.Entries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RootInstanceEntriesPlugin
{
    public static class RootInstanceEbxEntryDb
    {
        public static bool IsLoaded { get; private set; }

        private const uint m_cacheVersion = 1;
        private static Dictionary<Guid, Guid> m_ebxRootInstanceGuidList = new Dictionary<Guid, Guid>();

        public static bool LoadEbxRootInstanceEntries(FrostyTaskWindow task)
        {
            m_ebxRootInstanceGuidList.Clear();

            if (!ReadCache(task))
            {
                MessageBoxResult result = FrostyMessageBox.Show(string.Format("RootInstanceEntries Cache required. Would you like to generate one?"), "RootInstanceEntriesPlugin", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                {
                    return false;
                }

                uint totalCount = App.AssetManager.GetEbxCount();
                uint index = 0;

                task.Update("Collecting ebx root instance guids");

                foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx())
                {
                    uint progress = (uint)((index / (float)totalCount) * 100);
                    task.Update(progress: progress);

                    EbxAsset asset = App.AssetManager.GetEbx(entry);
                    m_ebxRootInstanceGuidList.Add(asset.RootInstanceGuid, entry.Guid);

                    index++;
                }

                WriteToCache(task);
            }
            IsLoaded = true;
            return true;
        }

        public static EbxAssetEntry GetEbxEntryByRootInstanceGuid(Guid guid)
        {
            return m_ebxRootInstanceGuidList.ContainsKey(guid) ? App.AssetManager.GetEbxEntry(m_ebxRootInstanceGuidList[guid]) : null;
        }

        public static bool ReadCache(FrostyTaskWindow task)
        {
            if (!File.Exists($"{App.FileSystemManager.CacheName}_rootinstances.cache"))
                return false;

            task.Update($"Loading data ({App.FileSystemManager.CacheName}_rootinstances.cache)");

            using (NativeReader reader = new NativeReader(new FileStream($"{App.FileSystemManager.CacheName}_rootinstances.cache", FileMode.Open, FileAccess.Read)))
            {
                uint version = reader.ReadUInt();
                if (version != m_cacheVersion)
                    return false;

                int profileHash = reader.ReadInt();
                if (profileHash != Utils.HashString(ProfilesLibrary.ProfileName))
                    return false;

                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Guid rootInstanceGuid = reader.ReadGuid();
                    Guid fileGuid = reader.ReadGuid();

                    m_ebxRootInstanceGuidList.Add(rootInstanceGuid, fileGuid);
                }
            }

            return true;
        }

        public static void WriteToCache(FrostyTaskWindow task)
        {
            FileInfo fi = new FileInfo($"{App.FileSystemManager.CacheName}_rootinstances.cache");
            if (!Directory.Exists(fi.DirectoryName))
                Directory.CreateDirectory(fi.DirectoryName);

            task.Update("Caching data");

            using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
            {
                writer.Write(m_cacheVersion);
                writer.Write(Utils.HashString(ProfilesLibrary.ProfileName));

                writer.Write(m_ebxRootInstanceGuidList.Count);
                foreach (KeyValuePair<Guid, Guid> kv in m_ebxRootInstanceGuidList)
                {
                    writer.Write(kv.Key); // Root Instance Guid
                    writer.Write(kv.Value); // File Guid
                }
            }
        }
    }
}
