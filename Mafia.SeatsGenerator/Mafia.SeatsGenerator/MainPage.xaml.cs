using System.Collections.ObjectModel;
using Mafia.SeatsGenerator.Models;
using Mafia.SeatsGenerator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Mafia.SeatsGenerator
{
    public partial class MainPage : STabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
        }
    }
}