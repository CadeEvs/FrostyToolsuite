using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core
{
    /// <summary>
    /// Describes the context in which the plugin manager is loading.
    /// </summary>
    public enum PluginManagerType
    {
        /// <summary>
        /// The plugin is loading into the editor.
        /// </summary>
        Editor,

        /// <summary>
        /// The plugin is loading into the mod manager.
        /// </summary>
        ModManager,

        /// <summary>
        /// The plugin is loading for both the editor and mod manager.
        /// </summary>
        Both
    }
}
