using System;
using System.Windows.Controls;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a custom tab item to the plugin system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class RegisterTabExtensionAttribute : Attribute
    {
        /// <summary>
        /// Gets the type to use to construct the tab item.
        /// </summary>
        /// <returns>The type to use to construct the tab item</returns>
        public Type TabExtensionType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterTabExtensionAttribute"/> class using the tab extension type.
        /// </summary>
        /// <param name="type">The type of the menu extension. This type must derive from <see cref="MenuExtension"/></param>
        public RegisterTabExtensionAttribute(Type type)
        {
            TabExtensionType = type;
        }
    }
}
