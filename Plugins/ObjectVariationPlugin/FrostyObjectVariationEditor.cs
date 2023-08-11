using System;
using System.Collections.Generic;
using System.Windows;
using Frosty.Core.Viewport;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using Frosty.Hash;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using Frosty.Core;
using MeshSetPlugin.Resources;

namespace ObjectVariationPlugin
{
    [TemplatePart(Name = PART_AssetPropertyGrid, Type = typeof(FrostyPropertyGrid))]
    public class FrostyObjectVariationEditor : FrostyAssetEditor
    {
        private const string PART_AssetPropertyGrid = "PART_AssetPropertyGrid";

        private FrostyPropertyGrid pgAsset;

        private bool firstTimeLoad = true;
        private List<ShaderBlockDepot> shaderBlockDepots = new List<ShaderBlockDepot>();
        private List<MeshVariation> meshVariations = new List<MeshVariation>();

        public FrostyObjectVariationEditor(ILogger inLogger) 
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            pgAsset = GetTemplateChild(PART_AssetPropertyGrid) as FrostyPropertyGrid;
            pgAsset.OnModified += PgAsset_OnModified;

            Loaded += FrostyObjectVariationEditor_Loaded;
        }

        private void PgAsset_OnModified(object sender, ItemModifiedEventArgs e)
        {
            for (int i = 0; i < meshVariations.Count; i++)
            {
                EbxAsset meshEbx = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(meshVariations[i].MeshGuid));
                dynamic meshObj = meshEbx.RootObject;

                // load mesh asset associated with this variation
                MeshSet meshAsset = App.AssetManager.GetResAs<MeshSet>(App.AssetManager.GetResEntry(meshObj.MeshSetResource));

                // iterate mesh lods
                for (int j = 0; j < meshAsset.Lods.Count; j++)
                {
                    var sbe = shaderBlockDepots[i].GetSectionEntry(j);

                    // iterate mesh sections
                    for (int k = 0; k < meshAsset.Lods[j].Sections.Count; k++)
                    {
                        var texturesBlock = sbe.GetTextureParams(k);
                        var paramsBlock = sbe.GetParams(k);

                        dynamic material = meshObj.Materials[meshAsset.Lods[j].Sections[k].MaterialId].Internal;
                        AssetClassGuid materialGuid = material.GetInstanceGuid();

                        // search mesh variation for appropriate variation material
                        foreach (var meshVariationMaterial in meshVariations[i].Materials)
                        {
                            if (meshVariationMaterial.MaterialGuid == materialGuid.ExportedGuid)
                            {
                                dynamic mvMaterial = Asset.GetObject(meshVariationMaterial.MaterialVariationClassGuid);
                                foreach (dynamic param in mvMaterial.Shader.BoolParameters)
                                {
                                    string paramName = param.ParameterName;
                                    bool value = param.Value;

                                    paramsBlock.SetParameterValue(paramName, value);
                                }
                                foreach (dynamic param in mvMaterial.Shader.VectorParameters)
                                {
                                    string paramName = param.ParameterName;
                                    dynamic vec = param.Value;

                                    paramsBlock.SetParameterValue(paramName, new float[] { vec.x, vec.y, vec.z, vec.w });
                                }
                                foreach (dynamic param in mvMaterial.Shader.ConditionalParameters)
                                {
                                    string value = param.Value;
                                    PointerRef assetRef = param.ConditionalAsset;

                                    if (assetRef.Type == PointerRefType.External)
                                    {
                                        EbxAsset asset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(assetRef.External.FileGuid));
                                        dynamic conditionalAsset = asset.RootObject;

                                        string conditionName = conditionalAsset.ConditionName;
                                        byte idx = (byte)conditionalAsset.Branches.IndexOf(value);

                                        paramsBlock.SetParameterValue(conditionName, idx);
                                    }
                                }
                                foreach (dynamic param in mvMaterial.Shader.TextureParameters)
                                {
                                    string paramName = param.ParameterName;
                                    PointerRef value = param.Value;

                                    texturesBlock.SetParameterValue(paramName, value.External.ClassGuid);
                                }

                                texturesBlock.IsModified = true;
                                paramsBlock.IsModified = true;

                                break;
                            }
                        }
                    }
                }

                // modify the ShaderBlockDepot
                App.AssetManager.ModifyRes(shaderBlockDepots[i].ResourceId, shaderBlockDepots[i]);
                AssetEntry.LinkAsset(App.AssetManager.GetResEntry(shaderBlockDepots[i].ResourceId));
            }
        }

        private void FrostyObjectVariationEditor_Loaded(object sender, RoutedEventArgs e)
        {
            MeshVariationDb.LoadModifiedVariations(); // make sure we load the modified variations too, that way we don't miss dupes
            if (firstTimeLoad)
            {
                if (!MeshVariationDb.IsLoaded)
                {
                    FrostyTaskWindow.Show("Loading Variations", "", (task) =>
                    {
                        MeshVariationDb.LoadVariations(task);
                    });
                }

                dynamic ebxData = RootObject;

                // store every unique mesh variation for this object variation
                foreach (MeshVariation mvEntry in MeshVariationDb.FindVariations(ebxData.NameHash))
                {
                    meshVariations.Add(mvEntry);
                }

                foreach (dynamic obj in RootObjects)
                {
                    Type objType = obj.GetType();
                    if (TypeLibrary.IsSubClassOf(objType, "MeshMaterialVariation"))
                    {
                        // use the first mesh variation to populate the texture parameters
                        // of this object variation

                        if (obj.Shader.TextureParameters.Count == 0)
                        {
                            AssetClassGuid guid = obj.GetInstanceGuid();
                            MeshVariationMaterial mm = null;

                            foreach (MeshVariationMaterial mvm in meshVariations[0].Materials)
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
                                    obj.Shader.TextureParameters.Add(param);
                            }
                        }
                    }
                }

                string path = AssetEntry.Name.ToLower();
                foreach (var mv in meshVariations)
                {
                    // using the mesh variation mesh, obtain the relevant ShaderBlockDepot
                    EbxAssetEntry ebxEntry = App.AssetManager.GetEbxEntry(mv.MeshGuid);
                    ResAssetEntry resEntry = App.AssetManager.GetResEntry(path + "/" + ebxEntry.Filename + "_" + (uint)Fnv1.HashString(ebxEntry.Name.ToLower()) + "/shaderblocks_variation/blocks");
                    if (resEntry != null)
                    {
                        shaderBlockDepots.Add(App.AssetManager.GetResAs<ShaderBlockDepot>(resEntry));
                    }
                }

                //Double check that our data isn't incorrect
                if (meshVariations.Count > 0)
                {
                    firstTimeLoad = false;
                }

                //If it is incorrect, then this data is rubbish and next time the asset is opened we should completely redo.
                else
                {
                    firstTimeLoad = true;
                    MeshVariationDb.IsLoaded = false;
                    App.Logger.LogError("Cannot find any Mesh Variation Databases which have this Object Variation. If the asset is duped, please add it to Mesh Variation Databases.");
                }
            }
        }
    }
}
