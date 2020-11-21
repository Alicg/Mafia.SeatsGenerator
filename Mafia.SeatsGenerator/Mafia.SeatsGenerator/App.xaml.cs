using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("iconize-fontawesome-regular.ttf", Alias = "FA-R")]
[assembly: ExportFont("iconize-fontawesome-solid.ttf", Alias = "FA-S")]

namespace Mafia.SeatsGenerator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            XF.Material.Forms.Material.Init(this);

            Plugin.Iconize.Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule())
                .With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule())
                .With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule());

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}