using Frosty.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Frosty.Core;
using Frosty.Core.Mod;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls;

namespace FrostyEditor.Windows
{
    /// <summary>
    /// Interaction logic for ModSettingsWindow.xaml
    /// </summary>
    public partial class ModSettingsWindow : FrostyDockableWindow
    {
        private ModSettings ModSettings => project.GetModSettings();
        private FrostyProject project;

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

        public ModSettingsWindow(FrostyProject inProject = null)
        {
            InitializeComponent();

            project = inProject;
            Loaded += ModSettingsWindow_Loaded;
        }

        private void ModSettingsWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ErrorMessageLabel.Visibility = System.Windows.Visibility.Collapsed;

            modCategoryComboBox.ItemsSource = categories;
            
            modTitleTextBox.Text = ModSettings.Title;
            modAuthorTextBox.Text = ModSettings.Author;
            modCategoryComboBox.SelectedIndex = ModSettings.SelectedCategory;
            modVersionTextBox.Text = ModSettings.Version;
            modDescriptionTextBox.Text = ModSettings.Description;

            if (modCategoryComboBox.SelectedItem.ToString() == "Custom")
            {
                modCategoryTextBox.Text = ModSettings.Category;
                modCategoryTextBox.IsEnabled = true;
            }
            else
            {
                modCategoryTextBox.Text = categories[ModSettings.SelectedCategory];
                modCategoryTextBox.IsEnabled = false;
            }

            iconImageButton.SetImage(ModSettings.Icon);
            ssImageButton1.SetImage(ModSettings.GetScreenshot(0));
            ssImageButton2.SetImage(ModSettings.GetScreenshot(1));
            ssImageButton3.SetImage(ModSettings.GetScreenshot(2));
            ssImageButton4.SetImage(ModSettings.GetScreenshot(3));
        }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void saveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<string> errors = new List<string>();

            // Check Title
            if (String.IsNullOrEmpty(modTitleTextBox.Text))
            {
                errors.Add("Title is a mandatory field");
                modTitleTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else if (modTitleTextBox.Text.Any(c => c > sbyte.MaxValue))
            {
                errors.Add("Title can only contain ASCII characters");
                modTitleTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                modTitleTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
            }

            // Check Author
            if (String.IsNullOrEmpty(modAuthorTextBox.Text))
            {
                errors.Add("Author is a mandatory field");
                modAuthorTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else if (modAuthorTextBox.Text.Any(c => c > sbyte.MaxValue))
            {
                errors.Add("Author can only contain ASCII characters");
                modAuthorTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                modAuthorTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
            }

            // Check Category
            if (String.IsNullOrEmpty(modCategoryTextBox.Text))
            {
                errors.Add("Category is a mandatory field");
                modCategoryTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else if (modCategoryTextBox.Text.Any(c => c > sbyte.MaxValue))
            {
                errors.Add("Category can only contain ASCII characters");
                modCategoryTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                modCategoryTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
            }

            // Check Version
            if (String.IsNullOrEmpty(modVersionTextBox.Text))
            {
                errors.Add("Version is a mandatory field");
                modVersionTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else if (modVersionTextBox.Text.Any(c => c > sbyte.MaxValue))
            {
                errors.Add("Version can only contain ASCII characters");
                modVersionTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                modVersionTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
            }

            // Check Link
            string[] approvedDomains = { "nexusmods.com", "moddb.com" };
            if (!String.IsNullOrEmpty(modPageLinkTextBox.Text))
            {
                if (!Uri.IsWellFormedUriString(modPageLinkTextBox.Text, UriKind.RelativeOrAbsolute))
                {
                    errors.Add("Link needs to be valid");
                    modPageLinkTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                else if (!approvedDomains.Any(modPageLinkTextBox.Text.Contains))
                {
                    errors.Add("Link needs to be nexusmods.com or moddb.com");
                    modPageLinkTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    if (Uri.TryCreate(modPageLinkTextBox.Text, UriKind.Absolute, out Uri uriResult))
                        modPageLinkTextBox.Text = uriResult.AbsoluteUri;

                    if (modPageLinkTextBox.Text.Any(c => c > sbyte.MaxValue))
                    {
                        errors.Add("Link can only contain ASCII characters");
                        modPageLinkTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    }
                    else
                    {
                        modPageLinkTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
                    }
                }
            }
            else
            {
                modPageLinkTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
            }

            // Check Description
            if (!String.IsNullOrEmpty(modDescriptionTextBox.Text) &&
                !modDescriptionTextBox.Text.Any(c => c > sbyte.MaxValue))
            {
                errors.Add("Description can only contain ASCII characters");
                modDescriptionTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                modDescriptionTextBox.BorderBrush = (System.Windows.Media.Brush)FindResource("ControlBackground");
            }

            // Check if error exits
            if (errors.Any())
            {
                ErrorMessageLabel.Visibility = System.Windows.Visibility.Visible;
                (ErrorMessageLabel.Content as TextBlock).Text = String.Join(Environment.NewLine, errors);
                return;
            }

            ErrorMessageLabel.Visibility = System.Windows.Visibility.Collapsed;

            ModSettings.Title = modTitleTextBox.Text;
            ModSettings.Author = modAuthorTextBox.Text;
            ModSettings.Category = modCategoryTextBox.Text;
            ModSettings.SelectedCategory = modCategoryComboBox.SelectedIndex;
            ModSettings.Version = modVersionTextBox.Text;
            ModSettings.Description = modDescriptionTextBox.Text;
            ModSettings.Link = modPageLinkTextBox.Text;
            ModSettings.Icon = iconImageButton.GetImage();
            ModSettings.SetScreenshot(0, ssImageButton1.GetImage());
            ModSettings.SetScreenshot(1, ssImageButton2.GetImage());
            ModSettings.SetScreenshot(2, ssImageButton3.GetImage());
            ModSettings.SetScreenshot(3, ssImageButton4.GetImage());

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
    }
}
