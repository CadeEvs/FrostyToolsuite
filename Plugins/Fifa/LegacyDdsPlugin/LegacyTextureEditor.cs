using Frosty.Core.Viewport;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using D3D11 = SharpDX.Direct3D11;
using Frosty.Core.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using Frosty.Core.Screens;
using FrostySdk.Managers.Entries;
using TexturePlugin;

namespace LegacyDdsPlugin
{
    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyRenderImage))]
    [TemplatePart(Name = PART_TextureFormat, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_DebugText, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_MipsComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_SliceComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_SliceToolBarItem, Type = typeof(Border))]
    public class LegacyTextureEditor : FrostyAssetEditor
    {
        private const string PART_Renderer = "PART_Renderer";
        private const string PART_TextureFormat = "PART_TextureFormat";
        private const string PART_DebugText = "PART_DebugText";
        private const string PART_MipsComboBox = "PART_MipsComboBox";
        private const string PART_SliceComboBox = "PART_SliceComboBox";
        private const string PART_SliceToolBarItem = "PART_SliceToolBarItem";

        #region -- GridVisible --
        public static readonly DependencyProperty GridVisibleProperty = DependencyProperty.Register("GridVisible", typeof(bool), typeof(LegacyTextureEditor), new FrameworkPropertyMetadata(true));
        public bool GridVisible
        {
            get => (bool)GetValue(GridVisibleProperty);
            set => SetValue(GridVisibleProperty, value);
        }
        #endregion

        private FrostyViewport renderer;
        private TextBlock textureFormatText;
        private Texture textureAsset;
        private TextBox debugTextBox;
        private ComboBox mipsComboBox;
        private ComboBox sliceComboBox;
        private Border sliceToolBarItem;
        private bool textureIsSRGB;
        private TextureUtils.DDSHeader textureHeader;

        //private bool firstTimeLoad = true;

        static LegacyTextureEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegacyTextureEditor), new FrameworkPropertyMetadata(typeof(LegacyTextureEditor)));
        }

        public LegacyTextureEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            renderer = GetTemplateChild(PART_Renderer) as FrostyViewport;
            if (renderer != null)
            {
                textureAsset = CreateTexture();
                textureIsSRGB = textureAsset.PixelFormat.Contains("SRGB") || ((textureAsset.Flags & TextureFlags.SrgbGamma) != 0);

                renderer.Screen = new TextureScreen(textureAsset);
            }

            textureFormatText = GetTemplateChild(PART_TextureFormat) as TextBlock;
            debugTextBox = GetTemplateChild(PART_DebugText) as TextBox;

            mipsComboBox = GetTemplateChild(PART_MipsComboBox) as ComboBox;
            mipsComboBox.SelectionChanged += MipsComboBox_SelectionChanged;

            sliceComboBox = GetTemplateChild(PART_SliceComboBox) as ComboBox;
            sliceComboBox.SelectionChanged += SliceComboBox_SelectionChanged;

            sliceToolBarItem = GetTemplateChild(PART_SliceToolBarItem) as Border;
            if (textureAsset.Depth == 1)
                sliceToolBarItem.Visibility = Visibility.Collapsed;

            UpdateControls();
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new ToolbarItem("Export", "Export Texture", "Images/Export.png", new RelayCommand((object state) => { ExportButton_Click(this, new RoutedEventArgs()); })),
                new ToolbarItem("Import", "Import Texture", "Images/Import.png", new RelayCommand((object state) => { ImportButton_Click(this, new RoutedEventArgs()); })),
            };
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import Texture", "PNG (*.png)|*.png|TGA (*.tga)|*.tga|HDR (*.hdr)|*.hdr|DDS (*.dds)|*.dds", "Texture");
            if (ofd.ShowDialog())
            {
                AssetEntry entry = AssetEntry;
                FrostyTaskWindow.Show("Importing Texture", "", (task) =>
                {
                    ImageFormat fmt = (ImageFormat)(ofd.FilterIndex - 1);
                    byte[] buf = null;
                    BlobData blob = new BlobData();

                    if (fmt == ImageFormat.DDS)
                    {
                        // straight up DDS import
                        buf = NativeReader.ReadInStream(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read));
                    }
                    else
                    {
                        // convert other image types
                        TextureImportOptions options = new TextureImportOptions
                        {
                            type = textureAsset.Type,
                            format = TextureUtils.ToShaderFormat(textureAsset.PixelFormat, (textureAsset.Flags & TextureFlags.SrgbGamma) != 0),
                            generateMipmaps = textureAsset.MipCount > 1,
                            mipmapsFilter = 0,
                            resizeTexture = false,
                            resizeFilter = 0,
                            resizeHeight = 0,
                            resizeWidth = 0
                        };

                        buf = NativeReader.ReadInStream(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read));
                        FrostyTextureEditor.ConvertImageToDDS(buf, buf.Length, (ImageFormat)(ofd.FilterIndex - 1), options, ref blob);
                        buf = ModifyTexture(blob.Data);
                    }

                    App.AssetManager.ModifyCustomAsset("legacy", entry.Name, buf);
                    FrostyTextureEditor.ReleaseBlob(blob);

                });

                TextureScreen screen = renderer.Screen as TextureScreen;

                textureAsset = CreateTexture();
                screen.TextureAsset = textureAsset;

                UpdateControls();
                InvokeOnAssetModified();

                logger.Log("Texture " + ofd.FileName + " successfully imported");
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ImageFormat format = ImageFormat.PNG;
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Export Texture", "PNG (*.png)|*.png|TGA (*.tga)|*.tga|HDR (*.hdr)|*.hdr|DDS (*.dds)|*.dds", "Texture", AssetEntry.Filename, false);
            if (sfd.ShowDialog())
            {
                format = (ImageFormat)(sfd.FilterIndex - 1);
                AssetEntry entry = AssetEntry;

                FrostyTaskWindow.Show("Exporting Texture", AssetEntry.Filename, (task) =>
                {
                    if (sfd.FilterIndex == 4)
                    {
                        // just save out as plain old DDS
                        using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                            writer.Write(NativeReader.ReadInStream(App.AssetManager.GetCustomAsset("legacy", entry)));
                    }
                    else
                    {
                        using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                        {
                            byte[] ddsData = NativeReader.ReadInStream(App.AssetManager.GetCustomAsset("legacy", entry));

                            BlobData blob = new BlobData();
                            FrostyTextureEditor.ConvertDDSToImage(ddsData, ddsData.Length, format, ref blob);
                            writer.Write(blob.Data);
                            FrostyTextureEditor.ReleaseBlob(blob);
                        }
                    }
                });
                logger.Log("Texture successfully exported to " + sfd.FileName);
            }
        }

        private void SliceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextureScreen screen = renderer.Screen as TextureScreen;
            screen.SliceLevel = sliceComboBox.SelectedIndex;
        }

        private void MipsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextureScreen screen = renderer.Screen as TextureScreen;
            screen.MipLevel = mipsComboBox.SelectedIndex;
        }

        private void UpdateControls()
        {
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

            string pf = textureAsset.PixelFormat;
            if (pf.StartsWith("BC") && textureAsset.Flags.HasFlag(TextureFlags.SrgbGamma))
                pf = pf.Replace("UNORM", "SRGB");

            textureFormatText.Text = pf;

            ushort width = textureAsset.Width;
            ushort height = textureAsset.Height;

            mipsComboBox.Items.Clear();
            for (int i = 0; i < textureAsset.MipCount; i++)
            {
                mipsComboBox.Items.Add(string.Format("{0}x{1}", width, height));

                width >>= 1;
                height >>= 1;
            }
            mipsComboBox.SelectedIndex = 0;

            if (textureAsset.Depth > 1)
            {
                sliceComboBox.ItemsSource = null;
                if (textureAsset.Type == TextureType.TT_Cube)
                {
                    // give cube maps actual names for the slices
                    string[] cubeItems = new string[] { "X+", "X-", "Y+", "Y-", "Z+", "Z-" };
                    sliceComboBox.ItemsSource = cubeItems;
                }
                else
                {
                    // other textures just have numbered slices
                    string[] sliceItems = new string[textureAsset.Depth];
                    for (int i = 0; i < textureAsset.Depth; i++)
                        sliceItems[i] = i.ToString();
                    sliceComboBox.ItemsSource = sliceItems;
                }
                sliceComboBox.SelectedIndex = 0;
            }
        }

        private Texture CreateTexture()
        {
            using (NativeReader reader = new NativeReader(App.AssetManager.GetCustomAsset("legacy", AssetEntry)))
            {
                textureHeader = new TextureUtils.DDSHeader();
                if (textureHeader.Read(reader))
                {
                    TextureType type = TextureType.TT_2d;
                    if (textureHeader.HasExtendedHeader)
                    {
                        if (textureHeader.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture2D)
                        {
                            if ((textureHeader.ExtendedHeader.miscFlag & 4) != 0)
                                type = TextureType.TT_Cube;
                            else if (textureHeader.ExtendedHeader.arraySize > 1)
                                type = TextureType.TT_2dArray;
                        }
                        else if (textureHeader.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture3D)
                            type = TextureType.TT_3d;
                    }
                    else
                    {
                        if ((textureHeader.dwCaps2 & TextureUtils.DDSCaps2.CubeMap) != 0)
                            type = TextureType.TT_Cube;
                        else if ((textureHeader.dwCaps2 & TextureUtils.DDSCaps2.Volume) != 0)
                            type = TextureType.TT_3d;
                    }

                    byte[] buffer = new byte[reader.Length - reader.Position];
                    reader.Read(buffer, 0, (int)(reader.Length - reader.Position));

                    GetPixelFormat(textureHeader, out string pixelFormat, out TextureFlags baseFlags);

                    ushort depth = (textureHeader.HasExtendedHeader && textureHeader.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture2D)
                            ? (ushort)textureHeader.ExtendedHeader.arraySize
                            : (ushort)1;
                    byte mipMapCount = (byte)((textureHeader.dwMipMapCount == 0) ? 1 : textureHeader.dwMipMapCount);

                    // cubemaps are just 6 slice arrays
                    if ((textureHeader.dwCaps2 & TextureUtils.DDSCaps2.CubeMap) != 0)
                        depth = 6;
                    if ((textureHeader.dwCaps2 & TextureUtils.DDSCaps2.Volume) != 0)
                        depth = (ushort)textureHeader.dwDepth;

                    Texture textureAsset = new Texture(type, pixelFormat, (ushort)textureHeader.dwWidth, (ushort)textureHeader.dwHeight, depth)
                    {
                        FirstMip = 0,
                        TextureGroup = ""
                    };
                    textureAsset.CalculateMipData(mipMapCount, TextureUtils.GetFormatBlockSize(pixelFormat), TextureUtils.IsCompressedFormat(pixelFormat), (uint)buffer.Length);
                    textureAsset.Flags = baseFlags;

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

        private byte[] ModifyTexture(byte[] data)
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                using (NativeReader reader = new NativeReader(new MemoryStream(data)))
                {
                    TextureUtils.DDSHeader newHeader = new TextureUtils.DDSHeader();
                    newHeader.Read(reader);

                    SharpDX.DXGI.Format format = newHeader.ExtendedHeader.dxgiFormat;
                    int stride = (SharpDX.DXGI.FormatHelper.IsCompressed(format))
                        ? SharpDX.DXGI.FormatHelper.SizeOfInBits(format) / 2
                        : SharpDX.DXGI.FormatHelper.SizeOfInBytes(format);

                    textureHeader.dwWidth = newHeader.dwWidth;
                    textureHeader.dwHeight = newHeader.dwHeight;

                    int width = textureHeader.dwWidth;
                    int height = textureHeader.dwHeight;
                    int mipCount = (textureHeader.dwMipMapCount == 0) ? 1 : textureHeader.dwMipMapCount;
                    int totalSize = 0;

                    // calculate total size
                    for (int i = 0; i < mipCount; i++)
                    {
                        int size = SharpDX.DXGI.FormatHelper.IsCompressed(format)
                            ? Math.Max(1, ((width + 3) / 4)) * stride * height
                            : width * stride * height;
                        totalSize += size;

                        width >>= 1;
                        height >>= 1;
                        if (width < 1) width = 1;
                        if (height < 1) height = 1;
                    }

                    // write header
                    textureHeader.dwPitchOrLinearSize = totalSize;
                    textureHeader.Write(writer);

                    width = textureHeader.dwWidth;
                    height = textureHeader.dwHeight;

                    // write out mip levels
                    for (int i = 0; i < mipCount; i++)
                    {
                        int size = SharpDX.DXGI.FormatHelper.IsCompressed(format)
                            ? Math.Max(1, ((width + 3) / 4)) * stride * height
                            : width * stride * height;

                        writer.Write(reader.ReadBytes(size));

                        width >>= 1;
                        height >>= 1;
                        if (width < 1) width = 1;
                        if (height < 1) height = 1;
                    }
                }

                return writer.ToByteArray();;
            }
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
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.B8G8R8A8_UNorm)
                    pixelFormat = "B8G8R8A8_UNORM";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb)
                    pixelFormat = "B8G8R8A8_SRGB";
                else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R10G10B10A2_UNorm)
                    pixelFormat = "R10G10B10A2_UNORM";
            }
        }
    }
}
