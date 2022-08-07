using Frosty.Core.Viewport;
using FrostySdk;
using LevelEditorPlugin.Entities;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;

namespace LevelEditorPlugin.Render.Proxies
{
    public class TerrainRenderProxy : RenderProxy
    {
        protected TerrainChunkRenderable renderData;
        protected int previewLayer = -1;
        protected int previewHoleLayer = -1;

        private ShaderPermutation permutation;
        private D3D11.Buffer pixelParameters;
        private List<D3D11.ShaderResourceView> pixelTextures = new List<D3D11.ShaderResourceView>();

        private List<ShaderParameter> materialParameters = new List<ShaderParameter>();
        private List<ShaderParameter> materialTextures = new List<ShaderParameter>();

        private static GeometryDeclarationDesc GeometryDecl = GeometryDeclarationDesc.Create(new GeometryDeclarationDesc.Element[]
        {
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.Pos,
                Format = VertexElementFormat.Float3
            },
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.Normal,
                Format = VertexElementFormat.Float3
            },
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.TexCoord0,
                Format = VertexElementFormat.Float2
            },
            new GeometryDeclarationDesc.Element
            {
                Usage = VertexElementUsage.Color0,
                Format = VertexElementFormat.Float3
            }
        });

        public TerrainRenderProxy(RenderCreateState state, TerrainEntity owner, TerrainChunkRenderable terrainChunk)
            : base(owner)
        {
            renderData = terrainChunk;

            BoundingBox = terrainChunk.Bounds.GetBoundingBox();

            //materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 0.0f, 1.0f, 0.0f, 1.0f }));
            switch (renderData.Level)
            {
                case 1: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 1.0f, 0.0f, 0.0f, 1.0f })); break;
                case 2: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 0.0f, 1.0f, 0.0f, 1.0f })); break;
                case 3: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 0.0f, 0.0f, 1.0f, 1.0f })); break;
                case 4: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 1.0f, 1.0f, 0.0f, 1.0f })); break;
                case 5: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 0.0f, 1.0f, 1.0f, 1.0f })); break;
                case 6: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 1.0f, 0.0f, 1.0f, 1.0f })); break;
                case 7: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 1.0f, 1.0f, 1.0f, 1.0f })); break;
                case 8: materialParameters.Add(new ShaderParameter("Color", ShaderParameterType.Float4, new[] { 1.0f, 1.0f, 1.0f, 1.0f })); break;
            }
            materialParameters.Add(new ShaderParameter("HoleMaskIndex", ShaderParameterType.Float4, new[] { (float)previewLayer, (float)previewHoleLayer, 0, 0 }));

            for (int i = 0; i < terrainChunk.TexOffsets.Count; i++)
            {
                materialParameters.Add(new ShaderParameter($"LayerOffsets{i + 1}", ShaderParameterType.Float4, terrainChunk.TexOffsets[i].ToArray()));
            }

            permutation = state.ShaderLibrary.GetUserShader("TerrainShader", GeometryDecl);
            permutation.IsTwoSided = true;
            permutation.LoadShaders(state.Device);
            permutation.AssignParameters(state, materialParameters, materialTextures, ref pixelParameters, ref pixelTextures);
        }

        public void SetPreviewLayers(RenderCreateState state, int layerIndex, int holeIndex)
        {
            if (layerIndex != previewLayer || holeIndex != previewHoleLayer)
            {
                previewLayer = layerIndex;
                previewHoleLayer = holeIndex;

                int index = materialParameters.FindIndex(sp => sp.Name == "HoleMaskIndex");
                materialParameters.RemoveAt(index);

                materialParameters.Add(new ShaderParameter("HoleMaskIndex", ShaderParameterType.Float4, new[] { (float)previewLayer, (float)previewHoleLayer, 0, 0 }));
                permutation.AssignParameters(state, materialParameters, materialTextures, ref pixelParameters, ref pixelTextures);
            }
        }

        public override void Update(RenderCreateState state)
        {
            TerrainEntity terrainEntity = OwnerEntity as TerrainEntity;
            SetPreviewLayers(state, terrainEntity.PreviewLayerIndex, terrainEntity.PreviewHoleIndex);
        }

        public override void Render(D3D11.DeviceContext context, MeshRenderPath renderPath)
        {
            if (renderPath == MeshRenderPath.Deferred)
            {
                permutation.SetState(context, renderPath);
                context.PixelShader.SetShaderResources(1, pixelTextures.ToArray());
                context.PixelShader.SetConstantBuffer(2, pixelParameters);

                renderData.Render(context, renderPath);
            }
        }

        public override bool ShouldRender(float distToCamera, float screenSize)
        {
            return OwnerEntity.IsVisible && screenSize > 0.02f;
        }

        public override bool HitTest(Ray hitTestRay, out Vector3 hitLocation)
        {
            hitLocation = Vector3.Zero;
            return false;
        }

        public override void Dispose()
        {
            renderData.Dispose();
            permutation.Dispose();
            pixelParameters.Dispose();
            pixelTextures.Clear();
        }
    }
}
