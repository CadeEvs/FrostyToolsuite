using Frosty.Controls;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Frosty.Core.Windows
{
    /// <summary>
    /// Interaction logic for ClassSelector.xaml
    /// </summary>
    public partial class ClassSelector : FrostyDockableWindow
    {
        public Type SelectedClass => classSelector.SelectedClass;
        private List<Type> types = new List<Type>();

        public ClassSelector(Type[] inTypes, bool allowAssets = false)
        {
            InitializeComponent();

            foreach (Type type in inTypes)
            {
                // ignore abstract classes
                if (type.GetCustomAttribute<IsAbstractAttribute>() != null)
                    continue;

                if (TypeLibrary.IsSubClassOf(type, "Asset"))
                {
                    // only allow asset types that can be created inline
                    if (type.GetCustomAttribute<IsInlineAttribute>() == null && !allowAssets)
                        continue;
                }
                types.Add(type);
            }

            selectButton.IsEnabled = false;
            classSelector.Types = types;

            Loaded += ClassSelector_Loaded;
        }

        private void ClassSelector_Loaded(object sender, RoutedEventArgs e)
        {
            classSelector.Focus();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void moduleClassView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectButton.IsEnabled = false;
            if (e.NewValue == null)
                return;
            if (e.NewValue is ClassSelectorModuleItem)
                return;
            selectButton.IsEnabled = true;
        }

        private void moduleClassView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
