using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute defines the plugins version that will be displayed when loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class PluginVersionAttribute : Attribute
    {
        /// <summary>
        /// Gets the version of the plugin.
        /// </summary>
        /// <returns>The version.</returns>
        public string Version { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginVersionAttribute"/> class with the version of the plugin.
        /// </summary>
        /// <param name="version">A string containing the version.</param>
        public PluginVersionAttribute(string version)
        {
            Version = version;
        }
    }
}
