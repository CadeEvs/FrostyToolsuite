using Frosty.Core;
using System.Windows.Media;

namespace BundleEditPlugin
{
    public class BundleEditorMenuExtension : MenuExtension
    {
        public static ImageSource iconImageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/BundleEditorPlugin;component/Images/BundleEdit.png") as ImageSource;

        public override string TopLevelMenuName => "View";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Bundle Editor";
        public override ImageSource Icon => iconImageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("Bundle Editor", new BundleEditor());
        });
    }
}
