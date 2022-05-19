using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Frosty.Controls;
using Frosty.Core;
using Frosty.ModSupport;
using FrostySdk;

namespace FrostyModManager
{

    public class Pack
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }


    /// <summary>
    /// Author: Clonedelta, Dyvinia
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
                Top = y - (MaxHeight / 2.0);
            }

            // Draws the user's ModData directory on modDataNameTextBox
            string modDataPath = getModDataPath();
            modDataNameTextBox.Text = modDataPath;

            // Done to avoid potential IO error on init
            if (!Directory.Exists(modDataPath))
                Directory.CreateDirectory(modDataPath);

            listPacks();
        }

        /// <summary>
        /// Method <c>getModDataPath</c> Returns the path to the ModData folder.
        /// </summary>
        private string getModDataPath()
        {
            return Config.Get<string>("GamePath", "", ConfigScope.Game, ProfilesLibrary.ProfileName) + "\\ModData";
        }

        /// <summary>
        /// Method <c>listPacks</c> lists the available packs in the ModData folder
        /// </summary>
        private void listPacks()
        {
            // Cleans out old items
            modDataList.Items.Clear();

            string modDataPath = getModDataPath();

            // Grabs the packs currently in the ModData folder.
            string[] modDataPacks = Directory.GetDirectories(modDataPath, "*", SearchOption.TopDirectoryOnly);

            // Adds them to the ComboBox in the window.
            foreach (string packNamePath in modDataPacks)
            {
                modDataList.Items.Add(new Pack { Name = Path.GetFileName(packNamePath), Path = packNamePath });
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

        /// <summary>
        /// Method <c>deleteModData_Click</c> Delete operation for the ModData pack folder
        /// </summary>
        private void deleteModData_Click(object sender, RoutedEventArgs e)
        {
            Pack selectedPack = ((Button)sender).DataContext as Pack;

            MessageBoxResult result = FrostyMessageBox.Show("Do you want to delete pack \"" + selectedPack.Name + "\"?", "Frosty Mod Manager", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.Delete(selectedPack.Path, true);
                }
                catch (IOException)
                {
                    System.Threading.Tasks.Task.Run(() => {
                        FrostyMessageBox.Show("Error deleting Pack!\nTry running Frosty as Administrator.", "Frosty Mod Manager", MessageBoxButton.OK);
                    });
                }
            }   
            listPacks();
        }

        /// <summary>
        /// Method <c>openModData_Click</c> Open ModData pack folder
        /// </summary>
        private void openModData_Click(object sender, RoutedEventArgs e)
        {
            Pack selectedPack = ((Button)sender).DataContext as Pack;
            Process.Start(selectedPack.Path);
        }

        /// <summary>
        /// Method <c>launchModData_Click</c> Attempts to launch game with existing ModData pack folder
        /// </summary>
        private void launchModData_Click(object sender, RoutedEventArgs e)
        {
            Pack selectedPack = ((Button)sender).DataContext as Pack;
            FrostyModExecutor.ExecuteProcess($"{Path.GetDirectoryName(getModDataPath())}\\{ProfilesLibrary.ProfileName}.exe", $"-dataPath \"{selectedPack.Path.Trim('\\')}\"");
        }
    }
}
