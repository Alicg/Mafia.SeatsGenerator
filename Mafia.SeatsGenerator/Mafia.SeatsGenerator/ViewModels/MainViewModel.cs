using System.Collections.ObjectModel;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly ObservableCollection<Player> registeredPlayers = new ObservableCollection<Player>();

        public MainViewModel()
        {
            this.PlayersSetupPageViewModel = new PlayersSetupPageViewModel(this.registeredPlayers);
            this.RoomsPageViewModel = new RoomsPageViewModel(this.registeredPlayers, new PopupService());
            this.EventsPageViewModel = new EventsPageViewModel(new EventsService(), this.PlayersSetupPageViewModel, this.RoomsPageViewModel);
        }

        public PlayersSetupPageViewModel PlayersSetupPageViewModel { get; } 
        
        public RoomsPageViewModel RoomsPageViewModel { get; } 
        
        public EventsPageViewModel EventsPageViewModel { get; }
    }
}