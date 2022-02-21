using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LocalizedStringPlugin
{ 
    [TemplatePart(Name = PART_ExportButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_LocalizedString, Type = typeof(TextBox))]
    class FrostyLocalizedStringViewer : FrostyBaseEditor
    {
        public override ImageSource Icon => LocalizedStringViewerMenuExtension.imageSource;

        private const string PART_ExportButton = "PART_ExportButton";
        private const string PART_LocalizedString = "PART_LocalizedString";

        private const string PART_StringIdList = "PART_StringIdList";

        private TextBox tbLocalizedString;
        private Button btnExport;

        private ListBox stringIdListBox;

        private List<uint> stringIds = new List<uint>();
        private int currentIndex = 0;
        private bool firstTimeLoad = true;
        private ILogger logger;

        static FrostyLocalizedStringViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyLocalizedStringViewer), new FrameworkPropertyMetadata(typeof(FrostyLocalizedStringViewer)));
        }

        public FrostyLocalizedStringViewer(ILogger inLogger)
        {
            logger = inLogger;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            stringIdListBox = GetTemplateChild(PART_StringIdList) as ListBox;

            tbLocalizedString = GetTemplateChild(PART_LocalizedString) as TextBox;

            btnExport = GetTemplateChild(PART_ExportButton) as Button;

            stringIdListBox.SelectionChanged += stringIdListbox_SelectionChanged;
            btnExport.Click += PART_ExportButton_Click;

            Loaded += FrostyLocalizedStringViewer_Loaded;
        }

        private void FrostyLocalizedStringViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                FrostyTaskWindow.Show("Loading strings", "", (task) =>
                {
                    stringIds = LocalizedStringDatabase.Current.EnumerateStrings().ToList();
                    stringIds.Sort();
                });
                firstTimeLoad = false;
            }

            if (stringIds.Count == 0)
            {
                btnExport.IsEnabled = false;
                return;
            }

            foreach (uint stringId in stringIds)
                stringIdListBox.Items.Add(stringId.ToString("X8") + " - " + LocalizedStringDatabase.Current.GetString(stringId));
        }

        private void stringIdListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateLocalizedString(stringIds[stringIdListBox.SelectedIndex].ToString("X8"));
        }

        private void PopulateLocalizedString(string stringText)
        {
            stringText = stringText.ToLower();
            if (stringText.StartsWith("id_"))
            {
                tbLocalizedString.Text = LocalizedStringDatabase.Current.GetString(stringText);
                return;
            }

            if (!uint.TryParse(stringText, System.Globalization.NumberStyles.HexNumber, null, out uint value))
            {
                //tbStringId.Text = "";
                tbLocalizedString.Text = "";
                return;
            }

            tbLocalizedString.Text = LocalizedStringDatabase.Current.GetString(value);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (stringIds.Count == 0)
                return;

            currentIndex--;
            if (currentIndex < 0)
                currentIndex = stringIds.Count - 1;
            PopulateLocalizedString(stringIds[currentIndex].ToString("X8"));
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (stringIds.Count == 0)
                return;

            currentIndex++;
            if (currentIndex > stringIds.Count - 1)
                currentIndex = 0;
            PopulateLocalizedString(stringIds[currentIndex].ToString("X8"));
        }

        private uint HashStringId(string stringId)
        {
            uint result = 0xFFFFFFFF;
            for (int i = 0; i < stringId.Length; i++)
                result = stringId[i] + 33 * result;
            return result;
        }

        private void PART_ExportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Localized Strings", "*.csv (CSV File)|*.csv", "LocalizedStrings");
            if (sfd.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting Localized Strings", "", (task) =>
                {
                    using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create), false, true))
                    {
                        int index = 0;
                        foreach (uint stringId in stringIds)
                        {
                            string str = LocalizedStringDatabase.Current.GetString(stringId);

                            str = str.Replace("\r", "");
                            str = str.Replace("\n", " ");

                            writer.WriteLine(stringId.ToString("X8") + ",\"" + str + "\"");
                            task.Update(progress: ((index++) / (double)stringIds.Count) * 100.0);
                        }
                    }
                });

                App.Logger.Log("Localized strings saved to {0}", sfd.FileName);
            }
        }
    }
}
