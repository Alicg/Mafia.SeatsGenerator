using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class MainViewModel : BindableObject
    {
        public MainViewModel()
        {
            var popupService = new PopupService();
            this.PlayersSetupPageViewModel = new PlayersSetupPageViewModel(popupService);
            this.RoomsPageViewModel = new RoomsPageViewModel(this.PlayersSetupPageViewModel, popupService);
            this.EventsPageViewModel = new EventsPageViewModel(new EventsService(), this.PlayersSetupPageViewModel, this.RoomsPageViewModel);
        }

        public PlayersSetupPageViewModel PlayersSetupPageViewModel { get; } 
        
        public RoomsPageViewModel RoomsPageViewModel { get; } 
        
        public EventsPageViewModel EventsPageViewModel { get; }
    }
}