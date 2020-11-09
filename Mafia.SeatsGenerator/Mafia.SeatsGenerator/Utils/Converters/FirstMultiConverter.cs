using System;
using System.Globalization;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class FirstMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}