using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Utils;
using FrostyEditor.Views;

namespace FrostyEditor.ViewModels.Windows;

public partial class ProfileTaskWindowViewModel : ObservableObject, ILogger
{   
    [ObservableProperty]
    private string? m_info;
    
    [ObservableProperty]
    private double m_progress = 0.0;

    public async Task Setup(string inKey, string inPath)
    {
        AssetManager.Logger = this;
        if (await MainWindowViewModel.SetupFrostySdk(inKey, inPath))
        {
            ShowMainWindow();
        }
        else
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktopLifetime.MainWindow?.Close();
            }
        }
    }
    
    private void ShowMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            MainWindowViewModel mainWindowViewModel;
            Window? window = desktopLifetime.MainWindow;
            MainWindow mainWindow = new()
            {
                DataContext = mainWindowViewModel = new()
            };

            mainWindow.Closing += (_, _) =>
            {
                mainWindowViewModel?.CloseLayout();
            };
            
            desktopLifetime.MainWindow = mainWindow;
            
            desktopLifetime.Exit += (_, _) =>
            {
                mainWindowViewModel?.CloseLayout();
            };
            
            desktopLifetime.MainWindow.Show();
            window?.Close();
        }
    }

    public void Report(string category, string message)
    {
        Info = message;
    }

    public void Report(string category, string message, double progress)
    {
        Info = message;
        Progress = progress;
    }

    public void Report(double progress)
    {
        Progress = progress;
    }
}