using BiowareLocalizationPlugin.ExportImport;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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
    [TemplatePart(Name = PART_ToggleDisplayTextsOrAdjectives, Type = typeof(Button))]
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
    [TemplatePart(Name = PART_ResourceSelector, Type = typeof(ComboBox))]
    class BiowareLocalizedStringEditor : FrostyBaseEditor
    {
        #region constants and variables
        private const string PART_LocalizedString = "PART_LocalizedString";

        private const string PART_StringIdList = "PART_StringIdList";

        private const string PART_ToggleDisplayTextsOrAdjectives = "PART_ToggleDisplayTextsOrAdjectives";
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

        private const string PART_ResourceSelector = "PART_ResourceSelector";

        //##############################################################################

        private Button toggleTextsOrAdjectivesButton;

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

        //##############################################################################
        // Below are fields and properties for supporting the adjectives in DA:I

        /// <summary>
        /// Enum for the types of strings to display.
        /// These can be the texts as defined in the text id block, or the declinated adjectives used for the generated names of crafted items in DA:I.
        /// Maybe even a combination of those, currently the text ids and adjective ids do not overlapp afaik.
        /// </summary>
        private enum DisplayType
        {

            /// <summary>
            /// Show only the texts - this is the defaul behaviour.
            /// </summary>
            SHOW_TEXTS,

            /// <summary>
            /// Show only the adjectives - this is useless for ME:A and very likely not often used for DA:I
            /// </summary>
            SHOW_DECLINATED_ADJECTIVES
        };

        /// <summary>
        /// What to display at this time
        /// </summary>
        private DisplayType _displayType = DisplayType.SHOW_TEXTS;

        private readonly string toggleDisplayButtonTextsString = "Search Text with ID:";
        private readonly string toggleDisplayButtonAdjectiveString = "Search Adjective with ID:";

        private readonly string toggleDisplayButtonTooltipTextsString = "Searches for text entries from the default text id space\r\n"
            + "Use this toggle to show only declinated adjectives used for names of crafted items in DA:I";

        private readonly string toggleDisplayButtonTooltipAdjectiveString = "Searches for declinated adjectives used for generating the names of crafted items in DA:I\r\n"
            + "Use this toggle to show only the normal texts used in both DA:I and ME:A";

        private const string SHOW_ALL_RESOURCES = "<Show All>";
        private ComboBox resourceSelectorCb;

        /// <summary>
        /// The last selected resource - Note that this can be null!
        /// </summary>
        private string _selectedResource = SHOW_ALL_RESOURCES;

        //##############################################################################
        #endregion constants and variables

        #region initialization

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

            toggleTextsOrAdjectivesButton = GetTemplateChild(PART_ToggleDisplayTextsOrAdjectives) as Button;
            toggleTextsOrAdjectivesButton.Content = toggleDisplayButtonTextsString;
            toggleTextsOrAdjectivesButton.ToolTip = toggleDisplayButtonTooltipTextsString;
            toggleTextsOrAdjectivesButton.Click += ToggleTextsOrAdjectives;

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
            modifiedOnlyCB.Click += ReLoadStrings;

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
            refreshButton.Click += ReLoadStrings;

            Button exportButton = GetTemplateChild(PART_Export) as Button;
            exportButton.Click += Export;

            Button importButton = GetTemplateChild(PART_Import) as Button;
            importButton.Click += Import;

            // listeners are active form the beginning, so we have to keep a certain order in here for the language and resource listeners.
            languageSelectorCb = GetTemplateChild(PART_LanguageSelector) as ComboBox;
            languageSelectorCb.ItemsSource = _textDB.GellAllLanguages();
            languageSelectorCb.SelectedItem = _selectedLanguageFormat;
            languageSelectorCb.SelectionChanged += SelectLanguage;

            resourceSelectorCb = GetTemplateChild(PART_ResourceSelector) as ComboBox;
            SetSelectableResources();
            resourceSelectorCb.SelectionChanged += SelectResource;

            Loaded += LoadFirstTime;

        }

        #endregion initialization

        #region data loading
        private void LoadFirstTime(object sender, RoutedEventArgs e)
        {
            if (_firstTimeInitialization)
            {
                LoadStrings(sender, e);
                _firstTimeInitialization = false;
            }
        }

        private void ReLoadStrings(object sender, RoutedEventArgs e)
        {
            _textIdsList.Clear();
            stringIdListBox.Items.Clear();

            LoadStrings(sender, e);
        }

        private void LoadStrings(object sender, RoutedEventArgs e)
        {

            bool? nullableModifiedOnly = modifiedOnlyCB.IsChecked;
            bool modifiedOnly = nullableModifiedOnly.HasValue && nullableModifiedOnly.Value;

            if (_selectedResource == null)
            {
                _textIdsList = new List<uint>();
                return;
            }

            switch (_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    LoadTexts0(modifiedOnly);
                    break;
                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    LoadAdjectives0(modifiedOnly);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown DisplayType: " + _displayType);
            }

            if (_textIdsList.Count == 0)
            {
                return;
            }
            stringIdListBox.ScrollIntoView(stringIdListBox.Items[0]);
        }

        private void LoadTexts0(bool modifiedOnly)
        {
            bool showTextsFromAllResources = SHOW_ALL_RESOURCES.Equals(_selectedResource);
            FrostyTaskWindow.Show("Loading texts", "", (task) =>
            {
                _textIdsList = LoadTextIds(modifiedOnly, showTextsFromAllResources);
            });

            _textIdsList.Sort();

            foreach (uint textId in _textIdsList)
            {
                stringIdListBox.Items.Add(textId.ToString("X8") + " - " + _textDB.GetText(_selectedLanguageFormat, textId));
            }
        }

        private List<uint> LoadTextIds(bool modifiedOnly, bool showTextsFromAllResources)
        {

            List<uint> textIds;

            if (!modifiedOnly && showTextsFromAllResources)
            {
                textIds = _textDB.GetAllTextIds(_selectedLanguageFormat).ToList();
            }
            else if (modifiedOnly && showTextsFromAllResources)
            {
                textIds = _textDB.GetAllModifiedTextsIds(_selectedLanguageFormat).ToList();
            }
            else if (modifiedOnly && !showTextsFromAllResources)
            {
                textIds = _textDB.GetAllModifiedTextIdsFromResource(_selectedLanguageFormat, _selectedResource).ToList();
            }
            else //if( !modifiedOnly && !showTextsFromAllResources)
            {
                textIds = _textDB.GetAllTextIdsFromResource(_selectedLanguageFormat, _selectedResource).ToList();
            }

            return textIds;
        }

        private void LoadAdjectives0(bool modifiedOnly)
        {

            FrostyTaskWindow.Show("Loading adjectives", "", (task) =>
            {
                _textIdsList = LoadAdjectiveIds(modifiedOnly);
            });

            _textIdsList.Sort();

            foreach (uint adjectiveId in _textIdsList)
            {
                List<string> adjectives = _textDB.GetDeclinatedAdjectives(_selectedLanguageFormat, _selectedResource, adjectiveId);

                string firstAdjective = adjectives.Count > 0 ? adjectives[0] : "";

                stringIdListBox.Items.Add(adjectiveId.ToString("X8") + " - " + firstAdjective);
            }
        }

        private List<uint> LoadAdjectiveIds(bool modifiedOnly)
        {
            if (modifiedOnly)
            {
                return _textDB.GetModifiedDeclinatedAdjectiveIdsFromResource(_selectedLanguageFormat, _selectedResource).ToList();
            }
            else
            {
                return _textDB.GetAllDeclinatedAdjectiveIdsFromResource(_selectedLanguageFormat, _selectedResource).ToList();
            }
        }

        /// <summary>
        /// Sets the list of currently applicable resources in the resource selector.
        /// </summary>
        private void SetSelectableResources()
        {

            resourceSelectorCb.Items.Clear();
            List<string> resourceNames = GetApplicableResources();

            foreach (string resourceName in resourceNames)
            {
                resourceSelectorCb.Items.Add(resourceName);
            }

            if (resourceNames.Count > 0)
            {
                resourceSelectorCb.SelectedItem = resourceSelectorCb.Items[0];
            }
            else
            {
                resourceSelectorCb.SelectedItem = null;
            }
        }

        /// <summary>
        /// Returns the list or resources that are selectable for the currently selected language and display type.
        /// </summary>
        /// <returns></returns>
        private List<string> GetApplicableResources()
        {

            if (_selectedLanguageFormat == null)
            {
                // we are either still in setup or something went wrong...
                return new List<string>();
            }

            List<string> resourceList = new List<string>();

            switch (_displayType)
            {
                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    resourceList.AddRange(_textDB.GetAllResourceNamesWithDeclinatedAdjectives(_selectedLanguageFormat));
                    break;

                case DisplayType.SHOW_TEXTS:
                default:
                    resourceList.Add(SHOW_ALL_RESOURCES);
                    resourceList.AddRange(_textDB.GetAllResourceNames(_selectedLanguageFormat));
                    break;
            }

            return resourceList;
        }

        #endregion data loading

        /// <summary>
        /// Returns the id of the currently selected text, or zero 0, if no text is currently selected.
        /// </summary>
        /// <returns></returns>
        private uint GetCurrentStringId()
        {
            int selectedIndex = stringIdListBox.SelectedIndex;
            return selectedIndex >= 0 && selectedIndex < _textIdsList.Count ? _textIdsList[selectedIndex] : 0;
        }

        #region string selection listeners

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

            string textToShow;
            switch (_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    textToShow = _textDB.GetText(_selectedLanguageFormat, textId);
                    break;

                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    textToShow = LoadDeclinatedAdjectiveToShow(textId);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown DisplayType: " + _displayType);
            }

            localizedStringTb.Text = textToShow;
        }

        private string LoadDeclinatedAdjectiveToShow(uint adjectiveId)
        {

            if (_selectedResource == null)
            {
                return "";
            }

            List<string> declinations = _textDB.GetDeclinatedAdjectives(_selectedLanguageFormat, _selectedResource, adjectiveId);

            StringBuilder displayBuilder = new StringBuilder();
            foreach (string declination in declinations)
            {
                displayBuilder.AppendLine(declination);
            }

            return displayBuilder.ToString();
        }

        #endregion string selection listeners

        #region top search bar button listeners

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

        /// <summary>
        /// Toggles the button and the displayed texts or adjectives.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleTextsOrAdjectives(object sender, RoutedEventArgs e)
        {
            string buttonText;
            string buttonTooltip;

            switch (_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    _displayType = DisplayType.SHOW_DECLINATED_ADJECTIVES;
                    buttonText = toggleDisplayButtonAdjectiveString;
                    buttonTooltip = toggleDisplayButtonTooltipAdjectiveString;
                    break;

                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                default:
                    _displayType = DisplayType.SHOW_TEXTS;
                    buttonText = toggleDisplayButtonTextsString;
                    buttonTooltip = toggleDisplayButtonTooltipTextsString;
                    break;
            }

            toggleTextsOrAdjectivesButton.Content = buttonText;
            toggleTextsOrAdjectivesButton.ToolTip = buttonTooltip;

            //ReLoadStrings(sender, e);
            SetSelectableResources();
        }

        private void GetUpdateFromTextField(NumberStyles style, Action<uint> textIdAction)
        {
            string stringIdAsText = searchfieldTb.Text;

            if (stringIdAsText == null || stringIdAsText.Length == 0)
            {
                return;
            }

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

        private void SelectLanguage(object sender, SelectionChangedEventArgs e)
        {

            string newLanguageFormat = (string)languageSelectorCb.SelectedItem;

            if (!_selectedLanguageFormat.Equals(newLanguageFormat))
            {
                _selectedLanguageFormat = newLanguageFormat;

                SetSelectableResources();
            }
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

        private void SelectResource(object sender, SelectionChangedEventArgs e)
        {

            string newResource = (string)resourceSelectorCb.SelectedItem;

            bool changed = false;
            if (_selectedResource == null || newResource == null)
            {
                _selectedResource = newResource;
                changed = true;
            }
            else if (!_selectedResource.Equals(newResource))
            {
                _selectedResource = newResource;
                changed = true;
            }

            if (changed)
            {
                ReLoadStrings(sender, e);
            }
        }

        #endregion top search bar button listeners

        #region operation buttons

        /// <summary>
        /// Opens another window that details extra information about the text that is not normally necessary to have.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTextInfo(object sender, RoutedEventArgs e)
        {

            if (DisplayType.SHOW_DECLINATED_ADJECTIVES == _displayType)
            {
                App.Logger.Log("This function is currently not available for adjectives");
                return;
            }

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
        /// Shows the edit window, taking the results into the currently displayed entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAddEditWindow(object sender, RoutedEventArgs e)
        {

            if (DisplayType.SHOW_DECLINATED_ADJECTIVES == _displayType)
            {
                App.Logger.Log("This function is currently not available for adjectives");
                return;
            }

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

            if (DisplayType.SHOW_DECLINATED_ADJECTIVES == _displayType)
            {
                App.Logger.Log("This function is currently not available for adjectives");
                return;
            }

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

        private void Export(object sender, RoutedEventArgs e)
        {
            XmlExporter.Export(_textDB, _selectedLanguageFormat);
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            XmlImporter.Import(_textDB);

            ReLoadStrings(sender, e);
        }

        #endregion operation buttons

        #region -- Closing Handler --

        public override void Closed()
        {
            _closingHandler.OnEditorClose();
        }

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
