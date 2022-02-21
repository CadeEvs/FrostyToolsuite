using LegacyDatabasePlugin.Database;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LegacyDatabasePlugin.Converters
{
    public class ColumnRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LegacyDbRow row = value as LegacyDbRow;
            string columnName = (string)parameter;
            if (row[columnName] == null)
                return "";
            return row[columnName].ToString().Replace('\n', ' ').Replace('\r', ' ').Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
