using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace DuplicationPlugin.Windows
{
    /// <summary>
    /// Interaction logic for DuplicateAssetWindow.xaml
    /// </summary>
    public partial class DuplicateAssetWindow : FrostyDockableWindow
    {
        public string SelectedPath { get; private set; } = "";
        public string SelectedName { get; private set; } = "";
        public Type SelectedType { get; private set; } = null;
        private EbxAssetEntry entry;

        public DuplicateAssetWindow(EbxAssetEntry currentEntry)
        {
            InitializeComponent();

            pathSelector.ItemsSource = App.AssetManager.EnumerateEbx();
            entry = currentEntry;

            assetNameTextBox.Text = currentEntry.Filename;
            assetTypeTextBox.Text = entry.Type;
        }

        private void AssetNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string tmp = assetNameTextBox.Text.Replace('\\', '/').Trim('/');
            string fullName = pathSelector.SelectedPath + "/" + tmp;

            if (!string.IsNullOrEmpty(assetNameTextBox.Text) && !entry.Name.Equals(fullName, StringComparison.OrdinalIgnoreCase))
            {
                if (!tmp.Contains("//"))
                {
                    SelectedName = tmp;
                    SelectedPath = pathSelector.SelectedPath;

                    DialogResult = true;
                    Close();
                }
                else
                {
                    FrostyMessageBox.Show("Name of asset is invalid", "Frosty Editor");
                }
            }
            else
            {
                FrostyMessageBox.Show("Name of asset must be unique", "Frosty Editor");
            }
        }

        private void FrostyDockableWindow_FrostyLoaded(object sender, EventArgs e)
        {
            pathSelector.SelectAsset(entry);
        }

        private void ClassSelector_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        private void typeButton_Click(object sender, RoutedEventArgs e)
        {
            ClassSelector win = new ClassSelector(TypeLibrary.GetTypes("Asset"), allowAssets: true);
            if (win.ShowDialog() == true)
            {
                Type selectedType = win.SelectedClass;
                if (selectedType != null)
                {
                    SelectedType = selectedType;
                    assetTypeTextBox.Text = selectedType.Name;
                }
            }
        }
    }
}
