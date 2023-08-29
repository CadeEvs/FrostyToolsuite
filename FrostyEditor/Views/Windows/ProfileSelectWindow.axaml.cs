using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FrostyEditor.Views.Windows;

public partial class ProfileSelectWindow : Window
{
    public ProfileSelectWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        PointerPressed += (_, e) => BeginMoveDrag(e);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}