using Frosty.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Frosty.Core;
using Frosty.Core.Mod;
using System.Collections.Generic;
using Frosty.Core.Controls;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Frosty.Core.Windows;
using System;

namespace FrostyModManager.Windows
{
    /// <summary>
    /// Interaction logic for ModSettingsWindow.xaml
    /// </summary>
    public partial class CollectionSettingsWindow : FrostyDockableWindow
    {
        private List<FrostyAppliedMod> availableMods;

        private List<FrostyMod> appliedMods = new List<FrostyMod>();

        private List<string> categories = new List<string>()
        {
            "Custom",
            "Audio",
            "Cosmetic",
            "Gameplay",
            "Graphic",
            "Map",
            "User Interface"
        };

        public CollectionSettingsWindow(List<ISuperGamerLeagueGamer> mods)
        {
            InitializeComponent();

            availableMods = new List<FrostyAppliedMod>();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i] is FrostyModCollection)
                {
                    foreach (FrostyMod mod in (mods[i] as FrostyModCollection).Mods)
                        availableMods.Add(new FrostyAppliedMod(mod, false));
                }
                else
                    availableMods.Add(new FrostyAppliedMod(mods[i] as FrostyMod, false));
            }

            Loaded += ModSettingsWindow_Loaded;
        }

        private void ModSettingsWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            modCategoryComboBox.ItemsSource = categories;

            availableModsList.ItemsSource = availableMods;

            if (modCategoryComboBox.SelectedIndex == 0)
            {
                modCategoryTextBox.IsEnabled = true;
            }
            else if (modCategoryComboBox.SelectedIndex < 0)
                modCategoryComboBox.SelectedIndex = 0;
            else
            {
                modCategoryTextBox.Text = categories[0];
                modCategoryTextBox.IsEnabled = false;
            }
        }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void saveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (modTitleTextBox.Text == "" || modAuthorTextBox.Text == "" || modCategoryTextBox.Text == "" || modVersionTextBox.Text == "")
            {
                FrostyMessageBox.Show("Title, Author, Category and Version are mandatory fields", "Frosty Editor");
                return;
            }
            if (modLinkTextBox.Text != "" && (!Uri.IsWellFormedUriString(modLinkTextBox.Text, UriKind.RelativeOrAbsolute) || (!modLinkTextBox.Text.Contains("nexusmods.com") && !modLinkTextBox.Text.Contains("moddb.com"))))
            {
                FrostyMessageBox.Show("Link needs to be valid", "Frosty Editor");
                return;
            }

            List<string> paths = new List<string>(availableMods.Count);
            for (int i = 0; i < availableMods.Count; i++)
            {
                if (availableMods[i].IsEnabled)
                {
                    appliedMods.Add(availableMods[i].Mod as FrostyMod);
                    paths.Add(availableMods[i].Mod.Filename);
                }
            }
            CollectionManifest details = new CollectionManifest()
            {
                title = modTitleTextBox.Text,
                author = modAuthorTextBox.Text,
                category = modCategoryTextBox.Text,
                version = modVersionTextBox.Text,
                description = modDescriptionTextBox.Text,
                link = modLinkTextBox.Text,
            };

            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Export Collection", "*.zip (Zip File) |*.zip", "");
            if (sfd.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting Collection", "", (task) =>
                {
                    using (ZipArchive archive = ZipFile.Open(sfd.FileName, ZipArchiveMode.Create))
                    {
                        FrostyModCollection collection = new FrostyModCollection(details, appliedMods);
                        collection.ModDetails.SetIcon(iconImageButton.GetImage());
                        // TODO: add screenshots for collections
                        //collection.ModDetails.AddScreenshot(ssImageButton1.GetImage());
                        //collection.ModDetails.AddScreenshot(ssImageButton2.GetImage());
                        //collection.ModDetails.AddScreenshot(ssImageButton3.GetImage());
                        //collection.ModDetails.AddScreenshot(ssImageButton4.GetImage());

                        ZipArchiveEntry manifestEntry = archive.CreateEntry(collection.Filename);
                        using (Stream stream = manifestEntry.Open())
                        {
                            byte[] buffer = collection.WriteCollection();

                            stream.Write(buffer, 0, buffer.Length);
                        }

                        foreach (FrostyMod mod in appliedMods)
                        {
                            archive.CreateEntryFromFile(mod.Path, mod.Filename);
                        }

                        archive.Dispose();
                    }
                });
            }

            DialogResult = true;
            Close();
        }

        private bool FrostyImageButton_OnValidate(object sender, FileInfo fi, BitmapImage bimage)
        {
            FrostyImageButton btn = sender as FrostyImageButton;
            if (btn == iconImageButton)
            {
                if (bimage.PixelWidth > 128 || bimage.PixelHeight > 128)
                {
                    FrostyMessageBox.Show("Icon cannot be larger than 128x128");
                    return false;
                }
            }
            else
            {
                if (fi.Length > (5 * 1024 * 1024))
                {
                    FrostyMessageBox.Show("Screenshots cannot be larger than 5mb each");
                    return false;
                }
            }

            return true;
        }

        private void modCategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (modCategoryComboBox.SelectedItem.ToString() == "Custom")
            {
                modCategoryTextBox.Text = "";
                modCategoryTextBox.IsEnabled = true;
            }
            else
            {
                modCategoryTextBox.Text = modCategoryComboBox.SelectedItem.ToString();
                modCategoryTextBox.IsEnabled = false;
            }
        }

        private void enabledCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void enabledCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void downButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrostyAppliedMod mod = availableModsList.SelectedItem as FrostyAppliedMod;

            int index = availableMods.FindIndex((FrostyAppliedMod a) => a == mod);

            if (index >= availableMods.Count - 1)
                return;

            availableMods.RemoveAt(index);
            availableMods.Insert(++index, mod);

            availableModsList.Items.Refresh();

            if (availableModsList.SelectedIndex > 0)
                upButton.IsEnabled = true;
            else
                upButton.IsEnabled = false;

            if (availableModsList.SelectedIndex <= availableMods.Count - 1)
                downButton.IsEnabled = true;
            else
                downButton.IsEnabled = false;
        }

        private void upButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrostyAppliedMod mod = availableModsList.SelectedItem as FrostyAppliedMod;

            int index = availableMods.FindIndex((FrostyAppliedMod a) => a == mod);

            if (index <= 0)
                return;

            availableMods.RemoveAt(index);
            availableMods.Insert(--index, mod);

            availableModsList.Items.Refresh();

            if (availableModsList.SelectedIndex > 0)
                upButton.IsEnabled = true;
            else
                upButton.IsEnabled = false;

            if (availableModsList.SelectedIndex <= availableMods.Count - 1)
                downButton.IsEnabled = true;
            else
                downButton.IsEnabled = false;
        }

        private void availableModsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (availableModsList.SelectedIndex > 0)
                upButton.IsEnabled = true;
            else
                upButton.IsEnabled = false;

            if (availableModsList.SelectedIndex <= availableMods.Count - 1)
                downButton.IsEnabled = true;
            else
                downButton.IsEnabled = false;
        }
    }
}