using System.Collections.ObjectModel;

namespace Mafia.SeatsGenerator.Models
{
    public class Room
    {
        public Room(int roomNumber)
        {
            this.RoomNumber = roomNumber;
        }

        public int RoomNumber { get; }
        
        public ObservableCollection<Game> Games { get; } = new ObservableCollection<Game>();
    }
}