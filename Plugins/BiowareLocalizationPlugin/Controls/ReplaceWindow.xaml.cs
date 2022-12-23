using Frosty.Controls;
using Frosty.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BiowareLocalizationPlugin.Controls
{

    /// <summary>
    /// This window is used for replacing text parts within the currently shown list of strings.
    /// </summary>
    public partial class ReplaceWindow : FrostyDockableWindow
    {

        /// <summary>
        /// The list box from the main edit window - we search and select directly on there!
        /// </summary>
        private readonly ListBox m_mainWindowTextSelectionBox;

        /// <summary>
        /// The language selected in the main view.
        /// </summary>
        private readonly string m_selectedLanguageFormat;

        /// <summary>
        /// The actual database object.
        /// </summary>
        private readonly BiowareLocalizedStringDatabase m_stringDb;

        /// <summary>
        /// Marker whether any text was replaced at all.
        /// </summary>
        private bool m_wasAnythingReplaced = false;
        public bool WasAnyTextReplaced => m_wasAnythingReplaced;

        public ReplaceWindow(BiowareLocalizedStringDatabase inStringDb, string inSelectedLanguage, ListBox inTextListBox)
        {
            // TODO need better input than the list box, maybe do add replace all function after all? Otoh, NO!
            InitializeComponent();

            m_stringDb = inStringDb;
            m_selectedLanguageFormat = inSelectedLanguage;
            m_mainWindowTextSelectionBox = inTextListBox;
        }

        private void FindNext(object sender, RoutedEventArgs e)
        {

            ClearSelection();

            string searchedFor = Part_ToFindTextField.Text;
            if (searchedFor == null || searchedFor.Length == 0)
            {
                return;
            }

            int searchIndex = m_mainWindowTextSelectionBox.SelectedIndex + 1;
            int maxTextEntries = m_mainWindowTextSelectionBox.Items.Count;
            if (searchIndex >= maxTextEntries)
            {
                return;
            }

            bool? nullableSearchCaseSensitive = Part_CaseSensitiveSearchCB.IsChecked;
            bool searchCaseSensitive = nullableSearchCaseSensitive.HasValue && nullableSearchCaseSensitive.Value;
            StringComparison comparisonType = searchCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

            while (searchIndex >= 0 && searchIndex < maxTextEntries)
            {
                string queriedText = (string)m_mainWindowTextSelectionBox.Items[searchIndex];

                // first 8 chars are the text Id, followed by 3 chars delimiter -> text starts at index 10
                int searchTextPosition = queriedText.IndexOf(searchedFor, 10, comparisonType);
                if (searchTextPosition > 0)
                {
                    m_mainWindowTextSelectionBox.SelectedIndex = searchIndex;
                    HandleFoundTextToReplace(queriedText, searchedFor, searchCaseSensitive);
                    return;
                }

                searchIndex++;
            }
        }

        private void ClearSelection()
        {
            Part_TextIdField.Text = "";
            Part_OriginalTextBox.Text = "";
            Part_EditedTextBox.Text = "";

            Part_EditedTextBox.IsEnabled = false;
        }

        private void HandleFoundTextToReplace(string selectedTextEntryWithId, string searchedText, bool searchCaseSensitive)
        {
            string textIdPart = selectedTextEntryWithId.Substring(0, 8);

            string replaceWith = Part_ReplaceWithField.Text;

            string originalTextToDisplay = selectedTextEntryWithId.Substring(10);

            string replacedText;
            if (searchCaseSensitive)
            {
                replacedText = originalTextToDisplay.Replace(searchedText, replaceWith);
            }
            else
            {
                replacedText = ReplaceTextCaseInsensitive(originalTextToDisplay, searchedText, replaceWith);
            }


            Part_TextIdField.Text = textIdPart;
            Part_OriginalTextBox.Text = originalTextToDisplay;
            Part_EditedTextBox.Text = replacedText;

            // TODO mark the original and changed parts in the textboxes accordingly...
        }

        private string ReplaceTextCaseInsensitive(string originalText, string toReplace, string replacement)
        {

            int toReplaceLength = toReplace.Length;

            int currentPosition = 0;
            int lastPosition = 0;

            StringBuilder sb = new StringBuilder();

            while (currentPosition >= 0)
            {
                currentPosition = originalText.IndexOf(toReplace, lastPosition, StringComparison.InvariantCultureIgnoreCase);

                if (currentPosition >= 0)
                {
                    int length = currentPosition - lastPosition;
                    sb.Append(originalText.Substring(lastPosition, length));
                    sb.Append(replacement);
                    lastPosition = currentPosition + toReplaceLength;
                }
            }

            sb.Append(originalText.Substring(lastPosition));

            return sb.ToString();
        }

        private void Replace(object sender, RoutedEventArgs e)
        {
            string stringIdAsText = Part_TextIdField.Text;

            if (stringIdAsText == null || stringIdAsText.Length == 0)
            {
                App.Logger.LogError("No Text Id set!");
                return;
            }

            bool canReadId = uint.TryParse(stringIdAsText, NumberStyles.HexNumber, null, out uint textId);
            if (!canReadId)
            {
                App.Logger.LogError("Cannot read text id: <{0}>", stringIdAsText);
                return;
            }


            var resourceNames = m_stringDb.GetAllLocalizedStringResourcesForTextId(m_selectedLanguageFormat, textId).Select(r => r.Name).ToList();
            m_stringDb.SetText(m_selectedLanguageFormat, resourceNames, textId, Part_EditedTextBox.Text);

            App.Logger.Log("Replaced text <{0}>", stringIdAsText);

            m_wasAnythingReplaced = true;
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            Part_EditedTextBox.IsEnabled = true;
            Part_EditedTextBox.Focus();
            Part_EditedTextBox.Select(0, 0);
        }

        private void ReplaceAndFindNext(object sender, RoutedEventArgs e)
        {
            Replace(sender, e);
            FindNext(sender, e);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            DialogResult = WasAnyTextReplaced;

            Close();
        }
    }
}
