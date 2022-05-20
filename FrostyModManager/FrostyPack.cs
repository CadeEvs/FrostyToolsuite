using Frosty.Core;
using Frosty.Core.Mod;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace FrostyModManager
{
    public class FrostyPack
    {
        public string Name { get; }
        public List<FrostyAppliedMod> AppliedMods { get; } = new List<FrostyAppliedMod>();
        public event RoutedEventHandler AppliedModsUpdated;

        public FrostyPack(string inName)
        {
            Name = inName;
        }

        public void Refresh()
        {
            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void AddMod(FrostyMod mod, bool isEnabled = true, string backupFileName = "")
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a.Mod != null ? a.Mod == mod : a.ModName == backupFileName);
            if (index != -1)
                return;

            // if mod is null and couldn't be found, create a FrostyAppliedMod with IsFound to false
            if (mod != null)
                AppliedMods.Add(new FrostyAppliedMod(mod, isEnabled));
            else
                AppliedMods.Add(new FrostyAppliedMod(backupFileName, isEnabled));

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void RemoveMod(FrostyAppliedMod mod)
        {
            AppliedMods.Remove(mod);
            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModUp(FrostyAppliedMod mod)
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);
            index--;

            if (index < 0)
                return;

            AppliedMods.RemoveAt(index + 1);
            AppliedMods.Insert(index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModTop(FrostyAppliedMod mod) {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);
            
            AppliedMods.RemoveAt(index);
            index = 0;
            AppliedMods.Insert(index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModDown(FrostyAppliedMod mod)
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);
            index++;

            if (index >= AppliedMods.Count)
                return;

            AppliedMods.RemoveAt(index - 1);
            AppliedMods.Insert(index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModBottom(FrostyAppliedMod mod) {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

            if (index >= AppliedMods.Count)
                return;

            AppliedMods.RemoveAt(index);
            index = AppliedMods.Count;
            AppliedMods.Insert(index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        private string ToConfigString()
        {
            StringBuilder sb = new StringBuilder();
            if (AppliedMods.Count > 0)
            {
                foreach (FrostyAppliedMod mod in AppliedMods)
                {
                    // append backup file name is mod isn't found
                    if (mod.Mod != null)
                        sb.Append(mod.Mod.Filename + ":" + mod.IsEnabled + "|");
                    else
                        sb.Append(mod.BackupFileName + ":" + mod.IsEnabled + "|");
                }
                    
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
    }
}
