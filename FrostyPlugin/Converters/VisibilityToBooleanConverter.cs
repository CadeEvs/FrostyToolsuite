using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Frosty.Core.Converters
{
    public class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
    }
}
