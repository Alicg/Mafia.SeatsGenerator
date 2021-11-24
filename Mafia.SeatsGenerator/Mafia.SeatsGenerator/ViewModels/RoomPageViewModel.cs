using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using SQLite;
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
        public ICommand DeleteFromGameCommand => new Command<PlayerInGame>(this.DeletePlayerFromGame);
        public ICommand FirstKilledCommand => new Command<PlayerInGame>(this.SetFirstKilled);
        public ICommand StopGameCommand => new Command<Game>(this.StopGame);
        public ICommand ResumeGameCommand => new Command<Game>(this.StartGame);
        public ICommand ChangeHostCommand => new Command<Game>(this.ChangeHost);
        public ICommand ChangePlayerCommand => new Command<PlayerInGame>(this.ChangePlayer);

        public void ClearRoom(bool askConfirmation = true)
        {
            foreach (var game in this.Room.Games.Items.ToArray())
            {
                this.RemoveGameInternal(game, askConfirmation);
            }
        }

        public void CreateEmptyGame()
        {
            var newGame = new Game();
            this.AddGame(newGame);
            newGame.AddPlayers(Enumerable.Repeat(Player.EmptyPlayer, 10));
        }
        
        private void SetFirstKilled(PlayerInGame p)
        {
            p.Game.SetFirstKilled(p);
        }

        private void DeletePlayerFromGame(PlayerInGame p)
        {
            p.Game.ReplacePlayers(p, Player.EmptyPlayer);
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
            this.RemoveGameInternal(game);
        }

        private async void RemoveGameInternal(Game game, bool askConfirmation = true)
        {
            var confirmed = askConfirmation ? await this.popupService.ConfirmationPopup("Удалить игру?", "Удаление игры.") : true; 
            if (!confirmed.HasValue || !confirmed.Value) 
            {
                return;
            }
            
            this.Room.Games.Remove(game);

            if (game.Host != null)
            {
                game.Host.Player.HostedGames--;
            }
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
            var sortedPlayers = this.playersSetupPageViewModel.Players.Where(v => !v.IsBusy).Except(new[] {host}).OrderBy(v => v.IsVip).ThenBy(v => v.PlayedGames).Take(10).ToList();
            if (sortedPlayers.Count < 10)
            {
                sortedPlayers.AddRange(Enumerable.Repeat(Player.EmptyPlayer, 10 - sortedPlayers.Count));
            }
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
            var possiblePlayers = this.playersSetupPageViewModel.Players.Where(v => !v.IsBusy && v != playerInGame.Player).OrderBy(v => v.PlayedGames).ToList();
            var newPlayer = await this.popupService.SelectChoicePopup(possiblePlayers, "Выберите нового игрока");
            
            if (newPlayer != null)
            {
                playerInGame.Game.ReplacePlayers(playerInGame, newPlayer);
            }
        }
    }
}