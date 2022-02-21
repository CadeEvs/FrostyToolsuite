using System;
using System.Windows.Interop;
using D3D11 = SharpDX.Direct3D11;
using System.Windows.Media;
using SharpDX;
using System.Runtime.InteropServices;

namespace Frosty.Core.Controls
{
    public class FrostyRenderImage : IDisposable
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHight, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool UnmapViewOfFile(IntPtr hFileMappingObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static extern IntPtr CopyMemory(IntPtr dest, IntPtr src, uint count);

        public ImageSource ImageSource => bmp;

        private static readonly object d3dLock = new object();
        private static int refCount;
        private bool disposed;

        private IntPtr fileMapping;
        private IntPtr mapView;
        private InteropBitmap bmp;
        private D3D11.Texture2D backBuffer;
        private D3D11.Texture2D staging;

        public FrostyRenderImage()
        {
            lock(d3dLock)
            {
                refCount++;
            }
        }

        ~FrostyRenderImage()
        {
            Dispose(false);
        }

        public void SetBackBuffer(D3D11.Texture2D texture)
        {
            if (disposed)
                return;

            if (fileMapping != IntPtr.Zero)
            {
                bmp = null;
                UnmapViewOfFile(mapView);
                CloseHandle(fileMapping);

                mapView = IntPtr.Zero;
                fileMapping = IntPtr.Zero;

            }

            if (texture == null)
                return;

            D3D11.Texture2DDescription texDesc = texture.Description;

            PixelFormat format = PixelFormats.Pbgra32;
            uint pixelCount = (uint)(texDesc.Width * texDesc.Height * format.BitsPerPixel / 8);

            fileMapping = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, pixelCount, null);
            mapView = MapViewOfFile(fileMapping, 0xF001F, 0, 0, pixelCount);

            bmp = (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(fileMapping, texDesc.Width, texDesc.Height, PixelFormats.Pbgra32, texDesc.Width * (format.BitsPerPixel / 8), 0);
            backBuffer = texture;

            texDesc.CpuAccessFlags = D3D11.CpuAccessFlags.Read;
            texDesc.Usage = D3D11.ResourceUsage.Staging;
            texDesc.BindFlags = D3D11.BindFlags.None;

            staging = new D3D11.Texture2D(backBuffer.Device, texDesc);
        }

        public void Invalidate()
        {
            if (disposed)
                return;

            D3D11.Device device = backBuffer.Device;
            device.ImmediateContext.CopyResource(backBuffer, staging);

            DataBox db = device.ImmediateContext.MapSubresource(staging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);
            {
                int rowWidth = backBuffer.Description.Width * (PixelFormats.Pbgra32.BitsPerPixel / 8);
                for (int i = 0; i < backBuffer.Description.Height; i++)
                {
                    CopyMemory(mapView + (i * rowWidth), db.DataPointer + (i * db.RowPitch), (uint)rowWidth);
                }
            }
            device.ImmediateContext.UnmapSubresource(staging, 0);
            bmp.Invalidate();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    SetBackBuffer(null);
                }

                lock (d3dLock)
                {
                    refCount--;
                    if (refCount == 0)
                    {
                        // @todo
                    }
                }

                disposed = true;
            }
        }
    }
}
