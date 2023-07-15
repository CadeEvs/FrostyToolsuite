using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FrostyEditor.ViewModels;
using FrostyEditor.Views;
using FrostyEditor.Views.Windows;

namespace FrostyEditor;

public class App : Application
{
    public override void Initialize()
    {
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