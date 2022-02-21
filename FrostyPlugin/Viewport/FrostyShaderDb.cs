using System;
using FrostySdk.IO;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using System.Reflection;
using Frosty.Core.Attributes;

namespace Frosty.Core.Viewport
{
    public static class FrostyShaderDb
    {
        public static T GetShader<T>(D3D11.Device device, string entryPoint) where T : D3D11.DeviceChild
        {
            byte[] buffer = GetShaderFromBin(entryPoint, typeof(T) == typeof(D3D11.PixelShader), typeof(T) == typeof(D3D11.ComputeShader));

            if (typeof(T) == typeof(D3D11.VertexShader))
                return (T)Convert.ChangeType(new D3D11.VertexShader(device, buffer), typeof(T));
            
            if (typeof(T) == typeof(D3D11.PixelShader))
                return (T)Convert.ChangeType(new D3D11.PixelShader(device, buffer), typeof(T));
            
            if (typeof(T) == typeof(D3D11.ComputeShader))
                return (T)Convert.ChangeType(new D3D11.ComputeShader(device, buffer), typeof(T));

            return default;
        }

        public static T GetShaderWithSignature<T>(D3D11.Device device, string entryPoint, out ShaderSignature signature)
        {
            byte[] buffer = GetShaderFromBin(entryPoint, typeof(T) == typeof(D3D11.PixelShader), typeof(T) == typeof(D3D11.ComputeShader));
            signature = new ShaderSignature(buffer);

            if (typeof(T) == typeof(D3D11.VertexShader))
                return (T)Convert.ChangeType(new D3D11.VertexShader(device, buffer), typeof(T));
            
            if (typeof(T) == typeof(D3D11.PixelShader))
                return (T)Convert.ChangeType(new D3D11.PixelShader(device, buffer), typeof(T));
            
            if (typeof(T) == typeof(D3D11.ComputeShader))
                return (T)Convert.ChangeType(new D3D11.ComputeShader(device, buffer), typeof(T));

            return default;
        }

        public static byte[] GetShaderFromBin(string name, bool pixelShader, bool computeShader)
        {
            ShaderType shaderType = ShaderType.VertexShader;
            if (pixelShader) shaderType = ShaderType.PixelShader;
            if (computeShader) shaderType = ShaderType.ComputeShader;

            byte[] buf = App.PluginManager.GetShader(shaderType, name);
            if (buf != null)
                return buf;

            using (NativeReader reader = new NativeReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Frosty.Core.Shaders.bin")))
            {
                int numVEntries = reader.ReadInt();
                long startVOffset = reader.ReadLong();
                int numPEntries = reader.ReadInt();
                long startPOffset = reader.ReadLong();

                reader.Position = startVOffset;
                int numEntries = numVEntries;
                if (pixelShader)
                {
                    reader.Position = startPOffset;
                    numEntries = numPEntries;
                }

                for (int i = 0; i < numEntries; i++)
                {
                    string shader = reader.ReadNullTerminatedString();
                    int size = reader.ReadInt();
                    long offset = reader.ReadLong();

                    if (string.Equals(shader, name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        reader.Position = offset;
                        return reader.ReadBytes(size);
                    }
                }
            }
            return null;
        }
    }
}
