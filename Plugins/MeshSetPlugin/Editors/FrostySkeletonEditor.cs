using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Controls.Editors;
using FrostySdk.Managers;
using System;
using System.Windows;
using System.Windows.Controls;
using FrostySdk.Managers.Entries;

namespace MeshSetPlugin.Editors
{
    //[TypeEditorUsage(EditorOnly = true)]
    public class FrostySkeletonEditor : FrostyTypeEditor<FrostySkeletonControl>
    {
        public FrostySkeletonEditor()
        {
            ValueProperty = FrostySkeletonControl.ResultProperty;
        }
    }

    [TemplatePart(Name = PART_Popup, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_MoreOptionsButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ClearButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_DataExplorer, Type = typeof(FrostyDataExplorer))]
    public class FrostySkeletonControl : Control
    {
        private const string PART_Popup = "PART_Popup";
        private const string PART_DataExplorer = "PART_DataExplorer";
        private const string PART_MoreOptionsButton = "PART_MoreOptionsButton";
        private const string PART_ClearButton = "PART_ClearButton";

        #region -- Result --
        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(string), typeof(FrostySkeletonControl), new FrameworkPropertyMetadata(""));
        public string Result
        {
            get => (string)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }
        #endregion

        #region -- SelectedItem --
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(EbxAssetEntry), typeof(FrostySkeletonControl), new FrameworkPropertyMetadata(null));
        public EbxAssetEntry SelectedItem
        {
            get => (EbxAssetEntry)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        #endregion

        private ComboBox popupComboBox;
        private FrostyDataExplorer dataExplorer;
        private Button moreOptionsButton;
        private Button clearButton;

        static FrostySkeletonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostySkeletonControl), new FrameworkPropertyMetadata(typeof(FrostySkeletonControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            popupComboBox = GetTemplateChild(PART_Popup) as ComboBox;
            moreOptionsButton = GetTemplateChild(PART_MoreOptionsButton) as Button;
            clearButton = GetTemplateChild(PART_ClearButton) as Button;

            popupComboBox.DropDownOpened += Popup_DropDownOpened;
            moreOptionsButton.Click += OptionsButton_Click;
            clearButton.Click += ClearButton_Click;

            SelectedItem = App.AssetManager.GetEbxEntry(Result);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Result = "";
            SelectedItem = null;
        }

        private void Popup_DropDownOpened(object sender, EventArgs e)
        {
            dataExplorer = popupComboBox.Template.FindName(PART_DataExplorer, popupComboBox) as FrostyDataExplorer;
            dataExplorer.ItemsSource = App.AssetManager.EnumerateEbx(type: "SkeletonAsset");
            dataExplorer.SelectionChanged += DataExplorer_SelectionChanged;
        }

        private void DataExplorer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AssetEntry selectedItem = dataExplorer.SelectedAsset;
            if (selectedItem == null)
            {
                return;
            }

            //label.Text = selectedItem.Name;
            popupComboBox.IsDropDownOpen = false;
            Result = selectedItem.Name;
            SelectedItem = selectedItem as EbxAssetEntry;
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            popupComboBox.IsDropDownOpen = true;
        }
    }
}