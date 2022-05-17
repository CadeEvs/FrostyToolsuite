using Frosty.Controls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace FrostyModManager
{
    public class TextExtension : MarkupExtension
    {
        private readonly string textFile;
        public TextExtension(string inTextFile)
        {
            textFile = inTextFile;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Uri uri = new Uri("pack://application:,,,/" + textFile);
            using (Stream stream = Application.GetResourceStream(uri).Stream)
            {
                using (TextReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : FrostyWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            versionTextBox.Text = "Version " + Frosty.Core.App.Version;
        }

        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
