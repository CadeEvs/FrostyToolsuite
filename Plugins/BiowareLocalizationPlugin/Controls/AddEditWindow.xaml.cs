using BiowareLocalizationPlugin.LocalizedResources;
using Frosty.Controls;
using Frosty.Core;
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
    /// Via this window the user can add or edit existing texts.
    /// This is a positive logic only window, i.e., you can add texts to additional resource, but cannot deselect it again.
    /// </summary>
    [TemplatePart(Name = "textIdField", Type = typeof(TextBox))]
    [TemplatePart(Name = "localizedTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "addedResourcesListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "defaultResourcesListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "saveButton", Type = typeof(Button))]
    [TemplatePart(Name = "cancelButton", Type = typeof(Button))]
    public partial class AddEditWindow : FrostyDockableWindow
    {

        private readonly string _selectedLanguageFormat;
        private readonly BiowareLocalizedStringDatabase _stringDb;

        /// <summary>
        /// List to keep track of resources where a text id was removed from.
        /// </summary>
        private readonly List<string> _removedResources = new List<string>();

        /// <summary>
        /// The save value tuple consiting of the text id, and text. This is only available after the save action!
        /// </summary>
        public Tuple<uint, string> SaveValue { get; private set; }

        public AddEditWindow(BiowareLocalizedStringDatabase stringDb, string languageFormat)
        {
            InitializeComponent();

            _selectedLanguageFormat = languageFormat;
            _stringDb = stringDb;
        }

        /// <summary>
        /// Initializes the window with the currently selected textid and text.
        /// </summary>
        /// <param name="textId"></param>
        public void Init(uint textId)
        {

            if(textId != 0)
            {
                textIdField.Text = textId.ToString("X8");
            }
            UpdateData(textId);
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            uint textId = ReadTextId();
            UpdateData(textId);
        }

        /// <summary>
        /// Updates the listed data
        /// </summary>
        private void UpdateData(uint textId)
        {

            if (textId == 0)
            {
                // revert;
                localizedTextBox.Text = "";
                DeselectAllResources();
            }
            else
            {
                localizedTextBox.Text = _stringDb.FindText(_selectedLanguageFormat, textId);
                DeselectAllResources();

                IEnumerable<LocalizedStringResource> addedResources = _stringDb.GetAddedLocalizedStringResourcesForTextId(_selectedLanguageFormat, textId);
                foreach (LocalizedStringResource res in addedResources)
                {
                    addedResourcesListBox.Items.Add(res.Name);
                }

                IEnumerable<LocalizedStringResource> defaultResources = _stringDb.GetDefaultLocalizedStringResourcesForTextId(_selectedLanguageFormat, textId);
                ListBoxUtils.SortListIntoListBox(defaultResources.Select(r => r.Name), defaultResourcesListBox);
            }
        }

        private void DeselectAllResources()
        {
            defaultResourcesListBox.Items.Clear();
            addedResourcesListBox.Items.Clear();

            _removedResources.Clear();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            uint textId = ReadTextId();

            if(textId != 0)
            {

                // TODO move all this text handling into a dedicated handler or something.
                // This is currently all over the place >_<

                _stringDb.RemoveText(_selectedLanguageFormat, _removedResources, textId);

                List<string> resources = new List<string>();
                foreach (string resourceName in defaultResourcesListBox.Items)
                {
                    resources.Add(resourceName);
                }
                foreach (string resourceName in addedResourcesListBox.Items)
                {
                    resources.Add(resourceName);
                }

                string text = localizedTextBox.Text;
                _stringDb.SetText(_selectedLanguageFormat, resources, textId, text);

                SaveValue = Tuple.Create(textId, text);

                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// Closes the dialog without saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            SaveValue = null;
            Close();
        }

        /// <summary>
        /// Reads the set hexadecimal text id and sets the lokal variable with the parsed result.
        /// </summary>
        /// <returns>_textId</returns>
        private uint ReadTextId()
        {
            string text = textIdField.Text;
            bool canRead = uint.TryParse(text, NumberStyles.HexNumber, null, out uint textId);
            if (canRead)
            {
                return textId;
            }

            textIdField.Text = "";
            App.Logger.LogWarning("Bad Input!");
            return 0;
        }

        /// <summary>
        /// Shows a popup dialog to select one or more resources where to add the current text into.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddResources(object sender, RoutedEventArgs e)
        {

            IEnumerable<string> selectableResources = _stringDb.GetAllResourceNames(_selectedLanguageFormat)
                .Where(r => !defaultResourcesListBox.Items.Contains(r)
                    && !addedResourcesListBox.Items.Contains(r) );

            ResourceSelectionWindow selectionDialog = new ResourceSelectionWindow(selectableResources);
            bool? saved = selectionDialog.ShowDialog();

            if(saved != null && saved.Value)
            {
                foreach(string resourceName in selectionDialog.SelectedResources)
                {
                    addedResourcesListBox.Items.Add(resourceName);
                }
            }
        }

        private void RemoveResources(object sender, RoutedEventArgs e)
        {

            List<string> selectedToRemove = addedResourcesListBox.SelectedItems.OfType<string>().ToList();
            foreach(string itemToRemove in selectedToRemove)
            {
                addedResourcesListBox.Items.Remove(itemToRemove);
            }

            _removedResources.AddRange(selectedToRemove);
        }

        /// <summary>
        /// Called to copy the selected values of either listbox to the clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopySelectionToClipboard(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxUtils.CopySelectionToClipboard(sender, e);
        }
    }
}
