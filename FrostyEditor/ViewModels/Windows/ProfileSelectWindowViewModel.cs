using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frosty.Sdk;
using FrostyEditor.Utils;
using FrostyEditor.Views;

namespace FrostyEditor.ViewModels.Windows;

public partial class ProfileSelectWindowViewModel : ObservableObject
{
    public class ProfileConfig
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Path { get; set; }

        // if they would ever actually release non windows version we would need to change the extension here
        public string FileName => System.IO.Path.Combine(Path, $"{Key}.exe");

        public ProfileConfig(string inKey)
        {
            Key = inKey;
            Path = Config.Get("GamePath", string.Empty, ConfigScope.Game, Key);
            Name = ProfilesLibrary.GetDisplayName(Key) ?? Key;
        }
    }

    [ObservableProperty]
    private ProfileConfig? m_selectedProfile;

    public ObservableCollection<ProfileConfig> Profiles { get; set; } = new();

    public ProfileSelectWindowViewModel()
    {
        // init ProfilesLibrary to load all profile json files
        ProfilesLibrary.Initialize();
        
        foreach (string profile in Config.GameProfiles)
        {
            ProfileConfig config = new(profile);
            if (File.Exists(config.FileName))
            {
                Profiles.Add(config);   
            }
            else
            {
                Config.RemoveGame(profile);
            }
        }
        Config.Save(App.ConfigPath);
    }

    [RelayCommand]
    private async Task AddProfile()
    {
        TopLevel? topLevel = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        if (topLevel is null)
        {
            return;
        }
        
        // Start async operation to open the dialog.
        IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Game",
            AllowMultiple = false
        });

        foreach (IStorageFile file in files)
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);
            Config.AddGame(key, Path.GetDirectoryName(file.Path.LocalPath) ?? string.Empty);
            Profiles.Add(new ProfileConfig(key));
        }
        Config.Save(App.ConfigPath);
    }

    [RelayCommand]
    private void CloseWindow()
    {
        if (SelectedProfile is not null && Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            Window? window = desktopLifetime.MainWindow;

            MainWindowViewModel mainWindowViewModel = new(SelectedProfile.Key, SelectedProfile.Path);
            MainWindow mainWindow = new()
            {
                DataContext = mainWindowViewModel
            };

            mainWindow.Closing += (_, _) =>
            {
                mainWindowViewModel.CloseLayout();
            };
            
            desktopLifetime.MainWindow = mainWindow;
            
            desktopLifetime.Exit += (_, _) =>
            {
                mainWindowViewModel.CloseLayout();
            };
            
            desktopLifetime.MainWindow.Show();
            window?.Close();
        }
    }
}