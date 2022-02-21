using System.Windows.Media;

namespace Frosty.Core
{
    public abstract class DataExplorerContextMenuExtension
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataExplorerContextMenuExtension"/> class.
        /// </summary>
        public DataExplorerContextMenuExtension()
        {
        }

        /// <summary>
        /// When implemented in a derived class, gets the name of the tab item this extension will create.
        /// </summary>
        /// <returns>The name to use for the menu item.</returns>
        public virtual string ContextItemName { get; }

        /// <summary>
        /// When implemented in a derived class, gets the icon displayed for this context menu item.
        /// </summary>
        /// <returns>A <see cref="ImageSource"/> that represents the icon to display for the context menu item.</returns>
        public virtual ImageSource Icon { get; }

        public virtual RelayCommand ContextItemClicked { get; }

    }
}
