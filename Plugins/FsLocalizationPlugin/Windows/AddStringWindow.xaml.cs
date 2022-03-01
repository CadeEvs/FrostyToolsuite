using System;
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

        public FsLocalizationStringDatabase db = LocalizedStringDatabase.Current as FsLocalizationStringDatabase;

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
        private uint HashStringId(string stringId)
        {
            uint result = 0xFFFFFFFF;
            for (int i = 0; i < stringId.Length; i++)
                result = stringId[i] + 33 * result;
            return result;
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {
            varHashTextBox.Text = "0x" + ((uint)rand.Next(1 << 30)).ToString("x").PadLeft(8, '0');
        }
        private static Random rand = new Random();
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            string HashOrID = varIdTextBox.Text;
            if (HashOrID.Length == 0)
            {
                HashOrID = Remove0X(varHashTextBox.Text);
                try
                {
                    uint result = Convert.ToUInt32(HashOrID, 16);
                    if (db.GetString(result) != "Invalid StringId: " + result.ToString("X8"))
                    {
                        string PreviousString = db.GetString(result);
                        db.SetString(result, varValueTextBox.Text);
                        //App.Logger.Log(string.Format("Replaced Value of String Hash: 0x{0}, with Value: \"{1}\", Previously: \"{2}\".", HashOrID, varValueTextBox.Text, PreviousString));
                    }
                    else
                    {
                        db.SetString(result, varValueTextBox.Text);
                        //App.Logger.Log(string.Format("Added String Hash: 0x{0}, with Value: \"{1}\".", HashOrID, varValueTextBox.Text));
                    }
                }
                catch
                {
                    App.Logger.Log("0x" + HashOrID + " is not a valid string hash.");
                }
            }
            else
            {
                uint result = HashStringId(HashOrID);
                if (db.GetString(result) != "Invalid StringId: " + result.ToString("X8"))
                {
                    string PreviousString = db.GetString(result);
                    db.SetString(result, varValueTextBox.Text);
                    //App.Logger.Log(string.Format("Replaced Value of String ID: \"{3}\", Hash: 0x{0}, with Value: \"{1}\", Previously: \"{2}\".", result.ToString("X8"), varValueTextBox.Text, PreviousString, HashOrID));
                }
                else
                {
                    db.SetString(result, varValueTextBox.Text);
                    //App.Logger.Log(string.Format("Added String ID: \"{2}\", Hash: 0x{0}, with Value: \"{1}\".", result.ToString("X8"), varValueTextBox.Text, HashOrID));
                }
            }

            DialogResult = true;
            Close();
        }

        public bool LazySolution = true;
        private void varHashTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (LazySolution == true)
            {
                LazySolution = false;
                varIdTextBox.Text = "";
                LazySolution = true;
            }
            try
            {
                string MyHash = Remove0X(varHashTextBox.Text);
                uint result = Convert.ToUInt32(MyHash, 16);
                if (db.GetString(result) != "Invalid StringId: " + result.ToString("X8"))
                {
                    varCurrentValueTextBox.Text = db.GetString(result);
                }
                else
                {
                    varCurrentValueTextBox.Text = "No matching hash found in localisation database.";
                }
            }
            catch
            {
                varCurrentValueTextBox.Text = "Invalid hash input.";
            }
        }

        public string Remove0X(string Hash)
        {
            if (Hash.StartsWith("0x"))
            {
                return Hash.Remove(0, 2);
            }
            return Hash;
        }
        private void varIdTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (LazySolution == true)
            {
                LazySolution = false;
                varHashTextBox.Text = "0x" + HashStringId(varIdTextBox.Text).ToString("x");
                LazySolution = true;
            }
        }
    }
}
