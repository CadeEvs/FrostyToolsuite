using BiowareLocalizationPlugin.ExportImport;
using Frosty.Controls;
using FrostySdk;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Application = System.Windows.Application;

namespace BiowareLocalizationPlugin.Controls
{
    /// <summary>
    /// Interaktionslogik für ImportTargetDialog.xaml
    /// </summary>
    public partial class ImportTargetDialog : FrostyDockableWindow
    {

        /// <summary>
        /// Both ME:A and DA:I have this with the language as part of their (or most) text resources.
        /// </summary>
        private static readonly string TEXTTABLE_PATTERN = "/texttable/[a-z]+/";
        private static readonly string TEXTTABLE_EN_PATH = "/texttable/en/";

        // These resources change name depending on whether english or any other localization is used.
        private static readonly string GLOBALMASTER = "globalmaster";
        private static readonly string GLOBALTRANSLATED = "globaltranslated";

        public TextFile SaveValue { get; set; }

        public ObservableCollection<ResourceRow> GridSource = new ObservableCollection<ResourceRow>();

        public List<string> TargetResourceList { get; } = new List<string>();

        private readonly BiowareLocalizedStringDatabase _textDb;
        private readonly TextFile _importTextFile;

        private string _selectedImportLanguageFormat;

        public ImportTargetDialog(BiowareLocalizedStringDatabase textDB, TextFile textFile)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            _textDb = textDB;
            _importTextFile = textFile;

            // TODO disable import button while not all texts are set!

            InitLanguage(_importTextFile.LanguageFormat);
            InitResources();
        }

        private void InitLanguage(string importLanguage)
        {
            languageTextBox.Text = importLanguage;

            var availableLanguages = _textDb.GellAllLanguages();
            languageSelector.ItemsSource = availableLanguages;

            languageSelector.SelectionChanged += LanguageFormatChanged;

            // index of uses compare, contains equality
            int languageIndex = availableLanguages.ToList().IndexOf(importLanguage);
            if (languageIndex >= 0)
            {
                languageSelector.SelectedIndex = languageIndex;
            }
        }

        private void InitResources()
        {
            List<string> importResourcesList = GetTextResources(_importTextFile);

            FillTargetResourceList();

            string targetTextTablePath = GetTargetTexttableSubstring();

            GridSource.Clear();
            foreach (string importResourceName in importResourcesList)
            {

                GridSource.Add(new ResourceRow()
                {
                    TextResource = importResourceName,
                    TargetResource = GetTargetResourceFor(importResourceName, targetTextTablePath)
                });
            }

            datagrid.ItemsSource = GridSource;
            targetResources.ItemsSource = TargetResourceList;
        }

        private static List<string> GetTextResources(TextFile textFile)
        {
            HashSet<string> importResourcesSet = new HashSet<string>();
            foreach (var text in textFile.Texts)
            {
                importResourcesSet.UnionWith(text.Resources);
            }

            if (textFile.DeclinatedAdjectives != null)
            {
                foreach (var adjectiveEntry in textFile.DeclinatedAdjectives)
                {
                    importResourcesSet.Add(adjectiveEntry.Resource);
                }
            }

            List<string> importResourcesList = importResourcesSet.ToList();
            importResourcesList.Sort();
            return importResourcesList;
        }

        private void FillTargetResourceList()
        {
            TargetResourceList.Clear();

            if (_selectedImportLanguageFormat != null)
            {
                TargetResourceList.AddRange(GetTargetResourceList(_selectedImportLanguageFormat));
            }
        }

        private List<string> GetTargetResourceList(string languageFormat)
        {
            return _textDb.GetAllResourceNames(languageFormat).ToList();
        }

        private string GetTargetResourceFor(string importResource, string targetTextTablePath)
        {
            if (TargetResourceList.Count > 0)
            {
                // indexOf uses equals instead of identity compare - which should result in less false negatives
                int index = TargetResourceList.IndexOf(importResource);
                if (index >= 0)
                {
                    return TargetResourceList[index];
                }

                // MEA and DAI have the localization files (or most of them) within paths such as /dlc/../texttable/{language_short}/...
                // -> Try to replace substring '/texttable/{language_short}/' with whatever is appropriate...
                string importTextTablePath = GetTextTableSubString(importResource);
                if (importTextTablePath != null && targetTextTablePath != null)
                {
                    return FindWithReplacedPathInfo(importResource, importTextTablePath, targetTextTablePath);
                }
            }

            return null;
        }

        private string GetTargetTexttableSubstring()
        {
            if (TargetResourceList.Count > 0)
            {
                var firstEntry = TargetResourceList[0];
                return GetTextTableSubString(firstEntry);
            }
            return null;
        }

        private static string GetTextTableSubString(string textDonor)
        {
            Match match = Regex.Match(textDonor, TEXTTABLE_PATTERN);
            return match.Success ? match.Value : null;
        }

