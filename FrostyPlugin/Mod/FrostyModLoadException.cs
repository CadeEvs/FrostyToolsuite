using System;

namespace Frosty.Core.Mod
{
    public sealed class FrostyModLoadException : Exception
    {
        public FrostyModLoadException(string message)
            : base(message)
        {
        }
    }
}
