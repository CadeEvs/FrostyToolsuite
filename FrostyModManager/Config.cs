/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrostySdk.IO;

namespace FrostyModManager
{
    public class Config
    {
        private class ConfigSection
        {
            private Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            public void Add(string key, object value)
            {
                if (values.ContainsKey(key))
                    values[key] = value;
                else
                    values.Add(key, value);
            }

            public T Get<T>(string key, T defaultValue = default(T))
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

            public IEnumerable<string> EnumerateKeys()
            {
                List<string> keys = values.Keys.ToList();
                foreach (string key in keys)
                    yield return key;
            }

            public void Write(NativeWriter writer)
            {
                foreach (string key in values.Keys)
                    writer.WriteLine(string.Format("{0}={1}", key, values[key].ToString()));
            }
        }

        private static Config Current;
        private Dictionary<string, ConfigSection> sections = new Dictionary<string, ConfigSection>();

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

                    string[] keyValuePair = line.Split('=');
                    section.Add(keyValuePair[0], keyValuePair[1]);
                }
            }

            return true;
        }


        public static bool LoadDefault(string configFilename)
        {
            Current = new Config();
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
                        Current.sections.Add(line, section);

                        continue;
                    }

                    string[] keyValuePair = line.Split('=');
                    if (keyValuePair.Length >= 2)
                        section.Add(keyValuePair[0], keyValuePair[1]);
                }
            }

            return true;
        }

        public static void Load(Config config)
        {
            Current = config;
        }

        public static void Save(string filename)
        {
            using (NativeWriter writer = new NativeWriter(new FileStream(filename, FileMode.Create)))
            {
                foreach (string sectionName in Current.sections.Keys)
                {
                    writer.WriteLine(string.Format("[{0}]", sectionName));
                    Current.sections[sectionName].Write(writer);
                }
            }
        }

        public static void Delete(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        public static void Add(string section, string key, object value)
        {
            if (!Current.sections.ContainsKey(section))
                Current.sections.Add(section, new ConfigSection());
            Current.sections[section].Add(key, value);
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

        public static void Remove(string section, string key)
        {
            if (!Current.sections.ContainsKey(section))
                return;
            Current.sections[section].Remove(key);
        }

        public T GetEntry<T>(string section, string key, T defaultValue)
        {
            return !sections.ContainsKey(section) ? defaultValue : sections[section].Get<T>(key, defaultValue);
        }

        public static IEnumerable<string> EnumerateKeys(string section)
        {
            return !Current.sections.ContainsKey(section) ? null : Current.sections[section].EnumerateKeys();
        }

        public static bool ContainsSection(string section)
        {
            return Current.sections.ContainsKey(section);
        }
    }
}
*/