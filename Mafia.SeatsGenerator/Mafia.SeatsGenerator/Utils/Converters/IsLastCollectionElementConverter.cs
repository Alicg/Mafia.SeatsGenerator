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
        public static string Inverted = "inverted";
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = values[0] as IList;
            if (collection?.Count == 0)
            {
                return false;
            }
            var lastElement = collection?[^1];
            var element = values[1];
            if (parameter is string inverted && inverted == Inverted)
            {
                return lastElement != null && element != null && lastElement != element;
            }

            var ret = lastElement != null && element != null && lastElement == element; 
            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}