using Frosty.Core;
using System.Windows.Media;

namespace DelayLoadBundlePlugin
{
    public class DelayLoadBundleMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/DelayLoadBundlePlugin;component/Images/DelayLoadBundle.png") as ImageSource;

        public override string TopLevelMenuName => "DAI";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "DelayLoadBundle Preview";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("DelayLoadBundle Preview", new DAIDelayLoadBundleEditor(App.Logger));
        });
    }
}
