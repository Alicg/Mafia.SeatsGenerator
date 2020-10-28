using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Headers;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class IsLastCollectionElementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = values[0] as IList;
            var element = values[1];
            return collection != null && element != null && collection[^1] == element;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}