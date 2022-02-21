using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System.Windows.Media;

namespace LegacyTextPlugin
{
    public class TextAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LegacyTextPlugin;component/Images/TextFileType.png") as ImageSource;
        public override ImageSource GetIcon(AssetEntry entry, double width, double height)
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new LegacyTextEditor(logger);
        }
    }
}
