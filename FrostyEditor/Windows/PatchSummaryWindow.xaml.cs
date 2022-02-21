using FrostySdk.Managers;
using System.Windows;
using System.Windows.Controls;
using Frosty.Controls;
using FrostySdk.IO;
using System.IO;
using Frosty.Core.Controls;

namespace FrostyEditor.Windows
{
    /// <summary>
    /// Interaction logic for PatchSummaryWindow.xaml
    /// </summary>
    public partial class PatchSummaryWindow : FrostyDockableWindow
    {
        public PatchSummaryWindow(AssetManagerImportResult result)
        {
            InitializeComponent();

            Loaded += PatchSummaryWindow_Loaded;

            addedAssetsList.ItemsSource = result.AddedAssets;
            modifiedAssetsList.ItemsSource = result.ModifiedAssets;
            removedAssetsList.ItemsSource = result.RemovedAssets;
        }

        private void PatchSummaryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            totalTextBlock.Text = "Total: " + addedAssetsList.Items.Count;
        }

        private void okayButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Patch Summary", "*.txt (Text File)|*.txt", "Patch");
            if (sfd.ShowDialog())
            {
                using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                {
                    writer.WriteLine("Added:");
                    foreach (EbxAssetEntry entry in addedAssetsList.ItemsSource)
                        writer.WriteLine(entry.Name);
                    writer.WriteLine("Modified:");
                    foreach (EbxAssetEntry entry in modifiedAssetsList.ItemsSource)
                        writer.WriteLine(entry.Name);
                    writer.WriteLine("Removed:");
                    foreach (EbxAssetEntry entry in removedAssetsList.ItemsSource)
                        writer.WriteLine(entry.Name);
                }

                FrostyMessageBox.Show("Successfully exported Patch Summary", "Frosty Editor");
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addedAssetsList == null)
                return;

            TabControl tc = sender as TabControl;
            TabItem ti = tc.SelectedItem as TabItem;
            string header = ti.Header as string;
            int totalCount = 0;

            if (header.Contains("Added")) totalCount = addedAssetsList.Items.Count;
            else if (header.Contains("Modified")) totalCount = modifiedAssetsList.Items.Count;
            else totalCount = removedAssetsList.Items.Count;

            totalTextBlock.Text = "Total: " + totalCount;
        }
    }
}
