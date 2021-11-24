using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models.db
{
    [Table("Rooms")]
    public class DbRoom
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public int RoomNumber { get; set; }
        
        [OneToMany("RoomId", CascadeOperations = CascadeOperation.All)]
        public List<DbGame> Games { get; set; }
        
        [ManyToOne("RoomEventId", CascadeOperations = CascadeOperation.All)]
        public DbEvent Event { get; set; }
        
        [ForeignKey(typeof(DbEvent))]
        public int RoomEventId { get; set; }
    }
}