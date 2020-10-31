using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class RoomsPageViewModel : BindableObject
    {
        private readonly SourceList<RoomPageViewModel> roomViewModels = new SourceList<RoomPageViewModel>();
        
        private readonly ObservableCollection<Player> players;
        private readonly PopupService popupService;
        
        private readonly ReadOnlyObservableCollection<RoomPageViewModel> sortedRoomViewModels;

        public RoomsPageViewModel(ObservableCollection<Player> players, PopupService popupService)
        {
            this.players = players;
            this.popupService = popupService;
            
            this.roomViewModels.Connect()
                .Sort(SortExpressionComparer<RoomPageViewModel>.Ascending(v => v.Room.RoomNumber))
                .ObserveOn(SynchronizationContext.Current)
                .Bind(out this.sortedRoomViewModels)
                .DisposeMany()
                .Subscribe();
            this.AddRoomExecute();
        }

        public ReadOnlyObservableCollection<RoomPageViewModel> SortedRoomViewModels => this.sortedRoomViewModels;

        public ICommand AddRoomCommand => new Command(() => this.AddRoomExecute());
        public ICommand RemoveRoomCommand => new Command<RoomPageViewModel>(this.RemoveRoomExecute);

        public void Clear()
        {
            var newRoomVm = this.AddRoomExecute(1);
            foreach (var roomViewModel in this.SortedRoomViewModels)
            {
                if (roomViewModel != newRoomVm)
                {
                    this.RemoveRoomInternal(roomViewModel);
                }
            }
        }

        private void AddRoomExecute()
        {
            if (this.sortedRoomViewModels.Count >= 6)
            {
                this.popupService.ShowAlert("Увы, нельзя добавить больше 6 столов, тк программа не сможет их нормально показать", "Нельзя добавить стол");
                return;
            }
            
            var newRoomNumber = Enumerable.Range(1, 100).First(v => !this.roomViewModels.Items.Select(r => r.Room.RoomNumber).Contains(v));
            this.AddRoomExecute(newRoomNumber);
        }

        private RoomPageViewModel AddRoomExecute(int roomNumber)
        {
            var newVm = new RoomPageViewModel(new Room(roomNumber), this.players, this.popupService);
            this.roomViewModels.Add(newVm);

            return newVm;
        }

        private async void RemoveRoomExecute(RoomPageViewModel roomViewModel)
        {
            if (this.sortedRoomViewModels.Count <= 1)
            {
                this.popupService.ShowAlert("Нельзя удалить единственный стол", "Нельзя удалить стол");
                return;
            }

            var confirmed = await this.popupService.ConfirmationPopup("Все игры стола пропадут. Удалить выбранный стол?", "Требуется подтверждение");
            if (confirmed.HasValue && confirmed.Value)
            {
                this.RemoveRoomInternal(roomViewModel);
            }
        }

        private void RemoveRoomInternal(RoomPageViewModel roomViewModel)
        {
            roomViewModel.ClearRoom();
            this.roomViewModels.Remove(roomViewModel);
        } 
    }
}