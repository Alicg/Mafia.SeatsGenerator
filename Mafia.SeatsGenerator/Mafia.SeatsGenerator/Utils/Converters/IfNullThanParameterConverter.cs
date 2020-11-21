using System;
using System.Globalization;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class IfNullThanParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}