using Frosty.Core;

namespace FsLocalizationPlugin
{
    public class FsLocalizationMenuExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => "Localized Strings";
        public override string MenuItemName => "Add String";

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            AddStringWindow win = new AddStringWindow();
            win.ShowDialog();
        });
    }
}
