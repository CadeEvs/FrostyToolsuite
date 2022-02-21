using System;
using System.Globalization;
using System.Windows.Data;

namespace Frosty.Core.Converters
{
    public class StringToPathDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string str = value as string;
            //Type type = TypeLibrary.GetType(str);

            //IconAttribute iconAttr = type.GetCustomAttribute<IconAttribute>();
            //if (iconAttr == null)
            //    return (App.Current.Resources["BlankAsset"] as System.Windows.Shapes.Path).Data as Geometry;

            //return (App.Current.Resources[iconAttr.Icon] as System.Windows.Shapes.Path).Data as Geometry;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
