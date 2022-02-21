using Frosty.Controls;
using System.Windows.Navigation;

namespace FrostyEditor.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : FrostyDockableWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            versionTextBox.Text = "Version " + App.Version;
        }

        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
