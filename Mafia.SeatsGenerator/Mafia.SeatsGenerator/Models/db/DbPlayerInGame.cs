using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models.db
{
    [Table("PlayersInGame")]
    public class DbPlayerInGame
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [ManyToOne("GameId", CascadeOperations = CascadeOperation.All)]
        public DbGame Game { get; set; }
        
        [ForeignKey(typeof(DbGame))]
        public int GameId { get; set; }
        
        [ManyToOne("PlayerInEventId", CascadeOperations = CascadeOperation.All)]
        public DbPlayerInEvent PlayerInEvent { get; set; }
        
        [ForeignKey(typeof(DbPlayerInEvent))]
        public int? PlayerInEventId { get; set; }
    }
}