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
        
        [OneToMany("EventId", CascadeOperations = CascadeOperation.All)]
        public List<DbPlayerInEvent> Visitors { get; set; }
    }
}