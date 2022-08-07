using Frosty.Controls;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Editors;
using System.Windows.Media;

namespace LevelEditorPlugin.Definitions
{
    public class DetachedSubWorldDataAssetDefinition : BaseAssetDefinition
    {
        protected override ImageSource ImageIcon => Icons.DetachedSubWorldImageSource;
        protected override SvgImageData SvgIcon => Icons.DetachedSubWorldIcon;

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new LevelEditor(logger);
        }
    }
}
