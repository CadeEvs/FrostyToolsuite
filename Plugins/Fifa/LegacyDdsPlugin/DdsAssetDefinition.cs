using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace LegacyDdsPlugin
{
    public class DdsAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LegacyDdsPlugin;component/Images/ImageFileType.png") as ImageSource;
        public override ImageSource GetIcon(AssetEntry entry, double width, double height)
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new LegacyTextureEditor(logger);
        }
    }
}
