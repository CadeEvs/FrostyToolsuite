using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System.Windows.Media;

namespace LegacyBigFilePlugin
{
    public class BigFileAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LegacyBigFilePlugin;component/Images/ArchiveFileType.png") as ImageSource;

        public override ImageSource GetIcon(AssetEntry entry, double width, double height)
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new LegacyBigFileEditor(logger);
        }
    }
}
