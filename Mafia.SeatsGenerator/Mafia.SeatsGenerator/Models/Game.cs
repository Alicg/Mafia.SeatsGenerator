using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
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

        public int SlotsOccupied => this.Members.Count(v => v.Player != Player.EmptyPlayer);

        public void AddPlayers(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                this.AddPlayer(player);
            }
            this.OnPropertyChanged(nameof(this.SlotsOccupied));
        }

        public void ReplacePlayers(PlayerInGame from, Player to)
        {
            if (this.FirstKilled == from)
            {
                // снимаем признак ПУ если он стоит на игроке.
                this.SetFirstKilled(from);
            }

            try
            {
                this.Members.Replace(from, new PlayerInGame(to, this));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            from.Player.PlayedGames -= 1;
            from.Player.IsBusy = false;
            
            to.PlayedGames += 1;
            to.IsBusy = true;
            
            this.OnPropertyChanged(nameof(this.SlotsOccupied));
        }

        private void AddPlayer(Player player)
        {
            this.Members.Add(new PlayerInGame(player, this));
            player.PlayedGames++;
            if (!this.isStopped)
            {
                player.IsBusy = true;
            }
        }

        public void SetHost(Player newHost)
        {
            if (this.Host != null)
            {
                this.Host.Player.IsBusy = false;
                this.Host.Player.HostedGames--;
            }

            this.Host = new PlayerInGame(newHost, this);
            newHost.IsBusy = true;
            newHost.HostedGames++;
        }

        public void SetFirstKilled(PlayerInGame p)
        {
            if (this.FirstKilled != null)
            {
                this.FirstKilled.Player.PlayedGames += 0.5;
            }

            if (this.FirstKilled == p)
            {
                this.FirstKilled = null;
                return;
            }
            this.FirstKilled = p;
            this.FirstKilled.Player.PlayedGames -= 0.5;
        }

        public Game Clone(IList<Player> playersToUse)
        {
            var clonedGame = new Game
            {
                Number = this.Number,
                IsStopped = this.IsStopped
            };
            clonedGame.Host = new PlayerInGame(playersToUse.First(v => v.Name == this.Host.Player.Name), clonedGame);
            clonedGame.Members.Clear();
            clonedGame.Members.AddRange(playersToUse.Where(v => this.Members.Any(p => p.Player.Name == v.Name)).Select(v => new PlayerInGame(v, clonedGame)));
            
            if (this.FirstKilled != null)
            {
                clonedGame.FirstKilled = clonedGame.Members.First(v => v.Player.Name == this.FirstKilled.Player.Name);
            }

            return clonedGame;
        }
    }
}