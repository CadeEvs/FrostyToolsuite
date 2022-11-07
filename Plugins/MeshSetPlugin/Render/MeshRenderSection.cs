using Frosty.Core.Viewport;
using MeshSetPlugin.Resources;
using SharpDX;
using SharpDX.Direct3D;
using System.Collections.Generic;
using D3D11 = SharpDX.Direct3D11;

namespace MeshSetPlugin.Render
{
    public class MeshRenderSection
    {
        public ShaderPermutation Permutation;
        public MeshRenderSkeleton Skeleton;
        public D3D11.Buffer VertexParameters;
        public D3D11.Buffer PixelParameters;
        public D3D11.Buffer[] VertexBuffers;
        public D3D11.VertexBufferBinding[] VertexBufferBindings;
        public List<D3D11.ShaderResourceView> VertexTextures = new List<D3D11.ShaderResourceView>();
        public List<D3D11.SamplerState> VertexSamplers = new List<D3D11.SamplerState>();
        public List<D3D11.ShaderResourceView> PixelTextures = new List<D3D11.ShaderResourceView>();
        public MeshSetSection MeshSection;
        public int StartIndex;
        public int VertexOffset;
        public int PrimitiveCount;
        public PrimitiveTopology PrimitiveType;
        public int VertexStride;

        public bool IsFallback;
        public bool IsSelected;
        public bool IsVisible = true;

        // @temp
        public List<uint> BoneIndices = new List<uint>();

        public string DebugName => MeshSection.Name;

        public void SetState(D3D11.DeviceContext context, MeshRenderPath renderPath)
        {
            if (Permutation.IsSkinned)
            {
                // obtain bone matrices from skeleton
                List<Matrix> boneMatrices = new List<Matrix>();
                for (int i = 0; i < BoneIndices.Count; i++)
                {
                    int boneIndex = (int)BoneIndices[i];
                    if ((BoneIndices[i] & 0x8000) != 0)
                    {
                        boneIndex = (boneIndex & 0x7FFF) + Skeleton.BoneCount;
                    }

                    while (i >= boneMatrices.Count)
                    {
                        boneMatrices.Add(Matrix.Identity);
                    }

                    if (boneIndex == -1)
                    {
                        continue;
                    }

                    boneMatrices[i] = Skeleton.GetBoneMatrix(boneIndex);
                }

                // update the bone buffer
                Permutation.boneBuffer.Update(context, Skeleton.BoneCount, boneMatrices.ToArray());
            }

            Permutation.SetState(context, renderPath);

            context.InputAssembler.PrimitiveTopology = PrimitiveType;
            context.InputAssembler.SetVertexBuffers(0, VertexBufferBindings);

            if (renderPath != MeshRenderPath.Shadows)
            {
                context.PixelShader.SetShaderResources(1, PixelTextures.ToArray());
                context.PixelShader.SetConstantBuffer(2, PixelParameters);
            }
        }

        public void Draw(D3D11.DeviceContext context)
        {
            context.DrawIndexed(PrimitiveCount * 3, StartIndex, 0);
        }
    }
}
