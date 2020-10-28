using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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
        
        private void SetFirstKilled(PlayerInGame p)
        {
            if (p.Game.FirstKilled != null)
            {
                p.Game.FirstKilled.Player.PriorityPoints += 0.5;
            }
            p.Game.FirstKilled = p;
            p.Game.FirstKilled.Player.PriorityPoints -= 0.5;
        }

        private void GenerateNewGame()
        {
            try
            {
                var newGame = new Game();
                newGame.Host = new PlayerInGame(this.GetNewHost(), newGame);
            
                foreach (var player in this.GetNewSetOfPlayers(newGame.Host.Player))
                {
                    newGame.Members.Add(new PlayerInGame(player, newGame));
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
            
            this.RecalculateNumbers();
        }

        private void RecalculateNumbers()
        {
            for (var index = 0; index < this.Room.Games.Count; index++)
            {
                var game = this.Room.Games[index];
                game.Number = index + 1;
            }
        }

        private IEnumerable<Player> GetNewSetOfPlayers(Player host)
        {
            var sortedPlayers = this.players.Except(new[] {host}).OrderBy(v => v.IsVip).ThenBy(v => v.PriorityPoints).Take(10);
            return sortedPlayers.OrderBy(v => this.randomGenerator.Next(0, 9));
        }

        private Player GetNewHost()
        {
            var sortedPossibleHosts = this.players.Where(v => v.CanBeHost).OrderBy(v => v.PriorityPoints).ToList();
            if (sortedPossibleHosts.Count == 0)
            {
                this.popupService.ShowAlert(@"Установите признак ""Ведущий"" хотя бы для одного игрока во вкладке ""Игроки""", "Нет ведущих.");
                throw new ApplicationException("Нет ведущих.");
            }

            var maxPoints = sortedPossibleHosts.Max(h => h.PriorityPoints);
            var hostsWithMaxPoints = sortedPossibleHosts.Where(v => Math.Abs(v.PriorityPoints - maxPoints) < 0.01).ToList();
            return hostsWithMaxPoints[this.randomGenerator.Next(0, hostsWithMaxPoints.Count - 1)];
        }
    }
}