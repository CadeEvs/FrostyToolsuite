using Frosty.Core;
using Frosty.Core.Viewport;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using MeshSetPlugin.Resources;
using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Managers.Entries;
using D3D11 = SharpDX.Direct3D11;

namespace MeshSetPlugin.Render
{
    public class MeshRenderLod : MeshRenderBase, IDisposable
    {
        public IEnumerable<MeshRenderSection> Sections => sections;
        public override string DebugName => meshLod.ShortName;

        private D3D11.Buffer indexBuffer;
        private SharpDX.DXGI.Format indexBufferFormat;
        private List<MeshRenderSection> sections = new List<MeshRenderSection>();
        private MeshSetLod meshLod;

        public MeshRenderLod(RenderCreateState state, MeshSetLod lod, MeshMaterialCollection materials, MeshRenderSkeleton skeleton)
        {
            meshLod = lod;

            byte[] chunkData = GetChunkData();
            using (DataStream chunkStream = new DataStream((int)lod.IndexBufferSize, false, true))
            {
                chunkStream.Write(chunkData, (int)lod.VertexBufferSize, (int)lod.IndexBufferSize);
                chunkStream.Position = 0;

                indexBuffer = new D3D11.Buffer(state.Device, chunkStream, (int)lod.IndexBufferSize, D3D11.ResourceUsage.Default, D3D11.BindFlags.IndexBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, lod.IndexUnitSize / 8);
            }

            indexBufferFormat = (lod.IndexUnitSize == 16)
                ? SharpDX.DXGI.Format.R16_UInt
                : SharpDX.DXGI.Format.R32_UInt;

            foreach (MeshSetSection section in lod.Sections)
            {
                // @todo: Fix DAI
                if (lod.IsSectionRenderable(section) || lod.IsSectionInCategory(section, MeshSubsetCategory.MeshSubsetCategory_ZOnly))
                {
                    if (section.VertexCount == 0)
                    {
                        continue;
                    }

                    MeshMaterial material = materials[section.MaterialId];
                    EbxAssetEntry shaderAsset = App.AssetManager.GetEbxEntry(material.Shader.External.FileGuid);
                    if (shaderAsset == null)
                    {
                        continue;
                    }

                    ShaderPermutation permutation = state.ShaderLibrary.GetUserShader(shaderAsset.Name, section.GeometryDeclDesc[0]);

                    MeshRenderSection renderSection = new MeshRenderSection();
                    if (permutation != null)
                    {
                        if (!permutation.LoadShaders(state.Device))
                        {
                            permutation = null;
                        }
                    }
                    renderSection.StartIndex = (int)section.StartIndex;
                    renderSection.VertexOffset = (int)section.VertexOffset;
                    renderSection.PrimitiveCount = (int)section.PrimitiveCount;
                    renderSection.VertexStride = (int)section.VertexStride;
                    renderSection.MeshSection = section;

                    if (lod.Type == MeshType.MeshType_Composite)
                    {
                        skeleton = new MeshRenderSkeleton();
                        foreach (LinearTransform lt in lod.PartTransforms)
                        {
                            skeleton.AddBone(new MeshRenderSkeleton.Bone()
                            {
                                NameHash = Fnv1.HashString("Part"),
                                ModelPose = Matrix.Identity,
                                LocalPose = SharpDXUtils.FromLinearTransform(lt),
                                ParentBoneId = -1
                            });
                        }
                    }

                    renderSection.Skeleton = skeleton;

                    switch (section.PrimitiveType)
                    {
                        case PrimitiveType.PrimitiveType_LineList: renderSection.PrimitiveType = PrimitiveTopology.LineList; break;
                        case PrimitiveType.PrimitiveType_LineStrip: renderSection.PrimitiveType = PrimitiveTopology.LineStrip; break;
                        case PrimitiveType.PrimitiveType_PointList: renderSection.PrimitiveType = PrimitiveTopology.PointList; break;
                        case PrimitiveType.PrimitiveType_TriangleList: renderSection.PrimitiveType = PrimitiveTopology.TriangleList; break;
                        case PrimitiveType.PrimitiveType_TriangleStrip: renderSection.PrimitiveType = PrimitiveTopology.TriangleStrip; break;
                    }
                    sections.Add(renderSection);

                    RebuildSection(state, renderSection, permutation, chunkData);
                    if (renderSection.IsFallback)
                    {
                        renderSection.Permutation = state.ShaderLibrary.GetFallbackShader();
                        renderSection.Permutation.AssignFallbackParameters(state, renderSection, material);
                    }
                    else
                    {
                        renderSection.Permutation = permutation;
                        renderSection.Permutation.AssignParameters(state, material, ref renderSection.PixelParameters, ref renderSection.PixelTextures);
                    }
                }
            }
        }

