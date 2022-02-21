using Frosty.Core;
using System.Windows.Media;

namespace LegacyKitPreviewPlugin
{
    public class LegacyKitPreviewMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LegacyKitPreviewPlugin;component/Images/KitPreview.png") as ImageSource;

        public override string TopLevelMenuName => "Fifa";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Kit Previewer";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("Kit Previewer", new LegacyKitPreviewEditor(App.Logger));
        });
    }
}
