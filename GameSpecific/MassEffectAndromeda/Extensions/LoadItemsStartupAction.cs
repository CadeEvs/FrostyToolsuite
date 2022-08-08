using Frosty.Core;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using LevelEditorPlugin.Assets;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Extensions
{
    public class LoadItemsStartupAction : StartupAction
    {
        private string ItemsDatabasePath => $"{App.ProfileSettingsPath}/ItemsDatabase.bin";

        public override Action<ILogger> Action => new Action<ILogger>((ILogger logger) =>
        {
           if (File.Exists(ItemsDatabasePath))
            {
                using (NativeReader reader = new NativeReader(new FileStream(ItemsDatabasePath, FileMode.Open, FileAccess.Read)))
                {
                    int itemCount = reader.ReadInt();
                    for (int i = 0; i < itemCount; i++)
                    {
                        uint hash = reader.ReadUInt();
                        string name = reader.ReadNullTerminatedString();

                        ItemData.itemHashDict.Add(hash, name);
                    }
                }
            }
            else
            {
                logger.Log("Building items database");

                var itemsList = App.AssetManager.EnumerateEbx("ItemData");
                int itemsCount = itemsList.Count();
                int index = 0;

                FileInfo fi = new FileInfo(ItemsDatabasePath);
                if (!fi.Directory.Exists)
                    Directory.CreateDirectory(fi.DirectoryName);

                using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
                {
                    writer.Write(itemsCount);
                    foreach (var entry in itemsList)
                    {
                        var itemData = LoadedAssetManager.Instance.LoadAsset<ItemData>(entry.Name);
                        ItemData.itemHashDict.Add(itemData.Data.ItemHash, entry.Name);
                        LoadedAssetManager.Instance.UnloadAsset(itemData);

                        writer.Write(itemData.Data.ItemHash);
                        writer.WriteNullTerminatedString(entry.Name);

                        logger.Log("progress:" + ((index++ / (double)itemsCount) * 100.0));
                    }
                }
            }
        });
    }
}
