using System;
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
    }
}