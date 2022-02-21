using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrostySdk;
using FrostySdk.IO;
using Newtonsoft.Json;

namespace Frosty.Core
{
    public enum ConfigScope
    {
        Global,
        Game,
        Pack
    }

    // @todo: allow overrides of any option within game specific options (will be allowed via manual edits if people really want to)
    // @todo: make fully static
    public sealed class Config
    {
        public static Config Current { get; private set; }
        public static List<string> GameProfiles => Current.Games.Keys.ToList();

        public Dictionary<string, GameOptions> Games { get; } = new Dictionary<string, GameOptions>();
        public Dictionary<string, object> GlobalOptions { get; } = new Dictionary<string, object>();

        // game specific options object
        public class GameOptions
        {
            //public string ProfileName { get; }
            public string GamePath { get; private set; }
            public string BookmarkDb { get; set; } = "[Asset Bookmarks]|[Legacy Bookmarks]";

            public Dictionary<string, object> Options { get; } = new Dictionary<string, object>();
            public Dictionary<string, string> Packs { get; set; } = new Dictionary<string, string>();

            public GameOptions(string gamePath)
            {
                GamePath = gamePath;
            }

            internal bool Contains(string option, ConfigScope scope = ConfigScope.Game) => scope == ConfigScope.Game ? Options.ContainsKey(option) : Packs.ContainsKey(option);

            internal void Add(string option, object value) => this[option] = value;

            internal void Remove(string option, ConfigScope scope = ConfigScope.Game)
            {
                if (scope == ConfigScope.Game)
                {
                    if (Options.ContainsKey(option))
                        Options.Remove(option);
                }
                else
                {
                    if (Packs.ContainsKey(option))
                        Packs.Remove(option);
                }
            }

            internal IEnumerable<string> EnumerateKeys(ConfigScope scope = ConfigScope.Game)
            {
                if (scope == ConfigScope.Game)
                {
                    List<string> keys = Options.Keys.ToList();
                    foreach (string key in keys)
                        yield return key;
                }
                else
                {
                    List<string> keys = Packs.Keys.ToList();
                    foreach (string key in keys)
                        yield return key;
                }
            }

