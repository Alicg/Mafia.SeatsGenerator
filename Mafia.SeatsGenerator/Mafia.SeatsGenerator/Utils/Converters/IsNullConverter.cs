using System;
using System.Globalization;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class IsNullConverter : IValueConverter
    {
        public static string ModeNotNull = "NotNull";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = parameter as string;
            if (mode == ModeNotNull)
            {
                return value != null;
            }

            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}