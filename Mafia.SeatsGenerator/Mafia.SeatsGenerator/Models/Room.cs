using System;
using System.Collections.ObjectModel;
using DynamicData;

namespace Mafia.SeatsGenerator.Models
{
    public class Room
    {
        private readonly ReadOnlyObservableCollection<Game> bindingListOfGames;

        public Room(int roomNumber)
        {
            this.RoomNumber = roomNumber;
            
            this.Games = new SourceList<Game>();
            this.Games.Connect().Bind(out this.bindingListOfGames).Subscribe();
        }

        public int RoomNumber { get; }
        
        public SourceList<Game> Games { get; }

        public ReadOnlyObservableCollection<Game> BindingListOfGames => this.bindingListOfGames;
    }
}