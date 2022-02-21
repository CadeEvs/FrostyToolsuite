using DuplicationPlugin.Windows;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuplicationPlugin
{
    public class TextureExtension : DuplicateAssetExtension
    {
        public override string AssetType => "TextureAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);

            // Get the original asset root object data
            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic textureAsset = asset.RootObject;

            // Get the original chunk and res entries
            ResAssetEntry resEntry = App.AssetManager.GetResEntry(textureAsset.Resource);
            ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(textureAsset.ChunkId);

            // Read the texture data 
            Texture texture = new Texture();
            texture.Read(new NativeReader(App.AssetManager.GetRes(resEntry)), App.AssetManager, resEntry, null);

            // Duplicate the chunk
            Guid chunkGuid = App.AssetManager.AddChunk(new NativeReader(texture.Data).ReadToEnd(), null, texture);
            ChunkAssetEntry newChunkEntry = App.AssetManager.GetChunkEntry(chunkGuid);
            texture.ChunkId = chunkGuid;

            // Duplicate the res
            ResAssetEntry newResEntry = App.AssetManager.AddRes(newName, ResourceType.Texture, resEntry.ResMeta, new NativeReader(App.AssetManager.GetRes(resEntry)).ReadToEnd());
            ((dynamic)newAsset.RootObject).Resource = newResEntry.ResRid;

            // Add the new chunk/res entries to the original bundles
            newResEntry.AddedBundles.AddRange(resEntry.EnumerateBundles());
            newChunkEntry.AddedBundles.AddRange(chunkEntry.EnumerateBundles());

            // Link the newly duplicates ebx, chunk, and res entries together
            newResEntry.LinkAsset(newChunkEntry);
            newEntry.LinkAsset(newResEntry);

            return newEntry;
        }
    }

    public class DuplicateAssetExtension
    {
        public virtual string AssetType => null;

        public virtual EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            EbxAsset asset = App.AssetManager.GetEbx(entry);
            EbxAsset newAsset = null;

            if (createNew)
            {
                newAsset = new EbxAsset(TypeLibrary.CreateObject(newType.Name));
            }
            else
            {
                using (EbxBaseWriter writer = EbxBaseWriter.CreateWriter(new MemoryStream(), EbxWriteFlags.DoNotSort | EbxWriteFlags.IncludeTransient))
                {
                    writer.WriteAsset(asset);
                    byte[] buf = writer.ToByteArray();
                    using (EbxReader reader = EbxReader.CreateReader(new MemoryStream(buf)))
                        newAsset = reader.ReadAsset<EbxAsset>();
                }
            }

            newAsset.SetFileGuid(Guid.NewGuid());

            dynamic obj = newAsset.RootObject;
            obj.Name = newName;

            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(newAsset.Objects, (Type)obj.GetType(), newAsset.FileGuid), -1);
            obj.SetInstanceGuid(guid);

            EbxAssetEntry newEntry = App.AssetManager.AddEbx(newName, newAsset);

            newEntry.AddedBundles.AddRange(entry.EnumerateBundles());
            newEntry.ModifiedEntry.DependentAssets.AddRange(newAsset.Dependencies);

            return newEntry;
        }
    }

    public class DuplicateContextMenuItem : DataExplorerContextMenuExtension
    {
        private Dictionary<string, DuplicateAssetExtension> extensions = new Dictionary<string, DuplicateAssetExtension>();

        public DuplicateContextMenuItem()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(DuplicateAssetExtension)))
                {
                    var extension = (DuplicateAssetExtension)Activator.CreateInstance(type);
                    extensions.Add(extension.AssetType, extension);
                }
            }
            extensions.Add("null", new DuplicateAssetExtension());
        }

        public override string ContextItemName => "Duplicate";

        public override RelayCommand ContextItemClicked => new RelayCommand((o) =>
        {
            EbxAssetEntry entry = App.SelectedAsset as EbxAssetEntry;
            EbxAsset asset = App.AssetManager.GetEbx(entry);

            DuplicateAssetWindow win = new DuplicateAssetWindow(entry);
            if (win.ShowDialog() == false)
                return;

            string newName = win.SelectedPath + "/" + win.SelectedName;
            newName = newName.Trim('/');

            Type newType = win.SelectedType;
            FrostyTaskWindow.Show("Duplicating asset", "", (task) =>
            {
                try
                {
                    string key = entry.Type;
                    if (!extensions.ContainsKey(entry.Type))
                        key = "null";
                    extensions[key].DuplicateAsset(entry, newName, newType != null, newType);
                }
                catch (Exception e)
                {
                    App.Logger.Log($"Failed to duplicate {entry.Name}");
                }
            });

            App.EditorWindow.DataExplorer.RefreshAll();
        });
    }
}
