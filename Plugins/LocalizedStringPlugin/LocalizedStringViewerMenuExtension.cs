using Frosty.Core;
using System.Windows.Media;

namespace LocalizedStringPlugin
{
    public class LocalizedStringViewerMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/LocalizedStringPlugin;component/Images/LocalizedStringViewer.png") as ImageSource;

        public override string TopLevelMenuName => "View";
        public override string SubLevelMenuName => null;
        public override string MenuItemName => "Localized String Explorer";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("Localized String Viewer", new FrostyLocalizedStringViewer(App.Logger));
        });
    }
}
