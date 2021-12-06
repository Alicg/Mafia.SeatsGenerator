using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models
{
    public class Room : BindableObject
    {
        private readonly ReadOnlyObservableCollection<Game> bindingListOfGames;
        private Game currentGame;

        public Room(int roomNumber, IEnumerable<Game> games = null)
        {
            this.RoomNumber = roomNumber;
            
            this.Games = new SourceList<Game>();
            this.Games.Connect().Bind(out this.bindingListOfGames).Subscribe();
            this.Games.AddRange(games ?? new Game[0]);
        }
        
        public int Id { get; set; }

        public int RoomNumber { get; }
        
        public SourceList<Game> Games { get; }

        public ReadOnlyObservableCollection<Game> BindingListOfGames => this.bindingListOfGames;

        public Game CurrentGame
        {
            get => this.currentGame;
            set
            {
                this.currentGame = value;
                this.OnPropertyChanged(nameof(this.CurrentGame));
            }
        }

        public Room Clone(IList<Player> playersToUse)
        {
            return new Room(this.RoomNumber, this.BindingListOfGames.Select(v => v.Clone(playersToUse)));
        }
    }
}