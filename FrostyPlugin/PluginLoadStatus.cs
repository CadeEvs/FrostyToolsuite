using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core
{
    public enum PluginLoadStatus
    {
        /// <summary>
        /// An enum entry that indicates that the associated plugin has failed to load.
        /// </summary>
        Failed,

        /// <summary>
        /// An enum entry that indicates that the associated plugin has successfully loaded.
        /// </summary>
        Loaded,

        /// <summary>
        /// An enum entry that indicates that the associated plugin has successfully loaded, but is invalid for the currently loaded profile.
        /// </summary>
        LoadedInvalid
    }
}
