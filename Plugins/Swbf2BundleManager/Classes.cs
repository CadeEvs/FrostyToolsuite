using AtlasTexturePlugin;
using Frosty.Core;
using Frosty.Core.Viewport;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;
using MeshSetPlugin.Resources;
using RootInstanceEntriesPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BundleManager
{
    //Used to find ebx, chunk and res references of "special" assets such as meshassets
    internal class AssetLogger
    {
        public AssetManager AM = App.AssetManager;
        public virtual string AssetType => null;

        public virtual AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = new AssetData() { EbxReferences = new List<EbxAssetEntry>(), Chunks = new List<ChunkAssetEntry>(), Res = new List<ResAssetEntry>() };

            data.EbxReferences = parEntry.EnumerateDependencies().Select(o => App.AssetManager.GetEbxEntry(o)).ToList();

            ResAssetEntry resNameShare = AM.GetResEntry(parEntry.Name.ToLower());
            if (resNameShare != null && !data.Res.Contains(resNameShare))
                data.Res.Add(resNameShare);

            void AddChunk(Guid chkId)
            {
                ChunkAssetEntry chkEntry = AM.GetChunkEntry(chkId);
                if (chkEntry != null && !data.Chunks.Contains(chkEntry) && (chkEntry.IsAdded || chkEntry.EnumerateBundles().ToList().Count > 0))
                    data.Chunks.Add(chkEntry);
            }
            void AddRes(ulong resId)
            {
                ResAssetEntry resEntry = AM.GetResEntry(resId);
                if (resEntry != null && !data.Res.Contains(resEntry))
                    data.Res.Add(resEntry);
            }
            void CheckList(dynamic obj)
            {
                Type listType = obj.GetType();
                if (listType.GetGenericArguments()[0].GetProperties().Length > 0)
                {
                    foreach (var item in obj)
                        CheckProperties(item);
                }
                else
                {
                    if (obj.GetType().GetGenericArguments()[0].Name == "ResourceRef")
                        foreach (uint resId in obj)
                            AddRes(resId);
                    else if (obj.GetType().GetGenericArguments()[0].Name == "Guid")
                        foreach (Guid guid in obj)
                            AddChunk(guid);
                }
            }
            List<string> exclusionTypes = new List<string>() { "__Id", "__InstanceGuid", "LinearTransform", "Vec3", "PointerRef" };
            void CheckProperties(dynamic obj)
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    string propTypeName = pi.PropertyType.Name;
                    if (exclusionTypes.Contains(propTypeName))
                        continue;
                    else if (propTypeName == "ResourceRef")
                        AddRes((ulong)pi.GetValue(obj));
                    else if (propTypeName == "Guid")
                        AddChunk((Guid)pi.GetValue(obj));
                    else if (propTypeName == "List`1")
                        CheckList(pi.GetValue(obj));
                    else if (pi.PropertyType.GetProperties().Length > 0)
                    {
                        CheckProperties(pi.PropertyType.GetProperties());
                    }
                }
            }

            foreach (dynamic obj in parAsset.Objects)
            {
                //App.Logger.Log(obj.GetType().Name);
                CheckProperties(obj);
            }
            return data;
        }
    }

    internal class AL_ClassSchematics : AssetLogger
    {
        public override string AssetType => "SchematicsAsset";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);


            dynamic parRoot = parAsset.RootObject;
            EbxAssetEntry refEntry = AM.GetEbxEntry(parRoot.InstanceType.ToString().Substring(9, parRoot.InstanceType.ToString().Length - 10));
            if (refEntry != null)
                data.EbxReferences.Add(refEntry);
            return data;
        }
    }

    internal class AL_Vur : AssetLogger
    {
        public override string AssetType => "VisualUnlockRootAsset";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);

            dynamic parRoot = parAsset.RootObject;

            if (parRoot.VisualUnlockAssets.Count == 0)
                data.EbxReferences.Clear();
            return data;
        }
    }

    internal class AL_TextureAsset : AssetLogger
    {
        public override string AssetType => "TextureAsset";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);

            foreach (ResAssetEntry resEntry in data.Res)
            {
                Texture texture = App.AssetManager.GetResAs<Texture>(resEntry);
                ChunkAssetEntry chkEntry = App.AssetManager.GetChunkEntry(texture.ChunkId);
                if (chkEntry != null)
                    data.Chunks.Add(chkEntry);
            }
            return data;
        }
    }

    internal class AL_AtlasTextureAsset : AssetLogger
    {
        public override string AssetType => "AtlasTextureAsset";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);

            foreach (ResAssetEntry resEntry in data.Res)
            {
                AtlasTexture texture = App.AssetManager.GetResAs<AtlasTexture>(resEntry);
                ChunkAssetEntry chkEntry = App.AssetManager.GetChunkEntry(texture.ChunkId);
                if (chkEntry != null)
                    data.Chunks.Add(chkEntry);
            }
            return data;
        }
    }
    internal class AL_ShaderGraph : AssetLogger
    {
        public override string AssetType => "ShaderGraph";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);

            string resBlocksName = parEntry.Name.ToLower() + "_graph/blocks";
            ResAssetEntry resBlockEntry = AM.GetResEntry(resBlocksName);

            if (resBlockEntry != null)
            {
                data.Res.Add(resBlockEntry);
                using (NativeReader reader = new NativeReader(AM.GetRes(resBlockEntry)))
                {
                    for (int idx = 72; idx < Convert.ToInt32(reader.BaseStream.Length - 12); idx = idx + 4)
                    {
                        reader.BaseStream.Position = idx;
                        Guid ReadGuid = reader.ReadGuid();
                        if (RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid) != null && RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid) != parEntry && !data.EbxReferences.Contains(RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid)))
                            data.EbxReferences.Add(RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid));
                    }
                }
            }

            return data;
        }
    }

    internal class AL_MeshAsset : AssetLogger
    {
        public override string AssetType => "MeshAsset";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);
            dynamic parRoot = parAsset.RootObject;

            ResAssetEntry MeshSetResEntry = AM.GetResEntry(parRoot.MeshSetResource);
            if (MeshSetResEntry != null)
            {
                // Blocks Resources
                string resBlocksName = parEntry.Name.ToLower() + "_mesh/blocks";
                ResAssetEntry resBlockEntry = AM.GetResEntry(resBlocksName);
                if (resBlockEntry != null)
                {
                    data.Res.Add(resBlockEntry);
                    using (NativeReader reader = new NativeReader(AM.GetRes(resBlockEntry)))
                    {
                        for (int idx = 72; idx < Convert.ToInt32(reader.BaseStream.Length - 12); idx = idx + 4)
                        {
                            reader.BaseStream.Position = idx;
                            Guid ReadGuid = reader.ReadGuid();
                            if (RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid) != null && RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid) != parEntry && !data.EbxReferences.Contains(RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid)))
                                data.EbxReferences.Add(RootInstanceEbxEntryDb.GetEbxEntryByRootInstanceGuid(ReadGuid));
                        }
                    }
                }

                //Chunks
                MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(MeshSetResEntry);
                foreach (MeshSetLod lod in meshSet.Lods)
                {
                    if (lod.ChunkId != Guid.Empty)
                    {
                        ChunkAssetEntry chkEntry = App.AssetManager.GetChunkEntry(lod.ChunkId);
                        data.Chunks.Add(chkEntry);
                    }
                }
            }

            return data;
        }
    }

    internal class AL_EmitterGraph : AssetLogger
    {
        public override string AssetType => "EmitterGraph";

        public override AssetData GetAssetData(EbxAssetEntry parEntry, EbxAsset parAsset)
        {
            AssetData data = base.GetAssetData(parEntry, parAsset);
            dynamic parRoot = parAsset.RootObject;
            foreach (EbxAssetEntry refEntry in new List<string> { parRoot.MeshVertexShaderFragmentAssetName, parRoot.VertexShaderFragmentAssetName }.Select(o => AM.GetEbxEntry(o)).ToList())
            {
                if (refEntry != null && !data.EbxReferences.Contains(refEntry) && refEntry != parEntry)
                    data.EbxReferences.Add(refEntry);
            }
            return data;
        }
    }
    public class BM_BundleData
    {
        public List<int> Parents;
        public List<EbxAssetEntry> ModifiedAssets;
    }
    public class MeshVariData
    {
        public BM_MeshVariationDatabaseEntry BM_MeshVariationDatabaseEntry;
        //public dynamic MeshVariationDatabaseEntry;
        public List<Guid> refGuids;
    }
    public class MeshVariOriginalData : MeshVariData
    {
        public Dictionary<EbxAssetEntry, int> dbLocations;
    }
    public class AssetData
    {
        public List<EbxAssetEntry> EbxReferences;
        public List<ChunkAssetEntry> Chunks;
        public List<ResAssetEntry> Res;
        public List<EbxImportReference> Objects;
        public MeshVariData meshVari;
    }

    public class BM_TextureShaderParameter
    {
        public PointerRef Value;
        public CString ParameterName;

        public BM_TextureShaderParameter(NativeReader reader, Boolean Test)
        {
            ParameterName = reader.ReadNullTerminatedString();
            Value = new PointerRef(new EbxImportReference() { FileGuid = reader.ReadGuid(), ClassGuid = reader.ReadGuid() });
        }

        public BM_TextureShaderParameter(dynamic texparam)
        {
            Value = texparam.Value;
            ParameterName = texparam.ParameterName;
        }

        public dynamic WriteToGameTexParam()
        {
            dynamic mvdbTexParam = TypeLibrary.CreateObject("TextureShaderParameter");
            mvdbTexParam.Value = Value;
            mvdbTexParam.ParameterName = ParameterName;
            return mvdbTexParam;
        }
    }

    public class BM_MeshVariationDatabaseMaterial
    {
        public PointerRef Material;
        public PointerRef MaterialVariation;
        public Int64 MaterialId;
        public Guid SurfaceShaderGuid;
        public UInt32 SurfaceShaderId;
        public List<BM_TextureShaderParameter> TextureParameters = new List<BM_TextureShaderParameter>();
        public BM_MeshVariationDatabaseMaterial(NativeReader reader, PointerRef mesh, UInt32 var)
        {
            Material = new PointerRef(new EbxImportReference() { FileGuid = mesh.External.FileGuid, ClassGuid = reader.ReadGuid() });
            if (var != 0)
                MaterialVariation = new PointerRef(new EbxImportReference() { FileGuid = reader.ReadGuid(), ClassGuid = reader.ReadGuid() });
            SurfaceShaderId = reader.ReadUInt();
            SurfaceShaderGuid = reader.ReadGuid();
            MaterialId = reader.ReadLong();
            int texParamCount = reader.ReadInt();
            for (int i = 0; i < texParamCount; i++)
                TextureParameters.Add(new BM_TextureShaderParameter(reader, true));
        }

        public BM_MeshVariationDatabaseMaterial(dynamic material)
        {
            Material = material.Material;
            MaterialVariation = material.MaterialVariation;
            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
            {
                MaterialId = material.MaterialId;
                SurfaceShaderGuid = material.SurfaceShaderGuid;
                SurfaceShaderId = material.SurfaceShaderId;
            }
            foreach (dynamic texparam in material.TextureParameters)
                TextureParameters.Add(new BM_TextureShaderParameter(texparam));
        }

        public BM_MeshVariationDatabaseMaterial(EbxAssetEntry parEntry, dynamic material)
        {
            Material = new PointerRef(new EbxImportReference() { FileGuid = parEntry.Guid, ClassGuid = material.__InstanceGuid.ExportedGuid });
            dynamic shader = material.Shader;
            if (shader.Shader.Type == PointerRefType.External)
            {
                SurfaceShaderGuid = shader.Shader.External.FileGuid;
                EbxAssetEntry shaderEntry = App.AssetManager.GetEbxEntry(shader.Shader.External.FileGuid);
                if (shaderEntry != null)
                    SurfaceShaderId = (uint)Utils.HashString(shaderEntry.Name.ToLower());
            }
            foreach (dynamic texparam in shader.TextureParameters)
                TextureParameters.Add(new BM_TextureShaderParameter(texparam));
        }

        public BM_MeshVariationDatabaseMaterial(EbxAssetEntry parEntry, dynamic material, EbxAssetEntry varEntry, dynamic variationMaterial)
        {
            Material = new PointerRef(new EbxImportReference() { FileGuid = parEntry.Guid, ClassGuid = material.__InstanceGuid.ExportedGuid });
            MaterialVariation = new PointerRef(new EbxImportReference() { FileGuid = varEntry.Guid, ClassGuid = variationMaterial.__InstanceGuid.ExportedGuid });

            dynamic shader = variationMaterial.Shader.Shader.Type == PointerRefType.External ? variationMaterial.Shader :  material.Shader;
            if (shader.Shader.Type == PointerRefType.External)
            {
                SurfaceShaderGuid = shader.Shader.External.FileGuid;

                EbxAssetEntry shaderEntry = App.AssetManager.GetEbxEntry(shader.Shader.External.FileGuid);
                if (shaderEntry != null)
                    SurfaceShaderId = (uint)Utils.HashString(shaderEntry.Name.ToLower());
            }
            foreach (dynamic texparam in variationMaterial.Shader.TextureParameters)
                TextureParameters.Add(new BM_TextureShaderParameter(texparam));
        }

        public dynamic WriteToGameMaterial()
        {
            dynamic mvdbMaterial = TypeLibrary.CreateObject("MeshVariationDatabaseMaterial");
            mvdbMaterial.Material = Material;
            mvdbMaterial.MaterialVariation = MaterialVariation;
            mvdbMaterial.MaterialId = MaterialId;
            mvdbMaterial.SurfaceShaderGuid = SurfaceShaderGuid;
            mvdbMaterial.SurfaceShaderId = SurfaceShaderId;
            foreach (BM_TextureShaderParameter BM_TexParam in TextureParameters)
                mvdbMaterial.TextureParameters.Add(BM_TexParam.WriteToGameTexParam());

            return mvdbMaterial;
        }
    }
    public class BM_MeshVariationDatabaseEntry
    {
        public PointerRef Mesh;
        public List<BM_MeshVariationDatabaseMaterial> Materials = new List<BM_MeshVariationDatabaseMaterial>();
        public UInt32 VariationAssetNameHash;

        public BM_MeshVariationDatabaseEntry(NativeReader reader, PointerRef mesh)
        {
            Mesh = mesh;
            VariationAssetNameHash = reader.ReadUInt();
            int matCount = reader.ReadInt();
            for (int i = 0; i < matCount; i++)
                Materials.Add(new BM_MeshVariationDatabaseMaterial(reader, mesh, VariationAssetNameHash));
        }
        public BM_MeshVariationDatabaseEntry(dynamic mvdbEntry) //Constructor from base game MeshVariationDatabaseEntry 
        {

            Mesh = mvdbEntry.Mesh;
            VariationAssetNameHash = mvdbEntry.VariationAssetNameHash;
            foreach (dynamic material in mvdbEntry.Materials)
                Materials.Add(new BM_MeshVariationDatabaseMaterial(material));
        }

        public BM_MeshVariationDatabaseEntry(EbxAssetEntry parEntry, EbxAsset parAsset, dynamic parRoot)
        {
            Mesh = new PointerRef(new EbxImportReference() { FileGuid = parEntry.Guid, ClassGuid = parAsset.RootInstanceGuid });
            if (TypeLibrary.IsSubClassOf(parRoot.GetType(), "MeshAsset"))
                VariationAssetNameHash = 0;

            foreach(dynamic pr in parRoot.Materials)
            {
                if (pr.Type == PointerRefType.Internal)
                    Materials.Add(new BM_MeshVariationDatabaseMaterial(parEntry, pr.Internal));
            }

        }

        public BM_MeshVariationDatabaseEntry(EbxAssetEntry meshEntry, EbxAsset meshAsset, dynamic meshRoot, EbxAssetEntry varEntry, dynamic varRoot, Dictionary<Guid, dynamic> meshSectionToVariationSection)
        {
            Mesh = new PointerRef(new EbxImportReference() { FileGuid = meshEntry.Guid, ClassGuid = meshAsset.RootInstanceGuid });
            VariationAssetNameHash = (uint)Utils.HashString(varEntry.Name.ToLower());

            foreach (dynamic pr in meshRoot.Materials)
            {
                if (pr.Type == PointerRefType.Internal)
                {
                    Materials.Add(new BM_MeshVariationDatabaseMaterial(meshEntry, pr.Internal,  varEntry, meshSectionToVariationSection[pr.Internal.__InstanceGuid.ExportedGuid]));
                }
            }
        }

        public dynamic WriteToGameEntry()
        {
            dynamic mvdbObject = TypeLibrary.CreateObject("MeshVariationDatabaseEntry");
            mvdbObject.Mesh = Mesh;
            mvdbObject.VariationAssetNameHash = VariationAssetNameHash;
            foreach(BM_MeshVariationDatabaseMaterial BM_Material in Materials)
                mvdbObject.Materials.Add(BM_Material.WriteToGameMaterial());
            return mvdbObject;
        }

        public bool CheckMeshNeedsUpdating(EbxAsset parAsset, dynamic parRoot)
        {

            if (Materials.Count != parRoot.Materials.Count)
                return true;

            foreach (BM_MeshVariationDatabaseMaterial material in Materials)
            {
                dynamic matObj = parAsset.GetObject(material.Material.External.ClassGuid);
                if (matObj == null)
                    return true;

                if (matObj.Shader.Shader.External.FileGuid != material.SurfaceShaderGuid)
                    return true;

                if (material.TextureParameters.Count != matObj.Shader.TextureParameters.Count)
                    return true;

                for (int i = 0; i < material.TextureParameters.Count; i++)
                {
                    if ((material.TextureParameters[i].ParameterName != matObj.Shader.TextureParameters[i].ParameterName)
                        || (material.TextureParameters[i].Value.External.FileGuid != matObj.Shader.TextureParameters[i].Value.External.FileGuid))
                        return true;
                }
            }
            return false;
        }

        public bool CheckVariationNeedsUpdating(EbxAsset parAsset, dynamic parRoot, EbxAsset meshAsset)
        {

            if (Materials.Count != parRoot.Materials.Count)
                return true;

            foreach (BM_MeshVariationDatabaseMaterial material in Materials)
            {
                if (material.MaterialVariation.Type != PointerRefType.External)
                    continue;
                dynamic matMesh = meshAsset.GetObject(material.Material.External.ClassGuid);
                dynamic matVar = parAsset.GetObject(material.MaterialVariation.External.ClassGuid);

                if (matMesh == null || matVar == null)
                    return true;

                if (matVar.Shader.Shader.Type != PointerRefType.External)
                {
                    if (matMesh.Shader.Shader.External.FileGuid != material.SurfaceShaderGuid)
                        return true;
                }
                else
                    if (matVar.Shader.Shader.External.FileGuid != material.SurfaceShaderGuid)
                        return true;

                if (material.TextureParameters.Count != matVar.Shader.TextureParameters.Count)
                    return true;

                for (int i = 0; i < material.TextureParameters.Count; i++)
                {
                    if ((material.TextureParameters[i].ParameterName != matVar.Shader.TextureParameters[i].ParameterName)
                        || (material.TextureParameters[i].Value.External.FileGuid != matVar.Shader.TextureParameters[i].Value.External.FileGuid))
                        return true;
                }
            }
            return false;
        }

        public List<Guid> GetReferenceGuids()
        {
            List<Guid> guids = new List<Guid>();
            if (App.AssetManager.GetEbxEntry(Mesh.External.FileGuid) != null)
                guids.Add(Mesh.External.FileGuid);
            foreach(BM_MeshVariationDatabaseMaterial material in Materials)
            {
                if (App.AssetManager.GetEbxEntry(material.Material.External.FileGuid) != null && !guids.Contains(material.Material.External.FileGuid))
                    guids.Add(material.Material.External.FileGuid);

                foreach(BM_TextureShaderParameter texParam in material.TextureParameters)
                {
                    if (App.AssetManager.GetEbxEntry(texParam.Value.External.FileGuid) != null && !guids.Contains(texParam.Value.External.FileGuid))
                        guids.Add(texParam.Value.External.FileGuid);
                }
            }

            return guids;
        }


    }
}
