using System.Collections.ObjectModel;
using System.Windows.Input;
using Mafia.SeatsGenerator.Models;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class PlayersSetupPageViewModel
    {
        public PlayersSetupPageViewModel(ObservableCollection<Player> players)
        {
            this.Players = players;
        }
        
        public ObservableCollection<Player> Players { get; }

        public ICommand AddPlayerCommand => new Command(this.AddPlayer);
        public ICommand RemovePlayerCommand => new Command<Player>(this.RemovePlayer);
        
        private void AddPlayer()
        {
            var visitor = new Player();
            this.Players.Add(visitor);
            this.RecalculateNumbers();
        }

        private void RemovePlayer(Player player)
        {
            this.Players.Remove(player);
            this.RecalculateNumbers();
        }

        private void RecalculateNumbers()
        {
            for (var index = 0; index < this.Players.Count; index++)
            {
                var player = this.Players[index];
                player.Number = index + 1;
            }
        }
    }
}