using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Controls.Editors;
using Frosty.Core.Misc;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using FrostySdk;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Frosty.Core.Windows
{
    public class OptionsDisplayNameToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type valueType = value.GetType();
            var attr = valueType.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                return attr.Name;
            return valueType.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FrostyLocalizationLanguageDataEditor : FrostyCustomComboDataEditor<string, string>
    {
    }

    public abstract class BaseOptionsData : OptionsExtension
    {
        [Category("Update Checking")]
        [DisplayName("Check for Updates")]
        [Description("Check Github for Frosty updates on startup")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool UpdateCheck { get; set; } = true;

        [Category("Update Checking")]
        [DisplayName("Check for Prerelease Updates")]
        [Description("Check Github for Frosty Alpha and Beta updates on startup")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool UpdateCheckPrerelease { get; set; } = false;

        public override void Load()
        {
            base.Load();
            
            UpdateCheck = Config.Get<bool>("UpdateCheck", true);
            UpdateCheckPrerelease = Config.Get<bool>("UpdateCheckPrerelease", false);
        }

        public override void Save()
        {
            base.Save();
            
            Config.Add("UpdateCheck", UpdateCheck);
            Config.Add("UpdateCheckPrerelease", UpdateCheckPrerelease);
        }
    }
    
    [DisplayName("Editor Options")]
    public class EditorOptionsData : BaseOptionsData
    {
        [Category("Localization")]
        [Description("Selects which localized language files to read from the game files.")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        [Editor(typeof(FrostyLocalizationLanguageDataEditor))]
        public CustomComboData<string, string> Language { get; set; }

        [Category("Autosave")]
        [DisplayName("Enabled")]
        [Description("Enables autosaving for projects.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool AutosaveEnabled { get; set; } = true;
        [Category("Autosave")]
        [DisplayName("Period")]
        [Description("How often to autosave the project. Value is defined in minutes.")]
        [EbxFieldMeta(EbxFieldType.Int32)]
        [DependsOn("AutosaveEnabled")]
        public int AutosavePeriod { get; set; } = 5;
        [Category("Autosave")]
        [DisplayName("Max Saves")]
        [Description("Maximum number of autosave files to generate per project.")]
        [EbxFieldMeta(EbxFieldType.Int32)]
        [DependsOn("AutosaveEnabled")]
        public int AutosaveMaxSaves { get; set; } = 10;

        [Category("Text Editor")]
        [DisplayName("Tab Size")]
        [Description("Size of opened tabs in the Editor.")]
        [EbxFieldMeta(EbxFieldType.Int32)]
        public int TextEditorTabSize { get; set; } = 4;
        [Category("Text Editor")]
        [DisplayName("Indent on Enter")]
        [Description("Indents text upon pressing the Enter key.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool TextEditorIndentOnEnter { get; set; } = false;

        [Category("Discord RPC")]
        [DisplayName("Enabled")]
        [Description("Turns on rich presence for Discord.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool DiscordEnabled { get; set; } = false;

        [Category("Mod Settings")]
        [DisplayName("Default Author")]
        [Description("Sets the default author for a mod.")]
        [EbxFieldMeta(EbxFieldType.String)]
        public string ModSettingsAuthor { get; set; } = "";

        [Category("Asset")]
        [DisplayName("Display Module in Class Id")]
        [Description("Determines whether a class's default Id, when viewed in the property grid, is prepended with the module name of that class.\r\n\r\nTrue: Entity.MathEntityData\r\nFalse: MathEntityData")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool AssetDisplayModuleInId { get; set; } = true;

        [Category("Editor")]
        [DisplayName("Remember Profile Choice")]
        [Description("If true, the last chosen profile will be automatically loaded when starting the Editor.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool RememberChoice { get; set; } = false;

        [Category("Editor")]
        [DisplayName("Set as Default Installation")]
        [Description("Use this installation for .fbproject files.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool DefaultInstallation { get; set; } = false;

        public override void Load()
        {
            base.Load();
            
            if (ProfilesLibrary.HasLoadedProfile)
            {
                List<string> langs = GetLocalizedLanguages();
                Language = new CustomComboData<string, string>(langs, langs) { SelectedIndex = langs.IndexOf(Config.Get<string>("Language", "English", ConfigScope.Game)) };
            }
            else
            {
                List<string> emptyLangs = new List<string>();
                Language = new CustomComboData<string, string>(emptyLangs, emptyLangs);
            }

            AutosaveEnabled = Config.Get<bool>("AutosaveEnabled", true);
            AutosavePeriod = Config.Get<int>("AutosavePeriod", 5);
            AutosaveMaxSaves = Config.Get<int>("AutosaveMaxCount", 10);

            TextEditorTabSize = Config.Get<int>("TextEditorTabSize", 4);
            TextEditorIndentOnEnter = Config.Get<bool>("TextEditorIndentOnEnter", false);

            DiscordEnabled = Config.Get<bool>("DiscordRPCEnabled", false);
            ModSettingsAuthor = Config.Get<string>("ModAuthor", "");

            AssetDisplayModuleInId = Config.Get<bool>("DisplayModuleInId", false);
            RememberChoice = Config.Get<bool>("UseDefaultProfile", false);

            //Checks the registry for the current association instead of loading from config
            string KeyName = "frostyproject";
            string OpenWith = Assembly.GetEntryAssembly().Location;

            if (Registry.CurrentUser.OpenSubKey("Software\\Classes\\" + KeyName) != null) {
                string openCommand = (string)Registry.CurrentUser.OpenSubKey("Software\\Classes\\" + KeyName).OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command").GetValue("");
                if (openCommand.Contains(OpenWith)) DefaultInstallation = true;
            }
        }

        public override void Save()
        {
            base.Save();
            
            Config.Add("AutosaveEnabled", AutosaveEnabled);
            Config.Add("AutosavePeriod", AutosavePeriod);
            Config.Add("AutosaveMaxCount", AutosaveMaxSaves);

            Config.Add("TextEditorTabSize", TextEditorTabSize);
            Config.Add("TextEditorIndentOnEnter", TextEditorIndentOnEnter);

            Config.Add("DiscordRPCEnabled", DiscordEnabled);
            Config.Add("ModAuthor", ModSettingsAuthor);
            Config.Add("DisplayModuleInId", AssetDisplayModuleInId);
            Config.Add("UseDefaultProfile", RememberChoice);

            if (RememberChoice)
                Config.Add("DefaultProfile", ProfilesLibrary.ProfileName);
            else
                Config.Remove("DefaultProfile");

            Config.Add("Language", Language.SelectedName, ConfigScope.Game);

            Config.Save();

            LocalizedStringDatabase.Current.Initialize();

            //Create file association if enabled
            if (DefaultInstallation) {
                string Extension = ".fbproject";
                string KeyName = "frostyproject";
                string OpenWith = Assembly.GetEntryAssembly().Location;
                string FileDescription = "Frosty Project";

                RegistryKey BaseKey;
                RegistryKey OpenMethod;
                RegistryKey Shell;
                RegistryKey CurrentUser;

                BaseKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + Extension);
                BaseKey.SetValue("", KeyName);

                OpenMethod = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + KeyName);
                OpenMethod.SetValue("", FileDescription);
                OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + OpenWith + "\",0");
                Shell = OpenMethod.CreateSubKey("shell");
                Shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
                Shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
                BaseKey.Close();
                OpenMethod.Close();
                Shell.Close();

                CurrentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + Extension, true);
                CurrentUser.DeleteSubKey("UserChoice", false);
                CurrentUser.Close();

                // Tell explorer the file association has been changed
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public override bool Validate()
        {
            return true;
        }
        
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private List<string> GetLocalizedLanguages()
        {
            List<string> languages = new List<string>();
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx("LocalizationAsset"))
            {
                // read master localization asset
                dynamic localizationAsset = App.AssetManager.GetEbx(entry).RootObject;

                // iterate through localized texts
                foreach (PointerRef pointer in localizationAsset.LocalizedTexts)
                {
                    EbxAssetEntry textEntry = App.AssetManager.GetEbxEntry(pointer.External.FileGuid);
                    if (textEntry == null)
                        continue;

                    // read localized text asset
                    dynamic localizedText = App.AssetManager.GetEbx(textEntry).RootObject;

                    string lang = localizedText.Language.ToString();
                    lang = lang.Replace("LanguageFormat_", "");

                    languages.Add(lang);
                }
            }

            if (languages.Count == 0)
                languages.Add("English");

            return languages;
        }
    }

    [DisplayName("Mod Manager Options")]
    public class ModManagerOptionsData : BaseOptionsData
    {
        [Category("Manager")]
        [DisplayName("Remember Profile Choice")]
        [Description("If true, the last chosen profile will be automatically loaded when starting the Mod Manager.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool RememberChoice { get; set; } = false;

        [Category("Manager")]
        [DisplayName("Command Line Arguments")]
        [Description("Command line arguments to run on launch.")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public string CommandLineArgs { get; set; } = "";

        public override void Load()
        {
            base.Load();
            
            RememberChoice = Config.Get<bool>("UseDefaultProfile", false);
            CommandLineArgs = Config.Get<string>("CommandLineArgs", "", ConfigScope.Game);
        }

        public override void Save()
        {
            base.Save();
            
            Config.Add("UseDefaultProfile", RememberChoice);
            Config.Add("CommandLineArgs", CommandLineArgs, ConfigScope.Game);

            if (RememberChoice)
                Config.Add("DefaultProfile", ProfilesLibrary.ProfileName);
            else
                Config.Remove("DefaultProfile");

            Config.Save();
        }

        public override bool Validate()
        {
            return true;
        }
    }

    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : FrostyDockableWindow
    {
        private List<OptionsExtension> optionDataList = new List<OptionsExtension>();

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void OptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Add default settings
            if (App.PluginManager.IsManagerType(PluginManagerType.Editor))
                optionDataList.Add(new EditorOptionsData());
            else
                optionDataList.Add(new ModManagerOptionsData());

            // Add plugin settings
            foreach (var optionExtType in App.PluginManager.OptionsExtensions)
                optionDataList.Add((OptionsExtension)Activator.CreateInstance(optionExtType));

            foreach (var optionData in optionDataList)
            {
                optionData.Load();

                FrostyTabItem ti = new FrostyTabItem
                {
                    Header = new OptionsDisplayNameToStringConverter().Convert(optionData, null, null, null),
                    Content = new FrostyPropertyGrid() {Object = optionData}
                };
                optionsTabControl.Items.Add(ti);
            }

            optionsTabControl.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                foreach (var optionData in optionDataList)
                    optionData.Save();

                Config.Save();

                Close();
            }
        }

        private bool Validate()
        {
            foreach (var optionData in optionDataList)
            {
                if (!optionData.Validate())
                    return false;
            }
            return true;
        }
    }
}
