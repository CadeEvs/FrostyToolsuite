using Frosty.Controls;
using FrostySdk.Ebx;
using System.Windows;
using System.Windows.Data;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyCStringEditor : FrostyTypeEditor<FrostyCStringControl>
    {
        public FrostyCStringEditor()
        {
            ValueProperty = FrostyCStringControl.ValueProperty;
            BindingMode = BindingMode.TwoWay;
            NotifyOnTargetUpdated = true;
        }
    }

    public class FrostyCStringControl : FrostyEllipsedTextBox
    {
        #region -- Properties --

        #region -- Value --
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(CString), typeof(FrostyCStringControl), new FrameworkPropertyMetadata(new CString()));
        public CString Value
        {
            get => (CString)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotFocus += FrostyCStringControl_GotFocus;
            LostFocus += FrostyCStringControl_LostFocus;
            TargetUpdated += FrostyCStringControl_TargetUpdated;

            ShowStringDisplay();
        }

        private void FrostyCStringControl_LostFocus(object sender, RoutedEventArgs e)
        {
            string oldStr = Value;
            string newStr = Text;

            if(!oldStr.Equals(newStr))
                Value = new CString(newStr);

            ShowStringDisplay();
            //e.Handled = true;
        }

        private void FrostyCStringControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Text = Value;
            ToolTip = null;
            SelectAll();
            //e.Handled = true;
        }

        public void ShowStringDisplay()
        {
            string value = Value;
            if (value.StartsWith("ID_"))
            {
                Text = LocalizedStringDatabase.Current.GetString(value);
                ToolTip = value;
            }
            else
                Text = value;
        }

        private void FrostyCStringControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            Text = Value;
            ShowStringDisplay();
        }
    }
}
