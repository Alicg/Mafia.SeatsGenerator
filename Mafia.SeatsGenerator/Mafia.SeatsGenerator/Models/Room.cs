using System;
using System.Collections.ObjectModel;
using DynamicData;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models
{
    public class Room : BindableObject
    {
        private readonly ReadOnlyObservableCollection<Game> bindingListOfGames;
        private Game currentGame;

        public Room(int roomNumber)
        {
            this.RoomNumber = roomNumber;
            
            this.Games = new SourceList<Game>();
            this.Games.Connect().Bind(out this.bindingListOfGames).Subscribe();
        }

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
    }
}