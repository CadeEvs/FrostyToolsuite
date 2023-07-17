using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrostyEditor.Views;

namespace FrostyEditor.ViewModels;

public partial class ProfileSelectWindowViewModel : ObservableObject
{
    public class ProfileConfig
    {
        public string Key { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
    
    private bool m_wasSuccessful = true;
    
    private string m_profileKey = string.Empty;
    private string m_profilePath = string.Empty;

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
    private void CloseWindow()
    {
        if (m_wasSuccessful && Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            Window? window = desktopLifetime.MainWindow;

            MainWindowViewModel mainWindowViewModel = new(m_profileKey, m_profilePath);
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