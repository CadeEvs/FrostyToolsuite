using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Frosty.Core.Viewport;
using FrostySdk;
using System.Windows.Data;
using System.Globalization;
using FrostySdk.Ebx;
using Buffer = SharpDX.Direct3D11.Buffer;
using FrostySdk.Attributes;
using Frosty.Hash;
using Frosty.Core.Controls;
using Frosty.Core;
using Frosty.Core.Controls.Editors;
using Frosty.Core.Windows;
using SharpDX;
using SharpDX.Direct3D11;
using Frosty.Core.Screens;
using MeshSetPlugin.Fbx;
using System.Reflection;
using MeshSetPlugin.Screens;
using MeshSetPlugin.Render;
using MeshSetPlugin.Resources;
using MeshSetPlugin.Editors;
using Frosty.Controls;

namespace MeshSetPlugin
{
    public class MeshSetMaterialDetails
    {
        public object TextureParameters { get; set; } = new List<dynamic>();
    }

    [EbxClassMeta(EbxFieldType.Struct)]
    public class MeshSetVariationEntryDetails
    {
        [EbxFieldMeta(EbxFieldType.Pointer, "MeshVariationDatabase")]
        public PointerRef VariationDb { get; set; }
        public int Index { get; set; }
    }

    [DisplayName("Material Variation")]
    [Description("This class shows all the resources that are present on a single variation of the current mesh. This includes the materials, material parameters, textures from both the material itself and any mesh variation databases. It also shows in which mesh variation databases this variation can be found.")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class MeshSetVariationDetails
    {
        [IsHidden]
        public string Name { get; set; }
        [Editor(typeof(FrostyAutoDisableBooleanEditor))]
        [EbxFieldMeta(0x0, 0, typeof(object), false, 0)]
        public bool Preview { get; set; }
        [EbxFieldMeta(EbxFieldType.Pointer, "ObjectVariation")]
        public PointerRef Variation { get; set; }
        [EbxFieldMeta(0x02, 0, typeof(object), false, 0)]
        public MeshMaterialCollection.Container MaterialCollection { get; set; }
        [EbxFieldMeta(0x02, 0, typeof(object), true, 0)]
        public List<MeshSetVariationEntryDetails> MeshVariationDbs { get; set; } = new List<MeshSetVariationEntryDetails>();
    }

    [EbxClassMeta(EbxFieldType.Struct)]
    public class PreviewMeshData
    {
        [EbxFieldMeta(EbxFieldType.Pointer, "MeshAsset")]
        public PointerRef Mesh { get; set; }
        [EbxFieldMeta(EbxFieldType.Pointer, "ObjectVariation")]
        public PointerRef Variation { get; set; }
        [EbxFieldMeta(EbxFieldType.Struct)]
        public dynamic Transform { get; set; }
        public int MeshId = -1;
        public EbxAsset Asset;

        public PreviewMeshData()
        {
            Transform = TypeLibrary.CreateObject("LinearTransform");
        }
    }

    [EbxClassMeta(EbxFieldType.Struct)]
    public class PreviewLightData
    {
        [EbxFieldMeta(EbxFieldType.Struct)]
        public /* Vec3 */ dynamic Color { get; set; }

        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 200000.0f, 10.0f, 100.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float Intensity { get; set; } = 1000.0f;

        [EbxFieldMeta(EbxFieldType.Struct)]
        public /* LinearTransform */ dynamic Transform { get; set; }

        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 1000.0f, 1.0f, 10.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float AttenuationRadius { get; set; } = 1.0f;

        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 100.0f, 0.1f, 1.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float SphereRadius { get; set; } = 0.0f;

        public int LightId = -1;

        public PreviewLightData()
        {
            Color = TypeLibrary.CreateObject("Vec3");
            Color.x = 1.0f;
            Color.y = 1.0f;
            Color.z = 1.0f;
            Transform = TypeLibrary.CreateObject("LinearTransform");
        }
    }

    [DisplayName("SectionData")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class PreviewMeshSectionData
    {
        [IsReadOnly]
        [EbxFieldMeta(EbxFieldType.Pointer, "MeshMaterial")]
        public PointerRef MaterialId { get; set; }
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool Highlight { get; set; }
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool Visible { get; set; }
        [IsReadOnly]
        [EbxFieldMeta(EbxFieldType.Int32)]
        public int MaxBonesPerVertex { get; set; }
        [IsReadOnly]
        [EbxFieldMeta(EbxFieldType.Int32)]
        public int NumUVChannels { get; set; }
        [IsReadOnly]
        [EbxFieldMeta(EbxFieldType.Int32)]
        public int NumColorChannels { get; set; }
        [EbxFieldMeta(EbxFieldType.Array, arrayType: EbxFieldType.CString)]
        public List<CString> AdditionalChannels { get; set; } = new List<CString>();
    }

    [DisplayName("LodData")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class PreviewMeshLodData
    {
        //[EbxFieldMeta(EbxFieldType.Boolean)]
        //[Editor("FrostyAutoDisableBooleanEditor")]
        //public bool Preview { get; set; }
        [IsReadOnly]
        [EbxFieldMeta(EbxFieldType.CString)]
        public CString Name { get; set; }
        [EbxFieldMeta(EbxFieldType.Struct)]
        public List<PreviewMeshSectionData> Sections { get; set; } = new List<PreviewMeshSectionData>();
    }

    [DisplayName("Mesh Settings")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class MeshSetMeshSettings
    {
        [Category("Mesh")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        public List<PreviewMeshLodData> Lods { get; set; } = new List<PreviewMeshLodData>();
    }

    [DisplayName("Preview Settings")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class MeshSetPreviewSettings
    {
        // Lights

        [Category("Lights")]
        [DisplayName("Sun Position")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        public dynamic SunPosition { get; set; }

        [Category("Lights")]
        [DisplayName("Sun Intensity")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 2000000.0f, 100.0f, 1000.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float SunIntensity { get; set; } = 1000.0f;

        [Category("Lights")]
        [DisplayName("Sun Angular Radius")]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float SunAngularRadius { get; set; } = 0.029f;

        [Category("Lights")]
        [DisplayName("Additional Lights")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        public List<PreviewLightData> PreviewLights { get; set; } = new List<PreviewLightData>();

        // Meshes

        [Category("Meshes")]
        [DisplayName("Additional Meshes")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        public List<PreviewMeshData> PreviewMeshes { get; set; } = new List<PreviewMeshData>();

        // Scene

        [Category("Scene")]
        [DisplayName("Light Probe Texture")]
        [EbxFieldMeta(EbxFieldType.Pointer, "TextureAsset")]
        public PointerRef LightProbeTexture { get; set; }

        [Category("Scene")]
        [DisplayName("Light Probe Intensity")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 1000.0f, 0.01f, 0.1f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float LightProbeIntensity { get; set; } = 1.0f;

        [Category("Scene")]
        [DisplayName("Color Lookup Table")]
        [EbxFieldMeta(EbxFieldType.Pointer, "TextureAsset")]
        public PointerRef ColorLookupTable { get; set; }

        // Camera

        [Category("Camera")]
        [DisplayName("Speed Multiplier")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(1.0f, 8.0f, 1.0f, 1.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float CameraSpeedMultiplier { get; set; } = 1.0f;

#if FROSTY_DEVELOPER
        // Camera
        [Category("Camera (Developer)")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(-10.0f, 20.0f, 1.0f, 2.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float MinEV100 { get; set; } = 8.0f;

        [Category("Camera")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(-10.0f, 20.0f, 1.0f, 2.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float MaxEV100 { get; set; } = 20.0f;
#endif

#if FROSTY_DEVELOPER
        [Category("Meshes (Developer)")]
        [Editor(typeof(FrostyImagePathEditor))]
        [Extension("dat", "Anim Files")]
        public string Animation { get; set; } = "";
#endif

#if FROSTY_DEVELOPER
        [Category("Shadows (Developer)")]
        public int iDepthBias { get; set; } = 100;

        [Category("Shadows")]
        public float fSlopeScaledDepthBias { get; set; } = 5;

        [Category("Shadows")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 1.0f, 0.0000001f, 0.000001f, false)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float fDistanceBiasMin { get; set; } = 0.00000001f;

        [Category("Shadows")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 1.0f, 0.0000001f, 0.000001f, false)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float fDistanceBiasFactor { get; set; } = 0.00000001f;

        [Category("Shadows")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 1000000.0f, 1.0f, 10.0f, false)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float fDistanceBiasThreshold { get; set; } = 700.0f;

        [Category("Shadows")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0, 30000.0f, 0.1f, 1.0f, false)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float fDistanceBiasPower { get; set; } = 0.3f;
#endif

        public MeshSetPreviewSettings()
        {
            SunPosition = TypeLibrary.CreateObject("Vec3");
            SunPosition.x = 10.0f;
            SunPosition.y = 20.0f;
            SunPosition.z = 20.0f;
        }
    }

    #region -- Exporters --
    public class FBXExporter
    {
        private int totalExportCount = 0;
        private int currentProgress = 0;
        private int boneCount;
        private FbxGeometryConverter geomConverter;
        private bool flattenHierarchy = true;
        private bool exportSingleLod = false;
        private FrostyTaskWindow task;

        public FBXExporter(FrostyTaskWindow inTask)
        {
            task = inTask;
        }

        /// <summary>
        /// Exports the specified mesh to a FBX file
        /// </summary>
        public void ExportFBX(dynamic meshAsset, string filename, string fbxVersion, string units, bool inFlattenHierarchy, bool inExportSingleLod, string skeleton, string fileType, params MeshSet[] meshSets)
        {
            flattenHierarchy = inFlattenHierarchy;
            exportSingleLod = inExportSingleLod;
            using (FbxManager manager = new FbxManager())
            {
                FbxIOSettings settings = new FbxIOSettings(manager, FbxIOSettings.IOSROOT);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_MATERIAL, false);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_TEXTURE, false);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_GLOBAL_SETTINGS, true);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_SHAPE, true);
                manager.SetIOSettings(settings);

                FbxScene scene = new FbxScene(manager, "");
                FbxDocumentInfo sceneInfo = new FbxDocumentInfo(manager, "SceneInfo")
                {
                    Title = "Frosty FBX Exporter",
                    Subject = "Export FBX meshes from Frosty Editor",
                    OriginalApplicationVendor = "Frosty Editor",
                    OriginalApplicationName = "Frosty Editor",
                    OriginalApplicationVersion = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                    LastSavedApplicationVendor = "Frosty Editor",
                    LastSavedApplicationName = "Frosty Editor",
                    LastSavedApplicationVersion = Assembly.GetEntryAssembly().GetName().Version.ToString()
                };
                scene.SceneInfo = sceneInfo;

                geomConverter = new FbxGeometryConverter(manager);

                switch (units)
                {
                    case "Millimeters": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Millimeters); break;
                    case "Centimeters": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Centimeters); break;
                    case "Meters": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Meters); break;
                    case "Kilometers": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Kilometers); break;
                }

                totalExportCount++;
                foreach (MeshSet meshSet in meshSets)
                {
                    foreach (MeshSetLod lod in meshSet.Lods)
                    {
                        foreach (MeshSetSection section in lod.Sections)
                            totalExportCount += (section.Name != "") ? 1 : 0;
                    }
                }

                // only use the primary mesh asset to export skeleton
                List<FbxNode> boneNodes = new List<FbxNode>();
                if (meshSets[0].Lods[0].Type == MeshType.MeshType_Skinned && skeleton != "")
                {
                    // skinned mesh requires external skeleton
                    task.Update("Writing skeleton");
                    FbxNode rootNode = FBXCreateSkeleton(scene, meshAsset, skeleton, ref boneNodes);
                    scene.RootNode.AddChild(rootNode);
                }
                else if (meshSets[0].Lods[0].Type == MeshType.MeshType_Composite)
                {
                    // composite skeleton has parts defined in mesh
                    task.Update("Writing composite skeleton");
                    FbxNode rootNode = FBXCreateCompositeSkeleton(scene, meshSets[0].Lods[0].PartTransforms, ref boneNodes);
                    scene.RootNode.AddChild(rootNode);
                }

                currentProgress++;
                foreach (MeshSet meshSet in meshSets)
                {
                    foreach (MeshSetLod lod in meshSet.Lods)
                    {
                        task.Update("Writing " + lod.String03);
                        FBXCreateMesh(scene, lod, boneNodes);
                        if (exportSingleLod)
                        {
                            break;
                        }
                    }
                }

                // move composite parts
                if (meshSets[0].Type == MeshType.MeshType_Composite)
                {
                    MeshSetLod lod = meshSets[0].Lods[0];
                    for (int i = 0; i < lod.PartTransforms.Count; i++)
                    {
                        LinearTransform lt = lod.PartTransforms[i];
                        FbxNode node = boneNodes[i];

                        Matrix boneMatrix = SharpDXUtils.FromLinearTransform(lt);

                        Vector3 scale = boneMatrix.ScaleVector;
                        Vector3 translation = boneMatrix.TranslationVector;
                        Vector3 euler = SharpDXUtils.ExtractEulerAngles(boneMatrix);

                        node.LclTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                        node.LclRotation = new Vector3(euler.X, euler.Y, euler.Z);
                    }
                }

                using (FbxExporter exporter = new FbxExporter(manager, ""))
                {
                    exporter.SetFileExportVersion("FBX" + fbxVersion + "00");

                    int count = manager.IOPluginRegistry.WriterFormatCount;
                    for (int i = 0; i < count; i++)
                    {
                        //if (Manager.IOPluginRegistry.WriterIsFBX(i))
                        {
                            string desc = manager.IOPluginRegistry.GetWriterFormatDescription(i);
                            if (desc.Contains(fileType))
                            {
                                if (exporter.Initialize(filename, i, settings))
                                    exporter.Export(scene);
                                break;
                            }
                        }
                    }
                }

                geomConverter.Dispose();
            }
        }

#if FROSTY_DEVELOPER
        public void ExportFacePoseFBX(string inname, string filename, string skeleton, string fbxVersion, string fileType)
        {
            using (FbxManager manager = new FbxManager())
            {
                FbxIOSettings settings = new FbxIOSettings(manager, FbxIOSettings.IOSROOT);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_MATERIAL, false);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_TEXTURE, false);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_GLOBAL_SETTINGS, true);
                settings.SetBoolProp(FbxIOSettings.EXP_FBX_SHAPE, true);
                manager.SetIOSettings(settings);

                FbxScene scene = new FbxScene(manager, "");
                FbxDocumentInfo sceneInfo = new FbxDocumentInfo(manager, "SceneInfo")
                {
                    Title = "Frosty FBX Exporter",
                    Subject = "Export FBX meshes from Frosty Editor",
                    OriginalApplicationVendor = "Frosty Editor",
                    OriginalApplicationName = "Frosty Editor",
                    OriginalApplicationVersion = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                    LastSavedApplicationVendor = "Frosty Editor",
                    LastSavedApplicationName = "Frosty Editor",
                    LastSavedApplicationVersion = Assembly.GetEntryAssembly().GetName().Version.ToString()
                };
                scene.SceneInfo = sceneInfo;

                scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Centimeters);

                List<FbxNode> boneNodes = new List<FbxNode>();
                FbxNode rootNode = FBXCreateSkeleton(scene, null, skeleton, ref boneNodes);
                scene.RootNode.AddChild(rootNode);

                foreach (FbxNode node in boneNodes)
                {
                    if (node.Name.Contains("FacialRoot"))
                        continue;

                    using (NativeReader poseReader = new NativeReader(new FileStream(inname, FileMode.Open, FileAccess.Read)))
                    {
                        while (poseReader.Position < poseReader.Length)
                        {
                            string str = poseReader.ReadNullTerminatedString();
                            uint type = poseReader.ReadUInt();

                            Quaternion q = new Quaternion();
                            Vector3 v = new Vector3();
                            Vector3 s = new Vector3();

                            if (type == 0xE)
                            {
                                q.X = poseReader.ReadFloat();
                                q.Y = poseReader.ReadFloat();
                                q.Z = poseReader.ReadFloat();
                                q.W = poseReader.ReadFloat();
                            }
                            else if (type == 0x7a2e5497)
                            {
                                v.X = poseReader.ReadFloat();
                                v.Y = poseReader.ReadFloat();
                                v.Z = poseReader.ReadFloat();
                            }
                            else
                            {
                                s.X = poseReader.ReadFloat();
                                s.Y = poseReader.ReadFloat();
                                s.Z = poseReader.ReadFloat();
                            }

                            if (str.ToLower() == node.Name.ToLower())
                            {
                                if (type == 0x7a2e5497)
                                {
                                    node.LclTranslation = v;
                                }
                                else if (type == 0xE)
                                {
                                    Vector3 euler = SharpDXUtils.ExtractEulerAngles(Matrix.RotationQuaternion(q));
                                    node.LclRotation = euler;
                                }
                                else
                                {
                                    node.LclScaling = s;
                                }
                            }
                        }
                    }
                }

                using (FbxExporter exporter = new FbxExporter(manager, ""))
                {
                    exporter.SetFileExportVersion("FBX" + fbxVersion + "00");

                    int count = manager.IOPluginRegistry.WriterFormatCount;
                    for (int i = 0; i < count; i++)
                    {
                        //if (Manager.IOPluginRegistry.WriterIsFBX(i))
                        {
                            string desc = manager.IOPluginRegistry.GetWriterFormatDescription(i);
                            if (desc.Contains(fileType))
                            {
                                if (exporter.Initialize(filename, i, settings))
                                    exporter.Export(scene);
                                break;
                            }
                        }
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Creates a skeleton from a list of composite part transforms
        /// </summary>
        private FbxNode FBXCreateCompositeSkeleton(FbxScene scene, List<LinearTransform> partTransforms, ref List<FbxNode> boneNodes)
        {
            int partCount = 0;

            // root part (not used in skinning)
            FbxSkeleton skeletonAttribute = new FbxSkeleton(scene, "ROOT");
            skeletonAttribute.SetSkeletonType(FbxSkeleton.EType.eRoot);
            skeletonAttribute.Size = 1.0;

            FbxNode rootNode = new FbxNode(scene, "ROOT");
            rootNode.SetNodeAttribute(skeletonAttribute);

            Matrix boneMatrix = Matrix.Identity;

            Vector3 scale = boneMatrix.ScaleVector;
            Vector3 translation = boneMatrix.TranslationVector;
            Vector3 euler = SharpDXUtils.ExtractEulerAngles(boneMatrix);

            rootNode.LclTranslation = new Vector3(translation.X, translation.Y, translation.Z);
            rootNode.LclRotation = new Vector3(euler.X, euler.Y, euler.Z);

            foreach (LinearTransform lt in partTransforms)
            {
                string name = "PART_" + partCount;

                skeletonAttribute = new FbxSkeleton(scene, name);
                skeletonAttribute.SetSkeletonType(FbxSkeleton.EType.eLimbNode);
                skeletonAttribute.Size = 1.0;

                FbxNode boneNode = new FbxNode(scene, name);
                boneNode.SetNodeAttribute(skeletonAttribute);

                // parts are skinned to identity then moved
                boneMatrix = Matrix.Identity;

                scale = boneMatrix.ScaleVector;
                translation = boneMatrix.TranslationVector;
                euler = SharpDXUtils.ExtractEulerAngles(boneMatrix);

                boneNode.LclTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                boneNode.LclRotation = new Vector3(euler.X, euler.Y, euler.Z);

                boneNodes.Add(boneNode);
                rootNode.AddChild(boneNode);

                partCount++;
            }

            return rootNode;
        }
            
        /// <summary>
        /// Creates a skeleton from a mesh and skeleton resource
        /// </summary>
        private FbxNode FBXCreateSkeleton(FbxScene scene, dynamic meshAsset, string skeleton, ref List<FbxNode> boneNodes)
        {
            dynamic skeletonAsset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(skeleton)).RootObject;
            boneCount = skeletonAsset.BoneNames.Count;

            for (int boneIdx = 0; boneIdx < boneCount; boneIdx++)
            {
                dynamic pose = skeletonAsset.LocalPose;
                Matrix boneMatrix = new Matrix(
                    pose[boneIdx].right.x, pose[boneIdx].right.y, pose[boneIdx].right.z, 0.0f,
                    pose[boneIdx].up.x, pose[boneIdx].up.y, pose[boneIdx].up.z, 0.0f,
                    pose[boneIdx].forward.x, pose[boneIdx].forward.y, pose[boneIdx].forward.z, 0.0f,
                    pose[boneIdx].trans.x, pose[boneIdx].trans.y, pose[boneIdx].trans.z, 1.0f
                    );

                Vector3 scale = boneMatrix.ScaleVector;
                Vector3 translation = boneMatrix.TranslationVector;
                Vector3 euler = SharpDXUtils.ExtractEulerAngles(boneMatrix);

                FbxSkeleton skeletonAttribute = new FbxSkeleton(scene, skeletonAsset.BoneNames[boneIdx]);
                skeletonAttribute.SetSkeletonType((boneIdx == 0) ? FbxSkeleton.EType.eRoot : FbxSkeleton.EType.eLimbNode);
                skeletonAttribute.Size = 1.0;

                FbxNode boneNode = new FbxNode(scene, skeletonAsset.BoneNames[boneIdx]);
                boneNode.SetNodeAttribute(skeletonAttribute);

                boneNode.LclTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                boneNode.LclRotation = new Vector3(euler.X, euler.Y, euler.Z);


                if (boneIdx != 0)
                {
                    int parentIdx = skeletonAsset.Hierarchy[boneIdx];
                    boneNodes[parentIdx].AddChild(boneNode);
                }

                boneNodes.Add(boneNode);
            }

            if ((ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville) || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons && meshAsset != null)
            {
                int procIndex = 0;
                dynamic skinnedProcAnim = meshAsset.SkinnedProceduralAnimation;

                foreach (dynamic bone in skinnedProcAnim.Bones)
                {
                    string boneName = "PROC_Bone" + (procIndex++).ToString();

                    FbxNode parentBone = boneNodes[bone.ParentIndex];
                    Matrix boneMatrix = new Matrix(
                        bone.Pose.right.x, bone.Pose.right.y, bone.Pose.right.z, 0.0f,
                        bone.Pose.up.x, bone.Pose.up.y, bone.Pose.up.z, 0.0f,
                        bone.Pose.forward.x, bone.Pose.forward.y, bone.Pose.forward.z, 0.0f,
                        bone.Pose.trans.x, bone.Pose.trans.y, bone.Pose.trans.z, 1.0f
                    );

                    Vector3 scale = boneMatrix.ScaleVector;
                    Vector3 translation = boneMatrix.TranslationVector;
                    Vector3 euler = SharpDXUtils.ExtractEulerAngles(boneMatrix);

                    FbxSkeleton skeletonAttribute = new FbxSkeleton(scene, boneName);
                    skeletonAttribute.SetSkeletonType(FbxSkeleton.EType.eLimbNode);
                    skeletonAttribute.Size = 1.0;

                    FbxNode procBoneNode = new FbxNode(scene, boneName);
                    procBoneNode.SetNodeAttribute(skeletonAttribute);

                    procBoneNode.LclTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                    procBoneNode.LclRotation = new Vector3(euler.X, euler.Y, euler.Z);

                    parentBone.AddChild(procBoneNode);
                    boneNodes.Add(procBoneNode);
                }
            }

            return boneNodes[0];
        }

        /// <summary>
        /// Creates the FBX mesh
        /// </summary>
        private void FBXCreateMesh(FbxScene scene, MeshSetLod lod, List<FbxNode> boneNodes)
        {
            int indexSize = (lod.IndexUnitSize / 8);

            FbxNode meshNode = (flattenHierarchy) 
                ? scene.RootNode 
                : new FbxNode(scene, lod.String03);

            foreach (MeshSetSection section in lod.Sections)
            {
                if (section.Name == "")
                    continue;

                task.Update(progress: (currentProgress++ / (double)totalExportCount) * 100.0);

                Stream chunkStream = (lod.ChunkId != Guid.Empty)
                    ? App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(lod.ChunkId))
                    : new MemoryStream(lod.InlineData);

                using (NativeReader reader = new NativeReader(chunkStream))
                {
                    FbxNode actor = FBXExportSubObject(scene, section, lod.VertexBufferSize, indexSize, reader);
                    if (flattenHierarchy)
                        actor.Name = $"{section.Name}:{lod.String03.Insert(lod.String03.Length - 1, ".00")}";
                    meshNode.AddChild(actor);

                    if ((lod.Type == MeshType.MeshType_Skinned || lod.Type == MeshType.MeshType_Composite) && boneNodes.Count > 0)
                    {
                        List<ushort> boneList = section.BoneList;
                        if (lod.Type == MeshType.MeshType_Composite)
                        {
                            if (lod.PartTransforms.Count != 0)
                            {
                                boneList = new List<ushort>();
                                for (ushort i = 0; i < lod.PartTransforms.Count; i++)
                                    boneList.Add(i);
                            }
                        }

                        // fifa doesnt appear to use the sub bone list in mesh sections (directly maps to indices)
                        if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 
                            || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20)
                        {
                            boneList.Clear();
                            for (ushort i = 0; i < boneNodes.Count; i++)
                                boneList.Add(i);
                        }

                        FBXCreateSkin(scene, section, actor, boneNodes, boneList, lod.Type, reader);
                        FBXCreateBindPose(scene, section, actor);
                    }
                }
            }
            if (!flattenHierarchy)
                scene.RootNode.AddChild(meshNode);
        }

        /// <summary>
        /// Creates the skin modifier for the specified section
        /// </summary>
        private void FBXCreateSkin(FbxScene scene, MeshSetSection section, FbxNode actor, List<FbxNode> boneNodes, List<ushort> boneList, MeshType meshType, NativeReader reader)
        {
            FbxMesh meshAttribute = new FbxMesh(actor.GetNodeAttribute(FbxNodeAttribute.EType.eMesh));
            FbxSkin skin = new FbxSkin(scene, "");

            List<FbxCluster> boneClusters = new List<FbxCluster>();
            for (int i = 0; i < section.VertexCount; i++)
            {
                ushort[] boneIndices = new ushort[8];
                float[] boneWeights = new float[8];

                int totalStride = 0;
                foreach (GeometryDeclarationDesc.Stream stream in section.GeometryDeclDesc[0].Streams)
                {
                    if (stream.VertexStride == 0)
                        continue;

                    int currentStride = 0;
                    foreach (GeometryDeclarationDesc.Element elem in section.GeometryDeclDesc[0].Elements)
                    {
                        if (currentStride >= totalStride && currentStride < (totalStride + stream.VertexStride))
                        {
                            if (elem.Usage == VertexElementUsage.BoneIndices || elem.Usage == VertexElementUsage.BoneIndices2 || elem.Usage == VertexElementUsage.BoneWeights || elem.Usage == VertexElementUsage.BoneWeights2)
                            {
                                reader.Position = section.VertexOffset + (totalStride * section.VertexCount) + (i * stream.VertexStride) + (currentStride - totalStride);
                                if (elem.Usage == VertexElementUsage.BoneIndices)
                                {
                                    if (elem.Format == VertexElementFormat.Byte4 || elem.Format == VertexElementFormat.Byte4N || elem.Format == VertexElementFormat.UByte4 || elem.Format == VertexElementFormat.UByte4N)
                                    {
                                        boneIndices[3] = reader.ReadByte();
                                        boneIndices[2] = reader.ReadByte();
                                        boneIndices[1] = reader.ReadByte();
                                        boneIndices[0] = reader.ReadByte();
                                    }
                                    else if (elem.Format == VertexElementFormat.UShort4 || elem.Format == VertexElementFormat.UShort4N)
                                    {
                                        boneIndices[3] = reader.ReadUShort();
                                        boneIndices[2] = reader.ReadUShort();
                                        boneIndices[1] = reader.ReadUShort();
                                        boneIndices[0] = reader.ReadUShort();
                                    }
                                }
                                else if (elem.Usage == VertexElementUsage.BoneIndices2)
                                {
                                    if (elem.Format == VertexElementFormat.Byte4 || elem.Format == VertexElementFormat.Byte4N || elem.Format == VertexElementFormat.UByte4 || elem.Format == VertexElementFormat.UByte4N)
                                    {
                                        boneIndices[7] = reader.ReadByte();
                                        boneIndices[6] = reader.ReadByte();
                                        boneIndices[5] = reader.ReadByte();
                                        boneIndices[4] = reader.ReadByte();
                                    }
                                    else if (elem.Format == VertexElementFormat.UShort4 || elem.Format == VertexElementFormat.UShort4N)
                                    {
                                        boneIndices[7] = reader.ReadUShort();
                                        boneIndices[6] = reader.ReadUShort();
                                        boneIndices[5] = reader.ReadUShort();
                                        boneIndices[4] = reader.ReadUShort();
                                    }
                                }
                                else if (elem.Usage == VertexElementUsage.BoneWeights)
                                {
                                    boneWeights[3] = reader.ReadByte() / 255.0f;
                                    boneWeights[2] = reader.ReadByte() / 255.0f;
                                    boneWeights[1] = reader.ReadByte() / 255.0f;
                                    boneWeights[0] = reader.ReadByte() / 255.0f;
                                }
                                else if (elem.Usage == VertexElementUsage.BoneWeights2)
                                {
                                    boneWeights[7] = reader.ReadByte() / 255.0f;
                                    boneWeights[6] = reader.ReadByte() / 255.0f;
                                    boneWeights[5] = reader.ReadByte() / 255.0f;
                                    boneWeights[4] = reader.ReadByte() / 255.0f;
                                }
                            }
                        }

                        currentStride += elem.Size;
                    }

                    totalStride += stream.VertexStride;
                }

                if (meshType == MeshType.MeshType_Composite)
                    boneWeights[0] = 1.0f;

                for (int j = 0; j < 8; j++)
                {
                    if (boneWeights[j] > 0.0f)
                    {
                        int subIndex = boneIndices[j];
                        if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield5 && ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                            subIndex = boneList[subIndex];

                        // account for proc bones
                        if ((subIndex & 0x8000) != 0)
                            subIndex = (subIndex - 0x8000) + boneCount;

                        while (subIndex >= boneClusters.Count)
                            boneClusters.Add(null);

                        if (boneClusters[subIndex] == null)
                        {
                            // add new cluster
                            boneClusters[subIndex] = new FbxCluster(scene, "");
                            boneClusters[subIndex].SetLink(boneNodes[subIndex]);
                            boneClusters[subIndex].SetLinkMode(FbxCluster.ELinkMode.eTotalOne);

                            FbxAMatrix linkMatrix = boneNodes[subIndex].EvaluateGlobalTransform();
                            boneClusters[subIndex].SetTransformLinkMatrix(linkMatrix);

                            skin.AddCluster(boneClusters[subIndex]);
                        }

                        boneClusters[subIndex].AddControlPointIndex(i, boneWeights[j]);
                    }
                }
            }

            meshAttribute.AddDeformer(skin);
        }

        /// <summary>
        /// Creates the bind pose for the specified section
        /// </summary>
        private void FBXCreateBindPose(FbxScene scene, MeshSetSection section, FbxNode actor)
        {
            List<FbxNode> totalNodes = new List<FbxNode>();
            FbxMesh geomAttribute = new FbxMesh(actor.GetNodeAttribute(FbxNodeAttribute.EType.eMesh));

            int skinCount = geomAttribute.GetDeformerCount(FbxDeformer.EDeformerType.eSkin);
            int clusterCount = 0;

            for (int i = 0; i < skinCount; i++)
            {
                FbxSkin skin = new FbxSkin(geomAttribute.GetDeformer(i, FbxDeformer.EDeformerType.eSkin));
                clusterCount += skin.ClusterCount;
            }

            if (clusterCount > 0)
            {
                for (int i = 0; i < skinCount; i++)
                {
                    FbxSkin skin = new FbxSkin(geomAttribute.GetDeformer(i, FbxDeformer.EDeformerType.eSkin));
                    for (int j = 0; j < skin.ClusterCount; j++)
                    {
                        FbxNode link = skin.GetCluster(j).GetLink();
                        FBXAddNodeRecursively(totalNodes, link);
                    }
                }

                totalNodes.Add(actor);
            }

            if (totalNodes.Count > 0)
            {
                FbxPose fbxPose = new FbxPose(scene, section.Name) {IsBindPose = true};

                for (int i = 0; i < totalNodes.Count; i++)
                {
                    FbxNode node = totalNodes[i];
                    FbxMatrix linkMatrix = new FbxMatrix(node.EvaluateGlobalTransform());

                    fbxPose.Add(node, linkMatrix);
                }

                scene.AddPose(fbxPose);
            }
        }

        /// <summary>
        /// Turns mesh data into FBX export data
        /// </summary>
        private FbxNode FBXExportSubObject(FbxScene scene, MeshSetSection section, long indicesOffset, int indexSize, NativeReader reader)
        {
            const int MAX_CHANNELS = 15;

            FbxNode actor = new FbxNode(scene, section.Name);
            FbxMesh fmesh = new FbxMesh(scene, section.Name);

            FbxLayerElementNormal layerElemNormal = null;
            FbxLayerElementTangent layerElemTangent = null;
            FbxLayerElementBinormal layerElemBinormal = null;
            FbxLayerElementVertexColor[] layerElemVertexColor = new FbxLayerElementVertexColor[MAX_CHANNELS];
            FbxLayerElementUV[] layerElemUV = new FbxLayerElementUV[MAX_CHANNELS];

            Dictionary<VertexElementUsage, int> uvMapping = new Dictionary<VertexElementUsage, int>();
            Dictionary<VertexElementUsage, int> colorMapping = new Dictionary<VertexElementUsage, int>();

            reader.Position = section.VertexOffset;
            fmesh.InitControlPoints((int)section.VertexCount);

            IntPtr buffer = fmesh.GetControlPoints();
            int uvChannelIndex = 0;
            int colorChannelIndex = 0;
            int totalStride = 0;

            bool packedBinormal = false;
            bool tangentSpaceUnpack = false;

            Vector3 normal = new Vector3();
            Vector3 tangent = new Vector3();
            Vector3 binormal = new Vector3();

            List<float> binormalSigns = new List<float>();
            List<object> tangentSpace = new List<object>();

            foreach (GeometryDeclarationDesc.Stream stream in section.GeometryDeclDesc[0].Streams)
            {
                if (stream.VertexStride == 0)
                    continue;

                for (int i = 0; i < section.VertexCount; i++)
                {
                    int currentStride = 0;
                    foreach (GeometryDeclarationDesc.Element elem in section.GeometryDeclDesc[0].Elements)
                    {
                        if (elem.Usage == VertexElementUsage.Unknown)
                            continue;

                        if (currentStride >= totalStride && currentStride < (totalStride + stream.VertexStride))
                        {
                            if (elem.Usage == VertexElementUsage.Pos)
                            {
                                unsafe
                                {
                                    double* ptr = (double*)(buffer);
                                    if (elem.Format == VertexElementFormat.Float3)
                                    {
                                        ptr[(i * 4) + 0] = reader.ReadFloat();
                                        ptr[(i * 4) + 1] = reader.ReadFloat();
                                        ptr[(i * 4) + 2] = reader.ReadFloat();
                                    }
                                    else if (elem.Format == VertexElementFormat.Half3 || elem.Format == VertexElementFormat.Half4)
                                    {
                                        ptr[(i * 4) + 0] = HalfUtils.Unpack(reader.ReadUShort());
                                        ptr[(i * 4) + 1] = HalfUtils.Unpack(reader.ReadUShort());
                                        ptr[(i * 4) + 2] = HalfUtils.Unpack(reader.ReadUShort());

                                        if (elem.Format == VertexElementFormat.Half4)
                                            reader.ReadUShort(); // most likely packed TBN
                                    }
                                }
                            }
                            else if (elem.Usage == VertexElementUsage.BinormalSign)
                            {
                                if (elem.Format == VertexElementFormat.Half4 || elem.Format == VertexElementFormat.Float4)
                                {
                                    if (layerElemTangent == null)
                                    {
                                        layerElemTangent = new FbxLayerElementTangent(fmesh, "")
                                        {
                                            MappingMode = EMappingMode.eByControlPoint,
                                            ReferenceMode = EReferenceMode.eDirect
                                        };
                                    }

                                    if (elem.Format == VertexElementFormat.Half4)
                                    {
                                        tangent.X = -HalfUtils.Unpack(reader.ReadUShort());
                                        tangent.Y = -HalfUtils.Unpack(reader.ReadUShort());
                                        tangent.Z = -HalfUtils.Unpack(reader.ReadUShort());
                                        binormalSigns.Add(HalfUtils.Unpack(reader.ReadUShort()));
                                    }
                                    else
                                    {
                                        tangent.X = -reader.ReadFloat();
                                        tangent.Y = -reader.ReadFloat();
                                        tangent.Z = -reader.ReadFloat();
                                        binormalSigns.Add(reader.ReadFloat());
                                    }
                                }
                                else if (elem.Format == VertexElementFormat.Float)
                                {
                                    binormalSigns.Add(reader.ReadFloat());
                                }
                                else
                                {
                                    binormalSigns.Add(HalfUtils.Unpack(reader.ReadUShort()));
                                }
                                packedBinormal = true;
                            }
                            else if (elem.Usage == VertexElementUsage.Normal)
                            {
                                if (layerElemNormal == null)
                                {
                                    layerElemNormal = new FbxLayerElementNormal(fmesh, "")
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };
                                }

                                normal.X = HalfUtils.Unpack(reader.ReadUShort());
                                normal.Y = HalfUtils.Unpack(reader.ReadUShort());
                                normal.Z = HalfUtils.Unpack(reader.ReadUShort());

                                layerElemNormal.DirectArray.Add(normal.X, normal.Y, normal.Z);
                                binormal.Y = HalfUtils.Unpack(reader.ReadUShort());
                            }
                            else if (elem.Usage == VertexElementUsage.Binormal)
                            {
                                if (layerElemBinormal == null)
                                {
                                    layerElemBinormal = new FbxLayerElementBinormal(fmesh, "")
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };
                                }

                                binormal.X = HalfUtils.Unpack(reader.ReadUShort());
                                binormal.Y = HalfUtils.Unpack(reader.ReadUShort());
                                binormal.Z = HalfUtils.Unpack(reader.ReadUShort());

                                layerElemBinormal.DirectArray.Add(binormal.X, binormal.Y, binormal.Z);
                                reader.ReadUShort();
                            }
                            else if (elem.Usage == VertexElementUsage.Tangent)
                            {
                                if (layerElemTangent == null)
                                {
                                    layerElemTangent = new FbxLayerElementTangent(fmesh, "")
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };
                                }

                                tangent.X = -HalfUtils.Unpack(reader.ReadUShort());
                                tangent.Y = -HalfUtils.Unpack(reader.ReadUShort());
                                tangent.Z = -HalfUtils.Unpack(reader.ReadUShort());

                                layerElemTangent.DirectArray.Add(tangent.X, tangent.Y, tangent.Z);
                                binormal.Z = HalfUtils.Unpack(reader.ReadUShort());
                            }
                            else if (elem.Usage == VertexElementUsage.TangentSpace)
                            {
                                if (layerElemTangent == null)
                                {
                                    layerElemNormal = new FbxLayerElementNormal(fmesh, "")
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };

                                    layerElemTangent = new FbxLayerElementTangent(fmesh, "")
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };

                                    layerElemBinormal = new FbxLayerElementBinormal(fmesh, "")
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };
                                }

                                if (elem.Format == VertexElementFormat.UByte4N)
                                {
                                    // axis angle compact
                                    tangentSpace.Add(new Vector4(reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f));
                                }
                                else if (elem.Format == VertexElementFormat.UShort4N)
                                {
                                    // axis angle precise
                                    tangentSpace.Add(new Vector4(reader.ReadUShort() / 65535.0f, reader.ReadUShort() / 65535.0f, reader.ReadUShort() / 65535.0f, reader.ReadUShort() / 65535.0f));
                                }
                                else if (elem.Format == VertexElementFormat.UInt)
                                {
                                    // quaternion
                                    tangentSpace.Add(reader.ReadUInt());
                                }
                                tangentSpaceUnpack = true;
                            }
                            else if (elem.Usage >= VertexElementUsage.TexCoord0 && elem.Usage <= VertexElementUsage.TexCoord7)
                            {
                                if (!uvMapping.ContainsKey(elem.Usage))
                                {
                                    layerElemUV[uvChannelIndex] = new FbxLayerElementUV(fmesh, "UVChannel" + uvChannelIndex)
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };

                                    uvMapping.Add(elem.Usage, uvChannelIndex);
                                    uvChannelIndex++;
                                }

                                float x, y;
                                if (elem.Format == VertexElementFormat.Float2)
                                {
                                    x = reader.ReadFloat();
                                    y = 1.0f - reader.ReadFloat();
                                }
                                else
                                {
                                    x = HalfUtils.Unpack(reader.ReadUShort());
                                    y = 1.0f - HalfUtils.Unpack(reader.ReadUShort());
                                }

                                layerElemUV[uvMapping[elem.Usage]].DirectArray.Add(x, y);
                            }
                            else if (elem.Usage >= VertexElementUsage.Color0 && elem.Usage <= VertexElementUsage.Color1)
                            {
                                if (!colorMapping.ContainsKey(elem.Usage))
                                {
                                    layerElemVertexColor[colorChannelIndex] = new FbxLayerElementVertexColor(fmesh, "VColor" + (colorChannelIndex + 1))
                                    {
                                        MappingMode = EMappingMode.eByControlPoint,
                                        ReferenceMode = EReferenceMode.eDirect
                                    };

                                    colorMapping.Add(elem.Usage, colorChannelIndex);
                                    colorChannelIndex++;
                                }

                                layerElemVertexColor[colorMapping[elem.Usage]].DirectArray.Add(
                                    reader.ReadByte() / 255.0,
                                    reader.ReadByte() / 255.0,
                                    reader.ReadByte() / 255.0,
                                    reader.ReadByte() / 255.0
                                    );
                            }
                            else if (elem.Usage == VertexElementUsage.RadiosityTexCoord)
                            {
                                // ignore (unused)
                                reader.Position += elem.Size;
                            }
                            else if (elem.Usage == VertexElementUsage.DisplacementMapTexCoord)
                            {
                                // ignore (generated in editor)
                                reader.Position += elem.Size;
                            }
                            else
                            {
                                string channelName = elem.Usage.ToString();

                                // handle all other UV cases
                                if (elem.Format == VertexElementFormat.Half2 || elem.Format == VertexElementFormat.Float2 || elem.Format == VertexElementFormat.Short2N)
                                {
                                    if (!uvMapping.ContainsKey(elem.Usage))
                                    {
                                        layerElemUV[uvChannelIndex] = new FbxLayerElementUV(fmesh, channelName)
                                        {
                                            MappingMode = EMappingMode.eByControlPoint,
                                            ReferenceMode = EReferenceMode.eDirect
                                        };

                                        uvMapping.Add(elem.Usage, uvChannelIndex);
                                        uvChannelIndex++;
                                    }

                                    float x = 0.0f;
                                    float y = 0.0f;

                                    if (elem.Format == VertexElementFormat.Half2)
                                    {
                                        x = HalfUtils.Unpack(reader.ReadUShort());
                                        y = HalfUtils.Unpack(reader.ReadUShort());
                                    }
                                    else if (elem.Format == VertexElementFormat.Float2)
                                    {
                                        x = reader.ReadFloat();
                                        y = reader.ReadFloat();
                                    }
                                    else
                                    {
                                        x = (reader.ReadShort() / (float)short.MaxValue) * 0.5f + 0.5f;
                                        y = (reader.ReadShort() / (float)short.MaxValue) * 0.5f + 0.5f;
                                    }

                                    layerElemUV[uvMapping[elem.Usage]].DirectArray.Add(x, y);
                                }

                                // handle all other vcolor cases
                                else if (elem.Format == VertexElementFormat.Half4 || elem.Format == VertexElementFormat.Float4)
                                {
                                    if (!colorMapping.ContainsKey(elem.Usage))
                                    {
                                        layerElemVertexColor[colorChannelIndex] = new FbxLayerElementVertexColor(fmesh, channelName)
                                        {
                                            MappingMode = EMappingMode.eByControlPoint,
                                            ReferenceMode = EReferenceMode.eDirect
                                        };

                                        colorMapping.Add(elem.Usage, colorChannelIndex);
                                        colorChannelIndex++;
                                    }

                                    float x = 0.0f;
                                    float y = 0.0f;
                                    float z = 0.0f;
                                    float w = 0.0f;

                                    if (elem.Format == VertexElementFormat.Half4)
                                    {
                                        x = HalfUtils.Unpack(reader.ReadUShort());
                                        y = HalfUtils.Unpack(reader.ReadUShort());
                                        z = HalfUtils.Unpack(reader.ReadUShort());
                                        w = HalfUtils.Unpack(reader.ReadUShort());
                                    }
                                    else
                                    {
                                        x = reader.ReadFloat();
                                        y = reader.ReadFloat();
                                        z = reader.ReadFloat();
                                        w = reader.ReadFloat();
                                    }

                                    layerElemVertexColor[colorMapping[elem.Usage]].DirectArray.Add(x, y, z, w);
                                }

                                // BlendWeights
                                else if (elem.Format == VertexElementFormat.Float)
                                {
                                    if (!colorMapping.ContainsKey(elem.Usage))
                                    {
                                        layerElemVertexColor[colorChannelIndex] = new FbxLayerElementVertexColor(fmesh, channelName)
                                        {
                                            MappingMode = EMappingMode.eByControlPoint,
                                            ReferenceMode = EReferenceMode.eDirect
                                        };

                                        colorMapping.Add(elem.Usage, colorChannelIndex);
                                        colorChannelIndex++;
                                    }

                                    float x = reader.ReadFloat();
                                    layerElemVertexColor[colorMapping[elem.Usage]].DirectArray.Add(x, 0, 0, 0);
                                }

                                // RegionIds
                                else if (elem.Format == VertexElementFormat.Int)
                                {
                                    if (!colorMapping.ContainsKey(elem.Usage))
                                    {
                                        layerElemVertexColor[colorChannelIndex] = new FbxLayerElementVertexColor(fmesh, channelName)
                                        {
                                            MappingMode = EMappingMode.eByControlPoint,
                                            ReferenceMode = EReferenceMode.eDirect
                                        };

                                        colorMapping.Add(elem.Usage, colorChannelIndex);
                                        colorChannelIndex++;
                                    }

                                    int x = reader.ReadInt();
                                    layerElemVertexColor[colorMapping[elem.Usage]].DirectArray.Add(x / 255.0f, 0, 0, 0);
                                }
                                else
                                    reader.Position += elem.Size;
                            }
                        }

                        currentStride += elem.Size;
                    }
                }

                totalStride += stream.VertexStride;
            }

            if (packedBinormal)
            {
                // calculate binormal using the normal/tangent and binormal sign
                layerElemBinormal = new FbxLayerElementBinormal(fmesh, "")
                {
                    MappingMode = EMappingMode.eByControlPoint,
                    ReferenceMode = EReferenceMode.eDirect
                };

                Vector4 n;
                Vector4 t;
                for (int i = 0; i < section.VertexCount; i++)
                {
                    layerElemNormal.DirectArray.GetAt(i, out n);
                    layerElemTangent.DirectArray.GetAt(i, out t);

                    normal = new Vector3(n.X, n.Y, n.Z);
                    tangent = new Vector3(t.X, t.Y, t.Z);

                    binormal = Vector3.Cross(normal, tangent) * binormalSigns[i];
                    layerElemBinormal.DirectArray.Add(binormal.X, binormal.Y, binormal.Z);
                }
            }

            if (tangentSpaceUnpack)
            {
                string shaderName = "UnpackAxisAngle";
                int inputSize = tangentSpace.Count * 4 * 4;

                if (tangentSpace[0] is uint)
                {
                    // quaternion packed
                    shaderName = "UnpackQuaternion";
                    inputSize = tangentSpace.Count * 4;
                }

                // use compute shader to unpack tangent space to TBN
                Device device = FrostyDeviceManager.Current.GetDevice();
                ComputeShader cs = FrostyShaderDb.GetShader<ComputeShader>(device, shaderName);
                Buffer inputBuffer = new Buffer(device, new BufferDescription()
                {
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    SizeInBytes = inputSize,
                    StructureByteStride = 4,
                    Usage = ResourceUsage.Default
                });
                Buffer outputBuffer = new Buffer(device, new BufferDescription()
                {
                    BindFlags = BindFlags.UnorderedAccess,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    SizeInBytes = tangentSpace.Count * 3 * 4 * 4,
                    StructureByteStride = 4 * 4,
                    Usage = ResourceUsage.Default
                });
                Buffer stagingBuffer = new Buffer(device, new BufferDescription()
                {
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    Usage = ResourceUsage.Staging,
                    SizeInBytes = tangentSpace.Count * 3 * 4 * 4,
                    StructureByteStride = 4 * 4
                });
                ShaderResourceView u0 = new ShaderResourceView(device, inputBuffer, new ShaderResourceViewDescription()
                {
                    Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.ExtendedBuffer,
                    BufferEx = new ShaderResourceViewDescription.ExtendedBufferResource()
                    {
                        ElementCount = inputSize / 4,
                        FirstElement = 0
                    }
                });
                UnorderedAccessView u1 = new UnorderedAccessView(device, outputBuffer);

                // populate input buffer
                device.ImmediateContext.MapSubresource(inputBuffer, MapMode.Write, MapFlags.None, out DataStream ds);
                {
                    for (int i = 0; i < tangentSpace.Count; i++)
                    {
                        if (tangentSpace[i] is uint)
                        {
                            // write it straight out as a uint
                            ds.Write((uint)tangentSpace[i]);
                        }
                        else
                        {
                            // write out as a float4
                            ds.Write((Vector4)tangentSpace[i]);
                        }
                    }
                    ds.Position = 0;
                }
                device.ImmediateContext.UnmapSubresource(inputBuffer, 0);

                // run compute shader
                device.ImmediateContext.ComputeShader.Set(cs);
                device.ImmediateContext.ComputeShader.SetShaderResource(0, u0);
                device.ImmediateContext.ComputeShader.SetUnorderedAccessViews(0, u1);
                device.ImmediateContext.Dispatch(tangentSpace.Count, 1, 1);
                device.ImmediateContext.ComputeShader.Set(null);
                device.ImmediateContext.ComputeShader.SetUnorderedAccessViews(0, new UnorderedAccessView[] { null, null });
                device.ImmediateContext.CopyResource(outputBuffer, stagingBuffer);

                // read output buffer
                device.ImmediateContext.MapSubresource(stagingBuffer, MapMode.Read, MapFlags.None, out ds);
                {
                    for (int i = 0; i < tangentSpace.Count; i++)
                    {
                        Matrix4x3 m = ds.Read<Matrix4x3>();
                        layerElemTangent.DirectArray.Add(m.M11, m.M21, m.M31, 1.0f);
                        layerElemBinormal.DirectArray.Add(m.M12, m.M22, m.M32, 1.0f);
                        layerElemNormal.DirectArray.Add(m.M13, m.M23, m.M33, 1.0f);
                    }
                }
                device.ImmediateContext.UnmapSubresource(stagingBuffer, 0);

                // dispose
                u0.Dispose();
                u1.Dispose();
                outputBuffer.Dispose();
                inputBuffer.Dispose();
                stagingBuffer.Dispose();
                cs.Dispose();
            }

            reader.Position = indicesOffset + (section.StartIndex * indexSize);
            for (int i = 0; i < section.PrimitiveCount; i++)
            {
                fmesh.BeginPolygon(0);
                for (int j = 0; j < 3; j++)
                {
                    int index = (int)((indexSize == 2)
                        ? reader.ReadUShort()
                        : reader.ReadUInt()
                        );
                    fmesh.AddPolygon(index);
                }
                fmesh.EndPolygon();
            }

            for (int i = 0; i < MAX_CHANNELS; i++)
            {
                FbxLayer layer = fmesh.GetLayer(i);
                if (layer == null)
                {
                    fmesh.CreateLayer();
                    layer = fmesh.GetLayer(i);
                }

                if (i == 0)
                {
                    if (layerElemNormal != null) layer.SetNormals(layerElemNormal);
                    if (layerElemTangent != null) layer.SetTangents(layerElemTangent);
                    if (layerElemBinormal != null) layer.SetBinormals(layerElemBinormal);
                }

                if (layerElemVertexColor[i] != null) layer.SetVertexColors(layerElemVertexColor[i]);
                if (layerElemUV[i] != null) layer.SetUVs(layerElemUV[i]);
            }

            fmesh.BuildMeshEdgeArray();
            geomConverter.ComputeEdgeSmoothingFromNormals(fmesh);
            actor.SetNodeAttribute(fmesh);

            return actor;
        }

