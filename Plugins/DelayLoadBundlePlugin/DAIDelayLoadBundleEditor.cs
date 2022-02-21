using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Frosty.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using Frosty.Core;
using System.Windows.Media;

namespace DelayLoadBundlePlugin
{
    [TemplatePart(Name = PART_ListView, Type = typeof(ListView))]
    [TemplatePart(Name = PART_FilterTextBox, Type = typeof(FrostyWatermarkTextBox))]
    [TemplatePart(Name = PART_CopyMenuItem, Type = typeof(MenuItem))]
    public class DAIDelayLoadBundleEditor : FrostyBaseEditor
    {
        public override ImageSource Icon => DelayLoadBundleMenuExtension.imageSource;

        private const string PART_ListView = "PART_ListView";
        private const string PART_FilterTextBox = "PART_FilterTextBox";
        private const string PART_CopyMenuItem = "PART_CopyMenuItem";

        private ILogger logger;
        private ListView lv;
        private FrostyWatermarkTextBox filterTb;

        private Dictionary<uint, DelayLoadBundle> bundles = new Dictionary<uint, DelayLoadBundle>();

        static DAIDelayLoadBundleEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DAIDelayLoadBundleEditor), new FrameworkPropertyMetadata(typeof(DAIDelayLoadBundleEditor)));
        }

        public DAIDelayLoadBundleEditor(ILogger inLogger)
        {
            logger = inLogger;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            lv = GetTemplateChild(PART_ListView) as ListView;
            filterTb = GetTemplateChild(PART_FilterTextBox) as FrostyWatermarkTextBox;

            Loaded += DAIDelayLoadBundleEditor_Loaded;
            filterTb.LostFocus += FilterTb_LostFocus;
            filterTb.KeyUp += FilterTb_KeyUp;
            lv.MouseDoubleClick += Lv_MouseDoubleClick;

            int i = 0;
            foreach (GridViewColumn column in (lv.View as GridView).Columns)
            {
                GridViewColumnHeader header = column.Header as GridViewColumnHeader;
                header.Click += LvGridViewColumnHeader_Click;

                if (i == 0)
                {
                    lastSortHeader = header;
                    lastSortDirection = ListSortDirection.Ascending;
                    i++;
                }
            }

            MenuItem mi = GetTemplateChild(PART_CopyMenuItem) as MenuItem;
            mi.Click += CopyMenuItem_Click;

            RoutedCommand copyCmd = new RoutedCommand();
            copyCmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(copyCmd, CopyMenuItem_Click));
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DelayLoadBundle selectedItem = lv.SelectedItem as DelayLoadBundle;
            if (selectedItem == null)
                return;

            Clipboard.SetText(selectedItem.Hash.ToString());
        }

        GridViewColumnHeader lastSortHeader;
        ListSortDirection lastSortDirection;

        private void LvGridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            if (column.Column == null)
                return;

            string sortBy = column.Tag.ToString();
            ListSortDirection sortDir = ListSortDirection.Ascending;
            if (column == lastSortHeader)
                sortDir = 1 - lastSortDirection;

            lv.Items.SortDescriptions.Clear();
            lv.Items.SortDescriptions.Add(new SortDescription(sortBy, sortDir));

            if (lastSortHeader != null)
                lastSortHeader.Column.HeaderTemplate = FindResource("lvNoSorting") as DataTemplate;

            column.Column.HeaderTemplate = (sortDir == ListSortDirection.Ascending)
                ? FindResource("lvAscendingSorting") as DataTemplate
                : FindResource("lvDescendingSorting") as DataTemplate;

            lastSortHeader = column;
            lastSortDirection = sortDir;
        }

        private void Lv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DelayLoadBundle selectedItem = lv.SelectedItem as DelayLoadBundle;
            if (selectedItem == null)
                return;

            App.EditorWindow.DataExplorer.SelectAsset(App.AssetManager.GetEbxEntry(selectedItem.Name));
        }

        private void FilterTb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                filterTb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void FilterTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (filterTb.Text == "")
            {
                lv.Items.Filter = null;
                return;
            }

            if (uint.TryParse(filterTb.Text, out uint filterHash))
            {
                lv.Items.Filter = new Predicate<object>((object o) =>
                {
                    DelayLoadBundle dlb = o as DelayLoadBundle;
                    return dlb.Hash == filterHash;
                });
            }
        }

        private void DAIDelayLoadBundleEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (bundles.Count == 0)
            {
                FrostyTaskWindow.Show("Loading DelayLoadBundles", "", (task) =>
                {
                    foreach (ResAssetEntry entry in App.AssetManager.EnumerateRes(resType: (int)ResourceType.DelayLoadBundleResource))
                    {
                        DelayLoadBundleResource resource = App.AssetManager.GetResAs<DelayLoadBundleResource>(entry);
                        foreach (DelayLoadBundle bundle in resource.EnumerateBundles())
                        {
                            if (!bundles.ContainsKey(bundle.Hash))
                                bundles.Add(bundle.Hash, bundle);
                        }
                    }
                });
            }

            lv.ItemsSource = bundles.Values;
            lv.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Hash", System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
