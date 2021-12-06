using System.Collections.ObjectModel;
using System.Linq;
using SQLite;

namespace Mafia.SeatsGenerator.Models
{
    public class Event
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public ObservableCollection<Player> Visitors { get; set; }
        
        public ObservableCollection<Room> Rooms { get; set; }

        public Event Clone()
        {
            var newEvent = new Event
            {
                Name = this.Name
            };

            var clonedPlayers = this.Visitors.Select(v => v.Clone());
            var rooms = this.Rooms.Select(v => v.Clone(clonedPlayers.ToList()));

            newEvent.Rooms = new ObservableCollection<Room>(rooms);
            newEvent.Visitors = new ObservableCollection<Player>(clonedPlayers);

            return newEvent;
        }
    }
}