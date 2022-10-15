using Bookmarks = Frosty.Core.Bookmarks;
using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Commands;
using Frosty.Core.Controls;
using Frosty.Core.Converters;
using Frosty.Core.Interfaces;
using Frosty.Core.Legacy;
using Frosty.Core.Mod;
using Frosty.Core.Windows;
using Frosty.ModSupport;
using FrostyCore;
using FrostyEditor.Windows;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace FrostyEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FrostyWindow, IEditorWindow
    {
        private static ImageSourceConverter m_imageSourceConverter = new ImageSourceConverter();

        private MenuItem m_clearRecentsMenuItem = new MenuItem
        {
            Header = "Clear Recents",
            Height = 22,
            Icon = new Image
            {
                Source = (ImageSource)m_imageSourceConverter.ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Clear.png")
            }
        };

        public FrostyDataExplorer m_currentExplorer;

        private System.Timers.Timer m_autoSaveTimer;

        private FrostyProject m_project;

        private List<string> m_recentProjects = Config.Get("RecentProjects", new List<string>(), ConfigScope.Game);

        public FrostyDataExplorer DataExplorer => dataExplorer;

        public FrostyDataExplorer LegacyExplorer => legacyExplorer;

        public TabControl MiscTabControl => miscTabControl;

        public FrostyProject Project => m_project;

        public FrostyDataExplorer VisibleExplorer => m_currentExplorer;

        public ItemDoubleClickCommand BookmarkItemDoubleClickCommand { get; private set; }

        public MainWindow()
        {
            FrostySdk.Attributes.GlobalAttributes.DisplayModuleInClassId = Config.Get<bool>("DisplayModuleInId", false);
            BookmarkItemDoubleClickCommand = new ItemDoubleClickCommand(BookmarkTreeView_MouseDoubleClick);
            Bookmarks.BookmarkDb.LoadDb();
            m_project = new FrostyProject();

            InitializeComponent();

            explorerTabContent.HeaderControl = explorerTabControl;
            tabContent.HeaderControl = tabControl;
            miscTabContent.HeaderControl = miscTabControl;

            UpdateWindowTitle();

            RoutedCommand newCmd = new RoutedCommand();
            RoutedCommand openCmd = new RoutedCommand();
            RoutedCommand saveCmd = new RoutedCommand();
            RoutedCommand saveAsCmd = new RoutedCommand();
            RoutedCommand addBookmarkCmd = new RoutedCommand();
            RoutedCommand removeBookmarkCmd = new RoutedCommand();
            RoutedCommand launchGameCmd = new RoutedCommand();
            RoutedCommand focusAssetFilterCmd = new RoutedCommand();

            newCmd.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            openCmd.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            saveCmd.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            saveAsCmd.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            addBookmarkCmd.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control));
            removeBookmarkCmd.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Shift));
            launchGameCmd.InputGestures.Add(new KeyGesture(Key.F5));
            focusAssetFilterCmd.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));

            CommandBindings.Add(new CommandBinding(newCmd, newModMenuItem_Click));
            CommandBindings.Add(new CommandBinding(openCmd, openModMenuItem_Click));
            CommandBindings.Add(new CommandBinding(saveCmd, saveModMenuItem_Click));
            CommandBindings.Add(new CommandBinding(saveAsCmd, saveAsModMenuItem_Click));
            CommandBindings.Add(new CommandBinding(addBookmarkCmd, BookmarkAddButton_Click));
            CommandBindings.Add(new CommandBinding(removeBookmarkCmd, BookmarkRemoveButton_Click));
            CommandBindings.Add(new CommandBinding(focusAssetFilterCmd, (s, e) => dataExplorer.FocusFilter()));

            if (ProfilesLibrary.EnableExecution)
            {
                CommandBindings.Add(new CommandBinding(launchGameCmd, launchButton_Click));
                launchButton.IsEnabled = true;
            }

            InitGameSpecificMenus();

            LoadMenuExtensions();
            LoadTabExtensions();
            LoadDataExplorerMenuItemExtensions();

            LoadedPluginsList.ItemsSource = App.PluginManager.LoadedPlugins;

            if (toolsMenuItem.Items.Count != 0)
                toolsMenuItem.Items.Add(new Separator());

            MenuItem optionsMenuItem = new MenuItem()
            {
                Header = "Options",
                Icon = new Image() { Source = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Settings.png") as ImageSource },
            };
            optionsMenuItem.Click += optionsMenuItem_Click;
            toolsMenuItem.Items.Add(optionsMenuItem);

            Bookmarks.BookmarkDb.ContextChanged += BookmarkDb_ContextChanged;
            BookmarkContextPicker.ItemsSource = Bookmarks.BookmarkDb.Contexts.Values;
            if (Bookmarks.BookmarkDb.CurrentContext != null)
            {
                Bookmarks.BookmarkDb.CurrentContext.TargetAvailable += CurrentBookmarkContext_TargetAvailable;
                Bookmarks.BookmarkDb.CurrentContext.TargetUnavailable += CurrentBookmarkContext_TargetUnavailable;
            }
            BookmarkContextPicker.SelectedItem = Bookmarks.BookmarkDb.CurrentContext;
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();

            m_clearRecentsMenuItem.Click += delegate (object sender, RoutedEventArgs e)
            {
                m_recentProjects.Clear();
                recentProjectsMenuItem.Items.Clear();
                recentProjectsMenuItem.IsEnabled = false;

                Config.Add("RecentProjects", m_recentProjects, ConfigScope.Game);
                Config.Save();
            };

            RefreshRecentProjects();

            m_currentExplorer = dataExplorer;
        }

        private void LoadMenuExtensions()
        {
            // Add menu extensions to Editor
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
                    menu.Items.Add(foundMenuItem);
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

        private void LoadDataExplorerMenuItemExtensions()
        {
            foreach (var contextItemExtension in App.PluginManager.DataExplorerContextMenuExtensions)
            {
                MenuItem contextMenuItem = new MenuItem
                {
                    Header = contextItemExtension.ContextItemName,
                    Icon = new Image() { Source = contextItemExtension.Icon, Opacity = 0.5 },
                    Command = contextItemExtension.ContextItemClicked
                };
                dataExplorer.AssetContextMenu.Items.Add(contextMenuItem);
            }
        }

        private void LoadTabExtensions()
        {
            foreach (var tabExtension in App.PluginManager.TabExtensions)
            {
                FrostyTabItem tabExtItem = new FrostyTabItem
                {
                    Header = tabExtension.TabItemName,
                    Content = tabExtension.TabContent,
                    Icon = tabExtension.TabItemIcon,
                    CloseButtonVisible = false
                };
                miscTabControl.Items.Add(tabExtItem);
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDiscordState();

            FrostyTabItem ti = tabControl.SelectedItem as FrostyTabItem;

            if (!(ti?.Content is FrostyAssetEditor editor))
                return;

            editorToolbarItems.ItemsSource = editor.RegisterToolbarItems();
        }

        private void UpdateDiscordState()
        {
            string state = "";

            if (tabControl.SelectedItem is FrostyTabItem ti)
            {
                string header = ti.Header as string;
                string tabId = ti.TabId;

                if (tabId != null)
                {
                    AssetEntry ebx = App.AssetManager.GetEbxEntry(tabId) ?? App.AssetManager.GetCustomAssetEntry("legacy", tabId);

                    state = "Viewing " + ebx.Filename;
                    if (ebx.IsDirty && ProfilesLibrary.EnableExecution)
                        state = "Editing " + ebx.Filename;
                }
                else
                    state = header;
            }

            App.UpdateDiscordRPC(state);
        }

        private void InitGameSpecificMenus()
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville)
                InitFifaMenu();
        }

        private void InitFifaMenu()
        {
            legacyExplorerTabItem.Visibility = Visibility.Visible;
        }

        private void explorerTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (explorerTabControl.SelectedItem == dataExplorerTabItem)
                m_currentExplorer = dataExplorer;
            else
                m_currentExplorer = legacyExplorer;
        }

        private void UpdateWindowTitle()
        {
            Title = "Frosty Editor - " + App.Version + " (" + ProfilesLibrary.DisplayName + ") ";

            if (ProfilesLibrary.EnableExecution)
                Title += "[" + m_project.DisplayName + "]";
            else
                Title += "[Read Only]";
        }

        private void FrostyWindow_Loaded(object sender, EventArgs e)
        {
            (App.Logger as FrostyLogger).AddBinding(tb, TextBox.TextProperty);
            dataExplorer.ItemsSource = App.AssetManager.EnumerateEbx();
            legacyExplorer.ItemsSource = App.AssetManager.EnumerateCustomAssets("legacy");

            DirectoryInfo di = new DirectoryInfo("Mods/" + ProfilesLibrary.ProfileName);
            if (!di.Exists)
                Directory.CreateDirectory(di.FullName);

            // kick off autosave timer
            bool enabled = Config.Get<bool>("AutosaveEnabled", true);
            //bool enabled = Config.Get<bool>("Autosave", "Enabled", true);
            if (enabled)
            {
                //int timerInterval = Config.Get<int>("Autosave", "Period", 5) * 60 * 1000;
                int timerInterval = Config.Get<int>("AutosavePeriod", 5) * 60 * 1000;
                if (timerInterval > 0)
                {
                    m_autoSaveTimer = new System.Timers.Timer {Interval = timerInterval};
                    m_autoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
                    m_autoSaveTimer.AutoReset = false;
                    m_autoSaveTimer.Start();
                }
            }
        }

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb.IsFocused)
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            tb.ScrollToEnd();
        }

        int lastSaveIndex = 0;
        private void AutoSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_project.IsDirty)
            {
                ++lastSaveIndex;
                if (lastSaveIndex > Config.Get<int>("AutosaveMaxCount", 10)) // Config.Get<int>("Autosave", "MaxCount", 10)
                    lastSaveIndex = 1;

                string projectName = m_project.DisplayName.Remove(m_project.DisplayName.Length - 10);
                FileInfo fi = new FileInfo("Autosave/" + projectName + "_" + lastSaveIndex.ToString("D3") + ".fbproject");

                App.Logger.Log("Initiated autosave of project to " + fi.FullName);

                // save project but dont update dirty state
                m_project.Save(overrideFilename: fi.FullName, updateDirtyState: false);
            }

            // begin timer again
            m_autoSaveTimer?.Start();
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private static ImageSource PropertiesImage = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Properties.png") as ImageSource;

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ProfilesLibrary.EnableExecution)
                return;

            // setup ability to cancel the process
            CancellationTokenSource cancelToken = new CancellationTokenSource();

            Random r = new Random();
            string editorModName = $"EditorMod{r.Next(1000, 9999).ToString("D4")}.fbmod";
            launchButton.IsEnabled = false;

            // get all mods
            List<string> modPaths = new List<string>();

            DirectoryInfo modDirectory = new DirectoryInfo($"Mods/{ProfilesLibrary.ProfileName}");
            foreach (string modPath in Directory.EnumerateFiles($"Mods/{ProfilesLibrary.ProfileName}/", "*.fbmod", SearchOption.AllDirectories))
                modPaths.Add(Path.GetFileName(modPath));

            // create temporary editor mod
            ModSettings editorSettings = new ModSettings { Title = "Editor Mod", Author = "Frosty Editor", Version = "1", Category = "Editor" };

            // apply mod
            string additionalArgs = "";
            FrostyModExecutor executor = new FrostyModExecutor();

            // Set pack
            App.SelectedPack = "Editor";

            try
            {
                // run mod applying process
                FrostyTaskWindow.Show("Launching", "", (task) => 
                {
                    try
                    {
                        foreach (var executionAction in App.PluginManager.ExecutionActions)
                            executionAction.PreLaunchAction(task.TaskLogger, PluginManagerType.Editor, cancelToken.Token);

                        task.Update("Exporting Mod");
                        ExportMod(editorSettings, $"Mods/{ProfilesLibrary.ProfileName}/{editorModName}", true);
                        modPaths.Add(editorModName);

                        // allow cancelling in case of a big mod (will cancel after processing the mod)
                        // @todo: add cancellation to different stages of mod exportation, to allow cancelling
                        //        at any stage of a large mod

                        cancelToken.Token.ThrowIfCancellationRequested();

                        task.Update("");
                        executor.Run(App.FileSystem, cancelToken.Token, task.TaskLogger, $"Mods/{ProfilesLibrary.ProfileName}/", App.SelectedPack, additionalArgs, modPaths.ToArray());

                        foreach (var executionAction in App.PluginManager.ExecutionActions)
                            executionAction.PostLaunchAction(task.TaskLogger, PluginManagerType.Editor, cancelToken.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // swollow

                        foreach (var executionAction in App.PluginManager.ExecutionActions)
                            executionAction.PostLaunchAction(task.TaskLogger, PluginManagerType.ModManager, cancelToken.Token);
                    }

                }, showCancelButton: true, cancelCallback: (task) => cancelToken.Cancel());
            }
            catch (OperationCanceledException)
            {
                // process was cancelled
                App.Logger.Log("Launch Cancelled");
            }

            // remove editor mod
            FileInfo editorMod = new FileInfo($"Mods/{ProfilesLibrary.ProfileName}/{editorModName}");
            if (editorMod.Exists)
                editorMod.Delete();

            launchButton.IsEnabled = true;

            GC.Collect();
        }

        private void unimplementedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrostyMessageBox.Show("This feature is currently unimplemented", "Frosty Editor");
        }

        public void ExportMod(ModSettings modSettings, string filename, bool bSilent)
        {
            m_project.WriteToMod(filename, modSettings);
            if (!bSilent)
                App.Logger.Log("Mod saved to {0}", filename);
        }

        private void newModMenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_autoSaveTimer?.Stop();

            if (AskIfShouldSaveProject() == MessageBoxResult.Cancel)
            {
                return;
            }

            // close all open tabs
            RemoveAllTabs();

            // clear all modifications
            App.AssetManager.Reset();
            dataExplorer.RefreshAll();
            legacyExplorer.RefreshAll();

            // create a new blank project
            m_project = new FrostyProject();

            App.Logger.Log("New project started");

            UpdateWindowTitle();
            UpdateDiscordState();

            // reset show only modified flag
            dataExplorer.ShowOnlyModified = false;
            legacyExplorer.ShowOnlyModified = false;

            m_autoSaveTimer?.Start();
        }

        private void openModMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool savePrevProject = false;
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Open Project", "*.fbproject (Frosty Project)|*.fbproject", "Project");

            if (!ofd.ShowDialog())
            {
                return;
            }

            if (AskIfShouldSaveProject(true) == MessageBoxResult.Cancel)
            {
                return;
            }

            LoadProject(ofd.FileName, savePrevProject);
        }

        private void saveModMenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_autoSaveTimer?.Stop();
            if (SaveProject(false))
            {
                FrostyTaskWindow.Show("Saving Project", m_project.Filename, (task) => m_project.Save());

                AddRecentProject(m_project.Filename);

                dataExplorer.RefreshItems();
                legacyExplorer.RefreshItems();
                RefreshTabs();

                App.Logger.Log("Project saved to {0}", m_project.Filename);

                UpdateWindowTitle();
                UpdateDiscordState();
            }
            m_autoSaveTimer?.Start();
        }

        private void saveAsModMenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_autoSaveTimer?.Stop();
            if (SaveProject(true))
            {
                FrostyTaskWindow.Show("Saving Project", m_project.Filename, (task) => m_project.Save());

                AddRecentProject(m_project.Filename);

                dataExplorer.RefreshItems();
                legacyExplorer.RefreshItems();
                RefreshTabs();

                App.Logger.Log("Project saved to {0}", m_project.Filename);

                UpdateWindowTitle();
                UpdateDiscordState();
            }
            m_autoSaveTimer?.Start();
        }

        private void LoadProject(string filename, bool saveProject)
        {
            m_autoSaveTimer?.Stop();

            // close all open tabs
            RemoveAllTabs();

            FrostyProject newProject = null;
            FrostyTaskWindow.Show("Loading Project", "", (task) =>
            {
                if (saveProject)
                {
                    m_project.Save();
                    App.Logger.Log("Project saved to {0}", m_project.Filename);
                }

                task.Update(filename);

                // clear all modifications
                App.AssetManager.Reset();

                // load project
                newProject = new FrostyProject();
                if (!newProject.Load(filename))
                {
                    // failed to load for whatever reason
                    App.Logger.LogWarning("Failed to load {0}", filename);
                    newProject = null;
                }
            });

            if (newProject != null)
            {
                m_project = newProject;

                dataExplorer.ShowOnlyModified = false;
                dataExplorer.ShowOnlyModified = true;
                dataExplorer.RefreshItems();

                legacyExplorer.ShowOnlyModified = false;
                legacyExplorer.ShowOnlyModified = true;
                legacyExplorer.RefreshItems();

                // report success
                App.Logger.Log("Loaded {0}", m_project.Filename);

                UpdateWindowTitle();
                UpdateDiscordState();

                AddRecentProject(m_project.Filename);
            }

            m_autoSaveTimer?.Start();
        }

        private bool SaveProject(bool forceSaveAs)
        {
            if (m_project.Filename == "" || forceSaveAs)
            {
                FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Project As", "*.fbproject (Frosty Project)|*.fbproject", "Project");
                if (!sfd.ShowDialog())
                    return false;

                m_project.Filename = sfd.FileName;
            }

            return true;
        }

        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        public void OpenEditor(string title, FrostyBaseEditor editor)
        {
            FrostyTabItem ti = new FrostyTabItem();

            RoutedCommand closeCmd = new RoutedCommand();
            closeCmd.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));

            ti.Header = title;
            ti.Content = editor;
            ti.Icon = editor.Icon;
            ti.CloseButtonVisible = true;
            ti.CloseButtonClick += (o, e2) => { RemoveTab(ti); };
            ti.MiddleMouseButtonClick += (s, o) => { RemoveTab(ti); };
            ti.IsSelected = true;
            editor.CommandBindings.Add(new CommandBinding(closeCmd, (o, e) => { RemoveTab(ti); }));

            AddTab(ti);
        }

        public void OpenAsset(AssetEntry asset, bool createDefaultEditor = true)
        {
            if (asset == null)
                return;

            if (asset.Type == "EncryptedAsset")
                return;

            foreach (FrostyTabItem currentTi in tabControl.Items)
            {
                if (currentTi.TabId == asset.Name)
                {
                    currentTi.IsSelected = true;
                    return;
                }
            }

            FrostyTabItem ti = new FrostyTabItem();
            FrostyAssetEditor editor = null;

            var definition = App.PluginManager.GetAssetDefinition(asset.Type);
            if (definition != null)
            {
                editor = definition.GetEditor(App.Logger);
            }
            
            if (editor == null)
            {
                if (!createDefaultEditor)
                    return;

                editor = new FrostyAssetEditor(App.Logger);
            }

            try
            {
                //FrostyTask.Begin("Opening Asset");
                editor.SetAsset(asset);
                //FrostyTask.End();
            }
            catch (Exception)
            {
                FrostyMessageBox.Show("Unable to open asset", "Frosty Editor");
                //FrostyTask.End();
                return;
            }

            editor.OnAssetModified += (s, o) =>
            {
                // update UI
                ti.Header = asset.DisplayName;
                ti.Icon = new AssetEntryToBitmapSourceConverter().Convert(asset, typeof(ImageSource), null, null) as ImageSource;
                dataExplorer.RefreshItems();
                legacyExplorer.RefreshItems();

                UpdateDiscordState();
            };

            RoutedCommand closeCmd = new RoutedCommand();
            closeCmd.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));

            ti.Icon = (new AssetEntryToBitmapSourceConverter().Convert(asset, typeof(ImageSource), null, null) as ImageSource);
            ti.Content = editor;
            ti.Header = asset.DisplayName;
            ti.TabId = asset.Name;
            ti.IsSelected = true;
            ti.CloseButtonVisible = true;
            ti.CloseButtonClick += (s, o) =>
            {
                ShutdownEditorAndRemoveTab(editor, ti);
            };
            ti.MiddleMouseButtonClick += (s, o) =>
            {
                ShutdownEditorAndRemoveTab(editor, ti);
            };
            editor.CommandBindings.Add(new CommandBinding(closeCmd, (o, e) => { ShutdownEditorAndRemoveTab(editor, ti); }));

            AddTab(ti);
        }

        public void AddTab(FrostyTabItem ti)
        {
            tabControl.Items.Add(ti);
            FrostyTabItem welcomeTabItem = tabControl.Items[0] as FrostyTabItem;
            welcomeTabItem.Visibility = Visibility.Collapsed;
        }

        public void ShutdownEditorAndRemoveTab(FrostyAssetEditor editor, FrostyTabItem ti)
        {
            editor.Closed();
            if (ti.IsSelected)
                editorToolbarItems.ItemsSource = null;
            tabControl.Items.Remove(ti);
            if (tabControl.Items.Count == 1)
            {
                FrostyTabItem item = tabControl.Items[0] as FrostyTabItem;

                item.Visibility = Visibility.Visible;
                item.IsSelected = true;
            }
        }

        public void RemoveTab(FrostyTabItem ti)
        {
            FrostyBaseEditor editor = ti.Content as FrostyBaseEditor;
            editor?.Closed();

            tabControl.Items.Remove(ti);
            if (tabControl.Items.Count == 1)
            {
                FrostyTabItem item = tabControl.Items[0] as FrostyTabItem;

                item.Visibility = Visibility.Visible;
                item.IsSelected = true;
            }
        }

        public void RemoveAllTabs()
        {
            while (tabControl.Items.Count > 1)
            {
                FrostyTabItem tabItem = tabControl.Items[1] as FrostyTabItem;
                if (tabItem.Content is FrostyAssetEditor editor)
                    ShutdownEditorAndRemoveTab(editor, tabItem);
                else
                    RemoveTab(tabItem);
            }
        }

        public void RefreshTabs()
        {
            foreach (FrostyTabItem item in tabControl.Items)
            {
                if (item.Content is FrostyAssetEditor editor)
                {
                    if (editor.AssetEntry != null)
                        item.Header = editor.AssetEntry.DisplayName;
                }
            }
        }

        private void TabItem_MouseMove(object sender, MouseEventArgs e) {
            if (!(e.Source is FrostyTabItem tabItem)) {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        private void TabItem_Move(object sender, DragEventArgs e) {
            if (e.Source is FrostyTabItem tabItemTarget &&
                e.Data.GetData(typeof(FrostyTabItem)) is FrostyTabItem tabItemSource &&
                !tabItemTarget.Equals(tabItemSource) &&
                tabItemTarget.Parent is FrostyTabControl tabControl) {
                int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                tabControl.Items.Remove(tabItemSource);
                tabControl.Items.Insert(targetIndex, tabItemSource);
                tabItemSource.IsSelected = true;
            }
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow();
            win.ShowDialog();
        }

        private void FrostyWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = AskIfShouldSaveProject(true) == MessageBoxResult.Cancel;

            Bookmarks.BookmarkDb.SaveDb();
        }

        private void dataExplorer_SelectedAssetDoubleClick(object sender, RoutedEventArgs e)
        {
            AssetEntry entry = m_currentExplorer.SelectedAsset;
            OpenAsset(entry, m_currentExplorer == dataExplorer);
        }

        private void dataExplorer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = dataExplorer.SelectedAsset as EbxAssetEntry;
            App.SelectedAsset = entry;
        }

        #region -- Context Menu Callbacks --

        // context menu callbacks

        private void contextMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (m_currentExplorer.SelectedAsset == null)
                return;
            OpenAsset(m_currentExplorer.SelectedAsset, m_currentExplorer == dataExplorer);
        }

        private void contextMenuRevert_Click(object sender, RoutedEventArgs e)
        {
            AssetEntry entry = m_currentExplorer.SelectedAsset;
            if (!entry.IsModified)
                return;

            for (int i = 1; i < tabControl.Items.Count; i++)
            {
                FrostyTabItem tabItem = tabControl.Items[i] as FrostyTabItem;
                if (tabItem.TabId == entry.Name)
                {
                    RemoveTab(tabItem);
                    break;
                }
            }

            FrostyTaskWindow.Show("Reverting Asset", "", (task) => { App.AssetManager.RevertAsset(entry, suppressOnModify: false); });

            dataExplorer.RefreshAll();
            legacyExplorer.RefreshAll();
        }

        private void contextMenuImportAsset_Click(object sender, RoutedEventArgs e)
        {
            LegacyFileEntry selectedAsset = legacyExplorer.SelectedAsset as LegacyFileEntry;
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Open Legacy file", "*." + selectedAsset.Type + " (Legacy Files)|*." + selectedAsset.Type, "FifaLegacy");

            if (ofd.ShowDialog())
            {
                FrostyTaskWindow.Show("Importing Legacy Asset", "", (task) =>
                {
                    byte[] buffer = null;
                    using (NativeReader reader = new NativeReader(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read)))
                        buffer = reader.ReadToEnd();

                    App.AssetManager.ModifyCustomAsset("legacy", selectedAsset.Name, buffer);
                });

                App.Logger.Log("{0} imported to {1}", ofd.FileName, selectedAsset.Name);
                legacyExplorer.RefreshItems();
            }
        }

        private void contextMenuExportAsset_Click(object sender, RoutedEventArgs e)
        {
            if (legacyExplorer.SelectedAssets.Count == 0)
                return;

            LegacyFileEntry selectedAsset = legacyExplorer.SelectedAssets[0] as LegacyFileEntry;

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Save Legacy File",
                Filter = "*." + selectedAsset.Type + " (Legacy Files)|*." + selectedAsset.Type,
                FileName = selectedAsset.Filename
            };

            if (sfd.ShowDialog() == true)
            {
                IList<AssetEntry> assets = legacyExplorer.SelectedAssets;
                FrostyTaskWindow.Show("Exporting Legacy Assets", "", (task) =>
                {
                    App.AssetManager.SendManagerCommand("legacy", "SetCacheModeEnabled", true);
                    FileInfo fi = new FileInfo(sfd.FileName);

                    int progress = 0;
                    foreach (LegacyFileEntry asset in assets)
                    {
                        task.Update(asset.Name, (progress / (double)assets.Count) * 100.0);
                        progress++;

                        string outFileName = fi.Directory.FullName + "\\" + asset.Filename + "." + asset.Type;
                        using (NativeWriter writer = new NativeWriter(new FileStream(outFileName, FileMode.Create)))
                            writer.Write(new NativeReader(App.AssetManager.GetCustomAsset("legacy", asset)).ReadToEnd());
                    }

                    App.Logger.Log("Legacy files saved to {0}", fi.Directory.FullName);
                    App.AssetManager.SendManagerCommand("legacy", "FlushCache");
                });
            }
        }

        private void contextMenuImportEbx_Click(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = dataExplorer.SelectedAsset as EbxAssetEntry;

            AssetDefinition assetDefinition = App.PluginManager.GetAssetDefinition(entry.Type) ?? new AssetDefinition();

            List<AssetImportType> filters = new List<AssetImportType>();
            assetDefinition.GetSupportedImportTypes(filters);

            string filterString = "";
            foreach (AssetImportType filter in filters)
                filterString += "|" + filter.FilterString;
            filterString = filterString.Trim('|');

            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import Asset", filterString, assetDefinition.GetType().Name);
            if (ofd.ShowDialog())
            {
                if (assetDefinition.Import(entry, ofd.FileName, filters[ofd.FilterIndex - 1].Extension))
                {
                    dataExplorer.RefreshItems();
                    App.Logger.Log("Imported {0} to {1}", entry.Name, ofd.FileName);
                }
            }
        }

        private EbxAssetEntry DuplicateAsset(EbxAssetEntry entry, string newName, bool createNew, Type newType)
        {
            EbxAsset asset = App.AssetManager.GetEbx(entry);
            EbxAsset newAsset = null;

            if (createNew)
            {
                newAsset = new EbxAsset(TypeLibrary.CreateObject(newType.Name));
            }
            else
            {
                using (EbxBaseWriter writer = EbxBaseWriter.CreateWriter(new MemoryStream(), EbxWriteFlags.DoNotSort | EbxWriteFlags.IncludeTransient))
                {
                    writer.WriteAsset(asset);
                    byte[] buf = writer.ToByteArray();
                    using (EbxReader reader = EbxReader.CreateReader(new MemoryStream(buf)))
                        newAsset = reader.ReadAsset<EbxAsset>();
                }
            }

            newAsset.SetFileGuid(Guid.NewGuid());

            dynamic obj = newAsset.RootObject;
            obj.Name = newName;

            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(asset.Objects, (Type)obj.GetType(), asset.FileGuid), -1);
            obj.SetInstanceGuid(guid);

            EbxAssetEntry newEntry = App.AssetManager.AddEbx(newName, newAsset);

            newEntry.AddedBundles.AddRange(entry.EnumerateBundles());
            newEntry.ModifiedEntry.DependentAssets.AddRange(newAsset.Dependencies);

            return newEntry;
        }

        private void contextMenuExportEbx_Click(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = dataExplorer.SelectedAsset as EbxAssetEntry;

            AssetDefinition assetDefinition = App.PluginManager.GetAssetDefinition(entry.Type) ?? new AssetDefinition();

            List<AssetExportType> filters = new List<AssetExportType>();
            assetDefinition.GetSupportedExportTypes(filters);

            string filterString = "";
            foreach (AssetExportType filter in filters)
                filterString += "|" + filter.FilterString;
            filterString = filterString.Trim('|');

            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Export Asset", filterString, assetDefinition.GetType().Name, entry.Filename);
            if (sfd.ShowDialog())
            {
                if (assetDefinition.Export(entry, sfd.FileName, filters[sfd.FilterIndex - 1].Extension))
                {
                    App.Logger.Log("Exported {0} to {1}", entry.Name, sfd.FileName);
                }
            }
        }

        private void contextMenuBookmarkItemOpen_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkTreeView.SelectedItem == null)
                return;
            Bookmarks.BookmarkItem target = BookmarkTreeView.SelectedItem as Bookmarks.BookmarkItem;
            target.Target.NavigateTo(true);
        }

        private void contextMenuBookmarkItemFind_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkTreeView.SelectedItem == null)
                return;
            Bookmarks.BookmarkItem target = BookmarkTreeView.SelectedItem as Bookmarks.BookmarkItem;
            target.Target.NavigateTo(false);
        }

