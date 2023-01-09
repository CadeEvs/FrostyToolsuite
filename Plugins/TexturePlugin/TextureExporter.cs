using Frosty.Core.Viewport;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.IO;
using D3D11 = SharpDX.Direct3D11;

namespace TexturePlugin
{
    [EbxClassMeta(EbxFieldType.Struct)]
    public class FrostyTextureSettingsItem
    {
        [IsReadOnly]
        public CString Filename { get; set; }
    }
    public enum FrostyTextureCubeFace
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ,
    }
    [IsExpandedByDefault]
    [DisplayName("Texture Cube Face")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class FrostyTextureCubeItem : FrostyTextureSettingsItem
    {
        public FrostyTextureCubeFace Face { get; set; }
        public override string ToString()
        {
            string faceString = "";
            switch (Face)
            {
                case FrostyTextureCubeFace.PositiveX: faceString = "X+"; break;
                case FrostyTextureCubeFace.NegativeX: faceString = "X-"; break;
                case FrostyTextureCubeFace.PositiveY: faceString = "Y+"; break;
                case FrostyTextureCubeFace.NegativeY: faceString = "Y-"; break;
                case FrostyTextureCubeFace.PositiveZ: faceString = "Z+"; break;
                case FrostyTextureCubeFace.NegativeZ: faceString = "Z-"; break;
            }
            return "Cube Face (" + faceString + "): " + Filename;
        }
    }
    [IsExpandedByDefault]
    [DisplayName("Texture Array Slice")]
    [EbxClassMeta(EbxFieldType.Struct)]
    public class FrostyTextureArrayItem : FrostyTextureSettingsItem
    {
        public int Slice { get; set; }
        public override string ToString()
        {
            return "Array Slice (" + Slice + "): " + Filename;
        }
    }

    public class FrostyTextureImportSettings
    {
        [FixedSizeArray]
        [IsExpandedByDefault]
        [EbxFieldMeta(EbxFieldType.Array, arrayType: EbxFieldType.Struct)]
        public List<FrostyTextureSettingsItem> Textures { get; set; } = new List<FrostyTextureSettingsItem>();
    }

    public class TextureExporter
    {
        public void Export(Texture textureAsset, string filename, string filterType)
        {
            byte[] ddsData = WriteToDDS(textureAsset);

            if (filterType == "*.dds")
            {
                // just save out as plain old DDS
                using (NativeWriter writer = new NativeWriter(new FileStream(filename, FileMode.Create)))
                    writer.Write(ddsData);
            }
            else
            {
                ImageFormat format = ImageFormat.DDS;
                if (filterType == "*.png") format = ImageFormat.PNG;
                else if (filterType == "*.tga") format = ImageFormat.TGA;
                else if (filterType == "*.hdr") format = ImageFormat.HDR;

                // convert to relevant image type
                if (textureAsset.Type == TextureType.TT_2d)
                {
                    using (NativeWriter writer = new NativeWriter(new FileStream(filename, FileMode.Create)))
                    {
                        BlobData blob = new BlobData();
                        FrostyTextureEditor.ConvertDDSToImage(ddsData, ddsData.Length, format, ref blob);
                        writer.Write(blob.Data);
                        FrostyTextureEditor.ReleaseBlob(blob);
                    }
                }
                else
                {
                    int numSlices = 0;
                    string[] filenames = null;

                    if (textureAsset.Type == TextureType.TT_Cube)
                    {
                        numSlices = 6;
                        filenames = new string[6] { "px", "nx", "py", "ny", "pz", "nz" };

                        FileInfo fi = new FileInfo(filename);
                        for (int i = 0; i < numSlices; i++)
                            filenames[i] = $"{fi.FullName.Replace(fi.Extension, "")}_{filenames[i]}{fi.Extension}";
                    }
                    else
                    {
                        numSlices = textureAsset.SliceCount;
                        filenames = new string[numSlices];

                        FileInfo fi = new FileInfo(filename);
                        for (int i = 0; i < numSlices; i++)
                            filenames[i] = $"{fi.FullName.Replace(fi.Extension, "")}_{i:D3}{fi.Extension}";
                    }

                    BlobData[] blobs = new BlobData[numSlices];
                    FrostyTextureEditor.ConvertDDSToImages(ddsData, ddsData.Length, format, ref blobs, numSlices);

                    for (int i = 0; i < numSlices; i++)
                    {
                        using (NativeWriter writer = new NativeWriter(new FileStream(filenames[i], FileMode.Create)))
                        {
                            writer.Write(blobs[i].Data);
                            FrostyTextureEditor.ReleaseBlob(blobs[i]);
                        }
                    }
                }
            }
        }

        private byte[] WriteToDDS(Texture textureAsset)
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
                case "BC1_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC1_UNorm; break;
                //case "DXT3": header.ddspf.dwFourCC = 0x33545844; break;
                case "BC2_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC2_UNorm_SRgb; break;
                case "BC3_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC3_UNorm_SRgb; break;
                case "BC3A_SRGB": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC3_UNorm_SRgb; break;
                case "BC3_UNORM": header.ddspf.dwFourCC = 0x35545844; break;
                case "BC3A_UNORM": header.ddspf.dwFourCC = 0x31495441; break;
                case "BC4_UNORM": header.HasExtendedHeader = true; header.ExtendedHeader.dxgiFormat = SharpDX.DXGI.Format.BC4_UNorm; break;
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
}
