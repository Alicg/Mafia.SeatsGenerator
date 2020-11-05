using System;
using System.Globalization;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class CanBeHostToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var canBeHost = value as bool?;
            return canBeHost.HasValue && canBeHost.Value ? Color.Gold : Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}