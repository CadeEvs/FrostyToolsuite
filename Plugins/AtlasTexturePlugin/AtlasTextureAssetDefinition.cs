using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Viewport;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace AtlasTexturePlugin
{
    public class AtlasTextureAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ImageFileType.png") as ImageSource;
        public override void GetSupportedExportTypes(List<AssetExportType> exportTypes)
        {
            exportTypes.Add(new AssetExportType("dds", "Direct Draw Surface"));

            base.GetSupportedExportTypes(exportTypes);
        }

        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override bool Export(EbxAssetEntry entry, string path, string filterType)
        {
            if (!base.Export(entry, path, filterType))
            {
                if (filterType == "png" || filterType == "tga" || filterType == "hdr" || filterType == "dds")
                {
                    EbxAsset asset = App.AssetManager.GetEbx(entry);
                    dynamic textureAsset = (dynamic)asset.RootObject;

                    ResAssetEntry resEntry = App.AssetManager.GetResEntry(textureAsset.Resource);
                    AtlasTexture texture = App.AssetManager.GetResAs<AtlasTexture>(resEntry);

                    TextureUtils.DDSHeader header = new TextureUtils.DDSHeader
                    {
                        dwHeight = texture.Height,
                        dwWidth = texture.Width,
                        dwMipMapCount = texture.MipCount,
                        dwPitchOrLinearSize = (int)texture.Data.Length,
                        ddspf = { dwFourCC = 0x35545844 }
                    };

                    texture.Data.Position = 0;
                    using (NativeWriter writer = new NativeWriter(new FileStream(path, FileMode.Create)))
                    {
                        header.Write(writer);

                        byte[] tmpBuf = new byte[texture.Data.Length];
                        texture.Data.Read(tmpBuf, 0, tmpBuf.Length);

                        writer.Write(tmpBuf);
                    }
                    return true;
                }
            }

            return false;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyAtlasTextureEditor(logger);
        }
    }
}
