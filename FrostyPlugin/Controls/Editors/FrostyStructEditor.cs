using FrostySdk.Attributes;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                parameter = "{0}";

            Type valueType = value.GetType();
            EbxClassMetaAttribute meta = valueType.GetCustomAttribute<EbxClassMetaAttribute>();

            if (meta != null && meta.Type == FrostySdk.IO.EbxFieldType.Struct)
            {
                DisplayNameAttribute attr = valueType.GetCustomAttribute<DisplayNameAttribute>();
                return (attr != null) ? attr.Name : valueType.Name;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FrostyStructControl : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FrostyStructControl), new FrameworkPropertyMetadata(null));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        static FrostyStructControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyStructControl), new FrameworkPropertyMetadata(typeof(FrostyStructControl)));
        }
    }
    public class FrostyStructEditor : FrostyTypeEditor<FrostyStructControl>
    {
        public FrostyStructEditor()
        {
            ValueProperty = FrostyStructControl.ValueProperty;
            ValueConverter = new FrostyToStringConverter();
            ValueConverterParameter = "{0}";
        }
    }
}
