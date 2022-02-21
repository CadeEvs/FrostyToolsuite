using System.Collections.Generic;

namespace Frosty.Core.Bookmarks
{
    /// <summary>
    /// An entry in the user's bookmarks library.
    /// </summary>
    public class BookmarkItem
    {
        /// <summary>
        /// The destination target of this bookmark.
        /// </summary>
        public BookmarkTarget Target { get; }

        /// <summary>
        /// The user-assigned name of this particular item.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The readonly text shown on the bookmark.
        /// </summary>
        public string Text => Target?.Text ?? "";

        /// <summary>
        /// Returns the icon of this instance's <see cref="Target"/>.
        /// </summary>
        public System.Windows.Media.ImageSource Icon => Target?.Icon;

        /// <summary>
        /// The child bookmark items stored with this bookmark item.
        /// </summary>
        public List<BookmarkItem> Children { get; } = new List<BookmarkItem>();

        /// <summary>
        /// The bookmark item that this item is stored with.
        /// </summary>
        public BookmarkItem Parent = null;

        /// <summary>
        /// Whether the corresponding TreeViewItem is expanded.
        /// </summary>
        public bool IsExpanded { get; set; } = false;
        /// <summary>
        /// Whether the corresponding TreeViewItem is selected.
        /// </summary>
        public bool IsSelected { get; set; } = false;
        /// <summary>
        /// Whether the corresponding TreeViewItem is allowed to be displayed.
        /// </summary>
        public bool IsVisible { get; set; } = true;
        /// <summary>
        /// Whether this bookmark item has been excluded by the active filter.
        /// </summary>
        public bool IsFiltered { get; set; } = false;
        /// <summary>
        /// Whether the bookmark target is a folder.
        /// </summary>
        public bool IsFolder => (Target is FolderBookmarkTarget);

        /// <summary>
        /// Whether this bookmark item has been given a name by the user.
        /// </summary>
        public bool IsNamed => !string.IsNullOrEmpty(Name);

        /// <summary>
        /// Creates a new bokmark item with the specified target and parent.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        public BookmarkItem(BookmarkTarget target, BookmarkItem parent)
        {
            Target = target;
            Parent = parent;
        }
    }
}
