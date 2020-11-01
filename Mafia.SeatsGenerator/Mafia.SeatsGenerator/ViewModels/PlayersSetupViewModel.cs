using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Mafia.SeatsGenerator.Models;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class PlayersSetupPageViewModel : BindableObject
    {
        public PlayersSetupPageViewModel(ObservableCollection<Player> players)
        {
            this.Players = players;
        }

        public string Title => "Игроки";

        public int LeftBadgeValue => this.Players.Count(v => !v.IsBusy);
        
        public int RightBadgeValue => this.Players.Count(v => v.IsBusy);
        
        public ObservableCollection<Player> Players { get; }

        public ICommand AddPlayerCommand => new Command(this.AddPlayer);
        public ICommand RemovePlayerCommand => new Command<Player>(this.RemovePlayer);

        public void ClearPlayers()
        {
            foreach (var player in this.Players)
            {
                this.Players.Remove(player);
            }
            
            this.RecalculateNumbers();
            
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
        }

        public void AddRangePlayers(Player[] players)
        {
            foreach (var player in players)
            {
                this.Players.Add(player);
            }
            
            this.RecalculateNumbers();
            
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
        }
        
        private void AddPlayer()
        {
            var visitor = new Player();
            this.Players.Add(visitor);
            this.RecalculateNumbers();
            
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
        }

        private void RemovePlayer(Player player)
        {
            this.Players.Remove(player);
            this.RecalculateNumbers();
            
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
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