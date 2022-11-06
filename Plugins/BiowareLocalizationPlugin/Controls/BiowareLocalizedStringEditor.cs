using BiowareLocalizationPlugin.ExportImport;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BiowareLocalizationPlugin.Controls
{

    /// <summary>
    /// This is basically a copy of the FrostyLocalizedStringViewer, with some added functionality
    /// </summary>
    [TemplatePart(Name = PART_LocalizedString, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_StringIdList, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_hexSearchCB, Type = typeof(CheckBox))]
    [TemplatePart(Name = PART_Searchfield, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_SearchButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_SearchTextButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ModifiedOnlyCB, Type = typeof(CheckBox))]
    [TemplatePart(Name = PART_UpdateTextIdFieldCB, Type = typeof(CheckBox))]
    [TemplatePart(Name = PART_ShowTextInfo, Type = typeof(Button))]
    [TemplatePart(Name = PART_AddEdit, Type = typeof(Button))]
    [TemplatePart(Name = PART_Remove, Type = typeof(Button))]
    [TemplatePart(Name = PART_LanguageSelector, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_RefreshButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_Export, Type = typeof(Button))]
    [TemplatePart(Name = PART_Import, Type = typeof(Button))]
    class BiowareLocalizedStringEditor : FrostyBaseEditor
    {

        private const string PART_LocalizedString = "PART_LocalizedString";

        private const string PART_StringIdList = "PART_StringIdList";

        private const string PART_hexSearchCB = "PART_hexSearchCB";
        private const string PART_Searchfield = "PART_Searchfield";
        private const string PART_SearchButton = "PART_SearchButton";
        private const string PART_SearchTextButton = "PART_SearchTextButton";
        private const string PART_ModifiedOnlyCB = "PART_ModifiedOnlyCB";
        private const string PART_UpdateTextIdFieldCB = "PART_UpdateTextIdFieldCB";

        private const string PART_ShowTextInfo = "PART_ShowTextInfo";

        private const string PART_AddEdit = "PART_AddEdit";

        private const string PART_Remove = "PART_Remove";

        private const string PART_LanguageSelector = "PART_LanguageSelector";
        private const string PART_RefreshButton = "PART_RefreshButton";

        private const string PART_Export = "PART_Export";
        private const string PART_Import = "PART_Import";

        //#############################################

        // TODO ReplaceAll function?

        private CheckBox searchHexIdCB;

        private TextBox localizedStringTb;

        private ListBox stringIdListBox;

        private TextBox searchfieldTb;

        private CheckBox modifiedOnlyCB;

        private CheckBox updateTextIdFieldCB;

        private Button textInfoBt;

        private Button removeButton;

        private ComboBox languageSelectorCb;

        private List<uint> _textIdsList = new List<uint>();

        /// <summary>
        /// The text db instance, stored as variable for convenience
        /// </summary>
        private readonly BiowareLocalizedStringDatabase _textDB;

        private string _selectedLanguageFormat;

        /// <summary>
        /// handler to support closing child windows if this editor is closed.
        /// </summary>
        private readonly ClosingHandler _closingHandler;

        private bool _firstTimeInitialization = true;

        static BiowareLocalizedStringEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiowareLocalizedStringEditor), new FrameworkPropertyMetadata(typeof(BiowareLocalizedStringEditor)));
        }

        public BiowareLocalizedStringEditor(BiowareLocalizedStringDatabase textDb)
        {
            _textDB = textDb;
            _selectedLanguageFormat = textDb.DefaultLanguage;
            _closingHandler = new ClosingHandler();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            stringIdListBox = GetTemplateChild(PART_StringIdList) as ListBox;
            stringIdListBox.SelectionChanged += StringIdListbox_SelectionChanged;

            localizedStringTb = GetTemplateChild(PART_LocalizedString) as TextBox;

            searchHexIdCB = GetTemplateChild(PART_hexSearchCB) as CheckBox;
            searchHexIdCB.Checked += SearchFieldFormatChangedToHex;
            searchHexIdCB.Unchecked += SearchFieldFormatChangedToDecimal;

            searchfieldTb = GetTemplateChild(PART_Searchfield) as TextBox;
            searchfieldTb.PreviewKeyDown += SearchFieldActualized;
            Button btSearchButton = GetTemplateChild(PART_SearchButton) as Button;
            btSearchButton.Click += SearchButtonClicked;

            Button btSearchTextButton = GetTemplateChild(PART_SearchTextButton) as Button;
            btSearchTextButton.Click += ShowSearchDialog;

            modifiedOnlyCB = GetTemplateChild(PART_ModifiedOnlyCB) as CheckBox;
            modifiedOnlyCB.Click += ReLoadTexts;

            updateTextIdFieldCB = GetTemplateChild(PART_UpdateTextIdFieldCB) as CheckBox;

            textInfoBt = GetTemplateChild(PART_ShowTextInfo) as Button;
            textInfoBt.IsEnabled = false; // initially disabled until a text is selected
            textInfoBt.Click += ShowTextInfo;

            Button addButton = GetTemplateChild(PART_AddEdit) as Button;
            addButton.Click += ShowAddEditWindow;

            removeButton = GetTemplateChild(PART_Remove) as Button;
            removeButton.IsEnabled = false; // initially disabled until a text is selected
            removeButton.Click += Remove;

            Button refreshButton = GetTemplateChild(PART_RefreshButton) as Button;
            refreshButton.Click += ReLoadTexts;

            languageSelectorCb = GetTemplateChild(PART_LanguageSelector) as ComboBox;
            languageSelectorCb.ItemsSource = _textDB.GellAllLanguages();
            languageSelectorCb.SelectedItem = _selectedLanguageFormat;
            languageSelectorCb.SelectionChanged += SelectLanguage;

            Button exportButton = GetTemplateChild(PART_Export) as Button;
            exportButton.Click += Export;

            Button importButton = GetTemplateChild(PART_Import) as Button;
            importButton.Click += Import;

            Loaded += LoadFirstTime;

        }

        private void LoadFirstTime(object sender, RoutedEventArgs e)
        {
            if (_firstTimeInitialization)
            {
                LoadTexts(sender, e);
                _firstTimeInitialization = false;
            }
        }

        private void LoadTexts(object sender, RoutedEventArgs e)
        {

            bool? nullableModifiedOnly = modifiedOnlyCB.IsChecked;
            bool modifiedOnly = nullableModifiedOnly.HasValue && nullableModifiedOnly.Value;

            FrostyTaskWindow.Show("Loading texts", "", (task) =>
            {

                if (modifiedOnly)
                {
                    _textIdsList = _textDB.GetAllModifiedTextsIds(_selectedLanguageFormat).ToList();
                }
                else
                {
                    _textIdsList = _textDB.GetAllTextIds(_selectedLanguageFormat).ToList();
                }

                _textIdsList.Sort();
            });

            if (_textIdsList.Count == 0)
            {
                return;
            }

            foreach (uint textId in _textIdsList)
            {
                stringIdListBox.Items.Add(textId.ToString("X8") + " - " + _textDB.GetText(_selectedLanguageFormat, textId));
            }
        }

        private void StringIdListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint selectedTextId = GetCurrentStringId();
            PopulateLocalizedString(selectedTextId);

            bool isTextSelected = e.AddedItems.Count > 0;
            textInfoBt.IsEnabled = isTextSelected;
            removeButton.IsEnabled = isTextSelected;

            if (isTextSelected && updateTextIdFieldCB.IsChecked == true)
            {
                SetTextIdInSearchField(selectedTextId);
            }
        }

        private void SetTextIdInSearchField(uint textId)
        {
            string textIdFormat = (searchHexIdCB.IsChecked == true) ? "X8" : "D";
            searchfieldTb.Text = textId.ToString(textIdFormat);
        }

        private void PopulateLocalizedString(uint textId)
        {
            localizedStringTb.Text = _textDB.GetText(_selectedLanguageFormat, textId);
        }

        void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void SearchFieldActualized(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DoSearch();
            }
        }

        private void SearchFieldFormatChangedToHex(object sender, RoutedEventArgs e)
        {
            GetUpdateFromTextField(NumberStyles.Number, textId => SetTextIdInSearchField(textId));
        }

        private void SearchFieldFormatChangedToDecimal(object sender, RoutedEventArgs e)
        {
            GetUpdateFromTextField(NumberStyles.HexNumber, textId => SetTextIdInSearchField(textId));
        }

        private void GetUpdateFromTextField(NumberStyles style, Action<uint> textIdAction)
        {
            string stringIdAsText = searchfieldTb.Text;

            bool canRead = uint.TryParse(stringIdAsText, style, null, out uint textId);
            if (canRead)
            {
                textIdAction(textId);
            }
            else
            {
                App.Logger.LogWarning("Bad Input! Cannot read <{0}> as {1} formatted number for text Id", stringIdAsText, style.ToString());
            }
        }


        /// <summary>
        /// Searches the list of string ids for the text id (assumed a hexadecimal value!) given in the search box.
        /// </summary>
        private void DoSearch()
        {
            NumberStyles style = (searchHexIdCB.IsChecked == true) ? NumberStyles.HexNumber : NumberStyles.Integer;
            GetUpdateFromTextField(style, textId => SearchTextId(textId));
        }

        private void SearchTextId(uint textId)
        {
            int index = _textIdsList.IndexOf(textId);
            if (index < 0 && index < stringIdListBox.Items.Count)
            {
                stringIdListBox.UnselectAll();
                return;
            }

            stringIdListBox.SelectedIndex = index;
            stringIdListBox.ScrollIntoView(stringIdListBox.SelectedItem);

            SetTextIdInSearchField(textId);
        }

        /// <summary>
        /// Opens another window that details extra information about the text that is not normally necessary to have.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTextInfo(object sender, RoutedEventArgs e)
        {
            uint stringId = GetCurrentStringId();

            TextInfoWindow infoWindow = new TextInfoWindow
            {
                Owner = Application.Current.MainWindow
            };
            infoWindow.Init(_selectedLanguageFormat, stringId, _textDB);
            infoWindow.Show();

            _closingHandler.AddChildWindow(infoWindow);
        }

        /// <summary>
        /// Returns the id of the currently selected text, or zero 0, if no text is currently selected.
        /// </summary>
        /// <returns></returns>
        private uint GetCurrentStringId()
        {
            int selectedIndex = stringIdListBox.SelectedIndex;
            return selectedIndex >= 0 && selectedIndex < _textIdsList.Count ? _textIdsList[selectedIndex] : 0;
        }

        /// <summary>
        /// Shows the edit window, taking the results into the currently displayed entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAddEditWindow(object sender, RoutedEventArgs e)
        {

            uint stringId = GetCurrentStringId();
            AddEditWindow editWindow = new AddEditWindow(_textDB, _selectedLanguageFormat)
            {
                Owner = Application.Current.MainWindow
            };
            editWindow.Init(stringId);

            bool? save = editWindow.ShowDialog();
            if (save.HasValue && save.Value)
            {
                Tuple<uint, string> saveValue = editWindow.SaveValue;

                // textId is not necessarily the stringId originally given to the dialog!
                uint textId = saveValue.Item1;
                string text = saveValue.Item2;

                string entry = textId.ToString("X8") + " - " + text;

                int entryIndex = _textIdsList.IndexOf(textId);

                if (entryIndex < 0)
                {
                    stringIdListBox.Items.Add(entry);
                    _textIdsList.Add(textId);
                }
                else
                {
                    stringIdListBox.Items[entryIndex] = entry;
                }

                SearchTextId(textId);
            }
        }

        /// <summary>
        /// Removes / Reverts the selected text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove(object sender, RoutedEventArgs e)
        {

            int index = stringIdListBox.SelectedIndex;

            if (index < 0 || index >= _textIdsList.Count)
            {
                // not sure how this should be possible...
                App.Logger.LogWarning("Entered impossible state <Remove on no item selected>: Remove Operation did not complete!");
                return;
            }

            uint textId = _textIdsList[index];

            _textDB.RevertText(_selectedLanguageFormat, textId);

            string text = _textDB.FindText(_selectedLanguageFormat, textId);

            if (text != null)
            {
                string entry = textId.ToString("X8") + " - " + text;
                stringIdListBox.Items[index] = entry;
            }
            else
            {
                stringIdListBox.Items.RemoveAt(index);
                _textIdsList.RemoveAt(index);
            }

            SearchTextId(textId);
        }

        private void SelectLanguage(object sender, SelectionChangedEventArgs e)
        {

            string newLanguageFormat = (string)languageSelectorCb.SelectedItem;

            if (!_selectedLanguageFormat.Equals(newLanguageFormat))
            {
                _selectedLanguageFormat = newLanguageFormat;

                ReLoadTexts(sender, e);
            }
        }

        private void ReLoadTexts(object sender, RoutedEventArgs e)
        {
            _textIdsList.Clear();
            stringIdListBox.Items.Clear();

            LoadTexts(sender, e);
        }

        private void ShowSearchDialog(object sender, RoutedEventArgs e)
        {

            if (stringIdListBox != null && stringIdListBox.Items.Count > 0)
            {
                SearchFindWindow searchWindow = new SearchFindWindow(stringIdListBox);
                searchWindow.Show();

                _closingHandler.AddChildWindow(searchWindow);
            }
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            XmlExporter.Export(_textDB, _selectedLanguageFormat);
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            XmlImporter.Import(_textDB);

            ReLoadTexts(sender, e);
        }

        public override void Closed()
        {
            _closingHandler.OnEditorClose();
        }

        #region -- Closing Handler --
        internal class ClosingHandler
        {

            private readonly List<Window> nonModalChildren = new List<Window>();

            public void AddChildWindow(Window nonModalWindow)
            {
                nonModalChildren.Add(nonModalWindow);
                nonModalWindow.Closed += OnChildClose;
            }

            public void OnChildClose(object sender, EventArgs e)
            {
                nonModalChildren.Remove((Window)sender);
            }

            public void OnEditorClose()
            {
                foreach (Window childWindow in nonModalChildren)
                {
                    childWindow.Closed -= OnChildClose;
                    childWindow.Close();
                }
            }
        }
        #endregion

    }
}
