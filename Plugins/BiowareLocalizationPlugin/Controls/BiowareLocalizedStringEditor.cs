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
    [TemplatePart(Name = PART_Replace, Type = typeof(Button))]
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
        private const string PART_Replace = "PART_Replace";
        private const string PART_Remove = "PART_Remove";

        private const string PART_LanguageSelector = "PART_LanguageSelector";
        private const string PART_RefreshButton = "PART_RefreshButton";

        private const string PART_Export = "PART_Export";
        private const string PART_Import = "PART_Import";

        private const string PART_ResourceSelector = "PART_ResourceSelector";

        //##############################################################################

        private const string m_toggleDisplayButtonTextsString = "Search Text with ID:";
        private const string m_toggleDisplayButtonAdjectiveString = "Search Adjective with ID:";

        private const string m_toggleDisplayButtonTooltipTextsString = "Searches for text entries from the default text id space\r\n"
            + "Use this toggle to show only declinated adjectives used for names of crafted items in DA:I";

        private const string m_toggleDisplayButtonTooltipAdjectiveString = "Searches for declinated adjectives used for generating the names of crafted items in DA:I\r\n"
            + "Use this toggle to show only the normal texts used in both DA:I and ME:A";

        private const string m_SHOW_ALL_RESOURCES = "<Show All>";

        //##############################################################################

        /// <summary>
        /// The text db instance, stored as variable for convenience
        /// </summary>
        private readonly BiowareLocalizedStringDatabase m_textDB;

        /// <summary>
        /// handler to support closing child windows if this editor is closed.
        /// </summary>
        private readonly ClosingHandler m_closingHandler;

        private Button m_toggleTextsOrAdjectivesButton;

        private CheckBox m_searchHexIdCB;

        private TextBox m_localizedStringTb;

        private ListBox m_stringIdListBox;

        private TextBox m_searchfieldTb;

        private CheckBox m_modifiedOnlyCB;

        private CheckBox m_updateTextIdFieldCB;

        private Button m_textInfoBt;

        private Button m_replaceButton;

        private Button m_removeButton;

        private ComboBox m_languageSelectorCb;

        private List<uint> m_textIdsList = new List<uint>();

        private string m_selectedLanguageFormat;

        private bool m_isFirstTimeInitialization = true;

        private ComboBox m_resourceSelectorCb;

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
        private DisplayType m_displayType = DisplayType.SHOW_TEXTS;

        /// <summary>
        /// The last selected resource - Note that this can be null!
        /// </summary>
        private string m_selectedResource = m_SHOW_ALL_RESOURCES;

        //##############################################################################
        #endregion constants and variables

        #region initialization

        static BiowareLocalizedStringEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiowareLocalizedStringEditor), new FrameworkPropertyMetadata(typeof(BiowareLocalizedStringEditor)));
        }

        public BiowareLocalizedStringEditor(BiowareLocalizedStringDatabase inTextDb)
        {
            m_textDB = inTextDb;
            m_selectedLanguageFormat = inTextDb.DefaultLanguage;
            m_closingHandler = new ClosingHandler();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_stringIdListBox = GetTemplateChild(PART_StringIdList) as ListBox;
            m_stringIdListBox.SelectionChanged += StringIdListbox_SelectionChanged;

            m_localizedStringTb = GetTemplateChild(PART_LocalizedString) as TextBox;

            m_toggleTextsOrAdjectivesButton = GetTemplateChild(PART_ToggleDisplayTextsOrAdjectives) as Button;
            m_toggleTextsOrAdjectivesButton.Content = m_toggleDisplayButtonTextsString;
            m_toggleTextsOrAdjectivesButton.ToolTip = m_toggleDisplayButtonTooltipTextsString;
            m_toggleTextsOrAdjectivesButton.Click += ToggleTextsOrAdjectives;

            m_searchHexIdCB = GetTemplateChild(PART_hexSearchCB) as CheckBox;
            m_searchHexIdCB.Checked += SearchFieldFormatChangedToHex;
            m_searchHexIdCB.Unchecked += SearchFieldFormatChangedToDecimal;

            m_searchfieldTb = GetTemplateChild(PART_Searchfield) as TextBox;
            m_searchfieldTb.PreviewKeyDown += SearchFieldActualized;
            Button btSearchButton = GetTemplateChild(PART_SearchButton) as Button;
            btSearchButton.Click += SearchButtonClicked;

            Button btSearchTextButton = GetTemplateChild(PART_SearchTextButton) as Button;
            btSearchTextButton.Click += ShowSearchDialog;

            m_modifiedOnlyCB = GetTemplateChild(PART_ModifiedOnlyCB) as CheckBox;
            m_modifiedOnlyCB.Click += ReLoadStrings;

            m_updateTextIdFieldCB = GetTemplateChild(PART_UpdateTextIdFieldCB) as CheckBox;

            m_textInfoBt = GetTemplateChild(PART_ShowTextInfo) as Button;
            m_textInfoBt.IsEnabled = false; // initially disabled until a text is selected
            m_textInfoBt.Click += ShowTextInfo;

            Button addButton = GetTemplateChild(PART_AddEdit) as Button;
            addButton.Click += ShowAddEditWindow;

            m_replaceButton = GetTemplateChild(PART_Replace) as Button;
            m_replaceButton.Click += ShowReplaceWindow;

            m_removeButton = GetTemplateChild(PART_Remove) as Button;
            m_removeButton.IsEnabled = false; // initially disabled until a text is selected
            m_removeButton.Click += Remove;

            Button refreshButton = GetTemplateChild(PART_RefreshButton) as Button;
            refreshButton.Click += ReLoadStrings;

            Button exportButton = GetTemplateChild(PART_Export) as Button;
            exportButton.Click += Export;

            Button importButton = GetTemplateChild(PART_Import) as Button;
            importButton.Click += Import;

            // listeners are active form the beginning, so we have to keep a certain order in here for the language and resource listeners.
            m_languageSelectorCb = GetTemplateChild(PART_LanguageSelector) as ComboBox;
            m_languageSelectorCb.ItemsSource = m_textDB.GellAllLanguages();
            m_languageSelectorCb.SelectedItem = m_selectedLanguageFormat;
            m_languageSelectorCb.SelectionChanged += SelectLanguage;

            m_resourceSelectorCb = GetTemplateChild(PART_ResourceSelector) as ComboBox;
            SetSelectableResources();
            m_resourceSelectorCb.SelectionChanged += SelectResource;

            Loaded += LoadFirstTime;

        }

        #endregion initialization

        #region data loading
        private void LoadFirstTime(object sender, RoutedEventArgs e)
        {
            if (m_isFirstTimeInitialization)
            {
                LoadStrings(sender, e);
                m_isFirstTimeInitialization = false;
            }
        }

        private void ReLoadStrings(object sender, RoutedEventArgs e)
        {
            m_textIdsList.Clear();
            m_stringIdListBox.Items.Clear();

            LoadStrings(sender, e);
        }

        private void LoadStrings(object sender, RoutedEventArgs e)
        {

            bool? nullableModifiedOnly = m_modifiedOnlyCB.IsChecked;
            bool modifiedOnly = nullableModifiedOnly.HasValue && nullableModifiedOnly.Value;

            if (m_selectedResource == null)
            {
                m_textIdsList = new List<uint>();
                return;
            }

            switch (m_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    LoadTexts0(modifiedOnly);
                    break;
                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    LoadAdjectives0(modifiedOnly);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown DisplayType: " + m_displayType);
            }

            if (m_textIdsList.Count == 0)
            {
                return;
            }
            m_stringIdListBox.ScrollIntoView(m_stringIdListBox.Items[0]);
        }

        private void LoadTexts0(bool modifiedOnly)
        {
            bool showTextsFromAllResources = m_SHOW_ALL_RESOURCES.Equals(m_selectedResource);
            FrostyTaskWindow.Show("Loading texts", "", (task) =>
            {
                m_textIdsList = LoadTextIds(modifiedOnly, showTextsFromAllResources);
            });

            m_textIdsList.Sort();

            foreach (uint textId in m_textIdsList)
            {
                m_stringIdListBox.Items.Add(textId.ToString("X8") + " - " + m_textDB.GetText(m_selectedLanguageFormat, textId));
            }
        }

        private List<uint> LoadTextIds(bool modifiedOnly, bool showTextsFromAllResources)
        {

            List<uint> textIds;

            if (!modifiedOnly && showTextsFromAllResources)
            {
                textIds = m_textDB.GetAllTextIds(m_selectedLanguageFormat).ToList();
            }
            else if (modifiedOnly && showTextsFromAllResources)
            {
                textIds = m_textDB.GetAllModifiedTextsIds(m_selectedLanguageFormat).ToList();
            }
            else if (modifiedOnly && !showTextsFromAllResources)
            {
                textIds = m_textDB.GetAllModifiedTextIdsFromResource(m_selectedLanguageFormat, m_selectedResource).ToList();
            }
            else //if( !modifiedOnly && !showTextsFromAllResources)
            {
                textIds = m_textDB.GetAllTextIdsFromResource(m_selectedLanguageFormat, m_selectedResource).ToList();
            }

            return textIds;
        }

        private void LoadAdjectives0(bool modifiedOnly)
        {

            FrostyTaskWindow.Show("Loading adjectives", "", (task) =>
            {
                m_textIdsList = LoadAdjectiveIds(modifiedOnly);
            });

            m_textIdsList.Sort();

            foreach (uint adjectiveId in m_textIdsList)
            {
                List<string> adjectives = m_textDB.GetDeclinatedAdjectives(m_selectedLanguageFormat, m_selectedResource, adjectiveId);

                string firstAdjective = adjectives.Count > 0 ? adjectives[0] : "";

                m_stringIdListBox.Items.Add(adjectiveId.ToString("X8") + " - " + firstAdjective);
            }
        }

        private List<uint> LoadAdjectiveIds(bool modifiedOnly)
        {
            if (modifiedOnly)
            {
                return m_textDB.GetModifiedDeclinatedAdjectiveIdsFromResource(m_selectedLanguageFormat, m_selectedResource).ToList();
            }
            else
            {
                return m_textDB.GetAllDeclinatedAdjectiveIdsFromResource(m_selectedLanguageFormat, m_selectedResource).ToList();
            }
        }

        /// <summary>
        /// Sets the list of currently applicable resources for the selected language and displaytype in the resource selector.
        /// </summary>
        private void SetSelectableResources()
        {

            m_resourceSelectorCb.Items.Clear();
            List<string> resourceNames = GetApplicableResources();

            foreach (string resourceName in resourceNames)
            {
                m_resourceSelectorCb.Items.Add(resourceName);
            }

            if (resourceNames.Count > 0)
            {
                m_resourceSelectorCb.SelectedItem = m_resourceSelectorCb.Items[0];
            }
            else
            {
                m_resourceSelectorCb.SelectedItem = null;
            }
        }

        /// <summary>
        /// Returns the list or resources that are selectable for the currently selected language and display type.
        /// </summary>
        /// <returns></returns>
        private List<string> GetApplicableResources()
        {

            if (m_selectedLanguageFormat == null)
            {
                // we are either still in setup or something went wrong...
                return new List<string>();
            }

            List<string> resourceList = new List<string>();

            switch (m_displayType)
            {
                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    resourceList.AddRange(m_textDB.GetAllResourceNamesWithDeclinatedAdjectives(m_selectedLanguageFormat));
                    break;

                case DisplayType.SHOW_TEXTS:
                default:
                    resourceList.Add(m_SHOW_ALL_RESOURCES);
                    resourceList.AddRange(m_textDB.GetAllResourceNames(m_selectedLanguageFormat));
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
            int selectedIndex = m_stringIdListBox.SelectedIndex;
            return selectedIndex >= 0 && selectedIndex < m_textIdsList.Count ? m_textIdsList[selectedIndex] : 0;
        }

        #region string selection listeners

        private void StringIdListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint selectedTextId = GetCurrentStringId();
            PopulateLocalizedString(selectedTextId);

            bool isSelectionValid = e.AddedItems.Count > 0;
            bool isTextSelection = DisplayType.SHOW_TEXTS == m_displayType;

            m_textInfoBt.IsEnabled = isSelectionValid && isTextSelection;
            m_removeButton.IsEnabled = isSelectionValid;

            if (isSelectionValid && m_updateTextIdFieldCB.IsChecked == true)
            {
                SetTextIdInSearchField(selectedTextId);
            }
        }

        private void SetTextIdInSearchField(uint textId)
        {
            string textIdFormat = (m_searchHexIdCB.IsChecked == true) ? "X8" : "D";
            m_searchfieldTb.Text = textId.ToString(textIdFormat);
        }

        private void PopulateLocalizedString(uint textId)
        {

            string textToShow;
            switch (m_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    textToShow = m_textDB.GetText(m_selectedLanguageFormat, textId);
                    break;

                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    textToShow = LoadDeclinatedAdjectiveToShow(textId);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown DisplayType: " + m_displayType);
            }

            m_localizedStringTb.Text = textToShow;
        }

        private string LoadDeclinatedAdjectiveToShow(uint adjectiveId)
        {

            if (m_selectedResource == null)
            {
                return "";
            }

            List<string> declinations = m_textDB.GetDeclinatedAdjectives(m_selectedLanguageFormat, m_selectedResource, adjectiveId);

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

            switch (m_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    m_displayType = DisplayType.SHOW_DECLINATED_ADJECTIVES;
                    buttonText = m_toggleDisplayButtonAdjectiveString;
                    buttonTooltip = m_toggleDisplayButtonTooltipAdjectiveString;
                    m_replaceButton.IsEnabled = false;
                    break;

                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                default:
                    m_displayType = DisplayType.SHOW_TEXTS;
                    buttonText = m_toggleDisplayButtonTextsString;
                    buttonTooltip = m_toggleDisplayButtonTooltipTextsString;
                    m_replaceButton.IsEnabled = true;
                    break;
            }

            m_toggleTextsOrAdjectivesButton.Content = buttonText;
            m_toggleTextsOrAdjectivesButton.ToolTip = buttonTooltip;

            SetSelectableResources();
        }

        private void GetUpdateFromTextField(NumberStyles style, Action<uint> textIdAction)
        {
            string stringIdAsText = m_searchfieldTb.Text;

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
            NumberStyles style = (m_searchHexIdCB.IsChecked == true) ? NumberStyles.HexNumber : NumberStyles.Integer;
            GetUpdateFromTextField(style, textId => SearchTextId(textId));
        }

        private void SearchTextId(uint textId)
        {
            int index = m_textIdsList.IndexOf(textId);
            if (index < 0 && index < m_stringIdListBox.Items.Count)
            {
                m_stringIdListBox.UnselectAll();
                return;
            }

            m_stringIdListBox.SelectedIndex = index;
            m_stringIdListBox.ScrollIntoView(m_stringIdListBox.SelectedItem);

            SetTextIdInSearchField(textId);
        }

        private void SelectLanguage(object sender, SelectionChangedEventArgs e)
        {

            string newLanguageFormat = (string)m_languageSelectorCb.SelectedItem;

            if (!m_selectedLanguageFormat.Equals(newLanguageFormat))
            {
                m_selectedLanguageFormat = newLanguageFormat;

                SetSelectableResources();
            }
        }

        private void ShowSearchDialog(object sender, RoutedEventArgs e)
        {

            if (m_stringIdListBox != null && m_stringIdListBox.Items.Count > 0)
            {
                SearchFindWindow searchWindow = new SearchFindWindow(m_stringIdListBox);
                searchWindow.Show();

                m_closingHandler.AddChildWindow(searchWindow);
            }
        }

        private void SelectResource(object sender, SelectionChangedEventArgs e)
        {

            string newResource = (string)m_resourceSelectorCb.SelectedItem;

            bool changed = false;
            if (m_selectedResource == null || newResource == null)
            {
                m_selectedResource = newResource;
                changed = true;
            }
            else if (!m_selectedResource.Equals(newResource))
            {
                m_selectedResource = newResource;
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

            if (DisplayType.SHOW_DECLINATED_ADJECTIVES == m_displayType)
            {
                App.Logger.Log("This function is currently not available for adjectives");
                return;
            }

            uint stringId = GetCurrentStringId();

            TextInfoWindow infoWindow = new TextInfoWindow
            {
                Owner = Application.Current.MainWindow
            };
            infoWindow.Init(m_selectedLanguageFormat, stringId, m_textDB);
            infoWindow.Show();

            m_closingHandler.AddChildWindow(infoWindow);
        }

        /// <summary>
        /// Shows the edit window, taking the results into the currently displayed entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAddEditWindow(object sender, RoutedEventArgs e)
        {

            uint stringId = GetCurrentStringId();

            switch (m_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    ShowTextEditWindow(stringId);
                    break;

                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    ShowAdjectiveEditWindow(stringId);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown DisplayType: " + m_displayType);
            }

        }

        private void ShowTextEditWindow(uint textId)
        {
            AddEditWindow editWindow = new AddEditWindow(m_textDB, m_selectedLanguageFormat)
            {
                Owner = Application.Current.MainWindow
            };
            editWindow.Init(textId);

            bool? save = editWindow.ShowDialog();
            if (save.HasValue && save.Value)
            {
                Tuple<uint, string> saveValue = editWindow.SaveValue;

                // textId is not necessarily the stringId originally given to the dialog!
                textId = saveValue.Item1;
                string text = saveValue.Item2;

                string entry = textId.ToString("X8") + " - " + text;

                int entryIndex = m_textIdsList.IndexOf(textId);

                if (entryIndex < 0)
                {
                    m_stringIdListBox.Items.Add(entry);
                    m_textIdsList.Add(textId);
                }
                else
                {
                    m_stringIdListBox.Items[entryIndex] = entry;
                }

                SearchTextId(textId);
            }
        }

        private void ShowAdjectiveEditWindow(uint adjectiveId)
        {
            EditAdjectivesWindow editWindow = new EditAdjectivesWindow(m_textDB, m_selectedLanguageFormat, m_selectedResource)
            {
                Owner = Application.Current.MainWindow
            };
            editWindow.Init(adjectiveId);

            bool? save = editWindow.ShowDialog();
            if (save.HasValue && save.Value)
            {

                Tuple<uint, List<string>> saveValue = editWindow.SaveValue;

                adjectiveId = saveValue.Item1;
                List<string> declinations = saveValue.Item2;

                string firstDeclination = declinations.Count > 0 ? declinations[0] : "";

                string entry = adjectiveId.ToString("X8") + " - " + firstDeclination;

                int entryIndex = m_textIdsList.IndexOf(adjectiveId);

                if (entryIndex < 0)
                {
                    m_stringIdListBox.Items.Add(entry);
                    m_textIdsList.Add(adjectiveId);
                }
                else
                {
                    m_stringIdListBox.Items[entryIndex] = entry;
                }

                SearchTextId(adjectiveId);
            }
        }

        /// <summary>
        /// Removes / Reverts the selected text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove(object sender, RoutedEventArgs e)
        {

            int index = m_stringIdListBox.SelectedIndex;

            if (index < 0 || index >= m_textIdsList.Count)
            {
                // not sure how this should be possible...
                App.Logger.LogWarning("Entered impossible state <Remove on no item selected>: Remove Operation did not complete!");
                return;
            }

            uint selectedId = m_textIdsList[index];

            bool entryStillExists;
            switch (m_displayType)
            {
                case DisplayType.SHOW_TEXTS:
                    entryStillExists = RemoveText0(index, selectedId);
                    break;

                case DisplayType.SHOW_DECLINATED_ADJECTIVES:
                    entryStillExists = RevertAdjective0(index, selectedId);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown DisplayType: " + m_displayType);
            }

            if (!entryStillExists)
            {
                m_stringIdListBox.Items.RemoveAt(index);
                m_textIdsList.RemoveAt(index);
            }

            SearchTextId(selectedId);
        }

        /// <summary>
        /// Calls the appropriate methods to revert or delete a text entry. Returns whether or not a text of the given id still exists afterwards.
        /// </summary>
        /// <param name="idIndex"></param>
        /// <param name="textId"></param>
        /// <returns>true if a text of the id still exists, false if no such text exists anymore</returns>
        private bool RemoveText0(int idIndex, uint textId)
        {

            m_textDB.RevertText(m_selectedLanguageFormat, textId);

            string text = m_textDB.FindText(m_selectedLanguageFormat, textId);

            if (text != null)
            {
                string entry = textId.ToString("X8") + " - " + text;
                m_stringIdListBox.Items[idIndex] = entry;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calls the appropriate methods to revert or delete an adjective entry. Returns whether or not the declinated adjective of the given id still exists afterwards.
        /// </summary>
        /// <param name="idIndex"></param>
        /// <param name="adjectiveId"></param>
        /// <returns>true if an adjective entry of the id still exists, false if no such entry exists anymore</returns>
        private bool RevertAdjective0(int idIndex, uint adjectiveId)
        {

            m_textDB.RevertDeclinatedAdjective(m_selectedLanguageFormat, m_selectedResource, adjectiveId);

            List<string> declinations = m_textDB.GetDeclinatedAdjectives(m_selectedLanguageFormat, m_selectedResource, adjectiveId);

            if (declinations != null && declinations.Count > 0)
            {
                string entry = adjectiveId.ToString("X8") + " - " + declinations[0];
                m_stringIdListBox.Items[idIndex] = entry;

                return true;
            }

            return false;
        }

        private void ShowReplaceWindow(object sender, RoutedEventArgs e)
        {

            List<string> stringsWithId = new List<string>();
            foreach (string entry in m_stringIdListBox.Items)
            {
                stringsWithId.Add(entry);
            }

            ReplaceWindow replaceWindow = new ReplaceWindow(m_textDB, m_selectedLanguageFormat, stringsWithId, m_stringIdListBox.SelectedIndex)
            {
                Owner = Application.Current.MainWindow
            };

            bool? save = replaceWindow.ShowDialog();
            if (save.HasValue && save.Value)
            {
                // the replace window currently updates the selection of this view.
                uint lastReplacedId = m_textIdsList[replaceWindow.LastEditedIndex];

                ReLoadStrings(sender, e);

                SearchTextId(lastReplacedId);
            }
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            XmlExporter.Export(m_textDB, m_selectedLanguageFormat);
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            XmlImporter.Import(m_textDB);

            ReLoadStrings(sender, e);
        }

        #endregion operation buttons

        #region -- Closing Handler --

        public override void Closed()
        {
            m_closingHandler.OnEditorClose();
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
