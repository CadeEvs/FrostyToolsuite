using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DataField = FrostySdk.Ebx.DataField;

namespace LevelEditorPlugin.Converters
{
    public class DataFieldToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] arr = null;

            if (value is DataField)
            {
                DataField dataField = (DataField)value;
                string strValue = dataField.Value;
                arr = strValue.Split(new[] { ' ' }, 2);
            }
            else
            {
                arr = (value as string).Split(new[] { ' ' }, 2);
            }

            IObjectStringConverter valueConverter = null;
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterface("IObjectStringConverter") != null))
            {
                TypeProviderAttribute typeProvider = type.GetCustomAttribute<TypeProviderAttribute>();
                if (typeProvider == null)
                    continue;

                if (typeProvider.TypeName.Equals(arr[0], StringComparison.OrdinalIgnoreCase))
                {
                    valueConverter = (IObjectStringConverter)Activator.CreateInstance(type);
                    break;
                }
            }

            object retVal = null;
            if (valueConverter != null) retVal = valueConverter.ConvertToObject(arr[1]);
            else retVal = arr[1];

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            IObjectStringConverter valueConverter = null;
            if (value != null)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterface("IObjectStringConverter") != null))
                {
                    TypeProviderAttribute typeProvider = type.GetCustomAttribute<TypeProviderAttribute>();
                    if (typeProvider == null)
                        continue;

                    if (typeProvider.Type == value.GetType())
                    {
                        valueConverter = (IObjectStringConverter)Activator.CreateInstance(type);
                        break;
                    }
                }
            }

            string retVal = "";
            if (valueConverter != null) retVal = valueConverter.ConvertToString(value, true);
            else retVal = value.ToString();

            return retVal;
        }
    }
}
