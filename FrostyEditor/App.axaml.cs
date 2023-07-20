using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FrostyEditor.Interfaces;
using FrostyEditor.Themes;
using FrostyEditor.Utils;
using FrostyEditor.ViewModels;
using FrostyEditor.ViewModels.Windows;
using FrostyEditor.Views.Windows;

namespace FrostyEditor;

public class App : Application
{
    public static string ConfigPath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Frosty/editor_config.json";
    
    public static IThemeManager? ThemeManager;
    
    public override void Initialize()
    {
        ThemeManager = new FluentThemeManager();
        ThemeManager.Initialize(this);
        
        Config.Load(ConfigPath);

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