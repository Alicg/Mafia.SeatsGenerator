using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Models.db;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace Mafia.SeatsGenerator.Services
{
    public class EventsService
    {
        public void AddNewEvent(Event newEvent)
        {
            var connection = this.CreateNewConnection();

            var dbEvent = new DbEvent
            {
                Name = newEvent.Name,
                Visitors = newEvent.Visitors.Select(v => new DbPlayerInEvent
                {
                    Name = v.Name,
                    CanBeHost = v.CanBeHost,
                    IsVip = v.IsVip,
                    Number = v.Number
                }).ToList()
            };

            connection.InsertWithChildren(dbEvent);
        }

        public List<Event> SelectAllEvents()
        {
            var connection = this.CreateNewConnection();
            return connection.GetAllWithChildren<DbEvent>().Select(v => new Event
            {
                Id = v.Id,
                Name = v.Name,
                Visitors = new ObservableCollection<Player>(v.Visitors.Select(visitor => new Player
                {
                    Name = visitor.Name,
                    IsVip = visitor.IsVip,
                    CanBeHost = visitor.CanBeHost,
                    Number = visitor.Number
                }))
            }).ToList();
        }

        public void RemoveEvent(Event eventToRemove)
        {
            var connection = this.CreateNewConnection();
            connection.Delete<DbEvent>(eventToRemove.Id);
        }

        private SQLiteConnection CreateNewConnection()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "seats_generator.db3");
            var connection = new SQLiteConnection(dbPath);
            connection.CreateTable<DbEvent>();
            connection.CreateTable<DbPlayerInEvent>();
            return connection;
        }
    }
}