        private void FBXAddNodeRecursively(List<FbxNode> nodes, FbxNode node)
        {
            if (node != null)
            {
                FBXAddNodeRecursively(nodes, node.GetParent());

                if (!nodes.Contains(node))
                {
                    nodes.Add(node);
                }
            }
        }
    }

    #endregion

    #region -- Importers --

    #region -- Importer Exceptions --
    public class FBXImportInvalidLodCountException : Exception
    {
        public FBXImportInvalidLodCountException()
            : base("There was a mismatch in the amount of LODs defined in the imported file and existing mesh")
        {
        }
    }
    public class FBXImportInvalidSectionCountException : Exception
    {
        public FBXImportInvalidSectionCountException(int lodLevel)
            : base(string.Format("There was a mismatch in the amount of sections defined in the imported file for lod level {0} and existing mesh", lodLevel))
        {
        }
    }
    public class FBXImportInvalidMeshTypeException : Exception
    {
        public FBXImportInvalidMeshTypeException()
            : base(string.Format("Only rigid and skinned meshes are supported at this time"))
        {
        }
    }
    public class FBXImportMissingUvsException : Exception
    {
        public FBXImportMissingUvsException()
            : base(string.Format("Mesh must be exported with at least one UV channel"))
        {
        }
    }
    public class FBXImportMissingTangentsException : Exception
    {
        public FBXImportMissingTangentsException()
            : base(string.Format("Mesh must be exported with tangents/binormals"))
        {
        }
    }
    public class FBXImportUnimplementDataTypeException : Exception
    {
        public FBXImportUnimplementDataTypeException(VertexElementUsage usage)
            : base(string.Format("Mesh contains an unimplemented vertex data type '" + usage.ToString() + "'"))
        {
        }
    }
    public class FBXImportNoMeshesFoundException : Exception
    {
        public FBXImportNoMeshesFoundException(int lodLevel)
            : base(string.Format("Import file must contain at least one mesh at lod level " + lodLevel))
        {
        }
    }
    #endregion

    public class FBXImporter
    {
        public float Scale { get; set; } = 1.0f;
        public int XAxis { get; set; } = 0;
        public int YAxis { get; set; } = 1;
        public int ZAxis { get; set; } = 2;
        public float FlipZ { get; set; } = 1.0f;

        private MeshSet meshSet;
        private Stream resStream;
        private List<ShaderBlockDepot> shaderBlockDepots;
        private ResAssetEntry resEntry;
        private ILogger logger;
        private FrostyMeshImportSettings settings;

        public FBXImporter(ILogger inLogger)
        {
            logger = inLogger;
        }

        public void ImportFBX(string filename, MeshSet inMeshSet, EbxAsset asset, EbxAssetEntry entry, FrostyMeshImportSettings inSettings)
        {
            ulong resRid = ((dynamic)asset.RootObject).MeshSetResource;
            resEntry = App.AssetManager.GetResEntry(resRid);

            settings = inSettings;
            meshSet = inMeshSet;
            resStream = App.AssetManager.GetRes(resEntry);

            shaderBlockDepots = new List<ShaderBlockDepot>();
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                // collect every shader block depot that is used by this mesh
                string path = "/" + entry.Filename.ToLower();
                foreach (ResAssetEntry sbeEntry in App.AssetManager.EnumerateRes(resType: (uint)ResourceType.ShaderBlockDepot))
                {
                    if (sbeEntry.Name.Contains(path))
                    {
                        shaderBlockDepots.Add(App.AssetManager.GetResAs<ShaderBlockDepot>(sbeEntry));
                    }
                }
            }

            if (meshSet.Type != MeshType.MeshType_Rigid && meshSet.Type != MeshType.MeshType_Skinned)
                throw new FBXImportInvalidMeshTypeException();

            // @hack
            entry.LinkedAssets.Clear();
            resEntry.LinkedAssets.Clear();

            using (FbxManager manager = new FbxManager())
            {
                FbxIOSettings fbxSettings = new FbxIOSettings(manager, FbxIOSettings.IOSROOT);
                manager.SetIOSettings(fbxSettings);

                FbxScene scene = new FbxScene(manager, "");
                LoadScene(manager, scene, filename);

                List<FbxNode>[] lodNodes = new List<FbxNode>[7];
                int lodCount = 0;

                // look nodes
                foreach (FbxNode child in scene.RootNode.Children)
                {
                    string nodeName = child.Name.ToLower();
                    if (nodeName.Contains("lod"))
                    {
                        if (nodeName.Contains(":"))
                        {
                            // flat hierarchy, contains section:lod names
                            nodeName = nodeName.Substring(nodeName.Length - 1);
                            int lodIndex = -1;

                            if (int.TryParse(nodeName, out lodIndex))
                            {
                                if (lodNodes[lodIndex] == null)
                                {
                                    lodNodes[lodIndex] = new List<FbxNode>();
                                    lodCount++;
                                }
                                lodNodes[lodIndex].Add(child);
                            }
                        }
                        else
                        {
                            // standard hierarchy
                            nodeName = nodeName.Substring(nodeName.Length - 1);
                            int lodIndex = -1;

                            if (int.TryParse(nodeName, out lodIndex))
                            {
                                if (lodNodes[lodIndex] == null)
                                {
                                    lodNodes[lodIndex] = new List<FbxNode>();
                                    lodCount++;
                                }
                                lodNodes[lodIndex].AddRange(child.Children);
                            }
                        }
                    }
                }

                if (lodCount < meshSet.Lods.Count)
                    throw new FBXImportInvalidLodCountException();

                // process each lod
                for (int i = 0; i < meshSet.Lods.Count; i++)
                {
                    ProcessLod(lodNodes[i], i);
                }
            }

            meshSet.FullName = resEntry.Name;

            // modify resource
            App.AssetManager.ModifyRes(resRid, meshSet);
            entry.LinkAsset(resEntry);
        }

        private float CubeMapFaceID(float inX, float inY, float inZ)
        {
            if (Math.Abs(inZ) >= Math.Abs(inX) && Math.Abs(inZ) >= Math.Abs(inY))
                return (inZ < 0.0f) ? 5.0f : 4.0f; // faceID
            
            if (Math.Abs(inY) >= Math.Abs(inX))
                return (inY < 0.0f) ? 3.0f : 2.0f; // faceID

            return (inX < 0.0f) ? 1.0f : 0.0f; // faceID
        }

        private int FindGreatestComponent(Quaternion quat)
        {
            int xyzIndex = (int)(CubeMapFaceID(quat.X, quat.Y, quat.Z) * 0.5f);
            const int wIndex = 3;

            bool wBiggest = Math.Abs(quat.W) > Math.Max(Math.Abs(quat.X), Math.Max(Math.Abs(quat.Y), Math.Abs(quat.Z)));
            return (wBiggest) ? wIndex : xyzIndex;
        }

        private void LoadScene(FbxManager manager, FbxScene scene, string filename)
        {
            FbxManager.GetFileFormatVersion(out int major, out int minor, out int revision);

            using (FbxImporter importer = new FbxImporter(manager, ""))
            {
                if (!importer.Initialize(filename, -1, manager.GetIOSettings()))
                {
                    // unable to continue, most likely invalid file version
                    string errorString = importer.Status.ErrorString;
                    logger.Log(errorString);
                    return;
                }

                importer.GetFileVersion(out int fileMajor, out int fileMinor, out int fileRevision);

                if (importer.IsFBX())
                {
                    // try to import
                    bool status = importer.Import(scene);
                    if (!status)
                    {
                        // failed
                        logger.Log(importer.Status.ErrorString);
                        return;
                    }
                }
            }

            FbxDocumentInfo info = scene.SceneInfo;
        }

        private void ProcessLod(List<FbxNode> nodes, int lodIndex)
        {
            MeshSetLod meshLod = meshSet.Lods[lodIndex];
            List<FbxNode> sectionNodes = new List<FbxNode>();

            foreach (FbxNode child in nodes)
            {
                FbxNodeAttribute attr = child.GetNodeAttribute(FbxNodeAttribute.EType.eMesh);
                if (attr != null)
                    sectionNodes.Add(child);
            }

            if (sectionNodes.Count == 0)
                throw new FBXImportNoMeshesFoundException(lodIndex);

            List<MeshSetSection> meshSections = new List<MeshSetSection>();
            List<MeshSetSection> depthSections = new List<MeshSetSection>();
            List<MeshSetSection> shadowSections = new List<MeshSetSection>();

            foreach (MeshSetSection meshSection in meshLod.Sections)
            {
                if (meshSection.Name != "")
                    meshSections.Add(meshSection);
                else
                {
                    if (meshLod.IsSectionInCategory(meshSection, MeshSubsetCategory.MeshSubsetCategory_ZOnly))
                        depthSections.Add(meshSection);
                    else
                        shadowSections.Add(meshSection);
                }
            }

            List<FbxNode> sectionNodeMapping = new List<FbxNode>();
            List<FbxNode> unclaimedNodes = new List<FbxNode>();
            List<FbxNode> allNodes = new List<FbxNode>();
            sectionNodeMapping.AddRange(new FbxNode[meshSections.Count]);

            foreach (var node in sectionNodes)
            {
                string sectionName = node.Name;
                if (sectionName.Contains(':'))
                {
                    // remove the lod portion of the name
                    sectionName = sectionName.Remove(sectionName.IndexOf(':'));
                }

                int idx = meshSections.FindIndex((a) => a.Name == sectionName);
                if (idx != -1 && sectionNodeMapping[idx] == null)
                    sectionNodeMapping[idx] = node;
                else
                    unclaimedNodes.Add(node);
            }

            List<byte[]> sectionsVertices = new List<byte[]>();
            List<List<uint>> sectionsIndices = new List<List<uint>>();
            uint vertexBufferSize = 0;
            uint totalIndices = 0;

            // clear out old bones
            meshLod.ClearBones();

            // process each mesh renderable section first
            for (int i = 0; i < sectionNodeMapping.Count; i++)
            {
                var node = sectionNodeMapping[i];
                int sectionIndex = meshLod.Sections.IndexOf(meshSections[i]);

                if (node == null)
                {
                    if (unclaimedNodes.Count > 0)
                    {
                        node = unclaimedNodes.First();
                        unclaimedNodes.RemoveAt(0);
                    }
                    else
                    {
                        // kill section
                        meshSections[i].VertexCount = 0;
                        meshSections[i].PrimitiveCount = 0;
                        meshSections[i].VertexOffset = 0;
                        meshSections[i].StartIndex = 0;
                        continue;
                    }
                }

                MemoryStream vertices = new MemoryStream();
                List<uint> indices = new List<uint>();

                ProcessSection(new FbxNode[] { node }, meshLod, sectionIndex, vertices, indices, vertexBufferSize, ref totalIndices);

                sectionsVertices.Add(vertices.ToArray());
                sectionsIndices.Add(indices);

                vertexBufferSize += (uint)vertices.Length;
                vertices.Dispose();

                allNodes.Add(node);
            }

            // depth sections
            for (int i = 0; i < depthSections.Count; i++)
            {
                int sectionIndex = meshLod.Sections.IndexOf(depthSections[i]);

                if (i == 0)
                {
                    MemoryStream vertices = new MemoryStream();
                    List<uint> indices = new List<uint>();

                    ProcessSection(allNodes.ToArray(), meshLod, sectionIndex, vertices, indices, vertexBufferSize, ref totalIndices);

                    sectionsVertices.Add(vertices.ToArray());
                    sectionsIndices.Add(indices);

                    vertexBufferSize += (uint)vertices.Length;
                    vertices.Dispose();
                }
                else
                {
                    // kill section
                    depthSections[i].VertexCount = 0;
                    depthSections[i].PrimitiveCount = 0;
                    depthSections[i].VertexOffset = 0;
                    depthSections[i].StartIndex = 0;
                }
            }

            // shadow sections
            for (int i = 0; i < shadowSections.Count; i++)
            {
                int sectionIndex = meshLod.Sections.IndexOf(shadowSections[i]);

                if (i == 0)
                {
                    MemoryStream vertices = new MemoryStream();
                    List<uint> indices = new List<uint>();

                    ProcessSection(allNodes.ToArray(), meshLod, sectionIndex, vertices, indices, vertexBufferSize, ref totalIndices);

                    sectionsVertices.Add(vertices.ToArray());
                    sectionsIndices.Add(indices);

                    vertexBufferSize += (uint)vertices.Length;
                    vertices.Dispose();
                }
                else
                {
                    // kill section
                    shadowSections[i].VertexCount = 0;
                    shadowSections[i].PrimitiveCount = 0;
                    shadowSections[i].VertexOffset = 0;
                    shadowSections[i].StartIndex = 0;
                }
            }

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                // update shader block depot mesh parameters
                foreach (var depot in shaderBlockDepots)
                {
                    var sbe = depot.GetSectionEntry(lodIndex);
                    for (int i = 0; i < meshLod.Sections.Count; i++)
                    {
                        MeshParamDbBlock meshParams = sbe.GetMeshParams(i);
                        if (meshParams != null)
                        {
                            meshParams.SetParameterValue("!primitiveCount", meshLod.Sections[i].PrimitiveCount);
                            meshParams.SetParameterValue("!vertexStreamOffsets0", meshLod.Sections[i].VertexOffset);
                            meshParams.SetParameterValue("!startIndex", meshLod.Sections[i].StartIndex);
                            meshParams.IsModified = true;
                        }
                    }

                    App.AssetManager.ModifyRes(depot.ResourceId, depot);
                    resEntry.LinkAsset(App.AssetManager.GetResEntry(depot.ResourceId));
                }
            }

            bool largeIndexBuffer = false;
            foreach (var section in meshLod.Sections)
            {
                if (section.VertexCount > 65535)
                {
                    largeIndexBuffer = true;
                    break;
                }
            }

            // write out new chunk/inline data
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                foreach (byte[] buffer in sectionsVertices)
                    writer.Write(buffer);
                writer.WritePadding(0x10);
                meshLod.VertexBufferSize = (uint)writer.Position;
                foreach (List<uint> buffer in sectionsIndices)
                {
                    foreach (uint index in buffer)
                    {
                        if (largeIndexBuffer)
                            writer.Write(index);
                        else
                            writer.Write((ushort)index);
                    }
                }
                writer.WritePadding(0x10);
                meshLod.IndexBufferSize = (uint)(writer.Position - meshLod.VertexBufferSize);
                meshLod.SetIndexBufferFormatSize((largeIndexBuffer) ? 4 : 2);

                if (meshLod.ChunkId != Guid.Empty)
                {
                    // modify the chunk
                    App.AssetManager.ModifyChunk(meshLod.ChunkId, writer.ToByteArray());

                    ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(meshLod.ChunkId);
                    resEntry.LinkAsset(chunkEntry);
                }
                else
                {
                    // modify inline data
                    meshLod.SetInlineData(writer.ToByteArray());
                }
            }
        }

        private void ProcessSection(FbxNode[] sectionNodes, MeshSetLod meshLod, int sectionIndex, MemoryStream verticesBuffer, List<uint> indicesBuffer, uint vertexOffset, ref uint startIndex)
        {
            MeshSetSection meshSection = meshLod.Sections[sectionIndex];
            uint indexOffset = 0;

            meshSection.VertexOffset = vertexOffset;
            meshSection.StartIndex = startIndex;

            meshSection.VertexCount = 0;
            meshSection.PrimitiveCount = 0;

            List<ushort> boneList = new List<ushort>();
            List<string> boneNames = new List<string>();
            List<string> procBones = new List<string>();

            // load in skeleton
            dynamic skeleton = null;
            if (settings != null && settings.SkeletonAsset != "")
                skeleton = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(settings.SkeletonAsset)).RootObject;

            // collect bones first
            foreach (FbxNode sectionNode in sectionNodes)
            {
                if (sectionNode == null)
                    continue;

                FbxNodeAttribute attr = sectionNode.GetNodeAttribute(FbxNodeAttribute.EType.eMesh);
                FbxMesh fmesh = new FbxMesh(attr);
                FbxSkin fskin = (fmesh.GetDeformerCount(FbxDeformer.EDeformerType.eSkin) != 0)
                    ? new FbxSkin(fmesh.GetDeformer(0, FbxDeformer.EDeformerType.eSkin))
                    : null;

                if (fskin != null)
                {
                    // collect any procedural bones
                    foreach (FbxCluster cluster in fskin.Clusters)
                    {
                        FbxNode bone = cluster.GetLink();
                        if (bone.Name.StartsWith("PROC"))
                        {
                            if (!procBones.Contains(bone.Name))
                                procBones.Add(bone.Name);
                        }
                    }

                    // Madden19/FIFA17/FIFA18/Madden20
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20)
                    {
                        // @todo: if works, then merge with below
                        for (int i = 0; i < skeleton.BoneNames.Count; i++)
                        {
                            if (!boneList.Contains((ushort)i))
                            {
                                boneList.Add((ushort)i);
                                boneNames.Add(skeleton.BoneNames[i]);
                            }
                        }
                    }

                    // MEC/BF1/SWBF2/BFV/Anthem/FIFA19/FIFA20/BFN/SWS
                    else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 ||
                             ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    {
                        // ushort/uint, can handle long lists so just put all bones into sections
                        for (int i = 0; i < skeleton.BoneNames.Count; i++)
                        {
                            if (!boneList.Contains((ushort)i))
                            {
                                boneList.Add((ushort)i);
                                boneNames.Add(skeleton.BoneNames[i]);
                            }
                        }
                        foreach (string bone in procBones)
                        {
                            ushort boneIdx = ushort.Parse(bone.Replace("PROC_Bone", ""));
                            for (int i = 0; i <= boneIdx; i++)
                            {
                                if (!boneList.Contains((ushort)(skeleton.BoneNames.Count + i)))
                                {
                                    boneList.Add((ushort)(skeleton.BoneNames.Count + i));
                                    boneNames.Add("PROC_Bone" + i);
                                }
                            }
                        }
                    }
                    else
                    {
                        // byte, can only handle sublists, so only obtain bones used
                        foreach (FbxCluster cluster in fskin.Clusters)
                        {
                            if (cluster.ControlPointIndicesCount == 0)
                                continue;

                            FbxNode bone = cluster.GetLink();
                            ushort idx = (ushort)skeleton.BoneNames.IndexOf(bone.Name);

                            if (!boneList.Contains(idx))
                            {
                                boneList.Add(idx);
                                boneNames.Add(skeleton.BoneNames[idx]);
                            }
                        }
                    }
                }
            }

            ushort[] boneListArray = boneList.ToArray();
            string[] boneNamesArray = boneNames.ToArray();
            Array.Sort(boneListArray, boneNamesArray);

            meshSection.SetBones(boneListArray);
            meshLod.AddBones(boneListArray, boneNamesArray);

            boneList.Clear();
            boneList.AddRange(boneListArray);

            foreach (FbxNode sectionNode in sectionNodes)
            {
                if (sectionNode == null)
                    continue;

                // obtain the mesh component of the node
                FbxNodeAttribute attr = sectionNode.GetNodeAttribute(FbxNodeAttribute.EType.eMesh);
                FbxMesh fmesh = new FbxMesh(attr);
                FbxSkin fskin = (fmesh.GetDeformerCount(FbxDeformer.EDeformerType.eSkin) != 0)
                    ? new FbxSkin(fmesh.GetDeformer(0, FbxDeformer.EDeformerType.eSkin))
                    : null;

                // check for mandatory UVs/tangents/binormals
                if (fmesh.GetElementUV(0, FbxLayerElement.EType.eUnknown) == null)
                    throw new FBXImportMissingUvsException();
                if (fmesh.GetElementTangent(0) == null || fmesh.GetElementBinormal(0) == null)
                    throw new FBXImportMissingTangentsException();

                Matrix sectionMatrix = new FbxMatrix(sectionNode.EvaluateGlobalTransform()).ToSharpDX();

                List<List<ushort>> boneIndices = new List<List<ushort>>();
                List<List<byte>> boneWeights = new List<List<byte>>();
                int totalBoneInfluences = 0;

                // grab all the indices/weights from the skin clusters
                if (fskin != null)
                {
                      // collect all indices and weights
                    foreach (FbxCluster cluster in fskin.Clusters)
                    {
                        if (cluster.ControlPointIndicesCount == 0)
                            continue;

                        int[] tmpIndices = cluster.GetControlPointIndices();
                        double[] tmpWeights = cluster.GetControlPointWeights();

                        FbxNode bone = cluster.GetLink();
                        ushort boneIdx = 0xFFFF;

                        if (procBones.Contains(bone.Name))
                        {
                            // append 0x8000 to proc bones
                            boneIdx = (ushort)(0x8000 | ushort.Parse(bone.Name.Replace("PROC_Bone", "")));
                        }
                        else
                        {
                            boneIdx = (ushort)skeleton.BoneNames.IndexOf(bone.Name);
                            boneIdx = (ushort)boneList.IndexOf(boneIdx);
                        }

                        for (int i = 0; i < tmpIndices.Length; i++)
                        {
                            int idx = tmpIndices[i];
                            while (boneIndices.Count <= idx)
                            {
                                boneIndices.Add(new List<ushort>());
                                boneWeights.Add(new List<byte>());
                            }

                            if (((byte)Math.Round(tmpWeights[i] * 255.0)) > 0)
                            {
                                boneIndices[idx].Add(boneIdx);
                                boneWeights[idx].Add((byte)Math.Round(tmpWeights[i] * 255.0));
                            }
                        }
                    }

                    totalBoneInfluences = meshSection.BonesPerVertex;
                }

                // Write vertices into a temp import buffer, checking to see if there is a matching vertex already
                // stored, if so, return existing index,otherwise add new vertex and return the new index, this should reduce the
                // redundant vertices, but also ensure that vertices are duplicated in places where normals or UVs may differ

                List<DbObject> vertices = new List<DbObject>();
                vertices.AddRange(new DbObject[fmesh.ControlPointsCount]);

                List<int>[] indexToVertexMap = new List<int>[fmesh.ControlPointsCount];
                List<int> indices = new List<int>();
                IntPtr buffer = fmesh.GetControlPoints();
                int foundBoneInfluences = 0;

                for (int i = 0; i < fmesh.PolygonCount; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        //int index = (i * 3) + j;
                        int vertexIndex = fmesh.GetPolygonIndex(i, j);
                        DbObject vertex = DbObject.CreateObject();

                        Vector4 position;
                        Vector3 normal;
                        Vector3 tangent;
                        Vector3 binormal;
                        ushort[] finalBoneIndices = null;
                        byte[] finalBoneWeights = null;

                        // position
                        unsafe
                        {
                            double* pointsPtr = (double*)(buffer + (vertexIndex * 32));
                            position = new Vector4((float)pointsPtr[XAxis] * Scale, (float)pointsPtr[YAxis] * Scale, (float)(pointsPtr[ZAxis] * FlipZ) * Scale, 1.0f);
                        }

                        // normal
                        {
                            FbxLayerElementNormal layerElemNormal = fmesh.GetElementNormal(0);
                            int mappingIndex = (layerElemNormal.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                            int actualIndex = mappingIndex;
                            if (layerElemNormal.ReferenceMode != EReferenceMode.eDirect)
                                layerElemNormal.IndexArray.GetAt(mappingIndex, out actualIndex);

                            layerElemNormal.DirectArray.GetAt(actualIndex, out Vector4 tmp);
                            normal = new Vector3(tmp[XAxis], tmp[YAxis], tmp[ZAxis] * FlipZ);
                        }

                        // tangent
                        {
                            FbxLayerElementTangent layerElemTangent = fmesh.GetElementTangent(0);
                            int mappingIndex = (layerElemTangent.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                            int actualIndex = mappingIndex;
                            if (layerElemTangent.ReferenceMode != EReferenceMode.eDirect)
                                layerElemTangent.IndexArray.GetAt(mappingIndex, out actualIndex);

                            layerElemTangent.DirectArray.GetAt(actualIndex, out Vector4 tmp);
                            tangent = new Vector3(tmp[XAxis], tmp[YAxis], tmp[ZAxis] * FlipZ);
                        }

                        // binormal
                        {
                            FbxLayerElementBinormal layerElemBinormal = fmesh.GetElementBinormal(0);
                            int mappingIndex = (layerElemBinormal.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                            int actualIndex = mappingIndex;
                            if (layerElemBinormal.ReferenceMode != EReferenceMode.eDirect)
                                layerElemBinormal.IndexArray.GetAt(mappingIndex, out actualIndex);

                            layerElemBinormal.DirectArray.GetAt(actualIndex, out Vector4 tmp);
                            binormal = new Vector3(tmp[XAxis], tmp[YAxis], tmp[ZAxis] * FlipZ);
                        }

                        // indices/weights
                        if (fskin != null)
                        {
                            List<ushort> localBoneIndices = new List<ushort>();
                            List<byte> localBoneWeights = new List<byte>();

                            // sort indices/weights by biggest influence
                            var boneIndicesAndWeights = boneIndices[vertexIndex].Select((boneIndex, z) => new { boneIndex, boneWeight = boneWeights[vertexIndex][z] });
                            boneIndicesAndWeights = boneIndicesAndWeights.OrderByDescending(a => a.boneWeight);

                            localBoneIndices.AddRange(boneIndicesAndWeights.Select(a => a.boneIndex));
                            localBoneWeights.AddRange(boneIndicesAndWeights.Select(a => a.boneWeight));

                            foundBoneInfluences = (localBoneIndices.Count > foundBoneInfluences) ? localBoneIndices.Count : foundBoneInfluences;
                            while (localBoneIndices.Count > totalBoneInfluences)
                            {
                                // remove the lowest influence bones
                                localBoneIndices.RemoveRange(totalBoneInfluences, localBoneIndices.Count - totalBoneInfluences);
                                localBoneWeights.RemoveRange(totalBoneInfluences, localBoneWeights.Count - totalBoneInfluences);
                            }

                            int totalWeight = 0;
                            for (int k = 0; k < localBoneWeights.Count; k++)
                                totalWeight += localBoneWeights[k];

                            if (totalWeight != 255)
                            {
                                // normalize remaining weights across the range
                                for (int k = 0; k < localBoneWeights.Count; k++)
                                {
                                    localBoneWeights[k] = (byte)(Math.Round((localBoneWeights[k] / (double)totalWeight) * 255));
                                    if (localBoneWeights[k] <= 0)
                                        localBoneIndices[k] = 0;
                                }

                                totalWeight = 0;
                                int maxWeight = 0;
                                int maxIndex = -1;

                                for (int k = 0; k < localBoneWeights.Count; k++)
                                {
                                    totalWeight += localBoneWeights[k];
                                    if (localBoneWeights[k] > maxWeight)
                                    {
                                        maxWeight = localBoneWeights[k];
                                        maxIndex = k;
                                    }
                                }
                                if (totalWeight != 255)
                                {
                                    localBoneWeights[maxIndex] = (byte)(maxWeight + (255 - totalWeight));
                                }
                            }

                            // sort by bone indices
                            finalBoneIndices = localBoneIndices.ToArray();
                            finalBoneWeights = localBoneWeights.ToArray();
                            Array.Sort(finalBoneIndices, finalBoneWeights);

                            localBoneIndices.Clear();
                            localBoneWeights.Clear();

                            // resize arrays to hold exactly 8
                            int origCount = finalBoneIndices.Length;
                            for (int z = 0; z < 8 - origCount; z++)
                                localBoneIndices.Add(finalBoneIndices[0]);
                            localBoneIndices.AddRange(finalBoneIndices);

                            localBoneWeights.AddRange(new byte[8 - origCount]);
                            localBoneWeights.AddRange(finalBoneWeights);

                            // finalize
                            finalBoneIndices = localBoneIndices.ToArray();
                            finalBoneWeights = localBoneWeights.ToArray();
                        }

                        vertex.SetValue("Pos", position);
                        vertex.SetValue("Binormal", binormal);
                        vertex.SetValue("Normal", normal);
                        vertex.SetValue("Tangent", tangent);

                        // add mandatory UV channel
                        {
                            FbxLayerElementUV layerUV = fmesh.GetElementUV(0, FbxLayerElement.EType.eUnknown);
                            if (layerUV != null)
                            {
                                int mappingIndex = (layerUV.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                int actualIndex = mappingIndex;
                                if (layerUV.ReferenceMode != EReferenceMode.eDirect)
                                    layerUV.IndexArray.GetAt(mappingIndex, out actualIndex);

                                layerUV.DirectArray.GetAt(actualIndex, out Vector2 uv);
                                vertex.SetValue("TexCoord" + 0, uv);
                            }
                        }

                        foreach (GeometryDeclarationDesc.Element elem in meshSection.GeometryDeclDesc[0].Elements)
                        {
                            switch (elem.Usage)
                            {
                                case VertexElementUsage.BoneIndices: vertex.SetValue("BoneIndices", finalBoneIndices); break;
                                case VertexElementUsage.BoneWeights: vertex.SetValue("BoneWeights", finalBoneWeights); break;

                                case VertexElementUsage.Color0:
                                    {
                                        FbxLayerElementVertexColor vc = fmesh.GetElementVertexColor(0);
                                        if (vc != null)
                                        {
                                            int mappingIndex = (vc.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                            int actualIndex = mappingIndex;
                                            if (vc.ReferenceMode != EReferenceMode.eDirect)
                                                vc.IndexArray.GetAt(mappingIndex, out actualIndex);

                                            vc.DirectArray.GetAt(actualIndex, out ColorBGRA color);
                                            vertex.SetValue("Color0", color);
                                        }
                                    }
                                    break;

                                case VertexElementUsage.Color1:
                                    {
                                        FbxLayerElementVertexColor vc = fmesh.GetElementVertexColor(1);
                                        if (vc != null)
                                        {
                                            int mappingIndex = (vc.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                            int actualIndex = mappingIndex;
                                            if (vc.ReferenceMode != EReferenceMode.eDirect)
                                                vc.IndexArray.GetAt(mappingIndex, out actualIndex);

                                            vc.DirectArray.GetAt(actualIndex, out ColorBGRA color);
                                            vertex.SetValue("Color1", color);
                                        }
                                    }
                                    break;

                                case VertexElementUsage.MaskUv:
                                    {
                                        FbxLayerElementUV layerUV = fmesh.GetElementUV("MaskUv");
                                        if (layerUV != null)
                                        {
                                            int mappingIndex = (layerUV.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                            int actualIndex = mappingIndex;
                                            if (layerUV.ReferenceMode != EReferenceMode.eDirect)
                                                layerUV.IndexArray.GetAt(mappingIndex, out actualIndex);

                                            layerUV.DirectArray.GetAt(actualIndex, out Vector2 uv);
                                            vertex.SetValue("MaskUv", uv);
                                        }
                                    }
                                    break;

                                case VertexElementUsage.Delta:
                                    {
                                        FbxLayerElementVertexColor layerColor = fmesh.GetElementVertexColor("Delta");
                                        if (layerColor != null)
                                        {
                                            int mappingIndex = (layerColor.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                            int actualIndex = mappingIndex;
                                            if (layerColor.ReferenceMode != EReferenceMode.eDirect)
                                                layerColor.IndexArray.GetAt(mappingIndex, out actualIndex);

                                            layerColor.DirectArray.GetAt(actualIndex, out ColorBGRA color);
                                            vertex.SetValue("Delta", color);
                                        }
                                    }
                                    break;

                                case VertexElementUsage.BlendWeights:
                                    {
                                        FbxLayerElementVertexColor layerColor = fmesh.GetElementVertexColor("BlendWeights");
                                        if (layerColor != null)
                                        {
                                            int mappingIndex = (layerColor.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                            int actualIndex = mappingIndex;
                                            if (layerColor.ReferenceMode != EReferenceMode.eDirect)
                                                layerColor.IndexArray.GetAt(mappingIndex, out actualIndex);

                                            layerColor.DirectArray.GetAt(actualIndex, out ColorBGRA color);
                                            vertex.SetValue("Delta", color.R / 255.0f);
                                        }
                                    }
                                    break;

                                case VertexElementUsage.RegionIds:
                                    {
                                        FbxLayerElementVertexColor layerColor = fmesh.GetElementVertexColor("RegionIds");
                                        if (layerColor != null)
                                        {
                                            int mappingIndex = (layerColor.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                            int actualIndex = mappingIndex;
                                            if (layerColor.ReferenceMode != EReferenceMode.eDirect)
                                                layerColor.IndexArray.GetAt(mappingIndex, out actualIndex);

                                            layerColor.DirectArray.GetAt(actualIndex, out ColorBGRA color);
                                            vertex.SetValue("Delta", (int)color.R);
                                        }
                                    }
                                    break;

                                case VertexElementUsage.Pos:
                                case VertexElementUsage.BinormalSign:
                                case VertexElementUsage.Binormal:
                                case VertexElementUsage.Normal:
                                case VertexElementUsage.Tangent:
                                case VertexElementUsage.TangentSpace:
                                case VertexElementUsage.TexCoord0:
                                case VertexElementUsage.RadiosityTexCoord:
                                case VertexElementUsage.DisplacementMapTexCoord:
                                case VertexElementUsage.BoneIndices2:
                                case VertexElementUsage.BoneWeights2:
                                case VertexElementUsage.Unknown:
                                    break;

                                default:
                                    {
                                        if (elem.Usage >= VertexElementUsage.TexCoord1 && elem.Usage <= VertexElementUsage.TexCoord7)
                                        {
                                            int uvIndex = (int)(elem.Usage - VertexElementUsage.TexCoord0);
                                            FbxLayerElementUV layerUV = fmesh.GetElementUV(uvIndex, FbxLayerElement.EType.eUnknown);
                                            if (layerUV != null)
                                            {
                                                int mappingIndex = (layerUV.MappingMode == EMappingMode.eByControlPoint) ? vertexIndex : (i * 3) + j;
                                                int actualIndex = mappingIndex;
                                                if (layerUV.ReferenceMode != EReferenceMode.eDirect)
                                                    layerUV.IndexArray.GetAt(mappingIndex, out actualIndex);

                                                layerUV.DirectArray.GetAt(actualIndex, out Vector2 uv);
                                                vertex.SetValue("TexCoord" + uvIndex, uv);
                                            }
                                        }
                                        else
                                            throw new FBXImportUnimplementDataTypeException(elem.Usage);
                                    }
                                    break;
                            }
                        }

                        int foundIdx = -1;
                        if (indexToVertexMap[vertexIndex] != null)
                        {
                            foreach (int idx in indexToVertexMap[vertexIndex])
                            {
                                if (vertices[idx].GetValue<Vector4>("Pos") != vertex.GetValue<Vector4>("Pos"))
                                    continue;
                                if (vertices[idx].GetValue<Vector3>("Normal") != vertex.GetValue<Vector3>("Normal"))
                                    continue;
                                if (vertices[idx].GetValue<Vector3>("Tangent") != vertex.GetValue<Vector3>("Tangent"))
                                    continue;
                                if (vertices[idx].GetValue<Vector3>("Binormal") != vertex.GetValue<Vector3>("Binormal"))
                                    continue;

                                for (int k = 0; k < 8; k++)
                                {
                                    string channelName = "TexCoord" + k;
                                    if (vertex.HasValue(channelName))
                                    {
                                        if (vertices[idx].GetValue<Vector2>(channelName) != vertex.GetValue<Vector2>(channelName))
                                        {
                                            foundIdx = -2;
                                            break;
                                        }

                                    }
                                }

                                if (foundIdx == -1)
                                {
                                    foundIdx = idx;
                                    break;
                                }
                            }
                        }
                        else
                            indexToVertexMap[vertexIndex] = new List<int>();

                        if (foundIdx < 0)
                        {
                            if (vertices[vertexIndex] == null)
                            {
                                vertices[vertexIndex] = vertex;
                                indexToVertexMap[vertexIndex].Add(vertexIndex);
                                indices.Add(vertexIndex);
                            }
                            else
                            {
                                vertices.Add(vertex);
                                indexToVertexMap[vertexIndex].Add(vertices.Count - 1);
                                indices.Add(vertices.Count - 1);
                            }
                        }
                        else
                        {
                            indices.Add(foundIdx);
                        }
                    }
                }

                //if (foundBoneInfluences > totalBoneInfluences)
                //{
                //    // display warning about mismatching bones per vertex
                //    logger.LogWarning("Imported section '" + sectionNode.Name + "' contains " + foundBoneInfluences + " bones per vertex when the maximum for this section is " + totalBoneInfluences + " bones per vertex. Extra bone influences have been removed. This may result in incorrect skinning");
                //}

                // find and remove any redundant vertices
                List<int> indexToIndexMap = new List<int>(new int[vertices.Count]);
                int actualVertexId = 0;

                for (int vertexId = 0; vertexId < vertices.Count; vertexId++, actualVertexId++)
                {
                    if (vertices[vertexId] == null)
                    {
                        vertices.RemoveAt(vertexId);
                        vertexId--;
                    }
                    else
                    {
                        indexToIndexMap[actualVertexId] = vertexId;
                    }
                }

                // write new vertices
                using (NativeWriter chunkWriter = new NativeWriter(verticesBuffer, true))
                {
                    chunkWriter.Position = chunkWriter.Length;
                    int totalStride = 0;

                    foreach (GeometryDeclarationDesc.Stream stream in meshSection.GeometryDeclDesc[0].Streams)
                    {
                        if (stream.VertexStride == 0)
                            continue;

                        foreach (DbObject vertex in vertices)
                        {
                            Vector4 tmp = vertex.GetValue<Vector4>("Pos");
                            Vector4 position = Vector3.Transform(new Vector3(tmp.X, tmp.Y, tmp.Z), sectionMatrix);

                            Vector3 normal = Vector3.TransformNormal(vertex.GetValue<Vector3>("Normal"), sectionMatrix);
                            Vector3 tangent = Vector3.TransformNormal(vertex.GetValue<Vector3>("Tangent"), sectionMatrix);
                            Vector3 binormal = Vector3.TransformNormal(vertex.GetValue<Vector3>("Binormal"), sectionMatrix);

                            // for some reason the tangent gets stored inverted
                            tangent *= -1.0f;

                            ushort[] finalBoneIndices = vertex.GetValue<ushort[]>("BoneIndices");
                            byte[] finalBoneWeights = vertex.GetValue<byte[]>("BoneWeights");

                            int currentStride = 0;
                            foreach (GeometryDeclarationDesc.Element elem in meshSection.GeometryDeclDesc[0].Elements)
                            {
                                if (elem.Usage == VertexElementUsage.Unknown)
                                    continue;

                                if (currentStride >= totalStride && currentStride < (totalStride + stream.VertexStride))
                                {
                                    switch (elem.Usage)
                                    {
                                        case VertexElementUsage.Pos:
                                            {
                                                if (elem.Format == VertexElementFormat.Float3 || elem.Format == VertexElementFormat.Float4)
                                                {
                                                    chunkWriter.Write(position.X);
                                                    chunkWriter.Write(position.Y);
                                                    chunkWriter.Write(position.Z);

                                                    if (elem.Format == VertexElementFormat.Float4)
                                                        chunkWriter.Write(1.0f);
                                                }
                                                else if (elem.Format == VertexElementFormat.Half3 || elem.Format == VertexElementFormat.Half4)
                                                {
                                                    chunkWriter.Write(HalfUtils.Pack(position.X));
                                                    chunkWriter.Write(HalfUtils.Pack(position.Y));
                                                    chunkWriter.Write(HalfUtils.Pack(position.Z));

                                                    if (elem.Format == VertexElementFormat.Half4)
                                                        chunkWriter.Write(HalfUtils.Pack(1.0f));
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.BinormalSign:
                                            {
                                                if (elem.Format == VertexElementFormat.Half)
                                                {
                                                    chunkWriter.Write(HalfUtils.Pack((Vector3.Dot(binormal, Vector3.Cross(normal, tangent))) < 0.0f ? 1.0f : -1.0f));
                                                }
                                                else if (elem.Format == VertexElementFormat.Half4 || elem.Format == VertexElementFormat.Float4)
                                                {
                                                    if (elem.Format == VertexElementFormat.Half4)
                                                    {
                                                        chunkWriter.Write(HalfUtils.Pack(tangent.X));
                                                        chunkWriter.Write(HalfUtils.Pack(tangent.Y));
                                                        chunkWriter.Write(HalfUtils.Pack(tangent.Z));
                                                        chunkWriter.Write(HalfUtils.Pack((Vector3.Dot(binormal, Vector3.Cross(normal, tangent))) < 0.0f ? 1.0f : -1.0f));
                                                    }
                                                    else
                                                    {
                                                        chunkWriter.Write(tangent.X);
                                                        chunkWriter.Write(tangent.Y);
                                                        chunkWriter.Write(tangent.Z);
                                                        chunkWriter.Write((Vector3.Dot(binormal, Vector3.Cross(normal, tangent))) < 0.0f ? 1.0f : -1.0f);
                                                    }
                                                }
                                                else
                                                {
                                                    chunkWriter.Write((Vector3.Dot(binormal, Vector3.Cross(normal, tangent))) < 0.0f ? 1.0f : -1.0f);
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.BoneIndices:
                                            {
                                                if (elem.Format == VertexElementFormat.UShort4)
                                                {
                                                    chunkWriter.Write(finalBoneIndices[4]);
                                                    chunkWriter.Write(finalBoneIndices[5]);
                                                    chunkWriter.Write(finalBoneIndices[6]);
                                                    chunkWriter.Write(finalBoneIndices[7]);
                                                }
                                                else
                                                {
                                                    chunkWriter.Write((byte)finalBoneIndices[4]);
                                                    chunkWriter.Write((byte)finalBoneIndices[5]);
                                                    chunkWriter.Write((byte)finalBoneIndices[6]);
                                                    chunkWriter.Write((byte)finalBoneIndices[7]);
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.BoneWeights:
                                            {
                                                chunkWriter.Write(finalBoneWeights[4]);
                                                chunkWriter.Write(finalBoneWeights[5]);
                                                chunkWriter.Write(finalBoneWeights[6]);
                                                chunkWriter.Write(finalBoneWeights[7]);
                                            }
                                            break;

                                        case VertexElementUsage.BoneIndices2:
                                            {
                                                if (elem.Format == VertexElementFormat.UShort4)
                                                {
                                                    chunkWriter.Write(finalBoneIndices[0]);
                                                    chunkWriter.Write(finalBoneIndices[1]);
                                                    chunkWriter.Write(finalBoneIndices[2]);
                                                    chunkWriter.Write(finalBoneIndices[3]);
                                                }
                                                else
                                                {
                                                    chunkWriter.Write((byte)finalBoneIndices[0]);
                                                    chunkWriter.Write((byte)finalBoneIndices[1]);
                                                    chunkWriter.Write((byte)finalBoneIndices[2]);
                                                    chunkWriter.Write((byte)finalBoneIndices[3]);
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.BoneWeights2:
                                            {
                                                chunkWriter.Write(finalBoneWeights[0]);
                                                chunkWriter.Write(finalBoneWeights[1]);
                                                chunkWriter.Write(finalBoneWeights[2]);
                                                chunkWriter.Write(finalBoneWeights[3]);
                                            }
                                            break;

                                        case VertexElementUsage.Binormal:
                                            {
                                                chunkWriter.Write(HalfUtils.Pack(binormal.X));
                                                chunkWriter.Write(HalfUtils.Pack(binormal.Y));
                                                chunkWriter.Write(HalfUtils.Pack(binormal.Z));
                                                chunkWriter.Write(HalfUtils.Pack(1.0f));
                                            }
                                            break;

                                        case VertexElementUsage.Normal:
                                            {
                                                chunkWriter.Write(HalfUtils.Pack(normal.X));
                                                chunkWriter.Write(HalfUtils.Pack(normal.Y));
                                                chunkWriter.Write(HalfUtils.Pack(normal.Z));
                                                chunkWriter.Write(HalfUtils.Pack(1.0f));
                                            }
                                            break;

                                        case VertexElementUsage.Tangent:
                                            {
                                                chunkWriter.Write(HalfUtils.Pack(tangent.X));
                                                chunkWriter.Write(HalfUtils.Pack(tangent.Y));
                                                chunkWriter.Write(HalfUtils.Pack(tangent.Z));
                                                chunkWriter.Write(HalfUtils.Pack(1.0f));
                                            }
                                            break;

                                        case VertexElementUsage.TangentSpace:
                                            {
                                                if (elem.Format == VertexElementFormat.UInt)
                                                {
                                                    // packed as a packed quaternion

                                                    Vector3 b = Vector3.Cross(normal, tangent);
                                                    Quaternion quat = new Quaternion
                                                    {
                                                        X = normal.Y - binormal.Z,
                                                        Y = tangent.Z - normal.X,
                                                        Z = binormal.X - tangent.Y,
                                                        W = 1.0f + tangent.X + binormal.Y + normal.Z
                                                    };
                                                    quat.Normalize();

                                                    int idx = FindGreatestComponent(quat);
                                                    if (idx == 0) quat = new Quaternion(quat.W, quat.X, quat.Y, quat.Z);
                                                    if (idx == 1) quat = new Quaternion(quat.X, quat.W, quat.Y, quat.Z);
                                                    if (idx == 2) quat = new Quaternion(quat.X, quat.Y, quat.W, quat.Z);

                                                    Vector3 tmpQuat = new Vector3(quat.X, quat.Y, quat.Z) * Math.Sign(quat.W) * (float)Math.Sqrt(0.5f) + 0.5f;
                                                    Vector4 packedQuat = new Vector4(tmpQuat.X, tmpQuat.Y, tmpQuat.Z, idx / 3.0f);

                                                    uint ts = 0;
                                                    ts |= ((uint)(packedQuat.X * 1023.0f) << 22);
                                                    ts |= ((uint)(packedQuat.Y * 511.0f) << 13);
                                                    ts |= ((uint)(packedQuat.Z * 1023.0f) << 3);
                                                    ts |= ((uint)(packedQuat.W * 3.0f) << 1);
                                                    ts |= (uint)((Vector3.Dot(Vector3.Cross(normal, tangent), binormal)) < 0.0f ? 1 : 0);

                                                    chunkWriter.Write(ts);
                                                }
                                                else
                                                {
                                                    // axis angle normals (packed as either UByte4N or UShort4N)
                                                    // @todo
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.RadiosityTexCoord:
                                            {
                                                chunkWriter.Write(HalfUtils.Pack(0.0f));
                                                chunkWriter.Write(HalfUtils.Pack(0.0f));
                                            }
                                            break;

                                        case VertexElementUsage.Color0:
                                            {
                                                if (vertex.HasValue("Color0"))
                                                {
                                                    ColorBGRA color = vertex.GetValue<ColorBGRA>("Color0");
                                                    chunkWriter.Write(color.R);
                                                    chunkWriter.Write(color.G);
                                                    chunkWriter.Write(color.B);
                                                    chunkWriter.Write(color.A);
                                                }
                                                else
                                                    chunkWriter.Write(0);
                                            }
                                            break;

                                        case VertexElementUsage.Color1:
                                            {
                                                if (vertex.HasValue("Color1"))
                                                {
                                                    ColorBGRA color = vertex.GetValue<ColorBGRA>("Color0");
                                                    chunkWriter.Write(color.R);
                                                    chunkWriter.Write(color.G);
                                                    chunkWriter.Write(color.B);
                                                    chunkWriter.Write(color.A);
                                                }
                                                else
                                                    chunkWriter.Write(0);
                                            }
                                            break;

                                        case VertexElementUsage.MaskUv:
                                            {
                                                if (vertex.HasValue("MaskUv"))
                                                {
                                                    Vector2 uv = vertex.GetValue<Vector2>("MaskUv");
                                                    chunkWriter.Write((short)((uv.X * 2 - 1) * short.MaxValue));
                                                    chunkWriter.Write((short)((uv.Y * 2 - 1) * short.MaxValue));
                                                }
                                                else
                                                {
                                                    chunkWriter.Write((ushort)0);
                                                    chunkWriter.Write((ushort)0);
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.Delta:
                                            {
                                                if (vertex.HasValue("Delta"))
                                                {
                                                    ColorBGRA color = vertex.GetValue<ColorBGRA>("Delta");
                                                    chunkWriter.Write(HalfUtils.Pack(color.R * 255.0f));
                                                    chunkWriter.Write(HalfUtils.Pack(color.G * 255.0f));
                                                    chunkWriter.Write(HalfUtils.Pack(color.B * 255.0f));
                                                    chunkWriter.Write(HalfUtils.Pack(color.A * 255.0f));
                                                }
                                                else
                                                {
                                                    chunkWriter.Write(HalfUtils.Pack(0));
                                                    chunkWriter.Write(HalfUtils.Pack(0));
                                                    chunkWriter.Write(HalfUtils.Pack(0));
                                                    chunkWriter.Write(HalfUtils.Pack(0));
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.BlendWeights:
                                            {
                                                if (vertex.HasValue("BlendWeights"))
                                                {
                                                    float x = vertex.GetValue<float>("BlendWeights");
                                                    chunkWriter.Write(x);
                                                }
                                                else
                                                {
                                                    chunkWriter.Write(0.0f);
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.RegionIds:
                                            {
                                                if (vertex.HasValue("RegionIds"))
                                                {
                                                    int x = vertex.GetValue<int>("RegionIds");
                                                    chunkWriter.Write(x);
                                                }
                                                else
                                                {
                                                    chunkWriter.Write(0);
                                                }
                                            }
                                            break;

                                        case VertexElementUsage.Unknown:
                                            break;

                                        default:
                                            {
                                                if (elem.Usage >= VertexElementUsage.TexCoord0 && elem.Usage <= VertexElementUsage.TexCoord7)
                                                {
                                                    string channelName = "TexCoord" + (int)(elem.Usage - VertexElementUsage.TexCoord0);
                                                    if (vertex.HasValue(channelName))
                                                    {
                                                        Vector2 uv = vertex.GetValue<Vector2>(channelName);
                                                        chunkWriter.Write(HalfUtils.Pack(uv.X));
                                                        chunkWriter.Write(HalfUtils.Pack(1.0f - uv.Y));
                                                    }
                                                    else
                                                    {
                                                        chunkWriter.Write((ushort)0);
                                                        chunkWriter.Write((ushort)0);
                                                    }
                                                }
                                                else
                                                    throw new FBXImportUnimplementDataTypeException(elem.Usage);
                                            }
                                            break;
                                    }
                                }

                                currentStride += elem.Size;
                            }
                        }

                        totalStride += stream.VertexStride;
                    }

                    meshSection.VertexCount += (uint)vertices.Count;
                }

                // write indices
                int numIndices = 0;
                for (int i = 0; i < indices.Count; i++)
                {
                    indicesBuffer.Add((uint)(indexOffset + indexToIndexMap[indices[i]]));
                    numIndices++;
                }

                meshSection.PrimitiveCount += (uint)(numIndices / 3);

                startIndex += (uint)numIndices;
                indexOffset += (uint)vertices.Count;
            }
        }
    }
    #endregion

    class VariationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint variation = (uint)value;
            return variation.ToString("X8");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return uint.TryParse((string)value, NumberStyles.HexNumber, null, out uint variation) ? variation : 0;
        }
    }

    class MeshSetElementItemToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FrostyMeshSetEditor.MeshSetElementItem item = (FrostyMeshSetEditor.MeshSetElementItem)value;
            return (item.Name != "") ? item.Name : "<<unnamed>>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FrostyMeshImportSettings
    {
        [DisplayName("Skeleton")]
        [Editor(typeof(FrostySkeletonEditor))]
        public string SkeletonAsset { get; set; } = "";
    }

    [TemplatePart(Name = PART_LodComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_RenderModeComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyRenderImage))]
    [TemplatePart(Name = PART_DebugTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_VariationsListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_Details, Type = typeof(FrostyPropertyGrid))]
    [TemplatePart(Name = PART_ExtractMaterialInfoButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_VariationComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_PreviewSettings, Type = typeof(FrostyPropertyGrid))]
    [TemplatePart(Name = PART_MeshSettings, Type = typeof(FrostyPropertyGrid))]
    [TemplatePart(Name = PART_MeshTabContent, Type = typeof(FrostyDetachedTabControl))]
    [TemplatePart(Name = PART_MeshTabControl, Type = typeof(FrostyTabControl))]
#if FROSTY_DEVELOPER
    [TemplatePart(Name = PART_RenderDocButton, Type = typeof(Button))]
#endif
    public class FrostyMeshSetEditor : FrostyAssetEditor
    {
        public struct MeshSetElementItem
        {
            public string Name { get; set; }
            public int SectionIndex { get; set; }
            public int MaterialId { get; set; }
            public MeshSubsetCategoryFlags Categories { get; set; }
        }

        private const string PART_LodComboBox = "PART_LodComboBox";
        private const string PART_RenderModeComboBox = "PART_RenderModeComboBox";
        private const string PART_Renderer = "PART_Renderer";
        private const string PART_SectionListBox = "PART_SectionListBox";
        private const string PART_DebugTextBox = "PART_DebugTextBox";
        private const string PART_VariationsListBox = "PART_VariationsListBox";
        private const string PART_VariationComboBox = "PART_VariationComboBox";
        private const string PART_Details = "PART_Details";
        private const string PART_ExtractMaterialInfoButton = "PART_ExtractMaterialInfoButton";
        private const string PART_PreviewSettings = "PART_PreviewSettings";
        private const string PART_MeshSettings = "PART_MeshSettings";
        private const string PART_AssetPropertyGrid = "PART_AssetPropertyGrid";
        private const string PART_MeshTabControl = "PART_MeshTabControl";
        private const string PART_MeshTabContent = "PART_MeshTabContent";

#if FROSTY_DEVELOPER
        private const string PART_RenderDocButton = "PART_RenderDocButton";
#endif

        private ComboBox lodComboBox;
        private ComboBox renderModeComboBox;
        private MeshSet meshSet;
        private Guid meshGuid;
        private FrostyViewport viewport;
        private MultiMeshPreviewScreen screen = new MultiMeshPreviewScreen();
        private FrostyPropertyGrid pgDetails;
        private Button extractButton;
        private ComboBox variationsComboBox;
        private FrostyPropertyGrid pgPreviewSettings;
        private FrostyPropertyGrid pgAsset;
        private FrostyPropertyGrid pgMeshSettings;
        private FrostyDetachedTabControl meshTabContent;
        private FrostyTabControl meshTabControl;
#if FROSTY_DEVELOPER
        private Button renderDocButton;
#endif

        private bool firstTimeLoad = true;
        private static Dictionary<uint, EbxAssetEntry> objectVariationMapping = new Dictionary<uint, EbxAssetEntry>();
        private MeshSetPreviewSettings previewSettings = new MeshSetPreviewSettings();
        private MeshSetMeshSettings meshSettings = new MeshSetMeshSettings();
        private List<MeshSetVariationDetails> variations;
        
        private int selectedPreviewIndex = 0;
        private int selectedVariationsIndex = 0;

        private List<ShaderBlockDepot> shaderBlockDepots = new List<ShaderBlockDepot>();

        static FrostyMeshSetEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyMeshSetEditor), new FrameworkPropertyMetadata(typeof(FrostyMeshSetEditor)));
        }

        public FrostyMeshSetEditor()
            : base(null)
        {
        }

        public FrostyMeshSetEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += FrostyMeshSetEditor_Loaded;

            extractButton = GetTemplateChild(PART_ExtractMaterialInfoButton) as Button;
            extractButton.Click += ExtractButton_Click;

            viewport = GetTemplateChild(PART_Renderer) as FrostyViewport;

            lodComboBox = GetTemplateChild(PART_LodComboBox) as ComboBox;
            lodComboBox.SelectionChanged += LodComboBox_SelectionChanged;

            renderModeComboBox = GetTemplateChild(PART_RenderModeComboBox) as ComboBox;
            renderModeComboBox.SelectionChanged += RenderModeComboBox_SelectionChanged;

            pgDetails = GetTemplateChild(PART_Details) as FrostyPropertyGrid;
            pgDetails.OnModified += PgDetails_OnModified;
            pgDetails.OnPreModified += PgDetails_OnPreModified;

            pgPreviewSettings = GetTemplateChild(PART_PreviewSettings) as FrostyPropertyGrid;
            pgPreviewSettings.OnModified += PgPreviewSettings_OnModified;

            pgMeshSettings = GetTemplateChild(PART_MeshSettings) as FrostyPropertyGrid;
            pgMeshSettings.OnPreModified += PgMeshSettings_OnPreModified;
            pgMeshSettings.OnModified += PgMeshSettings_OnModified;

            pgAsset = GetTemplateChild(PART_AssetPropertyGrid) as FrostyPropertyGrid;
            pgAsset.OnModified += PgAsset_OnModified;

            variationsComboBox = GetTemplateChild(PART_VariationComboBox) as ComboBox;
            variationsComboBox.SelectionChanged += VariationsComboBox_SelectionChanged;

            meshTabContent = GetTemplateChild(PART_MeshTabContent) as FrostyDetachedTabControl;
            meshTabControl = GetTemplateChild(PART_MeshTabControl) as FrostyTabControl;

            meshTabContent.HeaderControl = meshTabControl;

#if FROSTY_DEVELOPER
            renderDocButton = GetTemplateChild(PART_RenderDocButton) as Button;
            renderDocButton.Click += RenderDocButton_Click;
#endif

            viewport.Screen = screen;
        }

        private void PgMeshSettings_OnModified(object sender, ItemModifiedEventArgs e)
        {
            if (e.Item.Path.Contains("Sections"))
            {
                int lodIdx = getArrayIndexFromPath("Lods", e.Item.Path);
                int sectionIdx = getArrayIndexFromPath("Sections", e.Item.Path);

                if (e.Item.Name == "Highlight")
                {
                    screen.SetMeshSectionSelected(0, lodIdx, sectionIdx, (bool)e.NewValue);
                }
                else if (e.Item.Name == "Visible")
                {
                    screen.SetMeshSectionVisible(0, lodIdx, sectionIdx, (bool)e.NewValue);
                }
            }
        }

        private void PgMeshSettings_OnPreModified(object sender, ItemPreModifiedEventArgs e)
        {
            if (e.Item.Path.EndsWith("Sections") || e.Item.Path.EndsWith("Lods"))
            {
                e.Ignore = true;
            }
        }

        private void PgAsset_OnModified(object sender, ItemModifiedEventArgs e)
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                if (e.Item.Path.Contains("BoolParameters") || e.Item.Path.Contains("VectorParameters") || e.Item.Path.Contains("TextureParameters") || e.Item.Path.Contains("ConditionalParameters"))
                {
                    dynamic ebxData = RootObject;

                    foreach (var sbd in shaderBlockDepots)
                    {
                        for (int lodIndex = 0; lodIndex < meshSet.Lods.Count; lodIndex++)
                        {
                            MeshSetLod lod = meshSet.Lods[lodIndex];
                            ShaderBlockEntry sbe = sbd.GetSectionEntry(lodIndex);

                            int index = 0;
                            foreach (MeshSetSection section in lod.Sections)
                            {
                                dynamic material = ebxData.Materials[section.MaterialId].Internal;
                                ShaderPersistentParamDbBlock texturesBlock = sbe.GetTextureParams(index);
                                ShaderPersistentParamDbBlock paramsBlock = sbe.GetParams(index);
                                index++;

                                foreach (dynamic param in material.Shader.BoolParameters)
                                {
                                    string paramName = param.ParameterName;
                                    bool value = param.Value;

                                    paramsBlock.SetParameterValue(paramName, value);
                                }
                                foreach (dynamic param in material.Shader.VectorParameters)
                                {
                                    string paramName = param.ParameterName;
                                    dynamic vec = param.Value;

                                    paramsBlock.SetParameterValue(paramName, new float[] { vec.x, vec.y, vec.z, vec.w });
                                }
                                foreach (dynamic param in material.Shader.ConditionalParameters)
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
                                foreach (dynamic param in material.Shader.TextureParameters)
                                {
                                    string paramName = param.ParameterName;
                                    PointerRef value = param.Value;

                                    texturesBlock.SetParameterValue(paramName, value.External.ClassGuid);
                                }

                                texturesBlock.IsModified = true;
                                paramsBlock.IsModified = true;
                            }
                        }

                        ulong resRid = ((dynamic)RootObject).MeshSetResource;
                        ResAssetEntry resEntry = App.AssetManager.GetResEntry(resRid);

                        App.AssetManager.ModifyRes(sbd.ResourceId, sbd);
                        AssetEntry.LinkAsset(resEntry);
                    }
                }
            }
        }

        private void PgDetails_OnPreModified(object sender, ItemPreModifiedEventArgs e)
        {
            if (!e.Item.Path.Contains("Preview"))
                e.Ignore = true;
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new ToolbarItem("Export", "Export MeshSet", "Images/Export.png", new RelayCommand((object state) => { ExportButton_Click(this, new RoutedEventArgs()); })),
                new ToolbarItem("Import", "Import MeshSet", "Images/Import.png", new RelayCommand((object state) => { ImportButton_Click(this, new RoutedEventArgs()); })),
            };
        }

#if FROSTY_DEVELOPER
        private void RenderDocButton_Click(object sender, RoutedEventArgs e)
        {
            // begin frame capture on the next frame
            screen.CaptureNextFrame();
        }
#endif

        private int getArrayIndexFromPath(string arrayName, string path)
        {
            int idx = path.IndexOf(arrayName + ".");

            if (idx == -1)
                return -1;

            path = path.Substring(idx + arrayName.Length + 1);
            path = path.Substring(0, path.IndexOf(']')).Trim('[', ']');

            return int.Parse(path);
        }

        private void PgPreviewSettings_OnModified(object sender, ItemModifiedEventArgs e)
        {
            if (e.Item.Name.Contains("SunPosition"))
            {
                screen.SunPosition = SharpDXUtils.FromVec3(previewSettings.SunPosition);
            }
            else if (e.Item.Name.Contains("SunIntensity"))
            {
                screen.SunIntensity = previewSettings.SunIntensity;
            }
            else if (e.Item.Name.Contains("SunAngularRadius"))
            {
                screen.SunAngularRadius = previewSettings.SunAngularRadius;
            }
#if FROSTY_DEVELOPER
            else if (e.Item.Name.Contains("EV100"))
            {
                screen.MinEV100 = previewSettings.MinEV100;
                screen.MaxEV100 = previewSettings.MaxEV100;
            }
            else if (e.Item.Path.Contains("Animation"))
            {
                screen.SetAnimation(LoadAnim(previewSettings.Animation));
            }
            else if (e.Item.Name.Contains("iDepthBias") || e.Item.Name.Contains("fSlopeScaledDepthBias") || e.Item.Name.Contains("fDistanceBiasMin") || e.Item.Name.Contains("fDistanceBiasFactor") || e.Item.Name.Contains("fDistanceBiasThreshold") || e.Item.Name.Contains("fDistanceBiasPower"))
            {
                screen.iDepthBias = previewSettings.iDepthBias;
                screen.fSlopeScaledDepthBias = previewSettings.fSlopeScaledDepthBias;
                screen.fDistanceBiasMin = previewSettings.fDistanceBiasMin;
                screen.fDistanceBiasFactor = previewSettings.fDistanceBiasFactor;
                screen.fDistanceBiasThreshold = previewSettings.fDistanceBiasThreshold;
                screen.fDistanceBiasPower = previewSettings.fDistanceBiasPower;
            }
#endif
            else if (e.Item.Name.Contains("CameraSpeedMultiplier"))
            {
                screen.camera.SetMoveScaler((float)Math.Pow(previewSettings.CameraSpeedMultiplier, 5));
            }
            else if (e.Item.Name.Contains("ColorLookupTable"))
            {
                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(previewSettings.ColorLookupTable.External.FileGuid);
                screen.SetLookupTableTexture(entry);
            }
            else if (e.Item.Path.Contains("PreviewLights"))
            {
                string path = e.Item.Path;
                int idx = getArrayIndexFromPath("PreviewLights", path);
                PreviewLightData lightEntity = null;

                if (e.OldValue == null)
                {
                    // new light
                    lightEntity = e.NewValue as PreviewLightData;
                }
                else if (e.NewValue == null)
                {
                    // light being removed
                    lightEntity = e.OldValue as PreviewLightData;
                    screen.RemoveLight(lightEntity.LightId);
                    return;
                }
                else if (e.NewValue is int count)
                {
                    if (count == 0)
                    {
                        screen.ClearLights();
                        return;
                    }
                }
                else
                {
                    if (idx == -1)
                        return;
                    lightEntity = previewSettings.PreviewLights[idx];
                }

                Matrix transform = SharpDXUtils.FromLinearTransform(lightEntity.Transform);
                Vector3 color = SharpDXUtils.FromVec3(lightEntity.Color);

                if (lightEntity.LightId == -1)
                {
                    // add new light to renderer
                    lightEntity.LightId = screen.AddLight(LightRenderType.Sphere, transform, color, lightEntity.Intensity, lightEntity.AttenuationRadius, lightEntity.SphereRadius);
                }
                else
                {
                    // modify existing light
                    screen.ModifyLight(lightEntity.LightId, transform, color, lightEntity.Intensity, lightEntity.AttenuationRadius, lightEntity.SphereRadius);
                }
            }
            else if (e.Item.Name == "LightProbeTexture")
            {
                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(previewSettings.LightProbeTexture.External.FileGuid);
                screen.SetDistantLightProbeTexture(entry);
            }
            else if (e.Item.Name == "LightProbeIntensity")
            {
                screen.LightProbeIntensity = previewSettings.LightProbeIntensity;
            }
            else if (e.Item.Name == "PreviewMeshes")
            {
                if (e.NewValue == null)
                {
                    // mesh being removed
                    PreviewMeshData meshData = e.OldValue as PreviewMeshData;
                    if (meshData.MeshId != -1)
                        screen.RemoveMesh(meshData.MeshId);
                }
                else if (e.NewValue is List<PreviewMeshData> list)
                {
                    if (list.Count == 0)
                        screen.ClearMeshes();
                }
            }
            else
            {
                string path = e.Item.Path;
                int idx = getArrayIndexFromPath("PreviewMeshes", path);

                if (idx == -1)
                    return;

                PreviewMeshData meshData = previewSettings.PreviewMeshes[idx];
                if (e.Item.Name == "Mesh")
                {
                    if (meshData.MeshId != -1)
                    {
                        // remove old mesh
                        screen.RemoveMesh(meshData.MeshId);
                    }

                    // load new mesh if specified
                    if (meshData.Mesh.Type != PointerRefType.Null)
                    {
                        EbxAssetEntry ebxEntry = App.AssetManager.GetEbxEntry(meshData.Mesh.External.FileGuid);
                        meshData.Asset = App.AssetManager.GetEbx(ebxEntry);

                        ulong resRid = ((dynamic)meshData.Asset.RootObject).MeshSetResource;
                        MeshSet previewMeshSet = App.AssetManager.GetResAs<MeshSet>(App.AssetManager.GetResEntry(resRid));
                        Matrix transform = SharpDXUtils.FromLinearTransform(meshData.Transform);

                        // add to renderer
                        meshData.MeshId = screen.AddMesh(previewMeshSet, new MeshMaterialCollection(meshData.Asset, meshData.Variation), /*Matrix.Scaling(1, 1, -1) **/ transform, LoadPose(ebxEntry.Filename, meshData.Asset));
                    }
                }
                else if (e.Item.Name == "Variation")
                {
                    // load variation
                    if (meshData.MeshId != -1)
                        screen.LoadMaterials(meshData.MeshId, new MeshMaterialCollection(meshData.Asset, meshData.Variation));
                }
                else if (e.Item.Path.Contains("Transform"))
                {
                    // change transform
                    if (meshData.MeshId != -1)
                    {
                        Matrix transform = SharpDXUtils.FromLinearTransform(meshData.Transform);
                        screen.SetTransform(meshData.MeshId, /*Matrix.Scaling(1, 1, -1) **/ transform);
                    }
                }
            }
        }

        private void VariationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedVariationsIndex = variationsComboBox.SelectedIndex;
            MeshSetVariationDetails variation = variations[selectedVariationsIndex];
            pgDetails.SetClass(variation);
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save material info", "*.xml (XML Files)|*.xml", "MaterialInfo");
            if (sfd.ShowDialog() == true)
            {
                MeshMaterialCollection materials = GetVariation(selectedPreviewIndex);
                FrostyTaskWindow.Show("Extracting Material Info", "", (task) =>
                {
                    using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                    {
                        int index = 0;
                        foreach (MeshMaterial material in materials)
                        {
                            MeshSetSection section = meshSet.Lods[0].Sections.Find((MeshSetSection a) => a.MaterialId == index);
                            if (section == null)
                                continue;

                            EbxAssetEntry shaderEntry = App.AssetManager.GetEbxEntry(material.Shader.External.FileGuid);
                            if (shaderEntry == null)
                                continue;

                            writer.WriteLine("<!-- " + shaderEntry.Name + " -->");
                            writer.WriteLine("<shader profile=\"" + ProfilesLibrary.ProfileName + "\">");
                            writer.WriteLine("\t<permutations>");
                            writer.WriteLine("\t\t<permutation>");
                            writer.WriteLine("\t\t\t<vertexshader>");
                            writer.WriteLine("\t\t\t\t<vertexlayout>");
                            foreach (GeometryDeclarationDesc.Element elem in section.GeometryDeclDesc[0].Elements)
                            {
                                if (elem.Usage == VertexElementUsage.Unknown)
                                    continue;

                                string line = "\t\t\t\t\t<layoutelement usage=\"" + elem.Usage + "\" format=\"" + elem.Format + "\" ";
                                if (section.GeometryDeclDesc[0].StreamCount > 1)
                                    line += "stream=\"" + elem.StreamIndex + "\"";
                                line += "/>";
                                writer.WriteLine(line);
                            }
                            writer.WriteLine("\t\t\t\t</vertexlayout>");
                            writer.WriteLine("\t\t\t</vertexshader>");
                            writer.WriteLine("\t\t\t<pixelshader>");
                            writer.WriteLine("\t\t\t\t<parameters>");
                            foreach (dynamic boolParam in material.BoolParameters)
                            {
                                string paramName = boolParam.ParameterName;
                                writer.WriteLine("\t\t\t\t\t<parameter name=\"" + paramName + "\" type=\"Bool\"/>");
                            }
                            foreach (dynamic vecParam in material.VectorParameters)
                            {
                                string paramName = vecParam.ParameterName;
                                writer.WriteLine("\t\t\t\t\t<parameter name=\"" + paramName + "\" type=\"Float4\"/>");
                            }
                            writer.WriteLine("\t\t\t\t</parameters>");
                            writer.WriteLine("\t\t\t\t<textures>");
                            foreach (dynamic texParam in material.TextureParameters)
                            {
                                string paramName = texParam.ParameterName;
                                writer.WriteLine("\t\t\t\t\t<texture name=\"" + paramName + "\" type=\"<Replace>\"/>");
                            }
                            writer.WriteLine("\t\t\t\t</textures>");
                            writer.WriteLine("\t\t\t</pixelshader>");
                            writer.WriteLine("\t\t</permutation>");
                            writer.WriteLine("\t</permutation>");
                            writer.WriteLine("</shader>");
                        }
                    }
                });
            }
        }

        private void PgDetails_OnModified(object sender, ItemModifiedEventArgs e)
        {
            if (pgDetails.SelectedClass is MeshSetVariationDetails variation && variation.Preview)
                screen.LoadMaterials(0, GetVariation(variation));
        }

        protected override void InvokeOnAssetModified()
        {
            base.InvokeOnAssetModified();
            if (selectedPreviewIndex != -1)
                screen.LoadMaterials(0, GetVariation(selectedPreviewIndex));
        }

        private void RenderModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screen == null)
                return;
            screen.RenderMode = (DebugRenderMode)renderModeComboBox.SelectedIndex;
        }

        private void FrostyMeshSetEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                ulong resRid = ((dynamic)RootObject).MeshSetResource;
                ResAssetEntry rEntry = App.AssetManager.GetResEntry(resRid);

                meshSet = App.AssetManager.GetResAs<MeshSet>(rEntry);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20)
                    meshSet.TangentSpaceCompressionType = (TangentSpaceCompressionType)((dynamic)RootObject).TangentSpaceCompressionType;

                meshGuid = App.AssetManager.GetEbxEntry(meshSet.FullName).Guid;

                if (!MeshVariationDb.IsLoaded)
                {
                    FrostyTaskWindow.Show("Loading Variations", "", MeshVariationDb.LoadVariations);
                }
                MeshVariationDb.LoadModifiedVariations();
                variations = LoadVariations();
                variations.Sort((MeshSetVariationDetails a, MeshSetVariationDetails b) =>
                {
                    if (a.Name.Equals("Default"))
                        return -1;

                    return b.Name.Equals("Default") ? 1 : a.Name.CompareTo(b.Name);
                });

                UpdateMeshSettings();

                pgPreviewSettings.SetClass(previewSettings);
                screen.AddMesh(meshSet, GetVariation(selectedPreviewIndex), Matrix.Identity /*Matrix.Scaling(1,1,-1)*/, LoadPose(AssetEntry.Filename, asset));

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
                {
                    string path = AssetEntry.Name.ToLower();
                    foreach (ResAssetEntry resEntry in App.AssetManager.EnumerateRes(resType: (uint)ResourceType.ShaderBlockDepot))
                    {
                        if (resEntry.Name.StartsWith(path))
                        {
                            shaderBlockDepots.Add(App.AssetManager.GetResAs<ShaderBlockDepot>(resEntry));
                        }
                    }

                    MeshSetVariationDetails variation = variations[0];
                    int index = 0;

                    dynamic ebxData = RootObject;
                    foreach (PointerRef pr in ebxData.Materials)
                    {
                        dynamic material = pr.Internal;
                        if (material.Shader.TextureParameters.Count == 0)
                        {
                            MeshMaterial varMaterial = variation.MaterialCollection.Materials[index++];
                            foreach (dynamic param in varMaterial.TextureParameters)
                                material.Shader.TextureParameters.Add(param);
                        }
                    }
                }

                firstTimeLoad = false;
            }

            UpdateControls();
        }

        private void UpdateMeshSettings()
        {
            meshSettings = new MeshSetMeshSettings();
            dynamic materials = ((dynamic)RootObject).Materials;

            foreach (MeshSetLod lod in meshSet.Lods)
            {
                PreviewMeshLodData lodData = new PreviewMeshLodData() { Name = lod.String03 };
                foreach (MeshSetSection section in lod.Sections)
                {
                    if (lod.IsSectionRenderable(section) && section.PrimitiveCount > 0)
                    {
                        dynamic material = materials[section.MaterialId].Internal;
                        material.__Id = section.Name;

                        PreviewMeshSectionData sectionData = new PreviewMeshSectionData() { Visible = true, MaterialId = materials[section.MaterialId], MaxBonesPerVertex = section.BonesPerVertex };
                        lodData.Sections.Add(sectionData);

                        foreach (var elem in section.GeometryDeclDesc[0].Elements)
                        {
                            if (elem.Usage >= VertexElementUsage.TexCoord0 && elem.Usage <= VertexElementUsage.TexCoord7)
                                sectionData.NumUVChannels++;
                            else if (elem.Usage >= VertexElementUsage.Color0 && elem.Usage <= VertexElementUsage.Color1)
                                sectionData.NumColorChannels++;
                            else
                            {
                                switch (elem.Usage)
                                {
                                    case VertexElementUsage.Pos:
                                    case VertexElementUsage.Normal:
                                    case VertexElementUsage.Binormal:
                                    case VertexElementUsage.BinormalSign:
                                    case VertexElementUsage.Tangent:
                                    case VertexElementUsage.TangentSpace:
                                    case VertexElementUsage.BoneIndices:
                                    case VertexElementUsage.BoneIndices2:
                                    case VertexElementUsage.BoneWeights:
                                    case VertexElementUsage.BoneWeights2:
                                    case VertexElementUsage.Unknown:
                                        break;

                                    default:
                                        sectionData.AdditionalChannels.Add(elem.Usage.ToString());
                                        break;
                                }
                            }
                        }
                    }
                }
                meshSettings.Lods.Add(lodData);
            }

            pgMeshSettings.SetClass(meshSettings);
        }

        private MeshRenderAnim LoadAnim(string name)
        {
            MeshRenderAnim anim = null;
            if (File.Exists(name))
            {
                using (NativeReader reader = new NativeReader(new FileStream(name, FileMode.Open, FileAccess.Read)))
                {
                    int frameCount = reader.ReadInt();
                    int keyFrameCount = reader.ReadInt();

                    anim = new MeshRenderAnim(frameCount);
                    Dictionary<string, MeshRenderAnim.Bone> bones = new Dictionary<string, MeshRenderAnim.Bone>();

                    for (int i = 0; i < keyFrameCount; i++)
                    {
                        int frameTime = reader.ReadInt();
                        int boneCount = reader.ReadInt();

                        for (int j = 0; j < boneCount; j++)
                        {
                            string boneName = reader.ReadNullTerminatedString().ToLower();
                            int type = reader.ReadInt();

                            if (!bones.ContainsKey(boneName))
                                bones.Add(boneName, new MeshRenderAnim.Bone() { NameHash = Fnv1.HashString(boneName) });

                            switch (type)
                            {
                                case 0x0E:
                                    {
                                        Quaternion q = new Quaternion()
                                        {
                                            X = reader.ReadFloat(),
                                            Y = reader.ReadFloat(),
                                            Z = reader.ReadFloat(),
                                            W = reader.ReadFloat()
                                        };
                                        bones[boneName].Rotations.Add(new MeshRenderAnim.Keyframe<Quaternion>() { FrameTime = frameTime, Value = q });
                                    }
                                    break;

                                case 0x0F:
                                    reader.ReadFloat();
                                    break;

                                case 0x7a2e5497:
                                    {
                                        Vector3 t = new Vector3()
                                        {
                                            X = reader.ReadFloat(),
                                            Y = reader.ReadFloat(),
                                            Z = reader.ReadFloat()
                                        };
                                        bones[boneName].Translations.Add(new MeshRenderAnim.Keyframe<Vector3>() { FrameTime = frameTime, Value = t });
                                    }
                                    break;

                                case 0x7a2e53c6:
                                    {
                                        Vector3 s = new Vector3()
                                        {
                                            X = reader.ReadFloat(),
                                            Y = reader.ReadFloat(),
                                            Z = reader.ReadFloat()
                                        };
                                        bones[boneName].Scales.Add(new MeshRenderAnim.Keyframe<Vector3>() { FrameTime = frameTime, Value = s });
                                    }
                                    break;
                            }
                        }
                    }

                    anim.AddBones(bones.Values);
                }
            }
            return anim;
        }

        private MeshRenderSkeleton LoadPose(string name, EbxAsset meshAsset)
        {
            MeshRenderSkeleton skeleton = new MeshRenderSkeleton();
            Dictionary<string, Tuple<Vector3, Vector3, Quaternion>> facePose = new Dictionary<string, Tuple<Vector3, Vector3, Quaternion>>();

            if (File.Exists("Faces/" + name + ".bin"))
            {
                string skeletonName = "";
                using (NativeReader poseReader = new NativeReader(new FileStream("Faces/" + name + ".bin", FileMode.Open, FileAccess.Read)))
                {
                    byte b = poseReader.ReadByte();
                    if (b == 0x00)
                    {
                        skeletonName = poseReader.ReadNullTerminatedString();
                    }
                    else
                        poseReader.Position--;

                    while (poseReader.Position < poseReader.Length)
                    {
                        string str = poseReader.ReadNullTerminatedString().ToLower();
                        uint type = poseReader.ReadUInt();

                        Quaternion q = new Quaternion(0, 0, 0, float.MaxValue);
                        Vector3 v = new Vector3(float.MaxValue, 0, 0);
                        Vector3 s = new Vector3(float.MaxValue, 0, 0);

                        if (!facePose.ContainsKey(str))
                            facePose.Add(str, new Tuple<Vector3, Vector3, Quaternion>(s, v, q));

                        if (type == 0xE)
                        {
                            q.X = poseReader.ReadFloat();
                            q.Y = poseReader.ReadFloat();
                            q.Z = poseReader.ReadFloat();
                            q.W = poseReader.ReadFloat();
                            facePose[str] = new Tuple<Vector3, Vector3, Quaternion>(facePose[str].Item1, facePose[str].Item2, q);
                        }
                        else if (type == 0x7a2e5497)
                        {
                            v.X = poseReader.ReadFloat();
                            v.Y = poseReader.ReadFloat();
                            v.Z = poseReader.ReadFloat();
                            facePose[str] = new Tuple<Vector3, Vector3, Quaternion>(facePose[str].Item1, v, facePose[str].Item3);
                        }
                        else if (type == 0x7a2e53c6)
                        {
                            s.X = poseReader.ReadFloat();
                            s.Y = poseReader.ReadFloat();
                            s.Z = poseReader.ReadFloat();
                            facePose[str] = new Tuple<Vector3, Vector3, Quaternion>(s, facePose[str].Item2, facePose[str].Item3);
                        }
                    }
                }

                if (skeletonName == "")
                {
                    switch (ProfilesLibrary.DataVersion)
                    {
                        case (int)ProfileVersion.StarWarsBattlefrontII: skeletonName = "Characters/Rigs/Humanoids/Walrus_HumanMale"; break;
                        case (int)ProfileVersion.Battlefield5: skeletonName = "Characters/skeletons/1P_MaleSoldier_FB"; break;
                        case (int)ProfileVersion.StarWarsBattlefront: skeletonName = "Animations/Rigs/Humanoids/HumanMale"; break;
                        case (int)ProfileVersion.MirrorsEdgeCatalyst: skeletonName = "Characters/Skeletons/Skeleton_Female"; break;
                        case (int)ProfileVersion.MassEffectAndromeda: skeletonName = "Game/characters/_Skeletons/bHMF_skeleton"; break;
                        case (int)ProfileVersion.Battlefield1: skeletonName = "Characters/skeletons/Character/3pAntSkeleton"; break;
                        case (int)ProfileVersion.Anthem: skeletonName = "Animation/HMM/HMM_Skeleton"; break;
                        case (int)ProfileVersion.DragonAgeInquisition: skeletonName = "DA3/Animation/Humanoid/Human/AdultMale/hm_skeleton"; break;
                    }
                }

                if (skeletonName != "")
                {
                    EbxAssetEntry skeletonAssetEntry = App.AssetManager.GetEbxEntry(skeletonName);
                    dynamic skeletonAsset = App.AssetManager.GetEbx(skeletonAssetEntry).RootObject;
                    dynamic boneNames = skeletonAsset.BoneNames;
                    dynamic pose = skeletonAsset.ModelPose;
                    dynamic localPose = skeletonAsset.LocalPose;

                    for (int boneIdx = 0; boneIdx < boneNames.Count; boneIdx++)
                    {
                        string boneName = boneNames[boneIdx];
                        if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
                            boneName = Murmur2.HashString64(boneName, 0x4eb23).ToString("X16");

                        boneName = boneName.ToLower();
                        Matrix boneMatrix = SharpDXUtils.FromLinearTransform(localPose[boneIdx]);

                        if (facePose.ContainsKey(boneName))
                        {
                            Vector3 scale = Vector3.One;
                            Vector3 trans = boneMatrix.TranslationVector;
                            Quaternion rot = Quaternion.RotationMatrix(boneMatrix);
                            Vector3 euler = Vector3.Zero;

                            boneMatrix.Decompose(out scale, out rot, out trans);
                            euler = SharpDXUtils.ExtractEulerAngles(boneMatrix);

                            if (facePose[boneName].Item1.X < float.MaxValue)
                                scale = facePose[boneName].Item1;
                            if (facePose[boneName].Item2.X < float.MaxValue)
                                trans = facePose[boneName].Item2;
                            if (facePose[boneName].Item3.W < float.MaxValue)
                                rot = facePose[boneName].Item3;

                            boneMatrix = Matrix.Scaling(scale) * Matrix.RotationQuaternion(rot) * Matrix.Translation(trans);
                        }

                        Matrix mp = SharpDXUtils.FromLinearTransform(pose[boneIdx]);
                        mp.Invert();

                        boneName = boneNames[boneIdx];
                        boneName = boneName.ToLower();

                        skeleton.AddBone(new MeshRenderSkeleton.Bone()
                        {
                            NameHash = Fnv1.HashString(boneName),
                            ModelPose = mp,
                            LocalPose = boneMatrix,
                            ParentBoneId = skeletonAsset.Hierarchy[boneIdx]
                        });
                    }

                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    {
                        if (TypeLibrary.IsSubClassOf(meshAsset.RootObject, "SkinnedMeshAsset"))
                        {
                            int boneId = 0;

                            // SWBF2 procedural animation bones
                            foreach (dynamic bone in ((dynamic)meshAsset.RootObject).SkinnedProceduralAnimation.Bones)
                            {
                                Matrix mp = SharpDXUtils.FromLinearTransform(bone.LocalPose);
                                Matrix boneMatrix = SharpDXUtils.FromLinearTransform(bone.Pose);
                                mp.Invert();

                                skeleton.AddBone(new MeshRenderSkeleton.Bone()
                                {
                                    NameHash = Fnv1.HashString("PROC_" + boneId++),
                                    ModelPose = mp,
                                    LocalPose = boneMatrix,
                                    ParentBoneId = bone.ParentIndex,
                                    IsProcedural = true
                                });
                            }

                            foreach (dynamic rootPose in ((dynamic)meshAsset.RootObject).SkinnedProceduralAnimation.RootPoses)
                            {
                                int index = rootPose.Index;
                                Matrix mp = SharpDXUtils.FromLinearTransform(rootPose.LocalPose);
                                mp.Invert();

                                skeleton.UpdateBone(index, modelPose: mp);
                            }

#if FROSTY_DEVELOPER
                            foreach (dynamic expression in ((dynamic)meshAsset.RootObject).SkinnedProceduralAnimation.Expressions)
                            {


                                EbxAssetEntry exprEntry = App.AssetManager.GetEbxEntry(expression.Graph.External.FileGuid);
                                if (exprEntry.Filename == "RollBone")
                                {
                                    int matrix1 = -1;
                                    int matrix2 = -1;
                                    int matrix3 = -1;
                                    Vector3 v1 = new Vector3(1, 1, 1);
                                    float f1 = 0.0f;

                                    List<dynamic> runtimeParams = new List<dynamic>();
                                    runtimeParams.AddRange(expression.RuntimeParameters);

                                    dynamic param = runtimeParams.Find((dynamic a) => a.NodeHash == 242850164);
                                    matrix1 = param.IntValue;

                                    param = runtimeParams.Find((dynamic a) => a.NodeHash == -1508053196);
                                    matrix2 = param.IntValue;

                                    param = runtimeParams.Find((dynamic a) => a.NodeHash == -72088988);
                                    matrix3 = param.IntValue;

                                    param = runtimeParams.Find((dynamic a) => a.NodeHash == -612073924);
                                    v1.X = param.FloatValue;

                                    param = runtimeParams.Find((dynamic a) => a.NodeHash == -300227826);
                                    v1.Y = param.FloatValue;

                                    param = runtimeParams.Find((dynamic a) => a.NodeHash == 1831777758);
                                    v1.Z = param.FloatValue;

                                    param = runtimeParams.Find((dynamic a) => a.NodeHash == -382567928);
                                    f1 = param.FloatValue;

                                    MeshRenderSkeleton.TestRollBoneExpression expr = new MeshRenderSkeleton.TestRollBoneExpression(
                                        new MeshRenderSkeleton.ExpressionValue<Vector3>(v1),
                                        new MeshRenderSkeleton.ExpressionValue<float>(f1),
                                        new MeshRenderSkeleton.BoneQueryExpressionValue(skeleton, matrix1),
                                        new MeshRenderSkeleton.BoneQueryExpressionValue(skeleton, matrix2),
                                        new MeshRenderSkeleton.BoneQueryExpressionValue(skeleton, matrix3)
                                    );
                                    skeleton.AddExpression(skeleton.BoneCount + expression.BoneIndices[0], expr);
                                }
                            }
#endif
                        }
                    }
                }
            }

            return skeleton;
        }

        private void UpdateControls()
        {
            lodComboBox.Items.Clear();
            for (int i = 0; i < meshSet.Lods.Count; i++)
                lodComboBox.Items.Add(i);
            lodComboBox.SelectedIndex = 0;

            variationsComboBox.ItemsSource = variations;
            variationsComboBox.SelectedIndex = selectedVariationsIndex;
        }

        private MeshMaterialCollection GetVariation(int index)
        {
            if (index >= variations.Count)
                return null;
            return GetVariation(variations[index]);
        }

        private MeshMaterialCollection GetVariation(MeshSetVariationDetails variation)
        {
            int newSelectedIndex = -1;
            int i = 0;

            foreach (object objClass in variations)
            {
                if (i == selectedPreviewIndex && objClass != variation)
                {
                    MeshSetVariationDetails mvd = objClass as MeshSetVariationDetails;
                    mvd.Preview = false;
                }
                if (objClass == variation)
                {
                    newSelectedIndex = i;
                }
                i++;
            }

            selectedPreviewIndex = newSelectedIndex;
            return new MeshMaterialCollection(asset, variation.Variation);
        }

        private List<MeshSetVariationDetails> LoadVariations()
        {
            //if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19)
            {
                dynamic ebxData = RootObject;
                MeshVariationDbEntry mvEntry = MeshVariationDb.GetVariations((AssetEntry as EbxAssetEntry).Guid);

                if (mvEntry != null)
                {
                    if (objectVariationMapping.Count == 0)
                    {
                        foreach (EbxAssetEntry varEntry in App.AssetManager.EnumerateEbx(type: "ObjectVariation"))
                            objectVariationMapping.Add((uint)Fnv1.HashString(varEntry.Name.ToLower()), varEntry);
                    }

                    List<MeshSetVariationDetails> detailsList = new List<MeshSetVariationDetails>();

                    List<MeshVariation> mVariations = mvEntry.Variations.Values.ToList();
                    mVariations.Sort((MeshVariation a, MeshVariation b) => { return a.AssetNameHash.CompareTo(b.AssetNameHash); });

                    foreach (MeshVariation mv in mVariations)
                    {
                        MeshSetVariationDetails variationDetails = new MeshSetVariationDetails {Name = "Default"};

                        if (objectVariationMapping.ContainsKey(mv.AssetNameHash))
                        {
                            EbxAsset asset = App.AssetManager.GetEbx(objectVariationMapping[mv.AssetNameHash]);
                            AssetClassGuid guid = ((dynamic)asset.RootObject).GetInstanceGuid();

                            variationDetails.Name = objectVariationMapping[mv.AssetNameHash].Filename;
                            variationDetails.Variation = new PointerRef(new EbxImportReference()
                            {
                                FileGuid = objectVariationMapping[mv.AssetNameHash].Guid,
                                ClassGuid = guid.ExportedGuid
                            });
                        }
                        else if (mv.AssetNameHash != 0)
                            continue;

                        for (int i = 0; i < ebxData.Materials.Count; i++)
                        {
                            dynamic material = ebxData.Materials[i].Internal;
                            AssetClassGuid guid = material.GetInstanceGuid();

                            MeshVariationMaterial varMaterial = mv.GetMaterial(guid.ExportedGuid);
                            MeshSetMaterialDetails details = new MeshSetMaterialDetails();
                            variationDetails.MaterialCollection = new MeshMaterialCollection.Container(new MeshMaterialCollection(asset, new PointerRef(varMaterial.MaterialVariationAssetGuid)));
                        }

                        foreach (Tuple<EbxImportReference, int> dbEntries in mv.DbLocations)
                            variationDetails.MeshVariationDbs.Add(new MeshSetVariationEntryDetails()
                            {
                                VariationDb = new PointerRef(dbEntries.Item1),
                                Index = dbEntries.Item2
                            });
                        detailsList.Add(variationDetails);
                        if (detailsList.Count == 1)
                            variationDetails.Preview = true;
                    }

                    return detailsList;
                }
                else
                {
                    List<MeshSetVariationDetails> detailsList = new List<MeshSetVariationDetails>();
                    MeshSetVariationDetails variationDetails = new MeshSetVariationDetails {Name = "Default"};

                    for (int i = 0; i < ebxData.Materials.Count; i++)
                    {
                        dynamic material = ebxData.Materials[i].Internal;
                        if (material == null)
                            continue;
                        MeshSetMaterialDetails details = new MeshSetMaterialDetails();
                        variationDetails.MaterialCollection = new MeshMaterialCollection.Container(new MeshMaterialCollection(asset, new PointerRef()));
                    }

                    detailsList.Add(variationDetails);
                    variationDetails.Preview = true;

                    return detailsList;
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            viewport.SetPaused(true);

            MeshExportSettings settings = (AssetEntry.Type == "SkinnedMeshAsset")
                ? new SkinnedMeshExportSettings()
                : new MeshExportSettings();

            // load settings
            string Version = Config.Get<string>("MeshSetExportVersion", "FBX_2012", ConfigScope.Game);
            string Scale = Config.Get<string>("MeshSetExportScale", "Centimeters", ConfigScope.Game);
            bool flattenHierarchy = Config.Get<bool>("MeshSetExportFlattenHierarchy", false, ConfigScope.Game);
            bool exportSingleLod = Config.Get<bool>("MeshSetExportExportSingleLod", false, ConfigScope.Game);
            bool exportAdditionalMeshes = Config.Get<bool>("MeshSetExportExportAdditionalMeshes", false, ConfigScope.Game);
            string skeleton = Config.Get<string>("MeshSetExportSkeleton", "", ConfigScope.Game);

            //string Version = Config.Get<string>("MeshSetExport", "Version", "FBX_2012");
            //string Scale = Config.Get<string>("MeshSetExport", "Scale", "Centimeters");
            //bool flattenHierarchy = Config.Get<bool>("MeshSetExport", "FlattenHierarchy", false);
            //bool exportAdditionalMeshes = Config.Get<bool>("MeshSetExport", "ExportAdditionalMeshes", false);
            //string skeleton = Config.Get<string>("MeshSetExport", "Skeleton", "");

            settings.Version = (MeshExportVersion)Enum.Parse(typeof(MeshExportVersion), Version);
            settings.Scale = (MeshExportScale)Enum.Parse(typeof(MeshExportScale), Scale);
            settings.FlattenHierarchy = flattenHierarchy;
            settings.ExportSingleLod = exportSingleLod;
            settings.ExportAdditionalMeshes = exportAdditionalMeshes;

            if (settings is SkinnedMeshExportSettings exportSettings)
                exportSettings.SkeletonAsset = skeleton;

            // show settings box
            if (FrostyImportExportBox.Show<MeshExportSettings>("Mesh Export Settings", FrostyImportExportType.Export, settings) == MessageBoxResult.OK)
            {
                string filter = "*.fbx (FBX Binary File)|*.fbx|*.fbx (FBX ASCII File)|*.fbx";
                if (!(settings is SkinnedMeshExportSettings) || ((SkinnedMeshExportSettings)settings).SkeletonAsset == "")
                    filter += "|*.obj (OBJ File)|*.obj";

                FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save MeshSet", filter, "Mesh", AssetEntry.Filename);
                if (sfd.ShowDialog())
                {
                    if (meshSet.Type == MeshType.MeshType_Skinned)
                        skeleton = ((SkinnedMeshExportSettings)settings).SkeletonAsset;

                    EbxAssetEntry entry = App.AssetManager.GetEbxEntry(((dynamic)RootObject).Name);

                    List<MeshSet> meshSets = new List<MeshSet> {meshSet};

                    if (settings.ExportAdditionalMeshes)
                    {
                        // collect all additional meshes added to the viewport
                        foreach (var previewMesh in previewSettings.PreviewMeshes)
                        {
                            meshSets.Add(screen.GetMesh(previewMesh.MeshId));
                        }
                    }

                    // fbx/obj exporting
                    string[] fileTypes = new string[] { "binary", "ascii", "obj" };
                    FrostyTaskWindow.Show("Exporting MeshSet", "", (task) =>
                    {
                        FBXExporter exporter = new FBXExporter(task);
                        exporter.ExportFBX(RootObject, sfd.FileName, settings.Version.ToString().Replace("FBX_", ""), settings.Scale.ToString(), settings.FlattenHierarchy, settings.ExportSingleLod, skeleton, fileTypes[sfd.FilterIndex - 1], meshSets.ToArray());
                    });

                    logger.Log("Exported {0} to {1}", entry.Name, sfd.FileName);

                    // save settings
                    Config.Add("MeshSetExportVersion", settings.Version.ToString(), ConfigScope.Game);
                    Config.Add("MeshSetExportScale", settings.Scale.ToString(), ConfigScope.Game);
                    Config.Add("MeshSetExportFlattenHierarchy", settings.FlattenHierarchy, ConfigScope.Game);
                    Config.Add("MeshSetExportExportSingleLod", settings.ExportSingleLod, ConfigScope.Game);
                    Config.Add("MeshSetExportExportAdditionalMeshes", settings.ExportAdditionalMeshes, ConfigScope.Game);

                    //Config.Add("MeshSetExport", "Version", settings.Version.ToString());
                    //Config.Add("MeshSetExport", "Scale", settings.Scale.ToString());
                    //Config.Add("MeshSetExport", "FlattenHierarchy", settings.FlattenHierarchy);
                    //Config.Add("MeshSetExport", "ExportAdditionalMeshes", settings.ExportAdditionalMeshes);

                    if (settings is SkinnedMeshExportSettings meshExportSettings)
                        Config.Add("MeshSetExportSkeleton", meshExportSettings.SkeletonAsset, ConfigScope.Game);
                    //Config.Add("MeshSetExport", "Skeleton", meshExportSettings.SkeletonAsset);

                    Config.Save();
                }
            }

            viewport.SetPaused(false);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            viewport.SetPaused(true);

            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import MeshSet", "*.fbx (FBX Files)|*.fbx", "Mesh");
            if (ofd.ShowDialog())
            {
                FrostyMeshImportSettings settings = null;
                bool bOk = false;

                if (meshSet.Type == MeshType.MeshType_Skinned)
                {
                    settings = new FrostyMeshImportSettings { SkeletonAsset = Config.Get<string>("MeshSetImportSkeleton", "", ConfigScope.Game) };
                    //settings = new FrostyMeshImportSettings {SkeletonAsset = Config.Get<string>("MeshSetImport", "Skeleton", "")};

                    if (FrostyImportExportBox.Show<FrostyMeshImportSettings>("Import Skinned Mesh", FrostyImportExportType.Import, settings) == MessageBoxResult.OK)
                    {
                        bOk = true;
                        Config.Add("MeshSetImportSkeleton", settings.SkeletonAsset, ConfigScope.Game);
                        //Config.Add("MeshSetImport", "Skeleton", settings.SkeletonAsset);
                    }
                }
                else
                    bOk = true;

                if (bOk)
                {
                    ulong resRid = ((dynamic)RootObject).MeshSetResource;
                    ResAssetEntry resEntry = App.AssetManager.GetResEntry(resRid);
                    Stream resStream = App.AssetManager.GetRes(resEntry);

                    EbxAsset localAsset = asset;
                    EbxAssetEntry localEntry = AssetEntry as EbxAssetEntry;
                    //List<ShaderBlockEntry> tmpShaderBlockEntries = new List<ShaderBlockEntry>();

                    FrostyTaskWindow.Show("Importing", "", (task) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                // import
                                FBXImporter importer = new FBXImporter(logger);
                                importer.ImportFBX(ofd.FileName, meshSet, localAsset, localEntry, settings);
                            }
                            catch (Exception exp)
                            {
                                App.AssetManager.RevertAsset(AssetEntry);
                                logger.LogError(exp.Message);
                            }
                        });
                    });

                    // @todo: Reload the main mesh shader block depot

                    // update UI
                    screen.ClearMeshes(clearAll: true);
                    screen.AddMesh(meshSet, GetVariation(selectedPreviewIndex), Matrix.Identity /*Matrix.Scaling(1,1,-1)*/, LoadPose(AssetEntry.Filename, asset));

                    UpdateMeshSettings();
                    UpdateControls();

                    InvokeOnAssetModified();
                }
            }

            viewport.SetPaused(false);
        }

        private void LodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lodComboBox.SelectedIndex == -1)
                return;
            screen.CurrentLOD = lodComboBox.SelectedIndex;
        }

        public override void Closed()
        {
            viewport.Shutdown();
        }

        //private ShaderBlockEntry FindShaderBlockEntry(int lodIndex)
        //{
        //    foreach (ShaderBlockEntry entry in shaderBlockEntries)
        //    {
        //        MeshParamDbBlock meshBlock = entry.GetMeshParams(0);
        //        if (meshBlock.LodIndex == lodIndex)
        //            return entry;
        //    }
        //    return null;
        //}
    }
}
