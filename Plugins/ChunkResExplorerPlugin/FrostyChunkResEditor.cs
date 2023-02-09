using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using Frosty.Core;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace ChunkResEditorPlugin
{
    public class FrostyChunkExportCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is FrameworkElement param && param.Tag is FrostyChunkResEditor explorer)
            {
                explorer.ExportChunk();
            }
        }
    }
    public class FrostyChunkImportCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is FrameworkElement param && param.Tag is FrostyChunkResEditor explorer)
            {
                explorer.ImportChunk();
            }
        }
    }
    public class FrostyChunkRevertCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is FrameworkElement param && param.Tag is FrostyChunkResEditor explorer)
            {
                explorer.RevertChunk();
            }
        }
    }
    public class FrostyChunkRightClickCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is ListBoxItem lbi) 
                lbi.IsSelected = true;
        }
    }

    [TemplatePart(Name = PART_ChunksListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_ChunksBundlesBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_ResExplorer, Type = typeof(FrostyDataExplorer))]
    [TemplatePart(Name = PART_ResBundlesBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_ResExportMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_ResImportMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_RevertMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_ChunkFilter, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ChunkModified, Type = typeof(CheckBox))]
    public class FrostyChunkResEditor : FrostyBaseEditor
    {
        public override ImageSource Icon => ChunkResEditorMenuExtension.imageSource;

        private const string PART_ChunksListBox = "PART_ChunksListBox";
        private const string PART_ChunksBundlesBox = "PART_ChunksBundlesBox";
        private const string PART_ResExplorer = "PART_ResExplorer";
        private const string PART_ResBundlesBox = "PART_ResBundlesBox";
        private const string PART_ResExportMenuItem = "PART_ResExportMenuItem";
        private const string PART_ResImportMenuItem = "PART_ResImportMenuItem";
        private const string PART_RevertMenuItem = "PART_RevertMenuItem";
        private const string PART_ChunkFilter = "PART_ChunkFilter";
        private const string PART_ChunkModified = "PART_ChunkModified";

        private ListBox chunksListBox;
        private ListBox chunksBundleBox;
        private FrostyDataExplorer resExplorer;
        private ListBox resBundleBox;
        private TextBox chunkFilterTextBox;
        private CheckBox chunkModifiedBox;
        private ILogger logger;

        static FrostyChunkResEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyChunkResEditor), new FrameworkPropertyMetadata(typeof(FrostyChunkResEditor)));
        }

        public FrostyChunkResEditor(ILogger inLogger = null)
        {
            logger = inLogger;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            chunksBundleBox = GetTemplateChild(PART_ChunksBundlesBox) as ListBox;
            resBundleBox = GetTemplateChild(PART_ResBundlesBox) as ListBox;
            chunksListBox = GetTemplateChild(PART_ChunksListBox) as ListBox;
            resExplorer = GetTemplateChild(PART_ResExplorer) as FrostyDataExplorer;
            chunkFilterTextBox = GetTemplateChild(PART_ChunkFilter) as TextBox;
            chunkModifiedBox = GetTemplateChild(PART_ChunkModified) as CheckBox;

            resExplorer.SelectionChanged += ResExplorer_SelectionChanged;
            MenuItem mi = GetTemplateChild(PART_ResExportMenuItem) as MenuItem;
            mi.Click += ResExportMenuItem_Click;

            mi = GetTemplateChild(PART_ResImportMenuItem) as MenuItem;
            mi.Click += ResImportMenuItem_Click;

            mi = GetTemplateChild(PART_RevertMenuItem) as MenuItem;
            mi.Click += ResRevertMenuItem_Click;

            Loaded += FrostyChunkResEditor_Loaded;
            chunksListBox.SelectionChanged += ChunksListBox_SelectionChanged;
            chunkFilterTextBox.LostFocus += ChunkFilterTextBox_LostFocus;
            chunkFilterTextBox.KeyUp += ChunkFilterTextBox_KeyUp;
            chunkModifiedBox.Checked += ChunkFilterTextBox_LostFocus;
            chunkModifiedBox.Unchecked += ChunkFilterTextBox_LostFocus;
        }

        private void ResExplorer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (resExplorer.SelectedAsset != null)
            {
                resBundleBox.Items.Clear();
                ResAssetEntry SelectedRes = (ResAssetEntry)resExplorer.SelectedAsset;
                resBundleBox.Items.Add("Selected resource is in Bundles: ");
                foreach (int bundle in SelectedRes.Bundles)
                {
                    resBundleBox.Items.Add(App.AssetManager.GetBundleEntry(bundle).Name);
                }
                if (SelectedRes.AddedBundles.Count != 0)
                {
                    resBundleBox.Items.Add("Added to Bundles:");
                    foreach (int bundle in SelectedRes.AddedBundles)
                    {
                        resBundleBox.Items.Add(App.AssetManager.GetBundleEntry(bundle).Name);
                    }
                }
            }
            else
            {
                resBundleBox.Items.Clear();
                resBundleBox.Items.Add("No res selected");
            }
        }

        private void ChunksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chunksListBox.SelectedIndex != -1)
            {
                chunksBundleBox.Items.Clear();
                ChunkAssetEntry SelectedChk = (ChunkAssetEntry)chunksListBox.SelectedItem;
                string FirstLine = "Selected chunk is in Bundles: ";
                if (SelectedChk.FirstMip != -1)
                    FirstLine += " (FirstMip:" + SelectedChk.FirstMip + ")";
                if (App.FileSystemManager.GetManifestChunk(SelectedChk.Id) != null)
                {
                    chunksBundleBox.Items.Add("Selected chunk is a Manifest chunk.");
                }
                else if (SelectedChk.SuperBundles.Count != 0)
                {
                    chunksBundleBox.Items.Add("Selected chunk is in SuperBundles:");
                    foreach (int superbundle in SelectedChk.SuperBundles)
                    {
                        chunksBundleBox.Items.Add(App.AssetManager.GetSuperBundle(superbundle).Name);
                    }
                }
                if (SelectedChk.AddedSuperBundles.Count != 0)
                {
                    chunksBundleBox.Items.Add("Added to SuperBundles:");
                    foreach (int superbundle in SelectedChk.AddedSuperBundles)
                    {
                        chunksBundleBox.Items.Add(App.AssetManager.GetSuperBundle(superbundle).Name);
                    }
                }
                if (SelectedChk.Bundles.Count != 0)
                {
                    chunksBundleBox.Items.Add(FirstLine);
                    foreach (int bundle in SelectedChk.Bundles)
                    {
                        chunksBundleBox.Items.Add(App.AssetManager.GetBundleEntry(bundle).Name);
                    }
                }
                if (SelectedChk.AddedBundles.Count != 0)
                {
                    chunksBundleBox.Items.Add("Added to Bundles:");
                    foreach (int bundle in SelectedChk.AddedBundles)
                    {
                        chunksBundleBox.Items.Add(App.AssetManager.GetBundleEntry(bundle).Name);
                    }
                }
            }
            else
            {
                chunksBundleBox.Items.Clear();
                chunksBundleBox.Items.Add("No chunk selected");
            }
        }

        private void ChunkFilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UpdateFilter();
        }

        private void ChunkFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            if (chunkFilterTextBox.Text == "" & chunkModifiedBox.IsChecked == false)
            {
                chunksListBox.Items.Filter = null;
                return;
            }
            else if (chunkFilterTextBox.Text != "" & chunkModifiedBox.IsChecked == false)
            {
                chunksListBox.Items.Filter = new Predicate<object>((object a) => ((ChunkAssetEntry)a).Id.ToString().Contains(chunkFilterTextBox.Text.ToLower()));
            }
            else if (chunkFilterTextBox.Text == "" & chunkModifiedBox.IsChecked == true)
            {
                chunksListBox.Items.Filter = new Predicate<object>((object a) => ((ChunkAssetEntry)a).IsModified);
            }
            else if (chunkFilterTextBox.Text != "" & chunkModifiedBox.IsChecked == true)
            {
                chunksListBox.Items.Filter = new Predicate<object>((object a) => (((ChunkAssetEntry)a).IsModified) & ((ChunkAssetEntry)a).Id.ToString().Contains(chunkFilterTextBox.Text.ToLower()));
            }
        }

        private void ResRevertMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(resExplorer.SelectedAsset is ResAssetEntry selectedAsset) || !selectedAsset.IsModified)
                return;

            FrostyTaskWindow.Show("Reverting Asset", "", (task) => { App.AssetManager.RevertAsset(selectedAsset, suppressOnModify: false); });
            resExplorer.RefreshItems();
        }

        private void ResImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ResAssetEntry selectedAsset = resExplorer.SelectedAsset as ResAssetEntry;
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Open Resource", "*.res (Resource Files)|*.res", "Res");

            if (ofd.ShowDialog())
            {
                FrostyTaskWindow.Show("Importing Asset", "", (task) =>
                {
                    using (NativeReader reader = new NativeReader(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read)))
                    {
                        byte[] resMeta = reader.ReadBytes(0x10);
                        byte[] buffer = reader.ReadToEnd();

                        if (App.PluginManager.GetCustomHandler((ResourceType)selectedAsset.ResType) != null)
                        {
                            // @todo: throw some kind of error
                        }

                        // @todo
                        //if (selectedAsset.ResType == (uint)ResourceType.ShaderBlockDepot)
                        //{
                        //    // treat manually imported shaderblocks as if every block has been modified

                        //    ShaderBlockDepot sbd = new ShaderBlockDepot(resMeta);
                        //    using (NativeReader subReader = new NativeReader(new MemoryStream(buffer)))
                        //        sbd.Read(subReader, App.AssetManager, selectedAsset, null);

                        //    for (int j = 0; j < sbd.ResourceCount; j++)
                        //    {
                        //        var sbr = sbd.GetResource(j);
                        //        if (sbr is ShaderPersistentParamDbBlock || sbr is MeshParamDbBlock)
                        //            sbr.IsModified = true;
                        //    }

                        //    App.AssetManager.ModifyRes(selectedAsset.Name, sbd);
                        //}

                        //else
                        {
                            App.AssetManager.ModifyRes(selectedAsset.Name, buffer, resMeta);
                        }
                    }
                });
                resExplorer.RefreshItems();
            }
        }

        private void ResExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ResAssetEntry selectedAsset = resExplorer.SelectedAsset as ResAssetEntry;
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Resource", "*.res (Resource Files)|*.res", "Res", selectedAsset.Filename);

            Stream resStream = App.AssetManager.GetRes(selectedAsset);
            if (resStream == null)
                return;

            if (sfd.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting Asset", "", (task) =>
                {
                    using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                    {
                        // write res meta first
                        writer.Write(selectedAsset.ResMeta);

                        // followed by remaining data
                        using (NativeReader reader = new NativeReader(resStream))
                            writer.Write(reader.ReadToEnd());
                    }
                });
                logger?.Log("Resource saved to {0}", sfd.FileName);
            }
        }

        public void ImportChunk()
        {
            ChunkAssetEntry selectedAsset = chunksListBox.SelectedItem as ChunkAssetEntry;
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Open Chunk", "*.chunk (Chunk Files)|*.chunk", "Chunk");

            if (ofd.ShowDialog())
            {
                FrostyTaskWindow.Show("Importing Chunk", "", (task) =>
                {
                    using (NativeReader reader = new NativeReader(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read)))
                    {
                        byte[] buffer = reader.ReadToEnd();
                        App.AssetManager.ModifyChunk(selectedAsset.Id, buffer);
                    }
                });
                RefreshChunksListBox(selectedAsset);
            }
        }

        public void ExportChunk()
        {
            ChunkAssetEntry selectedAsset = chunksListBox.SelectedItem as ChunkAssetEntry;
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Chunk", "*.chunk (Chunk Files)|*.chunk", "Chunk", selectedAsset.Filename);

            Stream chunkStream = App.AssetManager.GetChunk(selectedAsset);
            if (chunkStream == null)
                return;

            if (sfd.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting Chunk", "", (task) =>
                {
                    using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                    {
                        using (NativeReader reader = new NativeReader(chunkStream))
                            writer.Write(reader.ReadToEnd());
                    }
                });
                logger?.Log("Chunk saved to {0}", sfd.FileName);
            }
        }

        public void RevertChunk()
        {
            ChunkAssetEntry selectedAsset = chunksListBox.SelectedItem as ChunkAssetEntry;
            if (selectedAsset == null || !selectedAsset.IsModified)
                return;

            FrostyTaskWindow.Show("Reverting Chunk", "", (task) => { App.AssetManager.RevertAsset(selectedAsset, suppressOnModify: false); });
            RefreshChunksListBox(selectedAsset);
        }

        private void FrostyChunkResEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (resExplorer.ItemsSource != null)
                return;

            resExplorer.ItemsSource = App.AssetManager.EnumerateRes();
            chunksListBox.ItemsSource = App.AssetManager.EnumerateChunks();
            chunksListBox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("DisplayName", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void RefreshChunksListBox(ChunkAssetEntry selectedAsset)
        {
            chunksListBox.Items.Refresh();
        }
    }
}
