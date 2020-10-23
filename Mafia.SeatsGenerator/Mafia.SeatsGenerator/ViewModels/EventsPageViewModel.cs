using System.Collections.ObjectModel;
using System.Windows.Input;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class EventsPageViewModel : BindableObject
    {
        private readonly EventsService eventsService;
        private readonly ObservableCollection<Player> players;
        private readonly ObservableCollection<Game> games;

        public EventsPageViewModel(EventsService eventsService, ObservableCollection<Player> players, ObservableCollection<Game> games)
        {
            this.eventsService = eventsService;
            this.players = players;
            this.games = games;

            var archiveEvents = this.eventsService.SelectAllEvents();
            foreach (var archiveEvent in archiveEvents)
            {
                this.EventsArchive.Add(archiveEvent);
            }
        }
        
        public ObservableCollection<Event> EventsArchive { get; } = new ObservableCollection<Event>();
        
        public ICommand SaveNewEventCommand => new Command<string>(this.SaveNewEvent);
        public ICommand RemoveEventCommand => new Command<Event>(this.RemoveEvent);
        public ICommand LoadEventCommand => new Command<Event>(this.LoadEvent);

        private void SaveNewEvent(string name)
        {
            var newEvent = new Event
            {
                Name = name,
                Visitors = new ObservableCollection<Player>(this.players)
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
            this.games.Clear();
            this.players.Clear();
            foreach (var visitor in eventToLoad.Visitors)
            {
                this.players.Add(visitor);
            }
        }
    }
}