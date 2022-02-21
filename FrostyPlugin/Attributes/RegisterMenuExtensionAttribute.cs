using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a custom menu item to the plugin system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class RegisterMenuExtensionAttribute : Attribute
    {
        /// <summary>
        /// Gets the type to use to construct the menu item.
        /// </summary>
        /// <returns>The type to use to construct the menu item</returns>
        public Type MenuExtensionType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterMenuExtensionAttribute"/> class using the menu extension type.
        /// </summary>
        /// <param name="type">The type of the menu extension. This type must derive from <see cref="MenuExtension"/></param>
        public RegisterMenuExtensionAttribute(Type type)
        {
            MenuExtensionType = type;
        }
    }
}
