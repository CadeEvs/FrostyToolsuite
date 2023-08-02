using System;
using System.Windows;
using System.Windows.Controls;
using Frosty.Controls;
using Frosty.Core;

namespace FsLocalizationPlugin
{
    /// <summary>
    /// Interaction logic for AddProfileWindow.xaml
    /// </summary>
    public partial class ConvertIDWindow : FrostyDockableWindow
    {
        public string ProfileName { get; set; }

        public FsLocalizationStringDatabase db = LocalizedStringDatabase.Current as FsLocalizationStringDatabase;

        public ConvertIDWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            CheckLastResult();
        }
        private uint HashStringId(string stringId)
        {
            uint result = 0xFFFFFFFF;
            for (int i = 0; i < stringId.Length; i++)
                result = stringId[i] + 33 * result;
            return result;
        }

        private bool LazySolution = true;

        private string LastResult = null;

        private int? TryTimes = null;

        public string Remove0X(string Hash)
        {
            if (Hash.StartsWith("0x"))
            {
                return Hash.Remove(0, 2);
            }
            return Hash;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string hash = varHashTextBox.Text;
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            StringIDTextBox.Text = LastResult;
            LastResult = null;
            CheckLastResult();
        }

        private void varHashTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryTimes = null;
            StringIDTextBox.Text = null;
            LastResult = null;
            CheckLastResult();
            CheckGenerateButton();
            RefreshTryTimes();
            try
            {
                string MyHash = Remove0X(varHashTextBox.Text);
                uint result = Convert.ToUInt32(MyHash, 16);
                if (db.GetString(result) != "Invalid StringId: " + result.ToString("X8"))
                {
                    varCurrentValueTextBox.Text = db.GetString(result);
                    GenerateButton.IsEnabled = true;
                }
                else
                {
                    varCurrentValueTextBox.Text = "No matching hash found in localisation database.";
                    GenerateButton.IsEnabled = false;
                }
            }
            catch
            {
                varCurrentValueTextBox.Text = "Invalid hash input.";
                GenerateButton.IsEnabled = false;
            }
        }

        private void CheckLastResult()
        {
            if (LastResult == null)
            {
                RevertButton.IsEnabled = false;
            } else if (!(LastResult == null))
            {
                RevertButton.IsEnabled = true;
            }
        }

        private void CheckGenerateButton()
        {
            if (StringIDTextBox.Text == null)
            {
                GenerateButton.Content = "Generate next";
                GenerateButton.Width = 100;
            } else
            {
                GenerateButton.Content = "Generate";
                GenerateButton.Width = 75;
            }
        }

        private void RefreshTryTimes()
        {
            if (!TryTimes.HasValue)
            {
                TryTimesLable.Content = "Times: NULL";
            } 
            else
            {
                TryTimesLable.Content = "Times: " + TryTimes;
            }
        }
    }
}
