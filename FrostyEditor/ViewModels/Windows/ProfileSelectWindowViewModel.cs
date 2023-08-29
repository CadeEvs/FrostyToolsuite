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
        public object? Icon { get; set; }

        // if they would ever actually release non windows version we would need to change the extension here
        public string FileName => System.IO.Path.Combine(Path, $"{Key}.exe");

        public ProfileConfig(string inKey)
        {
            Key = inKey;
            Path = Config.Get("GamePath", string.Empty, ConfigScope.Game, Key);
            Name = ProfilesLibrary.GetDisplayName(Key) ?? Key;
            Icon = BitmapAssetValueConverter.Instance.Convert($"avares://FrostyEditor/Assets/Profiles/Icons/{ProfilesLibrary.GetInternalName(inKey)}.png");
            // TODO: Add image for these png, they are placeholder now
            // Assets/Profiles/Icons/
            //     anthem.png
            //     bf1.png
            //     bf4.png
            //     bf2042.png
            //     bfh.png
            //     bfv.png
            //     deadspace.png
            //     dragonage.png
            //     fifa17.png
            //     fifa18.png
            //     fifa19.png
            //     fifa20.png
            //     fifa21.png
            //     fifa22.png
            //     fifa23.png
            //     madden19.png
            //     madden20.png
            //     madden21.png
            //     madden22.png
            //     madden23.png
            //     masseffect.png
            //     mirrorsedge.png
            //     nfs14.png
            //     nfs16.png
            //     nfs17.png
            //     nfsedge.png
            //     nfsheat.png
            //     nfsunbound.png
            //     starwars.png
            //     starwarsiii.png
        }
    }

    [ObservableProperty]
    private ProfileConfig? m_selectedProfile;

    public ObservableCollection<ProfileConfig> Profiles { get; set; } = new();

    public ProfileSelectWindowViewModel()
    {
        // init ProfilesLibrary to load all profile json files
        ProfilesLibrary.Initialize();

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
            
            // Check if profile exists
            if (!ProfilesLibrary.HasProfile(key))
            {
                // TODO: Add MessageBox
                //FrostyMessageBox.Show($"There was an error when trying to load {key} using specified profile.", "Frosty Toolsuite");
                continue;
            }

            if (Config.AddGame(key, Path.GetDirectoryName(file.Path.LocalPath) ?? string.Empty))
            {
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
            Profiles.Remove(SelectedProfile);
            Config.Save(App.ConfigPath);
        }
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
