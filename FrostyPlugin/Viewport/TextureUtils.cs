using System;
using System.Collections.Generic;
using FrostySdk.IO;
using SharpDX;
using D3D11 = SharpDX.Direct3D11;
using System.Runtime.InteropServices;
using System.IO;
using FrostySdk.Resources;
using SharpDX.Direct3D;
using System.Windows;

namespace Frosty.Core.Viewport
{
    public class FrostyDeviceManager
    {
        private class Hashing
        {
            public static uint Hash(D3D11.SamplerStateDescription desc)
            {
                uint hash = 2166136261;
                hash = (hash * 16777619) ^ (uint)((int)desc.AddressU).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.AddressV).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.AddressW).GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.BorderColor.R.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.BorderColor.G.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.BorderColor.B.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.BorderColor.A.GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.ComparisonFunction).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.Filter).GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.MaximumAnisotropy.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.MaximumLod.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.MinimumLod.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.MipLodBias.GetHashCode();
                return hash;
            }

            public static uint Hash(D3D11.RasterizerStateDescription desc)
            {
                uint hash = 2166136261;
                hash = (hash * 16777619) ^ (uint)((int)desc.CullMode).GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.DepthBias.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.DepthBiasClamp.GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.FillMode).GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsAntialiasedLineEnabled.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsDepthClipEnabled.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsFrontCounterClockwise.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsMultisampleEnabled.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsScissorEnabled.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.SlopeScaledDepthBias.GetHashCode();
                return hash;
            }

            public static uint Hash(D3D11.DepthStencilStateDescription desc)
            {
                uint hash = 2166136261;
                hash = (hash * 16777619) ^ (uint)((int)desc.BackFace.Comparison).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.BackFace.DepthFailOperation).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.BackFace.FailOperation).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.BackFace.PassOperation).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.FrontFace.Comparison).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.FrontFace.DepthFailOperation).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.FrontFace.FailOperation).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.FrontFace.PassOperation).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.DepthComparison).GetHashCode();
                hash = (hash * 16777619) ^ (uint)((int)desc.DepthWriteMask).GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsDepthEnabled.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IsStencilEnabled.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.StencilReadMask.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.StencilWriteMask.GetHashCode();
                return hash;
            }
            
            public static uint Hash(D3D11.BlendStateDescription desc)
            {
                uint hash = 2166136261;
                hash = (hash * 16777619) ^ (uint)desc.AlphaToCoverageEnable.GetHashCode();
                hash = (hash * 16777619) ^ (uint)desc.IndependentBlendEnable.GetHashCode();
                foreach (D3D11.RenderTargetBlendDescription rtDesc in desc.RenderTarget)
                {
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.AlphaBlendOperation).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.BlendOperation).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.DestinationAlphaBlend).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.DestinationBlend).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.RenderTargetWriteMask).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.SourceAlphaBlend).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)((int)rtDesc.SourceBlend).GetHashCode();
                    hash = (hash * 16777619) ^ (uint)rtDesc.IsBlendEnabled.GetHashCode();
                }
                return hash;
            }
        }

        #region -- Singleton --
        public static FrostyDeviceManager Current => current ?? (current = new FrostyDeviceManager());

        private static FrostyDeviceManager current;
        private FrostyDeviceManager()
        {
        }
        #endregion

        private D3D11.Device device;
        private D3D11.DeviceDebug debugDevice;

        // state lists
        private Dictionary<uint, D3D11.SamplerState> samplerStates = new Dictionary<uint, D3D11.SamplerState>();
        private Dictionary<uint, D3D11.DepthStencilState> depthStencilStates = new Dictionary<uint, D3D11.DepthStencilState>();
        private Dictionary<uint, D3D11.BlendState> blendStates = new Dictionary<uint, D3D11.BlendState>();
        private Dictionary<uint, D3D11.RasterizerState> rasterizerStates = new Dictionary<uint, D3D11.RasterizerState>();

        public Controls.FrostyViewport CurrentViewport { get; set; }

        /// <summary>
        /// Returns the global D3D11 device (creates it if necessary)
        /// </summary>
        public D3D11.Device GetDevice()
        {
            if (device == null)
            {
                D3D11.DeviceCreationFlags flags = D3D11.DeviceCreationFlags.BgraSupport;
#if DEBUG
                flags |= D3D11.DeviceCreationFlags.Debug;
#endif

                int adapterIndex = Config.Get<int>("RenderAdapterIndex", 0);
                SharpDX.DXGI.Factory factory = new SharpDX.DXGI.Factory1();
                SharpDX.DXGI.Adapter adapter = factory.GetAdapter(adapterIndex);

                App.Logger.Log("Display Adapters:");
                int index = 0;

                foreach (var currentAdapter in factory.Adapters)
                {
                    App.Logger.Log(string.Format("  {0}: {1}", index++, currentAdapter.Description.Description));
                }

                device = new D3D11.Device(adapter, flags, FeatureLevel.Level_11_0);
#if DEBUG
                debugDevice = new D3D11.DeviceDebug(device);
#endif
                factory.Dispose();
                App.Logger.Log(string.Format("Selected D3D11 Adapter {0}: {1}", adapterIndex, adapter.Description.Description));

                // keep device around until frosty is shutting down
                Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            }

            return device;
        }

        public D3D11.SamplerState GetOrCreateSamplerState(D3D11.SamplerStateDescription desc)
        {
            uint hash = Hashing.Hash(desc);
            if (!samplerStates.ContainsKey(hash))
                samplerStates.Add(hash, new D3D11.SamplerState(GetDevice(), desc));
            return samplerStates[hash];
        }

        public D3D11.DepthStencilState GetOrCreateDepthStencilState(D3D11.DepthStencilStateDescription desc)
        {
            uint hash = Hashing.Hash(desc);
            if (!depthStencilStates.ContainsKey(hash))
                depthStencilStates.Add(hash, new D3D11.DepthStencilState(GetDevice(), desc));
            return depthStencilStates[hash];
        }

        public D3D11.BlendState GetOrCreateBlendState(D3D11.BlendStateDescription desc)
        {
            uint hash = Hashing.Hash(desc);
            if (!blendStates.ContainsKey(hash))
                blendStates.Add(hash, new D3D11.BlendState(GetDevice(), desc));
            return blendStates[hash];
        }

        public D3D11.RasterizerState GetOrCreateRasterizerState(D3D11.RasterizerStateDescription desc)
        {
            uint hash = Hashing.Hash(desc);
            if (!rasterizerStates.ContainsKey(hash))
                rasterizerStates.Add(hash, new D3D11.RasterizerState(GetDevice(), desc));
            return rasterizerStates[hash];
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            // if there is an active viewport, make sure it is shutdown 
            // before the device

            CurrentViewport?.Shutdown();

            // destroy device
            DisposeDevice();
        }

        public void DisposeDevice()
        {
            device?.Dispose();
            device = null;

#if DEBUG
            debugDevice.ReportLiveDeviceObjects(D3D11.ReportingLevel.Detail | D3D11.ReportingLevel.IgnoreInternal);
            debugDevice.Dispose();
#endif
        }
    }

    public class D3DUtils
    {
        public static D3D11.SamplerState CreateSamplerState(D3D11.SamplerStateDescription desc) { return FrostyDeviceManager.Current.GetOrCreateSamplerState(desc); }
        public static D3D11.BlendState CreateBlendState(D3D11.BlendStateDescription desc) { return FrostyDeviceManager.Current.GetOrCreateBlendState(desc); }
        public static D3D11.RasterizerState CreateRasterizerState(D3D11.RasterizerStateDescription desc) { return FrostyDeviceManager.Current.GetOrCreateRasterizerState(desc); }
        public static D3D11.DepthStencilState CreateDepthStencilState(D3D11.DepthStencilStateDescription desc) { return FrostyDeviceManager.Current.GetOrCreateDepthStencilState(desc); }

        public static D3D11.DepthStencilState CreateDepthStencilState(
            bool depthEnabled = true,
            D3D11.DepthWriteMask depthWriteMask = D3D11.DepthWriteMask.All,
            D3D11.Comparison depthComparison = D3D11.Comparison.Less,
            bool stencilEnabled = false,
            byte stencilReadMask = 0xFF,
            byte stencilWriteMask = 0xFF,
            D3D11.DepthStencilOperationDescription? frontFace = null,
            D3D11.DepthStencilOperationDescription? backFace = null)
        {
            D3D11.DepthStencilStateDescription desc = new D3D11.DepthStencilStateDescription()
            {
                IsDepthEnabled = depthEnabled,
                DepthWriteMask = depthWriteMask,
                DepthComparison = depthComparison,
                IsStencilEnabled = stencilEnabled,
                StencilReadMask = stencilReadMask,
                StencilWriteMask = stencilWriteMask
            };
            if (frontFace.HasValue)
                desc.FrontFace = frontFace.Value;
            if (backFace.HasValue)
                desc.BackFace = backFace.Value;

            return CreateDepthStencilState(desc);
        }

        public static D3D11.SamplerState CreateSamplerState(
            D3D11.TextureAddressMode address = D3D11.TextureAddressMode.Wrap,
            D3D11.TextureAddressMode addressU = 0,
            D3D11.TextureAddressMode addressV = 0,
            D3D11.TextureAddressMode addressW = 0,
            Color? borderColor = null,
            D3D11.Comparison comparisonFunc = D3D11.Comparison.Always,
            D3D11.Filter filter = D3D11.Filter.MinMagMipLinear,
            int maxAniso = 16,
            float maxLod = 20,
            float minLod = 0,
            float mipLodBias = 0)
        {
            D3D11.SamplerStateDescription desc = new D3D11.SamplerStateDescription()
            {
                AddressU = (addressU != 0) ? addressU : address,
                AddressV = (addressV != 0) ? addressV : address,
                AddressW = (addressW != 0) ? addressW : address,
                BorderColor = (borderColor.HasValue) ? borderColor.Value : Color.Black,
                ComparisonFunction = comparisonFunc,
                Filter = filter,
                MaximumAnisotropy = maxAniso,
                MaximumLod = maxLod,
                MinimumLod = minLod,
                MipLodBias = mipLodBias
            };
            return CreateSamplerState(desc);
        }

        public static D3D11.RasterizerState CreateRasterizerState(
            D3D11.CullMode cullMode = D3D11.CullMode.Back,
            D3D11.FillMode fillMode = D3D11.FillMode.Solid,
            bool antialiasedLines = false,
            bool depthClip = false,
            bool frontCounterClockwise = false,
            bool multisampled = false,
            bool scissor = false,
            int depthBias = 0,
            float depthBiasClamp = 0.0f,
            float slopeScaledDepthBias = 0.0f)
        {
            D3D11.RasterizerStateDescription desc = new D3D11.RasterizerStateDescription()
            {
                CullMode = cullMode,
                DepthBias = depthBias,
                DepthBiasClamp = depthBiasClamp,
                FillMode = fillMode,
                IsAntialiasedLineEnabled = antialiasedLines,
                IsDepthClipEnabled = depthClip,
                IsFrontCounterClockwise = frontCounterClockwise,
                IsMultisampleEnabled = multisampled,
                IsScissorEnabled = scissor,
                SlopeScaledDepthBias = slopeScaledDepthBias
            };
            return CreateRasterizerState(desc);
        }

        public static D3D11.RenderTargetBlendDescription CreateBlendStateRenderTarget(bool alphaBlend = false)
        {
            D3D11.RenderTargetBlendDescription rtDesc = new D3D11.RenderTargetBlendDescription()
            {
                IsBlendEnabled = false,
                SourceBlend = D3D11.BlendOption.One,
                DestinationBlend = D3D11.BlendOption.Zero,
                BlendOperation = D3D11.BlendOperation.Add,
                SourceAlphaBlend = D3D11.BlendOption.One,
                DestinationAlphaBlend = D3D11.BlendOption.Zero,
                AlphaBlendOperation = D3D11.BlendOperation.Add,
                RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All
            };
            if (alphaBlend)
            {
                rtDesc.SourceBlend = D3D11.BlendOption.SourceAlpha;
                rtDesc.DestinationBlend = D3D11.BlendOption.InverseSourceAlpha;
                rtDesc.BlendOperation = D3D11.BlendOperation.Add;
                rtDesc.SourceAlphaBlend = D3D11.BlendOption.One;
                rtDesc.DestinationAlphaBlend = D3D11.BlendOption.One;
                rtDesc.AlphaBlendOperation = D3D11.BlendOperation.Add;
            }
            return rtDesc;
        }

        public static D3D11.BlendState CreateBlendState(
            params D3D11.RenderTargetBlendDescription[] targets)
        {
            D3D11.BlendStateDescription desc = new D3D11.BlendStateDescription {IndependentBlendEnable = targets.Length > 1};
            for (int i = 0; i < targets.Length; i++)
            {
                desc.RenderTarget[i] = targets[i];
                desc.RenderTarget[i].IsBlendEnabled = true;
            }
            return CreateBlendState(desc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void BeginPerfEvent(D3D11.DeviceContext context, string name)
        {
#if FROSTY_DEVELOPER
            D3D11.UserDefinedAnnotation annotation = context.QueryInterface<D3D11.UserDefinedAnnotation>();
            annotation?.BeginEvent(name);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static void EndPerfEvent(D3D11.DeviceContext context)
        {
#if FROSTY_DEVELOPER
            D3D11.UserDefinedAnnotation annotation = context.QueryInterface<D3D11.UserDefinedAnnotation>();
            annotation?.EndEvent();
#endif
        }
    }

    public static class TextureUtils
    {
        #region -- DDS --
        public enum DDSCaps
        {
            Complex = 0x08,
            MipMap = 0x400000,
            Texture = 0x1000
        };

        [Flags]
        public enum DDSCaps2
        {
            CubeMap = 0x200,
            CubeMapPositiveX = 0x400,
            CubeMapNegativeX = 0x800,
            CubeMapPositiveY = 0x1000,
            CubeMapNegativeY = 0x2000,
            CubeMapPositiveZ = 0x4000,
            CubeMapNegativeZ = 0x8000,
            Volume = 0x200000,

            CubeMapAllFaces = CubeMapPositiveX | CubeMapPositiveY | CubeMapPositiveZ | CubeMapNegativeX | CubeMapNegativeY | CubeMapNegativeZ
        }

        [Flags]
        public enum DDSFlags
        {
            Caps = 0x01,
            Height = 0x02,
            Width = 0x04,
            Pitch = 0x08,
            PixelFormat = 0x1000,
            MipMapCount = 0x20000,
            LinearSize = 0x80000,
            Depth = 0x800000,

            Required = Caps | Height | Width | PixelFormat
        }

        [Flags]
        public enum DDSPFFlags
        {
            AlphaPixels = 0x01,
            Alpha = 0x02,
            FourCC = 0x04,
            RGB = 0x40,
            YUV = 0x200,
            Luminance = 0x20000
        }

        public struct DDSHeaderDX10
        {
            public SharpDX.DXGI.Format dxgiFormat;
            public D3D11.ResourceDimension resourceDimension;
            public uint miscFlag;
            public uint arraySize;
            public uint miscFlags2;
        }

        public struct DDSPixelFormat
        {
            public int dwSize;
            public DDSPFFlags dwFlags;
            public int dwFourCC;
            public int dwRGBBitCount;
            public uint dwRBitMask;
            public uint dwGBitMask;
            public uint dwBBitMask;
            public uint dwABitMask;
        }

        public class DDSHeader
        {
            public int dwMagic;
            public int dwSize;
            public DDSFlags dwFlags;
            public int dwHeight;
            public int dwWidth;
            public int dwPitchOrLinearSize;
            public int dwDepth;
            public int dwMipMapCount;
            public int[] dwReserved1;
            public DDSPixelFormat ddspf;
            public DDSCaps dwCaps;
            public DDSCaps2 dwCaps2;
            public int dwCaps3;
            public int dwCaps4;
            public int dwReserved2;

            public bool HasExtendedHeader;
            public DDSHeaderDX10 ExtendedHeader;

            public DDSHeader()
            {
                dwMagic = 0x20534444;
                dwSize = 0x7C;
                dwFlags = DDSFlags.Required;
                dwDepth = 0;
                dwCaps = DDSCaps.Texture;
                dwCaps2 = 0;
                dwReserved1 = new int[11];
                ddspf.dwSize = 0x20;
                ddspf.dwFlags = DDSPFFlags.FourCC;
                HasExtendedHeader = false;
            }

            public void Write(NativeWriter writer)
            {
                writer.Write(dwMagic);
                writer.Write(dwSize);
                writer.Write((int)dwFlags);
                writer.Write(dwHeight);
                writer.Write(dwWidth);
                writer.Write(dwPitchOrLinearSize);
                writer.Write(dwDepth);
                writer.Write(dwMipMapCount);
                for (int i = 0; i < 11; i++) writer.Write(dwReserved1[i]);
                writer.Write(ddspf.dwSize);
                writer.Write((int)ddspf.dwFlags);
                writer.Write(ddspf.dwFourCC);
                writer.Write(ddspf.dwRGBBitCount);
                writer.Write(ddspf.dwRBitMask);
                writer.Write(ddspf.dwGBitMask);
                writer.Write(ddspf.dwBBitMask);
                writer.Write(ddspf.dwABitMask);
                writer.Write((int)dwCaps);
                writer.Write((int)dwCaps2);
                writer.Write(dwCaps3);
                writer.Write(dwCaps4);
                writer.Write(dwReserved2);

                if (HasExtendedHeader)
                {
                    writer.Write((uint)ExtendedHeader.dxgiFormat);
                    writer.Write((uint)ExtendedHeader.resourceDimension);
                    writer.Write(ExtendedHeader.miscFlag);
                    writer.Write(ExtendedHeader.arraySize);
                    writer.Write(ExtendedHeader.miscFlags2);
                }
            }

            public bool Read(NativeReader reader)
            {
                dwMagic = reader.ReadInt();
                if (dwMagic != 0x20534444)
                    return false;

                dwSize = reader.ReadInt();
                if (dwSize != 0x7C)
                    return false;

                dwFlags = (DDSFlags)reader.ReadInt();
                dwHeight = reader.ReadInt();
                dwWidth = reader.ReadInt();
                dwPitchOrLinearSize = reader.ReadInt();
                dwDepth = reader.ReadInt();
                dwReserved1 = new int[11];
                dwMipMapCount = reader.ReadInt();
                for (int i = 0; i < 11; i++)
                    dwReserved1[i] = reader.ReadInt();
                ddspf.dwSize = reader.ReadInt();
                ddspf.dwFlags = (DDSPFFlags)reader.ReadInt();
                ddspf.dwFourCC = reader.ReadInt();
                ddspf.dwRGBBitCount = reader.ReadInt();
                ddspf.dwRBitMask = reader.ReadUInt();
                ddspf.dwGBitMask = reader.ReadUInt();
                ddspf.dwBBitMask = reader.ReadUInt();
                ddspf.dwABitMask = reader.ReadUInt();
                dwCaps = (DDSCaps)reader.ReadInt();
                dwCaps2 = (DDSCaps2)reader.ReadInt();
                dwCaps3 = reader.ReadInt();
                dwCaps4 = reader.ReadInt();
                dwReserved2 = reader.ReadInt();

                if (ddspf.dwFourCC == 0x30315844)
                {
                    HasExtendedHeader = true;
                    ExtendedHeader.dxgiFormat = (SharpDX.DXGI.Format)reader.ReadUInt();
                    ExtendedHeader.resourceDimension = (D3D11.ResourceDimension)reader.ReadUInt();
                    ExtendedHeader.miscFlag = reader.ReadUInt();
                    ExtendedHeader.arraySize = reader.ReadUInt();
                    ExtendedHeader.miscFlags2 = reader.ReadUInt();
                }

                return true;
            }
        }
        #endregion


        #region -- Texture Loading --
        public static SharpDX.DXGI.Format ToTextureFormat(string pixelFormat, bool bLegacySrgb = false)
        {
            if (bLegacySrgb)
            {
                if (pixelFormat.StartsWith("BC") && bLegacySrgb)
                    pixelFormat = pixelFormat.Replace("UNORM", "SRGB");
            }
            switch (pixelFormat)
            {
                //case "DXT1": return SharpDX.DXGI.Format.BC1_UNorm;
                case "NormalDXT1": return SharpDX.DXGI.Format.BC1_Typeless;
                case "NormalDXN": return SharpDX.DXGI.Format.BC5_Typeless;
                //case "DXT1A": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC1A_SRGB": return SharpDX.DXGI.Format.BC1_Typeless;
                case "BC1A_UNORM": return SharpDX.DXGI.Format.BC1_Typeless;
                case "BC1_SRGB": return SharpDX.DXGI.Format.BC1_Typeless;
                case "BC1_UNORM": return SharpDX.DXGI.Format.BC1_Typeless;
                case "BC2_SRGB": return SharpDX.DXGI.Format.BC2_Typeless;
                case "BC2_UNORM": return SharpDX.DXGI.Format.BC2_Typeless;
                //case "DXT3": return SharpDX.DXGI.Format.BC2_UNorm;
                case "BC3_SRGB": return SharpDX.DXGI.Format.BC3_Typeless;
                case "BC3_UNORM": return SharpDX.DXGI.Format.BC3_Typeless;
                case "BC3A_UNORM": return SharpDX.DXGI.Format.BC3_Typeless;
                case "BC3A_SRGB": return SharpDX.DXGI.Format.BC3_Typeless;
                case "BC4_UNORM": return SharpDX.DXGI.Format.BC4_Typeless;
                //case "DXT5": return SharpDX.DXGI.Format.BC3_UNorm;
                //case "DXT5A": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC5_UNORM": return SharpDX.DXGI.Format.BC5_Typeless;
                case "BC6U_FLOAT": return SharpDX.DXGI.Format.BC6H_Uf16;
                case "BC7": return SharpDX.DXGI.Format.BC7_Typeless;
                case "BC7_SRGB": return SharpDX.DXGI.Format.BC7_Typeless;
                case "BC7_UNORM": return SharpDX.DXGI.Format.BC7_Typeless;
                case "R8_UNORM": return SharpDX.DXGI.Format.R8_Typeless;
                case "R16G16B16A16_FLOAT": return SharpDX.DXGI.Format.R16G16B16A16_Float;
                case "ARGB32F": return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case "R32G32B32A32_FLOAT": return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case "R9G9B9E5F": return SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
                case "R9G9B9E5_FLOAT": return SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
                case "R8G8B8A8_UNORM": return SharpDX.DXGI.Format.R8G8B8A8_Typeless;
                case "R8G8B8A8_SRGB": return SharpDX.DXGI.Format.R8G8B8A8_Typeless;
                case "B8G8R8A8_UNORM": return SharpDX.DXGI.Format.B8G8R8A8_Typeless;
                case "R10G10B10A2_UNORM": return SharpDX.DXGI.Format.R10G10B10A2_Typeless;
                case "L8": return SharpDX.DXGI.Format.R8_Typeless;
                case "L16": return SharpDX.DXGI.Format.R16_Typeless;
                case "ARGB8888": return SharpDX.DXGI.Format.R8G8B8A8_Typeless;
                case "R16G16_UNORM": return SharpDX.DXGI.Format.R16G16_Typeless;
                case "D16_UNORM": return SharpDX.DXGI.Format.R16_UNorm;
                default: return SharpDX.DXGI.Format.Unknown;
            }
        }

        public static SharpDX.DXGI.Format ToShaderFormat(string pixelFormat, bool bLegacySrgb = false)
        {
            if (bLegacySrgb)
            {
                if (pixelFormat.StartsWith("BC") && bLegacySrgb)
                    pixelFormat = pixelFormat.Replace("UNORM", "SRGB");
            }

            switch (pixelFormat)
            {
                //case "DXT1": return SharpDX.DXGI.Format.BC1_UNorm;
                case "NormalDXT1": return SharpDX.DXGI.Format.BC1_UNorm;
                case "NormalDXN": return SharpDX.DXGI.Format.BC5_UNorm;
                //case "DXT1A": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC1A_SRGB": return SharpDX.DXGI.Format.BC1_UNorm_SRgb;
                case "BC1A_UNORM": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC1_SRGB": return SharpDX.DXGI.Format.BC1_UNorm_SRgb;
                case "BC1_UNORM": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC2_SRGB": return SharpDX.DXGI.Format.BC2_UNorm_SRgb;
                case "BC2_UNORM": return SharpDX.DXGI.Format.BC2_UNorm;
                //case "DXT3": return SharpDX.DXGI.Format.BC2_UNorm;
                case "BC3_SRGB": return SharpDX.DXGI.Format.BC3_UNorm_SRgb;
                case "BC3_UNORM": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC3A_UNORM": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC3A_SRGB": return SharpDX.DXGI.Format.BC3_UNorm_SRgb;
                case "BC4_UNORM": return SharpDX.DXGI.Format.BC4_UNorm;
                //case "DXT5": return SharpDX.DXGI.Format.BC3_UNorm;
                //case "DXT5A": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC5_UNORM": return SharpDX.DXGI.Format.BC5_UNorm;
                case "BC6U_FLOAT": return SharpDX.DXGI.Format.BC6H_Uf16;
                case "BC7": return SharpDX.DXGI.Format.BC7_UNorm;
                case "BC7_SRGB": return SharpDX.DXGI.Format.BC7_UNorm_SRgb;
                case "BC7_UNORM": return SharpDX.DXGI.Format.BC7_UNorm;
                case "R8_UNORM": return SharpDX.DXGI.Format.R8_UNorm;
                case "R16G16B16A16_FLOAT": return SharpDX.DXGI.Format.R16G16B16A16_Float;
                case "ARGB32F": return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case "R32G32B32A32_FLOAT": return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case "R9G9B9E5F": return SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
                case "R9G9B9E5_FLOAT": return SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
                case "R8G8B8A8_UNORM": return SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                case "R8G8B8A8_SRGB": return SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                case "B8G8R8A8_UNORM": return SharpDX.DXGI.Format.B8G8R8A8_UNorm;
                case "R10G10B10A2_UNORM": return SharpDX.DXGI.Format.R10G10B10A2_UNorm;
                case "L8": return SharpDX.DXGI.Format.R8_UNorm;
                case "L16": return SharpDX.DXGI.Format.R16_UNorm;
                case "ARGB8888": return SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                case "R16G16_UNORM": return SharpDX.DXGI.Format.R16G16_UNorm;
                case "D16_UNORM": return SharpDX.DXGI.Format.R16_UNorm;
                default: return SharpDX.DXGI.Format.Unknown;
            }
        }

        public static D3D11.Texture2D LoadTexture(D3D11.Device device, string filename, bool generateMips = false)
        {
            D3D11.Texture2D texture = null;
            using (NativeReader reader = new NativeReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
            {
                DDSHeader header = new DDSHeader();
                header.Read(reader);

                SharpDX.DXGI.Format format = SharpDX.DXGI.Format.Unknown;
                int arraySize = ((header.dwCaps2 & DDSCaps2.CubeMap) != 0) ? 6 : 1;

                if (header.HasExtendedHeader)
                {
                    format = header.ExtendedHeader.dxgiFormat;
                }

                D3D11.BindFlags bindFlags = D3D11.BindFlags.ShaderResource;
                D3D11.ResourceOptionFlags roFlags = D3D11.ResourceOptionFlags.None;
                roFlags |= ((header.dwCaps2 & DDSCaps2.CubeMap) != 0) ? D3D11.ResourceOptionFlags.TextureCube : D3D11.ResourceOptionFlags.None;

                int mipCount = header.dwMipMapCount;
                if (generateMips && mipCount == 1)
                {
                    roFlags |= D3D11.ResourceOptionFlags.GenerateMipMaps;
                    bindFlags |= D3D11.BindFlags.RenderTarget;
                    mipCount = 1 + (int)Math.Floor(Math.Log(Math.Max(header.dwWidth, header.dwHeight), 2));
                }

                D3D11.Texture2DDescription desc = new D3D11.Texture2DDescription()
                {
                    BindFlags = bindFlags,
                    Format = format,
                    Width = header.dwWidth,
                    Height = header.dwHeight,
                    MipLevels = mipCount,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                    Usage = D3D11.ResourceUsage.Default,
                    OptionFlags = roFlags,
                    CpuAccessFlags = D3D11.CpuAccessFlags.None,
                    ArraySize = arraySize
                };

                texture = new D3D11.Texture2D(device, desc);

                int stride = (SharpDX.DXGI.FormatHelper.IsCompressed(format))
                    ? SharpDX.DXGI.FormatHelper.SizeOfInBits(format) / 2
                    : SharpDX.DXGI.FormatHelper.SizeOfInBytes(format);
                int minSize = (SharpDX.DXGI.FormatHelper.IsCompressed(format)) ? 4 : 1;

                for (int sliceIdx = 0; sliceIdx < arraySize; sliceIdx++)
                {
                    int width = header.dwWidth;
                    int height = header.dwHeight;

                    for (int mipIdx = 0; mipIdx < header.dwMipMapCount; mipIdx++)
                    {
                        int rowPitch = 0;
                        int subResourceId = texture.CalculateSubResourceIndex(mipIdx, sliceIdx, out rowPitch);

                        int mipSize = mipSize = SharpDX.DXGI.FormatHelper.IsCompressed(format)
                            ? Math.Max(1, ((width + 3) / 4)) * stride * height
                            : width * stride * height;

                        byte[] buffer = reader.ReadBytes(mipSize);
                        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        IntPtr bufferPtr = handle.AddrOfPinnedObject();

                        DataBox box = new DataBox(bufferPtr, width * stride, 0);
                        device.ImmediateContext.UpdateSubresource(box, texture, subResourceId);

                        handle.Free();

                        width >>= 1;
                        height >>= 1;
                        if (width < minSize) width = minSize;
                        if (height < minSize) height = minSize;
                    }
                }
            }

            return texture;
        }

        public static D3D11.Texture2D LoadTexture(D3D11.Device device, Texture textureAsset, bool generateMips = false)
        {
            textureAsset.Data.Position = 0;

            // cube arrays need to use both slice and depth
            int arraySize = (textureAsset.Type == TextureType.TT_CubeArray)
                ? textureAsset.Depth * textureAsset.SliceCount
                : textureAsset.Depth;

            ushort width = textureAsset.Width;
            ushort height = textureAsset.Height;

            SharpDX.DXGI.Format format = TextureUtils.ToTextureFormat(textureAsset.PixelFormat, (textureAsset.Flags & TextureFlags.SrgbGamma) != 0);
            D3D11.ResourceOptionFlags roFlags = D3D11.ResourceOptionFlags.None;
            D3D11.BindFlags bindFlags = D3D11.BindFlags.ShaderResource;
            int mipCount = textureAsset.MipCount;

            if (textureAsset.Type == TextureType.TT_Cube)
                roFlags |= D3D11.ResourceOptionFlags.TextureCube;

            if (!SharpDX.DXGI.FormatHelper.IsCompressed(format))
            {
                if (generateMips && mipCount == 1)
                {
                    roFlags |= D3D11.ResourceOptionFlags.GenerateMipMaps;
                    bindFlags |= D3D11.BindFlags.RenderTarget;
                    mipCount = 1 + (int)Math.Floor(Math.Log(Math.Max(textureAsset.Width, textureAsset.Height), 2));
                }
            }

            D3D11.Texture2DDescription desc = new D3D11.Texture2DDescription()
            {
                BindFlags = bindFlags,
                Format = format,
                Width = textureAsset.Width,
                Height = textureAsset.Height,
                MipLevels = mipCount,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                OptionFlags = roFlags,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                ArraySize = arraySize
            };
            D3D11.Texture2D texture = new D3D11.Texture2D(device, desc);

            // stride differs between compressed formats and standard
            int stride = (SharpDX.DXGI.FormatHelper.IsCompressed(format))
                ? SharpDX.DXGI.FormatHelper.SizeOfInBits(format) / 2
                : SharpDX.DXGI.FormatHelper.SizeOfInBytes(format);

            // fill in texture data
            for (int mip = 0; mip < textureAsset.MipCount; mip++)
            {
                int mipSize = (int)textureAsset.MipSizes[mip];
                if (textureAsset.Type == TextureType.TT_3d)
                {
                    mipSize = SharpDX.DXGI.FormatHelper.IsCompressed(format)
                        ? Math.Max(1, ((width + 3) / 4)) * stride * height
                        : width * stride * height;
                }

                for (int slice = 0; slice < arraySize; slice++)
                {
                    byte[] buffer = new byte[mipSize];
                    textureAsset.Data.Read(buffer, 0, buffer.Length);

                    GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    IntPtr bufferPtr = handle.AddrOfPinnedObject();

                    SharpDX.DataBox box = new SharpDX.DataBox(bufferPtr, width * stride, 0);

                    int tmp = 0;
                    device.ImmediateContext.UpdateSubresource(box, texture, texture.CalculateSubResourceIndex(mip, slice, out tmp));

                    handle.Free();
                }

                width >>= 1;
                height >>= 1;
                if (width < 1) width = 1;
                if (height < 1) height = 1;
            }

            return texture;
        }

        public static bool IsCompressedFormat(string pixelFormat)
        {
            bool isCompressed = true;
            switch (pixelFormat)
            {
                case "R8_UNORM":
                case "R16G16B16A16_FLOAT":
                case "R32G32B32A32_FLOAT":
                case "R9G9B9E5_FLOAT":
                case "R8G8B8A8_UNORM":
                case "R8G8B8A8_SRGB":
                case "B8G8R8A8_UNORM":
                case "R10G10B10A2_UNORM":
                case "ARGB32F":
                case "R9G9B9E5F":
                case "L8":
                case "L16":
                case "ARGB8888":
                case "D16_UNORM":
                    isCompressed = false;
                    break;
            }
            return isCompressed;
        }

        public static int GetFormatBlockSize(string pixelFormat)
        {
            int blockSize = 8;
            switch (pixelFormat)
            {
                case "L8":
                    blockSize = 8;
                    break;

                case "BC3_UNORM":
                case "BC3_SRGB":
                case "BC5_UNORM":
                case "BC5_SRGB":
                case "BC6U_FLOAT":
                case "BC7_UNORM":
                case "BC7_SRGB":
                case "NormalDXN":
                case "BC2_UNORM":
                case "BC3A_UNORM":
                case "L16":
                case "D16_UNORM":
                    blockSize = 16;
                    break;

                case "R9G9B9E5_FLOAT":
                case "R8G8B8A8_UNORM":
                case "R8G8B8A8_SRGB":
                case "B8G8R8A8_UNORM":
                case "R10G10B10A2_UNORM":
                case "R9G9B9E5F":
                case "ARGB8888":
                    blockSize = 32;
                    break;

                case "R16G16B16A16_FLOAT":
                    blockSize = 64;
                    break;

                case "R32G32B32A32_FLOAT":
                case "ARGB32F":
                    blockSize = 128;
                    break;
            }
            return blockSize;
        }
    }
    #endregion
}
