using Frosty.Controls;
using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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

        public ReplaceWindow(BiowareLocalizedStringDatabase inStringDb, string inSelectedLanguage, ListBox inTextListBox)
        {
            // TODO need better input than the list box from the main window
            InitializeComponent();

            m_stringDb = inStringDb;
            m_selectedLanguageFormat = inSelectedLanguage;
            m_mainWindowTextSelectionBox = inTextListBox;
        }

        // https://stackoverflow.com/questions/53319918/highlighting-coloring-charactars-in-a-wpf-richtextbox
        private static void HighlightText(RichTextBox richTextBox, int startPoint, int endPoint, Color color)
        {
            //Trying to highlight charactars here
            TextPointer pointer = richTextBox.Document.ContentStart;
            TextRange range = new TextRange(pointer.GetPositionAtOffset(startPoint), pointer.GetPositionAtOffset(endPoint));
            range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
        }

        /// <summary>
        /// Sest the given text into the given textbox and highlights the given consecutive (!) charachter offsets in the given color.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="textToSet"></param>
        /// <param name="partsToHighlight"></param>
        /// <param name="color"></param>
        private static void SetTextAndHighlightParts(RichTextBox textBox, string textToSet, List<int[]> partsToHighlight, Color color)
        {

            Paragraph paragraph = new Paragraph(new Run(textToSet))
            {
                TextIndent = 0d,
            };
            textBox.Document.Blocks.Add(paragraph);

            int charOffset = 2;
            // the highlighting method works with texpointers which include non character symbols, like start and end tags for the hightlighting...
            foreach (int[] segment in partsToHighlight)
            {
                HighlightText(textBox, segment[0]+ charOffset, segment[1]+ charOffset, color);
                charOffset += 4;
            }
        }

        /// <summary>
        /// Finds the next text in the textSelectionBox which contains the searched for string.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                // first 8 chars are the text Id, followed by 3 chars delimiter -> text starts at index 11
                int searchTextPosition = queriedText.IndexOf(searchedFor, 11, comparisonType);
                if (searchTextPosition > 0)
                {
                    m_mainWindowTextSelectionBox.SelectedIndex = searchIndex;
                    HandleFoundTextToReplace(queriedText, searchedFor, searchCaseSensitive);
                    return;
                }

                searchIndex++;
            }
        }

        /// <summary>
        /// Clears the current selection, in order to prepare for the next finding that may never come.
        /// </summary>
        private void ClearSelection()
        {


            Part_TextIdField.Text = "";

            Part_OriginalTextBox.Document.Blocks.Clear();
            Part_EditedTextBox.Document.Blocks.Clear();

            Part_EditedTextBox.IsReadOnly = true;
        }

        /// <summary>
        /// Creates the replacement text and updates all ui elements with the new data.
        /// </summary>
        /// <param name="selectedTextEntryWithId"></param>
        /// <param name="searchedText"></param>
        /// <param name="searchCaseSensitive"></param>
        private void HandleFoundTextToReplace(string selectedTextEntryWithId, string searchedText, bool searchCaseSensitive)
        {
            Part_TextIdField.Text = selectedTextEntryWithId.Substring(0, 8);

            string originalTextToDisplay = selectedTextEntryWithId.Substring(11);

            List<int[]> partsToReplace = GetSearchPositionsInOriginalText(originalTextToDisplay, searchedText, searchCaseSensitive);
            SetTextAndHighlightParts(Part_OriginalTextBox, originalTextToDisplay, partsToReplace, Colors.DarkRed);

            string replaceWith = Part_ReplaceWithField.Text;

            string replacedText = GetEditTextAndUpdatePositions(originalTextToDisplay, replaceWith, partsToReplace);
            SetTextAndHighlightParts(Part_EditedTextBox, replacedText, partsToReplace, Colors.DarkGreen);
        }

        /// <summary>
        /// Returns a list of character posisitons (as array of start and end position) of each finding in the given original text.
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="toReplace"></param>
        /// <param name="searchCaseSensitive"></param>
        /// <returns></returns>
        private List<int[]> GetSearchPositionsInOriginalText(string originalText, string toReplace, bool searchCaseSensitive)
        {

            StringComparison comparetype = searchCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            int toReplaceLength = toReplace.Length;

            int currentPartStart = 0;
            int lastPartEnd = 0;

            List<int[]> positionsToReplace = new List<int[]>();

            while (currentPartStart >= 0)
            {
                currentPartStart = originalText.IndexOf(toReplace, lastPartEnd, comparetype);

                if (currentPartStart >= 0)
                {
                    lastPartEnd = currentPartStart + toReplaceLength;

                    positionsToReplace.Add(new int[] { currentPartStart, lastPartEnd });
                }
            }

            return positionsToReplace;
        }

        /// <summary>
        /// Returns a new string, replacing each of the given positionsToReplace in the given originalText with the replacement.
        /// Also updates the list of positions, so they now show the new offset(s) for the replacement string.
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="replacement"></param>
        /// <param name="positionsToReplace"></param>
        /// <returns></returns>
        private string GetEditTextAndUpdatePositions(string originalText, string replacement, List<int[]> positionsToReplace)
        {
            int replacementLength = replacement.Length;

            int lastSelectPosition = 0;
            int currentWritePosition = 0;

            StringBuilder sb = new StringBuilder();

            foreach (int[] segment in positionsToReplace)
            {

                // part before replace
                int selectLength = segment[0] - lastSelectPosition;
                string partBefore = originalText.Substring(lastSelectPosition, selectLength);

                sb.Append(partBefore);
                currentWritePosition += selectLength;
                segment[0] = currentWritePosition;

                // replace
                sb.Append(replacement);

                lastSelectPosition = segment[1];
                currentWritePosition += replacementLength;
                segment[1] = currentWritePosition;
            }

            sb.Append(originalText.Substring(lastSelectPosition));

            return sb.ToString();
        }

        /// <summary>
        /// Basically 'save': Sets the current edit textbox's content as the new text for the text id in all resources that it currently exists in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            string editedText = GetEditedReplacementText();

            var resourceNames = m_stringDb.GetAllLocalizedStringResourcesForTextId(m_selectedLanguageFormat, textId).Select(r => r.Name).ToList();
            m_stringDb.SetText(m_selectedLanguageFormat, resourceNames, textId, editedText);

            App.Logger.Log("Replaced text <{0}>", stringIdAsText);

            m_wasAnythingReplaced = true;
        }

        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-extract-the-text-content-from-a-richtextbox
        // Why didn't they just add that as default method in the fuckin richtextbox in the first place?
        private string GetEditedReplacementText()
        {
            TextRange textRange = new TextRange(
                Part_EditedTextBox.Document.ContentStart,
                Part_EditedTextBox.Document.ContentEnd
            );

            string text = textRange.Text.TrimEnd("\r\n".ToCharArray());
            return text;
        }

        /// <summary>
        /// Allows editing the edit textboxes content. Primarily removes all highlights.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit(object sender, RoutedEventArgs e)
        {

            TextRange range = new TextRange(Part_EditedTextBox.Document.ContentStart, Part_EditedTextBox.Document.ContentEnd);

            SolidColorBrush defaultBackground = FindResource("WindowBackground") as SolidColorBrush;
            range.ApplyPropertyValue(TextElement.BackgroundProperty, defaultBackground);

            Part_EditedTextBox.IsReadOnly = false;
            Part_EditedTextBox.Focus();
            Part_EditedTextBox.CaretPosition = Part_EditedTextBox.Document.ContentStart;
        }

        private void ReplaceAndFindNext(object sender, RoutedEventArgs e)
        {
            Replace(sender, e);
            FindNext(sender, e);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            DialogResult = m_wasAnythingReplaced;

            Close();
        }
    }
}
