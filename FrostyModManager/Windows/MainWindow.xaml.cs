using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Frosty.Controls;
using System.IO;
using System.Globalization;
using FrostySdk;
using FrostySdk.Interfaces;
using Microsoft.Win32;
using FrostySdk.IO;
using Frosty.ModSupport;
using FrostyModManager.Controls;
using FrostyModManager.Compression;
using System.Text;
using System.ComponentModel;
using Frosty.Hash;
using System.Threading;
using Frosty.Core.Mod;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk.Managers;
using Frosty.Core.IO;
using FrostyCore;
using Frosty.Core.Controls;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Media;

namespace FrostyModManager
{
    public static class Helpers
    {
        public static DependencyObject FindVisualAncestor(this DependencyObject wpfObject, Predicate<DependencyObject> condition)
        {
            while (wpfObject != null)
            {
                if (condition(wpfObject))
                {
                    return wpfObject;
                }

                wpfObject = VisualTreeHelper.GetParent(wpfObject);
            }

            return null;
        }
    }

    public class ModDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IFrostyMod mod = (IFrostyMod)value;
            if (!mod.HasWarnings)
                return mod.ModDetails.Description;

            string desc = "";
            foreach (string warning in mod.Warnings)
                desc += "(WARNING: " + warning + ")\n";
            desc += "\n";
            desc += mod.ModDetails.Description;
            return desc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModPrimaryActionTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModResourceInfo mri = (ModResourceInfo)value;
            List<ModAction> mods = (List<ModAction>)mri.Mods;
            string modName = (string)parameter;

