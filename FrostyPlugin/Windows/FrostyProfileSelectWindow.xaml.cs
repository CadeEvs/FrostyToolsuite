using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frosty.Controls;
using FrostySdk;
using Microsoft.Win32;

namespace Frosty.Core.Windows
{
    public partial class FrostyProfileSelectWindow
    {
        private readonly List<FrostyConfiguration> configurations = new List<FrostyConfiguration>();
        private string selectedProfileName;
        
        public FrostyProfileSelectWindow()
        {
            InitializeComponent();
        }

        private void ProfileSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshConfigurationList();
            
            // TODO: @techdebt only call this once or when needed
            ScanGames();

            RefreshConfigurationList();
        }
        
        private void RefreshConfigurationList()
        {
            configurations.Clear();

            foreach (string profile in Config.GameProfiles)
            {
                try
                {
                    configurations.Add(new FrostyConfiguration(profile));
                }
                catch (System.IO.FileNotFoundException)
                {
                    Config.RemoveGame(profile); // couldn't find the exe, so remove it from the profile list
                    Config.Save();
                }
            }

            ConfigurationListView.ItemsSource = configurations;
        }

        private void SelectConfiguration()
        {
            if (ConfigurationListView.SelectedIndex == -1)
                return;

            if (ConfigurationListView.SelectedItem is FrostyConfiguration configuration)
            {
                selectedProfileName = configuration.ProfileName;
                Close();
            }
        }
        
        private async void ScanGames()
        {
            await Task.Run((() =>
            {
                using (RegistryKey lmKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node"))
                {
                    int totalCount = 0;

                    IterateSubKeys(lmKey, ref totalCount);
                }
            }));
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
                            foreach (FrostyConfiguration config in configurations)
                            {
                                if (config.ProfileName == fi.Name.Remove(fi.Name.Length - 4))
                                    return;
                            }

                            Config.AddGame(fi.Name.Remove(fi.Name.Length - 4), fi.DirectoryName);
                            configurations.Add(new FrostyConfiguration(fi.Name.Remove(fi.Name.Length - 4)));

                            totalCount++;
                        }
                    }
                }
            }
        }

        public static string Show(bool hasLoadedProfile = false)
        {
            string profileName = "";

            FrostyProfileSelectWindow win = new FrostyProfileSelectWindow() { Owner = Application.Current.MainWindow };
            win.ShowDialog();
            
            profileName = win.selectedProfileName;

            return profileName;
        }

        private void RefreshButton_OnClicked(object sender, RoutedEventArgs e)
        {
            ScanGames();
        }
        
        private void AddConfigurationButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "*.exe (Game Executable)|*.exe",
                Title = "Choose Game Executable"
            };

            if (ofd.ShowDialog() == false)
            {
                FrostyMessageBox.Show("No game executable chosen.", "Frosty Core");
                return;
            }

            FileInfo fi = new FileInfo(ofd.FileName);

            // try to load game profile 
            if (!ProfilesLibrary.HasProfile(fi.Name.Remove(fi.Name.Length - 4)))
            {
                FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Core");
                return;
            }

            // make sure config doesnt already exist
            foreach (FrostyConfiguration config in configurations)
            {
                if (config.ProfileName == fi.Name.Remove(fi.Name.Length - 4))
                {
                    FrostyMessageBox.Show("That game already has a configuration.");
                    return;
                }
            }

            // create
            Config.AddGame(fi.Name.Remove(fi.Name.Length - 4), fi.DirectoryName);
            configurations.Add(new FrostyConfiguration(fi.Name.Remove(fi.Name.Length - 4)));
            Config.Save();

            ConfigurationListView.Items.Refresh();
        }

        private void SelectConfigurationButton_OnClick(object sender, RoutedEventArgs e)
        {
            SelectConfiguration();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfigurationListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectConfiguration();
        }

        private void ConfigurationListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectGameTextBlock.IsVisible)
            {
                SelectGameTextBlock.Visibility = Visibility.Collapsed;
            }
            
            if (ConfigurationListView.SelectedItem is FrostyConfiguration configuration)
            {
                ProfileNameTextBlock.Text = configuration.GameName;
                ProfilePathTextBlock.Text = configuration.GamePath;
            }
        }
    }
}