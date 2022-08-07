using Frosty.Core;
using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Extensions
{
    public class DumpLayoutsToLogExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Developer";
        public override string MenuItemName => "Dump Layouts to Log";
        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            SchematicsLayoutManager.Instance.PrintLayouts(App.Logger);
        });
    }
}
