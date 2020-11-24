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
            if (p.Game.FirstKilled != null)
            {
                p.Game.FirstKilled.Player.PlayedGames += 0.5;
            }

            if (p.Game.FirstKilled == p)
            {
                p.Game.FirstKilled = null;
                return;
            }
            p.Game.FirstKilled = p;
            p.Game.FirstKilled.Player.PlayedGames -= 0.5;
        }
    }
}