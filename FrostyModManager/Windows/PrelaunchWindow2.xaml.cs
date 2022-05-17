using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using FrostySdk;
using Microsoft.Win32;
using Frosty.Controls;
using Frosty.Core;
using FrostySdk.IO;
using FrostySdk.Managers;

namespace FrostyModManager.Windows
{
    /// <summary>
    /// Interaction logic for PrelaunchWindow2.xaml
    /// </summary>
    public partial class PrelaunchWindow2 : FrostyDockableWindow
    {
        private List<FrostyConfiguration> configs = new List<FrostyConfiguration>();
        private FrostyConfiguration defaultConfig = null;

        Config ini = new Config();

        public PrelaunchWindow2()
        {
            InitializeComponent();
        }

        private void LaunchConfig(string profile)
        {
            // load profiles
            if (!ProfilesLibrary.Initialize(profile))
            {
                FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Mod Manager");
                Close();
                return;
            }

            if (ProfilesLibrary.RequiresKey && ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19)
            {
                byte[] keyData = null;
                if (!File.Exists(ProfilesLibrary.CacheName + ".key"))
                {
                    // prompt for encryption key
                    KeyPromptWindow keyPromptWin = new KeyPromptWindow();
                    if (keyPromptWin.ShowDialog() == false)
                    {
                        FrostyMessageBox.Show("Encryption key not entered. Unable to load profile.", "Frosty Editor");
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

            // launch Mod Manager
            SplashWindow splashWin = new SplashWindow();
            App.Current.MainWindow = splashWin;
            splashWin.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshConfigurationList();

            RemoveConfigButton.IsEnabled = false;
            LaunchConfigButton.IsEnabled = false;

            string defaultConfigName = Config.Get<string>("DefaultProfile", null);

            foreach (FrostyConfiguration name in configs)
            {
                if (name.ProfileName == defaultConfigName)
                {
                    defaultConfig = name;
                }
            }

            ConfigList.SelectedItem = defaultConfig;
        }

        private void RefreshConfigurationList()
        {
            configs.Clear();

            foreach (string profile in Config.GameProfiles)
            {
                try
                {
                    configs.Add(new FrostyConfiguration(profile));
                }
                catch (System.IO.FileNotFoundException)
                {
                    Config.RemoveGame(profile); // couldn't find the exe, so remove it from the profile list
                    Config.Save();
                }
            }

            ConfigList.ItemsSource = configs;
        }

        private void ConfigList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveConfigButton.IsEnabled = true;
            LaunchConfigButton.IsEnabled = true;
        }

        private void NewConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "*.exe (Game Executable)|*.exe",
                Title = "Choose Game Executable"
            };

            if (ofd.ShowDialog() == false)
            {
                FrostyMessageBox.Show("No game executable chosen.", "Frosty Mod Manager");
                return;
            }

            FileInfo fi = new FileInfo(ofd.FileName);

            // try to load game profile 
            if (!ProfilesLibrary.HasProfile(fi.Name.Remove(fi.Name.Length - 4)))
            {
                FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Mod Manager");
                return;
            }

            // make sure config doesnt already exist
            foreach (FrostyConfiguration config in configs)
            {
                if (config.ProfileName == fi.Name.Remove(fi.Name.Length - 4))
                {
                    FrostyMessageBox.Show("That game already has a configuration.");
                    return;
                }
            }

            // create
            Config.AddGame(fi.Name.Remove(fi.Name.Length - 4), fi.DirectoryName);
            configs.Add(new FrostyConfiguration(fi.Name.Remove(fi.Name.Length - 4)));
            Config.Save();

            ConfigList.Items.Refresh();
        }

        private async void ConfigList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ConfigList.SelectedIndex == -1)
                return;

            if (ConfigList.SelectedItem is FrostyConfiguration config)
            {
                LaunchConfig(config.ProfileName);
                await Task.Delay(1);
                Close();
            }
            ConfigList.SelectedIndex = -1;
        }

        private void RemoveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (FrostyMessageBox.Show("Are you sure you want to delete this configuration?", "Frosty Mod Manager", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                FrostyConfiguration selectedItem = ConfigList.SelectedItem as FrostyConfiguration;

                Config.RemoveGame(selectedItem.ProfileName);

                configs.Remove(selectedItem);
                ConfigList.Items.Refresh();

                ConfigList.SelectedIndex = 0;
                Config.Save();
            }
        }

        private async void LaunchConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigList.SelectedIndex == -1)
                return;

            if (ConfigList.SelectedItem is FrostyConfiguration config)
            {
                LaunchConfig(config.ProfileName);
                await Task.Delay(1);
                Close();
            }
            ConfigList.SelectedIndex = -1;
        }

        private void ScanForGamesButton_Click(object sender, RoutedEventArgs e)
        {
            using (RegistryKey lmKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node"))
            {
                int totalCount = 0;

                IterateSubKeys(lmKey, ref totalCount);
            }

            ConfigList.Items.Refresh();
        }

        private void IterateSubKeys(RegistryKey subKey, ref int totalCount)
        {
            foreach (string subKeyName in subKey.GetSubKeyNames())
            {
                try
                {
                    IterateSubKeys(subKey.OpenSubKey(subKeyName), ref totalCount);
                }
                catch (System.Security.SecurityException)
                {
                    // do nothing
                }
            }

            foreach (string subKeyValue in subKey.GetValueNames())
            {
                if (subKeyValue.IndexOf("Install Dir", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    string installDir = subKey.GetValue("Install Dir") as string;
                    if (string.IsNullOrEmpty(installDir))
                        continue;
                    if (!Directory.Exists(installDir))
                        continue;

                    foreach (string filename in Directory.EnumerateFiles(installDir, "*.exe"))
                    {
                        FileInfo fi = new FileInfo(filename);
                        string nameWithoutExt = fi.Name.Replace(fi.Extension, "");

                        if (ProfilesLibrary.HasProfile(nameWithoutExt))
                        {
                            foreach (FrostyConfiguration config in configs)
                            {
                                if (config.ProfileName == fi.Name.Remove(fi.Name.Length - 4))
                                    return;
                            }

                            Config.AddGame(fi.Name.Remove(fi.Name.Length - 4), fi.DirectoryName);
                            configs.Add(new FrostyConfiguration(fi.Name.Remove(fi.Name.Length - 4)));

                            totalCount++;
                        }
                    }
                }
            }
        }
    }
}
