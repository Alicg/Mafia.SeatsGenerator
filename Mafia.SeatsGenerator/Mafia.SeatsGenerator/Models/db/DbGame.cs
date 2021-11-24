using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models.db
{
    [Table("Games")]
    public class DbGame
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public int Number { get; set; }
        
        [ManyToOne("HostId", CascadeOperations = CascadeOperation.All)]
        public DbPlayerInEvent Host { get; set; }
        
        [ForeignKey(typeof(DbPlayerInEvent))]
        public int HostId { get; set; }
        
        [OneToMany("GameId", CascadeOperations = CascadeOperation.All)]
        public List<DbPlayerInGame> Members { get; set; }
        
        [ManyToOne("RoomId", CascadeOperations = CascadeOperation.All)]
        public DbRoom Room { get; set; }
        
        [ForeignKey(typeof(DbRoom))]
        public int RoomId { get; set; }
    }
}