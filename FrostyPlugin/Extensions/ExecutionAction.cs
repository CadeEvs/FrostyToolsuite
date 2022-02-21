using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frosty.Core
{
    public abstract class ExecutionAction
    {
        public virtual Action<ILogger, PluginManagerType, CancellationToken> PreLaunchAction { get; }
        public virtual Action<ILogger, PluginManagerType, CancellationToken> PostLaunchAction { get; }

        public ExecutionAction()
        {
        }
    }
}
