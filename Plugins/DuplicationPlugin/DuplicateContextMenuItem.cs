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
    public class DuplicationTool
    {
        #region --Extensions--
        public class BlueprintBundleExtension : DuplicateAssetExtension
        {
            public override string AssetType => "BlueprintBundle";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                // Duplicate the ebx
                EbxAssetEntry newEntry = base.DuplicateAsset(entry, newName.ToLower(), createNew, newType);

                BundleEntry bundleEntry = App.AssetManager.GetBundleEntry(entry.Bundles[0]);

                EbxAssetEntry mvdb = App.AssetManager.EnumerateEbx("MeshVariationDatabase", bundleSubPath: bundleEntry.Name).First();

                BundleEntry newBundleEntry = App.AssetManager.AddBundle("win32/" + newName.ToLower(), BundleType.BlueprintBundle, bundleEntry.SuperBundleId);
                newBundleEntry.Blueprint = newEntry;

                if (App.AssetManager.GetEbx(newName.ToLower() + "/MeshVariationDb_Win32") != null)
                {
                    EbxAssetEntry newMvdb = base.DuplicateAsset(mvdb, newName.ToLower() + "/MeshVariationDb_Win32", false, null);
                    newMvdb.AddedBundles.Clear();
                    newMvdb.AddToBundle(App.AssetManager.GetBundleId(newBundleEntry));

                    newEntry.AddedBundles.Clear();
                    newEntry.AddToBundle(App.AssetManager.GetBundleId(newBundleEntry));
                }

                return newEntry;
            }
        }

        //ToDo: this doesn't work for SWBF2, so add in support for it
        public class ObjectVariationExtension : DuplicateAssetExtension
        {
            public override string AssetType => "ObjectVariation";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                EbxAssetEntry ebxAssetEntry = base.DuplicateAsset(entry, newName, createNew, newType);
                if (ebxAssetEntry == null)
                {
                    return null;
                }
                EbxAsset ebx = App.AssetManager.GetEbx(ebxAssetEntry);
                dynamic rootObject = ebx.RootObject;

                rootObject.NameHash = (uint)Fnv1.HashString(newName.ToLower()); //Setup the name hash

                //Warnings for the user, that way they know of potential Bundle Editor conflicts
                string guidString = newName.Split('/').ToList().Last().Split('_').ToList().First(); //What the fuck is this
                bool isValid = Guid.TryParse(guidString, out Guid originalMeshGuid);
                if (!isValid) //Check if the name is valid
                {
                    App.Logger.LogWarning("For the Bundle Editor to be able to add new variations to the MeshVariationDatabase, the first part of the name needs to be the original mesh guid. For example, 0cd5f035-1737-4056-8ac0-9cf4692084b7_ShockTrooperVariation");
                }

                else if (isValid) //If it is valid, then we will copy the materials from the original mesh into here
                {
                    int Index = -1; //The material index we are currently on

                    //Find the original mesh, based off of the root instance guid
                    EbxAsset meshEbx = new EbxAsset();
                    foreach (EbxAssetEntry potentialMesh in App.AssetManager.EnumerateEbx("MeshAsset"))
                    {
                        if (App.AssetManager.GetEbx(potentialMesh).RootInstanceGuid == originalMeshGuid)
                        {
                            meshEbx = App.AssetManager.GetEbx(potentialMesh);
                            break;
                        }
                    }

                    //Clear out the variation's materials
                    List<object> list = new List<object>(ebx.Objects.ToList());
                    foreach (object matObject in list)
                    {
                        if (matObject.GetType().ToString().Split('.').ToList().Last() == "MeshMaterialVariation")
                        {
                            ebx.RemoveObject(matObject);
                        }
                    }

                    //Create new materials in the variation
                    dynamic meshProperties = meshEbx.RootObject;
                    ResAssetEntry ResEntry = App.AssetManager.GetResEntry(meshProperties.MeshSetResource);
                    MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(ResEntry);
                    foreach (PointerRef matPointer in meshProperties.Materials)
                    {
                        //Grab the mesh's material
                        dynamic meshMatProperties = matPointer.Internal as dynamic;

                        //Create a new object
                        dynamic newMaterialV = TypeLibrary.CreateObject("MeshMaterialVariation");

                        AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(ebx.Objects, (Type)newMaterialV.GetType(), ebx.FileGuid), -1);
                        newMaterialV.SetInstanceGuid(guid);

                        //Write the properties
                        Index++;
                        MeshSetSection meshSection = meshSet.Lods[0].Sections[Index];
                        //newMaterialV.__Id = ((dynamic)matPointer.Internal).GetInstanceGuid().ExportedGuid.ToString() + "_" + meshSection.Name; //Change the ID to be valid for the Bundle Editor
                        newMaterialV.Shader.TextureParameters = meshMatProperties.Shader.TextureParameters;

                        ebx.AddRootObject(newMaterialV as object);
                    }
                }

                return ebxAssetEntry;
            }
        }

        //ToDo: double check that this works for everything
        public class SubworldExtension : DuplicateAssetExtension
        {
            public override string AssetType => "SubWorldData";

            public override EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
            {
                EbxAssetEntry DuplicatedEbxEntry = base.DuplicateAsset(entry, newName, createNew, newType);
                if (DuplicatedEbxEntry == null)
                {
                    return null;
                }

                //Remove bundles that already exist
                DuplicatedEbxEntry.AddedBundles.Clear();

                //Get the original super bundle, then create a new bundle and link it to that
                BundleEntry originalBundleEntry = App.AssetManager.GetBundleEntry(entry.Bundles.FirstOrDefault());
                BundleEntry newBundleEntry = App.AssetManager.AddBundle("win32/" + newName.ToLower(), BundleType.SubLevel, originalBundleEntry.SuperBundleId);
                //Add the new asset to the new bundle
                DuplicatedEbxEntry.AddToBundle(App.AssetManager.GetBundleId(newBundleEntry));
                App.Logger.Log("Created subworld {0}, and created a new bundle {1}", DuplicatedEbxEntry.Name, newBundleEntry.DisplayName);

                return DuplicatedEbxEntry;
            }
        }

        //Idk if this actually works, I am fairly certain it does and my testing in SWBF1 suggests it does, but the FX was also broken during those tests due to unrelated technical fuck-ups
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
                newTexture.SetData(texture.Width, texture.Height, newChunkEntry.Id, App.AssetManager);

                // Link the newly duplicates ebx, chunk, and res entries together
                newResEntry.LinkAsset(newChunkEntry);
                newEntry.LinkAsset(newResEntry);

                // Modify ebx and res
                App.AssetManager.ModifyEbx(newEntry.Name, newAsset);
                App.AssetManager.ModifyRes(newResEntry.Name, newTexture);

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

                // Get the original asset root object data
                dynamic SoundObjects = newAsset.RootObject;

                // Read the original sound chunks then duplicate them
                foreach (var SoundChunk in SoundObjects.Chunks)
                {
                    //Grab the original chunk entry, dupe it
                    ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(SoundChunk.ChunkId);
                    ChunkAssetEntry NewChunkEntry = DuplicateChunk(chunkEntry);
                    SoundChunk.ChunkId = NewChunkEntry.Id;

                    //Add to bundles and link asset
                    NewChunkEntry.AddedBundles.AddRange(chunkEntry.EnumerateBundles());
                    newEntry.LinkAsset(NewChunkEntry);
                }

                App.AssetManager.ModifyEbx(newEntry.Name, newAsset);
                return newEntry;
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
                    if (lod.ChunkId != null && lod.ChunkId != Guid.Empty)
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

                //Stuff for SBDs since SWBF2 is weird
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
                {
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

        public class SvgExtension : DuplicateAssetExtension
        {
            public override string AssetType => "SvgImage";

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
                SvgImage texture = App.AssetManager.GetResAs<SvgImage>(resEntry);

                //Duplicate the res
                ResAssetEntry newResAsset = DuplicateRes(resEntry, newName, ResourceType.SvgImage);

                // Modify ebx and res
                ((dynamic)newAsset.RootObject).Resource = newResAsset.ResRid;
                newEntry.LinkAsset(newResAsset);
                App.AssetManager.ModifyEbx(newEntry.Name, newAsset);

                return newEntry;
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
            foreach (int bId in entry.Bundles)
            {
                newEntry.AddToBundle(bId);
            }

            App.Logger.Log(string.Format("Duped chunk {0} to {1}", entry.Name, newGuid));
            return newEntry;
        }

        public static ResAssetEntry DuplicateRes(ResAssetEntry entry, string name, ResourceType resType)
        {
            if (App.AssetManager.GetResEntry(name) == null)
            {
                ResAssetEntry newEntry;
                using (NativeReader reader = new NativeReader(App.AssetManager.GetRes(entry))) //ToDo: figure out why the hell this isn't adding res to bundles
                {
                    newEntry = App.AssetManager.AddRes(name, resType, entry.ResMeta, reader.ReadToEnd(), entry.EnumerateBundles().ToArray());
                }

                //Hacky workaround to fix this not actually adding to bundles
                if (newEntry.Bundles.Count == 0)
                {
                    foreach (int bId in entry.Bundles)
                    {
                        newEntry.AddToBundle(bId);
                    }
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
