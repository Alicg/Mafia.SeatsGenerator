using System;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models
{
    public class Player : BindableObject
    {
        private string _name;
        private int _number;
        private double playedGames;
        private bool isBusy;
        private bool canBeHost;
        private double hostedGames;

        public string Name
        {
            get => this._name;
            set
            {
                this._name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        public bool CanBeHost
        {
            get => this.canBeHost;
            set
            {
                this.canBeHost = value;
                this.OnPropertyChanged(nameof(this.CanBeHost));
            }
        }

        public bool IsVip { get; set; }
        
        public int Number
        {
            get => this._number;
            set
            {
                this._number = value;
                this.OnPropertyChanged(nameof(this.Number));
            }
        }

        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                this.OnPropertyChanged(nameof(this.IsBusy));
            }
        }

        public double PlayedGames
        {
            get => this.playedGames;
            set
            {
                this.playedGames = value;
                this.OnPropertyChanged(nameof(this.PlayedGames));
            }
        }

        public double HostedGames
        {
            get => this.hostedGames;
            set
            {
                this.hostedGames = value;
                this.OnPropertyChanged(nameof(this.HostedGames));
            }
        }
        
        public int SortingValue { get; set; }

        public void DecreasePriority(bool firstKilled)
        {
            if (firstKilled)
            {
                this.PlayedGames -= 0.5;
            }
            else
            {
                this.PlayedGames -= 1;
            }
        }

        public static Player EmptyPlayer = new Player {Name = "Пусто", PlayedGames = 100000};

        public override string ToString()
        {
            return $"{this.Name}; Игр:{this.PlayedGames}";
        }
    }
}