using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using FrostyEditor.Interfaces;

namespace FrostyEditor.Themes;

public class FluentThemeManager : IThemeManager
{
    private static readonly Uri s_baseUri = new("avares://FrostyEditor/Styles");

    private static readonly FluentTheme s_fluent = new();

    private static readonly DockFluentTheme s_dockFluent = new();

    private static readonly TreeDataGridFluentTheme s_treeDataGridFluent = new();

    private static readonly Styles s_fluentDark = new()
    {
        new StyleInclude(s_baseUri)
        {
            Source = new Uri("avares://FrostyEditor/Themes/FluentDark.axaml")
        }
    };

    private static readonly Styles s_fluentLight = new()
    {
        new StyleInclude(s_baseUri)
        {
            Source = new Uri("avares://FrostyEditor/Themes/FluentLight.axaml")
        }
    };

    public void Switch(int index)
    {
        if (Application.Current is null)
        {
            return;
        }

        switch (index)
        {
            // Fluent Light
            case 0:
            {
                Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                Application.Current.Styles[0] = s_fluent;
                Application.Current.Styles[1] = s_dockFluent;
                Application.Current.Styles[2] = s_treeDataGridFluent;
                Application.Current.Styles[3] = s_fluentLight;
                break;
            }
            // Fluent Dark
            case 1:
            {
                Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                Application.Current.Styles[0] = s_fluent;
                Application.Current.Styles[1] = s_dockFluent;
                Application.Current.Styles[2] = s_treeDataGridFluent;
                Application.Current.Styles[3] = s_fluentDark;
                break;
            }
        }
    }

    public void Initialize(Application application)
    {
        application.RequestedThemeVariant = ThemeVariant.Dark;
        application.Styles.Insert(0, s_fluent);
        application.Styles.Insert(1, s_dockFluent);
        application.Styles.Insert(2, s_treeDataGridFluent);
        application.Styles.Insert(3, s_fluentDark);
    }
}
