using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute tells the plugin system not to load this plugin for the specified profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class PluginNotValidForProfileAttribute : Attribute
    {
        /// <summary>
        /// Gets the profile version that the plugin will not be loaded for.
        /// </summary>
        /// <returns>The profile version.</returns>
        public int ProfileVersion { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotValidForProfileAttribute"/> class with the profile version.
        /// </summary>
        /// <param name="profileVersion">An int representing profile version.</param>
        public PluginNotValidForProfileAttribute(int profileVersion)
        {
            ProfileVersion = profileVersion;
        }
    }
}
