using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core
{
    /// <summary>
    /// Describes when the plugin should be loaded.
    /// </summary>
    public enum PluginLoadType
    {
        /// <summary>
        /// The plugin loads on app startup.
        /// </summary>
        Startup,
        /// <summary>
        /// The plugin loads during the profile loading process.
        /// </summary>
        Initialize
    }
}
