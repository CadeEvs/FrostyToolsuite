using Frosty.Core;
using Frosty.Core.Viewport;
using Frosty.Core.Windows;
using System.Windows.Media;

namespace RootInstanceEntiresPlugin.MenuExtensions
{
    public class RefreshRootInstanceEntriesMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/RefreshMeshVariationsPlugin;component/Images/Refresh.png") as ImageSource;

        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Refresh Root Instance Ebx Entries";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            FrostyTaskWindow.Show("Refreshing Root Instance Ebx Entries", "", (task) =>
            {
                RootInstanceEbxEntryDb.LoadEbxRootInstanceEntries(task);
            });
        });
    }
}
