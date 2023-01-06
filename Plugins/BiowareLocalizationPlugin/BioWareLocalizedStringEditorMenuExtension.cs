using BiowareLocalizationPlugin.Controls;
using Frosty.Core;
using FrostySdk;

namespace BiowareLocalizationPlugin
{
    public class BioWareLocalizedStringEditorMenuExtension : MenuExtension
    {

        private const string ITEM_NAME = "Bioware Localized String Editor";

        public override string TopLevelMenuName => "View";
        
        public override string SubLevelMenuName => null;
        public override string MenuItemName => ITEM_NAME;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
            {
                App.Logger.Log("Not applicable for Anthem, sorry for the inconvenience!");
                return;
            }
            var textDb = (BiowareLocalizedStringDatabase) LocalizedStringDatabase.Current;
            App.EditorWindow.OpenEditor(ITEM_NAME, new BiowareLocalizedStringEditor(textDb));
        });
    }
}
