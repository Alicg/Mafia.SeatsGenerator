using Mafia.SeatsGenerator.Services;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.ViewModels
{
    public class AllocationPageViewModel : BindableObject
    {
        private BindableObject selectedPage;

        public AllocationPageViewModel(PlayersSetupPageViewModel playersSetupPageViewModel, PopupService popupService)
        {
            this.RoomsPageViewModel = new RoomsPageViewModel(playersSetupPageViewModel, popupService);
            this.ManualAllocationPageViewModel = new ManualAllocationPageViewModel(playersSetupPageViewModel, this.RoomsPageViewModel);
            this.ManualAllocationPageViewModel.NavigateToRoomPage += (sender, model) =>
            {
                this.SelectedPage = this.RoomsPageViewModel;
                this.RoomsPageViewModel.SelectedRoomPage = model;
            };
        }
        
        public string Title => "Рассадка";
        
        public ManualAllocationPageViewModel ManualAllocationPageViewModel { get; }
        
        public RoomsPageViewModel RoomsPageViewModel { get; }

        public BindableObject SelectedPage
        {
            get => this.selectedPage;
            set
            {
                if (this.selectedPage == value)
                {
                    return;
                }
                this.selectedPage = value;
                this.OnPropertyChanged(nameof(this.SelectedPage));
            }
        }
    }
}