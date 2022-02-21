using Frosty.Core.Screens;
using Frosty.Core.Viewport;
using FrostySdk.IO;
using FrostySdk.Resources;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using D3D11 = SharpDX.Direct3D11;

namespace Frosty.Core.Controls
{
    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyRenderImage))]
    public class FrostyTextureViewer : Control
    {
        private const string PART_Renderer = "PART_Renderer";

        #region -- GridVisible --
        public static readonly DependencyProperty GridVisibleProperty = DependencyProperty.Register("GridVisible", typeof(bool), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(true));
        public bool GridVisible
        {
            get => (bool)GetValue(GridVisibleProperty);
            set => SetValue(GridVisibleProperty, value);
        }
        #endregion

        #region -- Texture --
        public static readonly DependencyProperty TextureProperty = DependencyProperty.Register("Texture", typeof(object), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(null, OnTextureChanged));
        public object Texture
        {
            get => GetValue(TextureProperty);
            set => SetValue(TextureProperty, value);
        }
        protected static void OnTextureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrostyTextureViewer viewer = d as FrostyTextureViewer;
            viewer.LoadTexture((byte[])e.NewValue);
        }
        #endregion

        #region -- RedChannelEnabled --
        public static readonly DependencyProperty RedChannelEnabledProperty = DependencyProperty.Register("RedChannelEnabled", typeof(bool), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(true, OnRedChannelEnabledChanged));
        public bool RedChannelEnabled
        {
            get => (bool)GetValue(RedChannelEnabledProperty);
            set => SetValue(RedChannelEnabledProperty, value);
        }
        protected static void OnRedChannelEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrostyTextureViewer viewer && viewer.renderer != null)
            {
                TextureScreen screen = viewer.renderer.Screen as TextureScreen;
                screen.RedChannelEnabled = (bool)e.NewValue;
            }
        }
        #endregion

        #region -- GreenChannelEnabled --
        public static readonly DependencyProperty GreenChannelEnabledProperty = DependencyProperty.Register("GreenChannelEnabled", typeof(bool), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(true, OnGreenChannelEnabledChanged));
        public bool GreenChannelEnabled
        {
            get => (bool)GetValue(GreenChannelEnabledProperty);
            set => SetValue(GreenChannelEnabledProperty, value);
        }
        protected static void OnGreenChannelEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrostyTextureViewer viewer && viewer.renderer != null)
            {
                TextureScreen screen = viewer.renderer.Screen as TextureScreen;
                screen.GreenChannelEnabled = (bool)e.NewValue;
            }
        }
        #endregion

        #region -- BlueChannelEnabled --
        public static readonly DependencyProperty BlueChannelEnabledProperty = DependencyProperty.Register("BlueChannelEnabled", typeof(bool), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(true, OnBlueChannelEnabledChanged));
        public bool BlueChannelEnabled
        {
            get => (bool)GetValue(BlueChannelEnabledProperty);
            set => SetValue(BlueChannelEnabledProperty, value);
        }
        protected static void OnBlueChannelEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrostyTextureViewer viewer && viewer.renderer != null)
            {
                TextureScreen screen = viewer.renderer.Screen as TextureScreen;
                screen.BlueChannelEnabled = (bool)e.NewValue;
            }
        }
        #endregion

        #region -- AlphaChannelEnabled --
        public static readonly DependencyProperty AlphaChannelEnabledProperty = DependencyProperty.Register("AlphaChannelEnabled", typeof(bool), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(true, OnAlphaChannelEnabledChanged));
        public bool AlphaChannelEnabled
        {
            get => (bool)GetValue(AlphaChannelEnabledProperty);
            set => SetValue(AlphaChannelEnabledProperty, value);
        }
        protected static void OnAlphaChannelEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrostyTextureViewer viewer && viewer.renderer != null)
            {
                TextureScreen screen = viewer.renderer.Screen as TextureScreen;
                screen.AlphaChannelEnabled = (bool)e.NewValue;
            }
        }
        #endregion

        #region -- SrgbEnabled --
        public static readonly DependencyProperty SrgbEnabledProperty = DependencyProperty.Register("SrgbEnabled", typeof(bool), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(true, OnSrgbEnabledChanged));
        public bool SrgbEnabled
        {
            get => (bool)GetValue(SrgbEnabledProperty);
            set => SetValue(SrgbEnabledProperty, value);
        }
        protected static void OnSrgbEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrostyTextureViewer viewer && viewer.renderer != null)
            {
                TextureScreen screen = viewer.renderer.Screen as TextureScreen;
                screen.SrgbEnabled = (bool)e.NewValue;
            }
        }
        #endregion

        #region -- TextureFormat --
        public static readonly DependencyProperty TextureFormatProperty = DependencyProperty.Register("TextureFormat", typeof(string), typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(""));
        public string TextureFormat
        {
            get => (string)GetValue(TextureFormatProperty);
            set => SetValue(TextureFormatProperty, value);
        }
        #endregion

        private FrostyViewport renderer;
        private Texture textureAsset;

        static FrostyTextureViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyTextureViewer), new FrameworkPropertyMetadata(typeof(FrostyTextureViewer)));
        }

        public FrostyTextureViewer()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            renderer = GetTemplateChild(PART_Renderer) as FrostyViewport;
            if (renderer != null)
            {
                TextureScreen screen = new TextureScreen();
                renderer.Screen = screen;

                screen.TextureAsset = textureAsset;
                screen.RedChannelEnabled = RedChannelEnabled;
            }
            UpdateControls();
        }

        public void LoadTexture(byte[] data)
        {
            textureAsset = CreateTexture(data);
            TextureFormat = (textureAsset != null) ? textureAsset.PixelFormat : "";

            if (renderer != null)
            {
                TextureScreen screen = renderer.Screen as TextureScreen;
                screen.TextureAsset = textureAsset;
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            if (textureAsset == null)
                return;

            float newWidth = textureAsset.Width;
            float newHeight = textureAsset.Height;

            if (newWidth > 2048)
            {
                newWidth = 2048;
                newHeight = (newHeight * (newWidth / textureAsset.Width));
            }
            if (newHeight > 2048)
            {
                newHeight = 2048;
                newWidth = (newWidth * (newHeight / textureAsset.Height));
            }

            renderer.Width = newWidth;
            renderer.Height = newHeight;

            SrgbEnabled = textureAsset.PixelFormat.Contains("SRGB") || ((textureAsset.Flags & TextureFlags.SrgbGamma) != 0);
        }

        private Texture CreateTexture(byte[] textureData)
        {
            if (textureData == null)
                return null;

            using (NativeReader reader = new NativeReader(new MemoryStream(textureData)))
            {
                TextureUtils.DDSHeader header = new TextureUtils.DDSHeader();
                if (header.Read(reader))
                {
                    TextureType type = TextureType.TT_2d;
                    if (header.HasExtendedHeader)
                    {
                        if (header.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture2D)
                        {
                            if ((header.ExtendedHeader.miscFlag & 4) != 0)
                                type = TextureType.TT_Cube;
                            else if (header.ExtendedHeader.arraySize > 1)
                                type = TextureType.TT_2dArray;
                        }
                        else if (header.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture3D)
                            type = TextureType.TT_3d;
                    }
                    else
                    {
                        if ((header.dwCaps2 & TextureUtils.DDSCaps2.CubeMap) != 0)
                            type = TextureType.TT_Cube;
                        else if ((header.dwCaps2 & TextureUtils.DDSCaps2.Volume) != 0)
                            type = TextureType.TT_3d;
                    }

                    byte[] buffer = new byte[reader.Length - reader.Position];
                    reader.Read(buffer, 0, (int)(reader.Length - reader.Position));

                    GetPixelFormat(header, out string pixelFormat, out TextureFlags baseFlags);

                    ushort depth = (header.HasExtendedHeader && header.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture2D)
                            ? (ushort)header.ExtendedHeader.arraySize
                            : (ushort)1;
                    byte mipMapCount = (byte)((header.dwMipMapCount == 0) ? 1 : header.dwMipMapCount);

                    // cubemaps are just 6 slice arrays
                    if ((header.dwCaps2 & TextureUtils.DDSCaps2.CubeMap) != 0)
                        depth = 6;
                    if ((header.dwCaps2 & TextureUtils.DDSCaps2.Volume) != 0)
                        depth = (ushort)header.dwDepth;

                    Texture textureAsset = new Texture(type, pixelFormat, (ushort)header.dwWidth, (ushort)header.dwHeight, depth)
                    {
                        FirstMip = 0,
                        TextureGroup = "",
                        Flags = baseFlags
                    };

                    textureAsset.CalculateMipData(mipMapCount, TextureUtils.GetFormatBlockSize(pixelFormat), TextureUtils.IsCompressedFormat(pixelFormat), (uint)buffer.Length);

                    // rejig mips/slices
                    if (textureAsset.Type == TextureType.TT_Cube || textureAsset.Type == TextureType.TT_2dArray)
                    {
                        MemoryStream srcStream = new MemoryStream(buffer);
                        MemoryStream dstStream = new MemoryStream();

                        int sliceCount = 6;
                        if (textureAsset.Type == TextureType.TT_2dArray)
                            sliceCount = textureAsset.Depth;

                        // Need to rejig order of faces and mips
                        uint[] mipOffsets = new uint[textureAsset.MipCount];
                        for (int i = 0; i < textureAsset.MipCount - 1; i++)
                            mipOffsets[i + 1] = mipOffsets[i] + (uint)(textureAsset.MipSizes[i] * sliceCount);

                        byte[] tmpBuf = new byte[textureAsset.MipSizes[0]];

                        for (int slice = 0; slice < sliceCount; slice++)
                        {
                            for (int mip = 0; mip < textureAsset.MipCount; mip++)
                            {
                                int mipSize = (int)textureAsset.MipSizes[mip];

                                srcStream.Read(tmpBuf, 0, mipSize);
                                dstStream.Position = mipOffsets[mip] + (mipSize * slice);
                                dstStream.Write(tmpBuf, 0, mipSize);
                            }
                        }

                        buffer = dstStream.ToArray();
                    }

                    for (int i = 0; i < 4; i++)
                        textureAsset.Unknown3[i] = 0;
                    textureAsset.SetData(buffer);
                    textureAsset.AssetNameHash = 0;

                    return textureAsset;
                }
            }

            return null;
        }

        private void GetPixelFormat(TextureUtils.DDSHeader header, out string pixelFormat, out TextureFlags flags)
        {
            pixelFormat = "Unknown";
            flags = 0;

            // Newer format PixelFormats
            if (header.ddspf.dwFourCC == 0)
            {
                if (header.ddspf.dwRBitMask == 0x000000FF && header.ddspf.dwGBitMask == 0x0000FF00 && header.ddspf.dwBBitMask == 0x00FF0000 && header.ddspf.dwABitMask == 0xFF000000)
                    pixelFormat = "R8G8B8A8_UNORM";
                else
                    pixelFormat = "B8G8R8A8_UNORM";
            }

            // DXT1
            else if (header.ddspf.dwFourCC == 0x31545844)
                pixelFormat = "BC1_UNORM";

            // DXT3
            else if (header.ddspf.dwFourCC == 0x33545844)
                pixelFormat = "BC2_UNORM";

            // DXT5
            else if (header.ddspf.dwFourCC == 0x35545844)
                pixelFormat = "BC3_UNORM";

            // ATI1
            else if (header.ddspf.dwFourCC == 0x31495441)
                pixelFormat = "BC4_UNORM";

            // ATI2 or BC5U
            else if (header.ddspf.dwFourCC == 0x32495441 || header.ddspf.dwFourCC == 0x55354342)
                pixelFormat = "BC5_UNORM";

            // All others
            else if (header.HasExtendedHeader)
            {
                if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC1_UNorm)
                    pixelFormat = "BC1_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC3_UNorm)
                    pixelFormat = "BC3_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC4_UNorm)
                    pixelFormat = "BC4_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC5_UNorm)
                    pixelFormat = "BC5_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC1_UNorm_SRgb && textureAsset.PixelFormat == "BC1A_SRGB")
                    pixelFormat = "BC1A_SRGB";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC1_UNorm_SRgb)
                    pixelFormat = "BC1_SRGB";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC3_UNorm_SRgb)
                    pixelFormat = "BC3_SRGB";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC6H_Uf16)
                    pixelFormat = "BC6U_FLOAT";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC7_UNorm)
                    pixelFormat = "BC7_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC7_UNorm_SRgb)
                    pixelFormat = "BC7_SRGB";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R8_UNorm)
                    pixelFormat = "R8_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R16G16B16A16_Float)
                    pixelFormat = "R16G16B16A16_FLOAT";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R32G32B32A32_Float)
                    pixelFormat = "R32G32B32A32_FLOAT";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R9G9B9E5_Sharedexp)
                    pixelFormat = "R9G9B9E5_FLOAT";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R8G8B8A8_UNorm)
                    pixelFormat = "R8G8B8A8_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb)
                    pixelFormat = "R8G8B8A8_SRGB";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R10G10B10A2_UNorm)
                    pixelFormat = "R10G10B10A2_UNORM";
            }
        }
    }
}
