using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.Runtime.InteropServices;

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