        public void UpdateSectionMaterial(RenderCreateState state, MeshRenderSection section, MeshMaterial material)
        {
            EbxAssetEntry shaderAsset = App.AssetManager.GetEbxEntry(material.Shader.External.FileGuid);
            ShaderPermutation permutation = (shaderAsset == null) ? null : state.ShaderLibrary.GetUserShader(shaderAsset.Name, section.MeshSection.GeometryDeclDesc[0]);
            ShaderPermutation oldPermutation = section.Permutation;

            if (permutation != null)
            {
                if (!permutation.LoadShaders(state.Device))
                {
                    permutation = null;
                }
            }

            bool bRequiresRebuild = (section.IsFallback && permutation != null) || (!section.IsFallback && permutation == null);
            if (bRequiresRebuild)
            {
                RebuildSection(state, section, permutation, GetChunkData());
            }

            if (section.IsFallback)
            {
                section.Permutation = state.ShaderLibrary.GetFallbackShader();
                section.Permutation.AssignFallbackParameters(state, section, material);
            }
            else
            {
                section.Permutation = permutation;
                section.Permutation.AssignParameters(state, material, ref section.PixelParameters, ref section.PixelTextures);
            }

            state.ShaderLibrary.UnloadShader(oldPermutation);
        }

        public void SetMaterial(RenderCreateState state, int materialIdx, MeshMaterial material)
        {
            foreach (MeshRenderSection section in sections)
            {
                if (section.MeshSection.MaterialId == materialIdx)
                {
                    UpdateSectionMaterial(state, section, material);
                }
            }
        }

        public void SetMaterials(RenderCreateState state, MeshMaterialCollection materials)
        {
            foreach (MeshRenderSection section in sections)
            {
                if (section.MeshSection.MaterialId < materials.Count)
                {
                    UpdateSectionMaterial(state, section, materials[section.MeshSection.MaterialId]);
                }
            }
        }

        public MeshRenderSection GetSection(int idx)
        {
            if (idx >= sections.Count)
            {
                return null;
            }

            return sections[idx];
        }

        public override void Render(D3D11.DeviceContext context, MeshRenderPath renderPath)
        {
            if (sections.Count == 0)
            {
                return;
            }

            context.InputAssembler.SetIndexBuffer(indexBuffer, indexBufferFormat, 0);
            foreach (MeshRenderSection section in sections)
            {
                // during shadow pass, draw Z-Only meshes
                if (renderPath == MeshRenderPath.Shadows && !meshLod.IsSectionInCategory(section.MeshSection, MeshSubsetCategory.MeshSubsetCategory_ZOnly))
                {
                    continue;
                }

                // during deferred pass, draw renderable meshes
                if (renderPath != MeshRenderPath.Shadows && !meshLod.IsSectionRenderable(section.MeshSection))
                {
                    continue;
                }

                // only render selected sections during selection pass
                if (renderPath == MeshRenderPath.Selection && !section.IsSelected)
                {
                    continue;
                }

                // dont render invisible sections (exception during selection pass if selected)
                if (!section.IsVisible)
                {
                    if (renderPath != MeshRenderPath.Selection || !section.IsSelected)
                    {
                        continue;
                    }
                }

                D3DUtils.BeginPerfEvent(context, section.DebugName);
                {
                    D3D11.RasterizerState oldState = context.Rasterizer.State;

                    section.SetState(context, renderPath);
                    section.Draw(context);

                    context.Rasterizer.State = oldState;
                }
                D3DUtils.EndPerfEvent(context);
            }
        }

