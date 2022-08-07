using Frosty.Core.Screens;
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
    public class ModelRenderProxy : RenderProxy
    {
        protected MeshRenderable renderData;
        protected int lodIndex;

        private ShaderPermutation permutation;
        private D3D11.Buffer pixelParameters;
        private List<D3D11.ShaderResourceView> pixelTextures = new List<D3D11.ShaderResourceView>();
        private MeshMaterial material;

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
            }
        });

        public ModelRenderProxy(RenderCreateState state, ISpatialEntity owner, MeshRenderable meshData)
            : base(owner)
        {
            renderData = meshData;

            RecalculateBoundingBox();

            material = new MeshMaterial();
            material.VectorParameters.Add(
                new FrostySdk.Ebx.VectorShaderParameter()
                {
                    ParameterName = "Color",
                    ParameterType = FrostySdk.Ebx.ShaderParameterType.ShaderParameterType_Vec4,
                    Value = new FrostySdk.Ebx.Vec4()
                    {
                        x = OwnerEntity.Owner.Layer.LayerColor.Red,
                        y = OwnerEntity.Owner.Layer.LayerColor.Green,
                        z = OwnerEntity.Owner.Layer.LayerColor.Blue,
                        w = 1.0f
                    }
                });

            permutation = state.ShaderLibrary.GetUserShader("LevelShader", GeometryDecl);
            permutation.IsTwoSided = true;
            permutation.LoadShaders(state.Device);
            permutation.AssignParameters(state, material, ref pixelParameters, ref pixelTextures);
        }

        public ModelRenderProxy(RenderCreateState state, MeshProxyEntity owner)
            : this(state, owner, owner.Mesh.MeshData)
        {
        }

        public ModelRenderProxy(RenderCreateState state, StaticModelEntity owner)
            : this(state, owner, owner.Mesh.MeshData)
        {
        }

#if MASS_EFFECT
        public ModelRenderProxy(RenderCreateState state, BangerEntity owner)
            : this(state, owner, owner.Mesh.MeshData)
        {
        }
#endif

        public ModelRenderProxy(RenderCreateState state, ClothEntity owner)
            : this(state, owner, owner.Mesh.MeshData)
        {
        }

        public ModelRenderProxy(RenderCreateState state, VegetationTreeEntity owner)
            : this(state, owner, owner.Mesh.MeshData)
        {
        }

        public override void Render(D3D11.DeviceContext context, MeshRenderPath renderPath)
        {
            if (renderPath == MeshRenderPath.Deferred)
            {
                permutation.SetState(context, renderPath);
                context.PixelShader.SetShaderResources(1, pixelTextures.ToArray());
                context.PixelShader.SetConstantBuffer(2, pixelParameters);

                renderData.GetLod(lodIndex).Render(context, renderPath);
            }
        }

        public override bool ShouldRender(float distToCamera, float screenSize)
        {
            return OwnerEntity.IsVisible && screenSize > renderData.CullScreenArea;
        }

        public override MeshRenderInstance GetInstance(float distToCamera)
        {
            lodIndex = 0;
            for (lodIndex = 0; lodIndex < 5; lodIndex++)
            {
                if (distToCamera < renderData.LodDistances[lodIndex])
                {
                    break;
                }
            }

            return new MeshRenderInstance() { RenderMesh = this, Transform = Transform };
        }

        public override void SetSelected(RenderCreateState state, bool newSelected)
        {
            material.SetVectorParameter("Color", newSelected
                ? new FrostySdk.Ebx.Vec4() { x = 1.0f, y = 1.0f, z = 0.0f, w = 1.0f }
                : new FrostySdk.Ebx.Vec4()
                {
                    x = OwnerEntity.Owner.Layer.LayerColor.Red,
                    y = OwnerEntity.Owner.Layer.LayerColor.Green,
                    z = OwnerEntity.Owner.Layer.LayerColor.Blue,
                    w = 1.0f
                });
            permutation.AssignParameters(state, material, ref pixelParameters, ref pixelTextures);
        }

        public override bool HitTest(Ray hitTestRay, out Vector3 hitLocation)
        {
            return renderData.Lods[0].HitTest(Transform, hitTestRay, out hitLocation);
        }

        public override void RecalculateBoundingBox()
        {
            OrientedBoundingBox meshBbox = new OrientedBoundingBox(renderData.Bounds);
            meshBbox.Transform(Transform);

            BoundingBox = meshBbox.GetBoundingBox();
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
