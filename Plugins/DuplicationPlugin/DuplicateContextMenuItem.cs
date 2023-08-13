using AtlasTexturePlugin;
using DuplicationPlugin.Windows;
using Frosty.Core;
using Frosty.Core.Viewport;
using Frosty.Core.Windows;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using MeshSetPlugin.Resources;
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
    public class DuplicationTool
    {
        #region --Extensions--

        public class SoundWaveExtension : DuplicateAssetExtension
        {
            public override string AssetType => "SoundWaveAsset";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                EbxAssetEntry refEntry = base.DuplicateAsset(entry, newName, createNew, newType);

                EbxAsset refAsset = App.AssetManager.GetEbx(refEntry);
                dynamic refRoot = refAsset.RootObject;

                foreach (dynamic chkref in refRoot.Chunks)
                {
                    ChunkAssetEntry soundChunk = App.AssetManager.GetChunkEntry(chkref.ChunkId);
                    ChunkAssetEntry newSoundChunk = DuplicateChunk(soundChunk);
                    chkref.ChunkId = newSoundChunk.Id;
                }
                App.AssetManager.ModifyEbx(refEntry.Name, refAsset);

                return refEntry;
            }
        }

        public class PathfindingExtension : DuplicateAssetExtension
        {

            public override string AssetType => "PathfindingBlobAsset";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                EbxAssetEntry refEntry = base.DuplicateAsset(entry, newName, createNew, newType);

                EbxAsset refAsset = App.AssetManager.GetEbx(refEntry);
                dynamic refRoot = refAsset.RootObject;
                ChunkAssetEntry pathfindingChunk = App.AssetManager.GetChunkEntry(refRoot.Blob.BlobId);
                if (pathfindingChunk != null)
                {
                    ChunkAssetEntry newPathfindingChunk = DuplicateChunk(pathfindingChunk);
                    if (newPathfindingChunk != null)
                    {
                        refRoot.Blob.BlobId = pathfindingChunk.Id;
                    }
                }

                App.AssetManager.ModifyEbx(refEntry.Name, refAsset);

                return refEntry;
            }
        }

        #region --Bundles--

        public class BlueprintBundleExtension : DuplicateAssetExtension
        {
            public override string AssetType => "BlueprintBundle";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                // Duplicate the ebx
                EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);

                // Add new bundle
                BundleEntry oldBundle = App.AssetManager.GetBundleEntry(newEntry.AddedBundles[0]);
                BundleEntry newBundle = App.AssetManager.AddBundle("win32/" + newName.ToLower(), BundleType.BlueprintBundle, oldBundle.SuperBundleId);

                newEntry.AddedBundles.Clear();
                newEntry.AddedBundles.Add(App.AssetManager.GetBundleId(newBundle));

                newBundle.Blueprint = newEntry;

                return newEntry;
            }
        }

        public class SubWorldDataExtension : DuplicateAssetExtension
        {
            public override string AssetType => "SubWorldData";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                // Duplicate the ebx
                EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);

                // Add new bundle
                BundleEntry oldBundle = App.AssetManager.GetBundleEntry(newEntry.AddedBundles[0]);
                BundleEntry newBundle = App.AssetManager.AddBundle("win32/" + newName, BundleType.SubLevel, oldBundle.SuperBundleId);

                newEntry.AddedBundles.Clear();
                newEntry.AddedBundles.Add(App.AssetManager.GetBundleId(newBundle));

                newBundle.Blueprint = newEntry;
                newEntry.LinkAsset(entry);

                return newEntry;
            }
        }

        #endregion

        #region --Meshes--

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

                // Modify the ebx
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

                // Modify the ebx
                App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

                return newEntry;
            }
        }

        public class ObjectVariationExtension : DuplicateAssetExtension
        {
            public override string AssetType => "ObjectVariation";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                EbxAssetEntry newAssetEntry = base.DuplicateAsset(entry, newName, createNew, newType);

                //Get the ebx and root object from our duped entry
                EbxAsset newEbx = App.AssetManager.GetEbx(newAssetEntry);
                dynamic newRootObject = newEbx.RootObject;

                // Get the ebx and root object from the original entry
                EbxAsset oldEbx = App.AssetManager.GetEbx(entry);
                dynamic oldRootObject = oldEbx.RootObject;

                // The NameHash needs to be the 32 bit Fnv1 of the lowercased name
                newRootObject.NameHash = (uint)Utils.HashString(newName, true);

                // SWBF2 has fancy res files for object variations, we need to dupe these. Other games just need the namehash
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
                {
                    // Get the original name hash, this will be useful for when we change it
                    uint nameHash = oldRootObject.NameHash;

                    // find a Mesh Variation entry with our NameHash
                    MeshVariation meshVariation = MeshVariationDb.FindVariations(nameHash, true).First();

                    // Get meshSet
                    EbxAssetEntry meshEntry = App.AssetManager.GetEbxEntry(meshVariation.MeshGuid);
                    EbxAsset meshAsset = App.AssetManager.GetEbx(meshEntry);
                    dynamic meshRoot = meshAsset.RootObject;
                    ResAssetEntry meshRes = App.AssetManager.GetResEntry(meshRoot.MeshSetResource);
                    MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(meshRes);

                    foreach (object matObject in newEbx.RootObjects) // For each material in the new variation
                    {
                        // Check if this is actually a material
                        if (TypeLibrary.IsSubClassOf(matObject.GetType(), "MeshMaterialVariation") && ((dynamic)matObject).Shader.TextureParameters.Count == 0)
                        {
                            // Get the material object as a dynamic, so we can see and edit its properties
                            dynamic matProperties = matObject as dynamic;

                            AssetClassGuid guid = matProperties.GetInstanceGuid();

                            foreach (MeshVariationMaterial mvm in meshVariation.Materials) // For each material in the original assets MVDB Entry
                            {
                                if (mvm.MaterialVariationClassGuid == guid.ExportedGuid) //If it has the same guid
                                {
                                    // We then use its texture params as the texture params in the variation
                                    foreach (dynamic texParam in (dynamic)mvm.TextureParameters)
                                    {
                                        matProperties.Shader.TextureParameters.Add(texParam);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // Dupe sbd
                    ResAssetEntry resEntry = App.AssetManager.GetResEntry(entry.Name.ToLower() + "/" + meshEntry.Filename + "_" + (uint)Utils.HashString(meshEntry.Name, true) + "/shaderblocks_variation/blocks");
                    ResAssetEntry newResEntry = DuplicateRes(resEntry, newName.ToLower() + "/" + meshEntry.Filename + "_" + (uint)Utils.HashString(meshEntry.Name, true) + "/shaderblocks_variation/blocks", ResourceType.ShaderBlockDepot);
                    ShaderBlockDepot newShaderBlockDepot = App.AssetManager.GetResAs<ShaderBlockDepot>(newResEntry);

                    for (int i = 0; i < newShaderBlockDepot.ResourceCount; i++)
                    {
                        ShaderBlockResource res = newShaderBlockDepot.GetResource(i);
                        if (!(res is MeshParamDbBlock))
                            res.ChangeHash(newRootObject.NameHash);
                    }

                    // Change the references in the sbd
                    for (int lod = 0; lod < meshSet.Lods.Count; lod++)
                    {
                        ShaderBlockEntry sbEntry = newShaderBlockDepot.GetSectionEntry(lod);
                        ShaderBlockMeshVariationEntry sbMvEntry = newShaderBlockDepot.GetResource(sbEntry.Index + 1) as ShaderBlockMeshVariationEntry;

                        sbEntry.SetHash(meshSet.NameHash, newRootObject.NameHash, lod);
                        sbMvEntry.SetHash(meshSet.NameHash, newRootObject.NameHash, lod);
                    }

                    App.AssetManager.ModifyRes(newResEntry.Name, newShaderBlockDepot);
                    newAssetEntry.LinkAsset(newResEntry);
                }

                App.Logger.Log("Duped {0} with a namehash of {1}", newAssetEntry.Filename, newRootObject.NameHash.ToString());

                App.AssetManager.ModifyEbx(newName, newEbx);
                return newAssetEntry;
            }
        }

        public class MeshExtension : DuplicateAssetExtension
        {
            public override string AssetType => "MeshAsset";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                //2017 battlefront meshes always have lowercase names. This doesn't apply to all games, but its still safer to do so
                newName = newName.ToLower();
                
                // Duplicate the ebx
                EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName, createNew, newType);
                EbxAsset newAsset = App.AssetManager.GetEbx(newEntry);

                // Get the original asset root object data
                EbxAsset asset = App.AssetManager.GetEbx(entry);
                dynamic meshProperties = asset.RootObject;
                dynamic newMeshProperties = newAsset.RootObject;

                //Get the original res entry and duplicate it
                ResAssetEntry resAsset = App.AssetManager.GetResEntry(meshProperties.MeshSetResource);
                ResAssetEntry newResAsset = DuplicateRes(resAsset, newName, ResourceType.MeshSet);

                //Since this is a mesh we need to get the meshSet for the duplicated entry and set it up
                MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(newResAsset);
                meshSet.FullName = newResAsset.Name;

                //Go through all of the lods and duplicate their chunks
                foreach (MeshSetLod lod in meshSet.Lods)
                {
                    lod.Name = newResAsset.Name;
                    //Double check that the lod actually has a chunk id. If it doesn't this means the data is inline and we don't need to worry
                    if (lod.ChunkId != Guid.Empty)
                    {
                        //Get the original chunk and dupe it
                        ChunkAssetEntry chunk = App.AssetManager.GetChunkEntry(lod.ChunkId);
                        ChunkAssetEntry newChunk = DuplicateChunk(chunk);

                        //Now set the params for the lod
                        lod.ChunkId = newChunk.Id;

                        //Link the res and chunk
                        newResAsset.LinkAsset(newChunk);
                    }
                }

                //Set our new mesh's properties
                newMeshProperties.MeshSetResource = newResAsset.ResRid;
                newMeshProperties.NameHash = (uint)Utils.HashString(newName);

                //Link the res and ebx
                newEntry.LinkAsset(newResAsset);

                // Stuff for SBDs since SWBF2 is weird
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
                {
                    // Restore the texture params
                    MeshVariation meshVariation = MeshVariationDb.GetVariations(entry.Guid).Variations[0];

                    foreach (object matObject in newAsset.Objects) // For each material in the new variation
                    {
                        // Check if this is actually a material
                        if (TypeLibrary.IsSubClassOf(matObject.GetType(), "MeshMaterial") && ((dynamic)matObject).Shader.TextureParameters.Count == 0)
                        {
                            // Get the material object as a dynamic, so we can see and edit its properties
                            dynamic matProperties = matObject as dynamic;

                            AssetClassGuid guid = matProperties.GetInstanceGuid();

                            foreach (MeshVariationMaterial mvm in meshVariation.Materials) // For each material in the original assets MVDB Entry
                            {
                                if (mvm.MaterialGuid == guid.ExportedGuid) // If it has the same guid
                                {
                                    // We then use its texture params as the texture params in the variation
                                    foreach (dynamic texParam in (dynamic)mvm.TextureParameters)
                                    {
                                        matProperties.Shader.TextureParameters.Add(texParam);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // Duplicate the sbd
                    ResAssetEntry oldShaderBlock = App.AssetManager.GetResEntry(entry.Name.ToLower() + "_mesh/blocks");
                    ResAssetEntry newShaderBlock = DuplicateRes(oldShaderBlock, newResAsset.Name + "_mesh/blocks", ResourceType.ShaderBlockDepot);
                    ShaderBlockDepot newShaderBlockDepot = App.AssetManager.GetResAs<ShaderBlockDepot>(newShaderBlock);

                    // TODO: hacky way to generate unique hashes
                    for (int i = 0; i < newShaderBlockDepot.ResourceCount; i++)
                    {
                        ShaderBlockResource res = newShaderBlockDepot.GetResource(i);
                        res.ChangeHash(meshSet.NameHash);
                    }

                    // Change the references in the sbd
                    for (int lod = 0; lod < meshSet.Lods.Count; lod++)
                    {
                        ShaderBlockEntry sbEntry = newShaderBlockDepot.GetSectionEntry(lod);
                        ShaderBlockMeshVariationEntry sbMvEntry = newShaderBlockDepot.GetResource(sbEntry.Index + 1) as ShaderBlockMeshVariationEntry;

                        // calculate new entry hash
                        sbEntry.SetHash(meshSet.NameHash, 0, lod);
                        sbMvEntry.SetHash(meshSet.NameHash, 0, lod);

                        // Update the mesh guid
                        for (int section = 0; section < meshSet.Lods[lod].Sections.Count; section++)
                        {
                            MeshParamDbBlock mesh = sbEntry.GetMeshParams(section);
                            mesh.MeshAssetGuid = newAsset.RootInstanceGuid;
                        }
                    }

                    App.AssetManager.ModifyRes(newShaderBlock.Name, newShaderBlockDepot);

                    newResAsset.LinkAsset(newShaderBlock);
                }

                //Modify the res and ebx
                App.AssetManager.ModifyRes(newResAsset.Name, meshSet);
                App.AssetManager.ModifyEbx(newName, newAsset);

                return newEntry;
            }
        }

        #endregion

        #region --Textures--

        public class AtlasTextureExtension : DuplicateAssetExtension
        {
            public override string AssetType => "AtlasTextureAsset";

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
                AtlasTexture texture = App.AssetManager.GetResAs<AtlasTexture>(resEntry);
                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(texture.ChunkId);

                // Duplicate the chunk
                ChunkAssetEntry newChunkEntry = DuplicateChunk(chunkEntry);

                // Duplicate the res
                ResAssetEntry newResEntry = DuplicateRes(resEntry, newName, ResourceType.AtlasTexture);
                ((dynamic)newAsset.RootObject).Resource = newResEntry.ResRid;
                AtlasTexture newTexture = App.AssetManager.GetResAs<AtlasTexture>(newResEntry);

                // Set the data in the Atlas Texture
                newTexture.SetData(texture.Width, texture.Height, newChunkEntry.Id, App.AssetManager);
                newTexture.SetNameHash((uint)Utils.HashString($"Output/Win32/{newResEntry.Name}.res", true));

                // Link the newly duplicated ebx, chunk, and res entries together
                newResEntry.LinkAsset(newChunkEntry);
                newEntry.LinkAsset(newResEntry);

                // Modify ebx and res
                App.AssetManager.ModifyEbx(newEntry.Name, newAsset);
                App.AssetManager.ModifyRes(newResEntry.Name, newTexture);

                return newEntry;
            }
        }

        public class SvgImageExtension : DuplicateAssetExtension
        {
            public override string AssetType => "SvgImage";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                EbxAssetEntry refEntry = base.DuplicateAsset(entry, newName, createNew, newType);

                EbxAsset refAsset = App.AssetManager.GetEbx(refEntry);
                dynamic refRoot = refAsset.RootObject;

                ResAssetEntry resEntry = App.AssetManager.GetResEntry(refRoot.Resource);

                ResAssetEntry newResEntry = DuplicateRes(resEntry, refEntry.Name, ResourceType.SvgImage);
                if (newResEntry != null)
                {
                    refRoot.Resource = newResEntry.ResRid;
                    App.AssetManager.ModifyEbx(refEntry.Name, refAsset);
                }

                return refEntry;
            }
        }

        public class TextureExtension : DuplicateAssetExtension
        {
            public override string AssetType => "TextureBaseAsset";

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

                // Duplicate the chunk
                Guid chunkGuid = App.AssetManager.AddChunk(new NativeReader(texture.Data).ReadToEnd(), null, texture);
                ChunkAssetEntry newChunkEntry = App.AssetManager.GetChunkEntry(chunkGuid);

                // Duplicate the res
                ResAssetEntry newResEntry = App.AssetManager.AddRes(newName, ResourceType.Texture, resEntry.ResMeta, new NativeReader(App.AssetManager.GetRes(resEntry)).ReadToEnd());
                ((dynamic)newAsset.RootObject).Resource = newResEntry.ResRid;
                Texture newTexture = App.AssetManager.GetResAs<Texture>(newResEntry);
                newTexture.ChunkId = chunkGuid;
                newTexture.AssetNameHash = (uint)Utils.HashString(newResEntry.Name, true);

                // Add the new chunk/res entries to the original bundles
                newResEntry.AddedBundles.AddRange(resEntry.EnumerateBundles());
                newChunkEntry.AddedBundles.AddRange(chunkEntry.EnumerateBundles());

                // Link the newly duplicates ebx, chunk, and res entries together
                newResEntry.LinkAsset(newChunkEntry);
                newEntry.LinkAsset(newResEntry);

                // Modify ebx and res
                App.AssetManager.ModifyEbx(newEntry.Name, newAsset);
                App.AssetManager.ModifyRes(newResEntry.Name, newTexture);

                return newEntry;
            }
        }

        #endregion

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

        #endregion

        #region --Chunk and res support--

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

        #endregion

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

                        task.Update("Duplicating asset...");
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
}
