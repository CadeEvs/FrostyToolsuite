using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FrostySdk.Converters
{
    public class FunctionBasedValueConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBasedValueConverter"/> class.
        /// </summary>
        public FunctionBasedValueConverter()
        {
        }

        /// <summary>
        /// Converts a given value through a provided <see cref="Func{T, TResult}"/>, allowing for custom converters without the need for a new class.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="targetType">Invalid for this converter.</param>
        /// <param name="parameter">The <see cref="Func{T, TResult}"/> to be used.</param>
        /// <param name="culture">Invalid for this converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || !(parameter is Func<object, object>))
            {
                throw new ArgumentException("\"parameter\" is null or not of the type \"Func<object, object>\".");
            }

            return ((Func<object, object>)parameter).Invoke(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
