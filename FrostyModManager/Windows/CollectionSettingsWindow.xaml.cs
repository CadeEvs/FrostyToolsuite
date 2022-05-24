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

        public CollectionSettingsWindow(List<FrostyAppliedMod> mods)
        {
            InitializeComponent();

            availableMods = mods;

            Loaded += ModSettingsWindow_Loaded;
        }

        private void ModSettingsWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            modCategoryComboBox.ItemsSource = categories;

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

            for (int i = 0; i < availableMods.Count; i++)
            {
                if (availableMods[i].IsEnabled && availableMods[i].Mod is FrostyMod fmod)
                    appliedMods.Add(fmod);
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
                        
                        if (ssImageButton1.GetImage() != null)
                            collection.ModDetails.AddScreenshot(ssImageButton1.GetImage());
                        if (ssImageButton2.GetImage() != null)
                            collection.ModDetails.AddScreenshot(ssImageButton2.GetImage());
                        if (ssImageButton3.GetImage() != null)
                            collection.ModDetails.AddScreenshot(ssImageButton3.GetImage());
                        if (ssImageButton4.GetImage() != null)
                            collection.ModDetails.AddScreenshot(ssImageButton4.GetImage());

                        ZipArchiveEntry manifestEntry = archive.CreateEntry(collection.Filename);
                        using (Stream stream = manifestEntry.Open())
                        {
                            byte[] buffer = collection.WriteCollection();

                            stream.Write(buffer, 0, buffer.Length);
                        }

                        foreach (FrostyMod mod in appliedMods)
                        {
                            archive.CreateEntryFromFile(mod.Path, mod.Filename);
                            if (!mod.NewFormat)
                            {
                                archive.CreateEntryFromFile(mod.Path.Replace(".fbmod", "_01.archive"), mod.Filename.Replace(".fbmod", "_01.archive"));
                            }
                        }

                        archive.Dispose();
                    }
                });
                
                DialogResult = true;
                Close();
            }
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
    }
}