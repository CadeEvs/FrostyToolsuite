using AtlasTexturePlugin;
using DuplicationPlugin.Windows;
using Frosty.Core;
using Frosty.Core.Viewport;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;
using MeshSetPlugin.Resources;
//using SoundEditorPlugin.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DuplicationPlugin
{
    public class TextureBaseExtension : DuplicateAssetExtension
    {
        public override string AssetType => "TextureBaseAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newTextureAsset = newAsset.RootObject;

            // Get the original asset root object data
            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic textureAsset = asset.RootObject;

            // Get the original chunk and res entries
            ResAssetEntry resEntry = App.AssetManager.GetResEntry(textureAsset.Resource);
            Texture texture = App.AssetManager.GetResAs<Texture>(resEntry);
            ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(texture.ChunkId);

            // Duplicate the res
            ResAssetEntry newResEntry = DuplicateRes(resEntry, newName, ResourceType.Texture);
            Texture newTexture = App.AssetManager.GetResAs<Texture>(newResEntry);
            newTextureAsset.Resource = newResEntry.ResRid;

            // Duplicate the chunk
            ChunkAssetEntry newChunkEntry = DuplicateChunk(chunkEntry, (texture.Flags.HasFlag(TextureFlags.OnDemandLoaded) || texture.Type != TextureType.TT_2d) ? null : texture);
            newTexture.ChunkId = newChunkEntry.Id;

            // Link the newly duplicates ebx, chunk, and res entries together
            newResEntry.LinkAsset(newChunkEntry);
            newEntry.LinkAsset(newResEntry);

            // Modify the newly duplicates ebx, chunk, and res
            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);
            App.AssetManager.ModifyRes(newResEntry.Name, newTexture);

            return newEntry;
        }
    }

    public class AtlasTextureExtension : DuplicateAssetExtension
    {
        public override string AssetType => "AtlasTextureAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the res
            ResAssetEntry resEntry = App.AssetManager.GetResEntry(newRoot.Resource);
            ResAssetEntry newResEntry = DuplicateRes(resEntry, newEntry.Name, ResourceType.AtlasTexture);

            // Update the res
            AtlasTexture atlasTexture = App.AssetManager.GetResAs<AtlasTexture>(newResEntry);
            ChunkAssetEntry newChunkEntry = DuplicateChunk(App.AssetManager.GetChunkEntry(atlasTexture.ChunkId));
            atlasTexture.SetData(atlasTexture.Width, atlasTexture.Height, newChunkEntry.Id, App.AssetManager);
            atlasTexture.SetNameHash((uint)Utils.HashString($"Output/Win32/{newResEntry.Name}.res", true));

            newResEntry.LinkAsset(newChunkEntry);

            // Update the ebx
            newRoot.Resource = newResEntry.ResRid;
            newEntry.LinkAsset(newResEntry);

            App.AssetManager.ModifyRes(newResEntry.Name, atlasTexture);
            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class MeshExtension : DuplicateAssetExtension
    {
        public override string AssetType => "MeshAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Meshes always have lowercase names
            newName = newName.ToLower();

            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the res
            ResAssetEntry oldResEntry = App.AssetManager.GetResEntry(newRoot.MeshSetResource);
            ResAssetEntry newResEntry = DuplicateRes(oldResEntry, newName, ResourceType.MeshSet);

            // Update new meshset
            MeshSet newMeshSet = App.AssetManager.GetResAs<MeshSet>(newResEntry);
            newMeshSet.FullName = newResEntry.Name;

            // Duplicate the lod chunks
            foreach (var lod in newMeshSet.Lods)
            {
                lod.Name = newResEntry.Name;
                if (lod.ChunkId != Guid.Empty)
                {
                    ChunkAssetEntry lodChunk = App.AssetManager.GetChunkEntry(lod.ChunkId);
                    ChunkAssetEntry newChunkEntry = DuplicateChunk(lodChunk);
                    lod.ChunkId = newChunkEntry.Id;
                    newResEntry.LinkAsset(newChunkEntry);
                }
            }

            // Update the ebx
            newRoot.MeshSetResource = newResEntry.ResRid;
            newRoot.NameHash = (uint)Utils.HashString(newName);
            newEntry.LinkAsset(newResEntry);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
            {
                // Load in texture parameters
                if (!entry.IsDirectlyModified)
                {
                    MeshVariationDb.LoadModifiedVariations();
                    MeshVariation mvdbVariation = MeshVariationDb.GetVariations(entry.Guid).Variations[0];
                    for (int i = 0; i < newRoot.Materials.Count; i++)
                    {
                        dynamic material = newRoot.Materials[i].Internal;
                        if (material.Shader.TextureParameters.Count == 0)
                        {
                            dynamic textureParams = mvdbVariation.Materials[i].TextureParameters;
                            foreach (dynamic param in textureParams)
                                material.Shader.TextureParameters.Add(param);
                        }
                    }
                }

                // Duplicate the sbd
                ResAssetEntry oldShaderBlock = App.AssetManager.GetResEntry(entry.Name.ToLower() + "_mesh/blocks");
                ResAssetEntry newShaderBlock = DuplicateRes(oldShaderBlock, newResEntry.Name + "_mesh/blocks", ResourceType.ShaderBlockDepot);
                ShaderBlockDepot newShaderBlockDepot = App.AssetManager.GetResAs<ShaderBlockDepot>(newShaderBlock);

                // TODO: hacky way to generate unique hashes
                for (int i = 0; i < newShaderBlockDepot.ResourceCount; i++)
                {
                    ShaderBlockResource res = newShaderBlockDepot.GetResource(i);
                    res.ChangeHash(newMeshSet.NameHash);
                }

                // Change the references in the sbd
                for (int lod = 0; lod < newMeshSet.Lods.Count; lod++)
                {
                    ShaderBlockEntry sbEntry = newShaderBlockDepot.GetSectionEntry(lod);
                    ShaderBlockMeshVariationEntry sbMvEntry = newShaderBlockDepot.GetResource(sbEntry.Index + 1) as ShaderBlockMeshVariationEntry;

                    // calculate new entry hash
                    sbEntry.SetHash(newMeshSet.NameHash, 0, lod);
                    sbMvEntry.SetHash(newMeshSet.NameHash, 0, lod);

                    // Update the mesh guid
                    for (int section = 0; section < newMeshSet.Lods[lod].Sections.Count; section++)
                    {
                        MeshParamDbBlock mesh = sbEntry.GetMeshParams(section);
                        mesh.MeshAssetGuid = newAsset.RootInstanceGuid;
                    }
                }

                App.AssetManager.ModifyRes(newShaderBlock.Name, newShaderBlockDepot);

                newResEntry.LinkAsset(newShaderBlock);
            }

            App.AssetManager.ModifyRes(newResEntry.Name, newMeshSet);
            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class ClothWrappingExtension : DuplicateAssetExtension
    {
        public override string AssetType => "ClothWrappingAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the res
            ResAssetEntry resEntry = App.AssetManager.GetResEntry(newRoot.ClothWrappingAssetResource);
            ResAssetEntry newResEntry = DuplicateRes(resEntry, newEntry.Name, ResourceType.EAClothEntityData);

            // Update the ebx
            newRoot.ClothWrappingAssetResource = newResEntry.ResRid;
            newEntry.LinkAsset(newResEntry);

            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class ClothExtension : DuplicateAssetExtension
    {
        public override string AssetType => "ClothAsset";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Duplicate the res
            ResAssetEntry resEntry = App.AssetManager.GetResEntry(newRoot.ClothAssetResource);
            ResAssetEntry newResEntry = DuplicateRes(resEntry, newEntry.Name, ResourceType.EAClothAssetData);

            // Update the ebx
            newRoot.ClothAssetResource = newResEntry.ResRid;
            newEntry.LinkAsset(newResEntry);

            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class ObjectVariationExtension : DuplicateAssetExtension
    {
        public override string AssetType => "ObjectVariation";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII)
            {
                return base.DuplicateAsset(entry, newName, createNew, newType);
            }

            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
            EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);
            dynamic newRoot = newAsset.RootObject;

            // Get namehash needed for the sbd
            uint nameHash = newRoot.NameHash;

            foreach (var mv in MeshVariationDb.FindVariations(nameHash))
            {
                // Get meshSet
                EbxAssetEntry meshEntry = App.AssetManager.GetEbxEntry(mv.MeshGuid);
                EbxAsset meshAsset = App.AssetManager.GetEbx(meshEntry);
                dynamic meshRoot = meshAsset.RootObject;
                ResAssetEntry meshRes = App.AssetManager.GetResEntry(meshRoot.MeshSetResource);
                MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(meshRes);

                // Load in texture parameters
                if (!entry.IsDirectlyModified)
                {
                    foreach (dynamic rootMaterial in newAsset.RootObjects)
                    {
                        Type objType = rootMaterial.GetType();
                        if (TypeLibrary.IsSubClassOf(objType, "MeshMaterialVariation"))
                        {

                            if (rootMaterial.Shader.TextureParameters.Count == 0)
                            {
                                AssetClassGuid guid = rootMaterial.GetInstanceGuid();
                                MeshVariationMaterial mm = null;

                                foreach (MeshVariationMaterial mvm in mv.Materials)
                                {
                                    if (mvm.MaterialVariationClassGuid == guid.ExportedGuid)
                                    {
                                        mm = mvm;
                                        break;
                                    }
                                }

                                if (mm != null)
                                {
                                    dynamic texParams = mm.TextureParameters;
                                    foreach (dynamic param in texParams)
                                        rootMaterial.Shader.TextureParameters.Add(param);
                                }
                            }
                        }
                    }
                }

                // Dupe sbd
                ResAssetEntry resEntry = App.AssetManager.GetResEntry(entry.Name.ToLower() + "/" + meshEntry.Filename + "_" + (uint)Utils.HashString(meshEntry.Name, true) + "/shaderblocks_variation/blocks");
                ResAssetEntry newResEntry = DuplicateRes(resEntry, newName.ToLower() + "/" + meshEntry.Filename + "_" + (uint)Utils.HashString(meshEntry.Name, true) + "/shaderblocks_variation/blocks", ResourceType.ShaderBlockDepot);
                ShaderBlockDepot newShaderBlockDepot = App.AssetManager.GetResAs<ShaderBlockDepot>(newResEntry);

                // change namehash so the sbd hash can be calculated corretcly
                nameHash = (uint)Utils.HashString(newName, true);
                newRoot.NameHash = nameHash;

                for (int i = 0; i < newShaderBlockDepot.ResourceCount; i++)
                {
                    ShaderBlockResource res = newShaderBlockDepot.GetResource(i);
                    if (!(res is MeshParamDbBlock))
                        res.ChangeHash(nameHash);
                }

                // Change the references in the sbd
                for (int lod = 0; lod < meshSet.Lods.Count; lod++)
                {
                    ShaderBlockEntry sbEntry = newShaderBlockDepot.GetSectionEntry(lod);
                    ShaderBlockMeshVariationEntry sbMvEntry = newShaderBlockDepot.GetResource(sbEntry.Index + 1) as ShaderBlockMeshVariationEntry;

                    sbEntry.SetHash(meshSet.NameHash, nameHash, lod);
                    sbMvEntry.SetHash(meshSet.NameHash, nameHash, lod);
                }

                App.AssetManager.ModifyRes(newResEntry.Name, newShaderBlockDepot);
                newEntry.LinkAsset(newResEntry);
                App.AssetManager.ModifyEbx(newName, newAsset);

                break;
            }

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
            newEntry.LinkAsset(newResEntry);

            App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

            return newEntry;
        }
    }

    public class SoundDataExtension : DuplicateAssetExtension
    {
        public override string AssetType => "SoundDataAsset";

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
                ChunkAssetEntry newChunkEntry = DuplicateChunk(soundChunk);

                chunk.ChunkId = newChunkEntry.Id;
                newEntry.LinkAsset(newChunkEntry);
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

    public class BlueprintBundleExtension : DuplicateAssetExtension
    {
        public override string AssetType => "BlueprintBundle";

        public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            // BlueprintBundles always have lower case names
            newName = newName.ToLower();

            // Duplicate the ebx
            EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);

            // Add new bundle
            BundleEntry newBundle = App.AssetManager.AddBundle("win32/" + newName, BundleType.BlueprintBundle, 0);

            newEntry.AddedBundles.Clear();
            newEntry.AddedBundles.Add(App.AssetManager.GetBundleId(newBundle));

            newBundle.Blueprint = newEntry;

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

            byte[] random = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            while (true)
            {
                rng.GetBytes(random);

                if (App.AssetManager.GetEbxEntry(new Guid(random)) == null)
                {
                    break;
                }
                else
                {
                    App.Logger.Log("Randomised onto old guid: " + random.ToString());
                }
            }
            newAsset.SetFileGuid(new Guid(random));

            dynamic obj = newAsset.RootObject;
            obj.Name = newName;

            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(newAsset.Objects, (Type)obj.GetType(), newAsset.FileGuid), -1);
            obj.SetInstanceGuid(guid);

            EbxAssetEntry newEntry = App.AssetManager.AddEbx(newName, newAsset);

            newEntry.AddedBundles.AddRange(entry.EnumerateBundles());
            newEntry.ModifiedEntry.DependentAssets.AddRange(newAsset.Dependencies);

            return newEntry;
        }

        public static ChunkAssetEntry DuplicateChunk(ChunkAssetEntry entry, Texture texture = null)
        {
            byte[] random = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            while (true)
            {
                rng.GetBytes(random);

                random[15] |= 1;

                if (App.AssetManager.GetChunkEntry(new Guid(random)) == null)
                {
                    break;
                }
                else
                {
                    App.Logger.Log("Randomised onto old guid: " + random.ToString());
                }
            }
            Guid newGuid;
            using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(entry)))
            {
                newGuid = App.AssetManager.AddChunk(reader.ReadToEnd(), new Guid(random), texture, entry.EnumerateBundles().ToArray());
            }

            ChunkAssetEntry newEntry = App.AssetManager.GetChunkEntry(newGuid);

            App.Logger.Log(string.Format("Duped chunk {0} to {1}", entry.Name, newGuid));
            return newEntry;
        }
        public static ResAssetEntry DuplicateRes(ResAssetEntry entry, string name, ResourceType resType)
        {
            if (App.AssetManager.GetResEntry(name) == null)
            {
                ResAssetEntry newEntry;
                using (NativeReader reader = new NativeReader(App.AssetManager.GetRes(entry)))
                {
                    newEntry = App.AssetManager.AddRes(name, resType, entry.ResMeta, reader.ReadToEnd(), entry.EnumerateBundles().ToArray());
                }

                App.Logger.Log(string.Format("Duped res {0} to {1}", entry.Name, newEntry.Name));
                return newEntry;
            }
            else
            {
                App.Logger.Log(name + " already has a res files");
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

        public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Add.png") as ImageSource;

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
                if (!MeshVariationDb.IsLoaded)
                    MeshVariationDb.LoadVariations(task);

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
