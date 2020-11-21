using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Mafia.SeatsGenerator.Models;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils.Converters
{
    public class NameAndSeatNumberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var playerInGame = values[0] as PlayerInGame;
            var gameMembers = values[1] as ObservableCollection<PlayerInGame>;

            if (gameMembers != null && playerInGame != null)
            {
                return $"{gameMembers.IndexOf(playerInGame) + 1}. {playerInGame.Player.Name}";
            }

            return "пусто";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}