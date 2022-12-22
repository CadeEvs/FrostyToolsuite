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
using FrostySdk.Managers.Entries;
using Newtonsoft.Json;

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
        private FileSystemManager fs => Frosty.Core.App.FileSystemManager;

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

            string selectedProfileName = FrostyProfileSelectWindow.Show();
            if (!string.IsNullOrEmpty(selectedProfileName))
            {
                Frosty.Core.App.ClearProfileData();
                if (!Frosty.Core.App.LoadProfile(selectedProfileName))
                {
                    Closing -= FrostyWindow_Closing;
                    Close();
                    return;
                }
            }
            else
            {
                Closing -= FrostyWindow_Closing;
                Close();
                return;
            }

            Config.Save();
            Title = "Frosty Mod Manager - " + Frosty.Core.App.Version + " (" + ProfilesLibrary.DisplayName + ")";

            FrostyTaskWindow.Show("Loading Mods", "", (task) =>
            {
                DirectoryInfo di = new DirectoryInfo("Mods/" + ProfilesLibrary.ProfileName);
                if (!di.Exists)
                    Directory.CreateDirectory(di.FullName);


                // load mods
                foreach (FileInfo fi in di.EnumerateFiles())
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
                }
                // load collections
                foreach (FileInfo fi in di.EnumerateFiles())
                {
                    if (fi.Extension == ".fbcollection")
                    {
                        AddCollection(fi.FullName, 0);
                    }
                }
            });
            availableModsList.ItemsSource = availableMods;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ModDetails.Category", null, StringComparison.OrdinalIgnoreCase);
            view.GroupDescriptions.Add(groupDescription);

            foreach (string packName in Config.EnumerateKeys(ConfigScope.Pack))
            {
                string values = Config.Get(packName, "", ConfigScope.Pack);
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

            string selectedPackName = Config.Get<string>("SelectedPack", "", ConfigScope.Game);
            int selectedIndex = 0;
            if (selectedPackName != null)
            {
                selectedIndex = packs.FindIndex((FrostyPack a) => a.Name == selectedPackName);
                if (selectedIndex == -1)
                    selectedIndex = 0;
            }
            packsComboBox.SelectedIndex = selectedIndex;

            if (Config.Get("CollapseCategories", false))
            {
            }

            LoadedPluginsList.ItemsSource = App.PluginManager.LoadedPlugins;
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
                packs.Remove(selectedPack);

                DirectoryInfo di = new DirectoryInfo(fs.BasePath + "ModData\\" + selectedPack.Name);
                if (di.Exists)
                    di.Delete(true);


                packsComboBox.Items.Refresh();
                packsComboBox.SelectedIndex = 0;
            }
        }

        private void packRename_Click(object sender, RoutedEventArgs e)
        {

            AddProfileWindow win = new AddProfileWindow("Rename Pack");
            win.ShowDialog();

            if (win.DialogResult == true)
            {
                string newPackName = win.ProfileName;
                var oldPack = selectedPack;

                FrostyPack existingPack = packs.Find((FrostyPack a) => {
                    return a.Name.CompareTo(newPackName) == 0;
                });

                if (existingPack == null)
                {
                    Config.Rename(oldPack.Name, newPackName, ConfigScope.Pack);

                    FrostyPack newPack = new FrostyPack(newPackName);
                    foreach (FrostyAppliedMod mod in oldPack.AppliedMods)
                        newPack.AppliedMods.Add(mod);

                    packs.Add(newPack);
                    packs.Remove(oldPack);

                    packsComboBox.Items.Refresh();
                    packsComboBox.SelectedItem = newPack;
                }
                else
                    FrostyMessageBox.Show("A pack with the same name already exists", "Frosty Mod Manager");

            }
        }

        private void packDuplicate_Click(object sender, RoutedEventArgs e)
        {

            AddProfileWindow win = new AddProfileWindow("Duplicate Pack");
            win.ShowDialog();

            if (win.DialogResult == true)
            {
                string newPackName = win.ProfileName;
                var oldPack = selectedPack;

                FrostyPack existingPack = packs.Find((FrostyPack a) => {
                    return a.Name.CompareTo(newPackName) == 0;
                });

                if (existingPack == null)
                {
                    Config.Add(newPackName, ConfigScope.Pack);

                    FrostyPack newPack = new FrostyPack(newPackName);
                    foreach (FrostyAppliedMod mod in oldPack.AppliedMods)
                        newPack.AppliedMods.Add(mod);

                    packs.Add(newPack);

                    packsComboBox.Items.Refresh();
                    packsComboBox.SelectedItem = newPack;
                }
                else
                    FrostyMessageBox.Show("A pack with the same name already exists", "Frosty Mod Manager");
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
            for (int i = 0; i < (Keyboard.IsKeyDown(Key.LeftShift) ? 4 : 1); i++)
                selectedPack.MoveModsUp(appliedModsList.SelectedItems);

            if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                selectedPack.MoveModsTop(appliedModsList.SelectedItems);

            appliedModsList.Items.Refresh();

            updateAppliedModButtons();
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < (Keyboard.IsKeyDown(Key.LeftShift) ? 4 : 1); i++)
                selectedPack.MoveModsDown(appliedModsList.SelectedItems);

            if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                selectedPack.MoveModsBottom(appliedModsList.SelectedItems);

            appliedModsList.Items.Refresh();

            updateAppliedModButtons();
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Save();

            // initialize
            Frosty.Core.App.FileSystemManager = new FileSystemManager(Config.Get<string>("GamePath", "", ConfigScope.Game));
            foreach (FileSystemSource source in ProfilesLibrary.Sources)
                Frosty.Core.App.FileSystemManager.AddSource(source.Path, source.SubDirs);
            Frosty.Core.App.FileSystemManager.Initialize();

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
                    retCode = modExecutor.Run(fs, cancelToken.Token, task.TaskLogger, $"Mods/{ProfilesLibrary.ProfileName}/", App.SelectedPack, additionalArgs.Trim(), modPaths.ToArray());

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
            availableMods.Add(mod);

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

            foreach (var mod in collection.Mods)
            {
                int index = availableMods.FindIndex((IFrostyMod a) => a.Filename == mod.Filename);
                if (index != -1)
                    availableMods.RemoveAt(index);
            }

            availableMods.Add(collection);

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
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                InstallMods(filenames);

                ICollectionView view = CollectionViewSource.GetDefaultView(availableModsList.ItemsSource);
                view.Refresh();
            }
        }

        private void InstallMods(string[] filenames)
        {
            IFrostyMod lastInstalledMod = null;
            List<ImportErrorInfo> errors = new List<ImportErrorInfo>();

            PackManifest packManifest = null;

            FrostyTaskWindow.Show("Installing Mods", "", (task) =>
            {
                foreach (string filename in filenames)
                {
                    FileInfo fi = new FileInfo(filename);
                    task.Update(fi.Name);

                    try
                    {
                        if (IsCompressed(fi))
                        {
                            List<string> mods = new List<string>();
                            List<string> collections = new List<string>();
                            List<int> format = new List<int>();
                            List<string> archives = new List<string>();
                            int fbpacks = 0;

                            // create decompressor
                            IDecompressor decompressor = null;
                            if (fi.Extension == ".rar") decompressor = new RarDecompressor();
                            else if (fi.Extension == ".zip" || fi.Extension == ".fbpack") decompressor = new ZipDecompressor();
                            else if (fi.Extension == ".7z") decompressor = new SevenZipDecompressor();

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
                                    Dispatcher.Invoke(() => {
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
                                    DirectoryInfo di = new DirectoryInfo("Mods/" + ProfilesLibrary.ProfileName + "/");
                                    foreach (FileInfo archiveFi in di.GetFiles(mod.Replace(".fbmod", "") + "*.archive"))
                                        File.Delete(archiveFi.FullName);
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
                                        decompressor.DecompressToFile("Mods/" + ProfilesLibrary.ProfileName + "/" + compressedFi.Filename);
                                    }
                                }
                                decompressor.CloseArchive();

                                // and add them to the mod manager
                                for (int i = 0; i < mods.Count; i++)
                                {
                                    fi = new FileInfo("Mods/" + ProfilesLibrary.ProfileName + "/" + mods[i]);
                                    lastInstalledMod = AddMod(fi.FullName, format[i]);
                                }
                            }

                            if (collections.Count > 0)
                            {
                                // now actually decompress files
                                decompressor.OpenArchive(filename);
                                foreach (CompressedFileInfo compressedFi in decompressor.EnumerateFiles())
                                {
                                    if (collections.Contains(compressedFi.Filename))
                                    {
                                        decompressor.DecompressToFile("Mods/" + ProfilesLibrary.ProfileName + "/" + compressedFi.Filename);
                                    }
                                }
                                decompressor.CloseArchive();

                                // and add them to the mod manager
                                for (int i = 0; i < collections.Count; i++)
                                {
                                    fi = new FileInfo("Mods/" + ProfilesLibrary.ProfileName + "/" + collections[i]);
                                    lastInstalledMod = AddCollection(fi.FullName, 0);
                                }
                            }
                        }
                        else if (fi.Extension == ".daimod")
                        {
                            // special handling for DAI mod files
                            using (NativeReader reader = new NativeReader(new FileStream(fi.FullName, FileMode.Open)))
                            {
                                string magic = reader.ReadSizedString(8);
                                if (magic != "DAIMODV2")
                                {
                                    errors.Add(new ImportErrorInfo() { filename = fi.Name, error = "File is not a valid DAI Mod." });
                                    continue;
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

                                string configValues = "";
                                if (code != "")
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        ConfigWindow win = new ConfigWindow(code, resources, shouldWrite);
                                        win.ShowDialog();

                                        configValues = win.GetConfigValues();
                                    });
                                }

                                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                                xmlDoc.LoadXml(xml);

                                System.Xml.XmlElement elem = xmlDoc["daimod"]["details"];
                                string newDesc = "(Converted from .daimod)\r\n\r\n" + elem["description"].InnerText + "\r\n\r\n" + configValues;

                                DbObject modObject = new DbObject();
                                modObject.AddValue("magic", "FBMODV2");
                                modObject.AddValue("gameProfile", ProfilesLibrary.ProfileName);
                                modObject.AddValue("gameVersion", 0);

                                modObject.AddValue("title", elem["name"].InnerText);
                                modObject.AddValue("author", elem["author"].InnerText);
                                modObject.AddValue("category", "DAI Mods");
                                modObject.AddValue("version", elem["version"].InnerText);
                                modObject.AddValue("description", newDesc);

                                DbObject resourcesList = new DbObject(false);
                                DbObject actionsList = new DbObject(false);
                                DbObject screenshotList = new DbObject(false);
                                long offset = 0;

                                int index = -1;
                                int actualIndex = 0;

                                foreach (System.Xml.XmlElement subElem in xmlDoc["daimod"]["resources"])
                                {
                                    index++;
                                    if (subElem.GetAttribute("action") == "remove")
                                        continue;

                                    int resId = int.Parse(subElem.GetAttribute("resourceId"));
                                    if (shouldWrite[resId] == false)
                                        continue;

                                    int resSize = resources[resId].Length;
                                    string type = subElem.GetAttribute("type");

                                    DbObject resource = new DbObject();
                                    resource.AddValue("name", subElem.GetAttribute("name"));
                                    resource.AddValue("type", type);

                                    resource.AddValue("sha1", new Sha1(subElem.GetAttribute("sha1")));
                                    resource.AddValue("originalSize", 0);
                                    resource.AddValue("compressedSize", resSize);
                                    resource.AddValue("archiveIndex", 1);
                                    resource.AddValue("archiveOffset", offset);
                                    resource.AddValue("shouldInline", false);

                                    string actionString = subElem.GetAttribute("action");
                                    if (type == "ebx" || type == "res")
                                    {
                                        resource.AddValue("uncompressedSize", int.Parse(subElem.GetAttribute("originalSize")));
                                        if (actionString != "add")
                                            resource.AddValue("originalSha1", new Sha1(subElem.GetAttribute("originalSha1")));
                                        actionString = "modify";

                                        if (type == "res")
                                        {
                                            resource.AddValue("resType", uint.Parse(subElem.GetAttribute("resType")));
                                            if (resource.GetValue<uint>("resType") == 0x5C4954A6)
                                                resource.SetValue("shouldInline", true);
                                            resource.AddValue("resRid", (ulong)long.Parse(subElem.GetAttribute("resRid")));

                                            string resMetaString = subElem.GetAttribute("meta");
                                            byte[] resMeta = new byte[resMetaString.Length / 2];
                                            for (int i = 0; i < resMeta.Length; i++)
                                                resMeta[i] = byte.Parse(resMetaString.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);

                                            resource.AddValue("resMeta", resMeta);
                                        }
                                    }
                                    else
                                    {
                                        string chunkid = subElem.GetAttribute("name");
                                        byte[] chunkidBytes = new byte[chunkid.Length / 2];
                                        for (int i = 0; i < chunkidBytes.Length; i++)
                                            chunkidBytes[i] = byte.Parse(chunkid.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);

                                        resource.SetValue("name", (new Guid(chunkidBytes)).ToString());
                                        resource.AddValue("rangeStart", uint.Parse(subElem.GetAttribute("rangeStart")));
                                        resource.AddValue("rangeEnd", uint.Parse(subElem.GetAttribute("rangeEnd")));
                                        resource.AddValue("logicalOffset", uint.Parse(subElem.GetAttribute("logicalOffset")));
                                        resource.AddValue("logicalSize", uint.Parse(subElem.GetAttribute("logicalSize")));

                                        if (subElem.GetAttribute("meta") != "00")
                                            resource.SetValue("firstMip", 3);

                                        // add special chunks bundle
                                        DbObject action = new DbObject();
                                        action.AddValue("resourceId", resourcesList.Count - 1);
                                        action.AddValue("type", "add");
                                        action.AddValue("bundle", "chunks");
                                        actionsList.Add(action);
                                    }

                                    foreach (System.Xml.XmlElement bundleElem in xmlDoc["daimod"]["bundles"])
                                    {
                                        foreach (System.Xml.XmlElement entryElem in bundleElem["entries"])
                                        {
                                            int id = int.Parse(entryElem.GetAttribute("id"));
                                            if (id == index)
                                            {
                                                DbObject action = new DbObject();
                                                action.AddValue("resourceId", actualIndex);
                                                action.AddValue("type", actionString);
                                                action.AddValue("bundle", bundleElem.GetAttribute("name"));
                                                actionsList.Add(action);
                                            }
                                        }
                                    }

                                    resourcesList.Add(resource);
                                    offset += resSize;
                                    actualIndex++;
                                }

                                modObject.AddValue("screenshots", screenshotList);
                                modObject.AddValue("resources", resourcesList);
                                modObject.AddValue("actions", actionsList);

                                using (DbWriter writer = new DbWriter(new FileStream("Mods/" + ProfilesLibrary.ProfileName + "/" + fi.Name.Replace(".daimod", ".fbmod"), FileMode.Create)))
                                    writer.Write(modObject);
                                using (NativeWriter writer = new NativeWriter(new FileStream("Mods/" + ProfilesLibrary.ProfileName + "/" + fi.Name.Replace(".daimod", "_01.archive"), FileMode.Create)))
                                {
                                    for (int i = 0; i < resources.Count; i++)
                                    {
                                        if (shouldWrite[i])
                                            writer.Write(resources[i]);
                                    }
                                }

                                fi = new FileInfo("Mods/" + ProfilesLibrary.ProfileName + "/" + fi.Name.Replace(".daimod", ".fbmod"));
                                lastInstalledMod = AddMod(fi.FullName, 0);
                            }
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
                                    DirectoryInfo di = new DirectoryInfo("Mods/" + ProfilesLibrary.ProfileName + "/");
                                    foreach (FileInfo archiveFi in di.GetFiles(fi.Name.Replace(".fbmod", "") + "_*.archive"))
                                        File.Delete(archiveFi.FullName);
                                    File.Delete(di.FullName + "/" + fi.Name);
                                }
                            }

                            // copy mod over
                            File.Copy(fi.FullName, "Mods/" + ProfilesLibrary.ProfileName + "/" + fi.Name);
                            foreach (FileInfo archiveFi in fi.Directory.GetFiles(fi.Name.Replace(".fbmod", "") + "_*.archive"))
                                File.Copy(archiveFi.FullName, "Mods/" + ProfilesLibrary.ProfileName + "/" + archiveFi.Name);

                            // add mod to manager
                            fi = new FileInfo("Mods/" + ProfilesLibrary.ProfileName + "/" + fi.Name);
                            lastInstalledMod = AddMod(fi.FullName, newFormat ? 1 : 0);
                        }
                    }
                    catch (FrostyModLoadException e)
                    {
                        errors.Add(new ImportErrorInfo { error = e.Message, filename = fi.Name });
                        File.Delete(fi.FullName);
                    }
                }
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
                upButton.IsEnabled = appliedModsList.SelectedIndex != 0;
                downButton.IsEnabled = appliedModsList.SelectedIndex != (appliedModsList.Items.Count - 1);
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

        private void addModButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (IFrostyMod mod in availableModsList.SelectedItems)
                selectedPack.AddMod(mod);

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
            FrostyTaskWindow.Show("Updating Actions", "", (task) =>
            {
                try
                {
                    // Iterate through mod resources
                    for (int i = 0; i < selectedPack.AppliedMods.Count; i++)
                    {
                        FrostyAppliedMod appliedMod = selectedPack.AppliedMods[i];
                        if (!appliedMod.IsFound && !appliedMod.IsEnabled)
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
                                            else if (resource.Type == ModResourceType.FsFile)
                                                handler = (ICustomActionHandler)App.PluginManager.GetCustomAssetHandler("fs");

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
                if (!appliedMod.IsFound && !appliedMod.IsEnabled)
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
                        managerVersion = Frosty.Core.App.Version,
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
                Filter = "*.fbpack;*.zip (Frostbite Pack) | *.fbpack;*.zip",
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

        private void collectionModsList_LostFocus(object sender, RoutedEventArgs e)
        {
            ((ListView)sender).UnselectAll();
        }
    }
}
