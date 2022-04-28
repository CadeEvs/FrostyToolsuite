using System.IO;
using System.Windows;
using System.Windows.Media;
using Frosty.Controls;
using Frosty.Core;
using FrostySdk;

namespace FrostyModManager
{
    /// <summary>
    /// Author: Clonedelta
    /// Class <c>ManageModDataWindow</c> handles the logic for deleting specified ModData folders. 
    /// </summary>
    public partial class ManageModDataWindow : FrostyDockableWindow
    {
        public ManageModDataWindow()
        {
            InitializeComponent();

            // Draws the window in the center of the screen
            Window mainWin = Application.Current.MainWindow;
            if (mainWin != null)
            {
                double x = mainWin.Left + (mainWin.Width / 2.0);
                double y = mainWin.Top + (mainWin.Height / 2.0);

                Left = x - (Width / 2.0);
                Top = y - (Height / 2.0);
            }

            // Draws the user's ModData directory on modDataNameTextBox
            string modPath = getModDataPath();
            modDataNameTextBox.Text = modPath;

            if (!Directory.Exists(modPath + "\\ModData"))
            {
                Directory.CreateDirectory(modPath + "\\ModData"); // Done to avoid potential IO error on init
            }

            listPacks();
        }

        /// <summary>
        /// Method <c>getModDataPath</c> Returns the path to the ModData folder.
        /// </summary>
        private string getModDataPath()
        {
            return Config.Get<string>("GamePath", "", ConfigScope.Game, ProfilesLibrary.ProfileName);
        }

        /// <summary>
        /// Method <c>listPacks</c> lists the available packs in the ModData folder
        /// </summary>
        private void listPacks()
        {
            string modPath = getModDataPath();

            // Grabs the packs currently in the ModData folder.
            string[] modDataPacks = Directory.GetDirectories(@modPath + "\\ModData\\", "*", SearchOption.TopDirectoryOnly);

            // Adds them to the ComboBox in the window.
            // TODO: Find better way to get subdirectory names instead of parsing full path
            foreach (string packNamePath in modDataPacks)
            {
                string[] packNamePathSplit = packNamePath.Split('\\');
                string packName = packNamePathSplit[packNamePathSplit.Length - 1];
                modDataComboBox.Items.Add(packName);
            }
        }

        /// <summary>
        /// Method <c>deleteModDataButton_Click</c> Delete operation for the selected ModData pack folder
        /// </summary>
        private void deleteModDataButton_Click(object sender, RoutedEventArgs e)
        {
            string modPath = getModDataPath();

            try
            {
                // Checks which ComboBox item was selected
                switch ((string)modDataComboBox.SelectedValue)
                {
                    case "-- Select a Pack --":
                        outputText.Text = "Please select a Pack to delete.";
                        outputText.Foreground = Brushes.Red;
                        outputText.Visibility = Visibility.Visible;
                        break;

                    case "All ModData":
                        Directory.Delete(modPath + "\\ModData", true);
                        Directory.CreateDirectory(modPath + "\\ModData"); // Done to avoid potential IO error on init

                        outputText.Text = "ModData Successfully Deleted.";
                        outputText.Foreground = Brushes.Green;
                        outputText.Visibility = Visibility.Visible;

                        // Removes all options from the ComboBox
                        for (int i = 2; i < modDataComboBox.Items.Count;)
                        {
                            modDataComboBox.Items.RemoveAt(i);
                        }

                        modDataComboBox.SelectedIndex = 0;
                        break;

                    default:

                        Directory.Delete(modPath + "\\ModData\\" + modDataComboBox.SelectedItem, true);

                        outputText.Text = modDataComboBox.SelectedItem + "'s ModData Successfully Deleted.";
                        outputText.Foreground = Brushes.Green;
                        outputText.Visibility = Visibility.Visible;

                        modDataComboBox.Items.Remove(modDataComboBox.SelectedItem);
                        modDataComboBox.SelectedIndex = 0;
                        break;
                }
            }
            catch (IOException)
            {
                outputText.Text = "Error deleting Pack!\nTry running Frosty as Administrator.";
                outputText.Foreground = Brushes.Red;
                outputText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Method <c>closeButton_Click</c> closes the window
        /// </summary>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
