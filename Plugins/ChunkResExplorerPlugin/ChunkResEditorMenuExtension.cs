using Frosty.Core;
using System.Windows.Media;

namespace ChunkResEditorPlugin
{
    public class ChunkResEditorMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/ChunkResEditorPlugin;component/Images/ChunkResEditor.png") as ImageSource;

        public override string TopLevelMenuName => "View";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Chunk/Res Editor";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            App.EditorWindow.OpenEditor("Chunk/Res Editor", new FrostyChunkResEditor(App.Logger));
        });
    }
}
