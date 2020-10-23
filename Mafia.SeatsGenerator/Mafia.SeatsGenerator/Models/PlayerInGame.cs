namespace Mafia.SeatsGenerator.Models
{
    public class PlayerInGame
    {
        public PlayerInGame(Player player, Game game)
        {
            this.Player = player;
            this.Game = game;
        }

        public Player Player { get; private set; }
        public Game Game { get; private set; }
    }
}