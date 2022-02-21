using System.Windows.Media;

namespace Frosty.Core.Bookmarks
{
    public class FolderBookmarkTarget : BookmarkTarget
    {
        private static readonly ImageSource FolderSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/OpenFolder.png") as ImageSource;

        public override string Text => "";

        public override ImageSource Icon => FolderSource;

        public override void LoadData(string serializedData)
        {
            // Do nothing
        }

        public override void NavigateTo(bool bOpen)
        {
            // Do nothing
        }

        protected override string SerializeData()
        {
            return "";
        }
    }
}
