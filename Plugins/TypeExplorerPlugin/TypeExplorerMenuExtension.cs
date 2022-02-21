using Frosty.Core;
using System.Windows.Media;

namespace TypeExplorerPlugin
{
    public class TypeExplorerMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/TypeExplorerPlugin;component/Images/TypeExplorer.png") as ImageSource;

        public override string TopLevelMenuName => "View";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Type Explorer";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("Type Explorer", new FrostyTypeExplorer(App.Logger));
        });
    }
}
