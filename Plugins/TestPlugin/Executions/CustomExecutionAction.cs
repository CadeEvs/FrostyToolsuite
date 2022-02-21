using Frosty.Core;
using Frosty.Core.Mod;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPlugin.EditorExecutions
{
    public class CustomExecutionAction : ExecutionAction
    {
        public override Action<ILogger, PluginManagerType, CancellationToken> PreLaunchAction => new Action<ILogger, PluginManagerType, CancellationToken>((ILogger logger, PluginManagerType type, CancellationToken token) =>
        {
            Console.WriteLine($"{type}: PreLaunch Action");
        });

        public override Action<ILogger, PluginManagerType, CancellationToken> PostLaunchAction => new Action<ILogger, PluginManagerType, CancellationToken>((ILogger logger, PluginManagerType type, CancellationToken token) =>
        {
            Console.WriteLine($"{type}: PostLaunch Action");
        });
    }
}
