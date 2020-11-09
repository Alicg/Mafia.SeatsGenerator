using System.Collections.ObjectModel;
using SQLite;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models
{
    public class Game : BindableObject
    {
        private int number;
        private PlayerInGame firstKilled;
        private bool isStopped;
        private PlayerInGame host;

        public ObservableCollection<PlayerInGame> Members { get; } = new ObservableCollection<PlayerInGame>();

        public string HostName => this.Host?.Player?.Name;
        
        public PlayerInGame Host
        {
            get => this.host;
            set
            {
                this.host = value;
                this.OnPropertyChanged(nameof(this.Host));
                this.OnPropertyChanged(nameof(this.HostName));
            }
        }

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

        public bool IsStopped
        {
            get => this.isStopped;
            set
            {
                this.isStopped = value;
                this.OnPropertyChanged(nameof(this.IsStopped));
            }
        }
    }
}