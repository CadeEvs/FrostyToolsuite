using System.Windows;
using Frosty.Controls;
using Frosty.Core;

namespace FrostyModManager
{
    /// <summary>
    /// Interaction logic for AddProfileWindow.xaml
    /// </summary>
    public partial class LaunchOptionsWindow : FrostyDockableWindow
    {
        public string ProfileName { get; set; }

        public LaunchOptionsWindow()
        {
            InitializeComponent();

            Window mainWin = Application.Current.MainWindow;
            if (mainWin != null)
            {
                double x = mainWin.Left + (mainWin.Width / 2.0);
                double y = mainWin.Top + (mainWin.Height / 2.0);

                Left = x - (Width / 2.0);
                Top = y - (Height / 2.0);
            }

            cmdArgsTextBox.Focus();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Add("AdditionalArgs", cmdArgsTextBox.Text, ConfigScope.Game);
            //Config.Add("Application", "AdditionalArgs", cmdArgsTextBox.Text);
            DialogResult = true;
            Close();
        }
    }
}