        private string FindWithReplacedPathInfo(string importResource, string importTextTablePath, string targetTextTablePath)
        {
            string targetResource = importResource.Replace(importTextTablePath, targetTextTablePath);

            // globalmaster resources in english are named globaltranslated for other languages
            // for ME:A the globaltranslated resources are also in a subfolder, whereas globalmaster is not!
            if (importResource.Contains(GLOBALMASTER) && !targetTextTablePath.Equals(TEXTTABLE_EN_PATH))
            {
                targetResource = targetResource.Replace(GLOBALMASTER, GLOBALTRANSLATED);

                if (ProfilesLibrary.DataVersion == ((int)ProfileVersion.MassEffectAndromeda))
                {
                    targetResource = targetResource.Replace("/game/globaltranslated", "/game/localization/config/globaltranslated");
                }
            }
            else if (importResource.Contains(GLOBALTRANSLATED) && targetTextTablePath.Equals(TEXTTABLE_EN_PATH))
            {
                targetResource = targetResource.Replace(GLOBALTRANSLATED, GLOBALMASTER);

                if (ProfilesLibrary.DataVersion == ((int)ProfileVersion.MassEffectAndromeda))
                {
                    targetResource = targetResource.Replace("/game/localization/config/globalmaster", "/game/globalmaster");
                }
            }

            int index = TargetResourceList.IndexOf(targetResource);

            return index >= 0 ? TargetResourceList[index] : null;
        }

        public void LanguageFormatChanged(object sender, RoutedEventArgs e)
        {
            _selectedImportLanguageFormat = (string)languageSelector.SelectedItem;

            FillTargetResourceList();

            string targetTextTablePath = GetTargetTexttableSubstring();
            foreach (ResourceRow entry in GridSource)
            {
                entry.TargetResource = GetTargetResourceFor(entry.TextResource, targetTextTablePath);
            }
        }

        public void Import(object sender, RoutedEventArgs e)
        {

            Dictionary<string, string> resourceTranslation = new Dictionary<string, string>();
            foreach (ResourceRow resourceRow in GridSource)
            {

                if (resourceRow.TargetResource == null)
                {

                    string msg = string.Format("No target resource for <{0}> selected!", resourceRow.TextResource);
                    MessageBox.Show(msg, "Missing Entry", MessageBoxButton.OK);

                    return;
                }

                resourceTranslation.Add(resourceRow.TextResource, resourceRow.TargetResource);
            }

            TextFile updatedTarget = new TextFile()
            {
                LanguageFormat = _selectedImportLanguageFormat
            };


            TextRepresentation[] targetTextRepresentations = CreateTargetTextRepresentations(resourceTranslation);
            updatedTarget.Texts = targetTextRepresentations;

            DeclinatedAdjectiveRepresentation[] targetAdjectiveRepresentations = CreateTargetAdjectiveRepresentations(resourceTranslation);
            updatedTarget.DeclinatedAdjectives = targetAdjectiveRepresentations;

            SaveValue = updatedTarget;
            DialogResult = true;

            Close();
        }

        private TextRepresentation[] CreateTargetTextRepresentations(Dictionary<string, string> resourceTranslation)
        {
            List<TextRepresentation> targetRepresentations = new List<TextRepresentation>();
            foreach (TextRepresentation importRepresentation in _importTextFile.Texts)
            {
                TextRepresentation updatedRepresentation = new TextRepresentation()
                {
                    TextId = importRepresentation.TextId,
                    Text = importRepresentation.Text,
                    Resources = importRepresentation.Resources.Select(r => resourceTranslation[r]).ToArray()
                };

                targetRepresentations.Add(updatedRepresentation);
            }
            return targetRepresentations.ToArray();
        }

        private DeclinatedAdjectiveRepresentation[] CreateTargetAdjectiveRepresentations(Dictionary<string, string> resourceTranslation)
        {

            if (_importTextFile.DeclinatedAdjectives == null)
            {
                return null;
            }

            List<DeclinatedAdjectiveRepresentation> targetRepresentations = new List<DeclinatedAdjectiveRepresentation>();
            foreach (DeclinatedAdjectiveRepresentation importRepresentation in _importTextFile.DeclinatedAdjectives)
            {
                DeclinatedAdjectiveRepresentation updatedRepresentation = new DeclinatedAdjectiveRepresentation()
                {
                    Resource = resourceTranslation[importRepresentation.Resource],
                    AdjectiveId = importRepresentation.AdjectiveId,
                    Declinations = importRepresentation.Declinations
                };

                targetRepresentations.Add(updatedRepresentation);
            }
            return targetRepresentations.ToArray();
        }

        public void Abort(object sender, RoutedEventArgs e)
        {
            SaveValue = null;
            DialogResult = false;
            Close();
        }

    }

    #region -- ui data field stuff --
    public class ResourceRow : INotifyPropertyChanged
    {
        public string TextResource { get; set; }

        private string _targetResource;
        public string TargetResource
        {
            get { return _targetResource; }

            set
            {
                _targetResource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TargetResource)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    #endregion
}
