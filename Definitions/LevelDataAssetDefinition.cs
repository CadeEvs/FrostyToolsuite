using Frosty.Controls;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Editors;
using System.Windows.Media;

namespace LevelEditorPlugin.Definitions
{
    public class LevelDataAssetDefinition : BaseAssetDefinition
    {
        protected override ImageSource ImageIcon => Icons.LevelDataImageSource;
        protected override SvgImageData SvgIcon => Icons.LevelDataIcon;

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new LevelEditor(logger);
        }
    }
}
