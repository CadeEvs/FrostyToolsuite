using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute tells the plugin system to load this plugin for the specified profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class PluginValidForProfileAttribute : Attribute
    {
        /// <summary>
        /// Gets the profile name the plugin will be loaded for.
        /// </summary>
        /// <returns>The profile name.</returns>
        public int ProfileVersion { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginValidForProfileAttribute"/> class with the profile version.
        /// </summary>
        /// <param name="profileVersion">An int representing profile version.</param>
        public PluginValidForProfileAttribute(int profileVersion)
        {
            ProfileVersion = profileVersion;
        }
    }
}