#endregion

        #region -- Bookmarks --

        private void BookmarkTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBookmarkFilters();
        }

        private void BookmarkDb_ContextChanged(object sender, EventArgs e)
        {
            if (Bookmarks.BookmarkDb.CurrentContext != null)
            {
                Bookmarks.BookmarkDb.CurrentContext.TargetAvailable -= CurrentBookmarkContext_TargetAvailable;
                Bookmarks.BookmarkDb.CurrentContext.TargetUnavailable -= CurrentBookmarkContext_TargetUnavailable;
            }
            Bookmarks.BookmarkContext newcxt = sender as Bookmarks.BookmarkContext;
            if (newcxt != null)
            {
                newcxt.TargetAvailable += CurrentBookmarkContext_TargetAvailable;
                newcxt.TargetUnavailable += CurrentBookmarkContext_TargetUnavailable;
            }
            BookmarkContextPicker.SelectedItem = newcxt;
        }

        private void CurrentBookmarkContext_TargetAvailable(object sender, EventArgs e)
        {
            BookmarkAddButton.IsEnabled = true;
        }

        private void CurrentBookmarkContext_TargetUnavailable(object sender, EventArgs e)
        {
            BookmarkAddButton.IsEnabled = false;
        }

        private void BookmarkContextPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Bookmarks.BookmarkDb.CurrentContext = e.AddedItems[0] as Bookmarks.BookmarkContext;
        }

        private void AddSelectedBookmark()
        {
            Bookmarks.BookmarkItem newBookmark = null;
            if (Bookmarks.BookmarkDb.CurrentContext?.AvailableTarget != null)
            {
                Bookmarks.BookmarkTarget target = Bookmarks.BookmarkDb.CurrentContext.AvailableTarget;

                if (BookmarkTreeView.SelectedItem != null)
                {
                    Bookmarks.BookmarkItem parent = BookmarkTreeView.SelectedItem as Bookmarks.BookmarkItem;
                    // Abort if the selected parent is not a folder
                    if (parent.Target as Bookmarks.FolderBookmarkTarget == null)
                    {
                        // Add as a sibling of the selected item
                        parent = parent.Parent;
                        if (parent == null)
                        {
                            newBookmark = new Bookmarks.BookmarkItem(target, null);
                            Bookmarks.BookmarkDb.CurrentContext?.Bookmarks.Add(newBookmark);
                        }
                        else
                        {
                            newBookmark = new Bookmarks.BookmarkItem(target, parent) {Parent = parent};
                            parent.Children.Add(newBookmark);
                            parent.IsExpanded = true;
                        }
                    }
                    else
                    {
                        newBookmark = new Bookmarks.BookmarkItem(target, parent) {Parent = parent};
                        parent.Children.Add(newBookmark);
                        parent.IsExpanded = true;
                    }
                }
                else
                {
                    newBookmark = new Bookmarks.BookmarkItem(target, null);
                    Bookmarks.BookmarkDb.CurrentContext?.Bookmarks.Add(newBookmark);
                }
                TriggerPulseAnimation();
            }
            UpdateBookmarkFilters();
            Bookmarks.BookmarkDb.SaveDb();
            //Config.Save(App.configFilename);
        }

        private void RemoveSelectedBookmark()
        {
            if (BookmarkTreeView.SelectedItem != null)
            {
                Bookmarks.BookmarkItem selected = (Bookmarks.BookmarkItem)BookmarkTreeView.SelectedItem;
                if (selected.Parent != null)
                {
                    selected.Parent.Children.Remove(selected);
                }
                else
                {
                    Bookmarks.BookmarkDb.CurrentContext.Bookmarks.Remove(selected);
                }
                UpdateBookmarkFilters();
                Bookmarks.BookmarkDb.SaveDb();
                //Config.Save(App.configFilename);
            }
        }

        private void AddBookmarkFolder()
        {
            Bookmarks.BookmarkItem parent = BookmarkTreeView.SelectedItem as Bookmarks.BookmarkItem;
            Bookmarks.BookmarkItem newItem = new Bookmarks.BookmarkItem(new Bookmarks.FolderBookmarkTarget(), null) {Name = "New Folder"};

            if (parent != null)
            {
                if (parent.Target is Bookmarks.FolderBookmarkTarget)
                {
                    parent.Children.Add(newItem);
                    newItem.Parent = parent;
                }
                else
                {
                    if (parent.Parent != null)
                    {
                        parent.Parent.Children.Add(newItem);
                        newItem.Parent = parent.Parent;
                    }
                    else
                        Bookmarks.BookmarkDb.CurrentContext.Bookmarks.Add(newItem);
                }
            }
            else
                Bookmarks.BookmarkDb.CurrentContext.Bookmarks.Add(newItem);

            TriggerPulseAnimation();
            UpdateBookmarkFilters();
            Bookmarks.BookmarkDb.SaveDb();
            //Config.Save(App.configFilename);
        }

        private void BookmarkAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedBookmark();
        }

        private void BookmarkRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedBookmark();
        }

        private void BookmarkAddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookmarkFolder();
        }

        private void BookmarkEditLabelButton_Click(object sender, RoutedEventArgs e)
        {
            Stack<Bookmarks.BookmarkItem> stack = new Stack<Bookmarks.BookmarkItem>();
            Bookmarks.BookmarkItem item = BookmarkTreeView.SelectedItem as Bookmarks.BookmarkItem;
            Bookmarks.BookmarkItem alsoitem = item;
            if (item == null)
                return;

            stack.Push(item);
            // Walk up the tree
            while (item.Parent != null)
            {
                stack.Push(item.Parent);
                item = item.Parent;
            }


            ItemContainerGenerator gen = BookmarkTreeView.ItemContainerGenerator;
            while (stack.Count > 1)
            {
                gen = (gen.ContainerFromItem(stack.Pop()) as TreeViewItem).ItemContainerGenerator;
            }

            FrostyEditableLabel label = FindVisualChild<FrostyEditableLabel>(gen.ContainerFromItem(stack.Pop()));
            label.Text = (label.Text == null) ? "" : label.Text;
            label.BeginEdit();
        }   

        private void BookmarkTreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            item.IsSelected = true;
        }

        private void BookmarkTreeView_MouseDoubleClick()
        {
            if (BookmarkTreeView.SelectedItem != null)
            {
                Bookmarks.BookmarkItem target = BookmarkTreeView.SelectedItem as Bookmarks.BookmarkItem;
                target.Target.NavigateTo(true);
            }
        }

        private void BookmarkTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BookmarkRemoveButton.IsEnabled = BookmarkEditLabelButton.IsEnabled = BookmarkTreeView.SelectedItem != null;
        }

        private bool BookmarkFilter(Bookmarks.BookmarkItem target)
        {
            return string.IsNullOrEmpty(BookmarkFilterTextBox.Text) || target.Text.ToLower().Contains(BookmarkFilterTextBox.Text.ToLower()) || (target.Name?.ToLower().Contains(BookmarkFilterTextBox.Text.ToLower()) ?? false);
        }

        private void BookmarkFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBookmarkFilters();
        }

        private void ApplyBookmarkFilter(Bookmarks.BookmarkItem item)
        {
            bool self = false; // Track whether any children are visible
            foreach (Bookmarks.BookmarkItem child in item.Children)
            {
                ApplyBookmarkFilter(child);
                self = self || child.IsVisible;
            }

            bool filter = BookmarkFilter(item);
            item.IsVisible = self || filter;
            item.IsFiltered = !filter;
        }

        private int CountBookmarks(Bookmarks.BookmarkItem item, bool visibleOnly)
        {
            if (visibleOnly && !item.IsVisible)
                return 0;

            int result = 0;
            foreach (Bookmarks.BookmarkItem child in item.Children)
                result += CountBookmarks(child, visibleOnly);

            return result + (visibleOnly ? BookmarkFilter(item) ? 1 : 0 : 1);
        }

        private bool refreshing = false; // For some reason, BookmarkTreeView.Items.Refresh causes recursion here...
        private void UpdateBookmarkFilters()
        {
            if (refreshing)
                return;
            int visible = 0;
            int total = 0;
            if (Bookmarks.BookmarkDb.CurrentContext != null)
            {
                foreach (Bookmarks.BookmarkItem item in Bookmarks.BookmarkDb.CurrentContext.Bookmarks)
                {
                    ApplyBookmarkFilter(item);
                    total += CountBookmarks(item, false);
                    visible += CountBookmarks(item, true);
                }
            }
            refreshing = true;
            BookmarkTreeView.Items.Refresh();
            refreshing = false;

            if (string.IsNullOrEmpty(BookmarkFilterTextBox.Text))
            {
                BookmarkFilterInfo.Content = "(no filter)";
            }
            else
            {
                BookmarkFilterInfo.Content = "(" + visible + " / " + total + ")";
            }
        }

        private void FrostyEditableLabel_EditEnded(object sender, EventArgs e)
        {
            UpdateBookmarkFilters();
            Bookmarks.BookmarkDb.SaveDb();
            //Config.Save(App.configFilename);
            Focus();
        }

        private static SolidColorBrush NormalBrush = new SolidColorBrush(new Color { R = 140, G = 140, B = 140, A = 255 });
        private void TriggerPulseAnimation()
        {
            BookmarkTabItem.Background = NormalBrush;
            DoubleAnimation lastFadeAnimation = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(1.0))) {FillBehavior = FillBehavior.HoldEnd};
            NormalBrush.BeginAnimation(LinearGradientBrush.OpacityProperty, lastFadeAnimation);
        }

        #endregion

        private void listBundlesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = dataExplorer.SelectedAsset as EbxAssetEntry;

            App.Logger.Log("");
            foreach (int bundleId in entry.EnumerateBundles())
                App.Logger.Log(App.AssetManager.GetBundleEntry(bundleId).DisplayName);
        }

        private void optionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow win = new OptionsWindow();
            win.ShowDialog();
        }

        private void MiscTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ReferenceExplorerList_SelectedAssetDoubleClick(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = (sender as FrostyAssetListView).SelectedItem as EbxAssetEntry;
            if (entry == null)
                return;

            OpenAsset(entry);
        }

        private static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T dObj)
                    {
                        return dObj;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void closeAllDocumentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveAllTabs();
        }

        /// <summary>
        /// Adds a specified project path to the list of recent projects.
        /// </summary>
        /// <param name="path">The *.fbproject path to be used.</param>
        public void AddRecentProject(string path)
        {
            // check if the collection of recent projects already contains the loaded project
            if (m_recentProjects.Contains(path))
            {
                // remove the project from the list temporarily, as this will allow for it to be moved to the top
                m_recentProjects.Remove(path);
            }

            m_recentProjects.Insert(0, path);

            if (m_recentProjects.Count > 10)
            {
                // remove the last project from the list of recent projects, accommodating for the new project
                m_recentProjects.Remove(m_recentProjects.Last());
            }

            Config.Add("RecentProjects", m_recentProjects, ConfigScope.Game);
            Config.Save();
            RefreshRecentProjects();
        }

        /// <summary>
        /// If the project is modified, asks the user if they would like to save it. This is ideal for scenarios in which the project will be automatically switched or unloaded.
        /// </summary>
        /// <param name="isSilent">An optional bool determining whether or not the save should be made silently.</param>
        /// <returns>A <see cref="MessageBoxResult"/> representing the user's choice.</returns>
        public MessageBoxResult AskIfShouldSaveProject(bool isSilent = false)
        {
            // check if the project is modified, which determines whether or not execution should proceed
            if (!m_project.IsDirty)
            {
                return MessageBoxResult.None;
            }

            MessageBoxResult saveQuestionResult = FrostyMessageBox.Show("Do you wish to save changes to " + m_project.DisplayName + "?", "Frosty Editor", MessageBoxButton.YesNoCancel);

            // check if the user wishes to save the project
            if (saveQuestionResult == MessageBoxResult.Yes)
            {
                // check if the project should be saved
                if (SaveProject(forceSaveAs: false))
                {
                    if (isSilent)
                    {
                        m_project.Save();
                        AddRecentProject(m_project.Filename);
                        return saveQuestionResult;
                    }

                    // begin a FrostyTask to indicate the project is being saved
                    FrostyTaskWindow.Show("Saving Project", m_project.Filename, delegate
                    {
                        m_project.Save();
                        AddRecentProject(m_project.Filename);
                    });

                    App.Logger.Log("Project saved to {0}", m_project.Filename);
                }
            }

            return saveQuestionResult;
        }

        /// <summary>
        /// Ensures the <see cref="recentProjectsMenuItem"/> and its associated list of recent projects in the config are up-to-date.
        /// </summary>
        public void RefreshRecentProjects()
        {
            recentProjectsMenuItem.Items.Clear();

            // check if there are no recent projects to display
            if (m_recentProjects.Count == 0)
            {
                recentProjectsMenuItem.IsEnabled = false;
                return;
            }

            MenuItem currentMenuItem;
            int projectIndex = 1;

            // create a new list containing the current recent projects and iterate over that, which avoids any "collection modified" exceptions
            foreach (string recentProject in new List<string>(m_recentProjects))
            {
                // check if the current project does not exist
                if (!File.Exists(recentProject))
                {
                    m_recentProjects.Remove(recentProject);

                    // save the modified list of recent projects to the config
                    Config.Add("RecentProjects", m_recentProjects, ConfigScope.Game);
                    Config.Save();

                    continue;
                }

                currentMenuItem = new MenuItem
                {
                    Header = string.Format("{0}: {1}...\\{2}", new object[]
                    {
                        projectIndex,
                        Path.GetPathRoot(recentProject),
                        Path.GetFileName(recentProject)
                    }),
                    Height = 22
                };

                currentMenuItem.Click += delegate (object sender, RoutedEventArgs e)
                {
                    // check if the recent project no longer exists
                    if (!File.Exists(recentProject))
                    {
                        FrostyMessageBox.Show("The selected project does not exist.", "Frosty Editor");

                        // refresh the displayed recent projects to accommodate for the missing project
                        RefreshRecentProjects();
                        return;
                    }

                    // check if the user does not wish to load the selected recent project
                    if (AskIfShouldSaveProject() == MessageBoxResult.Cancel)
                    {
                        return;
                    }

                    // load the recent project
                    LoadProject(recentProject, false);
                };

                recentProjectsMenuItem.Items.Add(currentMenuItem);
                projectIndex++;
            }

            // check if all recent projects did not exist by checking if the quantity of projects is zero
            if (m_recentProjects.Count == 0)
            {
                // execute RefreshRecentProjects within itself to handle the lack of projects
                RefreshRecentProjects();
                return;
            }

            recentProjectsMenuItem.Items.Add(new Separator());
            recentProjectsMenuItem.Items.Add(m_clearRecentsMenuItem);
            recentProjectsMenuItem.IsEnabled = true;
        }
    }
}
