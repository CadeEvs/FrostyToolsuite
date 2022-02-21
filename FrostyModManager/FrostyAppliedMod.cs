using Frosty.Core.Mod;

namespace FrostyModManager
{
    public class FrostyAppliedMod
    {
        public bool IsEnabled { get; set; }
        public FrostyMod Mod { get; }

        public FrostyAppliedMod(FrostyMod inMod, bool inIsEnabled = true)
        {
            Mod = inMod;
            IsEnabled = inIsEnabled;
        }
    }
}
