using DuplicationPlugin.Windows;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using MeshSetPlugin.Resources;
//using SoundEditorPlugin.Resources;
using SvgImagePlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
            Texture texture = App.AssetManager.GetResAs<Texture>(resEntry);
            ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(texture.ChunkId);

            // Read the texture data 
            Texture newTexture = new Texture();
            newTexture.Read(new NativeReader(App.AssetManager.GetRes(resEntry)), App.AssetManager, resEntry, null);

            // Duplicate the chunk
            Guid chunkGuid = App.AssetManager.AddChunk(new NativeReader(newTexture.Data).ReadToEnd(), null, newTexture);
            ChunkAssetEntry newChunkEntry = App.AssetManager.GetChunkEntry(chunkGuid);
            newTexture.ChunkId = chunkGuid;

            // Duplicate the res
            ResAssetEntry newResEntry = App.AssetManager.AddRes(newName, ResourceType.Texture, resEntry.ResMeta, new NativeReader(App.AssetManager.GetRes(resEntry)).ReadToEnd());
            ((dynamic)newAsset.RootObject).Resource = newResEntry.ResRid;

            // Add the new chunk/res entries to the original bundles
            newResEntry.AddedBundles.AddRange(resEntry.EnumerateBundles());
            newChunkEntry.AddedBundles.AddRange(chunkEntry.EnumerateBundles());

            // Link the newly duplicates ebx, chunk, and res entries together
            newResEntry.LinkAsset(newChunkEntry);
            newEntry.LinkAsset(newResEntry);

            // Modify the newly duplicates ebx, chunk, and res
            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);
            App.AssetManager.ModifyRes(newResEntry.Name, newTexture);

            return newEntry;
        }
    }

    public class MeshExtension : DuplicateAssetExtension
    {
        public override string AssetType => "MeshAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the res
            ResAssetEntry oldResEntry = App.AssetManager.GetResEntry(newRoot.MeshSetResource);
            ResAssetEntry newResEntry = DuplicateRes(oldResEntry, newName.ToLower(), ResourceType.MeshSet);
            
            // Update new meshset
            MeshSet newMeshSet = App.AssetManager.GetResAs<MeshSet>(newResEntry);
            newMeshSet.FullName = newName.ToLower();
            
            // Duplicate the lod chunks
            foreach (var lod in newMeshSet.Lods)
            {
                ChunkAssetEntry lodChunk = App.AssetManager.GetChunkEntry(lod.ChunkId);
                lod.ChunkId = DuplicateChunk(lodChunk);
                lod.Name = newName.ToLower();
                newResEntry.LinkAsset(App.AssetManager.GetChunkEntry(lod.ChunkId));
            }
            
            // Update the ebx
            newRoot.MeshSetResource = newResEntry.ResRid;
            newRoot.NameHash = (uint)Utils.HashString(newName.ToLower());

            // Update ShaderBlockDepots
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                // Duplicate the sbd
                ResAssetEntry oldShaderBlock = App.AssetManager.GetResEntry(entry.Name.ToLower() + "_mesh/blocks");
                ResAssetEntry newShaderBlock = DuplicateRes(oldShaderBlock, newName.ToLower() + "_mesh/blocks", ResourceType.ShaderBlockDepot);
                ShaderBlockDepot shaderBlockDepot = App.AssetManager.GetResAs<ShaderBlockDepot>(oldShaderBlock);
                
                // Change the references in the sbd
                for (int lod = 0; lod < newMeshSet.Lods.Count; lod++)
                {
                    var sbEntry = shaderBlockDepot.GetSectionEntry(lod);
                    var mvEntry = shaderBlockDepot.GetResource(sbEntry.Index + 1);
                    
                    // Currently unknown, they link the mesh with the sbd
                    //sbEntry.Hash = 0x...
                    //mvEntry.Hash = 0x...
                    
                    // Update the mesh guid
                    for (int section = 0; section < newMeshSet.Lods[lod].Sections.Count; section++)
                    {
                        var mesh = sbEntry.GetMeshParams(section);
                        mesh.MeshAssetGuid = newAsset.RootInstanceGuid;
                    }
                }

                App.AssetManager.ModifyRes(newShaderBlock.Name, shaderBlockDepot);
                
                newEntry.LinkAsset(newShaderBlock);
            }

            App.AssetManager.ModifyRes(newResEntry.Name, newMeshSet);
            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            newEntry.LinkAsset(newResEntry);

            return newEntry;
        }
    }

    public class SvgImageExtension : DuplicateAssetExtension
    {
        public override string AssetType => "SvgImage";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the res
            ResAssetEntry resEntry = App.AssetManager.GetResEntry(newRoot.Resource);
            ResAssetEntry newResEntry = DuplicateRes(resEntry, newEntry.Name, ResourceType.SvgImage);

            // Update the ebx
            newRoot.Resource = newResEntry.ResRid;

            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class SoundWaveExtension : DuplicateAssetExtension
    {
        public override string AssetType => "SoundWaveAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the chunks
            foreach (dynamic chunk in newRoot.Chunks)
            {
                ChunkAssetEntry soundChunk = App.AssetManager.GetChunkEntry(chunk.ChunkId);
                Guid chunkId = DuplicateChunk(soundChunk);
                
                chunk.ChunkId = chunkId;
            }

            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class OctaneAssetExtension : DuplicateAssetExtension
    {
        public override string AssetType => "OctaneAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the chunks
            foreach (dynamic chunk in newRoot.Chunks)
            {
                ChunkAssetEntry soundChunk = App.AssetManager.GetChunkEntry(chunk.ChunkId);
                Guid chunkId = DuplicateChunk(soundChunk);

                chunk.ChunkId = chunkId;
            }

            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    //public class NewWaveExtension : DuplicateAssetExtension
    //{
    //    public override string AssetType => "NewWaveAsset";

    //    public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
    //    {
    //        // Duplicate the ebx
    //        EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
    //        EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
    //        dynamic newRoot = newAsset.RootObject;

    //        //Duplicate res
    //        ResAssetEntry resEntry = App.AssetManager.GetResEntry(entry.Name.ToLower());
    //        ResAssetEntry newRes = DuplicateRes(resEntry, newName.ToLower(), ResourceType.NewWaveResource);
    //        NewWaveResource newWave = App.AssetManager.GetResAs<NewWaveResource>(newRes);

    //        // Duplicate the chunks
    //        for (int i = 0; i < newRoot.Chunks.Count; i++)
    //        {
    //            ChunkAssetEntry soundChunk = App.AssetManager.GetChunkEntry(newRoot.Chunks[i].ChunkId);
    //            Guid chunkId = DuplicateChunk(soundChunk);

    //            newRoot.Chunks[i].ChunkId = chunkId;
    //            newWave.Chunks[i].ChunkId = chunkId;
    //        }

    //        App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

    //        return newEntry;
    //    }
    //}

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

        public static Guid DuplicateChunk(ChunkAssetEntry entry)
        {
            byte[] random = new Byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            bool isNewGuid = false;
            while (isNewGuid == false)
            {
                rng.GetBytes(random);
                if (App.AssetManager.GetChunkEntry(new Guid(random)) == null)
                {
                    isNewGuid = true;
                }
                else
                {
                    App.Logger.Log("Randomised onto old guid: " + random.ToString());
                }
            }
            Guid newGuid = App.AssetManager.AddChunk(random, Guid.NewGuid());
            ChunkAssetEntry newEntry = App.AssetManager.GetChunkEntry(newGuid);
            using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(entry)))
            {
                App.AssetManager.ModifyChunk(newGuid, reader.ReadToEnd(), null);
            }
            newEntry.AddedBundles.AddRange(entry.EnumerateBundles());
            App.Logger.Log(string.Format("Duped chunk {0} to {1}", entry.Name, newEntry.Name));
            return newGuid;
        }
        public static ResAssetEntry DuplicateRes(ResAssetEntry entry, string Name, ResourceType resType)
        {
            if (App.AssetManager.GetResEntry(Name) == null)
            {
                ResAssetEntry newEntry = App.AssetManager.AddRes(Name, resType, entry.ResMeta, new byte[0], null);
                using (NativeReader reader = new NativeReader(App.AssetManager.GetRes(entry)))
                {
                    App.AssetManager.ModifyRes(newEntry.ResRid, reader.ReadToEnd(), null);
                }
                newEntry.AddedBundles.AddRange(entry.EnumerateBundles());
                App.Logger.Log(string.Format("Duped res {0} to {1}", entry.Name, newEntry.Name));
                return newEntry;
            }
            else
            {
                App.Logger.Log(Name + " already has a res files");
                return null;
            }
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
                    string key = "null";
                    foreach (string typekey in extensions.Keys)
                    {
                        if (TypeLibrary.IsSubClassOf(entry.Type, typekey))
                        {
                            key = typekey;
                            break;
                        }
                    }

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
