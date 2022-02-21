using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System.Windows.Media;

namespace IesResourcePlugin
{
    public class IesResourceAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/IesResourcePlugin;component/Images/IesResourceFileType.png") as ImageSource;
        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyIesResourceEditor(logger);
        }
    }
}
