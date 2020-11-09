using System;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Utils;
using Mafia.SeatsGenerator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomPageView : ContentPage
    {
        public RoomPageView()
        {
            InitializeComponent();
        }

        private void MultiGestureView_Host_OnLongPressed(object sender, BindingEventArgs e)
        {
            var vm = this.BindingContext as RoomPageViewModel;
            vm?.ChangeHostCommand.Execute(e.BindingContext as Game);
        }

        private void MultiGestureView_Player_OnLongPressed(object sender, BindingEventArgs e)
        {
            var vm = this.BindingContext as RoomPageViewModel;
            vm?.ChangePlayerCommand.Execute(e.BindingContext as PlayerInGame);
        }

        private void MultiGestureView_OnTapped(object sender, BindingEventArgs e)
        {
            var vm = this.BindingContext as RoomPageViewModel;
            vm?.FirstKilledCommand.Execute(e.BindingContext as PlayerInGame);
        }
    }
}