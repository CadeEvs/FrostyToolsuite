using Frosty.Controls;
using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public interface IDockableItem
    {
        string Header { get; }
        string UniqueId { get; }
        string Icon { get; }
    }

    public enum DockLocation
    {
        None = -1,

        TopLeft = 0,
        BottomLeft = 1,
        TopRight = 2,
        BottomRight = 3,
        Bottom = 4,
        Floating = 5
    }

    public enum PanelLocation
    {
        Left = 0,
        Right = 1,
        Bottom = 2,
    }

    public static class ConfigExt
    {
        public static T GetJsonObject<T>(string option, T defaultValue, ConfigScope scope = ConfigScope.Global, string profile = null)
        {
            Newtonsoft.Json.Linq.JObject value = null;
            if (scope == ConfigScope.Game)
            {
                if (Config.Current.Games[profile ?? FrostySdk.ProfilesLibrary.ProfileName].Options[option] is T)
                    return (T)Config.Current.Games[profile ?? FrostySdk.ProfilesLibrary.ProfileName].Options[option];

                value = Config.Current.Games[profile ?? FrostySdk.ProfilesLibrary.ProfileName].Options[option] as Newtonsoft.Json.Linq.JObject;
            }
            else if (Config.Current.GlobalOptions.ContainsKey(option))
            {
                if (Config.Current.GlobalOptions[option] is T)
                    return (T)Config.Current.GlobalOptions[option];

                value = Config.Current.GlobalOptions[option] as Newtonsoft.Json.Linq.JObject;
            }

            if (value != null)
            {
                return value.ToObject<T>();
            }
            return defaultValue;
        }
    }
    public class DockManager
    {
        public event RoutedEventHandler ItemAdded;
        public event RoutedEventHandler ItemRemoved;

        private Dictionary<PanelLocation, DockPanel> dockPanels = new Dictionary<PanelLocation, DockPanel>();
        private Dictionary<string, DockLayoutData> dockedItemKeys = new Dictionary<string, DockLayoutData>();

        private List<string> visibleDockedItems = new List<string>();
        private List<FrostyDockableWindow> floatingDockedItems = new List<FrostyDockableWindow>();

        private Dictionary<string, DockLayoutFloatingData> floatingData = new Dictionary<string, DockLayoutFloatingData>();
        private Dictionary<string, Tuple<bool, bool>> initialData = new Dictionary<string, Tuple<bool, bool>>();

        private DockManagerConfigData config;
        private Control owner;

        public DockManager(Control inOwner)
        {
            owner = inOwner;
        }

        public void AddDockPanel(DockPanel panel)
        {
            System.Diagnostics.Debug.Assert(!dockPanels.ContainsKey(panel.DockLocation), "A DockPanel has already been added to this docking location!");
            dockPanels.Add(panel.DockLocation, panel);

            if (config.Panels != null)
            {
                DockPanelData panelData = config.Panels.FirstOrDefault(e => e.Location == panel.DockLocation);
                panel.Size = new GridLength(panelData.Size);
            }
        }

        public bool IsItemVisible(string uniqueId)
        {
            if (initialData.ContainsKey(uniqueId))
                return initialData[uniqueId].Item1;
            return visibleDockedItems.Contains(uniqueId);
        }

        public DockLocation GetItemsLastKnownDockLocation(string uniqueId)
        {
            if (!dockedItemKeys.ContainsKey(uniqueId))
                return DockLocation.None;
            return dockedItemKeys[uniqueId].Location;
        }

        public void AddItemOnLoad(IDockableItem itemToDock)
        {
            if (!dockedItemKeys.ContainsKey(itemToDock.UniqueId))
                return;

            if (initialData.ContainsKey(itemToDock.UniqueId))
            {
                if (initialData[itemToDock.UniqueId].Item1)
                {
                    AddItem(dockedItemKeys[itemToDock.UniqueId].Location, itemToDock, initialData[itemToDock.UniqueId].Item2);
                }
                initialData.Remove(itemToDock.UniqueId);
            }
        }

        public void AddItem(DockLocation location, IDockableItem itemToDock, bool isSelected = true)
        {
            if (visibleDockedItems.Contains(itemToDock.UniqueId))
                return;

            if (!dockedItemKeys.ContainsKey(itemToDock.UniqueId))
                dockedItemKeys.Add(itemToDock.UniqueId, new DockLayoutData() { UniqueId = itemToDock.UniqueId, Location = location });

            DockLayoutData layoutData = dockedItemKeys[itemToDock.UniqueId];
            layoutData.Location = location;

            if (location != DockLocation.Floating)
            {
                PanelLocation panelLocation = DockToPanelLocation(location);
                DockPanel panel = dockPanels[panelLocation];

                switch (location)
                {
                    case DockLocation.TopLeft: panel.AddTopItem(itemToDock, isSelected); break;
                    case DockLocation.TopRight: panel.AddTopItem(itemToDock, isSelected); break;
                    case DockLocation.BottomLeft: panel.AddBottomItem(itemToDock, isSelected); break;
                    case DockLocation.BottomRight: panel.AddBottomItem(itemToDock, isSelected); break;
                    case DockLocation.Bottom: panel.AddTopItem(itemToDock, isSelected); break;
                }
            }
            else
            {
                DockLayoutFloatingData floatingData = new DockLayoutFloatingData()
                {
                    Top = 0,
                    Left = 0,
                    Width = 400,
                    Height = 800
                };
                if (layoutData.FloatingData.HasValue)
                    floatingData = layoutData.FloatingData.Value;

                FrostyDockableWindow win = new FrostyDockableWindow();

                win.WindowParent = owner;
                win.DataContext = itemToDock;
                win.Content = itemToDock;
                win.ContentTemplate = owner.FindResource(new DataTemplateKey(itemToDock.GetType())) as DataTemplate;
                win.WindowStartupLocation = (floatingData.Top == 0 && floatingData.Left == 0) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.Manual;
                win.Owner = App.EditorWindow as Window;
                win.Width = floatingData.Width;
                win.Height = floatingData.Height;
                win.Title = itemToDock.Header;
                win.Icon = (new ImageSourceConverter().ConvertFromString("pack://application:,,,/LevelEditorPlugin;component/" + itemToDock.Icon)) as ImageSource;
                win.ShowActivated = true;
                win.Closing += (o, e) =>
                {
                    if (win.ShowActivated)
                    {
                        win.ShowActivated = false;
                        RemoveItem(itemToDock.UniqueId);
                    }
                };

                if (floatingData.Top != 0 || floatingData.Left != 0)
                {
                    win.Top = floatingData.Top;
                    win.Left = floatingData.Left;
                }

                win.Show();
                floatingDockedItems.Add(win);

                floatingData.Top = win.Top;
                floatingData.Left = win.Left;

                layoutData.FloatingData = floatingData;
            }

            dockedItemKeys[itemToDock.UniqueId] = layoutData;
            visibleDockedItems.Add(itemToDock.UniqueId);
            ItemAdded?.Invoke(itemToDock.UniqueId, new RoutedEventArgs());
        }

        public void RemoveItem(string uniqueId)
        {
            if (!visibleDockedItems.Contains(uniqueId))
                return;

            if (dockedItemKeys[uniqueId].Location == DockLocation.Floating)
            {
                FrostyDockableWindow win = floatingDockedItems.Find(w => (w.DataContext as IDockableItem).UniqueId == uniqueId);
                DockLayoutData layoutData = dockedItemKeys[uniqueId];

                layoutData.FloatingData = new DockLayoutFloatingData()
                {
                    Top = win.Top,
                    Left = win.Left,
                    Width = win.Width,
                    Height = win.Height
                };

                dockedItemKeys[uniqueId] = layoutData;
                floatingDockedItems.Remove(win);

                if (win.ShowActivated)
                {
                    win.ShowActivated = false;
                    win.Close();
                }
            }
            else
            {
                PanelLocation panelLocation = DockToPanelLocation(dockedItemKeys[uniqueId].Location);
                dockPanels[panelLocation].RemoveItem(uniqueId);
            }

            visibleDockedItems.Remove(uniqueId);
            ItemRemoved?.Invoke(uniqueId, new RoutedEventArgs());
        }

        public void Shutdown()
        {
            foreach (FrostyDockableWindow win in floatingDockedItems)
            {
                win.ShowActivated = false;
                win.Close();
            }
        }

        public void HideFloatingWindows()
        {
            foreach (FrostyDockableWindow win in floatingDockedItems)
            {
                win.Hide();
            }
        }

        public void ShowFloatingWindows()
        {
            foreach (FrostyDockableWindow win in floatingDockedItems)
            {
                win.Show();
            }
        }

        public struct DockLayoutFloatingData
        {
            public double Left;
            public double Top;
            public double Width;
            public double Height;
        }

        public struct DockLayoutData
        {
            public string UniqueId;
            public bool IsVisible;
            public bool IsSelected;
            public DockLocation Location;
            public DockLayoutFloatingData? FloatingData;
        }

        public struct DockPanelData
        {
            public PanelLocation Location;
            public double Size;
        }

        public struct DockManagerConfigData
        {
            public List<DockPanelData> Panels;
            public List<DockLayoutData> Layouts;
        }

        public void SaveToConfig(string editorName)
        {
            DockManagerConfigData configData = new DockManagerConfigData();

            configData.Panels = new List<DockPanelData>();
            foreach (KeyValuePair<PanelLocation, DockPanel> panelKvp in dockPanels)
            {
                double tmpSize = panelKvp.Value.Size.Value;
                if (!panelKvp.Value.Size.IsAbsolute)
                {
                    tmpSize = (panelKvp.Key == PanelLocation.Bottom) ? panelKvp.Value.ActualHeight : panelKvp.Value.ActualWidth;
                    if (tmpSize == 0 || double.IsInfinity(tmpSize) || double.IsNaN(tmpSize))
                        continue;
                }

                DockPanelData panelData = new DockPanelData();
                panelData.Location = panelKvp.Key;
                panelData.Size = tmpSize;

                configData.Panels.Add(panelData);
            }

            configData.Layouts = new List<DockLayoutData>();
            foreach (KeyValuePair<string, DockLayoutData> dockedItemKvp in dockedItemKeys)
            {
                DockLayoutData layoutData = dockedItemKvp.Value;
                layoutData.IsVisible = visibleDockedItems.Contains(dockedItemKvp.Key);

                if (layoutData.Location != DockLocation.Floating)
                {
                    PanelLocation panelLocation = DockToPanelLocation(layoutData.Location);
                    layoutData.IsSelected = dockPanels[panelLocation].IsItemSelected(dockedItemKvp.Key);
                }
                else
                {
                    FrostyDockableWindow win = floatingDockedItems.Find(w => (w.DataContext as IDockableItem).UniqueId == dockedItemKvp.Key);
                    if (win != null)
                    {
                        DockLayoutFloatingData floatingData = new DockLayoutFloatingData();
                        floatingData.Top = win.Top;
                        floatingData.Left = win.Left;
                        floatingData.Width = win.Width;
                        floatingData.Height = win.Height;

                        layoutData.FloatingData = floatingData;
                    }
                }

                configData.Layouts.Add(layoutData);
            }

            Config.Add($"{editorName}DockLayout", configData);
            Config.Save();
        }

        public void LoadFromConfig(string editorName, DockManagerConfigData defaultValue)
        {
            config = ConfigExt.GetJsonObject<DockManagerConfigData>($"{editorName}DockLayout", defaultValue);
            foreach (DockLayoutData layout in config.Layouts)
            {
                dockedItemKeys.Add(layout.UniqueId, layout);
                initialData.Add(layout.UniqueId, new Tuple<bool, bool>(layout.IsVisible, layout.IsSelected));
            }
        }

        private PanelLocation DockToPanelLocation(DockLocation dockLocation)
        {
            switch(dockLocation)
            {
                case DockLocation.TopLeft: return PanelLocation.Left;
                case DockLocation.BottomLeft: return PanelLocation.Left;
                case DockLocation.TopRight: return PanelLocation.Right;
                case DockLocation.BottomRight: return PanelLocation.Right;
                case DockLocation.Bottom: return PanelLocation.Bottom;
            }
            return PanelLocation.Left;
        }
    }

    [TemplatePart(Name = PART_TopTabControl, Type = typeof(FrostyTabControl))]
    [TemplatePart(Name = PART_TopTabContent, Type = typeof(FrostyDetachedTabControl))]
    [TemplatePart(Name = PART_BottomTabControl, Type = typeof(FrostyTabControl))]
    [TemplatePart(Name = PART_BottomTabContent, Type = typeof(FrostyDetachedTabControl))]
    public class DockPanel : Control
    {
        private const string PART_TopTabControl = "PART_TopTabControl";
        private const string PART_TopTabContent = "PART_TopTabContent";

        private const string PART_BottomTabControl = "PART_BottomTabControl";
        private const string PART_BottomTabContent = "PART_BottomTabContent";

        public static readonly DependencyProperty HasItemsProperty = DependencyProperty.Register("HasItems", typeof(bool), typeof(DockPanel), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty DockLocationProperty = DependencyProperty.Register("DockLocation", typeof(PanelLocation), typeof(DockPanel), new FrameworkPropertyMetadata(PanelLocation.Left));
        public static readonly DependencyProperty DockManagerProperty = DependencyProperty.Register("DockManager", typeof(DockManager), typeof(DockPanel), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SupportsBottomContentProperty = DependencyProperty.Register("SupportsBottomContent", typeof(bool), typeof(DockPanel), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(GridLength), typeof(DockPanel), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star)));

        public bool HasItems
        {
            get => (bool)GetValue(HasItemsProperty);
            set => SetValue(HasItemsProperty, value);
        }
        public PanelLocation DockLocation
        {
            get => (PanelLocation)GetValue(DockLocationProperty);
            set => SetValue(DockLocationProperty, value);
        }
        public DockManager DockManager
        {
            get => (DockManager)GetValue(DockManagerProperty);
            set => SetValue(DockManagerProperty, value);
        }
        public bool SupportsBottomContent
        {
            get => (bool)GetValue(SupportsBottomContentProperty);
            set => SetValue(SupportsBottomContentProperty, value);
        }
        public GridLength Size
        {
            get => (GridLength)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public ObservableCollection<FrostyTabItem> TopItems { get; private set; } = new ObservableCollection<FrostyTabItem>();
        public ObservableCollection<FrostyTabItem> BottomItems { get; private set; } = new ObservableCollection<FrostyTabItem>();

        static DockPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPanel), new FrameworkPropertyMetadata(typeof(DockPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            System.Diagnostics.Debug.Assert(DockManager != null, "DockManager needs to be bound!");
            DockManager.AddDockPanel(this);

            BindTabControlContent(PART_TopTabControl, PART_TopTabContent);
            if (SupportsBottomContent)
            {
                BindTabControlContent(PART_BottomTabControl, PART_BottomTabContent);
            }
        }

        public void AddTopItem(IDockableItem item, bool isSelected)
        {
            FrostyTabItem tabItem = new FrostyTabItem() { Content = item, Header = item.Header, TabId = item.UniqueId, IsSelected = isSelected, CloseButtonVisible = true, Icon = (new ImageSourceConverter().ConvertFromString("pack://application:,,,/LevelEditorPlugin;component/" + item.Icon)) as ImageSource };
            tabItem.CloseButtonClick += (o, e) =>
            {
                DockManager.RemoveItem(tabItem.TabId);
            };
            TopItems.Add(tabItem);
            HasItems = true;
        }

        public void AddBottomItem(IDockableItem item, bool isSelected)
        {
            System.Diagnostics.Debug.Assert(SupportsBottomContent, "This docking panel does not support content being added to this location.");

            FrostyTabItem tabItem = new FrostyTabItem() { Content = item, Header = item.Header, TabId = item.UniqueId, IsSelected = isSelected, CloseButtonVisible = true, Icon = (new ImageSourceConverter().ConvertFromString("pack://application:,,,/LevelEditorPlugin;component/" + item.Icon)) as ImageSource };
            tabItem.CloseButtonClick += (o, e) =>
            {
                DockManager.RemoveItem(tabItem.TabId);
            };
            BottomItems.Add(tabItem);
            HasItems = true;
        }

        public bool IsItemSelected(string uniqueId)
        {
            // Check top items
            FrostyTabItem tabItem = TopItems.ToArray().Where(ti => ti.TabId == uniqueId).FirstOrDefault();
            if (tabItem != null)
            {
                return tabItem.IsSelected;
            }

            if (SupportsBottomContent)
            {
                // Otherwise look in bottom items
                tabItem = BottomItems.ToArray().Where(ti => ti.TabId == uniqueId).FirstOrDefault();
                if (tabItem != null)
                {
                    return tabItem.IsSelected;
                }
            }

            return false;
        }

        public void RemoveItem(string uniqueId)
        {
            // Check top items
            FrostyTabItem tabItem = TopItems.ToArray().Where(ti => ti.TabId == uniqueId).FirstOrDefault();
            if (tabItem != null)
            {
                TopItems.Remove(tabItem);
                HasItems = TopItems.Count > 0 || BottomItems.Count > 0;
                return;
            }

            if (SupportsBottomContent)
            {
                // Otherwise look in bottom items
                tabItem = BottomItems.ToArray().Where(ti => ti.TabId == uniqueId).FirstOrDefault();
                if (tabItem != null)
                {
                    BottomItems.Remove(tabItem);
                    HasItems = TopItems.Count > 0 || BottomItems.Count > 0;
                    return;
                }
            }
        }

        private void BindTabControlContent(string controlName, string contentName)
        {
            FrostyTabControl tabControl = GetTemplateChild(controlName) as FrostyTabControl;
            FrostyDetachedTabControl tabContent = GetTemplateChild(contentName) as FrostyDetachedTabControl;
            tabContent.HeaderControl = tabControl;
        }
    }
}
