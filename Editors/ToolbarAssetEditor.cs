using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using LevelEditorPlugin.Library.Reflection;
using LevelEditorPlugin.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Frosty.Core.Managers;

namespace LevelEditorPlugin.Editors
{
    #region -- Toolbar Items --

    public class DockingToolbarItem : ToolbarItem, INotifyPropertyChanged
    {
        public bool IsToggled
        {
            get => isToggled;
            set
            {
                if (isToggled != value)
                {
                    isToggled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool DockChanged { get; set; }
        public int SelectedDockIndex
        {
            get => (int)Location;
            set
            {
                Controls.DockLocation newLoc = (Controls.DockLocation)value;
                if (newLoc != Location)
                {
                    Location = newLoc;
                    DockChanged = true;

                    CommandToInvoke.Execute(this);

                    DockChanged = false;
                }
            }
        }
        public Controls.DockLocation Location { get; set; }
        public RelayCommand CommandToInvoke { get; private set; }
        public ImageSource NewIcon { get; private set; }

        private string uniqueId;
        private bool isToggled;

        public DockingToolbarItem(string text, string tooltip, string icon, RelayCommand inCommand, Controls.DockManager dockManager, string uid)
            : base(text, tooltip, "", inCommand)
        {
            uniqueId = uid;
            Location = dockManager.GetItemsLastKnownDockLocation(uid);
            IsToggled = dockManager.IsItemVisible(uid);

            dockManager.ItemAdded += (o, e) => { if ((string)o == uniqueId) IsToggled = true; };
            dockManager.ItemRemoved += (o, e) => { if ((string)o == uniqueId) IsToggled = false; };

            CommandToInvoke = new RelayCommand((o) =>
            {
                var toolbarItem = o as DockingToolbarItem;
                bool visible = dockManager.IsItemVisible(uniqueId);

                if (visible || toolbarItem.DockChanged)
                {
                    dockManager.RemoveItem(uniqueId);
                    if (toolbarItem.DockChanged)
                    {
                        if (!visible)
                            return;
                    }
                    else if (visible)
                        return;
                }

                Command.Execute(o);
            });

            NewIcon = (new ImageSourceConverter().ConvertFromString("pack://application:,,,/LevelEditorPlugin;component/" + icon) as ImageSource);
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class FloatingOnlyDockingToolbarItem : DockingToolbarItem
    {
        public FloatingOnlyDockingToolbarItem(string text, string tooltip, string icon, RelayCommand inCommand, Controls.DockManager dockManager, string uid)
            : base(text, tooltip, icon, inCommand, dockManager, uid)
        {
        }
    }

    public class ToggleToolbarItem : ToolbarItem, INotifyPropertyChanged
    {
        public bool IsToggled
        {
            get => isToggled;
            set
            {
                if (isToggled != value)
                {
                    isToggled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ImageSource NewIcon { get; private set; }
        protected bool isToggled;

        public ToggleToolbarItem(string text, string tooltip, string icon, bool defaultValue, RelayCommand inCommand)
            : base(text, tooltip, "", inCommand)
        {
            isToggled = defaultValue;

            string app = icon.Substring(0, icon.IndexOf('/'));
            string path = icon.Substring(icon.IndexOf('/') + 1);

            NewIcon = (new ImageSourceConverter().ConvertFromString($"pack://application:,,,/{app};component/{path}") as ImageSource);
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class RegularToolbarItem : ToolbarItem
    {
        public ImageSource NewIcon { get; private set; }

        public RegularToolbarItem(string text, string tooltip, string icon, RelayCommand inCommand)
            : base(text, tooltip, "", inCommand)
        {
            string app = icon.Substring(0, icon.IndexOf('/'));
            string path = icon.Substring(icon.IndexOf('/') + 1);

            NewIcon = (new ImageSourceConverter().ConvertFromString($"pack://application:,,,/{app};component/{path}") as ImageSource);
        }
    }

    public class ComboBoxToolbarItem : ToolbarItem, INotifyPropertyChanged
    {
        private class DefaultNameConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.GetType().Name;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable ItemsSource { get; private set; }
        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    NotifyPropertyChanged();
                    Command.Execute(selectedItem);
                }
            }
        }
        public IValueConverter NameConverter => nameConverter;
        public double MinWidth => minWidth;

        private object selectedItem;
        private IValueConverter nameConverter;
        private double minWidth;

        public ComboBoxToolbarItem(string text, string tooltip, IEnumerable itemsSource, object defaultSelection, RelayCommand selectionChangedCommand, out RelayCommand updateCommand, double inMinWidth = 0.0, IValueConverter inNameConverter = null)
            : base(text, tooltip, "", selectionChangedCommand)
        {
            ItemsSource = itemsSource;
            selectedItem = defaultSelection;

            nameConverter = (inNameConverter != null) ? inNameConverter : new DefaultNameConverter();
            updateCommand = new RelayCommand(OnUpdate);

            minWidth = inMinWidth;
        }

        private void OnUpdate(object newValues)
        {
            ItemsSource = newValues as IEnumerable;
            NotifyPropertyChanged("ItemsSource");

            var itemsList = ItemsSource as IList;
            if (itemsList != null)
            {
                SelectedItem = (itemsList.Count > 0) ? itemsList[0] : null;
            }
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class TextBoxToolbarItem : ToolbarItem, INotifyPropertyChanged
    {
        public string ValueText
        {
            get => valueText;
            set
            {
                if (valueText != value)
                {
                    valueText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string valueText;
        public TextBoxToolbarItem(string text, string tooltip, string inValueText, out RelayCommand updateCommand)
            : base(text, tooltip, "", new RelayCommand((o) => { }))
        {
            valueText = inValueText;
            updateCommand = new RelayCommand(OnUpdate);
        }

        private void OnUpdate(object newData)
        {
            ValueText = (string)newData;
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class DividerToolbarItem : ToolbarItem
    {
        public DividerToolbarItem()
            : base("", "", "", new RelayCommand((o) => { }))
        {
        }
    }

    #endregion

    #region -- Toolbar Item Template Selector --

    public class ToolbarItemTemplateSelector : DataTemplateSelector
    {
        private static List<DataTemplate> templates;

        public ToolbarItemTemplateSelector()
        {
        }

        public ToolbarItemTemplateSelector(List<DataTemplate> inTemplates)
        {
            if (templates != null)
                return;

            templates = inTemplates;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var foundTemplate = templates.Find(dt => (Type)dt.DataType == item.GetType());
            if (foundTemplate == null)
                return templates[0];
            return foundTemplate;
        }
    }

    #endregion

    public class ToolbarAssetEditor : FrostyBaseAssetEditor
    {
        public ToolbarAssetEditor(ILogger inLogger) : base(inLogger)
        {
        }
        
        public override EbxAsset GetDependentObject(Guid guid)
        {
            return LoadedAssetManager.Instance.GetEbxAsset(guid);
        }

        protected void PerformTemplateMagic()
        {
            Window win = App.EditorWindow as Window;

            Grid g = win.Content as Grid;
            //g = g.Children[0] as Grid;
            g = g.Children[1] as Grid;

            Border b = g.Children[0] as Border;
            DockPanel dp = b.Child as DockPanel;
            b = dp.Children[2] as Border;

            // Inject a template selector into the toolbar items control so that new
            // types of toolbar items can be defined

            ItemsControl ic = b.Child as ItemsControl;
            ic.ItemTemplate = null;
            ic.ItemTemplateSelector = new ToolbarItemTemplateSelector(
                new List<DataTemplate>()
                {
                    FindResource("DefaultToolbarItem") as DataTemplate,
                    FindResource("RegularToolbarItem") as DataTemplate,
                    FindResource("DockingToolbarItem") as DataTemplate,
                    FindResource("FloatingOnlyDockingToolbarItem") as DataTemplate,
                    FindResource("ToggleToolbarItem") as DataTemplate,
                    FindResource("ComboBoxToolbarItem") as DataTemplate,
                    FindResource("TextBoxToolbarItem") as DataTemplate,
                    FindResource("DividerToolbarItem") as DataTemplate
                });

            App.EditorWindow.DataExplorer.SetValue(DragDropExtension.IsDragDropDataExplorerProperty, true);
        }
    }
}
