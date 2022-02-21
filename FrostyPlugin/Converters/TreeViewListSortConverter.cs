using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Frosty.Core.Converters
{
    public class TreeViewListSortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList collection = (IList)value;
            ListCollectionView view = new ListCollectionView(collection);
            view.SortDescriptions.Add(new SortDescription(parameter.ToString(), ListSortDirection.Ascending));

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw null;
    }
}
