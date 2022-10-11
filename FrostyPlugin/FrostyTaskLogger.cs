using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core
{
    internal class FrostyTaskLogger : ILogger
    {
        private FrostyTaskWindow task;

        public FrostyTaskLogger(FrostyTaskWindow inTask)
        {
            task = inTask;
        }

        public void Log(string text, params object[] vars)
        {
            if (text.StartsWith("progress:"))
            {
                text = text.Replace("progress:", "");
                task.Update(null, double.Parse(text));
            }
            else
            {
                task.Update(string.Format(text, vars));
            }
        }

        public void LogWarning(string text, params object[] vars)
        {
            throw new NotImplementedException();
        }

        public void LogError(string text, params object[] vars)
        {
            throw new NotImplementedException();
        }
    }
}
