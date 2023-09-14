using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FrostyEditor.Pages.WelcomeWindow;

public partial class ProjectsPage : UserControl
{
    public ProjectsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}