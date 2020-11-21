using System;
using System.Collections.Generic;
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
        private readonly Random randomGenerator = new Random();
        private readonly PlayersSetupPageViewModel playersSetupPageViewModel;
        private readonly PopupService popupService;

        public RoomPageViewModel() { }

        public RoomPageViewModel(Room room, PlayersSetupPageViewModel playersSetupPageViewModel, PopupService popupService)
        {
            this.playersSetupPageViewModel = playersSetupPageViewModel;
            this.popupService = popupService;
            this.Room = room;
        }
        
        public Room Room { get; }

        public ICommand GenerateNewGameCommand => new Command(this.GenerateNewGame);
        public ICommand RemoveGameCommand => new Command<Game>(this.RemoveGame);
        public ICommand FirstKilledCommand => new Command<PlayerInGame>(this.SetFirstKilled);
        public ICommand StopGameCommand => new Command<Game>(this.StopGame);
        public ICommand ResumeGameCommand => new Command<Game>(this.StartGame);
        public ICommand ChangeHostCommand => new Command<Game>(this.ChangeHost);
        public ICommand ChangePlayerCommand => new Command<PlayerInGame>(this.ChangePlayer);

        public void ClearRoom()
        {
            foreach (var game in this.Room.Games.Items.ToArray())
            {
                this.RemoveGame(game);
            }
        }

        public void CreateEmptyGame()
        {
            var newGame = new Game();
            this.AddGame(newGame);
        }
        
        private void SetFirstKilled(PlayerInGame p)
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
                hostPlayer.HostedGames++;
            
                newGame.AddPlayers(this.GetNewSetOfPlayers(newGame.Host.Player));
            
                this.AddGame(newGame);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Handling here.
            }
        }

        private void AddGame(Game newGame)
        {
            this.Room.Games.Add(newGame);
            this.RecalculateNumbers();
            
            this.StartGame(newGame);
        }

        private async void RemoveGame(Game game)
        {
            var confirmed = await this.popupService.ConfirmationPopup("Удалить игру?", "Удаление игры."); 
            if (!confirmed.HasValue || !confirmed.Value) 
            {
                return;
            }
            
            this.Room.Games.Remove(game);
            
            foreach (var player in game.Members)
            {
                player.Player.DecreasePriority(game.FirstKilled == player);
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
            var sortedPlayers = this.playersSetupPageViewModel.Players.Where(v => !v.IsBusy).Except(new[] {host}).OrderBy(v => v.IsVip).ThenBy(v => v.PlayedGames).Take(10);
            return sortedPlayers.OrderBy(v => this.randomGenerator.Next(0, 9));
        }

        private Player GetNewHost()
        {
            var sortedPossibleHosts = this.playersSetupPageViewModel.Players.Where(v => v.CanBeHost && !v.IsBusy).OrderBy(v => v.PlayedGames).ToList();
            if (sortedPossibleHosts.Count == 0)
            {
                this.popupService.ShowAlert(@"Установите признак ""Ведущий"" хотя бы для одного игрока во вкладке ""Игроки""", "Нет ведущих.");
                throw new ApplicationException("Нет ведущих.");
            }

            var maxPoints = sortedPossibleHosts.Max(h => h.PlayedGames);
            var hostsWithMaxPoints = sortedPossibleHosts.Where(v => Math.Abs(v.PlayedGames - maxPoints) < 0.01).ToList();
            return hostsWithMaxPoints[this.randomGenerator.Next(0, hostsWithMaxPoints.Count - 1)];
        }

        private void StopGame(Game game)
        {
            this.Room.CurrentGame = null;
            game.IsStopped = true;
            if (game.Host != null)
            {
                game.Host.Player.IsBusy = false;
            }
            foreach (var gameMember in game.Members)
            {
                gameMember.Player.IsBusy = false;
            }
        }

        private void StartGame(Game game)
        {
            foreach (var roomGame in this.Room.Games.Items)
            {
                if (roomGame != game)
                {
                    this.StopGame(roomGame);
                }
            }

            game.IsStopped = false;
            if (game.Host != null)
            {
                game.Host.Player.IsBusy = true;
            }
            foreach (var gameMember in game.Members)
            {
                gameMember.Player.IsBusy = true;
            }
            this.Room.CurrentGame = game;
        }

        private async void ChangeHost(Game game)
        {
            var possibleHosts = this.playersSetupPageViewModel.Players.Where(v => v.CanBeHost && !v.IsBusy && v != game.Host.Player).OrderByDescending(v => v.PlayedGames).ToList();

            var newHost = await this.popupService.SelectChoicePopup(possibleHosts, "Выберите нового ведущего");
            if (newHost != null)
            {
                game.SetHost(newHost);
            }
        }

        private async void ChangePlayer(PlayerInGame playerInGame)
        {
            if (playerInGame.Game.FirstKilled == playerInGame)
            {
                // снимаем признак ПУ если он стоит на игроке.
                this.SetFirstKilled(playerInGame);
            }
            
            var possiblePlayers = this.playersSetupPageViewModel.Players.Where(v => !v.IsBusy && v != playerInGame.Player).OrderBy(v => v.PlayedGames).ToList();

            var newPlayer = await this.popupService.SelectChoicePopup(possiblePlayers, "Выберите нового игрока");
            if (newPlayer != null)
            {
                var currentSeat = playerInGame.Game.Members.IndexOf(playerInGame);
                if (currentSeat != -1)
                {
                    playerInGame.Game.Members.RemoveAt(currentSeat);
                    playerInGame.Game.Members.Insert(currentSeat, new PlayerInGame(newPlayer, playerInGame.Game));

                    playerInGame.Player.PlayedGames -= 1;
                    playerInGame.Player.IsBusy = false;
                    
                    newPlayer.PlayedGames += 1;
                    newPlayer.IsBusy = true;
                }
            }
        }
    }
}