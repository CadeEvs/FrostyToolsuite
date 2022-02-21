using System.Windows.Media;

namespace Frosty.Core
{
    /// <summary>
    /// Describes an extension to the menu system. Classes that derive from this class can 
    /// specify the location of the menu item to construct, along with its name, icon and action
    /// to perform when clicked.
    /// </summary>
    public abstract class MenuExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuExtension"/> class.
        /// </summary>
        public MenuExtension()
        {
        }

        /// <summary>
        /// When implemented in a derived class, gets the name of the top level menu item this menu item will be placed.
        /// </summary>
        /// <returns>The name of the top level menu item to place the menu item into.</returns>
        public virtual string TopLevelMenuName { get; }

        /// <summary>
        /// When implemented in a derived class, gets the name of the child menu item (of the top level menu item) this menu item will be placed.
        /// </summary>
        /// <returns>The name of the child menu item to place the menu item into.</returns>
        public virtual string SubLevelMenuName { get; }

        /// <summary>
        /// When implemented in a derived class, gets the name of the menu item this extension will create.
        /// </summary>
        /// <returns>The name to use for the menu item.</returns>
        public virtual string MenuItemName { get; }

        /// <summary>
        /// When implemented in a derived class, gets the icon displayed for this menu item.
        /// </summary>
        /// <returns>A <see cref="ImageSource"/> that represents the icon to display for the menu item.</returns>
        public virtual ImageSource Icon { get; }

        /// <summary>
        /// When implemented in a derived class, gets the action to perform when this menu item is clicked.
        /// </summary>
        /// <returns>The action to perform when the menu item is clicked.</returns>
        public virtual RelayCommand MenuItemClicked { get; }
    }
}
