using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyEnumEditor : FrostyTypeEditor<FrostyEnumControl>
    {
        public FrostyEnumEditor()
        {
            ValueProperty = FrostyEnumControl.ValueProperty;
        }
    }

    [TemplatePart(Name = PART_ComboBox, Type = typeof(ComboBox))]
    public class FrostyEnumControl : Control
    {
        public class FrostyEnumToStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Type enumType = parameter as Type;
                return Enum.Parse(enumType, value as string);
            }
        }

        private const string PART_ComboBox = "PART_ComboBox";
        private ComboBox comboBox;
        private string[] comboBoxValues;

        #region -- Properties --

        #region -- Value -- 
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FrostyEnumControl), new FrameworkPropertyMetadata(null, OnValueChanged));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrostyEnumControl ctrl = o as FrostyEnumControl;

            dynamic value = e.NewValue;
            Type valueType = value.GetType();

            ctrl.comboBoxValues = valueType.GetEnumNames();
        }
        #endregion

        #endregion

        static FrostyEnumControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyEnumControl), new FrameworkPropertyMetadata(typeof(FrostyEnumControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = false;

            comboBox = GetTemplateChild(PART_ComboBox) as ComboBox;
            if (comboBox != null)
            {
                foreach (string str in comboBoxValues)
                    comboBox.Items.Add(str);

                Binding b = new Binding("Value")
                {
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    Converter = new FrostyEnumToStringConverter(),
                    ConverterParameter = Value.GetType()
                };

                BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, b);
            }
        }
    }
}
