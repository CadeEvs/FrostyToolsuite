using Frosty.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Frosty.Core.Controls.Editors
{
    class FrostyGuidEditor : FrostyTypeEditor<TextBox>
    {
        private class NumberToStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)parameter;
                string strValue = (string)value;

                if (!Guid.TryParse(strValue, out Guid tmpValue))
                {
                    FrostyMessageBox.Show("Value not in the correct format for a Guid", "Frosty Editor");
                    return (Guid)item.Value;
                }

                return tmpValue;
            }
        }

        public FrostyGuidEditor()
        {
            ValueProperty = TextBox.TextProperty;
            ValueConverter = new NumberToStringConverter();
        }

        protected override void CustomizeEditor(TextBox editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Padding = new Thickness(0);
            editor.Background = new SolidColorBrush(new Color() { A = 0, R = 0, G = 0, B = 0 });
            editor.Height = 22;
            editor.VerticalContentAlignment = VerticalAlignment.Center;
            editor.Margin = new Thickness(-2, 0, 0, 0);
        }
    }
}
