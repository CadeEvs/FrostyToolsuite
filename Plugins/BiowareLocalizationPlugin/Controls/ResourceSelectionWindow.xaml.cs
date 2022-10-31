using Frosty.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BiowareLocalizationPlugin.Controls
{
    /// <summary>
    /// Interaktionslogik für ResourceSelectionWindow.xaml
    /// </summary>
    [TemplatePart(Name = "resourcesListBox", Type =typeof(ListBox))]
    public partial class ResourceSelectionWindow : FrostyDockableWindow
    {

        /// <summary>
        /// The list of selected resources that is the return value of this dialog.
        /// </summary>
        public List<string> SelectedResources { get; private set; }

        /// <summary>
        /// Creates a new resource selection dialog.
        /// </summary>
        /// <param name="textDB">The db of localized strings.</param>
        /// <param name="languageFormat">The current language format.</param>
        /// <param name="resourcesToShow">The list of already assigned resources, that do not need to be displayed or selectable here.</param>
        public ResourceSelectionWindow(IEnumerable<string> resourcesToShow)
        {
            InitializeComponent();
            Loaded += (s, e) => ListBoxUtils.SortListIntoListBox(resourcesToShow, resourcesListBox);
        }

        private void Save(object sender, RoutedEventArgs e)
        {

            SelectedResources = new List<string>();
            foreach(string selected in resourcesListBox.SelectedItems)
            {
                SelectedResources.Add(selected);
            }
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
