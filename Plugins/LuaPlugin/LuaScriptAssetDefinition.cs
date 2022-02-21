using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System.Windows.Media;

namespace LuaPlugin
{
    public class LuaScriptAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LuaPlugin;component/Images/LuaFileType.png") as ImageSource;
        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyLuaEditor(logger);
        }
    }
}
