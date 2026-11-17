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

// [DAI fix] This file was changed to fix several problems with configurable .daimod files.
// See DAI_FIXES.md for the full list.

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

        // [DAI fix] actions that copy whatever is typed in the text boxes into the config values,
        // run before the script so a value that is still being edited isn't lost
        private List<Action> commitActions = new List<Action>();

        public FrostyConfigElementsList(ConfigWindow inWindow)
        {
            theWindow = inWindow;
        }

        public void CommitAll()
        {
            foreach (Action commit in commitActions)
                commit();
        }

        private void AddElement(string id, ConfigElement ce)
        {
            // [DAI fix] a duplicate id used to throw and abort the import; the later one now wins
            elements[id] = ce;
        }

        private static bool TryParseFloat(string text, out float value)
        {
            // [DAI fix] accept both the Windows region format and the plain "1.5" format
            return float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                || float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public override void AddIntElement(string id, string name, int defValue, int minValue, int maxValue)
        {
            ConfigElement<int> ce = new ConfigElement<int>(id, name, defValue, minValue, maxValue);
            AddElement(id, ce);

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = defValue.ToString(),
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

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = defValue.ToString("F3"),
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

            Label lbl = new Label {Content = name};

            CheckBox cb = new CheckBox
            {
                IsChecked = defValue,
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

            // [DAI fix] the dropdown position used to be passed to the script as the value, which is
            // only correct when the enum's values are 0, 1, 2... in order. Map positions to real values.
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

            // the default is normally an enum value; fall back to treating it as a position
            int selectedIndex = Array.IndexOf(values, defValue);
            if (selectedIndex < 0 && defValue >= 0 && defValue < values.Length)
                selectedIndex = defValue;
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

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = defValue ?? "",
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
            // [DAI fix] an unknown name used to crash the script; return 0 like the original
            // DAI scripting interface does when nothing is set up
            ConfigElement elem = config.GetElement(name);
            if (elem == null)
            {
                log.Add("Script asked for unknown setting \"" + name + "\", using 0");
                return 0;
            }
            return elem.CurrentValue;
        }

        // [DAI fix] script log output used to be thrown away; it now goes to the Mod Manager log
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
        /// <summary>
        /// [DAI fix] True only if the user pressed Save and the script ran successfully.
        /// Closing the window any other way leaves this false so the import can be cancelled
        /// (it used to import the mod with the script never having run).
        /// </summary>
        public bool ScriptApplied { get; private set; }

        /// <summary>
        /// [DAI fix] Set if the script threw an error while running.
        /// </summary>
        public string ScriptError { get; private set; }

        private DAIMod.ModScript modScript;
        private FrostyScripting scripting;

        public ConfigWindow(string code, List<byte[]> inResources, List<bool> inStates)
        {
            InitializeComponent();

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            // [DAI fix] reference assemblies by their full path so they are always found no matter
            // what the working directory is, and include the common framework libraries old scripts
            // may use ("System.Linq.dll" is not a .NET Framework assembly; Linq lives in System.Core)
            HashSet<string> references = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                typeof(Uri).Assembly.Location,                          // System.dll
                typeof(Enumerable).Assembly.Location,                   // System.Core.dll
                typeof(System.Xml.XmlDocument).Assembly.Location,       // System.Xml.dll
                typeof(System.Xml.Linq.XDocument).Assembly.Location,    // System.Xml.Linq.dll
                typeof(System.Data.DataTable).Assembly.Location,        // System.Data.dll
                typeof(System.Drawing.Color).Assembly.Location,         // System.Drawing.dll
                typeof(DAIMod.ModScript).Assembly.Location              // FrostyModSupport.dll
            };
            foreach (string reference in references)
            {
                if (!string.IsNullOrEmpty(reference))
                    parameters.ReferencedAssemblies.Add(reference);
            }

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);

            // [DAI fix] compile errors used to surface as an unhandled exception that killed the import
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

            scripting = new FrostyScripting(new FrostyConfigElementsList(this), inResources, inStates);
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

        public List<bool> GetResourceStates() => scripting.resourceState;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // make sure a value that is still being typed is included
            scripting.config.CommitAll();

            try
            {
                modScript.RunScript();
                ScriptApplied = true;
            }
            catch (Exception ex)
            {
                // [DAI fix] a script error used to crash the Mod Manager
                ScriptError = "The mod's configuration script failed while running: " + ex.Message;
            }

            foreach (string line in scripting.log)
                App.Logger.Log("[DAI script] " + line);

            Close();
        }
    }
}
