using Frosty.Controls;
using System.Windows.Media;

namespace LevelEditorPlugin.Definitions
{
    public class MEPowerBlueprintAssetDefinition : BaseAssetDefinition
    {
        protected override ImageSource ImageIcon => Icons.PowerImageSource;
        protected override SvgImageData SvgIcon => Icons.PowerIcon;
    }

    public class LayerDataAssetDefinition : BaseAssetDefinition
    {
        protected override ImageSource ImageIcon => Icons.LayerDataImageSource;
        protected override SvgImageData SvgIcon => Icons.LayerDataIcon;
    }

    public class SubWorldDataAssetDefinition : BaseAssetDefinition
    {
        protected override ImageSource ImageIcon => Icons.SubWorldImageSource;
        protected override SvgImageData SvgIcon => Icons.SubWorldIcon;
    }
}
