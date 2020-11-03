using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class RoomPageViewModel : BindableObject
    {
        private Random randomGenerator = new Random();
        private readonly ObservableCollection<Player> players;
        private readonly PopupService popupService;

        public RoomPageViewModel() { }

        public RoomPageViewModel(Room room, ObservableCollection<Player> players, PopupService popupService)
        {
            this.players = players;
            this.popupService = popupService;
            this.Room = room;
        }
        
        public Room Room { get; }

        public ICommand GenerateNewGameCommand => new Command(this.GenerateNewGame);
        public ICommand RemoveGameCommand => new Command<Game>(this.RemoveGame);
        public ICommand FirstKilledCommand => new Command<PlayerInGame>(this.SetFirstKilled);
        public ICommand StopGameCommand => new Command<Game>(this.StopGame);
        public ICommand ResumeGameCommand => new Command<Game>(this.ResumeGame);

        public void ClearRoom()
        {
            foreach (var game in this.Room.Games.Items.ToArray())
            {
                this.RemoveGame(game);
            }
        }
        
        private void SetFirstKilled(PlayerInGame p)
        {
            if (p.Game.FirstKilled != null)
            {
                p.Game.FirstKilled.Player.PriorityPoints += 0.5;
            }

            if (p.Game.FirstKilled == p)
            {
                p.Game.FirstKilled = null;
                return;
            }
            p.Game.FirstKilled = p;
            p.Game.FirstKilled.Player.PriorityPoints -= 0.5;
        }

        private void GenerateNewGame()
        {
            var activeGame = this.Room.Games.Items.FirstOrDefault(v => !v.IsStopped);
            if (activeGame != null)
            {
                this.StopGame(activeGame);
            }
            
            try
            {
                var newGame = new Game();
                var hostPlayer = this.GetNewHost();
                newGame.Host = new PlayerInGame(hostPlayer, newGame);
                hostPlayer.IsBusy = true;
            
                foreach (var player in this.GetNewSetOfPlayers(newGame.Host.Player))
                {
                    newGame.Members.Add(new PlayerInGame(player, newGame));
                    player.IsBusy = true;
                }
                this.Room.Games.Add(newGame);
            
                foreach (var player in newGame.Members)
                {
                    player.Player.PriorityPoints++;
                }
            
                this.RecalculateNumbers();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Handling here.
            }
        }

        private void RemoveGame(Game game)
        {
            this.Room.Games.Remove(game);
            
            foreach (var player in game.Members)
            {
                if (game.FirstKilled == player)
                {
                    player.Player.PriorityPoints -= 0.5;
                }
                else
                {
                    player.Player.PriorityPoints -= 1;
                }
            }

            if (!game.IsStopped)
            {
                this.StopGame(game);
            }
            
            this.RecalculateNumbers();
        }

        private void RecalculateNumbers()
        {
            for (var index = 0; index < this.Room.Games.Count; index++)
            {
                var game = this.Room.Games.Items.ToList()[index];
                game.Number = index + 1;
            }
        }

        private IEnumerable<Player> GetNewSetOfPlayers(Player host)
        {
            var sortedPlayers = this.players.Where(v => !v.IsBusy).Except(new[] {host}).OrderBy(v => v.IsVip).ThenBy(v => v.PriorityPoints).Take(10);
            return sortedPlayers.OrderBy(v => this.randomGenerator.Next(0, 9));
        }

        private Player GetNewHost()
        {
            var sortedPossibleHosts = this.players.Where(v => v.CanBeHost && !v.IsBusy).OrderBy(v => v.PriorityPoints).ToList();
            if (sortedPossibleHosts.Count == 0)
            {
                this.popupService.ShowAlert(@"Установите признак ""Ведущий"" хотя бы для одного игрока во вкладке ""Игроки""", "Нет ведущих.");
                throw new ApplicationException("Нет ведущих.");
            }

            var maxPoints = sortedPossibleHosts.Max(h => h.PriorityPoints);
            var hostsWithMaxPoints = sortedPossibleHosts.Where(v => Math.Abs(v.PriorityPoints - maxPoints) < 0.01).ToList();
            return hostsWithMaxPoints[this.randomGenerator.Next(0, hostsWithMaxPoints.Count - 1)];
        }

        private void StopGame(Game game)
        {
            game.IsStopped = true;
            game.Host.Player.IsBusy = false;
            foreach (var gameMember in game.Members)
            {
                gameMember.Player.IsBusy = false;
            }
        }

        private void ResumeGame(Game game)
        {
            foreach (var roomGame in this.Room.Games.Items)
            {
                if (roomGame != game)
                {
                    this.StopGame(roomGame);
                }
            }
            
            game.IsStopped = false;
            game.Host.Player.IsBusy = true;
            foreach (var gameMember in game.Members)
            {
                gameMember.Player.IsBusy = true;
            }
        }
    }
}