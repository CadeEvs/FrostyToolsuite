using Frosty.Core.Misc;
using System.Windows;
using System.Windows.Controls;

namespace Frosty.Core.Controls.Editors
{
    public class BaseCustomComboDataControl : Control
    {
        protected const string PART_ComboBox = "PART_ComboBox";
        protected ComboBox comboBox;

        #region -- Properties --

        #region -- Value -- 
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BaseCustomComboDataControl), new FrameworkPropertyMetadata(null));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #endregion

        static BaseCustomComboDataControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseCustomComboDataControl), new FrameworkPropertyMetadata(typeof(BaseCustomComboDataControl)));
        }
    }
    public class FrostyCustomComboDataControl<T, U> : BaseCustomComboDataControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = false;
            comboBox = GetTemplateChild(PART_ComboBox) as ComboBox;
            comboBox.SelectedIndex = ((CustomComboData<T, U>)Value).SelectedIndex;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int newIndex = comboBox.SelectedIndex;
            if (newIndex == -1)
                return;

            CustomComboData<T, U> data = (CustomComboData<T, U>)Value;
            data = new CustomComboData<T, U>(data.Values, data.Names) {SelectedIndex = newIndex};

            Value = data;
        }
    }
    public class FrostyCustomComboDataEditor<T, U> : FrostyTypeEditor<FrostyCustomComboDataControl<T, U>>
    {
        public FrostyCustomComboDataEditor()
        {
            ValueProperty = BaseCustomComboDataControl.ValueProperty;
        }
    }
}
