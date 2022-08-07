using Frosty.Core.Viewport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;
using SharpDX;
using FrostySdk;
using LevelEditorPlugin.Entities;

namespace LevelEditorPlugin.Render.Proxies
{
    public class SpriteRenderProxy : RenderProxy
    {
        private ObjRenderable renderData;

        private ShaderPermutation permutation;
        private D3D11.Buffer pixelParameters;
        private List<D3D11.ShaderResourceView> pixelTextures = new List<D3D11.ShaderResourceView>();
        private Matrix originalTransform;

        private List<ShaderParameter> materialParameters = new List<ShaderParameter>();
        private List<ShaderParameter> materialTextures = new List<ShaderParameter>();
        private Color4 tintColor;

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
            }
        });

        public SpriteRenderProxy(RenderCreateState state, Entity owner, ObjRenderable meshData, string spriteName, Color4? inTintColor = null)
            : base(owner as ISpatialEntity)
        {
            BoundingBox = BoundingBox.FromSphere(new BoundingSphere(Transform.TranslationVector, 0.5f));
            renderData = meshData;
            originalTransform = Transform;
            tintColor = inTintColor.HasValue ? inTintColor.Value : Color4.White;

            CreateMaterial(state, spriteName, tintColor);
        }

        public override void Update(RenderCreateState state)
        {
            if (OwnerEntity.RequiresTransformUpdate)
            {
                originalTransform = (OwnerEntity as ISpatialEntity).GetTransform();
                OwnerEntity.RequiresTransformUpdate = false;
            }
        }

        public override void Render(D3D11.DeviceContext context, MeshRenderPath renderPath)
        {
            Matrix cameraTransform = Screens.LevelEditorScreen.EditorCamera.WorldMatrix;
            float distToCamera = (Transform.TranslationVector - cameraTransform.TranslationVector).Length();

            cameraTransform.M41 = 0;
            cameraTransform.M42 = 0;
            cameraTransform.M43 = 0;

            float scaleAmount = (distToCamera < 10.0f) ? Math.Max(distToCamera / 10.0f, 0.125f) : 1.0f;
            Transform = Matrix.Scaling(scaleAmount) * cameraTransform * Matrix.Translation(originalTransform.TranslationVector);
            BoundingBox = BoundingBox.FromSphere(new BoundingSphere(Transform.TranslationVector, 0.5f * scaleAmount));

            if (renderPath == MeshRenderPath.Forward)
            {
                permutation.SetState(context, renderPath);
                context.PixelShader.SetShaderResources(1, pixelTextures.ToArray());
                context.PixelShader.SetConstantBuffer(2, pixelParameters);

                renderData.Render(context, renderPath);
            }
        }

        private void CreateMaterial(RenderCreateState state, string spriteName, Color4 tintColor)
        {
            materialParameters.Add(new ShaderParameter("Tint", ShaderParameterType.Float4, tintColor.Red, tintColor.Green, tintColor.Blue, 1.0f));
            materialTextures.Add(new ShaderParameter("Sprite", ShaderParameterType.Tex2d, $"Resources/Textures/Sprites/{spriteName}.dds"));

            permutation = state.ShaderLibrary.GetUserShader("SpriteShader", GeometryDecl);
            permutation.IsTwoSided = true;
            permutation.LoadShaders(state.Device);
            permutation.AssignParameters(state, materialParameters, materialTextures, ref pixelParameters, ref pixelTextures);
        }

        public override bool ShouldRender(float distToCamera, float screenSize)
        {
            return OwnerEntity.IsVisible && screenSize > 0.02f;
        }

        public override void SetSelected(RenderCreateState state, bool newSelected)
        {
            float[] colors = tintColor.ToArray();
            if (newSelected)
                colors = new float[] { 1, 1, 0, 1 };

            materialParameters[0] = new ShaderParameter("Tint", ShaderParameterType.Float4, colors);
            permutation.AssignParameters(state, materialParameters, materialTextures, ref pixelParameters, ref pixelTextures);
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
