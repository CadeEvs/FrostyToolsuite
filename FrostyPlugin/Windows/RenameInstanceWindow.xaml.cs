using Frosty.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frosty.Core.Windows
{
    /// <summary>
    /// Interaction logic for RenameInstanceWindow.xaml
    /// </summary>
    public partial class RenameInstanceWindow : FrostyDockableWindow
    {
        public string InstanceName { get; private set; } = "";
        public RenameInstanceWindow(string currentName)
        {
            InitializeComponent();
            instanceNameTextBox.Text = currentName;
        }

        private void InstanceNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            doneButton.IsEnabled = !string.IsNullOrEmpty(instanceNameTextBox.Text);
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            InstanceName = instanceNameTextBox.Text;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void InstanceNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(instanceNameTextBox.Text))
                    return;
                DialogResult = true;
                InstanceName = instanceNameTextBox.Text;
                Close();
            }
        }
    }
}
