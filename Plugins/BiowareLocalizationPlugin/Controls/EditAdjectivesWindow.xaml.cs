﻿using BiowareLocalizationPlugin.LocalizedResources;
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
    [TemplatePart(Name = "adjectiveIdField", Type = typeof(TextBox))]
    [TemplatePart(Name = "localizedAdjectiveListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "resourcesListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "saveButton", Type = typeof(Button))]
    [TemplatePart(Name = "cancelButton", Type = typeof(Button))]
    public partial class EditAdjectivesWindow : FrostyDockableWindow
    {

        private readonly string _selectedLanguageFormat;
        private readonly BiowareLocalizedStringDatabase _stringDb;

        /// <summary>
        /// The resource(s?) where to edit the adjective
        /// </summary>
        private string _selectedResource;

        /// <summary>
        /// The save value tuple consiting of the adjective id, and the declinations or the adjective. This is only available after the save action!
        /// </summary>
        public Tuple<uint, List<string>> SaveValue { get; private set; }

        public EditAdjectivesWindow(BiowareLocalizedStringDatabase stringDb, string languageFormat, string resource)
        {
            InitializeComponent();

            _selectedLanguageFormat = languageFormat;
            _stringDb = stringDb;

            _selectedResource = resource;
        }

        /// <summary>
        /// Initializes the window with the currently selected textid and text.
        /// </summary>
        /// <param name="adjectiveId"></param>
        public void Init(uint adjectiveId)
        {

            if (adjectiveId != 0)
            {
                adjectiveIdField.Text = adjectiveId.ToString("X8");
            }
            UpdateData(adjectiveId);

            resourcesListBox.Items.Add(_selectedResource);
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            uint adjectiveId = ReadAdjectiveId();
            UpdateData(adjectiveId);
        }

        /// <summary>
        /// Updates the listed data
        /// </summary>
        private void UpdateData(uint adjectiveId)
        {

            if (adjectiveId == 0 || _selectedResource == null)
            {
                // revert;
                localizedAdjectiveListBox.Items.Clear();
            }
            else
            {

                List<string> declinations = _stringDb.GetDeclinatedAdjectives(_selectedLanguageFormat, _selectedResource, adjectiveId);

                foreach (string declination in declinations)
                {
                    TextBox declinationBox = new TextBox
                    {
                        Text = declination,
                        IsReadOnly = false,
                        AcceptsReturn = false,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    localizedAdjectiveListBox.Items.Add(declinationBox);
                }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            uint adjectiveId = ReadAdjectiveId();

            if (adjectiveId != 0 && _selectedResource != null)
            {

                List<string> adjectiveDeclinations = new List<string>();
                foreach(TextBox declinationBox in localizedAdjectiveListBox.Items)
                {
                    adjectiveDeclinations.Add(declinationBox.Text);
                }
                _stringDb.SetDeclinatedAdjectve(_selectedLanguageFormat, _selectedResource, adjectiveId, adjectiveDeclinations);

                SaveValue = Tuple.Create(adjectiveId, adjectiveDeclinations);


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
        private uint ReadAdjectiveId()
        {
            string text = adjectiveIdField.Text;
            bool canRead = uint.TryParse(text, NumberStyles.HexNumber, null, out uint textId);
            if (canRead)
            {
                return textId;
            }

            adjectiveIdField.Text = "";
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
                .Where(r => !resourcesListBox.Items.Contains(r));

            ResourceSelectionWindow selectionDialog = new ResourceSelectionWindow(selectableResources);
            bool? saved = selectionDialog.ShowDialog();

            if (saved != null && saved.Value)
            {

                int selectionCount = selectionDialog.SelectedResources.Count;

                if (selectionCount == 0)
                {
                    return;
                }

                // TODO multiSelect is actually not allowed here!
                if( selectionDialog.SelectedResources.Count > 1 )
                {
                    App.Logger.LogError("Can only select a single resource for adjectives at the moment!");
                }

                string resourceName = selectionDialog.SelectedResources[0];
                resourcesListBox.Items.Add(resourceName);
                _selectedResource = resourceName;
            }
        }

        private void RemoveResources(object sender, RoutedEventArgs e)
        {

            List<string> selectedToRemove = resourcesListBox.SelectedItems.OfType<string>().ToList();
            foreach (string itemToRemove in selectedToRemove)
            {
                resourcesListBox.Items.Remove(itemToRemove);
            }
            _selectedResource = null;
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
