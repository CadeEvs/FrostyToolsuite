using FrostySdk.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Frosty.Core.Bookmarks;
using System.Linq;
using Frosty.Hash;
using Frosty.Core.Commands;
using System.Windows.Data;

namespace Frosty.Core.Controls
{
    public class PlainView : ViewBase
    {

        public static readonly DependencyProperty ItemContainerStyleProperty = ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(PlainView));
        public Style ItemContainerStyle
        {
            get => (Style)GetValue(ItemContainerStyleProperty);
            set => SetValue(ItemContainerStyleProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner(typeof(PlainView));
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemWidthProperty = WrapPanel.ItemWidthProperty.AddOwner(typeof(PlainView));
        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }


        public static readonly DependencyProperty ItemHeightProperty = WrapPanel.ItemHeightProperty.AddOwner(typeof(PlainView));
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }


        //protected override object DefaultStyleKey
        //{
        //    get
        //    {
        //        return new ComponentResourceKey(typeof(PlainView), "PlainViewDefaultStyle");
        //    }
        //}
    }

    internal class AssetPath
    {
        private static readonly ImageSource ClosedImage = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/CloseFolder.png") as ImageSource;
        private static readonly ImageSource OpenImage = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/OpenFolder.png") as ImageSource;

        public string DisplayName => PathName.Trim('!');
        public string PathName { get; private set; }
        public string FullPath { get; }
        public AssetPath Parent { get; }
        public List<AssetPath> Children { get; } = new List<AssetPath>();
        public bool IsSelected { get; set; }
        public bool IsRoot { get; }

        public bool IsExpanded 
        { 
            get => expanded && Children.Count != 0;
            set => expanded = value;
        }
        private bool expanded;

        public AssetPath(string inName, string path, AssetPath inParent, bool bInRoot = false)
        {
            PathName = inName;
            FullPath = path;
            IsRoot = bInRoot;
            Parent = inParent;
        }

        public void UpdatePathName(string newName)
        {
            PathName = newName;
        }
    }

    public class AssetDoubleClickedEventArgs : RoutedEventArgs
    {
        public AssetEntry SelectedAsset { get; private set; }

        public AssetDoubleClickedEventArgs(AssetEntry selectedAsset)
        {
            SelectedAsset = selectedAsset;
        }
    }

    [TemplatePart(Name = PART_ShowOnlyModifiedCheckBox, Type = typeof(CheckBox))]
    [TemplatePart(Name = PART_FilterTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_AssetTreeView, Type = typeof(TreeView))]
    [TemplatePart(Name = PART_AssetListView, Type = typeof(ListView))]
    public class FrostyDataExplorer : Control
    {
        public string FilteredText { get => m_filterTextBox.Text; }

        private const string PART_ShowOnlyModifiedCheckBox = "PART_ShowOnlyModifiedCheckBox";
        private const string PART_FilterTextBox = "PART_FilterTextBox";
        private const string PART_AssetTreeView = "PART_AssetTreeView";
        private const string PART_AssetListView = "PART_AssetListView";

        private enum FilterCommandType
        {
            Contains,
            StartsWith,
            EndsWith,
            RegEx,
            Type,
            Id,
            Hash
        }

        private enum FilterCombineType
        {
            Or,
            And,
        }

        private struct FilterData
        {
            public string Text;
            public FilterCommandType Command;
            public FilterCombineType Combine;
            public bool Not;
        }

        #region -- Properties --

        #region -- ItemsSource --
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(null, OnItemsSourceChanged));
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrostyDataExplorer ctrl = o as FrostyDataExplorer;
            ctrl.m_assetPathMapping.Clear();
            ctrl.SelectedAsset = null;
            ctrl.UpdateTreeView();
        }
        #endregion

        #region -- ShowOnlyModified --
        public static readonly DependencyProperty ShowOnlyModifiedProperty = DependencyProperty.Register(nameof(ShowOnlyModified), typeof(bool), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(false, OnShowOnlyModifiedChanged));
        public bool ShowOnlyModified
        {
            get => (bool)GetValue(ShowOnlyModifiedProperty);
            set => SetValue(ShowOnlyModifiedProperty, value);
        }
        private static void OnShowOnlyModifiedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrostyDataExplorer ctrl = o as FrostyDataExplorer;
            ctrl.UpdateTreeView();
        }
        #endregion

        #region -- MultiSelect --
        public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.Register(nameof(MultiSelect), typeof(bool), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(null));
        public bool MultiSelect
        {
            get => (bool)GetValue(MultiSelectProperty);
            set => SetValue(MultiSelectProperty, value);
        }
        #endregion

        #region -- SelectedAsset --
        public static readonly DependencyProperty SelectedAssetProperty = DependencyProperty.Register(nameof(SelectedAsset), typeof(AssetEntry), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(null));
        public AssetEntry SelectedAsset
        {
            get => (AssetEntry)GetValue(SelectedAssetProperty);
            set => SetValue(SelectedAssetProperty, value);
        }
        #endregion

        #region -- SelectedAssets --
        public static readonly DependencyProperty SelectedAssetsProperty = DependencyProperty.Register(nameof(SelectedAssets), typeof(IList<AssetEntry>), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(null));
        public IList<AssetEntry> SelectedAssets
        {
            get => (IList<AssetEntry>)GetValue(SelectedAssetsProperty);
            set => SetValue(SelectedAssetsProperty, value);
        }
        #endregion

        #region -- AssetContextMenu --
        public static readonly DependencyProperty AssetContextMenuProperty = DependencyProperty.Register(nameof(AssetContextMenu), typeof(ContextMenu), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(null));
        public ContextMenu AssetContextMenu
        {
            get => (ContextMenu)GetValue(AssetContextMenuProperty);
            set => SetValue(AssetContextMenuProperty, value);
        }
        #endregion

        #region -- ToolbarVisible --
        public static readonly DependencyProperty ToolbarVisibleProperty = DependencyProperty.Register(nameof(ToolbarVisible), typeof(bool), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(true));
        public bool ToolbarVisible
        {
            get => (bool)GetValue(ToolbarVisibleProperty);
            set => SetValue(ToolbarVisibleProperty, value);
        }
        #endregion

        #region -- AssetListVisible --
        public static readonly DependencyProperty AssetListVisibleProperty = DependencyProperty.Register(nameof(AssetListVisible), typeof(bool), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(true));
        public bool AssetListVisible
        {
            get => (bool)GetValue(AssetListVisibleProperty);
            set => SetValue(AssetListVisibleProperty, value);
        }
        #endregion

        #region -- BookmarkContext --
        public static readonly DependencyProperty BookmarkContextProperty = DependencyProperty.Register(nameof(BookmarkContext), typeof(string), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata("", OnBookmarkContextChanged));
        public string BookmarkContext
        {
            get => (string)GetValue(BookmarkContextProperty);
            set => SetValue(BookmarkContextProperty, value);
        }
        private static void OnBookmarkContextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrostyDataExplorer ctrl = o as FrostyDataExplorer;
            ctrl.m_bookmarkContext = BookmarkDb.GetContext(e.NewValue as string);
        }
        #endregion

        #region -- InitialHeight --
        public static readonly DependencyProperty InitialHeightProperty = DependencyProperty.Register(nameof(InitialHeight), typeof(GridLength), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star)));
        public GridLength InitialHeight
        {
            get => (GridLength)GetValue(InitialHeightProperty);
            set => SetValue(InitialHeightProperty, value);
        }
        #endregion

        #region -- SelectedPath --
        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register(nameof(SelectedPath), typeof(string), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(""));
        public string SelectedPath
        {
            get => (string)GetValue(SelectedPathProperty);
            set => SetValue(SelectedPathProperty, value);
        }
        #endregion

        #region -- TileTemplate --
        public static readonly DependencyProperty TileTemplateProperty = DependencyProperty.Register(nameof(TileTemplate), typeof(DataTemplate), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(null));
        public DataTemplate TileTemplate
        {
            get => (DataTemplate)GetValue(TileTemplateProperty);
            set => SetValue(TileTemplateProperty, value);
        }
        #endregion

        #region -- TileZoom --
        public static readonly DependencyProperty TileZoomProperty = DependencyProperty.Register(nameof(TileZoom), typeof(double), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(50.0));
        public double TileZoom
        {
            get => (double)GetValue(TileZoomProperty);
            set => SetValue(TileZoomProperty, value);
        }
        #endregion

        #region -- GridView --
        public static readonly DependencyProperty GridViewProperty = DependencyProperty.Register(nameof(GridView), typeof(bool), typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(true, OnGridViewChanged));
        public bool GridView
        {
            get => (bool)GetValue(GridViewProperty);
            set => SetValue(GridViewProperty, value);
        }
        private static void OnGridViewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrostyDataExplorer ctrl = o as FrostyDataExplorer;
            ctrl.UpdateViewType();
        }
        #endregion

        #endregion

        #region -- Commands --

        #region Double Clicked
        public static readonly DependencyProperty OnDoubleClickedCommandProperty = DependencyProperty.Register(nameof(OnDoubleClickedCommand), typeof(ICommand), typeof(FrostyDataExplorer), new UIPropertyMetadata(null));

        public ICommand OnDoubleClickedCommand
        {
            get => (ICommand)GetValue(OnDoubleClickedCommandProperty);
            set => SetValue(OnDoubleClickedCommandProperty, value);
        }

        public ItemDoubleClickCommand DoubleClickCommand => new ItemDoubleClickCommand(DoubleClickSelectedAsset);
        #endregion

        #region OnFindOpenedAsset

        public ICommand FindOpenedAssetCommand => new RelayCommand(FindOpenedAsset);

        #endregion

        #endregion
        
        public event EventHandler<RoutedEventArgs> SelectedAssetDoubleClick;
        public event EventHandler<RoutedEventArgs> SelectionChanged;
        
        private TextBox m_filterTextBox;
        private TreeView m_assetTreeView;
        private ListView m_assetListView;

        private GridViewColumnHeader m_lastSortHeader;
        private ListSortDirection m_lastSortDirection;

        private readonly Dictionary<string, AssetPath> m_assetPathMapping = new Dictionary<string, AssetPath>(StringComparer.OrdinalIgnoreCase);

        private AssetPath m_selectedPath;
        
        private readonly List<FilterData> m_filter = new List<FilterData>();
        private string m_prevFilterText = "";

        private BookmarkContext m_bookmarkContext;

        private GridView m_detailView;
        private PlainView m_tileView;

        static FrostyDataExplorer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyDataExplorer), new FrameworkPropertyMetadata(typeof(FrostyDataExplorer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_filterTextBox = GetTemplateChild(PART_FilterTextBox) as TextBox;
            m_assetTreeView = GetTemplateChild(PART_AssetTreeView) as TreeView;
            m_assetListView = GetTemplateChild(PART_AssetListView) as ListView;

            m_assetTreeView.SelectedItemChanged += assetTreeView_SelectedItemChanged;
            m_filterTextBox.KeyUp += FilterTextBox_KeyUp;
            m_filterTextBox.LostFocus += FilterTextBox_LostFocus;
            m_assetListView.SelectionChanged += assetListView_SelectionChanged;
            m_assetListView.PreviewMouseWheel += AssetList_MouseWheel;
            IsVisibleChanged += FrostyDataExplorer_IsVisibleChanged;

            m_detailView = new GridView();
            m_tileView = new PlainView {ItemTemplate = TileTemplate};
            UpdateViewType();

            m_detailView.Columns.Add(new GridViewColumn() { Header = new GridViewColumnHeader() { Content = "Name", Tag = "DisplayName" }, CellTemplate = FindResource("DisplayNameCellTemplate") as DataTemplate });
            m_detailView.Columns.Add(new GridViewColumn() { Header = new GridViewColumnHeader() { Content = "Type", Tag = "Type" }, CellTemplate = FindResource("TypeCellTemplate") as DataTemplate });

            Binding b = new Binding("TileZoom") {Source = this};

            BindingOperations.SetBinding(m_tileView, PlainView.ItemWidthProperty, b);
            BindingOperations.SetBinding(m_tileView, PlainView.ItemHeightProperty, b);

            int i = 0;
            foreach (GridViewColumn column in (m_assetListView.View as GridView).Columns)
            {
                b = new Binding("ActualWidth") { ElementName = "gridHelper" + (i + 1) };
                BindingOperations.SetBinding(column, GridViewColumn.WidthProperty, b);

                GridViewColumnHeader header = column.Header as GridViewColumnHeader;
                header.Click += assetListViewColumn_Click;

                if (i == 0)
                {
                    column.HeaderTemplate = FindResource("assetListViewAscendingSorting") as DataTemplate;
                    m_lastSortHeader = header;
                    m_lastSortDirection = ListSortDirection.Ascending;
                    i++;
                }
            }

            // default listView sort to name
            m_assetListView.Items.SortDescriptions.Add(new SortDescription("DisplayName", m_lastSortDirection));
            if (MultiSelect)
                m_assetListView.SelectionMode = SelectionMode.Extended;

            UpdateTreeView();

            if (IsVisible && m_bookmarkContext != null)
            {
                BookmarkDb.CurrentContext = m_bookmarkContext;
            }
        }

        public void FindOpenedAsset(object obj)
        {
            SelectAsset(App.EditorWindow.GetOpenedAssetEntry());
        }
        
        private void UpdateViewType()
        {
            if (GridView)
            {
                m_assetListView.ItemContainerStyle = FindResource("DetailViewItemContainerStyle") as Style;
                m_assetListView.View = m_detailView;
                m_assetListView.Style = FindResource(new ComponentResourceKey(typeof(FrostyPropertyGrid), "DetailViewDefaultStyle")) as Style;
            }
            else
            {
                m_assetListView.ItemContainerStyle = FindResource("TileViewItemContainerStyle") as Style;
                m_assetListView.View = m_tileView;
                m_assetListView.Style = FindResource(new ComponentResourceKey(typeof(FrostyPropertyGrid), "TileViewDefaultStyle")) as Style;
            }
        }

        private void FrostyDataExplorer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible && m_bookmarkContext != null)
            {
                BookmarkDb.CurrentContext = m_bookmarkContext;
            }
        }

        public void SelectAsset(AssetEntry entry)
        {
            if (m_assetTreeView == null)
            {
                SelectedAsset = entry;
                return;
            }

            if (entry == null)
            {
                SelectedAsset = null;
                m_assetListView.SelectedItem = null;
                return;
            }

            SetValue(ShowOnlyModifiedProperty, false);
            ClearFilter();

            AssetPath selectedPath = m_assetPathMapping["/" + entry.Path];
            if (selectedPath.FullPath != "")
            {
                string[] tmp = selectedPath.FullPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string totalPath = "";
                TreeViewItem tvi = null;

                foreach (string tmpStr in tmp)
                {
                    totalPath += "/" + tmpStr;
                    AssetPath path = m_assetPathMapping[totalPath];

                    if (tvi != null)
                    {
                        tvi.IsExpanded = true;
                        tvi.IsSelected = true;
                        tvi.Items.Refresh();
                        tvi.UpdateLayout();
                        tvi.BringIntoView();
                    }

                    tvi = (tvi == null)
                        ? (TreeViewItem)m_assetTreeView.ItemContainerGenerator.ContainerFromItem(path)
                        : (TreeViewItem)tvi.ItemContainerGenerator.ContainerFromItem(path);
                }
                if (tvi != null)
                {
                    tvi.BringIntoView();
                    tvi.IsSelected = true;
                }
            }
            else
            {
                TreeViewItem tvi = m_assetTreeView.ItemContainerGenerator.ContainerFromItem(selectedPath) as TreeViewItem;
                if(tvi != null)
                {
                    tvi.BringIntoView();
                    tvi.IsSelected = true;
                }
            }

            selectedPath.IsSelected = true;
            m_assetListView.SelectedItem = entry;
            m_assetListView.ScrollIntoView(entry);
            SelectedAsset = entry;
        }

        public void DoubleClickSelectedAsset()
        {
            if (SelectedAsset == null)
                return;

            OnDoubleClickedCommand?.Execute(new AssetDoubleClickedEventArgs(SelectedAsset));
            SelectedAssetDoubleClick?.Invoke(this, new RoutedEventArgs());
        }

        public void RefreshItems()
        {
            m_assetListView?.Items.Refresh();

        }

        public void RefreshAll()
        {
            UpdateTreeView();
        }

        public void FocusFilter()
        {
            m_filterTextBox.Focus();
        }

        private void assetTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            m_selectedPath = m_assetTreeView.SelectedItem as AssetPath;
            SelectedPath = string.IsNullOrEmpty(m_selectedPath.FullPath) ? "" : m_selectedPath.FullPath.Remove(0, 1);

            UpdateListView(m_selectedPath);
        }

        private void ClearFilter()
        {
            if (m_prevFilterText != "")
            {
                m_filterTextBox.Text = "";
                m_prevFilterText = m_filterTextBox.Text;
                BuildFilterData(m_filterTextBox.Text);
                UpdateTreeView();
            }
        }

        private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(m_filterTextBox.Text != m_prevFilterText)
            {
                m_prevFilterText = m_filterTextBox.Text;
                BuildFilterData(m_filterTextBox.Text);
                UpdateTreeView();
            }
        }

        private void FilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                m_filterTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void assetListViewColumn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is GridViewColumnHeader column))
                return;

            string sortBy = column.Tag.ToString();
            ListSortDirection sortDir = ListSortDirection.Ascending;
            if (column == m_lastSortHeader)
                sortDir = 1 - m_lastSortDirection;

            m_assetListView.Items.SortDescriptions.Clear();
            m_assetListView.Items.SortDescriptions.Add(new SortDescription(sortBy, sortDir));

            if (m_lastSortHeader != null)
                m_lastSortHeader.Column.HeaderTemplate = FindResource("assetListViewNoSorting") as DataTemplate;

            column.Column.HeaderTemplate = (sortDir == ListSortDirection.Ascending)
                ? FindResource("assetListViewAscendingSorting") as DataTemplate
                : FindResource("assetListViewDescendingSorting") as DataTemplate;

            m_lastSortHeader = column;
            m_lastSortDirection = sortDir;
        }

        private void UpdateTreeView()
        {
            if (m_assetTreeView == null)
                return;

            if (m_selectedPath != null)
                m_selectedPath.IsSelected = false;

            if (ItemsSource == null)
                return;

            AssetPath root = new AssetPath("", "", null);
            foreach (AssetEntry entry in ItemsSource)
            {
                if (ShowOnlyModified && !entry.IsModified)
                    continue;

                if (!FilterText(entry.Name, entry))
                    continue;

                string[] arr = entry.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                AssetPath next = root;

                foreach (string path in arr)
                {
                    bool bFound = false;
                    foreach (AssetPath child in next.Children)
                    {
                        if (child.PathName.Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            if (path.ToCharArray().Any(char.IsUpper))
                                child.UpdatePathName(path);

                            next = child;
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        string fullPath = next.FullPath + "/" + path;
                        AssetPath newPath = null;

                        if (!m_assetPathMapping.ContainsKey(fullPath))
                        {
                            newPath = new AssetPath(path, fullPath, next);
                            m_assetPathMapping.Add(fullPath, newPath);
                        }
                        else
                        {
                            newPath = m_assetPathMapping[fullPath];
                            newPath.Children.Clear();

                            if (newPath == m_selectedPath)
                                m_selectedPath.IsSelected = true;
                        }

                        next.Children.Add(newPath);
                        next = newPath;
                    }
                }
            }

            if(!m_assetPathMapping.ContainsKey("/"))
                m_assetPathMapping.Add("/", new AssetPath("![root]", "", null, true));           
            root.Children.Insert(0, m_assetPathMapping["/"]);

            m_assetTreeView.ItemsSource = root.Children;
            m_assetTreeView.Items.SortDescriptions.Add(new SortDescription("PathName", ListSortDirection.Ascending));

            UpdateListView(m_selectedPath);
        }

        private void UpdateListView(AssetPath path = null)
        {
            if (path == null)
            {
                m_assetListView.ItemsSource = null;
                return;
            }

            List<AssetEntry> items = new List<AssetEntry>();
            string fullPath = path.FullPath.Trim('/');

            foreach (AssetEntry entry in ItemsSource)
            {
                if (ShowOnlyModified && !entry.IsModified)
                    continue;
                if (entry.Path.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    if (!FilterText(entry.Name, entry))
                        continue;
                    items.Add(entry);
                }
            }

            m_assetListView.ItemsSource = items;

            if (SelectedAsset != null)
            {
                m_assetListView.SelectedItem = SelectedAsset;
            }
        }

        private void assetListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedItem = null;
            if (MultiSelect)
            {
                List<AssetEntry> selectedItems = new List<AssetEntry>();
                foreach (AssetEntry entry in m_assetListView.SelectedItems)
                    selectedItems.Add(entry);

                if (selectedItems.Count > 0)
                {
                    selectedItem = selectedItems[0];
                    SetValue(SelectedAssetsProperty, selectedItems);
                }
            }
            else
            {
                selectedItem = m_assetListView.SelectedItem;
            }

            if (m_bookmarkContext != null)
            {
                // True: Only switch contexts when there is something to bookmark.
                // False: This context doesn't have anything to bookmark, but the active one still might.
                m_bookmarkContext.AvailableTarget = selectedItem != null ? new AssetBookmarkTarget(m_assetListView.SelectedItem as AssetEntry) : null;
            }

            SelectedAsset = selectedItem as AssetEntry;
            SelectionChanged?.Invoke(this, new RoutedEventArgs());
        }

        private void AssetList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                double newZoom = TileZoom + (e.Delta / 10.0);
                if (TileZoom == 50.0 && newZoom > TileZoom && GridView)
                {
                    GridView = false;
                    newZoom = 50.0;
                }
                else if (newZoom < 50.0)
                {
                    if (newZoom < TileZoom && !GridView)
                        GridView = true;

                    newZoom = 50.0;
                }
                else if (newZoom > 162.0)
                    newZoom = 162.0;
                TileZoom = newZoom;

                e.Handled = true;
            }
        }

        private bool FilterText(string inText, AssetEntry inEntry)
        {
            string type = inEntry.Type ?? "";

            if (m_filter.Count == 0)
                return true;

            bool retCode = false;
            foreach (FilterData filterData in m_filter)
            {
                bool nextRetCode = false;
                switch (filterData.Command)
                {
                    case FilterCommandType.Contains: nextRetCode = inText.IndexOf(filterData.Text, StringComparison.OrdinalIgnoreCase) >= 0; break;
                    case FilterCommandType.StartsWith: nextRetCode = inText.StartsWith(filterData.Text, StringComparison.OrdinalIgnoreCase); break;
                    case FilterCommandType.EndsWith: nextRetCode = inText.EndsWith(filterData.Text, StringComparison.OrdinalIgnoreCase); break;
                    case FilterCommandType.RegEx: nextRetCode = System.Text.RegularExpressions.Regex.IsMatch(inText, filterData.Text); break;
                    case FilterCommandType.Type: nextRetCode = type.Equals(filterData.Text, StringComparison.OrdinalIgnoreCase); break;
                    case FilterCommandType.Id:
                        if (inEntry is EbxAssetEntry entry)
                        {
                            nextRetCode = entry.Guid.Equals(new Guid(filterData.Text));
                        }
                        else
                        {
                            if (ulong.TryParse(filterData.Text, System.Globalization.NumberStyles.HexNumber, null, out ulong resRid))
                                nextRetCode = (inEntry as ResAssetEntry).ResRid == resRid;

                            if (nextRetCode == true)
                            {
                            }
                        }
                        break;
                    case FilterCommandType.Hash:
                        {
                            if (int.TryParse(filterData.Text, System.Globalization.NumberStyles.HexNumber, null, out int hash))
                            {
                                if (inEntry is EbxAssetEntry || inEntry is ResAssetEntry)
                                {
                                    nextRetCode = Fnv1.HashString(inEntry.Name.ToLower()) == hash;
                                }
                            }
                        }
                        break;
                }

                if (filterData.Not)
                    nextRetCode = !nextRetCode;

                switch (filterData.Combine)
                {
                    case FilterCombineType.And: retCode &= nextRetCode; break;
                    case FilterCombineType.Or: retCode |= nextRetCode; break;
                }
            }

            return retCode;
        }

        private void BuildFilterData(string filterText)
        {
            m_filter.Clear();
            if (filterText != "")
            {
                string[] subStr = filterText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (subStr.Length == 1 && !subStr[0].Contains(":"))
                {
                    m_filter.Add(new FilterData()
                    {
                        Text = subStr[0],
                        Command = FilterCommandType.Contains,
                        Combine = FilterCombineType.Or,
                        Not = false
                    });
                    return;
                }

                try
                {
                    for (int i = 0; i < subStr.Length; i++)
                    {
                        FilterCommandType command = FilterCommandType.Contains;
                        FilterCombineType combine = FilterCombineType.Or;
                        bool not = false;

                        if (m_filter.Count != 0)
                            combine = (subStr[i++] == "AND") ? FilterCombineType.And : FilterCombineType.Or;

                        if (subStr[i] == "NOT")
                        {
                            not = true;
                            i++;
                        }

                        string[] cmdArr = subStr[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string cmdString = subStr[i];
                        string remaining = "";

                        if (cmdArr.Length > 1)
                        {
                            cmdString = cmdArr[0];
                            remaining = cmdArr[1];
                        }
                        else
                            remaining = subStr[++i];

                        foreach (string value in Enum.GetNames(typeof(FilterCommandType)))
                        {
                            if (cmdString.StartsWith(value.ToLower()))
                            {
                                command = (FilterCommandType)Enum.Parse(typeof(FilterCommandType), value);
                                break;
                            }
                        }

                        m_filter.Add(new FilterData()
                        {
                            Text = remaining,
                            Command = command,
                            Combine = combine,
                            Not = not
                        });
                    }
                }
                catch(Exception)
                {
                    m_filterTextBox.Text = "";
                    m_filter.Clear();
                }
            }
        }
    }
}
