using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute defines the plugins display name that will be displayed when loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class PluginDisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the display name of the plugin.
        /// </summary>
        /// <returns>The display name.</returns>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDisplayNameAttribute"/> class with the display name of the plugin.
        /// </summary>
        /// <param name="displayName">A string containing the display name.</param>
        public PluginDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
