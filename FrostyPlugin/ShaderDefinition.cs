using Frosty.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core
{
    /// <summary>
    /// Represents a shader loaded from a plugin
    /// </summary>
    public sealed class ShaderDefinition
    {
        /// <summary>
        /// Gets the plugin in which the shader is located.
        /// </summary>
        /// <returns>The <see cref="Assembly"/> that represents the plugin.</returns>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the name of the shader resource file in the format of Namespace.ResourceName.
        /// </summary>
        /// <returns>The name of the shader resource file.</returns>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the type of shader.
        /// </summary>
        /// <returns>The type of shader.</returns>
        public ShaderType ShaderType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderDefinition"/> class with the plugin, shader type, and resource name.
        /// </summary>
        /// <param name="plugin">The plugin in which the shader is located.</param>
        /// <param name="type">The type of shader.</param>
        /// <param name="resourceName">The resource name of the shader in the format of Namespace.ResourceName.</param>
        public ShaderDefinition(Assembly plugin, ShaderType type, string resourceName)
        {
            Assembly = plugin;
            ShaderType = type;
            ResourceName = resourceName;
        }
    }
}
