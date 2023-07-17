using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FrostyEditor.Interfaces;
using FrostyEditor.Themes;
using FrostyEditor.ViewModels;
using FrostyEditor.Views.Windows;

namespace FrostyEditor;

public class App : Application
{
    public static IThemeManager? ThemeManager;
    
    public override void Initialize()
    {
        ThemeManager = new FluentThemeManager();
        ThemeManager.Initialize(this);

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopLifetime:
            {
                ProfileSelectWindow selectWindow = new()
                {
                    DataContext = new ProfileSelectWindowViewModel()
                };

                desktopLifetime.MainWindow = selectWindow;

                break;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}