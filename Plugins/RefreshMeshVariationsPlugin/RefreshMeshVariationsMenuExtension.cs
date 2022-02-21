using Frosty.Core;
using Frosty.Core.Viewport;
using Frosty.Core.Windows;
using System.Windows.Media;

namespace RefreshMeshVariationsPlugin
{
    public class RefreshMeshVariationsMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/RefreshMeshVariationsPlugin;component/Images/Refresh.png") as ImageSource;

        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Refresh MeshVariations";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            FrostyTaskWindow.Show("Refreshing Variations", "", (task) =>
            {
                MeshVariationDb.LoadVariations(task);
            });
        });
    }
}
