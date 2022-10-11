using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Legacy;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FrostyModManager.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            versionTextBlock.Text = App.Version;
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            //Config.Save(App.configFilename);

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

                string sbDataName = fi.FullName.Replace(".cache", "_sbdata.cas");
                if (File.Exists(sbDataName))
                    File.Move(sbDataName, ".\\Caches\\" + fi.Name.Replace(".cache", "_sbdata.cas"));
            }

            TypeLibrary.Initialize();

            // load filesystem to gather details on game version
            string basePath = Config.Get<string>("GamePath", "", ConfigScope.Game);
            //string basePath = Config.Get<string>("Init", "GamePath", "");
            Frosty.Core.App.FileSystem = new FileSystem(basePath);
            foreach (FileSystemSource source in ProfilesLibrary.Sources)
                Frosty.Core.App.FileSystem.AddSource(source.Path, source.SubDirs);
            Frosty.Core.App.FileSystem.Initialize(KeyManager.Instance.GetKey("Key1"));

            // check to make sure SDK is up to date
            if (!File.Exists(Frosty.Core.App.FileSystem.CacheName + ".cache"))
            {
                ILogger logger = new SplashWindowLogger(this);

                // load data from game or cache
                await LoadData(logger);

                if (TypeLibrary.GetSdkVersion() != Frosty.Core.App.FileSystem.Head)
                {
                    // requires updating
                    SdkUpdateWindow sdkWin = new SdkUpdateWindow(this);
                    sdkWin.ShowDialog();
                }
            }

            // clear out all global managers
            Frosty.Core.App.AssetManager = null;
            Frosty.Core.App.ResourceManager = null;
            Frosty.Core.App.FileSystem = null;
            GC.Collect();

            // show the main editor window
            MainWindow win = new MainWindow();
            App.Current.MainWindow = win;
            win.Show();

            Close();
        }

        private BitmapImage LoadBanner(byte[] banner)
        {
            if (banner == null||banner.Length == 0)
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

        private async Task<int> LoadData(ILogger logger)
        {
            await Task.Run(() =>
            {
                // need to load the managers into the global core app class as the SDK updater
                // requires them to be valid

                Frosty.Core.App.ResourceManager = new ResourceManager(Frosty.Core.App.FileSystem);
                Frosty.Core.App.ResourceManager.SetLogger(logger);
                Frosty.Core.App.ResourceManager.Initialize();

                Frosty.Core.App.AssetManager = new AssetManager(Frosty.Core.App.FileSystem, Frosty.Core.App.ResourceManager);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville)
                    Frosty.Core.App.AssetManager.RegisterCustomAssetManager("legacy", typeof(LegacyFileManager));
                Frosty.Core.App.AssetManager.SetLogger(logger);

                Frosty.Core.App.AssetManager.Initialize(additionalStartup: false);
            });

            return 0;
        }
    }
}
