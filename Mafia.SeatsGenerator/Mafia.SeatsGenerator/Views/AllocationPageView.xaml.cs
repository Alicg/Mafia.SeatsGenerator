using DynamicData.Binding;
using Mafia.SeatsGenerator.Utils;
using Mafia.SeatsGenerator.ViewModels;
using System;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Mafia.SeatsGenerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllocationPageView : STabbedPage
    {
        public AllocationPageView()
        {
            InitializeComponent();
            
            this.On<Android>().SetIsSwipePagingEnabled(false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is AllocationPageViewModel vm)
            {
                vm.WhenPropertyChanged(v => v.SelectedPage).Subscribe(v =>
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

        private void AllocationPageView_OnCurrentPageChanged(object sender, EventArgs e)
        {
            if (this.BindingContext is AllocationPageViewModel vm)
            {
                vm.SelectedPage = this.CurrentPage?.BindingContext as BindableObject;
            }
        }
    }
}