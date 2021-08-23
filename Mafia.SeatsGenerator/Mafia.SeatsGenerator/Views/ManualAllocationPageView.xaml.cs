using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManualAllocationPageView : ContentPage
    {
        private List<Grid> playerGridsSelected = new List<Grid>();
        
        public ManualAllocationPageView()
        {
            InitializeComponent();
        }

        private void ToGame_Button_OnClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var grid = button.Parent as Grid;
                if (this.playerGridsSelected.Contains(grid))
                {
                    this.UnselectPlayerGrid(grid);
                }
                else
                {
                    this.SelectPlayerGrid(grid);
                }
            }
        }

        private void SelectPlayerGrid(Grid grid)
        {
            this.playerGridsSelected.Add(grid);
            grid?.Children.OfType<Label>().ForEach(v =>
            {
                v.FontAttributes = FontAttributes.Bold;
                v.TextColor = Color.Blue;
            });
        }

        private void UnselectPlayerGrid(Grid grid)
        {
            this.playerGridsSelected.Remove(grid);
            grid?.Children.OfType<Label>().ForEach(v =>
            {
                v.FontAttributes = FontAttributes.None;
                v.TextColor = Color.Black;
            });
        }

        private void ToBottom_Button_OnClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var player = button.BindingContext as Player;
                if (this.BindingContext is ManualAllocationPageViewModel vm)
                {
                    vm.MoveToBottomCommand.Execute(player);
                }

                this.SortByGamesBtn.BorderColor = this.SortByHostBtn.BorderColor = Color.BlueViolet;
            }
        }

        private void MoveToGame_Button_OnClicked(object sender, EventArgs e)
        {
            if (sender is MaterialButton button && button.BindingContext is RoomPageViewModel selectedRoomVm)
            {
                var playersToAdd = this.playerGridsSelected.Select(v => v.BindingContext as Player);
                if (this.playerGridsSelected.Any() && this.BindingContext is ManualAllocationPageViewModel vm)
                {
                    vm.AddPlayersToGameCommand.Execute(new ManualAllocationPageViewModel.AddPlayersToGameObject
                    {
                        Players = playersToAdd,
                        Game = selectedRoomVm.Room.CurrentGame
                    });
                }
            }

            foreach (var grid in this.playerGridsSelected.ToArray())
            {
                this.UnselectPlayerGrid(grid);
            }
        }

        private void SetGameHost_Button_OnClicked(object sender, EventArgs e)
        {
            if (sender is MaterialButton button && button.BindingContext is RoomPageViewModel selectedRoomVm)
            {
                var playersToAdd = this.playerGridsSelected.Select(v => v.BindingContext as Player);
                if (this.playerGridsSelected.Any() && this.BindingContext is ManualAllocationPageViewModel vm)
                {
                    vm.SetGameHostCommand.Execute(new ManualAllocationPageViewModel.SetGameHostObject
                    {
                        Players = playersToAdd,
                        Game = selectedRoomVm.Room.CurrentGame
                    });
                }
            }

            foreach (var grid in this.playerGridsSelected.ToArray())
            {
                this.UnselectPlayerGrid(grid);
            }
        }

        private void SortButton_OnClicked(object sender, EventArgs e)
        {
            this.RevertSortButtonColor(sender as Button);
        }

        private void RevertSortButtonColor(Button iconButton)
        {
            if (iconButton.BorderColor == Color.Blue)
            {
                iconButton.BorderColor = Color.BlueViolet;
            }
            else
            {
                this.SortByHostBtn.BorderColor = Color.BlueViolet;
                this.SortByGamesBtn.BorderColor = Color.BlueViolet;
                iconButton.BorderColor = Color.Blue;
            }
        }
    }
}