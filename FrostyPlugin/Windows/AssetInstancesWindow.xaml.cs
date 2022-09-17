using Frosty.Controls;
using Frosty.Core.Commands;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Windows
{
    class AssetInstanceInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Guid { get; set; }
        public object Data { get; private set; }

        public AssetInstanceInfo(dynamic inData)
        {
            Type type = inData.GetType();
            Id = inData.__Id;
            Type = type.Name;
            Guid = inData.GetInstanceGuid().ToString();
            Data = inData;
        }
    }

    /// <summary>
    /// Interaction logic for AssetInstancesWindow.xaml
    /// </summary>
    public partial class AssetInstancesWindow : FrostyDockableWindow
    {
        public object SelectedItem { get; private set; }
        public ItemDoubleClickCommand DoubleClickCommand { get; private set; }
        public List<object> NewlyCreatedObjects { get; private set; } = new List<object>();
        public List<object> DeletedObjects { get; private set; } = new List<object>();
        public bool Modified { get; private set; } = false;

        private GridViewColumnHeader lastSortHeader;
        private ListSortDirection lastSortDirection;
        private string filterText = "";
        private ObservableCollection<AssetInstanceInfo> instances;
        private object rootObject;
        private EbxAsset asset;
        private EbxAssetEntry entry;

        public AssetInstancesWindow(IEnumerable objects, object inSelected, EbxAsset inAsset, EbxAssetEntry inEntry)
        {
            InitializeComponent();
            DoubleClickCommand = new ItemDoubleClickCommand(DoubleClickSelectedAsset);

            instances = new ObservableCollection<AssetInstanceInfo>();
            AssetInstanceInfo selectedInfo = null;

            foreach (object obj in objects)
            {
                AssetInstanceInfo info = new AssetInstanceInfo(obj);
                instances.Add(info);

                if (info.Data == inSelected)
                    selectedInfo = info;
            }

            instancesListView.ItemsSource = instances;
            instancesListView.Items.Filter = InstanceFilter;
            instancesListView.SelectedItem = selectedInfo;
            instancesListView.ScrollIntoView(selectedInfo);

            Owner = Application.Current.MainWindow;

            foreach (GridViewColumn column in (instancesListView.View as GridView).Columns)
            {
                GridViewColumnHeader header = column.Header as GridViewColumnHeader;
                header.Click += instancesListView_Click;
            }

            asset = inAsset;
            entry = inEntry;
            rootObject = asset.RootObject;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (instancesListView.SelectedItem is AssetInstanceInfo info)
                SelectedItem = info.Data;
            DialogResult = true;
            Close();
        }

        private void instancesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectButton.IsEnabled = instancesListView.SelectedItem != null;
            deleteInstanceButton.IsEnabled = instancesListView.SelectedItem != null;
            renameInstanceButton.IsEnabled = instancesListView.SelectedItem != null;
            duplicateInstanceButton.IsEnabled = instancesListView.SelectedItem != null;
        }

        private void instancesListView_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            if (column.Column == null)
                return;

            string sortBy = column.Tag.ToString();
            ListSortDirection sortDir = ListSortDirection.Ascending;
            if (column == lastSortHeader)
                sortDir = 1 - lastSortDirection;

            instancesListView.Items.SortDescriptions.Clear();
            instancesListView.Items.SortDescriptions.Add(new SortDescription(sortBy, sortDir));
            
            if (lastSortHeader != null)
                lastSortHeader.Column.HeaderTemplate = FindResource("assetListViewNoSorting") as DataTemplate;

            column.Column.HeaderTemplate = (sortDir == ListSortDirection.Ascending)
                ? FindResource("assetListViewAscendingSorting") as DataTemplate
                : FindResource("assetListViewDescendingSorting") as DataTemplate;

            lastSortHeader = column;
            lastSortDirection = sortDir;
        }

        private void DoubleClickSelectedAsset()
        {
            if (instancesListView.SelectedItem is AssetInstanceInfo info)
                SelectedItem = info.Data;
            DialogResult = true;
            Close();
        }

        private void filterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (filterText != filterTextBox.Text.ToLower())
            {
                filterText = filterTextBox.Text.ToLower();
                CollectionViewSource.GetDefaultView(instancesListView.ItemsSource).Refresh();
            }
        }

        private void filterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                filterTextBox_LostFocus(this, new RoutedEventArgs());
        }

        private bool InstanceFilter(object item)
        {
            if (string.IsNullOrEmpty(filterText))
                return true;

            string[] arr = filterText.Split(':');
            if (arr[0].Equals("guid"))
                return ((item as AssetInstanceInfo).Guid.IndexOf(arr[1].Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            else if (arr[0].Equals("type"))
                return ((item as AssetInstanceInfo).Type.IndexOf(arr[1].Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            else
                return ((item as AssetInstanceInfo).Id.IndexOf(arr[0].Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void CreateInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            ClassSelector win = new ClassSelector(TypeLibrary.GetTypes("Asset"), allowAssets: true);
            if (win.ShowDialog() == true)
            {
                Type selectedType = win.SelectedClass;
                if (selectedType != null)
                {
                    dynamic obj = TypeLibrary.CreateObject(selectedType.Name);

                    NewlyCreatedObjects.Add(obj);
                    instances.Add(new AssetInstanceInfo(obj));
                    Modified = true;
                }
            }
        }

        private void DeleteInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            AssetInstanceInfo selectedObj = instancesListView.SelectedItem as AssetInstanceInfo;
            if (selectedObj.Data == rootObject)
            {
                FrostyMessageBox.Show("Cannot remove the primary instance", "Frosty Editor");
                return;
            }

            instances.Remove(selectedObj);

            foreach (object obj in NewlyCreatedObjects)
            {
                if (selectedObj.Data == obj)
                {
                    NewlyCreatedObjects.Remove(obj);
                    return;
                }
            }

            DeletedObjects.Add(selectedObj.Data);
            Modified = true;
        }

        private void RenameInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            AssetInstanceInfo selectedObj = instancesListView.SelectedItem as AssetInstanceInfo;
            dynamic obj = selectedObj.Data;

            RenameInstanceWindow win = new RenameInstanceWindow(obj.__Id);
            if (win.ShowDialog() == true)
            {
                obj.__Id = win.InstanceName;
                selectedObj.Id = win.InstanceName;
                instancesListView.Items.Refresh();

                Modified = true;
            }
        }

        private void DuplicateInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            AssetInstanceInfo selectedObj = instancesListView.SelectedItem as AssetInstanceInfo;
            dynamic obj = selectedObj.Data;

            FrostyTaskWindow.Show(this, "Duplicating instance", "This may take a while for large assets", (FrostyTaskWindow owner) =>
            {
                FrostyClipboard.Current.SetData(obj);
                obj = FrostyClipboard.Current.GetData(asset, entry);
            });

            NewlyCreatedObjects.Add(obj);
            instances.Add(new AssetInstanceInfo(obj));
            Modified = true;
        }
    }
}
