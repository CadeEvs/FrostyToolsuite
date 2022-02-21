using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a custom options item to the plugin system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class RegisterOptionsExtensionAttribute : Attribute
    {
        /// <summary>
        /// Gets the type to use to construct the custom options item.
        /// </summary>
        /// <returns>The type to use to construct the custom options item.</returns>
        public Type OptionsType { get; private set; }

        /// <summary>
        /// Gets the manager type to define if the options extension should be loaded for the Mod Manager or Editor.
        /// </summary>
        public PluginManagerType ManagerType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterOptionsExtensionAttribute"/> class using the options extension type.
        /// </summary>
        /// <param name="optionsType">The type of the option extension. This type must derive from <see cref="OptionsExtension"/></param>
        public RegisterOptionsExtensionAttribute(Type optionsType, PluginManagerType managerType = PluginManagerType.Editor)
        {
            OptionsType = optionsType;
            ManagerType = managerType;
        }
    }
}