            int index = mods.FindIndex((ModAction a) => a.Name == modName);
            if (index != -1)
            {
                var modAction = mods[index];
                switch (modAction.PrimaryAction)
                {
                    case ModPrimaryActionType.None: return null;
                    case ModPrimaryActionType.Modify: return (index == mri.FirstModToModifyIndex) ? "Resource is initially modified by this mod" : "Resource is replaced by this mod";
                    case ModPrimaryActionType.Add: return (index == mri.FirstModToModifyIndex) ? "Resource is initially added by this mod" : "Resource is replaced by this mod";
                    case ModPrimaryActionType.Merge: return (index == mri.FirstModToModifyIndex) ? "Resource is initially modified by this mod" : "Resource is merged by this mod";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModSecondaryActionTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModResourceInfo mri = (ModResourceInfo)value;
            List<ModAction> mods = (List<ModAction>)mri.Mods;
            string modName = (string)parameter;

            int index = mods.FindIndex((ModAction a) => a.Name == modName);
            if (index != -1)
            {
                var modAction = mods[index];
                if (modAction.SecondaryAction == ModSecondaryActionType.AddToBundle)
                    return "Resource is added to other bundle(s) by this mod";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModPrimaryActionConverter : IValueConverter
    {
        private static ImageSource blankSource = null;
        private static ImageSource primaryActionModifySource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyModManager;component/Images/PrimaryActionModify.png") as ImageSource;
        private static ImageSource primaryActionReplaceSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyModManager;component/Images/PrimaryActionReplace.png") as ImageSource;
        private static ImageSource primaryActionAddSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyModManager;component/Images/PrimaryActionAdd.png") as ImageSource;
        private static ImageSource primaryActionMergeSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyModManager;component/Images/PrimaryActionMerge.png") as ImageSource;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModResourceInfo mri = (ModResourceInfo)value;
            List<ModAction> mods = (List<ModAction>)mri.Mods;
            string modName = (string)parameter;

            int index = mods.FindIndex((ModAction a) => a.Name == modName);
            if (index != -1)
            {
                var modAction = mods[index];
                switch (modAction.PrimaryAction)
                {
                    case ModPrimaryActionType.None: return blankSource;
                    case ModPrimaryActionType.Modify: return (index == mri.FirstModToModifyIndex) ? primaryActionModifySource : primaryActionReplaceSource;
                    case ModPrimaryActionType.Add: return (index == mri.FirstModToModifyIndex) ? primaryActionAddSource : primaryActionReplaceSource;
                    case ModPrimaryActionType.Merge: return (index == mri.FirstModToModifyIndex) ? primaryActionModifySource : primaryActionMergeSource;
                }
            }

            return blankSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModSecondaryActionConverter : IValueConverter
    {
        private static readonly ImageSource BlankSource = null;
        private static readonly ImageSource SecondaryActionAddSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyModManager;component/Images/SecondaryActionAdd.png") as ImageSource;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModResourceInfo mri = (ModResourceInfo)value;
            List<ModAction> mods = (List<ModAction>)mri.Mods;
            string modName = (string)parameter;

            int index = mods.FindIndex((ModAction a) => a.Name == modName);
            if (index != -1)
            {
                var modAction = mods[index];
                if (modAction.SecondaryAction == ModSecondaryActionType.AddToBundle)
                    return SecondaryActionAddSource;
            }

            return BlankSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum ModPrimaryActionType
    {
        None,
        Modify,
        Add,
        Merge
    }
    public enum ModSecondaryActionType
    {
        None,
        AddToBundle
    }

    public class ModAction
    {
        public ModPrimaryActionType PrimaryAction;
        public ModSecondaryActionType SecondaryAction;
        public string Name;
    }

    public class ModResourceInfo
    {
        public string Name { get; }
        public string Type { get; }

        public IEnumerable<ModAction> Mods => mods;
        public int ModCount => mods.Count;
        public int FirstModToModifyIndex { get; private set; } = -1;

        private readonly int nameHash;
        private List<ModAction> mods = new List<ModAction>();
        private List<int> addBundles = new List<int>();

        public ModResourceInfo(string n, string t)
        {
            Name = n;
            Type = t;
            nameHash = Fnv1.HashString(t + "/" + n);
        }

        public void AddMod(string m, ModPrimaryActionType primaryAction, IEnumerable<int> modAddBundles)
        {
            bool isAdded = false;
            if (modAddBundles != null)
            {
                foreach (int addBundle in modAddBundles)
                {
                    if (!addBundles.Contains(addBundle))
                        isAdded = true;
                    addBundles.Add(addBundle);
                }
            }

            mods.Add(new ModAction() { Name = m, PrimaryAction = primaryAction, SecondaryAction = (isAdded) ? ModSecondaryActionType.AddToBundle : ModSecondaryActionType.None });
            if (FirstModToModifyIndex == -1)
            {
                if (primaryAction != ModPrimaryActionType.None)
                    FirstModToModifyIndex = mods.Count - 1;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is string s)
            {
                int hash = Fnv1.HashString(s);
                return hash == nameHash;
            }
            return base.Equals(obj);
        }
    }

    public class PackManifest
    {
        public string name;

        public string managerVersion;
        public int version;
        public List<string> mods;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FrostyWindow
    {
        private List<IFrostyMod> availableMods = new List<IFrostyMod>();
        private List<FrostyPack> packs = new List<FrostyPack>();
        private FrostyPack selectedPack;
        private FileSystem fs;
        private DirectoryInfo modsDir = new DirectoryInfo(Path.Combine("Mods", ProfilesLibrary.ProfileName));

        private static int manifestVersion = 1;

        public MainWindow()
        {
            InitializeComponent();
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();

            tabContent.HeaderControl = tabControl;
            availableModsTabContent.HeaderControl = availableModsTabControl;
        }

        private void FrostyWindow_FrostyLoaded(object sender, EventArgs e)
        {
            (App.Logger as FrostyLogger).AddBinding(tb, TextBox.TextProperty);

            string gamePath = Config.Get<string>("GamePath", "", ConfigScope.Game);
            //string gamePath = Config.Get<string>("Init", "GamePath", "");
            //string gameProfileName = Config.Get<string>("Init", "Profile", "");

            //if (!ProfilesLibrary.Initialize(gameProfileName))
            //{
            //    FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Mod Manager");
            //    Closing -= FrostyWindow_Closing;
            //    Close();
            //    return;
            //}

            if (!ProfilesLibrary.EnableExecution)
            {
                FrostyMessageBox.Show("The selected profile is a read-only profile, and therefore cannot be loaded in the mod manager", "Frosty Mod Manager");
                Closing -= FrostyWindow_Closing;
                Close();
                return;
            }

            fs = new FileSystem(gamePath);
            //fs = new FileSystem(Config.Get<string>("Init", "GamePath", ""));
            foreach (FileSystemSource source in ProfilesLibrary.Sources)
                fs.AddSource(source.Path, source.SubDirs);
            fs.Initialize();

            Config.Save();
            //Config.Save(App.configFilename);
            Title = "Frosty Mod Manager - " + App.Version + " (" + ProfilesLibrary.DisplayName + ")";

            TypeLibrary.Initialize();
            App.PluginManager.Initialize();

            LoadMenuExtensions();

            if (Directory.Exists(Config.Get<string>("CustomModsDirectory", "")))
            {
                modsDir = new DirectoryInfo(Path.Combine(Config.Get<string>("CustomModsDirectory", ""), ProfilesLibrary.ProfileName));
            }
            else
            {
                App.Logger.Log("Custom Mods Directory does not exist, using default instead");
            }

            FrostyTaskWindow.Show("Loading Mods", "", (task) =>
            {
                if (!modsDir.Exists)
                    Directory.CreateDirectory(modsDir.FullName);

                int currentMod = 0;
                int totalMods = modsDir.EnumerateFiles().Count();

                // load mods
                Parallel.ForEach(modsDir.EnumerateFiles(), fi =>
                {
                    if (fi.Extension == ".fbmod")
                    {
                        int retCode = 0;
                        using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                            retCode = VerifyMod(stream);

                        if (retCode >= 0)
                        {
                            try
                            {
                                // all good
                                AddMod(fi.FullName, (retCode & 0x8000) != 0 ? 1 : 0);
                            }
                            catch (FrostyModLoadException)
                            {
                                // failed to load for whatever reason
                                File.Delete(fi.FullName);
                                File.Delete(fi.FullName.Replace(".fbmod", "_01.archive"));
                            }
                        }
                        else if (retCode == -3)
                        {
                            // bad mod. delete it
                            File.Delete(fi.FullName);
                            File.Delete(fi.FullName.Replace(".fbmod", "_01.archive"));
                        }
                    }
                    task.TaskLogger.Log("progress:" + currentMod++ / (float)totalMods * 100d);
                });
                // load collections
                Parallel.ForEach(modsDir.EnumerateFiles(), fi =>
                {
                    if (fi.Extension == ".fbcollection")
                    {
                        AddCollection(fi.FullName, 0);
                    }
                    task.TaskLogger.Log("progress:" + currentMod++ / (float)totalMods * 100d);
                });
            });
            availableMods = availableMods.OrderBy(o => o.Filename).ToList();
            availableModsList.ItemsSource = availableMods;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ModDetails.Category", null, StringComparison.OrdinalIgnoreCase);
            view.GroupDescriptions.Add(groupDescription);

            foreach (string packName in Config.EnumerateKeys(ConfigScope.Pack))
            //foreach (string profileName in Config.EnumerateKeys("Profiles"))
            {
                string values = Config.Get(packName, "", ConfigScope.Pack);
                //string values = Config.Get<string>("Profiles", profileName, "");
                string[] valuesArray = values.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                FrostyPack pack = new FrostyPack(packName);
                packs.Add(pack);

                for (int i = 0; i < valuesArray.Length; i++)
                {
                    string[] modEnabledPair = valuesArray[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string backupFileName = modEnabledPair[0];
                    bool isEnabled = bool.Parse(modEnabledPair[1]);

                    IFrostyMod mod = availableMods.Find((IFrostyMod a) => a.Filename == modEnabledPair[0]);
                    if (mod == null)
                    {
                        List<IFrostyMod> collections = availableMods.FindAll((IFrostyMod a) => a is FrostyModCollection);
                        foreach (FrostyModCollection collection in collections)
                        {
                            mod = collection.Mods.Find((FrostyMod a) => a.Filename == modEnabledPair[0]);
                            if (mod != null)
                                break;
                        }
                    }

                    pack.AddMod(mod, isEnabled, backupFileName);
                }
            }

            if (packs.Count == 0)
                AddPack("Default");
            packsComboBox.ItemsSource = packs;

            if (App.LaunchGameImmediately)
            {
                int index = packs.FindIndex((FrostyPack a) => a.Name.Equals(App.LaunchProfile, StringComparison.OrdinalIgnoreCase));
                if (index == -1)
                {
                    FrostyMessageBox.Show(string.Format("Unable to find pack with name {0}. Launch request cancelled", App.LaunchProfile), "Frosty Mod Manager");
                    App.LaunchGameImmediately = false;
                }
                else
                {
                    packsComboBox.SelectedIndex = index;
                    launchButton_Click(this, new RoutedEventArgs());
                    return;
                }
            }

            if (toolsMenuItem.Items.Count != 0)
                toolsMenuItem.Items.Add(new Separator());

            MenuItem optionsMenuItem = new MenuItem()
            {
                Header = "Options",
                Icon = new Image() { Source = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Settings.png") as ImageSource },
            };
            optionsMenuItem.Click += optionsMenuItem_Click;
            toolsMenuItem.Items.Add(optionsMenuItem);

            string selectedProfileName = Config.Get<string>("SelectedPack", "", ConfigScope.Game);
            //string selectedProfileName = Config.Get<string>("Application", "SelectedProfile", "");
            int selectedIndex = 0;
            if (selectedProfileName != null)
            {
                selectedIndex = packs.FindIndex((FrostyPack a) => a.Name == selectedProfileName);
                if (selectedIndex == -1)
                    selectedIndex = 0;
            }
            packsComboBox.SelectedIndex = selectedIndex;

            if (Config.Get("CollapseCategories", false))
            {
            }

            LoadedPluginsList.ItemsSource = App.PluginManager.Plugins;

            if (Config.Get("ApplyModOrder", "List") == "List")
            {
                orderComboBox.SelectedIndex = 0;
            }
            else if (Config.Get("ApplyModOrder", "List") == "Priority")
            {
                orderComboBox.SelectedIndex = 1;
            }

            GC.Collect();
        }

        private void addProfileButton_Click(object sender, RoutedEventArgs e)
        {
            AddProfileWindow win = new AddProfileWindow();
            win.ShowDialog();

            if (win.DialogResult == true)
            {
                AddPack(win.ProfileName);
            }
        }

        private void packsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPack = packsComboBox.SelectedItem as FrostyPack;
            appliedModsList.ItemsSource = selectedPack?.AppliedMods;

            if (selectedPack == null)
                return;

            selectedPack.AppliedModsUpdated += SelectedProfile_AppliedModsUpdated;
            selectedPack.Refresh();

            Config.Add("SelectedPack", selectedPack.Name, ConfigScope.Game);
            //Config.Add("Application", "SelectedProfile", selectedProfile.Name);
        }

        private void removeProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (packsComboBox.Items.Count == 1)
            {
                FrostyMessageBox.Show("There must be at least one active pack", "Frosty Mod Manager");
                return;
            }

            if (FrostyMessageBox.Show("Are you sure you want to delete this pack?", "Frosty Mod Manager", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Config.Remove(selectedPack.Name, ConfigScope.Pack);
                //Config.Remove("Profiles", selectedProfile.Name);
                packs.Remove(selectedPack);

                DirectoryInfo di = new DirectoryInfo(fs.BasePath + "ModData\\" + selectedPack.Name);
                if (di.Exists)
                    di.Delete(true);


                packsComboBox.Items.Refresh();
                packsComboBox.SelectedIndex = 0;
            }
        }

        private void packRename_Click(object sender, RoutedEventArgs e) {

            AddProfileWindow win = new AddProfileWindow("Rename Pack");
            win.ShowDialog();

            if (win.DialogResult == true) {
                string newPackName = win.ProfileName;
                var oldPack = selectedPack;

                FrostyPack existingPack = packs.Find((FrostyPack a) => {
                    return a.Name.CompareTo(newPackName) == 0;
                });

                if (existingPack == null) {
                    FrostyPack newPack = new FrostyPack(newPackName);
                    foreach (var mod in oldPack.AppliedMods) {
                        newPack.AppliedMods.Add(mod);
                    }

                    Config.Add(newPackName, ConfigScope.Pack);
                    Config.Remove(oldPack.Name, ConfigScope.Pack);

                    packs.Add(newPack);
                    packs.Remove(oldPack);

                    packsComboBox.Items.Refresh();
                    packsComboBox.SelectedItem = newPack;
                }
                else FrostyMessageBox.Show("A pack with the same name already exists", "Frosty Mod Manager");

            }
        }

        private void packDuplicate_Click(object sender, RoutedEventArgs e) {

            AddProfileWindow win = new AddProfileWindow("Duplicate Pack");
            win.ShowDialog();

            if (win.DialogResult == true) {
                string newPackName = win.ProfileName;
                var oldPack = selectedPack;

                FrostyPack existingPack = packs.Find((FrostyPack a) => {
                    return a.Name.CompareTo(newPackName) == 0;
                });

                if (existingPack == null) {
                    Config.Add(newPackName, ConfigScope.Pack);

                    FrostyPack newPack = new FrostyPack(newPackName);
                    foreach (var mod in oldPack.AppliedMods) {
                        newPack.AppliedMods.Add(mod);
                    }

                    packs.Add(newPack);

                    packsComboBox.Items.Refresh();
                    packsComboBox.SelectedItem = newPack;
                }
                else FrostyMessageBox.Show("A pack with the same name already exists", "Frosty Mod Manager");
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = appliedModsList.SelectedIndex;

            foreach (FrostyAppliedMod mod in appliedModsList.SelectedItems)
                selectedPack.RemoveMod(mod);

            appliedModsList.Items.Refresh();

            appliedModsList.SelectedIndex = selectedIndex;
            updateAppliedModButtons();
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (orderComboBox.SelectedIndex == 0)
            {
                for (int i = 0; i < (Keyboard.IsKeyDown(Key.LeftShift) ? 4 : 1); i++)
                {
                    selectedPack.MoveModsUp(appliedModsList.SelectedItems);
                }

                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    selectedPack.MoveModsTop(appliedModsList.SelectedItems);
                }
            }
            else if (orderComboBox.SelectedIndex == 1)
            {
                for (int i = 0; i < (Keyboard.IsKeyDown(Key.LeftShift) ? 4 : 1); i++)
                {
                    selectedPack.MoveModsDown(appliedModsList.SelectedItems);
                }

                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    selectedPack.MoveModsBottom(appliedModsList.SelectedItems);
                }
            }
            appliedModsList.Items.Refresh();

            updateAppliedModButtons();
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (orderComboBox.SelectedIndex == 0)
            {
                for (int i = 0; i < (Keyboard.IsKeyDown(Key.LeftShift) ? 4 : 1); i++)
                {
                    selectedPack.MoveModsDown(appliedModsList.SelectedItems);
                }

                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    selectedPack.MoveModsBottom(appliedModsList.SelectedItems);
                }
            }
            else if (orderComboBox.SelectedIndex == 1)
            {
                for (int i = 0; i < (Keyboard.IsKeyDown(Key.LeftShift) ? 4 : 1); i++)
                {
                    selectedPack.MoveModsUp(appliedModsList.SelectedItems);
                }

                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    selectedPack.MoveModsTop(appliedModsList.SelectedItems);
                }
            }
            appliedModsList.Items.Refresh();

            updateAppliedModButtons();
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Save();
            //Config.Save(App.configFilename);

            // initialize
            Frosty.Core.App.FileSystem = new FileSystem(Config.Get<string>("GamePath", "", ConfigScope.Game));
            //FileSystem fs = new FileSystem(Config.Get<string>("Init", "GamePath", ""));
            foreach (FileSystemSource source in ProfilesLibrary.Sources)
                Frosty.Core.App.FileSystem.AddSource(source.Path, source.SubDirs);
            Frosty.Core.App.FileSystem.Initialize();

            // Set selected pack
            App.SelectedPack = selectedPack.Name;

            // get all applied mods
            List<string> modPaths = new List<string>();
            foreach (FrostyAppliedMod mod in selectedPack.AppliedMods)
            {
                if (mod.IsFound && mod.IsEnabled)
                    modPaths.Add(mod.Mod.Filename);
            }

            // combine stored args with launch args
            string additionalArgs = Config.Get<string>("CommandLineArgs", "", ConfigScope.Game) + " ";
            //string additionalArgs = Config.Get<string>("Init", "AdditionalArgs", "") + " ";
            additionalArgs += App.LaunchArgs;

            // setup ability to cancel the process
            CancellationTokenSource cancelToken = new CancellationTokenSource();

            // launch
            int retCode = 0;
            FrostyTaskWindow.Show("Launching", "", (task) =>
            {
                try
                {
                    foreach (var executionAction in App.PluginManager.ExecutionActions)
                        executionAction.PreLaunchAction(task.TaskLogger, PluginManagerType.ModManager, cancelToken.Token);

                    FrostyModExecutor modExecutor = new FrostyModExecutor();
                    retCode = modExecutor.Run(fs, cancelToken.Token, task.TaskLogger, modsDir.FullName, App.SelectedPack, additionalArgs.Trim(), modPaths.ToArray());

                    foreach (var executionAction in App.PluginManager.ExecutionActions)
                        executionAction.PostLaunchAction(task.TaskLogger, PluginManagerType.ModManager, cancelToken.Token);
                }
                catch (OperationCanceledException)
                {
                    retCode = -1;

                    foreach (var executionAction in App.PluginManager.ExecutionActions)
                        executionAction.PostLaunchAction(task.TaskLogger, PluginManagerType.ModManager, cancelToken.Token);

                    // process was cancelled
                    App.Logger.Log("Launch Cancelled");
                }

            }, showCancelButton: true, cancelCallback: (task) => cancelToken.Cancel());

            if (retCode != -1)
                WindowState = WindowState.Minimized;

            // kill the application if launched from the command line
            if (App.LaunchGameImmediately)
                Close();

            GC.Collect();
        }

        private void FrostyWindow_Closing(object sender, CancelEventArgs e)
            => Config.Save();
        //=> Config.Save(App.configFilename);

        private bool AddPack(string packName)
        {
            FrostyPack existingPack = packs.Find((FrostyPack a) =>
            {
                return a.Name.CompareTo(packName) == 0;
            });

            if (existingPack == null)
            {
                FrostyPack pack = new FrostyPack(packName);

                packs.Add(pack);
                packsComboBox.Items.Refresh();
                packsComboBox.SelectedItem = pack;

                Config.Add(pack.Name, "", ConfigScope.Pack);
                //Config.Add("Profiles", profile.Name, "");

                return true;
            }
            else
            {
                FrostyMessageBox.Show("A pack with the same name already exists", "Frosty Mod Manager");

                return false;
            }
        }

        private void enabledCheckBox_Checked(object sender, RoutedEventArgs e) => selectedPack.Refresh();

        private void installModButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "(All supported formats)|*.fbmod;*.rar;*.zip;*.7z;*.daimod" + "|*.fbmod (Frostbite Mod)|*.fbmod" + "|*.rar (Rar File)|*.rar" + "|*.zip (Zip File)|*.zip" + "|*.7z (7z File)|*.7z" + "|*.daimod (DragonAge Mod)|*.daimod",
                Title = "Install Mod",
                Multiselect = true
            };

            if (ofd.ShowDialog() == true)
            {
                InstallMods(ofd.FileNames);
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
            view.Refresh();
        }

        private void uninstallModButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (IFrostyMod mod in availableModsList.SelectedItems)
            {
                FileInfo fi = new FileInfo(mod.Path);

                // remove from available list
                availableMods.Remove(mod);

                // remove from current pack
                int idx = selectedPack.AppliedMods.FindIndex((FrostyAppliedMod a) => a.Mod == mod);
                if (idx != -1)
                    selectedPack.AppliedMods.RemoveAt(idx);

                // [DAI fix] also delete the copy kept for Reconfigure
                DaiDeleteOriginal(mod);

                if (!fi.Exists)
                    continue;

                File.Delete(fi.FullName);

                if (mod is FrostyModCollection && Config.Get<bool>("DeleteCollectionMods", true))
                {
                    foreach (FrostyMod cmod in ((FrostyModCollection)mod).Mods)
                    {
                        fi = new FileInfo(cmod.Path);
                        File.Delete(fi.FullName);
                    }
                }
            }

            availableModsList.SelectedItem = null;
            ICollectionView view = CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
            view.Refresh();

            selectedPack.Refresh();
            appliedModsList.Items.Refresh();

            FrostyMessageBox.Show("Mod(s) has been successfully uninstalled", "Frosty Mod Manager");
        }

        private int VerifyMod(Stream stream)
        {
            using (DbReader reader = new DbReader(stream, null))
            {
                ulong magic = reader.ReadULong();
                if (magic != FrostyMod.Magic)
                {
                    reader.Position = 0;

                    DbObject modObj = reader.ReadDbObject();
                    if (modObj == null)
                        return -1;

                    if (modObj.GetValue<string>("gameProfile").ToLower() != ProfilesLibrary.ProfileName.ToLower())
                        return -2;

                    if (modObj.GetValue<int>("gameVersion") != fs.Head)
                        return 1;
                }
                else
                {
                    reader.Position = 0;
                    using (FrostyModReader modReader = new FrostyModReader(stream))
                    {
                        if (!modReader.IsValid)
                            return -1;

                        return modReader.GameVersion != fs.Head ? 0x8001 : 0x8000;
                    }
                }
            }

            return 0;
        }

        private FrostyMod AddMod(string modFilename, int format)
        {
            FrostyMod mod = LoadMod(modFilename, format);
            lock (availableMods)
            {
                availableMods.Add(mod);
            }

            return mod;
        }

        // [DAI fix] split out of AddMod for Reconfigure
        private FrostyMod LoadMod(string modFilename, int format)
        {
            FrostyMod mod = null;
            if (format == 1)
            {
                mod = new FrostyMod(modFilename);
            }
            else
            {
                DbObject modObj = null;
                using (DbReader reader = new DbReader(new FileStream(modFilename, FileMode.Open, FileAccess.Read), null))
                    modObj = reader.ReadDbObject();

                mod = new FrostyMod(modFilename, modObj);
            }

            if (mod.GameVersion != fs.Head)
                mod.AddWarning("Mod was designed for a different game version");

            return mod;
        }

        private FrostyModCollection AddCollection(string collectionFilename, int format)
        {
            FrostyModCollection collection = null;
            try
            {
                collection = new FrostyModCollection(collectionFilename);
            }
            catch (Exception e)
            {
                FrostyMessageBox.Show(e.Message);
                return null;
            }

            foreach (FrostyMod mod in collection.Mods)
            {
                lock (availableMods)
                {
                    int index = availableMods.FindIndex((IFrostyMod a) => a.Filename == mod.Filename);
                    if (index != -1)
                    {
                        availableMods.RemoveAt(index);
                    }
                }
            }

            lock (availableMods) availableMods.Add(collection);

            return collection;
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
            => Close();

        private void availableModsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ListView)e.OriginalSource != availableModsList)
                availableModsList.SelectedIndex = -1;

            IFrostyMod mod = ((ListView)e.OriginalSource).SelectedItem as IFrostyMod;
            if (mod == null)
            {
                uninstallModButton.IsEnabled = false;
                addModButton.IsEnabled = false;

                if (tabControl.SelectedIndex == 1)
                    tabControl.SelectedIndex = 0;

                modDescTabItem.Visibility = Visibility.Collapsed;
                modDescTabItem.Content = null;

                return;
            }

            FrostyModDescription modDescPanel = new FrostyModDescription { Mod = mod };
            modDescPanel.ScreenshotClicked += ModDescPanel_ScreenshotClicked;

            modDescTabItem.Content = modDescPanel;
            modDescTabItem.Visibility = Visibility.Visible;

            uninstallModButton.IsEnabled = true;
            addModButton.IsEnabled = true;
        }

        private void ModDescPanel_ScreenshotClicked(object sender, ScreenshotButtonEventArgs e)
        {
            imagePanel.Visibility = Visibility.Visible;
            screenshotImage.Source = e.Screenshot;
        }

        private void largeScreenshot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            imagePanel.Visibility = Visibility.Collapsed;
        }

