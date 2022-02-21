using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReferencesPlugin
{
    [TemplatePart(Name = PART_RefExplorerToTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_RefExplorerFromTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_RefExplorerToListView, Type = typeof(FrostyAssetListView))]
    [TemplatePart(Name = PART_RefExplorerFromListView, Type = typeof(FrostyAssetListView))]
    public class ReferenceTabItem : FrostyTabItem
    {
        private const string PART_RefExplorerToTextBlock = "PART_RefExplorerToTextBlock";
        private const string PART_RefExplorerFromTextBlock = "PART_RefExplorerFromTextBlock";
        private const string PART_RefExplorerToListView = "PART_RefExplorerToListView";
        private const string PART_RefExplorerFromListView = "PART_RefExplorerFromListView";
        private const string PART_RefExplorerToOpenItem = "PART_RefExplorerToOpenItem";
        private const string PART_RefExplorerFromOpenItem = "PART_RefExplorerFromOpenItem";

        private const string PART_RefExplorerToFindItem = "PART_RefExplorerToFindItem";
        private const string PART_RefExplorerFromFindItem = "PART_RefExplorerFromFindItem";

        private FrostyAssetListView refExplorerToList;
        private FrostyAssetListView refExplorerFromList;
        private TextBlock refExplorerToText;
        private TextBlock refExplorerFromText;
        private MenuItem refExplorerToOpenItem;
        private MenuItem refExplorerFromOpenItem;

        private MenuItem refExplorerToFindItem;
        private MenuItem refExplorerFromFindItem;

        static ReferenceTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReferenceTabItem), new FrameworkPropertyMetadata(typeof(ReferenceTabItem)));
        }

        public ReferenceTabItem()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Bind References Asset List View Elements
            refExplorerToList = GetTemplateChild(PART_RefExplorerToListView) as FrostyAssetListView;
            refExplorerFromList = GetTemplateChild(PART_RefExplorerFromListView) as FrostyAssetListView;

            // Bind References Textblock Elements
            refExplorerToText = GetTemplateChild(PART_RefExplorerToTextBlock) as TextBlock;
            refExplorerFromText = GetTemplateChild(PART_RefExplorerFromTextBlock) as TextBlock;

            // Bind Open Asset Elements
            refExplorerToOpenItem = GetTemplateChild(PART_RefExplorerToOpenItem) as MenuItem;
            refExplorerFromOpenItem = GetTemplateChild(PART_RefExplorerFromOpenItem) as MenuItem;

            // Bind Find Asset Elements
            refExplorerToFindItem = GetTemplateChild(PART_RefExplorerToFindItem) as MenuItem;
            refExplorerFromFindItem = GetTemplateChild(PART_RefExplorerFromFindItem) as MenuItem;

            // Double Click Asset to Open
            refExplorerToList.SelectedAssetDoubleClick += ReferenceExplorerList_SelectedAssetDoubleClick;
            refExplorerFromList.SelectedAssetDoubleClick += ReferenceExplorerList_SelectedAssetDoubleClick;

            // Open Asset
            refExplorerToOpenItem.Click += contextMenuRefExplorerToOpen_Click;
            refExplorerFromOpenItem.Click += contextMenuRefExplorerFromOpen_Click;

            // Find Item in Data Explorer
            refExplorerToFindItem.Click += contextMenuRefExplorerToFind_Click;
            refExplorerFromFindItem.Click += contextMenuRefExplorerFromFind_Click;

            App.EditorWindow.DataExplorer.SelectionChanged += dataExplorer_SelectionChanged;
        }

        private void dataExplorer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry selectedEntry = App.SelectedAsset;

            RefreshReferences(selectedEntry);
        }

        private void ReferenceExplorerList_SelectedAssetDoubleClick(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = (sender as FrostyAssetListView).SelectedItem as EbxAssetEntry;
            if (entry == null)
                return;

            App.EditorWindow.OpenAsset(entry);
        }

        private void contextMenuRefExplorerToOpen_Click(object sender, RoutedEventArgs e)
        {
            if (refExplorerToList.SelectedItem == null)
                return;
            App.EditorWindow.OpenAsset(refExplorerToList.SelectedItem);
        }

        private void contextMenuRefExplorerFromOpen_Click(object sender, RoutedEventArgs e)
        {
            if (refExplorerFromList.SelectedItem == null)
                return;
            App.EditorWindow.OpenAsset(refExplorerFromList.SelectedItem);
        }

        private void contextMenuRefExplorerToFind_Click(object sender, RoutedEventArgs e)
        {
            if (refExplorerToList.SelectedItem == null)
                return;
            App.EditorWindow.DataExplorer.SelectAsset(refExplorerToList.SelectedItem);
        }

        private void contextMenuRefExplorerFromFind_Click(object sender, RoutedEventArgs e)
        {
            if (refExplorerFromList.SelectedItem == null)
                return;
            App.EditorWindow.DataExplorer.SelectAsset(refExplorerFromList.SelectedItem);
        }

        private void RefreshReferences(EbxAssetEntry entry)
        {
            //if (!ReferencesTabItem.IsSelected)
            //    return;

            if (entry == null)
            {
                refExplorerFromText.Text = "";
                refExplorerToText.Text = "No asset selected";
                refExplorerFromList.ItemsSource = null;
                refExplorerToList.ItemsSource = null;
                return;
            }

            refExplorerFromText.Text = "References from " + entry.Filename;
            refExplorerToText.Text = "References to " + entry.Filename;

            List<EbxAssetEntry> refToItems = new List<EbxAssetEntry>();
            List<EbxAssetEntry> refFromItems = new List<EbxAssetEntry>();

            foreach (EbxAssetEntry subEntry in App.AssetManager.EnumerateEbx())
            {
                if (subEntry.ContainsDependency(entry.Guid))
                    refToItems.Add(subEntry);
            }
            foreach (Guid guid in entry.EnumerateDependencies())
                refFromItems.Add(App.AssetManager.GetEbxEntry(guid));

            refExplorerToList.ItemsSource = refToItems;
            refExplorerFromList.ItemsSource = refFromItems;
        }
    }
}
