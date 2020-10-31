using System;
using System.Globalization;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class GameIsStoppedToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            return boolValue.HasValue && boolValue.Value ? 0.7 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}