﻿using Frosty.Controls;
using FrostySdk.IO;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrostyModManager
{
    class ConfigElement
    {
        public object CurrentValue
        {
            get => currentValue;
            internal set => currentValue = value;
        }

        protected string id;
        protected string displayName;
        protected object currentValue;

        public ConfigElement(string inId, string inDisplayName)
        {
            id = inId;
            displayName = inDisplayName;
        }

        public override string ToString() => displayName;
    }

    class ConfigElement<T> : ConfigElement
    {
        T defValue;
        T minValue;
        T maxValue;

        public ConfigElement(string inId, string inDisplayName, T inDefValue, T inMinValue, T inMaxValue)
            : base(inId, inDisplayName)
        {
            defValue = inDefValue;
            minValue = inMinValue;
            maxValue = inMaxValue;
            currentValue = defValue;
        }
    }

    class EnumConfigElement : ConfigElement
    {
        int defValue;
        public EnumConfigElement(string inId, string inDisplayName, Type inType, int inDefValue)
            : base(inId, inDisplayName)
        {
            defValue = inDefValue;
            currentValue = defValue;
        }
    }

    class StringConfigElement : ConfigElement
    {
        string defValue;
        public StringConfigElement(string inId, string inDisplayName, string inDefValue)
            : base(inId, inDisplayName)
        {
            defValue = inDefValue;
            CurrentValue = defValue;
        }
    }

    class FrostyConfigElementsList : DAIMod.ModConfigElementsList
    {
        public int NumElements => elements.Count;

        private ConfigWindow theWindow;
        private Dictionary<string, ConfigElement> elements = new Dictionary<string, ConfigElement>();

        // [DAI fix] applied before the script runs, so a value still being typed isn't lost
        private List<Action> commitActions = new List<Action>();

        // [DAI fix] settings chosen last time, by id
        private IDictionary<string, string> savedValues;

        // fallback for mods converted before settings were saved: the "<name> <value>" lines at the end of the description
        private string[] legacyLines;

        public FrostyConfigElementsList(ConfigWindow inWindow, IDictionary<string, string> inSavedValues = null, string inLegacyDescription = null)
        {
            theWindow = inWindow;
            savedValues = inSavedValues;
            if (!string.IsNullOrEmpty(inLegacyDescription))
                legacyLines = inLegacyDescription.Replace("\r\n", "\n").Split('\n');
        }

        public IEnumerable<KeyValuePair<string, ConfigElement>> Elements => elements;

        private bool TryGetSaved(string id, string name, out string value)
        {
            if (savedValues != null && savedValues.TryGetValue(id, out value) && value != null)
                return true;

            if (legacyLines != null && !string.IsNullOrEmpty(name))
            {
                // the settings are at the end of the description
                string prefix = name + " ";
                for (int i = legacyLines.Length - 1; i >= 0; i--)
                {
                    if (legacyLines[i].StartsWith(prefix, StringComparison.Ordinal))
                    {
                        value = legacyLines[i].Substring(prefix.Length);
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        public void CommitAll()
        {
            foreach (Action commit in commitActions)
                commit();
        }

        private void AddElement(string id, ConfigElement ce)
        {
            // [DAI fix] a duplicate id used to throw; last one wins
            elements[id] = ce;
        }

        private static bool TryParseFloat(string text, out float value)
        {
            // [DAI fix] accept the user's locale and plain "1.5"
            return float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                || float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public override void AddIntElement(string id, string name, int defValue, int minValue, int maxValue)
        {
            ConfigElement<int> ce = new ConfigElement<int>(id, name, defValue, minValue, maxValue);
            AddElement(id, ce);

            // [DAI fix] restore saved value
            int initial = defValue;
            if (TryGetSaved(id, name, out string saved) && int.TryParse(saved.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int savedInt))
                initial = Math.Max(minValue, Math.Min(maxValue, savedInt));
            ce.CurrentValue = initial;

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = initial.ToString(),
                Height = 24,
                Margin = new Thickness(1),
                Padding = new Thickness(2, 0, 0, 0),
                BorderThickness = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            Action commit = () =>
            {
                if (int.TryParse(tb.Text, out int newValue))
                {
                    if (newValue < minValue)
                        newValue = minValue;
                    else if (newValue > maxValue)
                        newValue = maxValue;
                }
                else
                {
                    newValue = defValue;
                    FrostyMessageBox.Show("Invalid value entered for \"" + name + "\", using the default instead", "Frosty Mod Manager");
                }

                tb.Text = newValue.ToString();
                ce.CurrentValue = newValue;
            };
            commitActions.Add(commit);

            tb.LostFocus += (o, e) => commit();
            tb.KeyDown += (o, e) => { if (e.Key == Key.Enter) { tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); } };

            theWindow.valuesStackPanel.Children.Add(tb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddFloatElement(string id, string name, float defValue, float minValue, float maxValue)
        {
            ConfigElement<float> ce = new ConfigElement<float>(id, name, defValue, minValue, maxValue);
            AddElement(id, ce);

            // [DAI fix] restore saved value
            float initial = defValue;
            if (TryGetSaved(id, name, out string saved) && TryParseFloat(saved.Trim(), out float savedFloat))
                initial = Math.Max(minValue, Math.Min(maxValue, savedFloat));
            ce.CurrentValue = initial;

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = initial.ToString("F3"),
                Height = 24,
                Margin = new Thickness(1),
                Padding = new Thickness(2, 0, 0, 0),
                BorderThickness = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            Action commit = () =>
            {
                if (TryParseFloat(tb.Text, out float newValue))
                {
                    if (newValue < minValue)
                        newValue = minValue;
                    else if (newValue > maxValue)
                        newValue = maxValue;
                }
                else
                {
                    newValue = defValue;
                    FrostyMessageBox.Show("Invalid value entered for \"" + name + "\", using the default instead", "Frosty Mod Manager");
                }

                tb.Text = newValue.ToString("F3");
                ce.CurrentValue = newValue;
            };
            commitActions.Add(commit);

            tb.LostFocus += (o, e) => commit();
            tb.KeyDown += (o, e) => { if (e.Key == Key.Enter) { tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); } };

            theWindow.valuesStackPanel.Children.Add(tb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddBoolElement(string id, string name, bool defValue)
        {
            ConfigElement<bool> ce = new ConfigElement<bool>(id, name, defValue, false, true);
            AddElement(id, ce);

            // [DAI fix] restore saved value
            bool initial = defValue;
            if (TryGetSaved(id, name, out string saved) && bool.TryParse(saved.Trim(), out bool savedBool))
                initial = savedBool;
            ce.CurrentValue = initial;

            Label lbl = new Label {Content = name};

            CheckBox cb = new CheckBox
            {
                IsChecked = initial,
                Height = 24,
                Margin = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            cb.Checked += (o, e) => { ce.CurrentValue = true; };
            cb.Unchecked += (o, e) => { ce.CurrentValue = false; };

            theWindow.valuesStackPanel.Children.Add(cb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddEnumElement(string id, string name, Type enumType, int defValue)
        {
            EnumConfigElement ce = new EnumConfigElement(id, name, enumType, defValue);
            AddElement(id, ce);

            Label lbl = new Label {Content = name};

            // [DAI fix] pass the enum value, not the dropdown index (they differ unless the values are 0, 1, 2...)
            string[] names = Enum.GetNames(enumType);
            int[] values = new int[names.Length];
            bool valuesUsable = true;
            try
            {
                Array rawValues = Enum.GetValues(enumType);
                for (int i = 0; i < values.Length; i++)
                    values[i] = Convert.ToInt32(rawValues.GetValue(i), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                valuesUsable = false;
            }
            if (!valuesUsable)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = i;
            }

            // defValue is normally an enum value, but may be an index
            int selectedIndex = Array.IndexOf(values, defValue);
            if (selectedIndex < 0 && defValue >= 0 && defValue < values.Length)
                selectedIndex = defValue;

            // saved as the enum value
            if (TryGetSaved(id, name, out string saved) && int.TryParse(saved.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int savedEnum))
            {
                int savedIndex = Array.IndexOf(values, savedEnum);
                if (savedIndex >= 0)
                    selectedIndex = savedIndex;
            }

            if (selectedIndex >= 0)
                ce.CurrentValue = values[selectedIndex];

            ComboBox cb = new ComboBox
            {
                ItemsSource = names,
                SelectedIndex = selectedIndex,
                Padding = new Thickness(4, 0, 0, 0),
                Margin = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 24
            };

            cb.SelectionChanged += (o, e) =>
            {
                if (cb.SelectedIndex >= 0 && cb.SelectedIndex < values.Length)
                    ce.CurrentValue = values[cb.SelectedIndex];
            };

            theWindow.valuesStackPanel.Children.Add(cb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddStringElement(string id, string name, string defValue)
        {
            StringConfigElement ce = new StringConfigElement(id, name, defValue ?? "");
            AddElement(id, ce);

            // [DAI fix] restore saved value
            string initial = defValue ?? "";
            if (TryGetSaved(id, name, out string saved))
                initial = saved;
            ce.CurrentValue = initial;

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = initial,
                Height = 24,
                Margin = new Thickness(1),
                Padding = new Thickness(2, 0, 0, 0),
                BorderThickness = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            Action commit = () => { ce.CurrentValue = tb.Text; };
            commitActions.Add(commit);

            tb.LostFocus += (o, e) => commit();
            tb.KeyDown += (o, e) => { if (e.Key == Key.Enter) { tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); } };

            theWindow.valuesStackPanel.Children.Add(tb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public ConfigElement GetElement(string name) => !elements.ContainsKey(name) ? null : elements[name];

        public ConfigElement GetElement(int index) => index >= elements.Count ? null : elements.Values.ElementAt(index);
    }

    class FrostyScripting : DAIMod.IScripting
    {
        public FrostyConfigElementsList config;
        public List<byte[]> resources;
        public List<bool> resourceState;
        public List<string> log = new List<string>();

        public FrostyScripting(FrostyConfigElementsList inConfig, List<byte[]> inResources, List<bool> inStates)
        {
            config = inConfig;
            resources = inResources;
            resourceState = inStates;
        }

        public object GetConfigParam(string name)
        {
            // [DAI fix] 0 for an unknown name instead of crashing, like DAI's default script interface
            ConfigElement elem = config.GetElement(name);
            if (elem == null)
            {
                log.Add("Script asked for unknown setting \"" + name + "\", using 0");
                return 0;
            }
            return elem.CurrentValue;
        }

        // [DAI fix] used to be discarded
        public void LogLn(string line) => log.Add(line);

        public byte[] GetResourceData(int index)
        {
            if (index < 0 || index >= resources.Count)
                return null;

            using (CasReader reader = new CasReader(new MemoryStream(resources[index])))
                return reader.Read();
        }

        public void SetResourceData(int index, byte[] data)
        {
            if (index < 0 || index >= resources.Count || data == null)
            {
                log.Add("Script tried to replace resource " + index + ", which does not exist; ignored");
                return;
            }
            resources[index] = FrostySdk.Utils.CompressFile(data);
        }

        public void SetResourceEnabled(int index, bool enabled)
        {
            if (index < 0 || index >= resourceState.Count)
            {
                log.Add("Script tried to enable/disable resource " + index + ", which does not exist; ignored");
                return;
            }
            resourceState[index] = enabled;
        }
    }

    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : FrostyDockableWindow
    {
        // [DAI fix] closing without Save used to import the mod with the script never run
        public bool ScriptApplied { get; private set; }

        public string ScriptError { get; private set; }

        private DAIMod.ModScript modScript;
        private FrostyScripting scripting;

        // [DAI fix] savedValues / legacyDescription pre-fill the settings (Reconfigure)
        public ConfigWindow(string code, List<byte[]> inResources, List<bool> inStates, IDictionary<string, string> savedValues = null, string legacyDescription = null)
        {
            InitializeComponent();

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            // [DAI fix] full paths so they resolve from any working directory. There is no System.Linq.dll
            // in .NET Framework (Linq is in System.Core).
            HashSet<string> references = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                typeof(Uri).Assembly.Location,
                typeof(Enumerable).Assembly.Location,
                typeof(System.Xml.XmlDocument).Assembly.Location,
                typeof(System.Xml.Linq.XDocument).Assembly.Location,
                typeof(System.Data.DataTable).Assembly.Location,
                typeof(System.Drawing.Color).Assembly.Location,
                typeof(DAIMod.ModScript).Assembly.Location
            };
            foreach (string reference in references)
            {
                if (!string.IsNullOrEmpty(reference))
                    parameters.ReferencedAssemblies.Add(reference);
            }

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);

            // [DAI fix] compile errors used to crash the import
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The mod's configuration script failed to compile:");
                foreach (CompilerError err in results.Errors)
                {
                    if (!err.IsWarning)
                        sb.AppendLine(string.Format("  line {0}: {1} ({2})", err.Line, err.ErrorText, err.ErrorNumber));
                }
                throw new InvalidOperationException(sb.ToString());
            }

            foreach (Type type in results.CompiledAssembly.GetTypes())
            {
                if (typeof(DAIMod.ModScript).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    modScript = (DAIMod.ModScript)Activator.CreateInstance(type);
                    break;
                }
            }

            if (modScript == null)
                throw new InvalidOperationException("The mod's configuration script does not contain a ModScript class.");

            scripting = new FrostyScripting(new FrostyConfigElementsList(this, savedValues, legacyDescription), inResources, inStates);
            DAIMod.Scripting.SetScriptInterface(scripting);

            modScript.ConstructUI(scripting.config);
        }

        public string GetConfigValues()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < scripting.config.NumElements; i++)
            {
                ConfigElement elem = scripting.config.GetElement(i);
                sb.AppendLine(elem.ToString() + " " + (elem.CurrentValue?.ToString() ?? ""));
            }

            return sb.ToString();
        }

        /// <summary>
        /// [DAI fix] The chosen settings keyed by setting id, written culture-independently, so they can
        /// be saved and passed back in next time (Reconfigure).
        /// </summary>
        public Dictionary<string, string> GetSettingValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (KeyValuePair<string, ConfigElement> pair in scripting.config.Elements)
            {
                object value = pair.Value.CurrentValue;
                if (value is float f)
                    values[pair.Key] = f.ToString("R", CultureInfo.InvariantCulture);
                else if (value is IFormattable formattable)
                    values[pair.Key] = formattable.ToString(null, CultureInfo.InvariantCulture);
                else
                    values[pair.Key] = value?.ToString() ?? "";
            }
            return values;
        }

        public List<bool> GetResourceStates() => scripting.resourceState;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // [DAI fix] commit a value still being typed, and catch script errors (they crashed the Mod Manager)
            scripting.config.CommitAll();

            try
            {
                modScript.RunScript();
                ScriptApplied = true;
            }
            catch (Exception ex)
            {
                ScriptError = "The mod's configuration script failed while running: " + ex.Message;
            }

            foreach (string line in scripting.log)
                App.Logger.Log("[DAI script] " + line);

            Close();
        }
    }
}
