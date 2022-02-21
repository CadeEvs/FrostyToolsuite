using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System.Windows.Media;

namespace SvgImagePlugin
{
    public class SvgImageAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/SvgImagePlugin;component/Images/SvgFileType.png") as ImageSource;
        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostySvgImageEditor(logger);
        }
    }
}
