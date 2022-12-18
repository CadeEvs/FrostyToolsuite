using BiowareLocalizationPlugin.LocalizedResources;
using Frosty.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BiowareLocalizationPlugin.Controls
{
    /// <summary>
    /// This is a popup window that displays in which resources the text of the selected id is found, it also displays what other text ids are stored at the same position(s) in those resources.
    /// </summary>
    [TemplatePart(Name = "resourceList", Type = typeof(ListBox))]
    [TemplatePart(Name = "stringIdList", Type = typeof(ListBox))]
    [TemplatePart(Name = "charactersList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_OkButton", Type = typeof(Button))]
    public partial class TextInfoWindow : FrostyDockableWindow
    {

        public TextInfoWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        public void Init(string languageFormat, uint textId, BiowareLocalizedStringDatabase localizedStringsDb)
        {

            Title = "TextInfo: " + textId.ToString("X8") + " -  " + localizedStringsDb.GetString(textId);

            IEnumerable<LocalizedStringResource> localizedResources = localizedStringsDb.GetAllLocalizedStringResourcesForTextId(languageFormat, textId);

            // multiple text ids can occur in multiple places
            var allResourceNames = new List<string>();
            var allUniqueTextIds = new SortedSet<string>();
            SortedSet<char> allSupportedCharacters = null;
            foreach (LocalizedStringResource resource in localizedResources)
            {
                // add the resource name...
                allResourceNames.Add(resource.Name);

                // ...add the other texts ids that share the position and such text...
                IEnumerable<string> textIds = resource.GetAllTextIdsAtPositionOf(textId);
                foreach (string anotherTextId in textIds)
                {
                    allUniqueTextIds.Add(anotherTextId);
                }

                // ... add or rather retain the limited set of characters supported in all the of the resources
                var supporedCharacters = resource.GetDefaultSupportedCharacters();
                if (allSupportedCharacters == null)
                {
                    allSupportedCharacters = new SortedSet<char>(supporedCharacters);
                }
                else
                {
                    allSupportedCharacters.IntersectWith(supporedCharacters);
                }
            }

            ListBoxUtils.SortListIntoListBox(allResourceNames, resourceList);
            ListBoxUtils.SortListIntoListBox(allUniqueTextIds, stringIdList);

            if (allSupportedCharacters != null)
            {
                ListBoxUtils.SortListIntoListBox(allSupportedCharacters, charactersList);
            }
        }

        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Called to copy the selected values of either listbox to the clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopySelection(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxUtils.CopySelectionToClipboard(sender, e);
        }
    }
}