            // indexer
            internal object this[string option, ConfigScope scope = ConfigScope.Game]
            {
                get
                {
                    if (scope == ConfigScope.Pack)
                        return Packs.ContainsKey(option) ? Packs[option] : null;
                    else
                    {
                        if (option == "GamePath")
                            return GamePath;

                        if (option == "BookmarkDb")
                            return BookmarkDb;

                        return Options.ContainsKey(option) ? Options[option] : null;
                    }
                }
                set
                {
                    if (scope == ConfigScope.Pack)
                    {
                        if (!Packs.ContainsKey(option))
                            Packs.Add(option, (string)value);
                        else
                            Packs[option] = (string)value;
                    }
                    else
                    {
                        if (option == "GamePath")
                            GamePath = (string)value;
                        else if (option == "BookmarkDb")
                            BookmarkDb = (string)value;
                        else if (!Options.ContainsKey(option))
                            Options.Add(option, value);
                        else
                            Options[option] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a <see cref="GameOptions"/> to the active <see cref="Config"/>.
        /// </summary>
        /// <param name="profile">The profile name.</param>
        /// <param name="gamePath">The path to the game.</param>
        public static void AddGame(string profile, string gamePath)
        {
            if (!Current.Games.ContainsKey(profile))
            {
                Current.Games.Add(profile, new GameOptions(gamePath));
                Save();
            }
        }

        /// <summary>
        /// Removes a <see cref="GameOptions"/> from the active <see cref="Config"/>.
        /// </summary>
        /// <param name="profile">The profile name.</param>
        public static void RemoveGame(string profile)
        {
            if (Current.Games.ContainsKey(profile))
            {
                Current.Games.Remove(profile);
                Save();
            }
        }

        /// <summary>
        /// Adds or modifies the specified option in the <see cref="Config"/>.
        /// </summary>
        /// <param name="option">The key of the option to add or modify.</param>
        /// <param name="value">The value of the option to store.</param>
        /// <param name="scope">The <see cref="ConfigScope"/> of the option.</param>
        /// <param name="profile">The profile the option belongs to. If null, the currently active profile will be used.</param>
        public static void Add(string option, object value, ConfigScope scope = ConfigScope.Global, string profile = null)
        {
            Current[option, scope, profile] = value;
        }

        /// <summary>
        /// Removes the specified option in the <see cref="Config"/>.
        /// </summary>
        /// <param name="option">The key of the option to remove.</param>
        /// <param name="scope">The <see cref="ConfigScope"/> of the option.</param>
        /// <param name="profile">The profile the option belongs to. If null, the currently active profile will be used.</param>
        public static void Remove(string option, ConfigScope scope = ConfigScope.Global, string profile = null)
        {
            if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
                Current.Games[profile ?? ProfilesLibrary.ProfileName].Remove(option, scope);
            else if (Current.GlobalOptions.ContainsKey(option))
                Current.GlobalOptions.Remove(option);
        }

        /// <summary>
        /// Determines whether the <see cref="Config"/> contains the specified key.
        /// </summary>
        /// <param name="option">The option key to locate in the <see cref="Config"/>.</param>
        public bool Contains(string option, ConfigScope scope = ConfigScope.Global)
        {
            if (scope == ConfigScope.Global)
                return GlobalOptions.ContainsKey(option);
            else if (scope == ConfigScope.Game)
                return Games[ProfilesLibrary.ProfileName].Contains(option, ConfigScope.Game);
            else
                return Games[ProfilesLibrary.ProfileName].Contains(option, ConfigScope.Pack);
        }

        public static IEnumerable<string> EnumerateKeys(ConfigScope scope = ConfigScope.Global, string profile = null)
        {
            if (scope == ConfigScope.Global)
                return Current.GlobalOptions.Keys;
            else
                return Current.Games[profile ?? ProfilesLibrary.ProfileName].EnumerateKeys(scope);
        }

        /// <summary>
        /// Adds or modifies the specified option in the <see cref="Config"/>.
        /// </summary>
        /// <param name="option">The key of the option in the <see cref="Config"/> to add or modify.</param>
        /// <param name="defaultValue">The default value of the option, if not found in the <see cref="Config"/>.</param>
        /// <param name="scope">The <see cref="ConfigScope"/> of the option.</param>
        /// <param name="profile">The profile the option belongs to. If null, the currently active profile will be used.</param>
        public static T Get<T>(string option, T defaultValue, ConfigScope scope = ConfigScope.Global, string profile = null)
        {
            return Current[option, scope, profile ?? ProfilesLibrary.ProfileName] == null ? defaultValue : (T)Convert.ChangeType(Current[option, scope, profile ?? ProfilesLibrary.ProfileName], typeof(T));
        }

        // indexer
        private object this[string option, ConfigScope scope = ConfigScope.Global, string profile = null]
        {
            get
            {
                if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
                    return Games[profile ?? ProfilesLibrary.ProfileName][option, scope]; // indexer will return null if not found

                return GlobalOptions.ContainsKey(option) ? GlobalOptions[option] : null;
            }
            set
            {
                if (scope == ConfigScope.Game || scope == ConfigScope.Pack)
                    Games[profile ?? ProfilesLibrary.ProfileName][option, scope] = value;
                else if (!GlobalOptions.ContainsKey(option))
                    GlobalOptions.Add(option, value);
                else
                    GlobalOptions[option] = value;
            }
        }

        // upgrades ini configs to unified json
        public static void UpgradeConfigs()
        {
            Current = new Config();

            if (File.Exists("config.json"))
            {
                if (!Directory.Exists(App.GlobalSettingsPath))
                    Directory.CreateDirectory(App.GlobalSettingsPath);

                File.Move("config.json", App.PluginManager.ManagerType == PluginManagerType.Editor ? $"{App.GlobalSettingsPath}/editor_config.json" : $"{App.GlobalSettingsPath}/manager_config.json");
            }
            else
            {
                foreach (string s in Directory.EnumerateFiles("./", "FrostyEditor*.ini"))
                {
                    string categoryName = null;
                    string currProfile = null;
                    string gamePath = null;

                    using (NativeReader reader = new NativeReader(new FileStream(s, FileMode.Open, FileAccess.Read)))
                    {
                        while (reader.Position < reader.Length)
                        {
                            string line = reader.ReadLine().Trim();

                            if (line.StartsWith("#"))
                                continue;

                            if (line.StartsWith("["))
                            {
                                line = line.Trim('[', ']');
                                categoryName = line;
                                continue;
                            }

                            int index = line.IndexOf('=');
                            string key = line.Substring(0, index);
                            string value = line.Substring(index + 1);

                            if (key == "Profile")
                                currProfile = value;
                            else if (key == "GamePath")
                                gamePath = value;

                            // create the GameOptions object for the game
                            if (!string.IsNullOrEmpty(currProfile) && !string.IsNullOrEmpty(gamePath) && !Current.Games.ContainsKey(currProfile))
                                Current.Games.Add(currProfile, new GameOptions(gamePath));

                            // use new default profile loading option
                            if (key == "RememberChoice")
                            {
                                key = "UseDefaultProfile";

                                if (value.ToLower() == "true")
                                    Add("DefaultProfile", currProfile);
                            }

                            object outValue;

                            if (value.ToLower() == "true" || value.ToLower() == "false")
                            {
                                outValue = Convert.ToBoolean(value.ToLower());
                            }
                            else if (int.TryParse(value, out _))
                            {
                                outValue = Convert.ToInt32(value);
                            }
                            else
                            {
                                outValue = value; // string
                            }

                            // set the new option names (no categories saved in config)
                            switch (categoryName)
                            {
                                case "Autosave":
                                case "TextEditor":
                                case "MeshSetViewer":
                                case "Render":
                                case "MeshSetExport":
                                case "MeshSetImport":
                                case "DiscordRPC": key = categoryName + key; break;
                                case "DialogPaths": key += "Path"; break;
                                case "ModSettings": key = "Mod" + key; break;
                            }

                            // move skeletons options to game specific options
                            if (categoryName == "MeshSetExport" || categoryName == "MeshSetImport" || key == "Language")
                                Add(key, outValue, ConfigScope.Game, currProfile);
                            else if (!string.IsNullOrEmpty(value))
                                Add(key, outValue);
                        }
                    }

                    // remove options that should not be global
                    Remove("GamePath");
                    Remove("Profile");
                    Remove("Language");

                    if (Current.GlobalOptions.ContainsKey("BookmarkDb"))
                    {
                        Current.Games[currProfile].BookmarkDb = (string)Current.GlobalOptions["BookmarkDb"];
                        Remove("BookmarkDb");
                    }
                }

                foreach (string s in Directory.EnumerateFiles("./", "FrostyModManager*.ini"))
                {
                    string categoryName = null;
                    string currProfile = null;
                    string gamePath = null;

                    using (NativeReader reader = new NativeReader(new FileStream(s, FileMode.Open, FileAccess.Read)))
                    {
                        while (reader.Position < reader.Length)
                        {
                            string line = reader.ReadLine().Trim();

                            if (line.StartsWith("#"))
                                continue;

                            if (line.StartsWith("["))
                            {
                                line = line.Trim('[', ']');
                                categoryName = line;
                                continue;
                            }

                            int index = line.IndexOf('=');
                            string key = line.Substring(0, index);
                            string value = line.Substring(index + 1);

                            if (key == "Profile")
                                currProfile = value;
                            else if (key == "GamePath")
                                gamePath = value;

                            // create the GameOptions object for the game
                            if (!string.IsNullOrEmpty(currProfile) && !string.IsNullOrEmpty(gamePath) && !Current.Games.ContainsKey(currProfile))
                                Current.Games.Add(currProfile, new GameOptions(gamePath));

                            // convert selected profile to pack
                            if (key == "SelectedProfile")
                            {
                                key = "SelectedPack";
                                Add("SelectedPack", value, ConfigScope.Game, currProfile);
                            }

                            object outValue;

                            if (value.ToLower() == "true" || value.ToLower() == "false")
                            {
                                outValue = Convert.ToBoolean(value.ToLower());
                            }
                            else if (int.TryParse(value, out _))
                            {
                                outValue = Convert.ToInt32(value);
                            }
                            else
                            {
                                outValue = value; // string
                            }

                            // move skeletons options to game specific options
                            if (categoryName == "Profiles")
                                Add(key, outValue, ConfigScope.Pack, currProfile);
                            else if (!string.IsNullOrEmpty(value))
                                Add(key, outValue);
                        }
                    }

                    // remove options that should not be global
                    Remove("GamePath");
                    Remove("Profile");
                    Remove("Language");
                    Remove("SelectedPack");

                    if (Current.GlobalOptions.ContainsKey("BookmarkDb"))
                    {
                        Current.Games[currProfile].BookmarkDb = (string)Current.GlobalOptions["BookmarkDb"];
                        Remove("BookmarkDb");
                    }

                    if (Current.GlobalOptions.ContainsKey("Profiles"))
                    {
                        Current.Games[currProfile].Packs = (Dictionary<string, string>)Current.GlobalOptions["Profiles"];
                        Remove("Profiles");
                    }
                }

                // save the new unified config
                Save();
            }
        }

        /// <summary>
        /// Serializes the configurable options to a json file.
        /// </summary>
        /// <param name="path">The output path of the configuration file.</param>
        public static void Save(string path = "")
        {
            if (path == "")
                path = App.PluginManager.ManagerType == PluginManagerType.Editor ? $"{App.GlobalSettingsPath}/editor_config.json" : $"{App.GlobalSettingsPath}/manager_config.json";

            if (!Directory.Exists(App.GlobalSettingsPath))
                Directory.CreateDirectory(App.GlobalSettingsPath);

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(JsonConvert.SerializeObject(Current, Formatting.Indented));
            }
        }

        /// <summary>
        /// Deserializes the configurable options from a json file into a <see cref="Config"/> object.
        /// </summary>
        /// <param name="path">The input path of the configuration file.</param>
        public static void Load(string path = "")
        {
            if (path == "")
                path = App.PluginManager.ManagerType == PluginManagerType.Editor ? $"{App.GlobalSettingsPath}/editor_config.json" : $"{App.GlobalSettingsPath}/manager_config.json";

            using (StreamReader reader = new StreamReader(path))
            {
                Current = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
            }
        }
    }

