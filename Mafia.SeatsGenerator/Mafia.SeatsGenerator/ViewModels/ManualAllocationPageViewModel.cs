using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class ManualAllocationPageViewModel : BindableObject
    {
        private readonly Random randomGenerator = new Random();
        private readonly SourceList<Player> playersToAllocate = new SourceList<Player>();
        
        private readonly Subject<IComparer<Player>> sortChangesObservable = new Subject<IComparer<Player>>();
        private readonly SortExpressionComparer<Player> byGamesComparer = SortExpressionComparer<Player>.Ascending(p => p.PlayedGames);
        private readonly SortExpressionComparer<Player> byHostComparer = SortExpressionComparer<Player>.Descending(p => p.HostedGames);
        private readonly SortExpressionComparer<Player> bySortingValueComparer = SortExpressionComparer<Player>.Ascending(p => p.SortingValue);
        
        public ManualAllocationPageViewModel(PlayersSetupPageViewModel playersSetupPageViewModel, RoomsPageViewModel roomsPageViewModel)
        {
            this.RoomsPageViewModel = roomsPageViewModel;
            this.playersToAllocate.AddRange(playersSetupPageViewModel.Players);
            playersSetupPageViewModel.Players.ObserveCollectionChanges().Subscribe(v =>
            {
                if (v.EventArgs.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.playersToAllocate.Clear();
                    this.AddPlayersToNotBusyList(playersSetupPageViewModel.Players);
                    return;
                }
                if (v.EventArgs.NewItems?.Count > 0)
                {
                    this.AddPlayersToNotBusyList(v.EventArgs.NewItems.Cast<Player>());
                }

                if (v.EventArgs.OldItems?.Count > 0)
                {
                    this.playersToAllocate.RemoveMany(v.EventArgs.OldItems.Cast<Player>());
                }
            });
            this.playersToAllocate.Connect().Sort(this.byGamesComparer, comparerChanged: this.sortChangesObservable).Bind(out var players).Subscribe();
            this.Players = players;
        }

        public string Title => "Вручную";

        public ReadOnlyObservableCollection<Player> Players { get; }
        
        public RoomsPageViewModel RoomsPageViewModel { get; }
        
        public ICommand MoveToBottomCommand => new Command<Player>(this.MoveToBottom);
        
        public ICommand CreateNewGameCommand => new Command<RoomPageViewModel>(this.CreateNewGame);
        
        public ICommand AddPlayersToGameCommand => new Command<AddPlayersToGameObject>(this.AddPlayersToGame);
        
        public ICommand SetGameHostCommand => new Command<SetGameHostObject>(this.SetGameHost);
        
        public ICommand NavigateToRoomCommand => new Command<RoomPageViewModel>(this.OnNavigateToRoomPage);
        
        public ICommand SortByCommand => new Command<string>(this.SortBy);

        public event EventHandler<RoomPageViewModel> NavigateToRoomPage; 

        private void MoveToBottom(Player player)
        {
            var maxSortingValue = this.playersToAllocate.Items.Max(v => v.SortingValue);
            player.SortingValue = maxSortingValue + 1;
            this.sortChangesObservable.OnNext(this.bySortingValueComparer);
        }

        private void CreateNewGame(RoomPageViewModel roomPageViewModel)
        {
            roomPageViewModel.CreateEmptyGame();
        }

        private void AddPlayersToGame(AddPlayersToGameObject playersToAdd)
        {
            var freeSlots = 10 - playersToAdd.Game.Members.Count;
            var slotsToFree = playersToAdd.Players.Count() - freeSlots;
            if (slotsToFree > 0)
            {
                var maxPlayedPlayers = playersToAdd.Game.Members.OrderBy(v => v.Player.PlayedGames).ThenBy(v => this.randomGenerator.Next(0, 9)).TakeLast(slotsToFree).ToList();
                var playersToAddList = playersToAdd.Players.ToList();
                for (var index = 0; index < maxPlayedPlayers.Count; index++)
                {
                    var maxPlayedPlayer = maxPlayedPlayers[index];
                    var playerToAdd = playersToAddList[index];
                    playersToAdd.Game.ReplacePlayers(maxPlayedPlayer, playerToAdd);
                }
            }
        }

        private void SetGameHost(SetGameHostObject setGameHostObject)
        {
            var firstHost = setGameHostObject.Players.FirstOrDefault();
            if (firstHost != null)
            {
                firstHost.CanBeHost = true;
                setGameHostObject.Game.SetHost(firstHost);
            }
        }

        private void AddPlayersToNotBusyList(IEnumerable<Player> players)
        {
            var enumerablePlayers = players as Player[] ?? players.ToArray();
            
            this.playersToAllocate.AddRange(enumerablePlayers);
            foreach (var player in enumerablePlayers)
            {
                player.WhenPropertyChanged(v => v.IsBusy).Subscribe(v =>
                {
                    if (v.Sender.IsBusy)
                    {
                        this.playersToAllocate.Remove(v.Sender);
                    }
                    else if (!this.playersToAllocate.Items.Contains(v.Sender))
                    {
                        this.playersToAllocate.Add(v.Sender);
                    }
                });
            }
        }

        private void SortBy(string column)
        {
            this.playersToAllocate.Items.ForEach(v => v.SortingValue = 0);
            switch (column)
            {
                case "games":
                {
                    this.sortChangesObservable.OnNext(this.byGamesComparer);
                    break;
                }
                case "host":
                {
                    this.sortChangesObservable.OnNext(this.byHostComparer);
                    break;
                }
            }
        }

        protected virtual void OnNavigateToRoomPage(RoomPageViewModel e)
        {
            this.NavigateToRoomPage?.Invoke(this, e);
        }

        public class AddPlayersToGameObject
        {
            public IEnumerable<Player> Players { get; set; }
            
            public Game Game { get; set; }
        }
        
        public class SetGameHostObject
        {
            public IEnumerable<Player> Players { get; set; }
            
            public Game Game { get; set; }
        }
        
        private enum SortingColumn
        {
            ByPlayedGames = 1,
            ByCanBeHost = 2
        }
    }
}