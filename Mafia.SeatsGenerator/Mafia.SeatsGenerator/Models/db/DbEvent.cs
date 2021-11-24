using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Mafia.SeatsGenerator.Models.db
{
    [Table("Events")]
    public class DbEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        [OneToMany("PlayerEventId", CascadeOperations = CascadeOperation.All)]
        public List<DbPlayerInEvent> Visitors { get; set; }
        
        [OneToMany("RoomEventId", CascadeOperations = CascadeOperation.All)]
        public List<DbRoom> Rooms { get; set; }
    }
}