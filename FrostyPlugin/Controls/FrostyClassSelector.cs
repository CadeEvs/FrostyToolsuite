using Frosty.Controls;
using FrostySdk.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Frosty.Core.Controls
{
    public class ClassSelectorModuleItem
    {
        public string Name { get; set; }
        public List<ClassSelectorClassItem> Classes { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
    }
    public class ClassSelectorClassItem
    {
        public Type Type { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsFiltered { get; set; }
        public string Name
        {
            get
            {
                DisplayNameAttribute attr = Type.GetCustomAttribute<DisplayNameAttribute>();
                return (attr != null) ? attr.Name : Type.Name;
            }
        }
    }
    public enum FrostyClassSelectionMode
    {
        Click,
        Drag
    }

    [TemplatePart(Name = PART_FilterTextBox, Type = typeof(FrostyWatermarkTextBox))]
    [TemplatePart(Name = PART_ModuleClassView, Type = typeof(TreeView))]
    public class FrostyClassSelector : Control
    {
        private const string PART_FilterTextBox = "PART_FilterTextBox";
        private const string PART_ModuleClassView = "PART_ModuleClassView";

        #region -- Types --
        public static readonly DependencyProperty TypesProperty = DependencyProperty.Register("Types", typeof(IList<Type>), typeof(FrostyClassSelector), new UIPropertyMetadata(null));
        public IList<Type> Types
        {
            get => (IList<Type>)GetValue(TypesProperty);
            set => SetValue(TypesProperty, value);
        }
        #endregion

        #region -- Mode --
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(FrostyClassSelectionMode), typeof(FrostyClassSelector), new UIPropertyMetadata(FrostyClassSelectionMode.Click));
        public FrostyClassSelectionMode Mode
        {
            get => (FrostyClassSelectionMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }
        #endregion

        public Type SelectedClass
        {
            get
            {
                //if (moduleClassView.SelectedItem is ClassSelectorModuleItem)
                //    return null;
                if (moduleClassView.SelectedItem is ClassSelectorClassItem item)
                    return item.Type;
                return null;
            }
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;
        public event MouseButtonEventHandler ItemDoubleClicked;
        public event RoutedEventHandler ItemBeginDrag;

        private List<ClassSelectorModuleItem> modules = new List<ClassSelectorModuleItem>();
        private string filterText = "";

        private FrostyWatermarkTextBox filterTextBox;
        private TreeView moduleClassView;

        static FrostyClassSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyClassSelector), new FrameworkPropertyMetadata(typeof(FrostyClassSelector)));
        }

        public FrostyClassSelector()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            filterTextBox = GetTemplateChild(PART_FilterTextBox) as FrostyWatermarkTextBox;
            moduleClassView = GetTemplateChild(PART_ModuleClassView) as TreeView;

            moduleClassView.SelectedItemChanged += moduleClassView_SelectedItemChanged;
            if (Mode == FrostyClassSelectionMode.Click)
                moduleClassView.MouseDoubleClick += moduleClassView_MouseDoubleClick;
            else
                moduleClassView.PreviewMouseLeftButtonDown += ModuleClassView_PreviewMouseLeftButtonDown;
            filterTextBox.LostFocus += filterTextBox_LostFocus;
            filterTextBox.KeyUp += filterTextBox_KeyUp;

            UpdateTreeView();
        }

        private void ModuleClassView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = GetDataFromMousePosition(moduleClassView);
            if (item == null || item is ClassSelectorModuleItem)
                return;

            ItemBeginDrag?.Invoke(sender, new RoutedEventArgs());
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            filterTextBox.Focus();
        }

        private void moduleClassView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemChanged?.Invoke(sender, e);
        }

        private void moduleClassView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (moduleClassView.SelectedItem == null || moduleClassView.SelectedItem is ClassSelectorModuleItem)
                return;

            ItemDoubleClicked?.Invoke(sender, e);
        }

        private void filterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (filterText != filterTextBox.Text)
            {
                filterText = filterTextBox.Text;
                UpdateTreeView();
            }
        }

        private void filterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                filterTextBox_LostFocus(this, new RoutedEventArgs());
        }

        private void UpdateTreeView()
        {
            modules.Clear();

            foreach (Type type in Types)
            {
                if (type.GetCustomAttribute<IsAbstractAttribute>() != null)
                    continue;

                if (filterText != "")
                {
                    if (type.Name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                }

                EbxClassMetaAttribute attr = type.GetCustomAttribute<EbxClassMetaAttribute>();
                string moduleName = (attr != null) ? attr.Namespace : "Reflection";

                int index = modules.FindIndex((ClassSelectorModuleItem item) => { return item.Name.Equals(moduleName); });
                if (index == -1)
                {
                    index = modules.Count;
                    modules.Add(new ClassSelectorModuleItem() { Name = moduleName, Classes = new List<ClassSelectorClassItem>() });
                }

                ClassSelectorModuleItem module = modules[index];
                module.Classes.Add(new ClassSelectorClassItem() { Type = type });
            }

            moduleClassView.ItemsSource = modules;
            moduleClassView.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }

        private object GetDataFromMousePosition(ItemsControl source)
        {
            if (source.InputHitTest(Mouse.GetPosition(source)) is UIElement element)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    (element as TreeViewItem).IsSelected = true;
                    return data is ClassSelectorModuleItem ? GetDataFromMousePosition(element as TreeViewItem) : data;
                }
            }

            return null;
        }
    }
}
