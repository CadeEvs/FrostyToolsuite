using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System.Windows.Media;

namespace ObjectVariationPlugin
{
    public class ObjectVariationAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/ObjectVariationPlugin;component/Images/ObjectVariationFileType.png") as ImageSource;
        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyObjectVariationEditor(logger);
        }
    }
}
