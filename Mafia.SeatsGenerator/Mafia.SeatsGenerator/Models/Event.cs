using System.Collections.ObjectModel;
using SQLite;

namespace Mafia.SeatsGenerator.Models
{
    public class Event
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public ObservableCollection<Player> Visitors { get; set; }
        
        public ObservableCollection<Room> Rooms { get; set; }
    }
}