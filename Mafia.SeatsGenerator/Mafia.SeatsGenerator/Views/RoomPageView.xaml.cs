using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Utils;
using Mafia.SeatsGenerator.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomPageView : ContentPage
    {
        private Subject<PlayerInGame> tapListener = new Subject<PlayerInGame>();
        
        public RoomPageView()
        {
            this.InitializeComponent();

            this.tapListener.Publish(ps => ps.Buffer(() => ps.Throttle(TimeSpan.FromSeconds(0.2)))).Where(v => v.Count == 2).ObserveOn(RxApp.MainThreadScheduler).Subscribe(v =>
            {
                var vm = this.BindingContext as RoomPageViewModel;
                vm?.DeleteFromGameCommand.Execute(v.First());
            });
            this.tapListener.Publish(ps => ps.Buffer(() => ps.Throttle(TimeSpan.FromSeconds(0.2)))).Where(v => v.Count == 1).ObserveOn(RxApp.MainThreadScheduler).Subscribe(v =>
            {
                var vm = this.BindingContext as RoomPageViewModel;
                vm?.FirstKilledCommand.Execute(v.First());
            });
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
            this.tapListener.OnNext(e.BindingContext as PlayerInGame);
        }
    }
}