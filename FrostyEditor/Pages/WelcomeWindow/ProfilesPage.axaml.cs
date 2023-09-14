using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FrostyEditor.Pages.WelcomeWindow;

public partial class ProfilesPage : UserControl
{
    public ProfilesPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}