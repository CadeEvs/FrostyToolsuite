using Frosty.Core.Commands;
using FrostySdk.Managers;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Controls
{
    [TemplatePart(Name = PART_AssetListView, Type = typeof(ListView))]
    public class FrostyAssetListView : Control
    {
        private const string PART_AssetListView = "PART_AssetListView";

        #region -- ItemsSource --
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FrostyAssetListView), new FrameworkPropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        #region -- AssetContextMenu --
        public static readonly DependencyProperty AssetContextMenuProperty = DependencyProperty.Register("AssetContextMenu", typeof(ContextMenu), typeof(FrostyAssetListView), new FrameworkPropertyMetadata(null));
        public ContextMenu AssetContextMenu
        {
            get => (ContextMenu)GetValue(AssetContextMenuProperty);
            set => SetValue(AssetContextMenuProperty, value);
        }
        #endregion

        public AssetEntry SelectedItem => assetListView.SelectedItem as AssetEntry;
        public event EventHandler<RoutedEventArgs> SelectedAssetDoubleClick;
        public ItemDoubleClickCommand DoubleClickCommand { get; private set; }

        private ListView assetListView;
        private GridViewColumnHeader lastSortHeader;
        private ListSortDirection lastSortDirection;

        static FrostyAssetListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyAssetListView), new FrameworkPropertyMetadata(typeof(FrostyAssetListView)));
        }

        public FrostyAssetListView()
        {
            DoubleClickCommand = new ItemDoubleClickCommand(DoubleClickSelectedAsset);
        }

        public void DoubleClickSelectedAsset()
        {
            if (assetListView.SelectedItem == null)
                return;
            SelectedAssetDoubleClick?.Invoke(this, new RoutedEventArgs());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            assetListView = GetTemplateChild(PART_AssetListView) as ListView;

            // setup sorting
            int i = 0;
            foreach (GridViewColumn column in (assetListView.View as GridView).Columns)
            {
                GridViewColumnHeader header = column.Header as GridViewColumnHeader;
                header.Click += assetListViewColumn_Click;

                if (i == 0)
                {
                    lastSortHeader = header;
                    lastSortDirection = ListSortDirection.Ascending;
                    i++;
                }
            }

            // default sort on display name
            assetListView.Items.SortDescriptions.Add(new SortDescription("DisplayName", lastSortDirection));
        }

        private void assetListViewColumn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is GridViewColumnHeader column))
                return;

            string sortBy = column.Tag.ToString();
            ListSortDirection sortDir = ListSortDirection.Ascending;
            if (column == lastSortHeader)
                sortDir = 1 - lastSortDirection;

            assetListView.Items.SortDescriptions.Clear();
            assetListView.Items.SortDescriptions.Add(new SortDescription(sortBy, sortDir));

            if (lastSortHeader != null)
                lastSortHeader.Column.HeaderTemplate = FindResource("assetListViewNoSorting") as DataTemplate;

            column.Column.HeaderTemplate = (sortDir == ListSortDirection.Ascending)
                ? FindResource("assetListViewAscendingSorting") as DataTemplate
                : FindResource("assetListViewDescendingSorting") as DataTemplate;

            lastSortHeader = column;
            lastSortDirection = sortDir;
        }



        static object GetObjectAtPoint<ItemContainer>(ItemsControl control, Point p)
        where ItemContainer : DependencyObject
        {
            // ItemContainer - can be ListViewItem, or TreeViewItem and so on(depends on control)
            ItemContainer obj = GetContainerAtPoint<ItemContainer>(control, p);
            return obj == null ? null : control.ItemContainerGenerator.ItemFromContainer(obj);
        }

        static ItemContainer GetContainerAtPoint<ItemContainer>(ItemsControl control, Point p)
        where ItemContainer : DependencyObject
        {
            HitTestResult result = VisualTreeHelper.HitTest(control, p);
            DependencyObject obj = result.VisualHit;

            while (VisualTreeHelper.GetParent(obj) != null && !(obj is ItemContainer))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            // Will return null if not found
            return obj as ItemContainer;
        }


    }
}
