using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Models
{
    public class Player : BindableObject
    {
        private string _name;
        private int _number;
        private double priorityPoints;
        private bool isBusy;
        private bool canBeHost;

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

        public double PriorityPoints
        {
            get => this.priorityPoints;
            set
            {
                this.priorityPoints = value;
                this.OnPropertyChanged(nameof(this.PriorityPoints));
            }
        }
    }
}