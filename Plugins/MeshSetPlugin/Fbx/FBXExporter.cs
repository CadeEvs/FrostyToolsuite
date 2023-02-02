using Frosty.Core;
using Frosty.Core.Screens;
using Frosty.Core.Viewport;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.IO;
using MeshSetPlugin.Fbx;
using MeshSetPlugin.Resources;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace MeshSetPlugin
{
    public class FBXExporter
    {
        private int m_totalExportCount = 0;
        private int m_currentProgress = 0;
        private int m_boneCount;
        private FbxGeometryConverter m_geomConverter;
        private bool m_flattenHierarchy = true;
        private bool m_exportSingleLod = false;
        private FrostyTaskWindow m_task;

        public FBXExporter(FrostyTaskWindow inTask)
        {
            m_task = inTask;
        }

        /// <summary>
        /// Exports the specified mesh to a FBX file
        /// </summary>
        public void ExportFBX(dynamic meshAsset, string filename, string fbxVersion, string units, bool inFlattenHierarchy, bool inExportSingleLod, string skeleton, string fileType, params MeshSet[] meshSets)
        {
            m_flattenHierarchy = inFlattenHierarchy;
            m_exportSingleLod = inExportSingleLod;
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

                m_geomConverter = new FbxGeometryConverter(manager);

                switch (units)
                {
                    case "Millimeters": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Millimeters); break;
                    case "Centimeters": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Centimeters); break;
                    case "Meters": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Meters); break;
                    case "Kilometers": scene.GlobalSettings.SetSystemUnit(FbxSystemUnit.Kilometers); break;
                }

                m_totalExportCount++;
                foreach (MeshSet meshSet in meshSets)
                {
                    foreach (MeshSetLod lod in meshSet.Lods)
                    {
                        foreach (MeshSetSection section in lod.Sections)
                        {
                            m_totalExportCount += (section.Name != "") ? 1 : 0;
                        }
                    }
                }

                // only use the primary mesh asset to export skeleton
                List<FbxNode> boneNodes = new List<FbxNode>();
                if (meshSets[0].Lods[0].Type == MeshType.MeshType_Skinned && skeleton != "")
                {
                    // skinned mesh requires external skeleton
                    m_task.Update("Writing skeleton");
                    FbxNode rootNode = FBXCreateSkeleton(scene, meshAsset, skeleton, ref boneNodes);
                    scene.RootNode.AddChild(rootNode);
                }
                else if (meshSets[0].Lods[0].Type == MeshType.MeshType_Composite)
                {
                    // composite skeleton has parts defined in mesh
                    m_task.Update("Writing composite skeleton");
                    FbxNode rootNode = FBXCreateCompositeSkeleton(scene, meshSets[0].Lods[0].PartTransforms, ref boneNodes);
                    scene.RootNode.AddChild(rootNode);
                }

                m_currentProgress++;
                foreach (MeshSet meshSet in meshSets)
                {
                    foreach (MeshSetLod lod in meshSet.Lods)
                    {
                        m_task.Update("Writing " + lod.ShortName);
                        FBXCreateMesh(scene, lod, boneNodes);
                        if (m_exportSingleLod)
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
                                {
                                    exporter.Export(scene);
                                }

                                break;
                            }
                        }
                    }
                }

                m_geomConverter.Dispose();
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
                    {
                        continue;
                    }

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
                                {
                                    exporter.Export(scene);
                                }

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
            m_boneCount = skeletonAsset.BoneNames.Count;

            for (int boneIdx = 0; boneIdx < m_boneCount; boneIdx++)
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

            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5,
                ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.StarWarsSquadrons,
                ProfileVersion.Madden22, ProfileVersion.Fifa22,
                ProfileVersion.Battlefield2042, ProfileVersion.Madden23,
                ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace) && meshAsset != null)
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

            FbxNode meshNode = (m_flattenHierarchy)
                ? scene.RootNode
                : new FbxNode(scene, lod.ShortName);

            Stream chunkStream = (lod.ChunkId != Guid.Empty)
                ? App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(lod.ChunkId))
                : new MemoryStream(lod.InlineData);

            using (NativeReader reader = new NativeReader(chunkStream))
            {
                foreach (MeshSetSection section in lod.Sections)
                {
                    if (!lod.IsSectionRenderable(section))
                    {
                        continue;
                    }

                    m_task.Update(progress: (m_currentProgress++ / (double)m_totalExportCount) * 100.0);

                    FbxNode actor = FBXExportSubObject(scene, section, lod.VertexBufferSize, indexSize, reader);
                    if (m_flattenHierarchy)
                    {
                        actor.Name = $"{section.Name}:lod.00{lod.ShortName.Last()}";
                    }

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
                                {
                                    boneList.Add(i);
                                }
                            }
                        }

                        // fifa doesnt appear to use the sub bone list in mesh sections (directly maps to indices)
                        if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                            ProfileVersion.Madden19, ProfileVersion.Fifa19,
                            ProfileVersion.Battlefield5, ProfileVersion.Fifa20))
                        {
                            boneList.Clear();
                            for (ushort i = 0; i < boneNodes.Count; i++)
                            {
                                boneList.Add(i);
                            }
                        }

                        FBXCreateSkin(scene, section, actor, boneNodes, boneList, lod.Type, reader);
                        FBXCreateBindPose(scene, section, actor);
                    }
                }
            }

            if (!m_flattenHierarchy)
            {
                scene.RootNode.AddChild(meshNode);
            }
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
                    {
                        continue;
                    }

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
                {
                    boneWeights[0] = 1.0f;
                }

                for (int j = 0; j < 8; j++)
                {
                    if (boneWeights[j] > 0.0f)
                    {
                        int subIndex = boneIndices[j];
                        //if (!ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII,
                        //    ProfileVersion.Battlefield5,
                        //    ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                        //    ProfileVersion.StarWarsSquadrons, ProfileVersion.Fifa22,
                        //    ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound))
                        //{
                        //    subIndex = boneList[subIndex];
                        //}

                        // account for proc bones
                        if ((subIndex & 0x8000) != 0)
                        {
                            subIndex = (subIndex - 0x8000) + m_boneCount;
                        }
                        else
                        {
                            subIndex = boneList[subIndex];
                        }

                        while (subIndex >= boneClusters.Count)
                        {
                            boneClusters.Add(null);
                        }

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
                FbxPose fbxPose = new FbxPose(scene, section.Name) { IsBindPose = true };

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

            bool packedBinormal = false;
            bool tangentSpaceUnpack = false;

            Vector3 normal = new Vector3();
            Vector3 tangent = new Vector3();
            Vector3 binormal = new Vector3();

            List<float> binormalSigns = new List<float>();
            List<object> tangentSpace = new List<object>();

            for (int j = 0; j < section.GeometryDeclDesc[0].Streams.Length; j++)
            {
                GeometryDeclarationDesc.Stream stream = section.GeometryDeclDesc[0].Streams[j];
                if (stream.VertexStride == 0)
                {
                    continue;
                }

                for (int i = 0; i < section.VertexCount; i++)
                {
                    int currentStride = 0;
                    foreach (GeometryDeclarationDesc.Element elem in section.GeometryDeclDesc[0].Elements)
                    {
                        if (elem.Usage == VertexElementUsage.Unknown)
                        {
                            continue;
                        }

                        if (elem.StreamIndex == j && currentStride < stream.VertexStride)
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
                                        {
                                            reader.ReadUShort(); // most likely packed TBN
                                        }
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
                                reader.ReadUShort();
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
                                reader.ReadUShort();
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

                                // SubMaterialIndex
                                // if no shader specified in ebx its used as color
                                // else its used as an index for the material
                                else if (elem.Format == VertexElementFormat.UByte4)
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

                                    byte r = reader.ReadByte();
                                    byte g = reader.ReadByte();
                                    byte b = reader.ReadByte();
                                    byte a = reader.ReadByte();

                                    layerElemVertexColor[colorMapping[elem.Usage]].DirectArray.Add(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
                                }
                                else
                                {
                                    reader.Position += elem.Size;
                                }
                            }

                            currentStride += elem.Size;
                        }
                    }

                    // rivals pads the vertex stride
                    if (currentStride != stream.VertexStride)
                    {
                        reader.Position += stream.VertexStride - currentStride;
                    }
                }
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
                    if (layerElemNormal != null)
                    {
                        layer.SetNormals(layerElemNormal);
                    }

                    if (layerElemTangent != null)
                    {
                        layer.SetTangents(layerElemTangent);
                    }

                    if (layerElemBinormal != null)
                    {
                        layer.SetBinormals(layerElemBinormal);
                    }
                }

                if (layerElemVertexColor[i] != null)
                {
                    layer.SetVertexColors(layerElemVertexColor[i]);
                }

                if (layerElemUV[i] != null)
                {
                    layer.SetUVs(layerElemUV[i]);
                }
            }

            fmesh.BuildMeshEdgeArray();
            m_geomConverter.ComputeEdgeSmoothingFromNormals(fmesh);
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
}
