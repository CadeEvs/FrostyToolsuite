using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute defines the plugins author that will be displayed when loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class PluginAuthorAttribute : Attribute
    {
        /// <summary>
        /// Gets the author of the plugin.
        /// </summary>
        /// <returns>The author.</returns>
        public string Author { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginAuthorAttribute"/> class with the author of the plugin.
        /// </summary>
        /// <param name="author">A string containing the author.</param>
        public PluginAuthorAttribute(string author)
        {
            Author = author;
        }
    }
}
