using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Frosty.Core.Controls.Editors
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExtensionAttribute : FrostySdk.Attributes.EditorMetaDataAttribute
    {
        public string Ext { get; set; }
        public string DisplayName { get; set; }
        public ExtensionAttribute(string inExt, string inDisplayName)
        {
            Ext = inExt;
            DisplayName = inDisplayName;
        }
    }

    //[TypeEditorUsage(EditorOnly = true)]
    public class FrostyImagePathEditor : FrostyTypeEditor<FrostyImagePathControl>
    {
        public FrostyImagePathEditor()
        {
            ValueProperty = FrostyImagePathControl.TextProperty;
        }

        protected override void CustomizeEditor(FrostyImagePathControl editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);

            ExtensionAttribute attr = item.GetCustomAttribute<ExtensionAttribute>();
            if (attr != null)
                editor.SetFilterType(attr.Ext, attr.DisplayName);
        }
    }

    [TemplatePart(Name = PART_BrowseButton, Type = typeof(Button))]
    public class FrostyImagePathControl : Control
    {
        private const string PART_BrowseButton = "PART_BrowseButton";
        private Button browseButton;
        private string filter;

        #region -- Properties --

        #region -- Text --
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FrostyImagePathControl), new FrameworkPropertyMetadata(""));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #endregion

        static FrostyImagePathControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyImagePathControl), new FrameworkPropertyMetadata(typeof(FrostyImagePathControl)));
        }

        public void SetFilterType(string ext, string displayName)
        {
            filter = string.Format("*.{0} ({1})|*.{0}", ext, displayName);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = false;

            browseButton = GetTemplateChild(PART_BrowseButton) as Button;
            if (browseButton != null)
            {
                browseButton.Click += BrowseButton_Click;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = filter};

            if (ofd.ShowDialog() == true)
            {
                Text = ofd.FileName;
            }
        }
    }
}
