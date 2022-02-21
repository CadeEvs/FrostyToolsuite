using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System.Windows.Media;

namespace LegacyDatabasePlugin
{
    public class DatabaseAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LegacyDatabasePlugin;component/Images/DatabaseFileType.png") as ImageSource;
        public override ImageSource GetIcon(AssetEntry entry, double width, double height)
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new LegacyDbEditor(logger);
        }
    }
}
