using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace Frosty.Core.Viewport
{
    public class ConstantBuffer<T> : IDisposable where T : struct
    {
        public SharpDX.Direct3D11.Buffer Buffer { get; private set; } = null;

        public ConstantBuffer(Device device, T value)
        {
            BufferDescription description = new BufferDescription(Utilities.SizeOf<T>(), BindFlags.ConstantBuffer, ResourceUsage.Dynamic) {CpuAccessFlags = CpuAccessFlags.Write};
            Buffer = new SharpDX.Direct3D11.Buffer(device, description);
            UpdateData(device.ImmediateContext, value);
        }

        public void UpdateData(DeviceContext c, T value)
        {
            c.MapSubresource(Buffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
            stream.Write(value);
            c.UnmapSubresource(Buffer, 0);
        }

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }
}