    /*
    public sealed class Config
    {
        private class ConfigSection
        {
            private readonly Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            public void Add(string key, object value)
            {
                if (values.ContainsKey(key))
                {
                    values[key] = value;
                    return;
                }

                values.Add(key, value);
            }

            public T Get<T>(string key, T defaultValue)
            {
                if (!values.ContainsKey(key))
                    return defaultValue;
                return (T)Convert.ChangeType(values[key], typeof(T));
            }

            public void Remove(string key)
            {
                if (!values.ContainsKey(key))
                    return;
                values.Remove(key);
            }

            public bool Contains(string key)
            {
                return values.ContainsKey(key);
            }

            public void Write(NativeWriter writer)
            {
                foreach (string key in values.Keys)
                    writer.WriteLine($"{key}={values[key]}");
            }
        }

        public static Config Current { get; private set; }
        private Dictionary<string, ConfigSection> sections = new Dictionary<string, ConfigSection>();

        public static bool Load(string configFilename)
        {
            Current = new Config();
            return Current.LoadEntries(configFilename);
        }

        public static void Load(Config config)
        {
            Current = config;
        }

        public bool LoadEntries(string configFilename)
        {
            if (!File.Exists(configFilename))
                return false;

            using (NativeReader reader = new NativeReader(new FileStream(configFilename, FileMode.Open, FileAccess.Read)))
            {
                ConfigSection section = null;

                while (reader.Position < reader.Length)
                {
                    string line = reader.ReadLine().Trim();

                    if (line.StartsWith("#"))
                        continue;

                    if (line.StartsWith("["))
                    {
                        line = line.Trim('[', ']');

                        section = new ConfigSection();
                        sections.Add(line, section);

                        continue;
                    }

                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index);
                    string value = line.Substring(index + 1);

                    section.Add(key, value);
                }
            }

            return true;
        }

        public static void Save(string filename)
        {
            Current.SaveEntries(filename);
        }

        public void SaveEntries(string filename)
        {
            using (NativeWriter writer = new NativeWriter(new FileStream(filename, FileMode.Create)))
            {
                foreach (string sectionName in sections.Keys)
                {
                    writer.WriteLine(string.Format("[{0}]", sectionName));
                    sections[sectionName].Write(writer);
                }
            }
        }

        public static void Add(string section, string key, object value)
        {
            Current.AddEntry(section, key, value);
        }

        public void AddEntry(string section, string key, object value)
        {
            if (!sections.ContainsKey(section))
                sections.Add(section, new ConfigSection());
            sections[section].Add(key, value);
        }

        public static T Get<T>(string section, string key, T defaultValue)
        {
            return Current.GetEntry<T>(section, key, defaultValue);
        }

        public T GetEntry<T>(string section, string key, T defaultValue)
        {
            return !sections.ContainsKey(section) ? defaultValue : sections[section].Get<T>(key, defaultValue);
        }

        public static void Remove(string section, string key)
        {
            Current.RemoveEntry(section, key);
        }

        public void RemoveEntry(string section, string key)
        {
            if (sections.ContainsKey(section))
                sections[section].Remove(key);
        }

        public static bool Contains(string section, string key)
        {
            return Current.ContainsEntry(section, key);
        }

        public bool ContainsEntry(string section, string key)
        {
            return !sections.ContainsKey(section) && sections[section].Contains(key);
        }
    }*/
}