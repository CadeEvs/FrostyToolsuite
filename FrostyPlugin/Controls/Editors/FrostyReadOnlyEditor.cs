using FrostySdk.Attributes;
using FrostySdk.Ebx;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyReadOnlyEditor : FrostyTypeEditor<TextBlock>
    {
        private class DisplayStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Type valueType = value.GetType();
                if (valueType == typeof(string))
                    return (string)value;
                
                if (valueType == typeof(BoxedValueRef) || valueType == typeof(FileRef) || valueType == typeof(TypeRef))
                    return value.ToString();
                
                if (valueType.GetCustomAttribute<DisplayNameAttribute>() != null)
                {
                    DisplayNameAttribute attr = valueType.GetCustomAttribute<DisplayNameAttribute>();
                    return attr.Name;
                }

                return value.GetType().Name;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return null;
            }
        }

        public FrostyReadOnlyEditor()
        {
            ValueProperty = TextBlock.TextProperty;
            ValueConverter = new DisplayStringConverter();
        }

        protected override void CustomizeEditor(TextBlock editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xf7, 0xf7, 0xf7));
        }
    }
}
