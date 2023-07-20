using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Frosty.Sdk;

namespace FrostyEditor.Utils;

public enum ConfigScope
{
    Global,
    Game,
    Pack
}

// @todo: allow overrides of any option within game specific options (will be allowed via manual edits if people really want to)
public static class Config
{
    private class InternalConfig
    {
        public Dictionary<string, GameOptions> Games { get; set; } = new();
        public Dictionary<string, object> GlobalOptions { get; set; } = new();

        /// <summary>
        /// Determines whether the <see cref="Config"/> contains the specified key.
        /// </summary>
        /// <param name="option">The option key to locate in the <see cref="Config"/>.</param>
        /// <param name="scope"></param>
        /// <param name="profile"></param>
        public bool Contains(string option, ConfigScope scope = ConfigScope.Global, string? profile = null)
        {
            if (scope == ConfigScope.Global)
            {
                return GlobalOptions.ContainsKey(option);
            }

            if (Games.TryGetValue(profile ?? ProfilesLibrary.ProfileName, out GameOptions? gameOptions))
            {
                return gameOptions.Contains(option, scope);
            }

            return false;
        }
        
        // indexer
        public object? this[string option, ConfigScope scope = ConfigScope.Global, string? profile = null]
        {
            get
            {
                if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
                {
                    if (Games.TryGetValue(profile ?? ProfilesLibrary.ProfileName, out GameOptions? game))
                    {
                        return game[option, scope];
                    }
                    return null;
                }

                return GlobalOptions.TryGetValue(option, out object? globalOption) ? globalOption : null;
            }
            set
            {
                if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
                {
                    if (Games.TryGetValue(profile ?? ProfilesLibrary.ProfileName, out GameOptions? game))
                    {
                        game[option, scope] = value;
                    }
                }
                else
                {
                    GlobalOptions[option] = value!;
                }
            }
        }

        public bool TryGetValue(string option, ConfigScope scope, string profileName, out object? value)
        {
            if (!Contains(option, scope, profileName))
            {
                value = null;
                return false;
            }

            value = this[option, scope, profileName];
            return true;
        }
    }

    /// <summary>
    /// Game specific options object
    /// </summary>
    private class GameOptions
    {
        public string GamePath { get; set; }
        public string BookmarkDb { get; set; } = "[Asset Bookmarks]|[Legacy Bookmarks]";

        public Dictionary<string, object> Options { get; set; } = new();
        public Dictionary<string, string> Packs { get; set; } = new();

        public GameOptions(string gamePath)
        {
            GamePath = gamePath;
        }

        internal bool Contains(string option, ConfigScope scope = ConfigScope.Game) => scope == ConfigScope.Game ? Options.ContainsKey(option) || option == "GamePath" || option == "BookmarkDb" : Packs.ContainsKey(option);

        internal void Add(string option, object value) => this[option] = value;

        internal void Remove(string option, ConfigScope scope = ConfigScope.Game)
        {
            if (scope == ConfigScope.Game)
            {
                Options.Remove(option);
            }
            else
            {
                Packs.Remove(option);
            }
        }

        internal void Rename(string option, string newoption, ConfigScope scope = ConfigScope.Game)
        {
            if (scope == ConfigScope.Pack && Packs.TryGetValue(option, out string? value))
            {
                Packs.Add(newoption, value);
                Packs.Remove(option);
            }
        }

        internal IEnumerable<string> EnumerateKeys(ConfigScope scope = ConfigScope.Game)
        {
            if (scope == ConfigScope.Game)
            {
                foreach (string key in Options.Keys)
                {
                    yield return key;
                }
            }
            else
            {
                foreach (string key in Packs.Keys)
                {
                    yield return key;
                }
            }
        }

        // indexer
        public object? this[string option, ConfigScope scope = ConfigScope.Game]
        {
            get
            {
                if (scope == ConfigScope.Pack)
                {
                    return Packs.TryGetValue(option, out string? pack) ? pack : null;
                }

                if (option == "GamePath")
                {
                    return GamePath;
                }

                if (option == "BookmarkDb")
                {
                    return BookmarkDb;
                }

                return Options.TryGetValue(option, out object? value) ? value : null;
            }
            set
            {
                if (scope == ConfigScope.Pack)
                {
                    Packs[option] = (string?)value ?? string.Empty;
                }
                else
                {
                    if (option == "GamePath")
                    {
                        GamePath = (string?)value ?? string.Empty;
                    }
                    else if (option == "BookmarkDb")
                    {
                        BookmarkDb = (string?)value ?? string.Empty;
                    }
                    else
                    {
                        Options[option] = value!;
                    }
                }
            }
        }
    }

