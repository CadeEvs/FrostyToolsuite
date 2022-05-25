using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Legacy;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;

namespace Frosty.Core.Windows
{
    /// <summary>
    /// Interaction logic for FrostyProfileTaskWindow.xaml
    /// </summary>
    public partial class FrostyProfileTaskWindow : Window
    {
        public ILogger TaskLogger { get; private set; }

        private class SplashWindowLogger : ILogger
        {
            private FrostyProfileTaskWindow parent;
            public SplashWindowLogger(FrostyProfileTaskWindow inParent)
            {
                parent = inParent;
            }

            public void Log(string text, params object[] vars)
            {
                string fullText = string.Format(text, vars);
                parent.logTextBox.Dispatcher.Invoke(() =>
                {
                    if (fullText.StartsWith("progress:"))
                    {
                        fullText = fullText.Replace("progress:", "");
                        double progress = double.Parse(fullText);

                        parent.progressBar.Value = progress;
                        parent.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        parent.TaskbarItemInfo.ProgressValue = progress / 100.0d;
                    }
                    else
                        parent.logTextBox.Text = fullText;
                });
            }

            public void LogError(string text, params object[] vars)
            {
            }

            public void LogWarning(string text, params object[] vars)
            {
            }
        }

        public FrostyProfileTaskWindow(Window owner)
        {
            InitializeComponent();

            versionTextBlock.Text = App.Version;
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();

            Owner = owner;
            TaskLogger = new SplashWindowLogger(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load encryption key for profiles that require it
            if (ProfilesLibrary.RequiresKey)
            {
                byte[] keyData = null;
                if (!File.Exists(ProfilesLibrary.CacheName + ".key"))
                {
                    // prompt for encryption key
                    KeyPromptWindow keyPromptWin = new KeyPromptWindow();
                    if (keyPromptWin.ShowDialog() == false)
                    {
                        FrostyMessageBox.Show("Encryption key not entered. Unable to load profile.", "Frosty Editor");
                        Close();
                        return;
                    }

                    keyData = keyPromptWin.EncryptionKey;
                    using (NativeWriter writer = new NativeWriter(new FileStream(ProfilesLibrary.CacheName + ".key", FileMode.Create)))
                        writer.Write(keyData);
                }
                else
                {
                    // otherwise just read the key from file
                    keyData = NativeReader.ReadInStream(new FileStream(ProfilesLibrary.CacheName + ".key", FileMode.Open, FileAccess.Read));
                }

                // add primary encryption key
                byte[] key = new byte[0x10];
                Array.Copy(keyData, key, 0x10);
                KeyManager.Instance.AddKey("Key1", key);

                if (keyData.Length > 0x10)
                {
                    // add additional encryption keys
                    key = new byte[0x10];
                    Array.Copy(keyData, 0x10, key, 0, 0x10);
                    KeyManager.Instance.AddKey("Key2", key);

                    key = new byte[0x4000];
                    Array.Copy(keyData, 0x20, key, 0, 0x4000);
                    KeyManager.Instance.AddKey("Key3", key);
                }
            }

            Config.Save();

            App.Logger.Log("Loading profile for " + ProfilesLibrary.DisplayName);

            profileTextBlock.Text = ProfilesLibrary.DisplayName;
            bannerImage.Source = LoadBanner(ProfilesLibrary.Banner);

            DirectoryInfo di = new DirectoryInfo("Caches");
            if (!Directory.Exists(di.FullName))
                Directory.CreateDirectory(di.FullName);

            // move any existing cache/sbdata.cas file over to the new caches directory
            foreach (var cacheName in Directory.EnumerateFiles(new FileInfo(Assembly.GetEntryAssembly().FullName).DirectoryName, "*.cache"))
            {
                FileInfo fi = new FileInfo(cacheName);
                File.Move(fi.FullName, ".\\Caches\\" + fi.Name);

                string sbDataName = fi.FullName.Replace(".cache", ".sbdata");
                if (File.Exists(sbDataName))
                    File.Move(sbDataName, ".\\Caches\\" + fi.Name.Replace(".cache", ".sbdata"));
            }

            // move any existing cache/sbdata.cas file over to the new caches directory
            foreach (var cacheName in Directory.EnumerateFiles(new FileInfo(Assembly.GetEntryAssembly().FullName).DirectoryName, "*.cache"))
            {
                FileInfo fi = new FileInfo(cacheName);
                File.Move(fi.FullName, ".\\Caches\\" + fi.Name);

                string sbDataName = fi.FullName.Replace(".cache", "_sbdata.cas");
                if (File.Exists(sbDataName))
                    File.Move(sbDataName, ".\\Caches\\" + fi.Name.Replace(".cache", "_sbdata.cas"));
            }

            AssetManagerImportResult result = new AssetManagerImportResult();
            // load data from game or cache
            await LoadData(KeyManager.Instance.GetKey("Key1"), result);

            // check to make sure SDK is up to date
            if (TypeLibrary.GetSdkVersion() != App.FileSystem.Head)
            {
                // requires updating
                SdkUpdateWindow sdkWin = new SdkUpdateWindow(this);
                sdkWin.ShowDialog();
            }

            if (App.IsEditor)
            {
                // load strings
                await LoadLocalizedStringResourceTables();
                await LoadStringList();

                // @todo: add support for Mod Manager startup actions
                foreach (StartupAction startupAction in App.PluginManager.StartupActions)
                {
                    await Task.Run(() =>
                    {
                        startupAction.Action(TaskLogger);
                    });
                }
            }

            App.Logger.Log("Initialization complete");
            App.NotificationManager.Show("Initialization complete");

            // cleanup any outstanding editor mods (in case of crash)
            if (Frosty.Core.App.IsEditor)
            {
                FileInfo exeInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                foreach (string file in Directory.EnumerateFiles(exeInfo.DirectoryName, $"Mods/{ProfilesLibrary.ProfileName}/EditorMod*"))
                {
                    File.Delete(file);
                }
                    
            }
            
            Close();

            if (App.IsEditor && result.InvalidatedDueToPatch)
            {
                // show the results of the most recent patch
                PatchSummaryWindow summaryWin = new PatchSummaryWindow(result);
                summaryWin.ShowDialog();
            }
        }

        private BitmapImage LoadBanner(byte[] banner)
        {
            if (banner == null || banner.Length == 0)
                return null;
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(banner))
            {
                bmp.BeginInit();
                bmp.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }
            bmp.Freeze();
            return bmp;
        }

