using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FrostyEditor.Views.Windows;

public partial class WelcomeWindow : Window
{
    public WelcomeWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}