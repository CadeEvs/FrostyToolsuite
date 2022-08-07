using Frosty.Core;
using Frosty.Core.Viewport;
using FrostySdk.IO;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using D3D11 = SharpDX.Direct3D11;

namespace LevelEditorPlugin.Assets
{
    public class TextureCreator
    {
        private struct BlobData
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

        private enum ImageFormat
        {
            PNG,
            TGA,
            HDR,
            DDS
        }

        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ConvertDDSToImage")]
        private static extern void ConvertDDSToImage(byte[] pData, long iDataSize, ImageFormat format, ref BlobData pOutData);
        [DllImport("thirdparty/dxtex.dll", EntryPoint = "ReleaseBlob")]
        private static extern void ReleaseBlob(BlobData pData);

        public static BitmapImage CreatePNG(Texture textureAsset)
        {
            byte[] ddsData = WriteToDDS(textureAsset);

            // convert to relevant image type
            if (textureAsset.Type == TextureType.TT_2d)
            {
                byte[] pngData = null;
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    BlobData blob = new BlobData();
                    ConvertDDSToImage(ddsData, ddsData.Length, ImageFormat.PNG, ref blob);
                    writer.Write(blob.Data);
                    ReleaseBlob(blob);

                    pngData = writer.ToByteArray();
                }

                return Convert(new Bitmap(new MemoryStream(pngData)));
            }

            return null;
        }

        private static BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private static byte[] WriteToDDS(Texture textureAsset)
        {
            TextureUtils.DDSHeader header = new TextureUtils.DDSHeader
            {
                dwHeight = textureAsset.Height,
                dwWidth = textureAsset.Width,
                dwPitchOrLinearSize = (int)textureAsset.MipSizes[0],
                dwMipMapCount = textureAsset.MipCount
            };

            if (textureAsset.MipCount > 1)
            {
                header.dwFlags |= TextureUtils.DDSFlags.MipMapCount;
                header.dwCaps |= TextureUtils.DDSCaps.MipMap | TextureUtils.DDSCaps.Complex;
            }

            switch (textureAsset.Type)
            {
                case TextureType.TT_2d:
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture2D;
                    header.ExtendedHeader.arraySize = 1;
                    break;
                case TextureType.TT_2dArray:
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture2D;
                    header.ExtendedHeader.arraySize = textureAsset.Depth;
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
                    header.dwDepth = textureAsset.Depth;
                    header.ExtendedHeader.resourceDimension = D3D11.ResourceDimension.Texture3D;
                    header.ExtendedHeader.arraySize = 1;
                    break;
            }

            // handle DAI old legacy SRGB flag
            string pixelFormat = textureAsset.PixelFormat;
            if (pixelFormat.StartsWith("BC") && textureAsset.Flags.HasFlag(TextureFlags.SrgbGamma))
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
                default: header.ddspf.dwFourCC = 0x00000000; break;
            }

            if (header.HasExtendedHeader)
            {
                header.ddspf.dwFourCC = 0x30315844;
            }

            MemoryStream srcStream = textureAsset.Data as MemoryStream;
            srcStream.Position = 0;

            byte[] buf = null;
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                header.Write(writer);

                if (textureAsset.Type == TextureType.TT_Cube || textureAsset.Type == TextureType.TT_2dArray)
                {
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

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.TextureBaseAsset))]
    public class TextureAsset : Asset, IAssetData<FrostySdk.Ebx.TextureBaseAsset>
    {
        public FrostySdk.Ebx.TextureBaseAsset Data => data as FrostySdk.Ebx.TextureBaseAsset;
        public BitmapImage Texture
        {
            get
            {
                if (bitmapImage == null)
                {
                    var textureRes = App.AssetManager.GetResAs<Texture>(App.AssetManager.GetResEntry(Data.Resource));
                    Application.Current.Dispatcher.Invoke(() => { bitmapImage = TextureCreator.CreatePNG(textureRes); });
                }
                return bitmapImage;
            }
        }

        protected BitmapImage bitmapImage;

        public TextureAsset(Guid fileGuid, FrostySdk.Ebx.TextureBaseAsset inData)
            : base(fileGuid, inData)
        {
        }
    }
}
