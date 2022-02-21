using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frosty.Controls
{
    [TemplatePart(Name = PART_Label, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class FrostyEditableLabel : Control
    {
        private const string PART_Label = "PART_Label";
        private const string PART_TextBox = "PART_TextBox";

        #region -- Properties --

        #region -- Text --
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FrostyEditableLabel));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #endregion

        private TextBlock Label = null;
        private TextBox TextBox = null;

        public event EventHandler EditEnded = null;

        static FrostyEditableLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyEditableLabel), new FrameworkPropertyMetadata(typeof(FrostyEditableLabel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Label = GetTemplateChild(PART_Label) as TextBlock;
            TextBox = GetTemplateChild(PART_TextBox) as TextBox;
            TextBox.LostFocus += TextBox_LostFocus;
            TextBox.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    EndEdit();
                }
                if (e.Key == Key.Escape)
                {
                    CancelEdit();
                }
            };
            this.PreviewMouseDoubleClick += (s, e) =>
            {
                if (Label.Visibility == Visibility.Visible)
                    BeginEdit();
                e.Handled = true;
            };
            Label.Padding = Text?.Length > 0 ? new Thickness(2, 0, 12, 0) : new Thickness(0);
        }

        /// <summary>
        /// Switches from being a label to being a textbox.
        /// </summary>
        public void BeginEdit()
        {
            ApplyTemplate();
            UpdateLayout();
            TextBox.Text = Text;
            TextBox.Visibility = Visibility.Visible;
            Label.Visibility = Visibility.Collapsed;
            TextBox.Focus();
            TextBox.SelectAll();
        }

        /// <summary>
        /// Switches from being a textbox to being a label, and saves the new text.
        /// </summary>
        public void EndEdit()
        {
            Text = TextBox.Text;
            TextBox.Visibility = Visibility.Collapsed;
            Label.Visibility = Visibility.Visible;
            EditEnded?.Invoke(this, new EventArgs());
            Label.Padding = Text.Length > 0 ? new Thickness(2, 0, 8, 0) : new Thickness(0);
        }

        /// <summary>
        /// Switches from being a textbox to being a label without saving the contents of the textbox
        /// </summary>
        public void CancelEdit()
        {
            TextBox.Visibility = Visibility.Collapsed;
            Label.Visibility = Visibility.Visible;
            EditEnded?.Invoke(this, new EventArgs());
            Label.Padding = Text.Length > 0 ? new Thickness(2, 0, 8, 0) : new Thickness(0);
        }

        private void TextBox_LostFocus(object sender, EventArgs args)
        {
            EndEdit();
        }
    }
}