    private static InternalConfig? s_current;

    public static IEnumerable<string> GameProfiles
    {
        get
        {
            s_current ??= new InternalConfig();
            return s_current.Games.Keys.ToList();
        }
    }

    /// <summary>
    /// Adds a <see cref="GameOptions"/> to the active <see cref="Config"/>.
    /// </summary>
    /// <param name="profile">The profile name.</param>
    /// <param name="gamePath">The path to the game.</param>
    public static void AddGame(string profile, string gamePath)
    {
        s_current ??= new InternalConfig();
        
        if (!s_current.Games.ContainsKey(profile))
        {
            s_current.Games.Add(profile, new GameOptions(gamePath));
        }
    }

    /// <summary>
    /// Removes a <see cref="GameOptions"/> from the active <see cref="Config"/>.
    /// </summary>
    /// <param name="profile">The profile name.</param>
    public static void RemoveGame(string profile)
    {
        s_current ??= new InternalConfig();
        
        s_current.Games.Remove(profile);
    }

    /// <summary>
    /// Adds or modifies the specified option in the <see cref="Config"/>.
    /// </summary>
    /// <param name="option">The key of the option to add or modify.</param>
    /// <param name="value">The value of the option to store.</param>
    /// <param name="scope">The <see cref="ConfigScope"/> of the option.</param>
    /// <param name="profile">The profile the option belongs to. If null, the currently active profile will be used.</param>
    public static void Add(string option, object value, ConfigScope scope = ConfigScope.Global, string? profile = null)
    {
        s_current ??= new InternalConfig();
        
        s_current[option, scope, profile] = value;
    }

    /// <summary>
    /// Removes the specified option in the <see cref="Config"/>.
    /// </summary>
    /// <param name="option">The key of the option to remove.</param>
    /// <param name="scope">The <see cref="ConfigScope"/> of the option.</param>
    /// <param name="profile">The profile the option belongs to. If null, the currently active profile will be used.</param>
    public static void Remove(string option, ConfigScope scope = ConfigScope.Global, string? profile = null)
    {
        s_current ??= new InternalConfig();
        
        if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
        {
            if (s_current.Games.TryGetValue(profile ?? ProfilesLibrary.ProfileName, out GameOptions? gameOptions))
            {
                gameOptions.Remove(option, scope);
            }
        }
        else
        {
            s_current.GlobalOptions.Remove(option);
        }
    }

    public static void Rename(string option, string newOption, ConfigScope scope = ConfigScope.Global, string? profile = null)
    {
        s_current ??= new InternalConfig();

        if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
        {
            if (s_current.Games.TryGetValue(profile ?? ProfilesLibrary.ProfileName, out GameOptions? gameOptions))
            {
                gameOptions.Rename(option, newOption, scope);
            }
        }
        else if (s_current.GlobalOptions.ContainsKey(option))
        {
            // TODO: is this correct ???
            s_current.GlobalOptions.Remove(option);
        }
    }

