using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;
using DynamicData;
using DynamicData.Binding;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Mafia.SeatsGenerator.Utils;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class PlayersSetupPageViewModel : BindableObject
    {
        private readonly ReadOnlyObservableCollection<Player> readOnlyObservableCollection;
        private readonly PopupService popupService;
        
        private readonly SourceList<Player> playersSourceList = new SourceList<Player>(); 

        public PlayersSetupPageViewModel(PopupService popupService)
        {
            this.popupService = popupService;

            this.playersSourceList.Connect().Bind(out this.readOnlyObservableCollection).Subscribe();
        }

        public string Title => "Игроки";

        public int LeftBadgeValue => this.Players.Count(v => !v.IsBusy);
        
        public int RightBadgeValue => this.Players.Count(v => v.IsBusy);

        public ReadOnlyObservableCollection<Player> Players => this.readOnlyObservableCollection;

        public ICommand AddPlayerCommand => new Command(this.AddPlayer);
        public ICommand RemovePlayerCommand => new Command<Player>(this.RemovePlayerExecute);

        public void ClearPlayers()
        {
            this.playersSourceList.Clear();
            
            this.RecalculateNumbers();
            
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
        }

        public void AddRangePlayers(Player[] players)
        {
            this.playersSourceList.AddRange(players);
            
            this.RecalculateNumbers();
            
            foreach (var player in players)
            {
                player.OnAnyPropertyChanges(nameof(Player.IsBusy)).Subscribe(_ =>
                {
                    this.OnPropertyChanged(nameof(this.LeftBadgeValue));
                    this.OnPropertyChanged(nameof(this.RightBadgeValue));
                });
            }
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
        }
        
        private void AddPlayer()
        {
            var visitor = new Player();
            this.AddPlayerInternal(visitor);
            this.RecalculateNumbers();
            
            this.OnPropertyChanged(nameof(this.LeftBadgeValue));
            this.OnPropertyChanged(nameof(this.RightBadgeValue));
        }

        private async void RemovePlayerExecute(Player player)
        {
            var confirmed = await this.popupService.ConfirmationPopup($"Удалить игрока '{player.Name}'?", "Требуется подтверждение"); 
            if (confirmed.HasValue && confirmed.Value)
            {
                this.playersSourceList.Remove(player);
                this.RecalculateNumbers();
            
                this.OnPropertyChanged(nameof(this.LeftBadgeValue));
                this.OnPropertyChanged(nameof(this.RightBadgeValue));
            }
        }

        private void RecalculateNumbers()
        {
            for (var index = 0; index < this.Players.Count; index++)
            {
                var player = this.Players[index];
                player.Number = index + 1;
            }
        }

        private void AddPlayerInternal(Player visitor)
        {
            this.playersSourceList.Add(visitor);
            visitor.OnAnyPropertyChanges(nameof(Player.IsBusy)).Subscribe(_ =>
            {
                this.OnPropertyChanged(nameof(this.LeftBadgeValue));
                this.OnPropertyChanged(nameof(this.RightBadgeValue));
            });
        }
    }
}