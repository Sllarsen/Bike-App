using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BikeVT.Views;
using Plugin.GoogleClient.Shared;
using BikeVT.Models;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BikeVT
{
    public partial class App : Application
    {

        public static User user;
        public static bool loggedIn;

        public App()
        {
            InitializeComponent();

            // MainPage = new MainPage();

            user = new User();

            loggedIn = false;

            if (loggedIn)
            {
                MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
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
