using Plugin.GoogleClient;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public void OnLoginClicked(object sender, EventArgs args)
        {

            CrossGoogleClient.Current.LoginAsync();
            Navigation.InsertPageBefore(new WelcomePage(), this);
            Navigation.PopAsync();

        }
    }
}