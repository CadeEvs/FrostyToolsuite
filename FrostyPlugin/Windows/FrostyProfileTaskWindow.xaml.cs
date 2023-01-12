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

        public FrostyProfileTaskWindow(Window owner)
        {
            InitializeComponent();

            versionTextBlock.Text = App.Version;
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();

            Owner = owner;
            TaskLogger = new FrostyProfileTaskWindowLogger(this);
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
                    {
                        writer.Write(keyData);
                    }
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
            {
                Directory.CreateDirectory(di.FullName);

            }
            // move any existing cache/sbdata.cas file over to the new caches directory
            foreach (string cacheName in Directory.EnumerateFiles(new FileInfo(Assembly.GetEntryAssembly().FullName).DirectoryName, "*.cache"))
            {
                FileInfo fi = new FileInfo(cacheName);
                File.Move(fi.FullName, ".\\Caches\\" + fi.Name);

                string sbDataName = fi.FullName.Replace(".cache", "_sbdata.cas");
                if (File.Exists(sbDataName))
                {
                    File.Move(sbDataName, ".\\Caches\\" + fi.Name.Replace(".cache", "_sbdata.cas"));
                }
            }

            AssetManagerImportResult result = new AssetManagerImportResult();
            // load data from game or cache
            await LoadData(KeyManager.Instance.GetKey("Key1"), result);

            // check to make sure SDK is up to date
            if (TypeLibrary.GetSdkVersion() != App.FileSystemManager.Head)
            {
                // requires updating
                if (UpdateSdk())
                {
                    Close();
                }
                if (ProfilesLibrary.EbxVersion > 4)
                {
                    // initialze assetmanager anyways
                    await FinishLoadingData(result);
                }
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
            if (App.IsEditor)
            {
                FileInfo exeInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                foreach (string file in Directory.EnumerateFiles(exeInfo.DirectoryName, $"Mods/{ProfilesLibrary.ProfileName}/EditorMod*"))
                {
                    File.Delete(file);
                }

            }

            DialogResult = true;

            Close();

            if (App.IsEditor && result.InvalidatedDueToPatch)
            {
                // show the results of the most recent patch
                PatchSummaryWindow summaryWin = new PatchSummaryWindow(result);
                summaryWin.ShowDialog();
            }
        }

        private bool UpdateSdk()
        {
            SdkUpdateWindow sdkWin = new SdkUpdateWindow(this);
            if (sdkWin.ShowDialog() == true)
            {
                return true;
            }
            else if (TypeLibrary.GetSdkVersion() == 0)
            {
                MessageBoxResult result = FrostyMessageBox.Show("Missing SDK.\nPlease generate a SDK for this game.", "Frosty", MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    return UpdateSdk();
                }
                return true;
            }
            return false;
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

                App.FileSystemManager = new FileSystemManager(basePath);
                foreach (FileSystemSource source in ProfilesLibrary.Sources)
                {
                    App.FileSystemManager.AddSource(source.Path, source.SubDirs);
                }
                App.FileSystemManager.Initialize(key);

                App.ResourceManager = new ResourceManager(App.FileSystemManager);
                App.ResourceManager.SetLogger(TaskLogger);
                App.ResourceManager.Initialize();

                App.AssetManager = new AssetManager(App.FileSystemManager, App.ResourceManager);

                TypeLibrary.Initialize();

                // initialize plugin extensions
                App.PluginManager.Initialize();

                // load legacy asset manager if profile uses legacy system
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17,
                        ProfileVersion.Fifa18,
                        ProfileVersion.Madden19,
                        ProfileVersion.Fifa19,
                        ProfileVersion.Madden20,
                        ProfileVersion.Fifa20,
                        ProfileVersion.PlantsVsZombiesBattleforNeighborville))
                {
                    App.AssetManager.RegisterCustomAssetManager("legacy", typeof(LegacyFileManager));
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21, ProfileVersion.Madden22, ProfileVersion.Fifa22,
                    ProfileVersion.Madden23, ProfileVersion.Fifa23))
                {
                    App.AssetManager.RegisterCustomAssetManager("legacy", typeof(LegacyFileManagerV2));
                }

                // ensure mods folder is created
                DirectoryInfo di = new DirectoryInfo("Mods/" + ProfilesLibrary.ProfileName);
                if (!di.Exists)
                {
                    Directory.CreateDirectory(di.FullName);
                }

                // newer ebx formats need the SDK for the types, so update the SDK before generating the cache
                if (ProfilesLibrary.EbxVersion > 4)
                {
                    if (TypeLibrary.GetSdkVersion() != App.FileSystemManager.Head)
                    {
                        return;
                    }
                }

                App.AssetManager.SetLogger(TaskLogger);
                App.AssetManager.Initialize(App.IsEditor, result);
            });

            return 0;
        }

        private async Task<int> FinishLoadingData(AssetManagerImportResult result)
        {
            await Task.Run(() =>
            {
                App.AssetManager.SetLogger(TaskLogger);
                App.AssetManager.Initialize(App.IsEditor, result);
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
            await Task.Run(() => Utils.LoadStringList("strings.txt", TaskLogger));
            return 0;
        }

        public static bool Show(Window owner)
        {
            FrostyProfileTaskWindow win = new FrostyProfileTaskWindow(owner);
            return win.ShowDialog() == true;
        }
    }
}
