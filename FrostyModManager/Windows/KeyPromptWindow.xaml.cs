using Frosty.Controls;
using System.Windows;

namespace FrostyModManager.Windows
{
    /// <summary>
    /// Interaction logic for KeyPromptWindow.xaml
    /// </summary>
    public partial class KeyPromptWindow : FrostyDockableWindow
    {
        public byte[] EncryptionKey;

        public KeyPromptWindow()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            EncryptionKey = new byte[keyTextBox.Text.Length / 2];
            for (int i = 0; i < keyTextBox.Text.Length / 2; i++)
                EncryptionKey[i] = byte.Parse(keyTextBox.Text.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
