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
            this.AllocationPageViewModel = new AllocationPageViewModel(this.PlayersSetupPageViewModel, popupService);
            this.EventsPageViewModel = new EventsPageViewModel(new EventsService(), this.PlayersSetupPageViewModel, this.AllocationPageViewModel.RoomsPageViewModel);
        }

        public PlayersSetupPageViewModel PlayersSetupPageViewModel { get; } 
        
        public AllocationPageViewModel AllocationPageViewModel { get; } 
        
        public EventsPageViewModel EventsPageViewModel { get; }
    }
}