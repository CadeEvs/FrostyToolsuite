using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core
{
    public abstract class StartupAction
    {
        public StartupAction()
        {
        }

        public virtual Action<ILogger> Action { get; }
    }
}
