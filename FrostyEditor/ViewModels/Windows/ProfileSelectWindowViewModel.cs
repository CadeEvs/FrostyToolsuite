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

        RefreshProfileList();
    }

    public void RefreshProfileList()
    {
        Profiles.Clear();
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

    public static FilePickerFileType ImageExe { get; } = new("Executable")
    {
        Patterns = new[] { "*.exe" },
        // https://developer.apple.com/documentation/uniformtypeidentifiers/uttype/3551492-exe
        AppleUniformTypeIdentifiers = new[] { "exe" },
        // https://www.iana.org/assignments/media-types/application/vnd.microsoft.portable-executable
        MimeTypes = new[] { "vnd.microsoft.portable-executable" }
    };

    [RelayCommand]
    private async Task AddProfile()
    {
        IReadOnlyList<IStorageFile>? files = await FileService.OpenFilesAsync(new FilePickerOpenOptions
        {
            Title = "Select Game Executable",
            FileTypeFilter = new[]
            {
                ImageExe
#if DEBUG
                , FilePickerFileTypes.All
#endif
            },
            AllowMultiple = false
        });

        if (files is null)
        {
            return;
        }
        
        foreach (IStorageFile file in files)
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);
            
            // Check if profile exists
            if (!ProfilesLibrary.HasProfile(key))
            {
                // TODO: Add MessageBox
                //FrostyMessageBox.Show($"There was an error when trying to load {key} using specified profile.", "Frosty Toolsuite");
                continue;
            }

            // Make sure config doesn't already exist
            bool isProfileExist = false;
            foreach (string profile in Config.GameProfiles)
            {
                if (key == profile)
                {
                    // TODO: Add MessageBox
                    //FrostyMessageBox.Show($"{key} already has a configuration.");
                    isProfileExist = true;
                    break;
                }
            }

            if (ProfilesLibrary.HasAntiCheat)
            {
                // TODO: Add MessageBox
                //FrostyMessageBox.Show($"{key} contains EasyAntiCheat. We will not support nor assist anyone who attempts to bypass it.");
            }

            if (!isProfileExist)
            {
                Config.AddGame(key, Path.GetDirectoryName(file.Path.LocalPath) ?? string.Empty);
                Profiles.Add(new ProfileConfig(key));
            }
        }
        Config.Save(App.ConfigPath);
    }

    [RelayCommand]
    private void DeleteProfile()
    {
        if (SelectedProfile is not null)
        {
            Config.RemoveGame(SelectedProfile.Key);
            Config.Save(App.ConfigPath);
        }
        RefreshProfileList();
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