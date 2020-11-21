using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Utils;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class ManualAllocationPageViewModel : BindableObject
    {
        private readonly SourceList<Player> playersToAllocate = new SourceList<Player>();
        
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
            this.playersToAllocate.Connect().Bind(out var players).Subscribe();
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

        public event EventHandler<RoomPageViewModel> NavigateToRoomPage; 

        private void MoveToBottom(Player player)
        {
            var currentIndex = this.playersToAllocate.Items.IndexOf(player);
            this.playersToAllocate.Move(currentIndex, this.playersToAllocate.Count - 1);
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
                var maxPlayedPlayers = playersToAdd.Game.Members.OrderBy(v => v.Player.PlayedGames).TakeLast(slotsToFree);
                playersToAdd.Game.Members.Remove(maxPlayedPlayers);
            }

            playersToAdd.Game.AddPlayers(playersToAdd.Players);
        }

        private void SetGameHost(SetGameHostObject setGameHostObject)
        {
            var firstHost = setGameHostObject.Players.FirstOrDefault(v => v.CanBeHost);
            if (firstHost != null)
            {
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
    }
}