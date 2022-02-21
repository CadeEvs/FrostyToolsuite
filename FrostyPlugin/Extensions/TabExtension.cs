using Frosty.Controls;
using System.Windows.Controls;
using System.Windows.Media;

namespace Frosty.Core
{
    /// <summary>
    /// Describes an extension to the tab system. Classes that derive from this class can
    /// specify the name, icon, and content of the tab item.
    /// </summary>
    public abstract class TabExtension
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TabExtension"/> class.
        /// </summary>
        public TabExtension()
        {
        }

        /// <summary>
        /// When implemented in a derived class, gets the name of the tab item this extension will create.
        /// </summary>
        /// <returns>The name to use for the menu item.</returns>
        public virtual string TabItemName { get; }

        /// <summary>
        /// When implemented in a derived class, gets the icon of the tab item this extension will create.
        /// </summary>
        /// <returns>The name to use for the menu item.</returns>
        public virtual ImageSource TabItemIcon { get; }

        /// <summary>
        /// When implemented in a derived clas, gets the content of the tab item this extension will create.
        /// </summary>
        /// <returns>A <see cref="FrostyTabItem" that represents the content to display for the tab item./></returns>
        public virtual FrostyTabItem TabContent { get; }
    }
}
