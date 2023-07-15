using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrostyEditor.Views;

namespace FrostyEditor.ViewModels;

public partial class ProfileSelectWindowViewModel : ObservableObject
{
    private bool m_wasSuccessful = true;
    private string m_profileKey = string.Empty;
    private string m_profilePath = string.Empty;
    
    private readonly IClassicDesktopStyleApplicationLifetime? m_desktopLifetime;

    public ProfileSelectWindowViewModel()
    {
        m_desktopLifetime = null;
    }
    
    public ProfileSelectWindowViewModel(IClassicDesktopStyleApplicationLifetime inDesktopLifetime)
    {
        m_desktopLifetime = inDesktopLifetime;
    }

    [RelayCommand]
    private void CloseWindow()
    {
        if (m_desktopLifetime is null)
        {
            return;
        }
        
        if (m_wasSuccessful)
        {
            Window? window = m_desktopLifetime.MainWindow;

            MainWindowViewModel mainWindowViewModel = new(m_profileKey, m_profilePath);
            MainWindow mainWindow = new()
            {
                DataContext = mainWindowViewModel
            };

            mainWindow.Closing += (_, _) =>
            {
                mainWindowViewModel.CloseLayout();
            };
            
            m_desktopLifetime.MainWindow = mainWindow;
            
            m_desktopLifetime.Exit += (_, _) =>
            {
                mainWindowViewModel.CloseLayout();
            };
            
            m_desktopLifetime.MainWindow!.Show();
            window?.Close();
        }
    }
}