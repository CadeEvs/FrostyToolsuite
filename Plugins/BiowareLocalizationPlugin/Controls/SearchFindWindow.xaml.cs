using Frosty.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BiowareLocalizationPlugin.Controls
{
    /// <summary>
    /// This is a non blocking dialog used to search for strings in the currently displayed texts.
    /// </summary>
    public partial class SearchFindWindow : FrostyDockableWindow
    {

        // this is the list box from the main edit window - we search and select directly on there!
        private readonly ListBox m_mainWindowTextSelectionBox;

        public SearchFindWindow(ListBox inStringSelectionBox)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            m_mainWindowTextSelectionBox = inStringSelectionBox;

            searchTextField.Focus();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Searches the text, if found the entry is selected and scrolled into view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search(object sender, RoutedEventArgs e)
        {

            string searchedFor = searchTextField.Text;
            if(searchedFor == null || searchedFor.Length == 0)
            {
                return;
            }

            bool? nullableSearchBackwards = backSearchCB.IsChecked;
            bool searchBackwards = nullableSearchBackwards.HasValue && nullableSearchBackwards.Value;

            bool? nullableSearchCaseSensitive = caseSensitiveSearchCB.IsChecked;
            bool searchCaseSensitive = nullableSearchCaseSensitive.HasValue && nullableSearchCaseSensitive.Value;

            Func<int, int> updFctn;
            if( searchBackwards)
            {
                updFctn = (int i) => --i;
            }
            else
            {
                updFctn = (int j) => ++j;
            }

            StringComparison comparisonType = searchCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

            int searchIndex = m_mainWindowTextSelectionBox.SelectedIndex;
            searchIndex = updFctn(searchIndex);

            while(searchIndex >=0 && searchIndex < m_mainWindowTextSelectionBox.Items.Count)
            {
                string queriedText = (string)m_mainWindowTextSelectionBox.Items[searchIndex];

                // first 8 chars are the text Id, followed by 3 chars delimiter -> text starts at index 11
                int searchTextPosition = queriedText.IndexOf(searchedFor, 10, comparisonType);
                if(searchTextPosition > 0)
                {
                    m_mainWindowTextSelectionBox.SelectedIndex = searchIndex;
                    m_mainWindowTextSelectionBox.ScrollIntoView(m_mainWindowTextSelectionBox.SelectedItem);
                    return;
                }

                searchIndex = updFctn(searchIndex);
            }
        }
    }
}
