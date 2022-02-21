using Frosty.Controls;
using FrostySdk.IO;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

        public FrostyConfigElementsList(ConfigWindow inWindow)
        {
            theWindow = inWindow;
        }

        public override void AddIntElement(string id, string name, int defValue, int minValue, int maxValue)
        {
            ConfigElement<int> ce = new ConfigElement<int>(id, name, defValue, minValue, maxValue);
            elements.Add(id, ce);

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

            tb.LostFocus += (o, e) =>
            {
                if (int.TryParse(tb.Text, out int newValue))
                {
                    if (newValue <= minValue || newValue >= maxValue)
                    {
                        if (newValue <= minValue)
                            newValue = minValue;
                        else if (newValue >= maxValue)
                            newValue = maxValue;
                    }
                }
                else
                {
                    newValue = defValue;
                    FrostyMessageBox.Show("Invalid value entered", "Frosty Mod Manager");
                }

                tb.Text = newValue.ToString();
                ce.CurrentValue = newValue;
            };
            tb.KeyDown += (o, e) => { if (e.Key == Key.Enter) { tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); } };

            theWindow.valuesStackPanel.Children.Add(tb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddFloatElement(string id, string name, float defValue, float minValue, float maxValue)
        {
            ConfigElement<float> ce = new ConfigElement<float>(id, name, defValue, minValue, maxValue);
            elements.Add(id, ce);

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

            tb.LostFocus += (o, e) =>
            {
                if (float.TryParse(tb.Text, out float newValue))
                {
                    if (newValue <= minValue || newValue >= maxValue)
                    {
                        if (newValue <= minValue)
                            newValue = minValue;
                        else if (newValue >= maxValue)
                            newValue = maxValue;
                    }
                }
                else
                {
                    newValue = defValue;
                    FrostyMessageBox.Show("Invalid value entered", "Frosty Mod Manager");
                }

                tb.Text = newValue.ToString("F3");
                ce.CurrentValue = newValue;
            };
            tb.KeyDown += (o, e) => { if (e.Key == Key.Enter) { tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); } };

            theWindow.valuesStackPanel.Children.Add(tb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddBoolElement(string id, string name, bool defValue)
        {
            ConfigElement<bool> ce = new ConfigElement<bool>(id, name, defValue, false, true);
            elements.Add(id, ce);

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
            elements.Add(id, ce);

            Label lbl = new Label {Content = name};

            ComboBox cb = new ComboBox
            {
                ItemsSource = Enum.GetNames(enumType),
                SelectedIndex = defValue,
                Padding = new Thickness(4, 0, 0, 0),
                Margin = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 24
            };

            cb.SelectionChanged += (o, e) =>
            {
                ce.CurrentValue = cb.SelectedIndex;
            };

            theWindow.valuesStackPanel.Children.Add(cb);
            theWindow.namesStackPanel.Children.Add(lbl);
        }

        public override void AddStringElement(string id, string name, string defValue)
        {
            StringConfigElement ce = new StringConfigElement(id, name, defValue);
            elements.Add(id, ce);

            Label lbl = new Label {Content = name};

            TextBox tb = new TextBox
            {
                Text = defValue,
                Height = 24,
                Margin = new Thickness(1),
                Padding = new Thickness(2, 0, 0, 0),
                BorderThickness = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            tb.LostFocus += (o, e) => { ce.CurrentValue = tb.Text; };
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

        public FrostyScripting(FrostyConfigElementsList inConfig, List<byte[]> inResources, List<bool> inStates)
        {
            config = inConfig;
            resources = inResources;
            resourceState = inStates;
        }

        public object GetConfigParam(string name) => config.GetElement(name).CurrentValue;

        public void LogLn(string line) { } // do nothing

        public byte[] GetResourceData(int index)
        {
            if (index >= resources.Count)
                return null;

            using (CasReader reader = new CasReader(new MemoryStream(resources[index])))
                return reader.Read();
        }

        public void SetResourceData(int index, byte[] data) => resources[index] = FrostySdk.Utils.CompressFile(data);

        public void SetResourceEnabled(int index, bool enabled) => resourceState[index] = enabled;
        
    }

    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : FrostyDockableWindow
    {
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
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Linq.dll");
            parameters.ReferencedAssemblies.Add("FrostyModSupport.dll");
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);

            foreach (Type type in results.CompiledAssembly.GetTypes())
            {
                if (type.GetInterface("DAIMod.ModScript") != null)
                {
                    modScript = (DAIMod.ModScript)Activator.CreateInstance(type);
                    break;
                }
            }

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
                sb.AppendLine(elem.ToString() + " " + elem.CurrentValue.ToString());
            }

            return sb.ToString();
        }

        public List<bool> GetResourceStates() => scripting.resourceState;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            modScript.RunScript();
            Close();
        }
    }
}
