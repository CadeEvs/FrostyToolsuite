using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BundleEditPlugin
{
    public class SvgImageExtension : AddToBundleExtension
    {
        public override string AssetType => "SvgImage";
        public override void AddToBundle(EbxAssetEntry entry, BundleEntry bentry)
        {
            base.AddToBundle(entry, bentry);

            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic svgAsset = asset.RootObject;

            ResAssetEntry resEntry = App.AssetManager.GetResEntry(svgAsset.Resource);
            resEntry.AddToBundle(App.AssetManager.GetBundleId(bentry));

            entry.LinkAsset(resEntry);
        }
    }

    public class TextureExtension : AddToBundleExtension
    {
        public override string AssetType => "TextureAsset";
        public override void AddToBundle(EbxAssetEntry entry, BundleEntry bentry)
        {
            base.AddToBundle(entry, bentry);

            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic textureAsset = asset.RootObject;

            ResAssetEntry resEntry = App.AssetManager.GetResEntry(textureAsset.Resource);
            resEntry.AddToBundle(App.AssetManager.GetBundleId(bentry));

            Texture texture = App.AssetManager.GetResAs<Texture>(resEntry);
            ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(texture.ChunkId);

            chunkEntry.AddToBundle(App.AssetManager.GetBundleId(bentry));
            chunkEntry.FirstMip = texture.FirstMip;

            resEntry.LinkAsset(chunkEntry);
            entry.LinkAsset(resEntry);
        }
    }

    public class MovieTexture2Extension : AddToBundleExtension
    {
        public override string AssetType => "MovieTexture2Asset";
        public override void AddToBundle(EbxAssetEntry entry, BundleEntry bentry)
        {
            base.AddToBundle(entry, bentry);

            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic movieAsset = asset.RootObject;

            ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(movieAsset.ChunkGuid);
            chunkEntry.AddToBundle(App.AssetManager.GetBundleId(bentry));
            entry.LinkAsset(chunkEntry);

            chunkEntry = App.AssetManager.GetChunkEntry(movieAsset.SubtitleChunkGuid);
            if (chunkEntry != null)
            {
                chunkEntry.AddToBundle(App.AssetManager.GetBundleId(bentry));
                entry.LinkAsset(chunkEntry);
            }
        }
    }

    public class SoundWaveExtension : AddToBundleExtension
    {
        public override string AssetType => "SoundWaveAsset";
        public override void AddToBundle(EbxAssetEntry entry, BundleEntry bentry)
        {
            base.AddToBundle(entry, bentry);

            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic soundAsset = asset.RootObject;

            foreach (var soundDataChunk in soundAsset.Chunks)
            {
                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);
                chunkEntry.AddToBundle(App.AssetManager.GetBundleId(bentry));
                entry.LinkAsset(chunkEntry);
            }
        }
    }

    public class AddToBundleExtension
    {
        public virtual string AssetType => null;
        public virtual void AddToBundle(EbxAssetEntry entry, BundleEntry bentry)
        {
            entry.AddToBundle(App.AssetManager.GetBundleId(bentry));
        }
    }

    [TemplatePart(Name = PART_BundleTypeComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_BundlesListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_DataExplorer, Type = typeof(FrostyDataExplorer))]
    [TemplatePart(Name = PART_SuperBundleTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_BundleFilterTextBox, Type = typeof(TextBox))]
    public class BundleEditor : FrostyBaseEditor
    {
        private const string PART_BundleTypeComboBox = "PART_BundleTypeComboBox";
        private const string PART_BundlesListBox = "PART_BundlesListBox";
        private const string PART_DataExplorer = "PART_DataExplorer";
        private const string PART_SuperBundleTextBox = "PART_SuperBundleTextBox";
        private const string PART_BundleFilterTextBox = "PART_BundleFilterTextBox";

        public override ImageSource Icon => BundleEditorMenuExtension.iconImageSource;
        public RelayCommand AddToBundleCommand { get; }

        private ComboBox bundleTypeComboBox;
        private ListBox bundlesListBox;
        private FrostyDataExplorer dataExplorer;
        private TextBox superBundleTextBox;
        private TextBox bundleFilterTextBox;

        private BundleType selectedBundleType = BundleType.SharedBundle;
        private Dictionary<string, AddToBundleExtension> extensions = new Dictionary<string, AddToBundleExtension>();

        static BundleEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BundleEditor), new FrameworkPropertyMetadata(typeof(BundleEditor)));
        }

        public BundleEditor()
        {
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(AddToBundleExtension)))
                {
                    var extension = (AddToBundleExtension)Activator.CreateInstance(type);
                    extensions.Add(extension.AssetType, extension);
                }
            }
            extensions.Add("null", new AddToBundleExtension());

            AddToBundleCommand = new RelayCommand(
                (o) =>
                {
                    EbxAssetEntry entry = App.EditorWindow.DataExplorer.SelectedAsset as EbxAssetEntry;
                    BundleEntry bentry = bundlesListBox.SelectedItem as BundleEntry;

                    string key = entry.Type;
                    if (!extensions.ContainsKey(entry.Type))
                        key = "null";
                    extensions[key].AddToBundle(entry, bentry);

                    RefreshExplorer();
                    App.EditorWindow.DataExplorer.RefreshItems();

                    dataExplorer.SelectAsset(entry);
                },
                (o) =>
                {
                    return App.EditorWindow.DataExplorer.SelectedAsset != null && bundlesListBox.SelectedItem != null;
                });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            bundleTypeComboBox = GetTemplateChild(PART_BundleTypeComboBox) as ComboBox;
            bundlesListBox = GetTemplateChild(PART_BundlesListBox) as ListBox;
            dataExplorer = GetTemplateChild(PART_DataExplorer) as FrostyDataExplorer;
            superBundleTextBox = GetTemplateChild(PART_SuperBundleTextBox) as TextBox;
            bundleFilterTextBox = GetTemplateChild(PART_BundleFilterTextBox) as TextBox;

            bundleTypeComboBox.SelectionChanged += bundleTypeComboBox_SelectionChanged;
            bundlesListBox.SelectionChanged += bundlesListBox_SelectionChanged;
            dataExplorer.SelectedAssetDoubleClick += dataExplorer_SelectedAssetDoubleClick;

            bundleFilterTextBox.KeyUp += BundleFilterTextBox_KeyUp;
            bundleFilterTextBox.LostFocus += BundleFilterTextBox_LostFocus;

            bundleTypeComboBox.SelectedIndex = 2;
        }

        private void BundleFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (bundleFilterTextBox.Text == "")
                bundlesListBox.Items.Filter = null;
            else
            {
                string filterText = bundleFilterTextBox.Text.ToLower();
                bundlesListBox.Items.Filter = (object a) => { return ((BundleEntry)a).Name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0; };
            }
        }

        private void BundleFilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bundleFilterTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void bundlesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshExplorer();
        }

        private void bundleTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bundlesListBox.Items.Filter = null;
            bundleFilterTextBox.Text = "";

            int index = bundleTypeComboBox.SelectedIndex;
            selectedBundleType = (new BundleType[] { BundleType.SubLevel, BundleType.BlueprintBundle, BundleType.SharedBundle })[index];
            RefreshList();
        }

        private void RefreshList()
        {
            bundlesListBox.ItemsSource = App.AssetManager.EnumerateBundles(selectedBundleType);
            bundlesListBox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("DisplayName", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void RefreshExplorer()
        {
            BundleEntry entry = bundlesListBox.SelectedItem as BundleEntry;
            if (entry == null)
                return;
            dataExplorer.ItemsSource = App.AssetManager.EnumerateEbx(entry);
            superBundleTextBox.Text = App.AssetManager.GetSuperBundle(entry.SuperBundleId).Name;
            if (entry.Type != BundleType.SharedBundle)
                dataExplorer.SelectAsset(entry.Blueprint);
        }

        private void dataExplorer_SelectedAssetDoubleClick(object sender, RoutedEventArgs e)
        {
            EbxAssetEntry entry = dataExplorer.SelectedAsset as EbxAssetEntry;
            App.EditorWindow.OpenAsset(entry);
        }
    }
}
