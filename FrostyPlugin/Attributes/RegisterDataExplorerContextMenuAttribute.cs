using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a data explorer context menu item to the plugin system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class RegisterDataExplorerContextMenuAttribute : Attribute
    {
        /// <summary>
        /// Gets the type to use to construct the tab item.
        /// </summary>
        /// <returns>The type to use to construct the tab item</returns>
        public Type ContextMenuItemExtensionType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterDataExplorerContextMenuAttribute"/> class using the tab extension type.
        /// </summary>
        /// <param name="type">The type of the menu extension. This type must derive from <see cref="ContextMenu"/></param>
        public RegisterDataExplorerContextMenuAttribute(Type type)
        {
            ContextMenuItemExtensionType = type;
        }
    }
}
