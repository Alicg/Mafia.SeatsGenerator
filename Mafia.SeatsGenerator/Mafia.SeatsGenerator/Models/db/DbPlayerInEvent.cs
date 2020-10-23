using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models.db
{
    [Table("PlayersInEvent")]
    public class DbPlayerInEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Name { get; set; }

        public bool CanBeHost { get; set; }
        
        public bool IsVip { get; set; }
        
        /// <summary>
        /// Каким по счету пришел на встречу.
        /// </summary>
        public int Number { get; set; }
        
        [ManyToOne("EventId", CascadeOperations = CascadeOperation.All)]
        public DbEvent Event { get; set; }
        
        [ForeignKey(typeof(DbEvent))]
        public int EventId { get; set; }
    }
}