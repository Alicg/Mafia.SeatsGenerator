using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DynamicData;
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

            var dbEvent = new DbEvent();
            dbEvent.Name = newEvent.Name;
            dbEvent.Visitors = newEvent.Visitors.Select(v => new DbPlayerInEvent
            {
                Name = v.Name,
                CanBeHost = v.CanBeHost,
                IsVip = v.IsVip,
                Number = v.Number
            }).ToList();
            dbEvent.Rooms = newEvent.Rooms.Select(room => new DbRoom
            {
                RoomNumber = room.RoomNumber,
            }).ToList();
            connection.InsertWithChildren(dbEvent);

            foreach (var room in newEvent.Rooms)
            {
                foreach (var game in room.Games.Items)
                {
                    var dbGame = new DbGame
                    {
                        RoomId = dbEvent.Rooms.First(v => v.RoomNumber == room.RoomNumber).Id, 
                        HostId = dbEvent.Visitors.First(v => v.Name == game.HostName).Id,
                        FirstKilledId = dbEvent.Visitors.FirstOrDefault(v => v.Name == game.FirstKilled?.Player.Name)?.Id,
                        Number = game.Number
                    };
                    connection.Insert(dbGame);
                    foreach (var playerInGame in game.Members)
                    {
                        var dbPlayerInGame = new DbPlayerInGame
                        {
                            GameId = dbGame.Id, 
                            PlayerInEventId = dbEvent.Visitors.FirstOrDefault(v => v.Name == playerInGame.Player.Name)?.Id
                        };
                        connection.Insert(dbPlayerInGame);
                    }
                }
            }
        }

        public List<Event> SelectAllEvents()
        {
            var connection = this.CreateNewConnection();
            return connection.GetAllWithChildren<DbEvent>(recursive:true).Select(v =>
            {
                var newEvent = new Event
                {
                    Id = v.Id,
                    Name = v.Name,
                };
                newEvent.Visitors = new ObservableCollection<Player>(v.Visitors.Select(visitor => new Player
                {
                    Name = visitor.Name,
                    IsVip = visitor.IsVip,
                    CanBeHost = visitor.CanBeHost,
                    Number = visitor.Number
                }));
                
                var rooms = new ObservableCollection<Room>(v.Rooms.Select(dbRoom =>
                {
                    var games = dbRoom.Games.Select(dbGame =>
                    {
                        var newGame = new Game();
                        newGame.Host = new PlayerInGame(newEvent.Visitors.FirstOrDefault(visitor => visitor.Name == dbGame.Host.Name), newGame);
                        if (dbGame.FirstKilledId != null)
                        {
                            newGame.FirstKilled = new PlayerInGame(newEvent.Visitors.FirstOrDefault(visitor => visitor.Name == dbGame.FirstKilled.Name), newGame);
                        }
                        newGame.Number = dbGame.Number;
                        newGame.Members.AddRange(dbGame.Members.Select(dbMember =>
                        {
                            var visitorInEvent = newEvent.Visitors.FirstOrDefault(visitor => visitor.Name == dbMember.PlayerInEvent?.Name);
                            return new PlayerInGame(visitorInEvent ?? Player.EmptyPlayer, newGame);
                        }));
                        return newGame;
                    });
                    return new Room(dbRoom.RoomNumber, games);
                }));
                newEvent.Rooms = rooms;
                
                return newEvent;
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
            connection.CreateTable<DbPlayerInGame>();
            connection.CreateTable<DbRoom>();
            connection.CreateTable<DbGame>();
            return connection;
        }
    }
}