using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BiowareLocalizationPlugin.Controls
{
    /// <summary>
    /// Class with some utility methods for handling io out of listboxes
    /// </summary>
    public class ListBoxUtils
    {

        private ListBoxUtils()
        {
            // prevent instantiation
        }

        public static void SortListIntoListBox<T>(IEnumerable<T> entries, ListBox listBox) where T : IComparable
        {
            var entryList = new List<T>(entries);
            entryList.Sort();

            foreach(T entry in entryList)
            {
                listBox.Items.Add(entry);
            }
        }

        /// <summary>
        /// Called to copy the selected values of either listbox to the clipboard.
        /// </summary>
        /// <param name="listBoxEventOriginator">The originator of the event</param>
        /// <param name="eventArgs">The event</param>
        public static void CopySelectionToClipboard(object listBoxEventOriginator, ExecutedRoutedEventArgs eventArgs)
        {
            ListBox origin = (ListBox)listBoxEventOriginator;

            IList selection = origin.SelectedItems;

            if (selection.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (object selected in selection)
                {
                    sb.AppendLine(selected.ToString());
                }

                Clipboard.SetText(sb.ToString());
            }
        }
    }
}
