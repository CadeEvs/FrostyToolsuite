using Frosty.Core;
using TestPlugin.Controls;

namespace TestPlugin.Extensions
{
    // This is an example of a menu extension. It will build a new menu item under the Tools
    // menu called 'InitFS Viewer'. When clicked it will open a new InitFS viewer editor.

    public class InitFsMenuExtension : MenuExtension
    {
        // The top level menu item to place this menu item into. In this case 'Tools'
        public override string TopLevelMenuName => "View";

        // The name of the menu item. In this case 'InitFS Explorer'
        public override string MenuItemName => "InitFS Explorer";

        // The action to perform when the menu item is clicked
        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            // Open a new editor
            App.EditorWindow.OpenEditor("InitFS Viewer", new InitFSViewer());
        });
    }
}
