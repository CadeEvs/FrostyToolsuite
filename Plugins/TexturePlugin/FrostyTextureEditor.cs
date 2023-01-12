using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using D3D11 = SharpDX.Direct3D11;
using Frosty.Controls;
using FrostySdk.Interfaces;
using Frosty.Core.Viewport;
using System.Collections.Generic;
using System.Linq;
using Frosty.Hash;
using Frosty.Core.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using Frosty.Core.Screens;
using System.Runtime.InteropServices;
using FrostySdk.Managers.Entries;

namespace TexturePlugin
{
    #region -- Import / Export --
    public struct BlobData
    {
        public byte[] Data
        {
            get
            {
                byte[] outBuf = new byte[size];
                Marshal.Copy(data, outBuf, 0, (int)size);
                return outBuf;
            }
        }
        private IntPtr data;
        private long size;
    }

    public enum ImageFormat
    {
        PNG,
        TGA,
        HDR,
        DDS
    }

    public struct TextureImportOptions
    {
        public TextureType type;
        public SharpDX.DXGI.Format format;
        public bool generateMipmaps;
        public int mipmapsFilter;
        public bool resizeTexture;
        public int resizeFilter;
        public int resizeWidth;
        public int resizeHeight;
    };
    #endregion

    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyRenderImage))]
    [TemplatePart(Name = PART_TextureFormat, Type = typeof(TextBlock))]
    //[TemplatePart(Name = PART_TextureGroup, Type = typeof(Label))]
    [TemplatePart(Name = PART_DebugText, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_MipsComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_SliceComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_SliceToolBarItem, Type = typeof(Border))]
    public class FrostyTextureEditor : FrostyAssetEditor
    {
        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ConvertDDSToImage")]
        public static extern void ConvertDDSToImage(byte[] pData, long iDataSize, ImageFormat format, ref BlobData pOutData);
        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ConvertDDSToImages")]
        public static extern void ConvertDDSToImages(byte[] pData, long iDataSize, ImageFormat format,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
            ref BlobData[] pOutDatas, int pOutCount);
        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ConvertImageToDDS")]
        public static extern void ConvertImageToDDS(byte[] pData, long iDataSize, ImageFormat origFormat, TextureImportOptions options, ref BlobData pOutData);
        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ConvertImagesToDDS")]
        public static extern void ConvertImagesToDDS(byte[] pData, long[] iDataSize, long iCount, ImageFormat origFormat, TextureImportOptions options, ref BlobData pOutData);
        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ReleaseBlob")]
        public static extern void ReleaseBlob(BlobData pData);

        private const string PART_Renderer = "PART_Renderer";
        private const string PART_TextureFormat = "PART_TextureFormat";
        //private const string PART_TextureGroup = "PART_TextureGroup";
        private const string PART_DebugText = "PART_DebugText";
        private const string PART_MipsComboBox = "PART_MipsComboBox";
        private const string PART_SliceComboBox = "PART_SliceComboBox";
        private const string PART_SliceToolBarItem = "PART_SliceToolBarItem";

        #region -- GridVisible --
        public static readonly DependencyProperty GridVisibleProperty = DependencyProperty.Register("GridVisible", typeof(bool), typeof(FrostyTextureEditor), new FrameworkPropertyMetadata(true));
        public bool GridVisible
        {
            get => (bool)GetValue(GridVisibleProperty);
            set => SetValue(GridVisibleProperty, value);
        }
        #endregion

        private FrostyViewport m_renderer;
        private TextBlock m_textureFormatText;
        //private Label textureGroupText;
        private Texture m_textureAsset;
        private TextBox m_debugTextBox;
        private ComboBox m_mipsComboBox;
        private ComboBox m_sliceComboBox;
        private Border m_sliceToolBarItem;
        private bool m_textureIsSrgb;

        static FrostyTextureEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyTextureEditor), new FrameworkPropertyMetadata(typeof(FrostyTextureEditor)));
        }

        public FrostyTextureEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_renderer = GetTemplateChild(PART_Renderer) as FrostyViewport;
            if (m_renderer != null)
            {
                ulong resRid = ((dynamic)RootObject).Resource;
                m_textureAsset = App.AssetManager.GetResAs<Texture>(App.AssetManager.GetResEntry(resRid));
                m_textureIsSrgb = m_textureAsset.PixelFormat.Contains("SRGB") || ((m_textureAsset.Flags & TextureFlags.SrgbGamma) != 0);

                m_renderer.Screen = new TextureScreen(m_textureAsset);
            }

            m_textureFormatText = GetTemplateChild(PART_TextureFormat) as TextBlock;
            m_debugTextBox = GetTemplateChild(PART_DebugText) as TextBox;

            m_mipsComboBox = GetTemplateChild(PART_MipsComboBox) as ComboBox;
            m_mipsComboBox.SelectionChanged += MipsComboBox_SelectionChanged;

            m_sliceComboBox = GetTemplateChild(PART_SliceComboBox) as ComboBox;
            m_sliceComboBox.SelectionChanged += SliceComboBox_SelectionChanged;

            m_sliceToolBarItem = GetTemplateChild(PART_SliceToolBarItem) as Border;
            if (m_textureAsset.Depth == 1)
            {
                m_sliceToolBarItem.Visibility = Visibility.Collapsed;
            }

            UpdateControls();
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            List<ToolbarItem> toolbarItems = base.RegisterToolbarItems();
            toolbarItems.Add(new ToolbarItem("Export", "Export Texture", "Images/Export.png", new RelayCommand((object state) => { ExportButton_Click(this, new RoutedEventArgs()); })));
            toolbarItems.Add(new ToolbarItem("Import", "Import Texture", "Images/Import.png", new RelayCommand((object state) => { ImportButton_Click(this, new RoutedEventArgs()); })));
            
            return toolbarItems;
        }

        private void SliceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextureScreen screen = m_renderer.Screen as TextureScreen;
            screen.SliceLevel = m_sliceComboBox.SelectedIndex;
        }

        private void MipsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextureScreen screen = m_renderer.Screen as TextureScreen;
            screen.MipLevel = m_mipsComboBox.SelectedIndex;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import Texture", "PNG (*.png)|*.png|TGA (*.tga)|*.tga|HDR (*.hdr)|*.hdr|DDS (*.dds)|*.dds", "Texture");
            if (m_textureAsset.Type != TextureType.TT_2d)
            {
                ofd.Multiselect = true;
                ofd.Title = "Import Textures";
            }

            if (ofd.ShowDialog())
            {
                FrostyTextureImportSettings settings = null;
                if (m_textureAsset.Type == TextureType.TT_Cube)
                {
                    if (ofd.FileNames.Length < 6)
                    {
                        // Error if not enough textures selected to create a cubemap
                        logger.LogWarning("Failed to import. Cubemaps require 6 textures, only {0} were selected", ofd.FileNames.Length);
                        FrostyMessageBox.Show("Texture failed to import", "Frosty Editor", MessageBoxButton.OK);
                        return;
                    }

                    // collect textures and assign faces
                    settings = new FrostyTextureImportSettings();
                    for (int i = 0; i < 6; i++)
                    {
                        FrostyTextureCubeItem face = new FrostyTextureCubeItem
                        {
                            Filename = ofd.FileNames[i],
                            Face = (FrostyTextureCubeFace)i
                        };
                        settings.Textures.Add(face);
                    }

                    while (true)
                    {
                        if (FrostyImportExportBox.Show("Import Textures", FrostyImportExportType.Import, settings) == MessageBoxResult.Cancel)
                            return;

                        bool[] faceExists = new bool[6];
                        foreach (FrostyTextureCubeItem item in settings.Textures)
                        {
                            // not duplicate
                            if (faceExists[(int)(item.Face)])
                                break;
                            faceExists[(int)(item.Face)] = true;
                        }

                        // exit loop if all faces exist
                        if (faceExists.All((bool a) => a==true))
                            break;

                        FrostyMessageBox.Show("There was a duplicate or missing face defined, please try again", "Frosty Editor", MessageBoxButton.OK);
                    }

                    // sort the textures based on cube face
                    settings.Textures.Sort((a, b) => (((FrostyTextureCubeItem)a).Face < ((FrostyTextureCubeItem)b).Face) ? -1 : 1);
                }
                else if (m_textureAsset.Type == TextureType.TT_2dArray || m_textureAsset.Type == TextureType.TT_3d)
                {
                    // collect textures and assign slices
                    settings = new FrostyTextureImportSettings();
                    for (int i = 0; i < ofd.FileNames.Length; i++)
                    {
                        FrostyTextureArrayItem slice = new FrostyTextureArrayItem
                        {
                            Filename = ofd.FileNames[i],
                            Slice = i
                        };
                        settings.Textures.Add(slice);
                    }

                    while (true)
                    {
                        if (FrostyImportExportBox.Show("Import Textures", FrostyImportExportType.Import, settings) == MessageBoxResult.Cancel)
                            return;

                        bool[] faceExists = new bool[settings.Textures.Count];
                        foreach (FrostyTextureArrayItem item in settings.Textures)
                        {
                            // within valid range
                            if (item.Slice >= faceExists.Length || item.Slice < 0)
                                break;
                            // not duplicate
                            if (faceExists[item.Slice])
                                break;
                            faceExists[item.Slice] = true;
                        }

                        // exit loop if all faces exist
                        if (faceExists.All((bool a) => a==true))
                            break;

                        FrostyMessageBox.Show("There was a duplicate or missing slice defined, please try again", "Frosty Editor", MessageBoxButton.OK);
                    }

                    // sort the textures based on slice
                    settings.Textures.Sort((a, b) => (((FrostyTextureArrayItem)a).Slice < ((FrostyTextureArrayItem)b).Slice) ? -1 : 1);
                }

                EbxAssetEntry assetEntry = AssetEntry as EbxAssetEntry;
                ulong resRid = ((dynamic)RootObject).Resource;

                bool bFailed = false;
                string errorMsg = "";

                FrostyTaskWindow.Show("Importing Texture", "", (task) =>
                {
                    ImageFormat fmt = (ImageFormat)(ofd.FilterIndex - 1);
                    MemoryStream memStream = null;
                    BlobData blob = new BlobData();

                    if (fmt == ImageFormat.DDS)
                    {
                        // straight up DDS import
                        byte[] buf = NativeReader.ReadInStream(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read));
                        memStream = new MemoryStream(buf);
                    }
                    else
                    {
                        // convert other image types
                        TextureImportOptions options = new TextureImportOptions
                        {
                            type = m_textureAsset.Type,
                            format = TextureUtils.ToShaderFormat(m_textureAsset.PixelFormat, (m_textureAsset.Flags & TextureFlags.SrgbGamma) != 0),
                            generateMipmaps = m_textureAsset.MipCount > 1,
                            mipmapsFilter = 0,
                            resizeTexture = false,
                            resizeFilter = 0,
                            resizeHeight = 0,
                            resizeWidth = 0
                        };

                        if (m_textureAsset.Type == TextureType.TT_2d)
                        {
                            // one image to one DDS
                            byte[] buf = NativeReader.ReadInStream(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read));
                            ConvertImageToDDS(buf, buf.Length, (ImageFormat)(ofd.FilterIndex - 1), options, ref blob);
                        }
                        else
                        {
                            string[] fileNames = new string[settings.Textures.Count];
                            for (int i = 0; i < settings.Textures.Count; i++)
                                fileNames[i] = settings.Textures[i].Filename;

                            // multiple images to one DDS
                            byte[] buf = new byte[0];
                            long[] sizes = new long[fileNames.Length];

                            for (int i = 0; i < fileNames.Length; i++)
                            {
                                byte[] tmpBuf = NativeReader.ReadInStream(new FileStream(fileNames[i], FileMode.Open, FileAccess.Read));
                                sizes[i] = tmpBuf.Length;

                                Array.Resize<byte>(ref buf, buf.Length + tmpBuf.Length);
                                Array.Copy(tmpBuf, 0, buf, buf.Length - tmpBuf.Length, tmpBuf.Length);
                            }

                            ConvertImagesToDDS(buf, sizes, sizes.Length, (ImageFormat)(ofd.FilterIndex - 1), options, ref blob);
                        }
                        memStream = new MemoryStream(blob.Data);
                    }

                    using (NativeReader reader = new NativeReader(memStream))
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

                            if (type != m_textureAsset.Type)
                            {
                                errorMsg = $"Imported texture must match original texture type. Original texture type is {m_textureAsset.Type}. Imported texture type is {type}";
                                bFailed = true;
                            }

                            if (!bFailed)
                            {
                                if (m_textureAsset.Type == TextureType.TT_2dArray)
                                {
                                    // @todo: additional validation
                                }
                                else if (m_textureAsset.Type == TextureType.TT_3d)
                                {
                                    // @todo: additional validation
                                }
                                else if (m_textureAsset.Type == TextureType.TT_Cube)
                                {
                                    // @todo: additional validation
                                }

                                if (!bFailed && m_textureIsSrgb)
                                {
                                    // dont allow changing of SRGB to non SRGB
                                    if (!header.HasExtendedHeader || !header.ExtendedHeader.dxgiFormat.ToString().ToLower().Contains("srgb"))
                                    {
                                        errorMsg = string.Format("Format must be SRGB variant");
                                        bFailed = true;
                                    }
                                }
                            }

                            GetPixelFormat(header, out string pixelFormat, out TextureFlags baseFlags);

                            // make sure texture mip maps can be generated
                            if (TextureUtils.IsCompressedFormat(pixelFormat) && m_textureAsset.MipCount > 1)
                            {
                                if (header.dwWidth % 4 != 0 || header.dwHeight % 4 != 0)
                                {
                                    errorMsg = "Texture width/height must be divisible by 4 for compressed formats requiring mip maps";
                                    bFailed = true;
                                }
                            }

                            if (!bFailed)
                            {
                                ResAssetEntry resEntry = App.AssetManager.GetResEntry(resRid);
                                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(m_textureAsset.ChunkId);

                                // revert any modifications
                                //App.AssetManager.RevertAsset(resEntry, dataOnly: true);

                                byte[] buffer = new byte[reader.Length - reader.Position];
                                reader.Read(buffer, 0, (int)(reader.Length - reader.Position));

                                ushort depth = (header.HasExtendedHeader && header.ExtendedHeader.resourceDimension == D3D11.ResourceDimension.Texture2D)
                                        ? (ushort)header.ExtendedHeader.arraySize
                                        : (ushort)1;

                                // cubemaps are just 6 slice arrays
                                if ((header.dwCaps2 & TextureUtils.DDSCaps2.CubeMap) != 0)
                                    depth = 6;
                                if ((header.dwCaps2 & TextureUtils.DDSCaps2.Volume) != 0)
                                    depth = (ushort)header.dwDepth;

                                Texture newTextureAsset = new Texture(m_textureAsset.Type, pixelFormat, (ushort)header.dwWidth, (ushort)header.dwHeight, depth, m_textureAsset.Version, m_textureAsset.ResourceMeta) { FirstMip = m_textureAsset.FirstMip };
                                if (header.dwMipMapCount <= m_textureAsset.FirstMip)
                                    newTextureAsset.FirstMip = 0;

                                newTextureAsset.TextureGroup = m_textureAsset.TextureGroup;
                                newTextureAsset.CalculateMipData((byte)header.dwMipMapCount, TextureUtils.GetFormatBlockSize(pixelFormat), TextureUtils.IsCompressedFormat(pixelFormat), (uint)buffer.Length);
                                newTextureAsset.Flags = baseFlags;

                                // just copy old flags (minus gamma) to new texture
                                TextureFlags oldFlags = m_textureAsset.Flags & ~(TextureFlags.SrgbGamma);
                                newTextureAsset.Flags |= oldFlags;

                                // rejig mips/slices
                                if (newTextureAsset.Type == TextureType.TT_Cube || newTextureAsset.Type == TextureType.TT_2dArray)
                                {
                                    MemoryStream srcStream = new MemoryStream(buffer);
                                    MemoryStream dstStream = new MemoryStream();

                                    int sliceCount = 6;
                                    if (newTextureAsset.Type == TextureType.TT_2dArray)
                                        sliceCount = newTextureAsset.Depth;

                                    // Need to rejig order of faces and mips
                                    uint[] mipOffsets = new uint[newTextureAsset.MipCount];
                                    for (int i = 0; i < newTextureAsset.MipCount - 1; i++)
                                        mipOffsets[i + 1] = mipOffsets[i] + (uint)(newTextureAsset.MipSizes[i] * sliceCount);

                                    byte[] tmpBuf = new byte[newTextureAsset.MipSizes[0]];

                                    for (int slice = 0; slice < sliceCount; slice++)
                                    {
                                        for (int mip = 0; mip < newTextureAsset.MipCount; mip++)
                                        {
                                            int mipSize = (int)newTextureAsset.MipSizes[mip];

                                            srcStream.Read(tmpBuf, 0, mipSize);
                                            dstStream.Position = mipOffsets[mip] + (mipSize * slice);
                                            dstStream.Write(tmpBuf, 0, mipSize);
                                        }
                                    }

                                    buffer = dstStream.ToArray();
                                }

                                // modify chunk
                                if (ProfilesLibrary.MustAddChunks && chunkEntry.Bundles.Count == 0 && !chunkEntry.IsAdded)
                                {
                                    // DAI requires adding new chunks if in chunks bundle
                                    List<int> sbIds = chunkEntry.SuperBundles;
                                    m_textureAsset.ChunkId = App.AssetManager.AddChunk(buffer, null, (newTextureAsset.Flags & TextureFlags.OnDemandLoaded) != 0 ? null : newTextureAsset);
                                    chunkEntry = App.AssetManager.GetChunkEntry(m_textureAsset.ChunkId);
                                    chunkEntry.AddedSuperBundles.AddRange(sbIds);
                                }
                                else
                                {
                                    // other games just modify
                                    App.AssetManager.ModifyChunk(m_textureAsset.ChunkId, buffer, ((newTextureAsset.Flags & TextureFlags.OnDemandLoaded) != 0 || newTextureAsset.Type != TextureType.TT_2d) ? null : newTextureAsset);
                                }

                                for (int i = 0; i < 4; i++)
                                    newTextureAsset.Unknown3[i] = m_textureAsset.Unknown3[i];
                                newTextureAsset.SetData(m_textureAsset.ChunkId, App.AssetManager);
                                newTextureAsset.AssetNameHash = (uint)Fnv1.HashString(resEntry.Name);

                                m_textureAsset.Dispose();
                                m_textureAsset = newTextureAsset;

                                // modify resource
                                App.AssetManager.ModifyRes(resRid, newTextureAsset);

                                // update linkage
                                resEntry.LinkAsset(chunkEntry);
                                assetEntry.LinkAsset(resEntry);
                            }
                        }
                        else
                        {
                            errorMsg = string.Format("Invalid DDS format");
                            bFailed = true;
                        }
                    }

                    ReleaseBlob(blob);
                });

                string message = "Texture " + ofd.FileName + " failed to import: " + errorMsg;
                if (!bFailed)
                {
                    TextureScreen screen = m_renderer.Screen as TextureScreen;
                    screen.TextureAsset = m_textureAsset;

                    UpdateControls();
                    InvokeOnAssetModified();

                    message = "Texture " + ofd.FileName + " successfully imported";
                }
                
                logger.Log(message);
            }
        }

        private void UpdateControls()
        {
            float newWidth = m_textureAsset.Width;
            float newHeight = m_textureAsset.Height;

            if (newWidth > 2048)
            {
                newWidth = 2048;
                newHeight = (newHeight * (newWidth / m_textureAsset.Width));
            }
            if (newHeight > 2048)
            {
                newHeight = 2048;
                newWidth = (newWidth * (newHeight / m_textureAsset.Height));
            }

            m_renderer.Width = newWidth;
            m_renderer.Height = newHeight;

            string pf = m_textureAsset.PixelFormat;
            if (pf.StartsWith("BC") && m_textureAsset.Flags.HasFlag(TextureFlags.SrgbGamma))
                pf = pf.Replace("UNORM", "SRGB");

            m_textureFormatText.Text = pf;
            //textureGroupText.Content = textureAsset.TextureGroup;
            //debugTextBox.Text = textureAsset.ToDebugString();

            ushort width = m_textureAsset.Width;
            ushort height = m_textureAsset.Height;

            m_mipsComboBox.Items.Clear();
            for (int i = 0; i < m_textureAsset.MipCount; i++)
            {
                m_mipsComboBox.Items.Add(string.Format("{0}x{1}", width, height));

                width >>= 1;
                height >>= 1;
            }
            m_mipsComboBox.SelectedIndex = 0;

            if (m_textureAsset.Depth > 1)
            {
                m_sliceComboBox.ItemsSource = null;
                if (m_textureAsset.Type == TextureType.TT_Cube)
                {
                    // give cube maps actual names for the slices
                    string[] cubeItems = new string[] { "X+", "X-", "Y+", "Y-", "Z+", "Z-" };
                    m_sliceComboBox.ItemsSource = cubeItems;
                }
                else
                {
                    // other textures just have numbered slices
                    string[] sliceItems = new string[m_textureAsset.Depth];
                    for (int i = 0; i < m_textureAsset.Depth; i++)
                        sliceItems[i] = i.ToString();
                    m_sliceComboBox.ItemsSource = sliceItems;
                }
                m_sliceComboBox.SelectedIndex = 0;
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ImageFormat format = ImageFormat.PNG;
            bool bResult = false;

            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Export Texture", "PNG (*.png)|*.png|TGA (*.tga)|*.tga|HDR (*.hdr)|*.hdr|DDS (*.dds)|*.dds", "Texture", AssetEntry.Filename, false);
            while (true)
            {
                string initialDir = sfd.InitialDirectory;
                bResult = sfd.ShowDialog();

                if (bResult)
                {
                    format = (ImageFormat)(sfd.FilterIndex - 1);

                    FileInfo fi = new FileInfo(sfd.FileName);
                    sfd.InitialDirectory = fi.DirectoryName;

                    if (m_textureAsset.Type == TextureType.TT_2d || format == ImageFormat.DDS)
                    {
                        if (fi.Exists)
                        {
                            if (FrostyMessageBox.Show(sfd.FileName + " already exists\r\nDo you want to replace it?", "Frosty Editor", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                break;
                        }
                        else break;
                    }
                    else if (m_textureAsset.Type == TextureType.TT_2dArray || m_textureAsset.Type == TextureType.TT_Cube || m_textureAsset.Type == TextureType.TT_3d)
                    {
                        string[] filenames = null;
                        if (m_textureAsset.Type == TextureType.TT_Cube)
                        {
                            filenames = new string[6] { "px", "nx", "py", "ny", "pz", "nz" };
                            for (int i = 0; i < 6; i++)
                                filenames[i] = string.Format("{0}_{1}{2}", fi.FullName.Replace(fi.Extension, ""), filenames[i], fi.Extension);
                        }
                        else
                        {
                            filenames = new string[m_textureAsset.SliceCount];
                            for (int i = 0; i < m_textureAsset.SliceCount; i++)
                                filenames[i] = string.Format("{0}_{1}{2}", fi.FullName.Replace(fi.Extension, ""), i.ToString("D3"), fi.Extension);
                        }

                        bool bExists = false;
                        foreach(string filename in filenames)
                        {
                            if (File.Exists(filename))
                                bExists |= true;
                        }

                        if (bExists)
                        {
                            if (FrostyMessageBox.Show("One of more of the requested files already exists\r\nDo you want to replace them?", "Frosty Editor", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                break;
                        }
                        else break;
                    }
                }
                else break;
            }

            if (!bResult)
                return;

            FrostyTaskWindow.Show("Exporting Texture", AssetEntry.Filename, (task) =>
            {
                string[] filters = new string[] { "*.png", "*.tga", "*.hdr", "*.dds" };

                TextureExporter exporter = new TextureExporter();
                exporter.Export(m_textureAsset, sfd.FileName, filters[sfd.FilterIndex - 1]);
            });
            logger.Log("Texture successfully exported to " + sfd.FileName);
        }

        private void GetPixelFormat(TextureUtils.DDSHeader header, out string pixelFormat, out TextureFlags flags)
        {
            pixelFormat = "Unknown";
            flags = 0;

            if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4, ProfileVersion.PlantsVsZombiesGardenWarfare, ProfileVersion.NeedForSpeedRivals))
            {
                // DXT1
                if (header.ddspf.dwFourCC == 0x31545844)
                {
                    pixelFormat = "BC1_UNORM";
                    if (m_textureAsset.PixelFormat.Contains("Normal"))
                        pixelFormat = m_textureAsset.PixelFormat;
                    else if (m_textureAsset.PixelFormat.StartsWith("BC1A"))
                        pixelFormat = m_textureAsset.PixelFormat;
                }

                // ATI2 or BC5U
                else if (header.ddspf.dwFourCC == 0x32495441 || header.ddspf.dwFourCC == 0x55354342)
                    pixelFormat = "NormalDXN";

                // DXT3
                else if (header.ddspf.dwFourCC == 0x33545844)
                    pixelFormat = "BC2_UNORM";

                // DXT5
                else if (header.ddspf.dwFourCC == 0x35545844)
                    pixelFormat = "BC3_UNORM";

                // ATI1
                else if (header.ddspf.dwFourCC == 0x31495441)
                    pixelFormat = "BC3A_UNORM";

                // All others
                else if (header.HasExtendedHeader)
                {
                    switch(header.ExtendedHeader.dxgiFormat)
                    {
                        case SharpDX.DXGI.Format.R32G32B32A32_Float: pixelFormat = "ARGB32F"; break;
                        case SharpDX.DXGI.Format.R9G9B9E5_Sharedexp: pixelFormat = "R9G9B9E5F"; break;
                        case SharpDX.DXGI.Format.R8_UNorm: pixelFormat = "L8"; break;
                        case SharpDX.DXGI.Format.R16_UNorm: pixelFormat = "L16"; break;
                        case SharpDX.DXGI.Format.R8G8B8A8_UNorm: pixelFormat = "ARGB8888"; break;
                        case SharpDX.DXGI.Format.BC1_UNorm:
                            pixelFormat = "BC1_UNORM";
                            if (m_textureAsset.PixelFormat.Contains("Normal") || m_textureAsset.PixelFormat.StartsWith("BC1A"))
                                pixelFormat = m_textureAsset.PixelFormat;
                            break;
                        case SharpDX.DXGI.Format.BC2_UNorm: pixelFormat = "BC2_UNORM"; break;
                        case SharpDX.DXGI.Format.BC3_UNorm: pixelFormat = "BC3_UNORM"; break;
                        case SharpDX.DXGI.Format.BC5_UNorm: pixelFormat = "NormalDXN"; break;
                        case SharpDX.DXGI.Format.BC7_UNorm: pixelFormat = "BC7_UNORM"; break;
                        case SharpDX.DXGI.Format.BC1_UNorm_SRgb: pixelFormat = "BC1_UNORM"; flags = TextureFlags.SrgbGamma; break;
                        case SharpDX.DXGI.Format.BC2_UNorm_SRgb: pixelFormat = "BC2_UNORM"; flags = TextureFlags.SrgbGamma; break;
                        case SharpDX.DXGI.Format.BC3_UNorm_SRgb:
                            pixelFormat = (m_textureAsset.PixelFormat == "BC3A_UNORM") ? m_textureAsset.PixelFormat : "BC3_UNORM";
                            flags = TextureFlags.SrgbGamma;
                            break;
                        case SharpDX.DXGI.Format.BC7_UNorm_SRgb: pixelFormat = "BC7_UNORM"; flags = TextureFlags.SrgbGamma; break;
                    }
                }
            }
            else
            {
                // Newer format PixelFormats
                if (header.ddspf.dwFourCC == 0)
                {
                    if (header.ddspf.dwRBitMask == 0x000000FF && header.ddspf.dwGBitMask == 0x0000FF00 && header.ddspf.dwBBitMask == 0x00FF0000 && header.ddspf.dwABitMask == 0xFF000000)
                        pixelFormat = "R8G8B8A8_UNORM";
                }

                // DXT1
                else if (header.ddspf.dwFourCC == 0x31545844)
                {
                    pixelFormat = "BC1_UNORM";
                    if (m_textureAsset.PixelFormat == "BC1A_UNORM")
                        pixelFormat = "BC1A_UNORM";
                }

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
                    {
                        pixelFormat = "BC1_UNORM";
                        if (m_textureAsset.PixelFormat == "BC1A_UNORM")
                            pixelFormat = "BC1A_UNORM";
                    }
                    else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC3_UNorm)
                        pixelFormat = "BC3_UNORM";
                    else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC4_UNorm)
                        pixelFormat = "BC4_UNORM";
                    else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC5_UNorm)
                        pixelFormat = "BC5_UNORM";
                    else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.BC1_UNorm_SRgb && m_textureAsset.PixelFormat == "BC1A_SRGB")
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
                    else if (header.ExtendedHeader.dxgiFormat == SharpDX.DXGI.Format.R16_UNorm)
                    {
                        pixelFormat = "R16_UNORM";
                        if (m_textureAsset.PixelFormat == "D16_UNORM")
                            pixelFormat = "D16_UNORM";
                    }
                }
            }
        }

        public byte[] WriteToDDS()
        {
            TextureUtils.DDSHeader header = new TextureUtils.DDSHeader
            {
                dwHeight = m_textureAsset.Height,
                dwWidth = m_textureAsset.Width,
                dwPitchOrLinearSize = (int)m_textureAsset.MipSizes[0],
                dwMipMapCount = m_textureAsset.MipCount
            };

            if (m_textureAsset.MipCount > 1)
            {
                header.dwFlags |= TextureUtils.DDSFlags.MipMapCount;
                header.dwCaps |= TextureUtils.DDSCaps.MipMap | TextureUtils.DDSCaps.Complex;
            }

            switch (m_textureAsset.Type)
            {
                case TextureType.TT_2d:
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture2D;
                    header.ExtendedHeader.arraySize = 1;
                    break;
                case TextureType.TT_2dArray:
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture2D;
                    header.ExtendedHeader.arraySize = m_textureAsset.Depth;
                    break;
                case TextureType.TT_Cube:
                    header.dwCaps2 = TextureUtils.DDSCaps2.CubeMap | TextureUtils.DDSCaps2.CubeMapAllFaces;
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture2D;
                    header.ExtendedHeader.arraySize = 1;
                    header.ExtendedHeader.miscFlag = 4;
                    break;
                case TextureType.TT_3d:
                    header.dwFlags |= TextureUtils.DDSFlags.Depth;
                    header.dwCaps2 |= TextureUtils.DDSCaps2.Volume;
                    header.dwDepth = m_textureAsset.Depth;
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture3D;
                    header.ExtendedHeader.arraySize = 1;
                    break;
            }

            // handle DAI old legacy SRGB flag
            string pixelFormat = m_textureAsset.PixelFormat;
            if (pixelFormat.StartsWith("BC") && m_textureAsset.Flags.HasFlag(TextureFlags.SrgbGamma))
                pixelFormat = pixelFormat.Replace("UNORM", "SRGB");

            switch (pixelFormat)
            {
                //case "DXT1": header.ddspf.dwFourCC = 0x31545844; break;
                case "NormalDXT1": header.ddspf.dwFourCC = 0x31545844; break;
                case "NormalDXN": header.ddspf.dwFourCC = 0x32495441; break;
                //case "DXT1A": header.ddspf.dwFourCC = 0x31545844; break;
                case "BC1A_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC1_UNorm_SRgb; break;
                case "BC1A_UNORM": header.ddspf.dwFourCC = 0x31545844; break;
                case "BC1_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC1_UNorm_SRgb; break;
                case "BC1_UNORM": header.ddspf.dwFourCC = 0x31545844; break;
                //case "DXT3": header.ddspf.dwFourCC = 0x33545844; break;
                case "BC2_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC2_UNorm_SRgb; break;
                case "BC3_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC3_UNorm_SRgb; break;
                case "BC3A_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC3_UNorm_SRgb; break;
                case "BC3_UNORM": header.ddspf.dwFourCC = 0x35545844; break;
                case "BC3A_UNORM": header.ddspf.dwFourCC = 0x31495441; break;
                case "BC4_UNORM": header.ddspf.dwFourCC = 0x31495441; break;
                //case "DXT5": header.ddspf.dwFourCC = 0x35545844; break;
                //case "DXT5A": header.ddspf.dwFourCC = 0x31495441; break;
                case "BC5_UNORM": header.ddspf.dwFourCC = 0x32495441; break;
                case "BC6U_FLOAT": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC6H_Uf16; break;
                case "BC7": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC7_UNorm; break;
                case "BC7_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC7_UNorm_SRgb; break;
                case "BC7_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC7_UNorm; break;
                case "R8_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R8_UNorm; break;
                case "R16G16B16A16_FLOAT": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R16G16B16A16_Float; break;
                case "ARGB32F": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R32G32B32A32_Float; break;
                case "R32G32B32A32_FLOAT": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R32G32B32A32_Float; break;
                case "R9G9B9E5F": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R9G9B9E5_Sharedexp; break;
                case "R9G9B9E5_FLOAT": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R9G9B9E5_Sharedexp; break;
                case "R8G8B8A8_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm; break;
                case "R8G8B8A8_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb; break;
                case "R10G10B10A2_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R10G10B10A2_UNorm; break;
                case "L8": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R8_UNorm; break;
                case "L16": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R16_UNorm; break;
                case "ARGB8888": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm; break;
                case "D16_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.R16_UNorm; break;
                default: header.ddspf.dwFourCC = 0x00000000; break;
            }

            if (header.HasExtendedHeader)
            {
                header.ddspf.dwFourCC = 0x30315844;
            }

            MemoryStream srcStream = m_textureAsset.Data as MemoryStream;
            srcStream.Position = 0;

            byte[] buf = null;
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                header.Write(writer);

                if (m_textureAsset.Type == TextureType.TT_Cube || m_textureAsset.Type == TextureType.TT_2dArray)
                {
                    int sliceCount = 6;
                    if (m_textureAsset.Type == TextureType.TT_2dArray)
                        sliceCount = m_textureAsset.Depth;

                    // Need to rejig order of faces and mips
                    uint[] mipOffsets = new uint[m_textureAsset.MipCount];
                    for (int i = 0; i < m_textureAsset.MipCount - 1; i++)
                        mipOffsets[i + 1] = mipOffsets[i] + (uint)(m_textureAsset.MipSizes[i] * sliceCount);

                    byte[] tmpBuf = new byte[m_textureAsset.MipSizes[0]];

                    for (int slice = 0; slice < sliceCount; slice++)
                    {
                        for (int mip = 0; mip < m_textureAsset.MipCount; mip++)
                        {
                            int mipSize = (int)m_textureAsset.MipSizes[mip];

                            srcStream.Position = mipOffsets[mip] + (mipSize * slice);
                            srcStream.Read(tmpBuf, 0, mipSize);
                            writer.Write(tmpBuf, 0, mipSize);
                        }
                    }
                }
                else
                {
                    // 2d/3d/1d/etc.
                    writer.Write(srcStream.ToArray());
                }

                buf = ((MemoryStream)writer.BaseStream).GetBuffer();
            }

            return buf;
        }
    }
}