        private async Task<int> LoadData(byte[] key, AssetManagerImportResult result)
        {
            await Task.Run(() =>
            {
                string basePath = Config.Get<string>("GamePath", null, ConfigScope.Game);

                App.FileSystem = new FileSystem(basePath);
                foreach (FileSystemSource source in ProfilesLibrary.Sources)
                    App.FileSystem.AddSource(source.Path, source.SubDirs);
                App.FileSystem.Initialize(key);

                App.ResourceManager = new ResourceManager(App.FileSystem);
                App.ResourceManager.SetLogger(TaskLogger);
                App.ResourceManager.Initialize();

                App.AssetManager = new AssetManager(App.FileSystem, App.ResourceManager);

                // initialize plugin extensions
                TypeLibrary.Initialize();
                App.PluginManager.Initialize();

                // load legacy asset manager if profile uses legacy system
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville)
                {
                    App.AssetManager.RegisterCustomAssetManager("legacy", typeof(LegacyFileManager));
                }

                // ensure mods folder is created
                DirectoryInfo di = new DirectoryInfo("Mods/" + ProfilesLibrary.ProfileName);
                if (!di.Exists)
                    Directory.CreateDirectory(di.FullName);
                
                App.AssetManager.SetLogger(TaskLogger);

                App.AssetManager.Initialize(true, result);
            });

            return 0;
        }

        private async Task<int> LoadLocalizedStringResourceTables()
        {
            TaskLogger.Log("Loading localized strings");
            await Task.Run(() =>
            {
                var localizedStringDb = App.PluginManager.GetLocalizedStringDatabase();
                localizedStringDb.Initialize();
            });
            return 0;
        }

        private async Task<int> LoadStringList()
        {
            TaskLogger.Log("Loading custom strings");
            await Task.Run(() => Utils.GetString(0));
            return 0;
        }

        public static void Show(Window owner)
        {
            FrostyProfileTaskWindow win = new FrostyProfileTaskWindow(owner);
            win.ShowDialog();
        }
    }
}
