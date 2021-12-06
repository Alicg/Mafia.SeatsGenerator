﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class EventsPageViewModel : BindableObject
    {
        private readonly EventsService eventsService;
        private readonly PlayersSetupPageViewModel playersSetupPageViewModel;
        private readonly RoomsPageViewModel roomsPageViewModel;

        public EventsPageViewModel(EventsService eventsService, PlayersSetupPageViewModel playersSetupPageViewModel, RoomsPageViewModel roomsPageViewModel)
        {
            this.eventsService = eventsService;
            this.playersSetupPageViewModel = playersSetupPageViewModel;
            this.roomsPageViewModel = roomsPageViewModel;

            var archiveEvents = this.eventsService.SelectAllEvents();
            foreach (var archiveEvent in archiveEvents)
            {
                foreach (var room in archiveEvent.Rooms)
                {
                    foreach (var game in room.BindingListOfGames)
                    {
                        foreach (var member in game.Members)
                        {
                            member.Player.PlayedGames++;
                        }

                        game.Host.Player.HostedGames++;
                    }
                }
                this.EventsArchive.Add(archiveEvent);
            }
        }

        public string Title => "Встречи";
        
        public ObservableCollection<Event> EventsArchive { get; } = new ObservableCollection<Event>();
        
        public ICommand SaveNewEventCommand => new Command<string>(this.SaveNewEvent);
        public ICommand RemoveEventCommand => new Command<Event>(this.RemoveEvent);
        public ICommand LoadEventCommand => new Command<Event>(this.LoadEvent);

        private void SaveNewEvent(string name)
        {
            var newEvent = new Event
            {
                Name = name,
                Visitors = new ObservableCollection<Player>(this.playersSetupPageViewModel.Players),
                Rooms = new ObservableCollection<Room>(this.roomsPageViewModel.SortedRoomViewModels.Select(v => v.Room))
            };
            this.eventsService.AddNewEvent(newEvent);
            this.EventsArchive.Add(newEvent);
        }

        private void RemoveEvent(Event eventToRemove)
        {
            this.eventsService.RemoveEvent(eventToRemove);
            this.EventsArchive.Remove(eventToRemove);
        }

        private void LoadEvent(Event eventToLoad)
        {
            var clonedEvent = eventToLoad.Clone();
            this.roomsPageViewModel.Clear();
            this.playersSetupPageViewModel.ClearPlayers();
            
            this.playersSetupPageViewModel.AddRangePlayers(clonedEvent.Visitors.ToArray());
            this.roomsPageViewModel.LoadRooms(clonedEvent.Rooms);
        }
    }
}