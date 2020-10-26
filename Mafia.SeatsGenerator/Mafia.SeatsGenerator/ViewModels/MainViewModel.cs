using System.Collections.ObjectModel;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly ObservableCollection<Player> registeredPlayers = new ObservableCollection<Player>();
        private readonly ObservableCollection<Game> registeredGames = new ObservableCollection<Game>();

        public MainViewModel()
        {
            this.PlayersSetupPageViewModel = new PlayersSetupPageViewModel(this.registeredPlayers);
            this.StaticGamesPageViewModel = new GamesPageViewModel(this.registeredPlayers, this.registeredGames, new PopupService());
            this.EventsPageViewModel = new EventsPageViewModel(new EventsService(), this.registeredPlayers, this.registeredGames);
        }

        public PlayersSetupPageViewModel PlayersSetupPageViewModel { get; } 
        
        public GamesPageViewModel StaticGamesPageViewModel { get; } 
        
        public EventsPageViewModel EventsPageViewModel { get; }
    }
}