        private void FrostyWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                InstallMods(filenames);

                ICollectionView view = CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
                view.Refresh();
            }
            else if (e.Data.GetFormats().Any(f => f == "FileContents"))
            {
                SystemSounds.Hand.Play();
                FrostyMessageBox.Show("Cannot import mod files that have not been extracted", "Frosty Mod Manager");
            }
        }

        #region -- [DAI fix] .daimod conversion helpers --

        private static string DaiXmlText(System.Xml.XmlElement parent, string childName)
        {
            if (parent == null || parent[childName] == null)
                return "";
            return parent[childName].InnerText;
        }

        private static byte[] DaiHexToBytes(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length % 2 != 0)
                return null;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                if (!byte.TryParse(hex.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out bytes[i]))
                    return null;
            }
            return bytes;
        }

        private static long DaiDecompressedLength(byte[] data)
        {
            try
            {
                using (CasReader reader = new CasReader(new MemoryStream(data)))
                    return reader.Read().Length;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // chunk meta is a hex string of a DbObject fragment
        private static int DaiParseFirstMip(string metaHex)
        {
            if (string.IsNullOrEmpty(metaHex) || metaHex == "00")
                return -1;

            byte[] meta = DaiHexToBytes(metaHex);
            if (meta == null)
                return -1;

            byte[] key = Encoding.ASCII.GetBytes("firstMip\0");
            for (int i = 0; i + key.Length + 4 <= meta.Length; i++)
            {
                bool match = true;
                for (int k = 0; k < key.Length; k++)
                {
                    if (meta[i + k] != key[k])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return DaiReadMipValue(meta, i + key.Length);
            }

            // not found: the old fixed position (hex char 20 = byte 10), read little-endian
            if (meta.Length >= 14)
                return DaiReadMipValue(meta, 10);

            return -1;
        }

        private class DaiTextureLayout
        {
            public int MipCount;
            public int FirstMip;
            public uint[] MipSizes;
        }

        // keyed by the chunk id each texture points at
        private static Dictionary<Guid, DaiTextureLayout> DaiReadTextureLayouts(System.Xml.XmlElement resourcesElem, List<byte[]> resources)
        {
            const uint TextureResType = 0x5C4954A6;
            Dictionary<Guid, DaiTextureLayout> layouts = new Dictionary<Guid, DaiTextureLayout>();

            foreach (System.Xml.XmlNode node in resourcesElem.ChildNodes)
            {
                System.Xml.XmlElement elem = node as System.Xml.XmlElement;
                if (elem == null || elem.GetAttribute("type") != "res" || elem.GetAttribute("action") == "remove")
                    continue;

                uint resType;
                int resId;
                if (!uint.TryParse(elem.GetAttribute("resType"), out resType) || resType != TextureResType)
                    continue;
                if (!int.TryParse(elem.GetAttribute("resourceId"), out resId) || resId < 0 || resId >= resources.Count)
                    continue;

                byte[] header;
                try
                {
                    using (CasReader reader = new CasReader(new MemoryStream(resources[resId])))
                        header = reader.Read();
                }
                catch (Exception)
                {
                    continue;
                }

                // DAI texture header: mipOffsets[2], type, format, flags(u16), width, height, depth,
                // sliceCount (u16 each), mipCount (u8), firstMip (u8), chunkId (guid), mipSizes[15]
                if (header == null || header.Length < 104)
                    continue;

                DaiTextureLayout layout = new DaiTextureLayout
                {
                    MipCount = header[26],
                    FirstMip = header[27],
                    MipSizes = new uint[15]
                };
                for (int i = 0; i < 15; i++)
                    layout.MipSizes[i] = BitConverter.ToUInt32(header, 44 + i * 4);

                byte[] guidBytes = new byte[16];
                Array.Copy(header, 28, guidBytes, 0, 16);
                layouts[new Guid(guidBytes)] = layout;
            }

            return layouts;
        }

        // Same layout as Utils.CompressTexture: from firstMip on is the always-loaded range.
        // False (values untouched) if there is nothing to split or no block boundary at that mip.
        private static bool DaiCalcTextureRange(DaiTextureLayout layout, byte[] chunkData,
            ref int firstMip, ref uint logicalOffset, ref uint logicalSize, ref uint rangeStart, ref uint rangeEnd)
        {
            if (layout.MipCount <= 1 || layout.MipCount > 15 || layout.FirstMip <= 0 || layout.FirstMip >= layout.MipCount)
                return false;

            long total = 0;
            for (int i = 0; i < layout.MipCount; i++)
                total += layout.MipSizes[i];
            if (total <= 0x10000)
                return false;

            // same rule as Utils.CompressTexture: the range keeps a minimum size of 0x10000,
            // the logical offset doesn't
            long first = 0, second = total, newLogicalOffset = 0;
            for (int i = 0; i < layout.FirstMip; i++)
            {
                if (second > 0x10000)
                {
                    first += layout.MipSizes[i];
                    second -= layout.MipSizes[i];
                }
                newLogicalOffset += layout.MipSizes[i];
            }

            // find the block that starts at uncompressed offset "first"
            long compressedPos = 0, uncompressedPos = 0, splitPos = -1;
            while (compressedPos + 8 <= chunkData.Length)
            {
                if (uncompressedPos == first)
                    splitPos = compressedPos;

                int p = (int)compressedPos;
                int decompressedSize = (chunkData[p] << 24) | (chunkData[p + 1] << 16) | (chunkData[p + 2] << 8) | chunkData[p + 3];
                ushort compressCode = (ushort)((chunkData[p + 4] << 8) | chunkData[p + 5]);
                int blockSize = (chunkData[p + 6] << 8) | chunkData[p + 7];

                // DAI block codes: 0x0270 = zlib (blockSize is the compressed size),
                // 0x0070 = stored (data is decompressedSize bytes)
                decompressedSize &= 0x00FFFFFF;
                if ((compressCode >> 8) == 0x00)
                    blockSize = decompressedSize;

                compressedPos += 8 + blockSize;
                uncompressedPos += decompressedSize;
            }

            if (compressedPos != chunkData.Length || uncompressedPos != total || splitPos < 0)
                return false;

            firstMip = layout.FirstMip;
            logicalOffset = (uint)newLogicalOffset;
            logicalSize = (uint)(total - newLogicalOffset);
            rangeStart = (uint)splitPos;
            rangeEnd = (uint)chunkData.Length;
            return true;
        }

        private static int DaiReadMipValue(byte[] buf, int pos)
        {
            // little-endian, but accept big-endian too
            int le = buf[pos] | (buf[pos + 1] << 8) | (buf[pos + 2] << 16) | (buf[pos + 3] << 24);
            if (le >= 0 && le < 16)
                return le;

            int be = (buf[pos] << 24) | (buf[pos + 1] << 16) | (buf[pos + 2] << 8) | buf[pos + 3];
            if (be >= 0 && be < 16)
                return be;

            return -1;
        }

        // The splash screen drops the asset manager after startup, so one is loaded from the cache
        // for .daimod imports and released afterwards.
        private AssetManager daiAssetManager;
        private bool daiAssetManagerFailed;

        private AssetManager DaiGetAssetManager()
        {
            if (Frosty.Core.App.AssetManager != null)
                return Frosty.Core.App.AssetManager;
            if (daiAssetManager != null || daiAssetManagerFailed || fs == null)
                return daiAssetManager;

            try
            {
                App.Logger.Log("[DAI] Loading game data from the cache to check bundle names");
                ResourceManager rm = new ResourceManager(fs);
                rm.Initialize();
                AssetManager am = new AssetManager(fs, rm);
                am.Initialize(additionalStartup: false);
                daiAssetManager = am;
            }
            catch (Exception e)
            {
                daiAssetManagerFailed = true;
                App.Logger.Log("[DAI] Could not load the game data (" + e.Message + "); using each .daimod's own add/modify flags");
            }
            return daiAssetManager;
        }

        private void DaiReleaseAssetManager()
        {
            daiAssetManager = null;
            daiAssetManagerFailed = false;
            GC.Collect();
        }

        private static Dictionary<string, int> DaiBuildBundleIdMap(AssetManager am)
        {
            if (am == null)
                return null;

            Dictionary<string, int> map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int bid = 0; ; bid++)
            {
                BundleEntry be = am.GetBundleEntry(bid);
                if (be == null)
                    break;

                if (be.Name != null && !map.ContainsKey(be.Name))
                    map.Add(be.Name, bid);
            }
            return map;
        }

        // Frosty only modifies entries already in the bundle and only adds listed additions,
        // so add vs modify has to match the game data.
        private static string DaiResolveBundleAction(AssetManager am, Dictionary<string, int> bundleIds, string type, string resName, Guid chunkGuid, string bundleName, string daimodAction, out bool usedFallback)
        {
            usedFallback = false;

            int bid;
            if (am != null && bundleIds != null && bundleIds.TryGetValue(bundleName, out bid))
            {
                AssetEntry existing = null;
                if (type == "ebx")
                    existing = am.GetEbxEntry(resName);
                else if (type == "res")
                    existing = am.GetResEntry(resName);
                else
                    existing = am.GetChunkEntry(chunkGuid);

                return (existing != null && existing.IsInBundle(bid)) ? "modify" : "add";
            }

            // bundle not in the game data: use the .daimod's flag
            usedFallback = true;
            return (daimodAction == "add") ? "add" : "modify";
        }

        #endregion

        #region -- [DAI fix] .daimod import and Reconfigure --

        // The original .daimod and its chosen settings are kept in "<mods folder>\DAI originals" for
        // Reconfigure. Only .fbmod files directly in the mods folder are loaded, so the subfolder is ignored.

        private const string DaiOriginalsFolderName = "DAI originals";
        private const string DaiConvertedPrefix = "(Converted from .daimod)";

        internal sealed class DaiSavedSettings
        {
            public int Format { get; set; } = 1;
            public string SourceFile { get; set; }
            public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
        }

        private string DaiOriginalsDir => Path.Combine(modsDir.FullName, DaiOriginalsFolderName);
        private string DaiOriginalPath(string baseName) => Path.Combine(DaiOriginalsDir, baseName + ".daimod");
        private string DaiSettingsPath(string baseName) => Path.Combine(DaiOriginalsDir, baseName + ".settings.json");

        private bool DaiCanReconfigure(IFrostyMod mod)
        {
            FrostyMod fm = mod as FrostyMod;
            if (fm == null || fm.NewFormat)
                return false;

            return File.Exists(DaiOriginalPath(Path.GetFileNameWithoutExtension(fm.Filename)))
                || (fm.ModDetails.Description ?? "").StartsWith(DaiConvertedPrefix, StringComparison.Ordinal);
        }

        private Dictionary<string, string> DaiLoadSettings(string baseName)
        {
            string path = DaiSettingsPath(baseName);
            if (!File.Exists(path))
                return null;

            try
            {
                DaiSavedSettings saved = JsonConvert.DeserializeObject<DaiSavedSettings>(File.ReadAllText(path));
                return saved?.Values;
            }
            catch (Exception e)
            {
                App.Logger.Log("[DAI] Could not read the saved settings for " + baseName + " (" + e.Message + "); using the mod's defaults");
                return null;
            }
        }

        private void DaiSaveOriginal(string sourcePath, string baseName, Dictionary<string, string> values)
        {
            Directory.CreateDirectory(DaiOriginalsDir);

            string target = DaiOriginalPath(baseName);
            if (!string.Equals(Path.GetFullPath(sourcePath), Path.GetFullPath(target), StringComparison.OrdinalIgnoreCase))
                File.Copy(sourcePath, target, true);

            DaiSavedSettings saved = new DaiSavedSettings
            {
                SourceFile = Path.GetFileName(sourcePath),
                Values = values ?? new Dictionary<string, string>()
            };
            File.WriteAllText(DaiSettingsPath(baseName), JsonConvert.SerializeObject(saved, Newtonsoft.Json.Formatting.Indented));
        }

        private void DaiDeleteOriginal(IFrostyMod mod)
        {
            if (!(mod is FrostyMod))
                return;

            string baseName = Path.GetFileNameWithoutExtension(mod.Filename);
            try
            {
                if (File.Exists(DaiOriginalPath(baseName)))
                    File.Delete(DaiOriginalPath(baseName));
                if (File.Exists(DaiSettingsPath(baseName)))
                    File.Delete(DaiSettingsPath(baseName));
            }
            catch (Exception e)
            {
                App.Logger.Log("[DAI] Could not remove the kept copy of " + baseName + ".daimod (" + e.Message + ")");
            }
        }

        private static bool DaiReadHeader(string path, out string title, out bool hasScript)
        {
            title = null;
            hasScript = false;

            using (NativeReader reader = new NativeReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                if (reader.ReadSizedString(8) != "DAIMODV2")
                    return false;

                reader.ReadInt();
                reader.ReadNullTerminatedString();
                string xml = reader.ReadNullTerminatedString();
                string code = reader.ReadNullTerminatedString();
                hasScript = code != "";

                try
                {
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.LoadXml(xml);
                    title = DaiXmlText(xmlDoc["daimod"]?["details"], "name");
                }
                catch (Exception)
                {
                    // the title is only used for a sanity check
                }
                return true;
            }
        }

        // UI thread only
        private FrostyMod DaiReplaceMod(FrostyMod oldMod, string fbmodPath)
        {
            FrostyMod newMod = LoadMod(fbmodPath, 0);

            lock (availableMods)
            {
                int index = availableMods.IndexOf(oldMod);
                if (index != -1)
                    availableMods[index] = newMod;
                else
                    availableMods.Add(newMod);
            }

            foreach (FrostyPack pack in packs)
                pack.ReplaceMod(oldMod, newMod);

            appliedModsList.Items.Refresh();
            return newMod;
        }

        // Re-importing an installed mod starts from its saved settings and replaces it in place
        // (it used to show up twice until restart).
        private FrostyMod DaiImportMod(FileInfo fi, List<ImportErrorInfo> errors)
        {
            string baseName = Path.GetFileNameWithoutExtension(fi.Name);

            FrostyMod existingMod;
            lock (availableMods)
                existingMod = availableMods.Find((IFrostyMod a) => string.Equals(a.Filename, baseName + ".fbmod", StringComparison.OrdinalIgnoreCase)) as FrostyMod;
            if (existingMod != null)
                baseName = Path.GetFileNameWithoutExtension(existingMod.Filename);

            Dictionary<string, string> savedSettings = existingMod != null ? DaiLoadSettings(baseName) : null;

            if (!DaiConvertMod(fi, modsDir.FullName, baseName, savedSettings, null, false, errors, out Dictionary<string, string> chosenValues))
                return null;

            if (chosenValues == null)
            {
                // no longer configurable: drop any stale copy
                if (existingMod != null)
                    DaiDeleteOriginal(existingMod);
            }
            else
            {
                try
                {
                    DaiSaveOriginal(fi.FullName, baseName, chosenValues);
                }
                catch (Exception e)
                {
                    errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "The mod was imported, but a copy could not be kept for Reconfigure: " + e.Message, isWarning = true });
                }
            }

            // caught here: InstallMods' handler would delete fi, which is the user's .daimod
            string fbmodPath = Path.Combine(modsDir.FullName, baseName + ".fbmod");
            try
            {
                if (existingMod == null)
                    return AddMod(fbmodPath, 0);

                FrostyMod newMod = null;
                Dispatcher.Invoke(() => newMod = DaiReplaceMod(existingMod, fbmodPath));
                return newMod;
            }
            catch (FrostyModLoadException e)
            {
                errors.Add(new ImportErrorInfo() { filename = fi.Name, error = e.Message });
                return null;
            }
        }

        private void DaiReconfigureMod(FrostyMod mod)
        {
            string title = mod.ModDetails.Title;
            string baseName = Path.GetFileNameWithoutExtension(mod.Filename);
            string sourcePath = DaiOriginalPath(baseName);
            string legacyDescription = null;
            Dictionary<string, string> savedSettings = null;

            if (File.Exists(sourcePath))
            {
                savedSettings = DaiLoadSettings(baseName);
            }
            else
            {
                // imported before Reconfigure existed: ask for the original, pre-fill from the description
                if (FrostyMessageBox.Show("Frosty doesn't have a copy of the original .daimod for \"" + title + "\", because it was imported before Reconfigure was added.\r\n\r\n" +
                    "Choose the original .daimod file now? Frosty keeps a copy, so you only need to do this once.", "Frosty Mod Manager", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;

                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "*.daimod (DragonAge Mod)|*.daimod",
                    Title = "Choose the original .daimod for " + title
                };
                if (ofd.ShowDialog() != true)
                    return;

                sourcePath = ofd.FileName;
                legacyDescription = mod.ModDetails.Description;
            }

            string daimodTitle;
            bool hasScript;
            try
            {
                if (!DaiReadHeader(sourcePath, out daimodTitle, out hasScript))
                {
                    FrostyMessageBox.Show(Path.GetFileName(sourcePath) + " is not a valid DAI mod.", "Frosty Mod Manager");
                    return;
                }
            }
            catch (Exception e)
            {
                FrostyMessageBox.Show("Could not read " + Path.GetFileName(sourcePath) + ": " + e.Message, "Frosty Mod Manager");
                return;
            }

            if (!hasScript)
            {
                FrostyMessageBox.Show("\"" + title + "\" has no settings to configure.", "Frosty Mod Manager");
                return;
            }

            if (legacyDescription != null && !string.IsNullOrEmpty(daimodTitle) && daimodTitle != title)
            {
                if (FrostyMessageBox.Show("The file you chose is \"" + daimodTitle + "\", not \"" + title + "\". Use it anyway?", "Frosty Mod Manager", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;
            }

            // convert into a temp folder so the installed mod is untouched on cancel or failure
            string tempDir = Path.Combine(DaiOriginalsDir, "_reconfigure");
            FileInfo sourceFi = new FileInfo(sourcePath);
            List<ImportErrorInfo> errors = new List<ImportErrorInfo>();
            Dictionary<string, string> chosenValues = null;
            bool converted = false;

            FrostyTaskWindow.Show("Reconfiguring Mod", title, (task) =>
            {
                try
                {
                    Directory.CreateDirectory(tempDir);
                    converted = DaiConvertMod(sourceFi, tempDir, baseName, savedSettings, legacyDescription, true, errors, out chosenValues);
                }
                catch (Exception e)
                {
                    errors.Add(new ImportErrorInfo() { filename = mod.Filename, error = "Reconfigure failed, the mod was left as it was: " + e.Message });
                }
                finally
                {
                    DaiReleaseAssetManager();
                }
            });

            FrostyMod newMod = null;
            if (converted)
            {
                try
                {
                    File.Copy(Path.Combine(tempDir, baseName + "_01.archive"), mod.Path.Replace(".fbmod", "_01.archive"), true);
                    File.Copy(Path.Combine(tempDir, baseName + ".fbmod"), mod.Path, true);
                    newMod = DaiReplaceMod(mod, mod.Path);
                    App.Logger.Log("[DAI] " + title + ": reconfigured");
                }
                catch (Exception e)
                {
                    errors.Add(new ImportErrorInfo() { filename = mod.Filename, error = "Reconfigure failed while replacing the mod's files: " + e.Message });
                }

                try
                {
                    DaiSaveOriginal(sourcePath, baseName, chosenValues);
                }
                catch (Exception e)
                {
                    errors.Add(new ImportErrorInfo() { filename = mod.Filename, error = "The new settings could not be saved for next time: " + e.Message, isWarning = true });
                }
            }

            try
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
            catch (Exception)
            {
                // left for the next Reconfigure to overwrite
            }

            if (newMod != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
                view.Refresh();
                availableModsList.SelectedItem = newMod;
            }

            if (errors.Count > 0)
            {
                InstallErrorsWindow win = new InstallErrorsWindow(errors);
                win.ShowDialog();
            }
            else if (newMod != null)
            {
                FrostyMessageBox.Show("\"" + title + "\" has been reconfigured. Its load order and enabled state are unchanged.", "Frosty Mod Manager");
            }
        }

        private void availableModsList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // the menu only has Reconfigure..., so only open it for converted .daimods
            IFrostyMod mod = availableModsList.SelectedItems.Count == 1 ? availableModsList.SelectedItem as IFrostyMod : null;
            if (mod == null || !DaiCanReconfigure(mod))
                e.Handled = true;
        }

        private void appliedModsList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            IFrostyMod mod = appliedModsList.SelectedItems.Count == 1 ? (appliedModsList.SelectedItem as FrostyAppliedMod)?.Mod : null;
            if (mod == null || !DaiCanReconfigure(mod))
                e.Handled = true;
        }

        private void availableModsReconfigureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (availableModsList.SelectedItem is FrostyMod mod)
                DaiReconfigureMod(mod);
        }

        private void appliedModsReconfigureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if ((appliedModsList.SelectedItem as FrostyAppliedMod)?.Mod is FrostyMod mod)
                DaiReconfigureMod(mod);
        }

        // Moved out of InstallMods. chosenValues is null if the mod has no script. On failure or
        // cancel nothing is written and the reason goes into errors.
        private bool DaiConvertMod(FileInfo fi, string outputDir, string outputBaseName, Dictionary<string, string> savedSettings, string legacyDescription, bool reconfiguring, List<ImportErrorInfo> errors, out Dictionary<string, string> chosenValues)
        {
            chosenValues = null;

            // [DAI fix] read-only (was read/write)
            using (NativeReader reader = new NativeReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read)))
            {
                string magic = reader.ReadSizedString(8);
                if (magic != "DAIMODV2")
                {
                    errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "File is not a valid DAI Mod." });
                    return false;
                }

                int unk = reader.ReadInt();
                string name = reader.ReadNullTerminatedString();
                string xml = reader.ReadNullTerminatedString();
                string code = reader.ReadNullTerminatedString();

                int resCount = reader.ReadInt();
                List<byte[]> resources = new List<byte[]>();
                List<bool> shouldWrite = new List<bool>();

                for (int i = 0; i < resCount; i++)
                {
                    resources.Add(reader.ReadBytes(reader.ReadInt()));
                    shouldWrite.Add(true);
                }

                // [DAI fix] to spot resources the script replaced
                byte[][] originalResources = resources.ToArray();

                string configValues = "";
                Dictionary<string, string> chosenSettings = null;
                string configError = null;
                bool configCancelled = false;
                if (code != "")
                {
                    Dispatcher.Invoke(() =>
                    {
                        // [DAI fix] script errors used to crash the import, and closing with X imported it unconfigured
                        try
                        {
                            ConfigWindow win = new ConfigWindow(code, resources, shouldWrite, savedSettings, legacyDescription);
                            if (!string.IsNullOrEmpty(name))
                                win.Title = "Mod Configuration - " + name;
                            win.ShowDialog();

                            if (win.ScriptError != null)
                                configError = win.ScriptError;
                            else if (!win.ScriptApplied)
                                configCancelled = true;
                            else
                            {
                                configValues = win.GetConfigValues();
                                chosenSettings = win.GetSettingValues();
                            }
                        }
                        catch (Exception ex)
                        {
                            configError = "The mod's configuration script could not be loaded: " + ex.Message;
                        }
                    });
                }

                if (configError != null)
                {
                    errors.Add(new ImportErrorInfo() { filename = fi.Name, error = configError });
                    return false;
                }
                if (configCancelled)
                {
                    errors.Add(new ImportErrorInfo() { filename = fi.Name, error = (reconfiguring ? "Reconfigure cancelled, the mod was left as it was" : "Import cancelled") + ": the configuration window was closed without pressing Save.", isWarning = true });
                    return false;
                }

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(xml);

                System.Xml.XmlElement rootElem = xmlDoc["daimod"];
                System.Xml.XmlElement elem = rootElem["details"];
                string newDesc = "(Converted from .daimod)\r\n\r\n" + DaiXmlText(elem, "description") + "\r\n\r\n" + configValues;

                DbObject modObject = new DbObject();
                modObject.AddValue("magic", "FBMODV3");
                modObject.AddValue("gameProfile", ProfilesLibrary.ProfileName);
                modObject.AddValue("gameVersion", 0);

                modObject.AddValue("title", DaiXmlText(elem, "name"));
                modObject.AddValue("author", DaiXmlText(elem, "author"));
                modObject.AddValue("category", "DAI Mods");
                modObject.AddValue("version", DaiXmlText(elem, "version"));
                modObject.AddValue("description", newDesc);

                DbObject resourcesList = new DbObject(false);
                DbObject actionsList = new DbObject(false);
                DbObject screenshotList = new DbObject(false);

                // [DAI fix] offsets in resource-id order, which is how the archive is written (was XML order)
                long[] archiveOffsets = new long[resources.Count];
                long runningOffset = 0;
                for (int i = 0; i < resources.Count; i++)
                {
                    archiveOffsets[i] = runningOffset;
                    if (shouldWrite[i])
                        runningOffset += resources[i].Length;
                }

                // XML resource index -> bundle names
                Dictionary<int, List<string>> bundleMap = new Dictionary<int, List<string>>();
                System.Xml.XmlElement bundlesElem = rootElem["bundles"];
                if (bundlesElem != null)
                {
                    foreach (System.Xml.XmlNode bundleNode in bundlesElem.ChildNodes)
                    {
                        System.Xml.XmlElement bundleElem = bundleNode as System.Xml.XmlElement;
                        if (bundleElem == null || bundleElem["entries"] == null)
                            continue;

                        string bundleName = bundleElem.GetAttribute("name");
                        foreach (System.Xml.XmlNode entryNode in bundleElem["entries"].ChildNodes)
                        {
                            System.Xml.XmlElement entryElem = entryNode as System.Xml.XmlElement;
                            if (entryElem == null)
                                continue;

                            int id;
                            if (!int.TryParse(entryElem.GetAttribute("id"), out id))
                                continue;

                            List<string> list;
                            if (!bundleMap.TryGetValue(id, out list))
                            {
                                list = new List<string>();
                                bundleMap.Add(id, list);
                            }
                            list.Add(bundleName);
                        }
                    }
                }

                // [DAI fix] add vs modify per bundle from the game data. Everything used to be "modify",
                // so additions were silently dropped.
                AssetManager am = DaiGetAssetManager();
                Dictionary<string, int> bundleIds = DaiBuildBundleIdMap(am);

                int numAdded = 0, numModified = 0, numRemoved = 0, numFallback = 0, numTexFixed = 0;
                string firstMissingBundle = null;
                int index = -1;

                System.Xml.XmlElement resourcesElem = rootElem["resources"];
                if (resourcesElem == null)
                {
                    errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "DAI Mod does not contain a resource list." });
                    return false;
                }

                // [DAI fix] some .daimods say firstMip 0 when the texture header doesn't, which gives
                // black/rainbow textures at a distance. The header wins.
                Dictionary<Guid, DaiTextureLayout> textureLayouts = DaiReadTextureLayouts(resourcesElem, resources);

                foreach (System.Xml.XmlNode resNode in resourcesElem.ChildNodes)
                {
                    System.Xml.XmlElement subElem = resNode as System.Xml.XmlElement;
                    if (subElem == null)
                        continue;

                    index++;

                    string type = subElem.GetAttribute("type");
                    string resAction = subElem.GetAttribute("action");
                    string resName = subElem.GetAttribute("name");
                    Guid chunkGuid = Guid.Empty;

                    if (type != "ebx" && type != "res")
                    {
                        byte[] chunkidBytes = DaiHexToBytes(resName);
                        if (chunkidBytes == null || chunkidBytes.Length != 16)
                        {
                            errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "Skipped a chunk with an invalid id: " + resName, isWarning = true });
                            continue;
                        }
                        chunkGuid = new Guid(chunkidBytes);
                        resName = chunkGuid.ToString();
                    }

                    List<string> targetBundles;
                    if (!bundleMap.TryGetValue(index, out targetBundles))
                        targetBundles = new List<string>();

                    int thisResourceId = resourcesList.Count;

                    if (resAction == "remove")
                    {
                        // [DAI fix] removals used to be ignored
                        if (targetBundles.Count == 0)
                            continue;

                        DbObject removal = new DbObject();
                        removal.AddValue("name", resName);
                        removal.AddValue("type", type);
                        removal.AddValue("isRemoval", true); // executor skips loading data for these

                        foreach (string bundleName in targetBundles)
                        {
                            DbObject action = new DbObject();
                            action.AddValue("resourceId", thisResourceId);
                            action.AddValue("type", "remove");
                            action.AddValue("bundle", bundleName);
                            actionsList.Add(action);
                            numRemoved++;
                        }

                        resourcesList.Add(removal);
                        continue;
                    }

                    int resId;
                    if (!int.TryParse(subElem.GetAttribute("resourceId"), out resId) || resId < 0 || resId >= resources.Count)
                    {
                        errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "Skipped " + resName + ": it points at a resource that is not in the file.", isWarning = true });
                        continue;
                    }
                    if (shouldWrite[resId] == false)
                        continue;

                    byte[] data = resources[resId];
                    bool scriptModified = !ReferenceEquals(data, originalResources[resId]);

                    DbObject resource = new DbObject();
                    resource.AddValue("name", resName);
                    resource.AddValue("type", type);

                    // [DAI fix] the XML hash is stale if the script replaced the data
                    resource.AddValue("sha1", FrostySdk.Utils.GenerateSha1(data));
                    resource.AddValue("originalSize", 0);
                    resource.AddValue("compressedSize", data.Length);
                    resource.AddValue("archiveIndex", 1);
                    resource.AddValue("archiveOffset", archiveOffsets[resId]);
                    resource.AddValue("shouldInline", false);

                    if (type == "ebx" || type == "res")
                    {
                        // [DAI fix] recalculate if the script replaced the data
                        long uncompressedSize;
                        if (scriptModified || !long.TryParse(subElem.GetAttribute("originalSize"), out uncompressedSize))
                        {
                            uncompressedSize = DaiDecompressedLength(data);
                            if (uncompressedSize < 0)
                            {
                                errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "Skipped " + resName + ": its data could not be read.", isWarning = true });
                                continue;
                            }
                        }
                        resource.AddValue("uncompressedSize", uncompressedSize);

                        if (resAction != "add" && subElem.HasAttribute("originalSha1"))
                            resource.AddValue("originalSha1", new Sha1(subElem.GetAttribute("originalSha1")));

                        if (type == "res")
                        {
                            // [DAI fix] GetValue<uint> overflowed for res types >= 0x80000000 (stored as int)
                            uint resType = uint.Parse(subElem.GetAttribute("resType"));
                            resource.AddValue("resType", resType);
                            if (resType == 0x5C4954A6)
                                resource.SetValue("shouldInline", true);
                            resource.AddValue("resRid", (ulong)long.Parse(subElem.GetAttribute("resRid")));

                            byte[] resMeta = DaiHexToBytes(subElem.GetAttribute("meta"));
                            resource.AddValue("resMeta", resMeta ?? new byte[0]);
                        }
                    }
                    else
                    {
                        uint logicalOffset = uint.Parse(subElem.GetAttribute("logicalOffset"));
                        uint logicalSize = uint.Parse(subElem.GetAttribute("logicalSize"));

                        // [DAI fix] recalculate if the script replaced it (only safe without a mip offset)
                        if (scriptModified)
                        {
                            if (logicalOffset == 0)
                            {
                                long newSize = DaiDecompressedLength(data);
                                if (newSize >= 0)
                                    logicalSize = (uint)newSize;
                            }
                            else
                            {
                                errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "The configuration script changed texture chunk " + resName + "; its size information was kept as-is and may be wrong.", isWarning = true });
                            }
                        }

                        uint rangeStart = uint.Parse(subElem.GetAttribute("rangeStart"));
                        uint rangeEnd = uint.Parse(subElem.GetAttribute("rangeEnd"));

                        // [DAI fix] little-endian int in the meta; was read as big-endian hex (3 became 50331648)
                        int firstMip = DaiParseFirstMip(subElem.GetAttribute("meta"));

                        // [DAI fix] match the texture header
                        DaiTextureLayout texLayout;
                        if (textureLayouts.TryGetValue(chunkGuid, out texLayout) && texLayout.FirstMip != firstMip)
                        {
                            if (DaiCalcTextureRange(texLayout, data, ref firstMip, ref logicalOffset, ref logicalSize, ref rangeStart, ref rangeEnd))
                                numTexFixed++;
                        }

                        resource.AddValue("rangeStart", rangeStart);
                        resource.AddValue("rangeEnd", rangeEnd);
                        resource.AddValue("logicalOffset", logicalOffset);
                        resource.AddValue("logicalSize", logicalSize);
                        resource.AddValue("h32", int.Parse(subElem.GetAttribute("chunkH32")));

                        if (firstMip >= 0)
                            resource.SetValue("firstMip", firstMip);

                        // add special chunks bundle
                        // [DAI fix] was resourcesList.Count - 1, the previous resource
                        DbObject chunkAction = new DbObject();
                        chunkAction.AddValue("resourceId", thisResourceId);
                        chunkAction.AddValue("type", "add");
                        chunkAction.AddValue("bundle", "chunks");
                        actionsList.Add(chunkAction);
                    }

                    foreach (string bundleName in targetBundles)
                    {
                        bool usedFallback;
                        string bundleAction = DaiResolveBundleAction(am, bundleIds, type, resName, chunkGuid, bundleName, resAction, out usedFallback);
                        if (usedFallback)
                        {
                            numFallback++;
                            if (firstMissingBundle == null)
                                firstMissingBundle = bundleName;
                        }
                        if (bundleAction == "add")
                            numAdded++;
                        else
                            numModified++;

                        DbObject action = new DbObject();
                        action.AddValue("resourceId", thisResourceId);
                        action.AddValue("type", bundleAction);
                        action.AddValue("bundle", bundleName);
                        actionsList.Add(action);
                    }

                    resourcesList.Add(resource);
                }

                App.Logger.Log(string.Format("[DAI] {0}: {1} bundle modifications, {2} additions, {3} removals{4}{5}",
                    fi.Name, numModified, numAdded, numRemoved,
                    numTexFixed > 0 ? string.Format(", {0} texture streaming ranges corrected", numTexFixed) : "",
                    numFallback > 0 ? string.Format(" ({0} decided from the .daimod's own add/modify flag because the bundle was not found in the game data)", numFallback) : ""));

                // [DAI fix] log why bundle names didn't match
                if (firstMissingBundle != null)
                {
                    string leaf = firstMissingBundle.Substring(firstMissingBundle.LastIndexOf('/') + 1);
                    string similar = null;
                    if (bundleIds != null)
                    {
                        foreach (string known in bundleIds.Keys)
                        {
                            if (known.EndsWith(leaf, StringComparison.OrdinalIgnoreCase))
                            {
                                similar = known;
                                break;
                            }
                        }
                    }
                    string sample = null;
                    if (bundleIds != null)
                    {
                        foreach (string known in bundleIds.Keys)
                        {
                            sample = known;
                            break;
                        }
                    }
                    App.Logger.Log(string.Format("[DAI] bundle lookup: game data has {0} bundles; '{1}' not found; similar name: '{2}'; example game bundle name: '{3}'",
                        bundleIds == null ? "no asset manager / 0" : bundleIds.Count.ToString(), firstMissingBundle, similar ?? "(none)", sample ?? "(none)"));
                }

                modObject.AddValue("screenshots", screenshotList);
                modObject.AddValue("resources", resourcesList);
                modObject.AddValue("actions", actionsList);

                using (DbWriter writer = new DbWriter(new FileStream(Path.Combine(outputDir, outputBaseName + ".fbmod"), FileMode.Create)))
                {
                    writer.Write(modObject);
                }
                using (NativeWriter writer = new NativeWriter(new FileStream(Path.Combine(outputDir, outputBaseName + "_01.archive"), FileMode.Create)))
                {
                    for (int i = 0; i < resources.Count; i++)
                    {
                        if (shouldWrite[i])
                            writer.Write(resources[i]);
                    }
                }

                chosenValues = chosenSettings;
                return true;
            }
        }

        #endregion

        private void InstallMods(string[] filenames)
        {
            IFrostyMod lastInstalledMod = null;
            List<ImportErrorInfo> errors = new List<ImportErrorInfo>();

            PackManifest packManifest = null;

            FrostyTaskWindow.Show("Installing Mods", "", (task) =>
            {
                List<string> collections = new List<string>();

                foreach (string filename in filenames)
                {
                    FileInfo fi = new FileInfo(filename);
                    task.Update(fi.Name);

                    try
                    {
                        if (IsCompressed(fi))
                        {
                            List<string> mods = new List<string>();
                            List<int> format = new List<int>();
                            List<string> archives = new List<string>();
                            int fbpacks = 0;

                            // create decompressor
                            IDecompressor decompressor = null;
                            if (fi.Extension == ".rar") decompressor = new RarDecompressor();
                            else if (fi.Extension == ".zip" || fi.Extension == ".fbpack") decompressor = new ZipDecompressor();
                            else if (fi.Extension == ".7z") decompressor = new SevenZipDecompressor();

                            try
                            {
                                // search out fbmods in archive
                                decompressor.OpenArchive(filename);
                                foreach (CompressedFileInfo compressedFi in decompressor.EnumerateFiles())
                                {

                                    if (compressedFi.Extension == ".fbpack")
                                    {
                                        //create temp file
                                        DirectoryInfo tempdir = new DirectoryInfo($"temp/");
                                        FileInfo tempfile = new FileInfo(tempdir + compressedFi.Filename);

                                        tempdir.Create();
                                        decompressor.DecompressToFile(tempfile.FullName);

                                        //install temp file
                                        Dispatcher.Invoke(() =>
                                        {
                                            InstallMods(new string[] { tempfile.FullName });
                                        });

                                        //delete temp files
                                        if (tempfile.Exists) tempfile.Delete();
                                        if (tempdir.Exists) tempdir.Delete();

                                        fbpacks++;
                                    }
                                    else if (compressedFi.Extension == ".fbcollection")
                                    {
                                        collections.Add(compressedFi.Filename);
                                    }
                                    else if (compressedFi.Extension == ".fbmod")
                                    {
                                        string modFilename = compressedFi.Filename;
                                        byte[] buffer = decompressor.DecompressToMemory();

                                        using (MemoryStream ms = new MemoryStream(buffer))
                                        {
                                            int retCode = VerifyMod(ms);
                                            if (retCode >= 0)
                                            {
                                                if ((retCode & 1) != 0)
                                                {
                                                    // continue with import (warning)
                                                    errors.Add(new ImportErrorInfo() { filename = modFilename, error = "Mod was designed for a different game version, it may or may not work.", isWarning = true });
                                                }

                                                // add mod
                                                mods.Add(compressedFi.Filename);
                                                format.Add((retCode & 0x8000) != 0 ? 1 : 0);
                                            }
                                            // ignore RetCode -1 here
                                            else if (retCode == -2)
                                            {
                                                errors.Add(new ImportErrorInfo() { filename = modFilename, error = "Mod was not designed for this game." });
                                            }
                                        }
                                    }
                                    else if (compressedFi.Extension == ".archive")
                                    {
                                        archives.Add(compressedFi.Filename);
                                    }
                                    else if (compressedFi.Filename == "manifest.json")
                                    {
                                        using (StreamReader reader = new StreamReader(compressedFi.Stream))
                                        {
                                            packManifest = JsonConvert.DeserializeObject<PackManifest>(reader.ReadToEnd());
                                        }
                                    }
                                }
                                decompressor.CloseArchive();
                            }
                            catch
                            {
                                errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "Failed to read Archive." });
                            }

                            if (mods.Count == 0 && fbpacks == 0)
                            {
                                // no point continuing with this archive
                                errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "Archive contains no installable mods." });
                                continue;
                            }

                            // remove any invalid mods
                            for (int i = 0; i < mods.Count; i++)
                            {
                                string mod = mods[i];
                                if (format[i] == 0)
                                {
                                    // old legacy format requires an archive
                                    if (!archives.Contains(mod.Replace(".fbmod", "_01.archive")))
                                    {
                                        errors.Add(new ImportErrorInfo() { filename = mod, error = "Mod is missing the archive component." });
                                        mods.RemoveAt(i);
                                        i--;
                                        continue;
                                    }
                                }

                                // check for existing mod of same name
                                FrostyMod existingMod = availableMods.Find((IFrostyMod a) => { return a.Filename.ToLower().CompareTo(mod.ToLower()) == 0; }) as FrostyMod;
                                if (existingMod != null)
                                {
                                    availableMods.Remove(existingMod);
                                    foreach (FileInfo archiveFi in modsDir.GetFiles(mod.Replace(".fbmod", "") + "*.archive"))
                                    {
                                        File.Delete(archiveFi.FullName);
                                    }
                                }
                            }

                            // remove unreferenced .archives
                            for (int i = 0; i < archives.Count; i++)
                            {
                                string archive = archives[i];
                                if (!mods.Contains(archive.Replace("_01.archive", ".fbmod")))
                                {
                                    archives.RemoveAt(i);
                                    i--;
                                }
                            }

                            if (mods.Count > 0)
                            {
                                // now actually decompress files
                                decompressor.OpenArchive(filename);
                                foreach (CompressedFileInfo compressedFi in decompressor.EnumerateFiles())
                                {
                                    if (mods.Contains(compressedFi.Filename) || archives.Contains(compressedFi.Filename))
                                    {
                                        decompressor.DecompressToFile(Path.Combine(modsDir.FullName, compressedFi.Filename));
                                    }
                                }

                                // and add them to the mod manager
                                for (int i = 0; i < mods.Count; i++)
                                {
                                    fi = new FileInfo(Path.Combine(modsDir.FullName, mods[i]));
                                    lastInstalledMod = AddMod(fi.FullName, format[i]);
                                }
                            }
                        }
                        else if (fi.Extension == ".daimod")
                        {
                            // special handling for DAI mod files
                            // [DAI fix] moved to DaiImportMod / DaiConvertMod
                            lastInstalledMod = DaiImportMod(fi, errors) ?? lastInstalledMod;
                        }
                        else if (fi.Extension == ".fbcollection")
                        {
                            collections.Add(fi.Name);
                        }
                        else
                        {
                            // dont allow any files without fbmod extension
                            if (fi.Extension != ".fbmod")
                            {
                                if (fi.Extension == ".archive")
                                    continue;

                                errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "File is not a valid Frosty Mod." });
                                continue;
                            }

                            // make sure mod is designed for current profile
                            bool newFormat = false;
                            using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                            {
                                int retCode = VerifyMod(stream);
                                if ((retCode & 1) != 0)
                                {
                                    // continue with import (warning)
                                    errors.Add(new ImportErrorInfo { filename = fi.Name, error = "Mod was designed for a different game version, it may or may not work.", isWarning = true });
                                }
                                else if (retCode == -1)
                                {
                                    errors.Add(new ImportErrorInfo { filename = fi.Name, error = "File is not a valid Frosty Mod." });
                                }
                                else if (retCode == -2)
                                {
                                    errors.Add(new ImportErrorInfo { filename = fi.Name, error = "Mod was not designed for this game." });
                                    continue;
                                }
                                else if (retCode == -3)
                                {
                                    errors.Add(new ImportErrorInfo { filename = fi.Name, error = "Mod was found to be invalid and cannot be used" });
                                    continue;
                                }

                                if ((retCode & 0x8000) != 0)
                                    newFormat = true;
                            }

                            if (!newFormat)
                            {
                                // make sure mod has archive file
                                if (!File.Exists(fi.FullName.Replace(".fbmod", "_01.archive")))
                                {
                                    errors.Add(new ImportErrorInfo { filename = fi.Name, error = "Mod is missing the archive component." });
                                    continue;
                                }
                            }

                            // check for existing mod of same name
                            {
                                FrostyMod existingMod = availableMods.Find((IFrostyMod a) => a.Filename.ToLower().CompareTo(fi.Name.ToLower()) == 0) as FrostyMod;
                                if (existingMod != null)
                                {
                                    availableMods.Remove(existingMod);
                                    foreach (FileInfo archiveFi in modsDir.GetFiles(fi.Name.Replace(".fbmod", "") + "_*.archive"))
                                    {
                                        File.Delete(archiveFi.FullName);
                                    }
                                    File.Delete(modsDir.FullName + "/" + fi.Name);
                                }
                            }

                            // copy mod over
                            File.Copy(fi.FullName, Path.Combine(modsDir.FullName, fi.Name));
                            foreach (FileInfo archiveFi in fi.Directory.GetFiles(fi.Name.Replace(".fbmod", "") + "_*.archive"))
                            {
                                File.Copy(archiveFi.FullName, Path.Combine(modsDir.FullName, archiveFi.Name));
                            }

                            // add mod to manager
                            fi = new FileInfo(Path.Combine(modsDir.FullName, fi.Name));
                            lastInstalledMod = AddMod(fi.FullName, newFormat ? 1 : 0);
                        }

                        if (collections.Count > 0)
                        {
                            if (filename.Contains(".zip"))
                            {
                                // now actually decompress files
                                ZipDecompressor decompressor = new ZipDecompressor();
                                decompressor.OpenArchive(filename);
                                foreach (CompressedFileInfo compressedFi in decompressor.EnumerateFiles())
                                {
                                    if (collections.Contains(compressedFi.Filename))
                                    {
                                        decompressor.DecompressToFile(Path.Combine(modsDir.FullName, compressedFi.Filename));
                                    }
                                }
                            }
                            else if (filename.Contains(".fbcollection"))
                            {
                                File.Copy(fi.FullName, Path.Combine(modsDir.FullName, fi.Name));
                            }
                        }
                    }
                    catch (FrostyModLoadException e)
                    {
                        errors.Add(new ImportErrorInfo { error = e.Message, filename = fi.Name });
                        File.Delete(fi.FullName);
                    }
                }

                // add collections to the mod manager
                for (int i = 0; i < collections.Count; i++)
                {
                    FileInfo fi = new FileInfo(Path.Combine(modsDir.FullName, collections[i]));
                    lastInstalledMod = AddCollection(fi.FullName, 0);
                }

                // [DAI fix] free the game data loaded for .daimod imports
                DaiReleaseAssetManager();
            });

            ICollectionView view = CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);

            view.Refresh();
            view.GroupDescriptions.Clear();

            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ModDetails.Category");
            view.GroupDescriptions.Add(groupDescription);

            if (lastInstalledMod != null)
            {
                // set description to last installed mod
                tabControl.SelectedIndex = 1;
                availableModsList.SelectedItem = lastInstalledMod;
            }

            if (errors.Count > 0)
            {
                // show error window
                InstallErrorsWindow win = new InstallErrorsWindow(errors);
                win.ShowDialog();
            }

            if (packManifest != null)
            {
                if (AddPack(packManifest.name))
                {
                    foreach (string modName in packManifest.mods)
                    {
                        FrostyMod mod = availableMods.Find((IFrostyMod a) =>
                        {
                            return a.Filename.CompareTo(modName) == 0;
                        }) as FrostyMod;

                        if (mod != null)
                            selectedPack.AddMod(mod);
                    }

                    appliedModsList.Items.Refresh();

                    // focus on tab item
                    appliedModsTabItem.IsSelected = true;

                    FrostyMessageBox.Show("Pack has been successfully imported", "Frosty Mod Manager");
                }
            }
        }

        private bool IsCompressed(FileInfo fi) => fi.Extension == ".rar" || fi.Extension == ".zip" || fi.Extension == ".7z" || fi.Extension == ".fbpack";

        private void launchOptionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LaunchOptionsWindow win = new LaunchOptionsWindow();
            win.ShowDialog();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow();
            win.ShowDialog();
        }

        private void modDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManageModDataWindow win = new ManageModDataWindow();
            win.ShowDialog();
        }

        private void appliedModsList_SelectionChanged(object sender, SelectionChangedEventArgs e) => updateAppliedModButtons();

        private void updateAppliedModButtons()
        {
            if (appliedModsList.SelectedItem != null)
            {
                removeButton.IsEnabled = true;

                if (orderComboBox.SelectedIndex == 0)
                {
                    upButton.IsEnabled = appliedModsList.SelectedIndex != 0;
                    downButton.IsEnabled = appliedModsList.SelectedIndex != (appliedModsList.Items.Count - 1);
                }
                else if (orderComboBox.SelectedIndex == 1)
                {
                    upButton.IsEnabled = appliedModsList.SelectedIndex != (appliedModsList.Items.Count - 1);
                    downButton.IsEnabled = appliedModsList.SelectedIndex != 0;
                }
            }
            else
            {
                removeButton.IsEnabled = false;
                upButton.IsEnabled = false;
                downButton.IsEnabled = false;
            }
        }

        private void availableModsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (availableModsList.SelectedIndex == -1)
                return;

            IFrostyMod selectedMod = availableModsList.SelectedItem as IFrostyMod;
            selectedPack.AddMod(selectedMod);
            appliedModsList.Items.Refresh();

            // focus on tab item
            appliedModsTabItem.IsSelected = true;
        }

        public List<Control> AllChildren(DependencyObject parent)
        {
            List<Control> children = new List<Control>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Control)
                    children.Add(child as Control);
                children.AddRange(AllChildren(child));
            }
            return children;
        }

        private void addModButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (IFrostyMod mod in availableModsList.SelectedItems)
                selectedPack.AddMod(mod);

            foreach (IFrostyMod item in availableModsList.Items)
            {
                DependencyObject container = availableModsList.ItemContainerGenerator.ContainerFromItem(item);
                ListView collectionModsList = AllChildren(container).OfType<ListView>().ToList().FirstOrDefault(x => x.Name.Equals("collectionModsList"));

                if (collectionModsList != null)
                    foreach (IFrostyMod mod in collectionModsList.SelectedItems)
                        selectedPack.AddMod(mod);
            }

            appliedModsList.Items.Refresh();

            // focus on tab item
            appliedModsTabItem.IsSelected = true;
        }

        private void SelectedProfile_AppliedModsUpdated(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedItem == conflictsTabItem)
                UpdateConflicts();

            conflictsTabItem.Visibility = Visibility.Visible;
        }

        private void UpdateConflicts()
        {
            bool onlyShowReplacements = (bool)showOnlyReplacementsCheckBox.IsChecked;

            StringBuilder sb = new StringBuilder();
            List<ModResourceInfo> totalResourceList = new List<ModResourceInfo>();

            CancellationTokenSource cancelToken = new CancellationTokenSource();

            bool cancelled = false;
            FrostyTaskWindow.Show("Updating Actions", "", (task) => {
                try
                {
                    // Iterate through mod resources
                    for (int i = 0; i < selectedPack.AppliedMods.Count; i++)
                    {
                        FrostyAppliedMod appliedMod = selectedPack.AppliedMods[i];
                        if (!appliedMod.IsFound || !appliedMod.IsEnabled)
                            continue;


                        FrostyMod[] mods;
                        if (appliedMod.Mod is FrostyModCollection)
                        {
                            mods = (appliedMod.Mod as FrostyModCollection).Mods.ToArray();
                        }
                        else
                        {
                            mods = new FrostyMod[1];
                            mods[0] = appliedMod.Mod as FrostyMod;
                        }

                        foreach (var mod in mods)
                        {
                            if (mod.NewFormat)
                            {
                                foreach (BaseModResource resource in mod.Resources)
                                {
                                    if (resource.Type == ModResourceType.Embedded)
                                        continue;

                                    string resType = resource.Type.ToString().ToLower();
                                    string resourceName = resource.Name;

                                    if (resource.UserData != "")
                                    {
                                        string[] arr = resource.UserData.Split(';');
                                        resType = arr[0].ToLower();
                                        resourceName = arr[1];
                                    }

                                    int index = totalResourceList.FindIndex((ModResourceInfo a) => a.Equals(resType + "/" + resourceName));

                                    if (index == -1)
                                    {
                                        ModResourceInfo resInfo = new ModResourceInfo(resourceName, resType);
                                        totalResourceList.Add(resInfo);
                                        index = totalResourceList.Count - 1;
                                    }

                                    cancelToken.Token.ThrowIfCancellationRequested();

                                    ModPrimaryActionType primaryAction = ModPrimaryActionType.None;
                                    if (resource.HasHandler)
                                    {
                                        if ((uint)resource.Handler == 0xBD9BFB65)
                                            primaryAction = ModPrimaryActionType.Merge;
                                        else
                                        {
                                            ICustomActionHandler handler = null;
                                            if (resource.Type == ModResourceType.Ebx)
                                                handler = App.PluginManager.GetCustomHandler((uint)resource.Handler);
                                            else if (resource.Type == ModResourceType.Res)
                                                handler = App.PluginManager.GetCustomHandler((ResourceType)(resource as ResResource).ResType);

                                            if (handler.Usage == HandlerUsage.Merge)
                                            {
                                                foreach (string actionString in handler.GetResourceActions(resource.Name, mod.GetResourceData(resource)))
                                                {
                                                    string[] arr = actionString.Split(';');
                                                    AddResourceAction(totalResourceList, mod.Filename, arr[0], arr[1], (ModPrimaryActionType)Enum.Parse(typeof(ModPrimaryActionType), arr[2]));
                                                }
                                                primaryAction = ModPrimaryActionType.Merge;
                                            }
                                            else primaryAction = ModPrimaryActionType.Modify;
                                        }
                                    }
                                    else if (resource.IsAdded) primaryAction = ModPrimaryActionType.Add;
                                    else if (resource.IsModified) primaryAction = ModPrimaryActionType.Modify;

                                    totalResourceList[index].AddMod(mod.Filename, primaryAction, resource.AddedBundles);
                                }
                            }
                        }
                    }

                }
                catch (OperationCanceledException)
                {
                    cancelled = true;
                }

                if (onlyShowReplacements)
                    totalResourceList.RemoveAll(item => item.ModCount <= 1);
            }, showCancelButton: true, cancelCallback: (task) => cancelToken.Cancel());

            if (cancelled)
            {
                Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedItem = appliedModsTabItem));
                return;
            }

            List<GridViewColumn> columns = new List<GridViewColumn>
            {
                new GridViewColumn
                {
                    Header = "Resource",
                    CellTemplate = conflictsListView.Resources["conflictsNameTemplate"] as DataTemplate
                }
            };

            for (int i = 0; i < selectedPack.AppliedMods.Count; i++)
            {
                FrostyAppliedMod appliedMod = selectedPack.AppliedMods[i];
                if (!appliedMod.IsFound || !appliedMod.IsEnabled)
                    continue;

                Binding primaryActionBinding = new Binding("") { Converter = new ModPrimaryActionConverter(), ConverterParameter = appliedMod.Mod.Filename };
                Binding primaryTooltipBinding = new Binding("") { Converter = new ModPrimaryActionTooltipConverter(), ConverterParameter = appliedMod.Mod.Filename };
                Binding secondaryActionBinding = new Binding("") { Converter = new ModSecondaryActionConverter(), ConverterParameter = appliedMod.Mod.Filename };
                Binding secondaryTooltipBinding = new Binding("") { Converter = new ModSecondaryActionTooltipConverter(), ConverterParameter = appliedMod.Mod.Filename };

                GridViewColumn gvc = new GridViewColumn() { Header = appliedMod.Mod.ModDetails.Title, HeaderTemplate = conflictsListView.Resources["conflictsModHeaderTemplate"] as DataTemplate };
                DataTemplate dt = new DataTemplate(typeof(Grid));

                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(StackPanel));
                factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                factory.SetValue(StackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                factory.SetValue(Grid.HeightProperty, 22.0d);

                FrameworkElementFactory img = new FrameworkElementFactory(typeof(Image));
                img.SetBinding(Image.SourceProperty, primaryActionBinding);
                img.SetValue(Image.HeightProperty, 18.0d);
                img.SetValue(Image.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                img.SetValue(Image.VerticalAlignmentProperty, VerticalAlignment.Center);
                img.SetValue(Image.OpacityProperty, 0.75d);
                img.SetValue(Image.ToolTipProperty, primaryTooltipBinding);

                FrameworkElementFactory simg = new FrameworkElementFactory(typeof(Image));
                simg.SetBinding(Image.SourceProperty, secondaryActionBinding);
                simg.SetValue(Image.HeightProperty, 18.0d);
                simg.SetValue(Image.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                simg.SetValue(Image.VerticalAlignmentProperty, VerticalAlignment.Center);
                simg.SetValue(Image.OpacityProperty, 0.75d);
                simg.SetValue(Image.ToolTipProperty, secondaryTooltipBinding);

                factory.AppendChild(img);
                factory.AppendChild(simg);

                dt.VisualTree = factory;
                gvc.CellTemplate = dt;
                gvc.Width = 150;

                columns.Add(gvc);
            }

            GridView gv = conflictsListView.View as GridView;

            gv.Columns.Clear();
            foreach (GridViewColumn gvc in columns)
                gv.Columns.Add(gvc);

            totalResourceList.Sort((ModResourceInfo a, ModResourceInfo b) =>
            {
                int result = a.Type[1].CompareTo(b.Type[1]);
                return result == 0 ? a.Name.CompareTo(b.Name) : result;
            });

            Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedItem = conflictsTabItem));
            conflictsListView.ItemsSource = totalResourceList;
            conflictsListView.SelectedIndex = 0;
        }

        private void AddResourceAction(List<ModResourceInfo> totalResourceList, string modName, string resourceName, string resourceType, ModPrimaryActionType type)
        {
            int index = totalResourceList.FindIndex((ModResourceInfo a) => a.Equals(resourceType + "/" + resourceName));
            if (index == -1)
            {
                ModResourceInfo resInfo = new ModResourceInfo(resourceName, resourceType);
                totalResourceList.Add(resInfo);
                index = totalResourceList.Count - 1;
            }

            totalResourceList[index].AddMod(modName, type, null);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (conflictsTabItem.IsSelected)
            {
                UpdateConflicts();
            }
        }

        private void launchConfigurationWindow_Click(object sender, RoutedEventArgs e)
        {
            Config.Save();
            //Config.Save(App.configFilename);

            Windows.PrelaunchWindow2 SelectConfiguration = new Windows.PrelaunchWindow2();
            App.Current.MainWindow = SelectConfiguration;
            SelectConfiguration.Show();
            Close();
        }

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb.IsFocused)
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            tb.ScrollToEnd();
        }

        private void availableModsFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                availableModsFilter_LostFocus(this, new RoutedEventArgs());
        }

        private void availableModsFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (availableModsFilterTextBox.Text == "")
            {
                availableModsList.Items.Filter = null;
                return;
            }

            availableModsList.Items.Filter = new Predicate<object>((object a) => ((IFrostyMod)a).ModDetails.Title.ToLower().Contains(availableModsFilterTextBox.Text.ToLower()));
        }

        private void PART_ShowOnlyReplacementsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConflicts();
        }

        private void optionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow win = new OptionsWindow();
            win.ShowDialog();
        }

        private void ZipPack(string filename)
        {
            FrostyTaskWindow.Show("Exporting Pack", "", (task) =>
            {
                try
                {
                    List<string> mods = new List<string>();
                    foreach (FrostyAppliedMod mod in selectedPack.AppliedMods)
                    {
                        if (mod.IsFound && mod.IsEnabled)
                            mods.Add(mod.Mod.Filename);
                    }

                    PackManifest manifest = new PackManifest()
                    {
                        managerVersion = App.Version,
                        version = manifestVersion,
                        name = selectedPack.Name,
                        mods = mods
                    };

                    using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Create))
                    {
                        foreach (FrostyAppliedMod mod in selectedPack.AppliedMods)
                        {
                            if (mod.Mod is FrostyModCollection)
                                continue;
                            archive.CreateEntryFromFile((mod.Mod as FrostyMod).Path, mod.Mod.Filename);
                        }

                        ZipArchiveEntry manifestEntry = archive.CreateEntry("manifest.json");
                        using (Stream stream = manifestEntry.Open())
                        {
                            byte[] buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(manifest, Formatting.Indented));

                            stream.Write(buffer, 0, buffer.Length);
                        }

                        archive.Dispose();
                    }
                }
                catch
                {
                    File.Delete(filename);
                }
            });
        }

        private void packImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "*.fbpack;*.zip (Fb Pack) |*.fbpack;*.zip",
                Title = "Import Pack",
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                InstallMods(ofd.FileNames);
            }
        }

        private void packExport_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Pack As", "*.fbpack (FBPack)|*.fbpack", "FBPack");
            if (sfd.ShowDialog())
            {
                if (File.Exists(sfd.FileName))
                    FrostyMessageBox.Show("A file with the same name already exists", "Frosty Mod Manager");
                else
                    ZipPack(sfd.FileName);
            }
        }

        private void importPackButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "*.fbpack;*.zip (Fb Pack) |*.fbpack;*.zip",
                Title = "Import Pack",
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                InstallMods(ofd.FileNames);
            }
        }

        private void collectionExport_Click(object sender, RoutedEventArgs e)
        {
            var ew = new Windows.CollectionSettingsWindow(selectedPack.AppliedMods);
            ew.ShowDialog();
        }

        private void collectionModsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView collectionModsList = (ListView)sender;
            if (collectionModsList.SelectedIndex == -1)
                return;

            availableModsList.SelectedIndex = -1;

            IFrostyMod selectedMod = collectionModsList.SelectedItem as IFrostyMod;
            selectedPack.AddMod(selectedMod);
            appliedModsList.Items.Refresh();

            // focus on tab item
            appliedModsTabItem.IsSelected = true;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ListBox && !e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void collectionModsList_LostFocus(object sender, RoutedEventArgs e) {
            ((ListView)sender).UnselectAll();
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Setter setter = new Setter(DockPanel.DockProperty, Dock.Top);
            switch (orderComboBox.SelectedIndex)
            {
                case 0:
                    setter = new Setter(DockPanel.DockProperty, Dock.Top);
                    Config.Add("ApplyModOrder", "List");
                    break;
                case 1:
                    setter = new Setter(DockPanel.DockProperty, Dock.Bottom);
                    Config.Add("ApplyModOrder", "Priority");
                    break;
            }
            Style style = new Style(typeof(ListBoxItem), FindResource(typeof(ListBoxItem)) as Style);
            style.Setters.Add(setter);
            appliedModsList.ItemContainerStyle = style;

            updateAppliedModButtons();
        }

        private void CopyFullExceptionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Plugin selectedPlugin = (Plugin)LoadedPluginsList.SelectedItem;

            // retrieve the selected plugin's load exception, execute ToString on it, and add the result to the clipboard
            Clipboard.SetText(string.Format("[{0}]\n{1}", new string[]
            {
                DateTime.Now.ToString(),
                selectedPlugin.LoadException.ToString()
            }));
        }

        private void LoadMenuExtensions()
        {
            // Add menu extensions to Mod Manager
            foreach (var menuExtension in App.PluginManager.MenuExtensions)
            {
                MenuItem foundMenuItem = null;
                foreach (MenuItem menuItem in menu.Items)
                {
                    // contains top level menu item already
                    if (menuExtension.TopLevelMenuName.Equals(menuItem.Header as string, StringComparison.OrdinalIgnoreCase))
                    {
                        foundMenuItem = menuItem;
                        break;
                    }
                }

                if (foundMenuItem == null)
                {
                    foundMenuItem = new MenuItem() { Header = menuExtension.TopLevelMenuName };

                    // insert the top-level Menu behind the Help Menu
                    menu.Items.Insert(menu.Items.Count - 1, foundMenuItem);
                }

                if (!string.IsNullOrEmpty(menuExtension.SubLevelMenuName))
                {
                    MenuItem parentMenuItem = null;
                    foreach (var menuItem in foundMenuItem.Items)
                    {
                        if (menuItem is MenuItem item)
                        {
                            if (menuExtension.SubLevelMenuName.Equals(item.Header as string, StringComparison.OrdinalIgnoreCase))
                            {
                                parentMenuItem = foundMenuItem;
                                foundMenuItem = item;
                                break;
                            }
                        }
                    }

                    if (parentMenuItem == null)
                    {
                        parentMenuItem = foundMenuItem;
                        foundMenuItem = new MenuItem { Header = menuExtension.SubLevelMenuName };
                        parentMenuItem.Items.Add(foundMenuItem);
                    }
                }

                MenuItem menuExtItem = new MenuItem
                {
                    Header = menuExtension.MenuItemName,
                    Icon = new Image() { Source = menuExtension.Icon },
                    Command = menuExtension.MenuItemClicked
                };
                foundMenuItem.Items.Add(menuExtItem);
            }
        }
    }
}
