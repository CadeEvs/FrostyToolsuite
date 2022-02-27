using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Frosty.Controls;
using Frosty.Core;

namespace FsLocalizationPlugin
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ReplaceMultipleStringWindow : FrostyDockableWindow
    {
        public string ProfileName { get; set; }

        public FsLocalizationStringDatabase db = LocalizedStringDatabase.Current as FsLocalizationStringDatabase;
        public ReplaceMultipleStringWindow()
        {
            InitializeComponent();
            varCaseComboBox.Items.Add("Only change exact match");
            varCaseComboBox.Items.Add("Change match of any case");
            varCaseComboBox.Items.Add("Smart case replacement");
            varCaseComboBox.SelectedIndex = 2;
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

        private static Random rand = new Random();
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            List<uint> TotalStrings = db.EnumerateStrings().Distinct().ToList();
            int count = 0;
            if (!(varCurrentValueTextBox.Text.Length == 0 & varEntireCheckBox.IsChecked == false))
            {
                foreach (uint stringid in TotalStrings)
                {
                    if (varCaseComboBox.SelectedIndex == 0)
                    {
                        if (db.GetString(stringid).Contains(varCurrentValueTextBox.Text))
                        {
                            count++;
                            if (varEntireCheckBox.IsChecked == false)
                            {
                                db.SetString(stringid, db.GetString(stringid).Replace(varCurrentValueTextBox.Text, varValueTextBox.Text));
                            }
                            else
                            {
                                db.SetString(stringid, varValueTextBox.Text);
                            }
                        }
                    }
                    else if (varCaseComboBox.SelectedIndex == 1)
                    {
                        if (db.GetString(stringid).ToLower().Contains(varCurrentValueTextBox.Text.ToLower()))
                        {
                            count++;
                            if (varEntireCheckBox.IsChecked == false)
                            {
                                db.SetString(stringid, Regex.Replace(db.GetString(stringid), varCurrentValueTextBox.Text, varValueTextBox.Text, RegexOptions.IgnoreCase));
                            }
                            else
                            {
                                db.SetString(stringid, varValueTextBox.Text);
                            }
                        }
                    }
                    else if (varCaseComboBox.SelectedIndex == 2)
                    {
                        if (db.GetString(stringid).ToLower().Contains(varCurrentValueTextBox.Text.ToLower()))
                        {
                            count++;
                            if (varEntireCheckBox.IsChecked == false)
                            {
                                string EditedString = db.GetString(stringid).Replace(varCurrentValueTextBox.Text.ToLower(), "{LOWER TO BE REPLACED DONT INPUT THIS}").Replace(varCurrentValueTextBox.Text.ToUpper(), "{UPPER TO BE REPLACED DONT INPUT THIS}");
                                EditedString = Regex.Replace(EditedString, varCurrentValueTextBox.Text, varValueTextBox.Text, RegexOptions.IgnoreCase);
                                db.SetString(stringid, EditedString.Replace("{LOWER TO BE REPLACED DONT INPUT THIS}", varValueTextBox.Text.ToLower()).Replace("{UPPER TO BE REPLACED DONT INPUT THIS}", varValueTextBox.Text.ToUpper()));
                            }
                            else
                            {
                                db.SetString(stringid, varValueTextBox.Text);
                            }
                        }
                    }
                }
            }

            App.Logger.Log(string.Format("Replaced {0} instances of \"{1}\" with \"{2}\".", count, varCurrentValueTextBox.Text, varValueTextBox.Text));
            DialogResult = true;
            Close();
        }
    }
}
