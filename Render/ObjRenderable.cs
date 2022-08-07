using Frosty.Core.Viewport;
using FrostySdk;
using FrostySdk.IO;
using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;

namespace LevelEditorPlugin.Render
{
    public class ObjRenderable
    {
        public BoundingBox Bounds { get; private set; }

        private D3D11.Buffer indexBuffer;
        private SharpDX.DXGI.Format indexBufferFormat = SharpDX.DXGI.Format.R32_UInt;

        private D3D11.Buffer vertexBuffer;
        private D3D11.VertexBufferBinding vertexBufferBinding;

        private int primitiveCount;
        private PrimitiveTopology primitiveType;
        private int vertexStride;

        public ObjRenderable(RenderCreateState state, string objName)
        {
            string relPath = $"Resources/Meshes/{objName}.bin";
            using (NativeReader reader = new NativeReader(new FileStream(relPath, FileMode.Open, FileAccess.Read)))
            {
                long vertexBufferSize = reader.ReadLong();
                vertexStride = reader.ReadInt();

                Vector3 minimum = new Vector3(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );
                Vector3 maximum = new Vector3(
                    reader.ReadFloat(),
                    reader.ReadFloat(),
                    reader.ReadFloat()
                    );

                byte[] vertexBufferData = reader.ReadBytes((int)vertexBufferSize);
                byte[] indexBufferData = reader.ReadToEnd();

                Bounds = new BoundingBox(minimum, maximum);

                using (DataStream chunkStream = new DataStream(indexBufferData.Length, false, true))
                {
                    chunkStream.Write(indexBufferData, 0, indexBufferData.Length);
                    chunkStream.Position = 0;

                    indexBuffer = new D3D11.Buffer(state.Device, chunkStream, indexBufferData.Length, D3D11.ResourceUsage.Default, D3D11.BindFlags.IndexBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 4);
                }

                primitiveCount = (indexBufferData.Length / 4) / 3;
                primitiveType = PrimitiveTopology.TriangleList;

                using (DataStream stream = new DataStream(vertexBufferData.Length, false, true))
                {
                    stream.Write(vertexBufferData, 0, vertexBufferData.Length);
                    stream.Position = 0;

                    vertexBuffer = new D3D11.Buffer(state.Device, stream, vertexBufferData.Length, D3D11.ResourceUsage.Default, D3D11.BindFlags.VertexBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
                    vertexBufferBinding = new D3D11.VertexBufferBinding(vertexBuffer, vertexStride, 0);
                }
            }
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public void Render(D3D11.DeviceContext context, MeshRenderPath path)
        {
            context.InputAssembler.SetIndexBuffer(indexBuffer, indexBufferFormat, 0);
            context.InputAssembler.PrimitiveTopology = primitiveType;
            context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);

            context.DrawIndexed(primitiveCount * 3, 0, 0);
        }
    }
}
