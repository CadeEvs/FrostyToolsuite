using Frosty.Core.Attributes;
using System;
using System.IO;
using System.Reflection;

namespace Frosty.Core
{
    public class Plugin
    {
        // Entirely refactored this class to simply make it better overall

        /// <summary>
        /// The author of this <see cref="Plugin"/>.
        /// </summary>
        public string Author
        {
            get
            {
                // Create a string for storing the retrieved author of this plugin's associated assembly
                string author = Assembly?.GetCustomAttribute<PluginAuthorAttribute>()?.Author;

                // If the author is not null or empty, return it; otherwise, return unknown
                return !string.IsNullOrEmpty(author) ? author : "Unknown";
            }
        }

        /// <summary>
        /// The assembly associated with this <see cref="Plugin"/>.
        /// </summary>
        public Assembly Assembly
        {
            get;
            set;
        }

        /// <summary>
        /// An exception that may have occurred as a result of loading this <see cref="Plugin"/>.
        /// </summary>
        public Exception LoadException
        {
            get;
            set;
        }

        /// <summary>
        /// The display name of this <see cref="Plugin"/>.
        /// </summary>
        public string Name
        {
            get
            {
                // Create a string for storing the retrieved display name of this plugin
                string name = Assembly?.GetCustomAttribute<PluginDisplayNameAttribute>()?.DisplayName;

                // Return the display name or the file name without its extension
                return !string.IsNullOrEmpty(name) ? name : Path.GetFileNameWithoutExtension(SourcePath);
            }
        }

        /// <summary>
        /// The path at which this <see cref="Plugin"/> was located.
        /// </summary>
        public string SourcePath
        {
            get;
            private set;
        }

        /// <summary>
        /// The load status of this <see cref="Plugin"/>.
        /// </summary>
        public PluginLoadStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// The version of this <see cref="Plugin"/>.
        /// </summary>
        public string Version
        {
            get
            {
                // Create a string for storing the version of this plugin
                string version = Assembly?.GetCustomAttribute<PluginVersionAttribute>()?.Version;

                // Return the version if it is not null or empty; otherwise, return a default version of 1.0.0.0
                return !string.IsNullOrEmpty(version) ? version : "1.0.0.0";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to be used.</param>
        /// <param name="domain">The <see cref="AppDomain"/> that hosts this plugin. This is essential for the ability to </param>
        /// <param name="sourcePath">The path to the plugin's assembly.</param>
        public Plugin(Assembly assembly, string sourcePath)
        {
            // Assign to the assembly
            Assembly = assembly;

            // Assign to the source path
            SourcePath = sourcePath;
        }
    }
}
