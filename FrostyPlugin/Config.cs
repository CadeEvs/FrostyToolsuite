using FrostySdk;
using FrostySdk.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            // check if the provided value implements IConvertible or JToken
            if (value is IConvertible || value is JToken)
            {
                // we can safely store the value without any modifications
                Current[option, scope, profile] = value;
            }
            else
            {
                // if the given value does not inherit IConvertible or JToken, it must be wrapped in a JToken
                // otherwise, types like collections, arrays, etc will cause an exception
                Current[option, scope, profile] = JToken.FromObject(value);
            }
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
            object retrievedValue = Current[option, scope, profile ?? ProfilesLibrary.ProfileName];

            if (retrievedValue != null)
            {
                // check if the object is or derives from a JToken (i.e raw JSON objects)
                if (retrievedValue is JToken)
                {
                    // utilize the JToken converter rather than the convert class' converter
                    retrievedValue = ((JToken)retrievedValue).ToObject(typeof(T));
                }
                else
                {
                    // if the retrieved value inherits IConvertible, use the Convert class for a conversion; otherwise, attempt to cast the value
                    retrievedValue = Current[option, scope, profile ?? ProfilesLibrary.ProfileName];
                    retrievedValue = retrievedValue is IConvertible ? (T)Convert.ChangeType(Current[option, scope, profile ?? ProfilesLibrary.ProfileName], typeof(T)) : (T)retrievedValue;
                }
            }

            return (T)(retrievedValue != null ? retrievedValue : defaultValue);
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

                // TODO techdebt: fix underlying issue with config getting set to null when saving
                if (Current == null)
                {
                    Current = new Config();
                }
            }
        }
    }
}