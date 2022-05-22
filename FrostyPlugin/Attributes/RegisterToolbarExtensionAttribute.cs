using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a custom toolbar item to the plugin system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class RegisterToolbarExtensionAttribute : Attribute
    {
        /// <summary>
        /// Gets the type to use to construct the toolbar item.
        /// </summary>
        /// <returns>The type to use to construct the toolbar item</returns>
        public Type ToolbarExtensionType { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterToolbarExtensionAttribute"/> class using the toolbar extension type.
        /// </summary>
        /// <param name="type">The type of the toolbar extension. This type must derive from <see cref="ToolbarExtension"/></param>
        public RegisterToolbarExtensionAttribute(Type type)
        {
            ToolbarExtensionType = type;
        }
    }
}