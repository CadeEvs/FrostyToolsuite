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
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void AddMod(ISuperGamerLeagueGamer mod, bool isEnabled = true, string backupFileName = "")
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
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void RemoveMod(FrostyAppliedMod mod)
        {
            AppliedMods.Remove(mod);
            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModUp(FrostyAppliedMod mod)
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

            if (index <= 0)
                return;

            AppliedMods.RemoveAt(index);
            AppliedMods.Insert(--index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModTop(FrostyAppliedMod mod)
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

            AppliedMods.RemoveAt(index);
            index = 0;
            AppliedMods.Insert(index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModDown(FrostyAppliedMod mod)
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

            if (index >= AppliedMods.Count - 1)
                return;

            AppliedMods.RemoveAt(index);
            AppliedMods.Insert(++index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModBottom(FrostyAppliedMod mod)
        {
            int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

            if (index >= AppliedMods.Count)
                return;

            AppliedMods.RemoveAt(index);
            index = AppliedMods.Count;
            AppliedMods.Insert(index, mod);

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModsUp(dynamic mods)
        {
            List<int> indices = new List<int>(mods.Count);
            foreach (FrostyAppliedMod mod in mods)
            {
                int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

                if (index <= 0)
                    return;

                indices.Add(index);
            }
            indices.Sort();

            for (int i = 0; i < indices.Count; i++)
            {
                FrostyAppliedMod mod = AppliedMods[indices[i]];
                AppliedMods.RemoveAt(indices[i]);
                AppliedMods.Insert(indices[i] - 1, mod);
            }

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModsTop(dynamic mods)
        {
            List<int> indices = new List<int>(mods.Count);
            foreach (FrostyAppliedMod mod in mods)
            {
                int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

                if (index <= 0)
                    return;

                indices.Add(index);
            }
            indices.Sort();

            for (int i = 0; i < indices.Count; i++)
            {
                FrostyAppliedMod mod = AppliedMods[indices[i]];
                AppliedMods.RemoveAt(indices[i]);
                AppliedMods.Insert(i, mod);
            }

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModsDown(dynamic mods)
        {
            List<int> indices = new List<int>(mods.Count);
            foreach (FrostyAppliedMod mod in mods)
            {
                int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

                if (index >= AppliedMods.Count - 1)
                    return;

                indices.Add(index);
            }
            indices.Sort();

            for (int i = indices.Count - 1; i >= 0; i--)
            {
                FrostyAppliedMod mod = AppliedMods[indices[i]];
                AppliedMods.RemoveAt(indices[i]);
                AppliedMods.Insert(indices[i] + 1, mod);
            }

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
            AppliedModsUpdated?.Invoke(this, new RoutedEventArgs());
        }

        public void MoveModsBottom(dynamic mods)
        {
            List<int> indices = new List<int>(mods.Count);
            foreach (FrostyAppliedMod mod in mods)
            {
                int index = AppliedMods.FindIndex((FrostyAppliedMod a) => a == mod);

                if (index >= AppliedMods.Count - 1)
                    return;

                indices.Add(index);
            }
            indices.Sort();

            for (int i = indices.Count - 1; i >= 0; i--)
            {
                FrostyAppliedMod mod = AppliedMods[indices[i]];
                AppliedMods.RemoveAt(indices[i]);
                AppliedMods.Insert(AppliedMods.Count, mod);
            }

            Config.Add(Name, ToConfigString(), ConfigScope.Pack);
            //Config.Add("Profiles", Name, ToConfigString());
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