    public static IEnumerable<string> EnumerateKeys(ConfigScope scope = ConfigScope.Global, string? profile = null)
    {
        s_current ??= new InternalConfig();
        if (scope == ConfigScope.Global)
        {
            return s_current.GlobalOptions.Keys;
        }

        if (s_current.Games.TryGetValue(profile ?? ProfilesLibrary.ProfileName, out GameOptions? gameOptions))
        {
            return gameOptions.EnumerateKeys(scope);   
        }

        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Adds or modifies the specified option in the <see cref="Config"/>.
    /// </summary>
    /// <param name="option">The key of the option in the <see cref="Config"/> to add or modify.</param>
    /// <param name="defaultValue">The default value of the option, if not found in the <see cref="Config"/>.</param>
    /// <param name="scope">The <see cref="ConfigScope"/> of the option.</param>
    /// <param name="profile">The profile the option belongs to. If null, the currently active profile will be used.</param>
    
    public static T Get<T>(string option, T defaultValue, ConfigScope scope = ConfigScope.Global, string? profile = null)
    {
        s_current ??= new InternalConfig();
        
        return s_current.TryGetValue(option, scope, profile ?? ProfilesLibrary.ProfileName, out object? value) ? (T?)Convert.ChangeType(value, typeof(T)) ?? defaultValue : defaultValue;
    }


    // upgrades ini configs to unified json
    // public static void UpgradeConfigs()
    // {
    //     s_current = new InternalConfig();
    //
    //     if (File.Exists("config.json"))
    //     {
    //         if (!Directory.Exists(App.GlobalSettingsPath))
    //         {
    //             Directory.CreateDirectory(App.GlobalSettingsPath);
    //         }
    //
    //         File.Move("config.json", App.PluginManager.ManagerType == PluginManagerType.Editor ? $"{App.GlobalSettingsPath}/editor_config.json" : $"{App.GlobalSettingsPath}/manager_config.json");
    //     }
    //     else
    //     {
    //         foreach (string s in Directory.EnumerateFiles("./", "FrostyEditor*.ini"))
    //         {
    //             string categoryName = null;
    //             string currProfile = null;
    //             string gamePath = null;
    //
    //             using (NativeReader reader = new NativeReader(new FileStream(s, FileMode.Open, FileAccess.Read)))
    //             {
    //                 while (reader.Position < reader.Length)
    //                 {
    //                     string line = reader.ReadLine().Trim();
    //
    //                     if (line.StartsWith("#"))
    //                     {
    //                         continue;
    //                     }
    //
    //                     if (line.StartsWith("["))
    //                     {
    //                         line = line.Trim('[', ']');
    //                         categoryName = line;
    //                         continue;
    //                     }
    //
    //                     int index = line.IndexOf('=');
    //                     string key = line.Substring(0, index);
    //                     string value = line.Substring(index + 1);
    //
    //                     if (key == "Profile")
    //                     {
    //                         currProfile = value;
    //                     }
    //                     else if (key == "GamePath")
    //                     {
    //                         gamePath = value;
    //                     }
    //
    //                     // create the GameOptions object for the game
    //                     if (!string.IsNullOrEmpty(currProfile) && !string.IsNullOrEmpty(gamePath) && !s_current.Games.ContainsKey(currProfile))
    //                     {
    //                         s_current.Games.Add(currProfile, new GameOptions(gamePath));
    //                     }
    //
    //                     // use new default profile loading option
    //                     if (key == "RememberChoice")
    //                     {
    //                         key = "UseDefaultProfile";
    //
    //                         if (value.ToLower() == "true")
    //                         {
    //                             Add("DefaultProfile", currProfile);
    //                         }
    //                     }
    //
    //                     object outValue;
    //
    //                     if (value.ToLower() == "true" || value.ToLower() == "false")
    //                     {
    //                         outValue = Convert.ToBoolean(value.ToLower());
    //                     }
    //                     else if (int.TryParse(value, out _))
    //                     {
    //                         outValue = Convert.ToInt32(value);
    //                     }
    //                     else
    //                     {
    //                         outValue = value; // string
    //                     }
    //
    //                     // set the new option names (no categories saved in config)
    //                     switch (categoryName)
    //                     {
    //                         case "Autosave":
    //                         case "TextEditor":
    //                         case "MeshSetViewer":
    //                         case "Render":
    //                         case "MeshSetExport":
    //                         case "MeshSetImport":
    //                         case "DiscordRPC": key = categoryName + key; break;
    //                         case "DialogPaths": key += "Path"; break;
    //                         case "ModSettings": key = "Mod" + key; break;
    //                     }
    //
    //                     // move skeletons options to game specific options
    //                     if (categoryName == "MeshSetExport" || categoryName == "MeshSetImport" || key == "Language")
    //                     {
    //                         Add(key, outValue, ConfigScope.Game, currProfile);
    //                     }
    //                     else if (!string.IsNullOrEmpty(value))
    //                     {
    //                         Add(key, outValue);
    //                     }
    //                 }
    //             }
    //
    //             // remove options that should not be global
    //             Remove("GamePath");
    //             Remove("Profile");
    //             Remove("Language");
    //
    //             if (s_current.GlobalOptions.ContainsKey("BookmarkDb"))
    //             {
    //                 s_current.Games[currProfile].BookmarkDb = (string)s_current.GlobalOptions["BookmarkDb"];
    //                 Remove("BookmarkDb");
    //             }
    //         }
    //
    //         foreach (string s in Directory.EnumerateFiles("./", "FrostyModManager*.ini"))
    //         {
    //             string categoryName = null;
    //             string currProfile = null;
    //             string gamePath = null;
    //
    //             using (NativeReader reader = new NativeReader(new FileStream(s, FileMode.Open, FileAccess.Read)))
    //             {
    //                 while (reader.Position < reader.Length)
    //                 {
    //                     string line = reader.ReadLine().Trim();
    //
    //                     if (line.StartsWith("#"))
    //                     {
    //                         continue;
    //                     }
    //
    //                     if (line.StartsWith("["))
    //                     {
    //                         line = line.Trim('[', ']');
    //                         categoryName = line;
    //                         continue;
    //                     }
    //
    //                     int index = line.IndexOf('=');
    //                     string key = line.Substring(0, index);
    //                     string value = line.Substring(index + 1);
    //
    //                     if (key == "Profile")
    //                     {
    //                         currProfile = value;
    //                     }
    //                     else if (key == "GamePath")
    //                     {
    //                         gamePath = value;
    //                     }
    //
    //                     // create the GameOptions object for the game
    //                     if (!string.IsNullOrEmpty(currProfile) && !string.IsNullOrEmpty(gamePath) && !s_current.Games.ContainsKey(currProfile))
    //                     {
    //                         s_current.Games.Add(currProfile, new GameOptions(gamePath));
    //                     }
    //
    //                     // convert selected profile to pack
    //                     if (key == "SelectedProfile")
    //                     {
    //                         key = "SelectedPack";
    //                         Add("SelectedPack", value, ConfigScope.Game, currProfile);
    //                     }
    //
    //                     object outValue;
    //
    //                     if (value.ToLower() == "true" || value.ToLower() == "false")
    //                     {
    //                         outValue = Convert.ToBoolean(value.ToLower());
    //                     }
    //                     else if (int.TryParse(value, out _))
    //                     {
    //                         outValue = Convert.ToInt32(value);
    //                     }
    //                     else
    //                     {
    //                         outValue = value; // string
    //                     }
    //
    //                     // move skeletons options to game specific options
    //                     if (categoryName == "Profiles")
    //                     {
    //                         Add(key, outValue, ConfigScope.Pack, currProfile);
    //                     }
    //                     else if (!string.IsNullOrEmpty(value))
    //                     {
    //                         Add(key, outValue);
    //                     }
    //                 }
    //             }
    //
    //             // remove options that should not be global
    //             Remove("GamePath");
    //             Remove("Profile");
    //             Remove("Language");
    //             Remove("SelectedPack");
    //
    //             if (s_current.GlobalOptions.ContainsKey("BookmarkDb"))
    //             {
    //                 s_current.Games[currProfile].BookmarkDb = (string)s_current.GlobalOptions["BookmarkDb"];
    //                 Remove("BookmarkDb");
    //             }
    //
    //             if (s_current.GlobalOptions.ContainsKey("Profiles"))
    //             {
    //                 s_current.Games[currProfile].Packs = (Dictionary<string, string>)s_current.GlobalOptions["Profiles"];
    //                 Remove("Profiles");
    //             }
    //         }
    //
    //         // save the new unified config
    //         Save();
    //     }
    // }

    /// <summary>
    /// Serializes the configurable options to a json file.
    /// </summary>
    /// <param name="path">The output path of the configuration file.</param>
    public static void Save(string path)
    {
        s_current ??= new InternalConfig();
        
        FileInfo fileInfo = new(path);

        if (fileInfo.DirectoryName is not null)
        {
            Directory.CreateDirectory(fileInfo.DirectoryName);
        }

        using (FileStream stream = new(fileInfo.FullName, FileMode.Create, FileAccess.Write))
        {
            JsonSerializer.Serialize(stream, s_current, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }

    /// <summary>
    /// Deserializes the configurable options from a json file into a <see cref="Config"/> object.
    /// </summary>
    /// <param name="path">The input path of the configuration file.</param>
    public static void Load(string path)
    {
        FileInfo fileInfo = new(path);
        
        if (fileInfo.Exists)
        {
            using (FileStream stream = new(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                s_current = JsonSerializer.Deserialize<InternalConfig>(stream);
            }
        }
        
        s_current ??= new InternalConfig();
    }
}