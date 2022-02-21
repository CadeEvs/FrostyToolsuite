using System.Windows;
using Frosty.Controls;
using Frosty.Core;

namespace FsLocalizationPlugin
{
    /// <summary>
    /// Interaction logic for AddProfileWindow.xaml
    /// </summary>
    public partial class AddStringWindow : FrostyDockableWindow
    {
        public string ProfileName { get; set; }

        public AddStringWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            FsLocalizationStringDatabase db = LocalizedStringDatabase.Current as FsLocalizationStringDatabase;
            db.AddString(varIdTextBox.Text, varValueTextBox.Text);

            App.Logger.Log("String with ID '{0}' added to localized string database", varIdTextBox.Text);

            DialogResult = true;
            Close();
        }
    }
}
