using FrostySdk.Managers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Frosty.Core.Windows;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Converters
{
    public class AssetEntryAndSizeToBitmapSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            AssetEntry entry = values[0] as AssetEntry;

            double width = (values[1] != DependencyProperty.UnsetValue) ? (double)values[1] : double.PositiveInfinity;
            double height = (values[2] != DependencyProperty.UnsetValue) ? (double)values[2] : double.PositiveInfinity;

            if (entry != null)
            {
                string sourceName = entry.Type;

                var definition = App.PluginManager.GetAssetDefinition(sourceName);
                return definition != null ? definition.GetIcon(entry, width, height) : new StringToBitmapSourceConverter().Convert(sourceName, targetType, parameter, culture);
            }
            return new StringToBitmapSourceConverter().Convert(values[0] as string, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AssetEntryToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AssetEntry entry)
            {
                string sourceName = entry.Type;

                var definition = App.PluginManager.GetAssetDefinition(sourceName);
                return definition != null ? definition.GetIcon(entry) : new StringToBitmapSourceConverter().Convert(sourceName, targetType, parameter, culture);
            }
            
            if (value is AssetInstanceInfo instance)
            {
                return new StringToBitmapSourceConverter().Convert(instance.Type, targetType, parameter, culture);
            }

            AssetInstanceInfo info = value as AssetInstanceInfo;
            if (info != null)
            {
                string sourceName = info.Type;

                var definition = App.PluginManager.GetAssetDefinition(sourceName);
                if (definition != null)
                {
                    return definition.GetIcon();
                }
                return new StringToBitmapSourceConverter().Convert(sourceName, targetType, parameter, culture);
            }

            return new StringToBitmapSourceConverter().Convert(value as string, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
