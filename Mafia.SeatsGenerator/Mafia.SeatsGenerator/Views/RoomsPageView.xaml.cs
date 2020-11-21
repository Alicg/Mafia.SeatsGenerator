﻿using System;
using DynamicData.Binding;
using Mafia.SeatsGenerator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using STabbedPage = Mafia.SeatsGenerator.Utils.STabbedPage;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomsPageView : STabbedPage
    {
        public RoomsPageView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is RoomsPageViewModel vm)
            {
                vm.WhenPropertyChanged(v => v.SelectedRoomPage).Subscribe(v =>
                {
                    foreach (var childPage in this.Children)
                    {
                        if (childPage.BindingContext == v.Value)
                        {
                            this.CurrentPage = childPage;
                            break;
                        }
                    }
                });
            }
        }

        private void RoomsPageView_OnCurrentPageChanged(object sender, EventArgs e)
        {
            if (this.BindingContext is RoomsPageViewModel vm)
            {
                vm.SelectedRoomPage = this.CurrentPage?.BindingContext as BindableObject;
            }
        }
    }
}