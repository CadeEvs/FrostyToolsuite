using AnimationEditorPlugin.Editors;
using Frosty.Core;

namespace AnimationEditorPlugin.Extensions
{
    public class AnimationEditorMenuExtension : MenuExtension
    {
        public override string TopLevelMenuName => "View";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Animation Editor";
        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("Animation Editor", new AnimationEditor(App.Logger));
        });
    }
}