using System.Collections.ObjectModel;
using SQLite;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models
{
    public class Game : BindableObject
    {
        private int number;
        private PlayerInGame firstKilled;

        public ObservableCollection<PlayerInGame> Members { get; } = new ObservableCollection<PlayerInGame>();
        
        public PlayerInGame Host { get; set; }

        public PlayerInGame FirstKilled
        {
            get => this.firstKilled;
            set
            {
                this.firstKilled = value;
                this.OnPropertyChanged(nameof(this.FirstKilled));
            }
        }

        public int Number
        {
            get => this.number;
            set
            {
                this.number = value;
                this.OnPropertyChanged(nameof(this.Number));
                this.OnPropertyChanged(nameof(this.GameColor));
            }
        }

        public Color GameColor => GameColors.Colors[this.number % GameColors.Colors.Count];
    }
}