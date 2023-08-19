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
using FrostyEditor.Views.Windows;

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
        IReadOnlyList<IStorageFile>? files = await FileService.OpenFilesAsync(new FilePickerOpenOptions
        {
            Title = "Select Game Executable",
            AllowMultiple = false
        });

        if (files is null)
        {
            return;
        }
        
        foreach (IStorageFile file in files)
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);
            Config.AddGame(key, Path.GetDirectoryName(file.Path.LocalPath) ?? string.Empty);
            Profiles.Add(new ProfileConfig(key));
        }
        Config.Save(App.ConfigPath);
    }

    [RelayCommand]
    private void SelectProfile()
    {
        if (SelectedProfile is not null && Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            Window? window = desktopLifetime.MainWindow;

            ProfileTaskWindowViewModel viewModel = new();
            
            desktopLifetime.MainWindow = new ProfileTaskWindow
            {
                DataContext = viewModel
            };
            
            desktopLifetime.MainWindow.Loaded += async (_, _) =>
            {
                await viewModel.Setup(SelectedProfile.Key, SelectedProfile.Path);
            };
            
            desktopLifetime.MainWindow.Show();
            
            window?.Close();
        }
    }
    
    [RelayCommand]
    private void Cancel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.MainWindow?.Close();
        }
    }
}