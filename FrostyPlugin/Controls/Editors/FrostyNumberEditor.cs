using Frosty.Controls;
using Frosty.Hash;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Frosty.Core.Controls.Editors
{
    class FrostyNumberEditor : FrostyTypeEditor<FrostyEllipsedTextBox>
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
                Type valueType = item.Value.GetType();

                string strValue = (string)value;
                if (strValue == "" || strValue == null)
                    strValue = "0";

                if (valueType == typeof(sbyte)) return sbyte.Parse(strValue);
                else if (valueType == typeof(byte)) return byte.Parse(strValue);
                else if (valueType == typeof(short)) return short.Parse(strValue);
                else if (valueType == typeof(ushort)) return ushort.Parse(strValue);
                else if (valueType == typeof(int))
                {
                    int tmpValue = 0;
                    if (strValue != "")
                    {
                        if (!int.TryParse(strValue, out tmpValue))
                        {
                            tmpValue = ConvertOrHashInt(strValue);
                        }
                    }
                    return tmpValue;
                }
                else if (valueType == typeof(uint))
                {
                    uint tmpValue = 0;
                    if (strValue != "")
                    {
                        if (!uint.TryParse(strValue, out tmpValue))
                        {
                            tmpValue = (uint)ConvertOrHashInt(strValue);
                        }
                    }
                    return tmpValue;
                }
                else if (valueType == typeof(long)) return long.Parse(strValue);
                else if (valueType == typeof(ulong)) return ulong.Parse(strValue);

                else if (valueType == typeof(float))
                {
                    if (float.TryParse(strValue, out float result))
                    {
                        return result;
                    }
                    else if (TrySolve(strValue, out double mathResult))
                    {
                        return (float)mathResult;
                    }
                }
                else if (valueType == typeof(double))
                {
                    if (double.TryParse(strValue, out double result))
                    {
                        return result;
                    }
                    else if (TrySolve(strValue, out double mathResult))
                    {
                        return mathResult;
                    }
                }

                return 0;
            }

            private int ConvertOrHashInt(string value)
            {
                if (value.StartsWith("0x"))
                {
                    value = value.Remove(0, 2);
                    return int.Parse(value, NumberStyles.HexNumber);
                }
                else
                {
                    value = value.Trim('\"');
                    return Fnv1.HashString(value);
                }
            }
        }
        private class NumberValidationRule : ValidationRule
        {
            private Type type;
            public NumberValidationRule(Type inType)
            {
                type = inType;
            }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                try
                {
                    string strValue = (string)value;
                    if (strValue == "" || strValue == null)
                        return new ValidationResult(true, null);

                    if (type == typeof(sbyte)) sbyte.Parse(strValue);
                    else if (type == typeof(byte)) byte.Parse(strValue);
                    else if (type == typeof(short)) short.Parse(strValue);
                    else if (type == typeof(ushort)) ushort.Parse(strValue);
                    else if (type == typeof(int))
                    {
                        try { int.Parse(strValue); }
                        catch (OverflowException) { return new ValidationResult(false, null); }
                        catch (Exception) { }
                        return new ValidationResult(true, null);
                    }
                    else if (type == typeof(uint))
                    {
                        try { uint.Parse(strValue); }
                        catch (OverflowException) { return new ValidationResult(false, null); }
                        catch (Exception) { }
                        return new ValidationResult(true, null);
                    }
                    else if (type == typeof(long)) long.Parse(strValue);
                    else if (type == typeof(ulong)) ulong.Parse(strValue);

                    else if (type == typeof(float)) 
                    {
                        if (float.TryParse(strValue, out float _) || TrySolve(strValue, out double _))
                        {
                            return new ValidationResult(true, null);
                        }
                        else
                        {
                            return new ValidationResult(false, null);
                        }
                    }
                    else if (type == typeof(double))
                    {
                        if (double.TryParse(strValue, out double _) || TrySolve(strValue, out double _))
                        {
                            return new ValidationResult(true, null);
                        }
                        else
                        {
                            return new ValidationResult(false, null);
                        }
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult(false, null);
                }
                return new ValidationResult(true, null);
            }
        }

        public FrostyNumberEditor()
        {
            ValueProperty = TextBox.TextProperty;
            ValueConverter = new NumberToStringConverter();
        }

        protected override void CustomizeEditor(FrostyEllipsedTextBox editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            ValidationRule = new NumberValidationRule(item.Value.GetType());

            editor.Padding = new Thickness(0);
            editor.Background = new SolidColorBrush(new Color() { A = 0, R = 0, G = 0, B = 0 });
            editor.Height = 22;
            editor.VerticalContentAlignment = VerticalAlignment.Center;
            editor.Margin = new Thickness(-2, 0, 0, 0);
            editor.GotKeyboardFocus += (s, o) => { editor.SelectAll(); };
            editor.AcceptsReturn = false;
        }

        private static bool TrySolve(string expression, out double result)
        {
            try
            {
                result = Convert.ToDouble(new DataTable().Compute(expression, null));
                return true;
            }
            catch
            {
                result = double.NaN;
                return false;
            }
        }
    }
}
