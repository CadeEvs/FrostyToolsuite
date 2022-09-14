using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace TexturePlugin
{
    public class TextureAssetDefinition : AssetDefinition
    {
        private static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ImageFileType.png") as ImageSource;

        public TextureAssetDefinition()
        {
        }

        public override void GetSupportedExportTypes(List<AssetExportType> exportTypes)
        {
            exportTypes.Add(new AssetExportType("png", "Portable Network Graphics"));
            exportTypes.Add(new AssetExportType("tga", "Truevision TGA"));
            exportTypes.Add(new AssetExportType("hdr", "High Dynamic Range"));
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
                    Texture texture = App.AssetManager.GetResAs<Texture>(resEntry);

                    TextureExporter exporter = new TextureExporter();
                    exporter.Export(texture, path, "*." + filterType);
                    return true;
                }
            }

            return false;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyTextureEditor(logger);
        }
    }
}
