using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrostyEditor.Utils;
using FrostyEditor.Views;

namespace FrostyEditor.ViewModels;

public partial class ProfileSelectWindowViewModel : ObservableObject
{
    public class ProfileConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public ProfileConfig()
        {
        }
        
        public ProfileConfig(string inKey)
        {
            Key = inKey;
        }
    }

    [ObservableProperty]
    private ProfileConfig? m_selectedProfile;

    public ObservableCollection<ProfileConfig> Profiles { get; set; } = new();

    public ProfileSelectWindowViewModel()
    {
        // TODO: get profiles from config file
        Profiles.Add(new ProfileConfig
        {
            Key = "NFS14",
            Path = "/some/path/Need for Speed(TM) Rivals"
        });
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
            Profiles.Add(new ProfileConfig(key) { Path = Path.GetDirectoryName(file.Path.LocalPath) ?? string.Empty });
        }
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