        private void RebuildSection(RenderCreateState state, MeshRenderSection section, ShaderPermutation permutation, byte[] chunkData)
        {
            if (section.VertexBuffers != null)
            {
                foreach (D3D11.Buffer vb in section.VertexBuffers)
                {
                    vb.Dispose();
                }
            }

            using (NativeReader reader = new NativeReader(new MemoryStream(chunkData)))
            {
                reader.Position = section.VertexOffset;
                if (permutation == null)
                {
                    section.VertexStride = Utilities.SizeOf<FallbackVertex>();
                    section.IsFallback = true;

                    int size = (int)(section.VertexStride * section.MeshSection.VertexCount);
                    AssignFallbackVertices(state, section, section.MeshSection, reader, meshLod.VertexBufferSize);
                }
                else
                {
                    section.IsFallback = false;
                    section.VertexBuffers = new D3D11.Buffer[section.MeshSection.GeometryDeclDesc[0].StreamCount];
                    section.VertexBufferBindings = new D3D11.VertexBufferBinding[section.MeshSection.GeometryDeclDesc[0].StreamCount];

                    for (int i = 0; i < section.VertexBuffers.Length; i++)
                    {
                        GeometryDeclarationDesc.Stream stream = section.MeshSection.GeometryDeclDesc[0].Streams[i];

                        int size = (int)(stream.VertexStride * section.MeshSection.VertexCount);
                        using (DataStream chunkStream = new DataStream(size, false, true))
                        {
                            chunkStream.Write(reader.ReadBytes(size), 0, size);
                            chunkStream.Position = 0;

                            section.VertexBuffers[i] = new D3D11.Buffer(state.Device, chunkStream, size, D3D11.ResourceUsage.Default, D3D11.BindFlags.VertexBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
                            section.VertexBufferBindings[i] = new D3D11.VertexBufferBinding(section.VertexBuffers[i], stream.VertexStride, 0);
                        }
                    }
                }
            }

            if (meshLod.Type == MeshType.MeshType_Rigid && section.IsFallback)
            {
                // if rigid, then add a dummy entry for the fallback shader
                section.BoneIndices.Add(0);
            }
            else if (meshLod.Type == MeshType.MeshType_Composite)
            {
                if (section.MeshSection.BoneList.Count > 0)
                {
                    uint i = 0;
                    foreach (uint boneIdx in section.MeshSection.BoneList)
                    {
                        while (section.BoneIndices.Count <= boneIdx)
                        {
                            section.BoneIndices.Add(i++);
                        }
                    }
                }
                else if (meshLod.PartCount > 0)
                {
                    for (uint i = 0; i < meshLod.PartCount; i++)
                    {
                        section.BoneIndices.Add(i);
                    }
                }
                else
                {
                    section.BoneIndices.Add(0);
                }
            }
            else
            {
                // add section bones to list
                foreach (ushort boneId in section.MeshSection.BoneList)
                {
                    section.BoneIndices.Add(boneId);
                }

                // special handling for games that only use the mesh lod bone list
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.Fifa19,
                    ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville))
                {
                    section.BoneIndices.Clear();
                    section.BoneIndices.AddRange(meshLod.BoneIndexArray);
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden19, ProfileVersion.Fifa17,
                    ProfileVersion.Fifa18, ProfileVersion.Madden20))
                {
                    section.BoneIndices.Clear();
                    uint i = 0;

                    foreach (uint boneIdx in meshLod.BoneIndexArray)
                    {
                        while (section.BoneIndices.Count <= boneIdx)
                        {
                            section.BoneIndices.Add(i++);
                        }
                    }
                }
            }
        }

        private byte[] GetChunkData()
        {
            ChunkAssetEntry entry = App.AssetManager.GetChunkEntry(meshLod.ChunkId);
            byte[] chunkData = meshLod.InlineData;
            if (entry != null)
            {
                chunkData = new NativeReader(App.AssetManager.GetChunk(entry)).ReadToEnd();
            }

            return chunkData;
        }

        /// <summary>
        /// Re-organizes the vertices into the fallback shader layout and writes them to the vertex buffer
        /// </summary>
        private void AssignFallbackVertices(RenderCreateState state, MeshRenderSection section, MeshSetSection meshSection, NativeReader reader, long indexBufferOffset)
        {
            FallbackVertex[] vertices = new FallbackVertex[meshSection.VertexCount];
            bool bitangentSign = false;
            //bool bRecalculateNormals = ProfilesLibrary.IsLoaded(ProfileVersion.Madden20);

            // re-organize vertices into the fallback shader layout
            for (int i = 0; i < meshSection.GeometryDeclDesc[0].Streams.Length; i++)
            {
                GeometryDeclarationDesc.Stream stream = meshSection.GeometryDeclDesc[0].Streams[i];
                if (stream.VertexStride == 0)
                {
                    continue;
                }

                for (int v = 0; v < meshSection.VertexCount; v++)
                {
                    FallbackVertex vertex = vertices[v];
                    int currentStride = 0;

                    foreach (GeometryDeclarationDesc.Element element in meshSection.GeometryDeclDesc[0].Elements)
                    {
                        if (element.Usage == VertexElementUsage.Unknown)
                        {
                            continue;
                        }

                        //reader.Position = section.VertexOffset + v * section.VertexStride + element.Offset;

                        if (element.StreamIndex == i && currentStride < stream.VertexStride)
                        {
                            if (element.Usage == VertexElementUsage.Pos)
                            {
                                if (element.Format == VertexElementFormat.Float3 || element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Position = new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
                                    if (element.Format == VertexElementFormat.Float4)
                                    {
                                        reader.ReadFloat();
                                    }
                                }
                                else if (element.Format == VertexElementFormat.Half3 || element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Position = new Vector3(HalfUtils.Unpack(reader.ReadUShort()),
                                                                  HalfUtils.Unpack(reader.ReadUShort()),
                                                                  HalfUtils.Unpack(reader.ReadUShort()));
                                    if (element.Format == VertexElementFormat.Half4)
                                    {
                                        HalfUtils.Unpack(reader.ReadUShort());
                                    }
                                }
                            }
                            else if (element.Usage == VertexElementUsage.Normal)
                            {
                                if (element.Format == VertexElementFormat.Float3 || element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Normal = new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), 1.0f);
                                    if (element.Format == VertexElementFormat.Float4)
                                    {
                                        vertex.Normal.W = reader.ReadFloat();
                                    }
                                }
                                else if (element.Format == VertexElementFormat.Half3 || element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Normal = new Vector4(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), 1.0f);
                                    if (element.Format == VertexElementFormat.Half4)
                                    {
                                        vertex.Normal.W = HalfUtils.Unpack(reader.ReadUShort());
                                    }
                                }
                            }
                            else if (element.Usage == VertexElementUsage.Binormal)
                            {
                                if (element.Format == VertexElementFormat.Float3 || element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Bitangent = new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), 1.0f);
                                    if (element.Format == VertexElementFormat.Float4)
                                    {
                                        vertex.Bitangent.W = reader.ReadFloat();
                                    }
                                }
                                else if (element.Format == VertexElementFormat.Half3 || element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Bitangent = new Vector4(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), 1.0f);
                                    if (element.Format == VertexElementFormat.Half4)
                                    {
                                        vertex.Bitangent.W = HalfUtils.Unpack(reader.ReadUShort());
                                    }
                                }
                            }
                            else if (element.Usage == VertexElementUsage.Tangent)
                            {
                                if (element.Format == VertexElementFormat.Float3 || element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Tangent = new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), 1.0f);
                                    if (element.Format == VertexElementFormat.Float4)
                                    {
                                        vertex.Tangent.W = reader.ReadFloat();
                                    }
                                }
                                else if (element.Format == VertexElementFormat.Half3 || element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Tangent = new Vector4(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), 1.0f);
                                    if (element.Format == VertexElementFormat.Half4)
                                    {
                                        vertex.Tangent.W = HalfUtils.Unpack(reader.ReadUShort());
                                    }
                                }

                                //if (vertex.BitangentSign == 0.0f)
                                //    vertex.BitangentSign = Vector3.Dot(Vector3.Cross(vertex.Normal, vertex.Tangent), vertex.Bitangent) < 0.0f ? -1.0f : 1.0f;
                            }
                            else if (element.Usage == VertexElementUsage.BinormalSign)
                            {
                                if (element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Tangent = new Vector4(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), 1.0f);
                                    vertex.Bitangent.W = HalfUtils.Unpack(reader.ReadUShort());
                                }
                                else if (element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Tangent = new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), 1.0f);
                                    vertex.Bitangent.W = reader.ReadFloat();
                                }
                                else
                                {
                                    vertex.Bitangent.W = (element.Format == VertexElementFormat.Float)
                                        ? reader.ReadFloat()
                                        : HalfUtils.Unpack(reader.ReadUShort());
                                }
                                bitangentSign = true;
                            }
                            else if (element.Usage == VertexElementUsage.TangentSpace)
                            {
                                if (element.Format == VertexElementFormat.UByte4N)
                                {
                                    vertex.Bitangent = new Vector4(reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f);
                                    vertex.TangentSpace = 1;
                                }
                                else if (element.Format == VertexElementFormat.UShort4N)
                                {
                                    vertex.Bitangent = new Vector4(reader.ReadUShort() / 65535.0f, reader.ReadUShort() / 65535.0f, reader.ReadUShort() / 65535.0f, reader.ReadUShort() / 65535.0f);
                                    vertex.TangentSpace = 1;
                                }
                                else
                                {
                                    vertex.TangentSpace = reader.ReadUInt();
                                }
                            }
                            else if (element.Usage == VertexElementUsage.TexCoord0)
                            {
                                if (element.Format == VertexElementFormat.Float2)
                                {
                                    vertex.TexCoord0 = new Vector2(reader.ReadFloat(), reader.ReadFloat());
                                }
                                else if (element.Format == VertexElementFormat.Half2)
                                {
                                    vertex.TexCoord0 = new Vector2(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()));
                                }
                            }
                            else if (element.Usage == VertexElementUsage.TexCoord1)
                            {
                                if (element.Format == VertexElementFormat.Float2)
                                {
                                    vertex.TexCoord1 = new Vector2(reader.ReadFloat(), reader.ReadFloat());
                                }
                                else if (element.Format == VertexElementFormat.Half2)
                                {
                                    vertex.TexCoord1 = new Vector2(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()));
                                }
                            }
                            else if (element.Usage == VertexElementUsage.TexCoord2)
                            {
                                if (element.Format == VertexElementFormat.Float2)
                                {
                                    vertex.TexCoord2 = new Vector2(reader.ReadFloat(), reader.ReadFloat());
                                }
                                else if (element.Format == VertexElementFormat.Half2)
                                {
                                    vertex.TexCoord2 = new Vector2(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()));
                                }
                            }
                            else if (element.Usage == VertexElementUsage.Color0)
                            {
                                if (element.Format == VertexElementFormat.UByte4N)
                                {
                                    vertex.Color0 = new Vector4(reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f);
                                }
                                else if (element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Color0 = new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
                                }
                                else if (element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Color0 = new Vector4(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()));
                                }
                            }
                            else if (element.Usage == VertexElementUsage.Color1)
                            {
                                if (element.Format == VertexElementFormat.UByte4N)
                                {
                                    vertex.Color1 = new Vector4(reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f, reader.ReadByte() / 255.0f);
                                }
                                else if (element.Format == VertexElementFormat.Float4)
                                {
                                    vertex.Color1 = new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
                                }
                                else if (element.Format == VertexElementFormat.Half4)
                                {
                                    vertex.Color1 = new Vector4(HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()), HalfUtils.Unpack(reader.ReadUShort()));
                                }
                            }
                            else if (element.Usage == VertexElementUsage.BoneIndices)
                            {
                                if (element.Format == VertexElementFormat.UShort4)
                                {
                                    vertex.BoneIndices0 = reader.ReadUShort();
                                    vertex.BoneIndices1 = reader.ReadUShort();
                                    vertex.BoneIndices2 = reader.ReadUShort();
                                    vertex.BoneIndices3 = reader.ReadUShort();
                                }
                                else
                                {
                                    vertex.BoneIndices0 = reader.ReadByte();
                                    vertex.BoneIndices1 = reader.ReadByte();
                                    vertex.BoneIndices2 = reader.ReadByte();
                                    vertex.BoneIndices3 = reader.ReadByte();
                                }
                            }
                            else if (element.Usage == VertexElementUsage.BoneIndices2)
                            {
                                if (element.Format == VertexElementFormat.UShort4)
                                {
                                    vertex.BoneIndices4 = reader.ReadUShort();
                                    vertex.BoneIndices5 = reader.ReadUShort();
                                    vertex.BoneIndices6 = reader.ReadUShort();
                                    vertex.BoneIndices7 = reader.ReadUShort();
                                }
                                else
                                {
                                    vertex.BoneIndices4 = reader.ReadByte();
                                    vertex.BoneIndices5 = reader.ReadByte();
                                    vertex.BoneIndices6 = reader.ReadByte();
                                    vertex.BoneIndices7 = reader.ReadByte();
                                }
                            }
                            else if (element.Usage == VertexElementUsage.BoneWeights)
                            {
                                vertex.BoneWeights0 = reader.ReadByte() / 255.0f;
                                vertex.BoneWeights1 = reader.ReadByte() / 255.0f;
                                vertex.BoneWeights2 = reader.ReadByte() / 255.0f;
                                vertex.BoneWeights3 = reader.ReadByte() / 255.0f;
                            }
                            else if (element.Usage == VertexElementUsage.BoneWeights2)
                            {
                                vertex.BoneWeights4 = reader.ReadByte() / 255.0f;
                                vertex.BoneWeights5 = reader.ReadByte() / 255.0f;
                                vertex.BoneWeights6 = reader.ReadByte() / 255.0f;
                                vertex.BoneWeights7 = reader.ReadByte() / 255.0f;
                            }
                            else
                            {
                                reader.Position += element.Size;
                            }

                            currentStride += element.Size;
                        }
                    }

                    // rivals pads the vertex stride
                    if (currentStride != stream.VertexStride)
                    {
                        reader.Position += stream.VertexStride - currentStride;
                    }

                    if (meshLod.Type == MeshType.MeshType_Composite)
                    {
                        vertex.BoneWeights3 = 1.0f;
                    }
                    else if (meshLod.Type == MeshType.MeshType_Rigid)
                    {
                        vertex.BoneIndices0 = 0;
                        vertex.BoneIndices1 = 0;
                        vertex.BoneIndices2 = 0;
                        vertex.BoneIndices3 = 0;
                        vertex.BoneIndices4 = 0;
                        vertex.BoneIndices5 = 0;
                        vertex.BoneIndices6 = 0;
                        vertex.BoneIndices7 = 0;

                        vertex.BoneWeights0 = 0.0f;
                        vertex.BoneWeights1 = 0.0f;
                        vertex.BoneWeights2 = 0.0f;
                        vertex.BoneWeights3 = 1.0f;
                        vertex.BoneWeights4 = 0.0f;
                        vertex.BoneWeights5 = 0.0f;
                        vertex.BoneWeights6 = 0.0f;
                        vertex.BoneWeights7 = 0.0f;
                    }

                    vertices[v] = vertex;
                }
            }

            //if (bRecalculateNormals)
            //{
            //    reader.Position = indexBufferOffset + (section.StartIndex * ((indexBufferFormat == SharpDX.DXGI.Format.R16_UInt) ? 2 : 4));
            //    uint[] indices = new uint[meshSection.PrimitiveCount * 3];
            //    for (int i = 0; i < indices.Length; i++)
            //        indices[i] = (this.indexBufferFormat == SharpDX.DXGI.Format.R16_UInt) ? reader.ReadUShort() : reader.ReadUInt();

            //    Vector3[] norm = new Vector3[vertices.Length];
            //    Vector3[] tan1 = new Vector3[vertices.Length];
            //    Vector3[] tan2 = new Vector3[vertices.Length];

            //    for (int i = 0; i < section.PrimitiveCount * 3; i += 3)
            //    {
            //        FallbackVertex v1 = vertices[(int)indices[i + 0]];
            //        FallbackVertex v2 = vertices[(int)indices[i + 1]];
            //        FallbackVertex v3 = vertices[(int)indices[i + 2]];

            //        Vector3 pos1 = v1.Position;
            //        Vector3 pos2 = v2.Position;
            //        Vector3 pos3 = v3.Position;

            //        Vector3 edge1 = (pos2 - pos1);
            //        Vector3 edge2 = (pos3 - pos1);

            //        Vector3 tangentX = edge1;
            //        Vector3 tangentZ = Vector3.Cross(edge1, edge2);

            //        tangentX.Normalize();
            //        tangentZ.Normalize();

            //        Vector3 tangentY = Vector3.Cross(tangentX, tangentZ);
            //        tangentY.Normalize();

            //        v1.Tangent = new Vector4(tangentX, 0.0f);
            //        v2.Tangent = new Vector4(tangentX, 0.0f);
            //        v3.Tangent = new Vector4(tangentX, 0.0f);

            //        v1.Bitangent = new Vector4(tangentY, 0.0f);
            //        v2.Bitangent = new Vector4(tangentY, 0.0f);
            //        v3.Bitangent = new Vector4(tangentY, 0.0f);

            //        v1.Normal = new Vector4(tangentZ, 0.0f);
            //        v2.Normal = new Vector4(tangentZ, 0.0f);
            //        v3.Normal = new Vector4(tangentZ, 0.0f);

            //        float bitangent = Vector3.Dot(Vector3.Cross(tangentZ, tangentX), tangentY) < 0.0f ? -1.0f : 1.0f;
            //        v1.Bitangent *= bitangent;
            //        v2.Bitangent *= bitangent;
            //        v3.Bitangent *= bitangent;

            //        vertices[(int)indices[i + 0]] = v1;
            //        vertices[(int)indices[i + 1]] = v2;
            //        vertices[(int)indices[i + 2]] = v3;

            //        //bitangentSign = true;
            //    }
            //}

            if (bitangentSign)
            {
                for (int v = 0; v < meshSection.VertexCount; v++)
                {
                    FallbackVertex vertex = vertices[v];
                    Vector3 t = new Vector3(vertex.Tangent.X, vertex.Tangent.Y, vertex.Tangent.Z);
                    Vector3 n = new Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    Vector3 b = Vector3.Cross(n, t) * vertex.Bitangent.W;

                    vertex.Bitangent = new Vector4(b.X, b.Y, b.Z, 1.0f);
                    vertex.Bitangent.Normalize();
                    vertices[v] = vertex;
                }
            }

            section.VertexBuffers = new D3D11.Buffer[1];
            section.VertexBufferBindings = new D3D11.VertexBufferBinding[1];

            // write them to the vertex buffer
            int size = (int)(Utilities.SizeOf<FallbackVertex>() * meshSection.VertexCount);
            using (DataStream stream = new DataStream(size, false, true))
            {
                foreach (FallbackVertex vert in vertices)
                {
                    stream.Write<FallbackVertex>(vert);
                }

                stream.Position = 0;

                section.VertexBuffers[0] = new D3D11.Buffer(state.Device, stream, size, D3D11.ResourceUsage.Default, D3D11.BindFlags.VertexBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
                section.VertexBufferBindings[0] = new D3D11.VertexBufferBinding(section.VertexBuffers[0], section.VertexStride, 0);
            }
        }

        public void Dispose()
        {
            foreach (MeshRenderSection section in sections)
            {
                foreach (D3D11.Buffer vb in section.VertexBuffers)
                {
                    vb.Dispose();
                }

                section.PixelParameters?.Dispose();
                section.PixelTextures.Clear();
            }

            indexBuffer.Dispose();
        }
    }
}
