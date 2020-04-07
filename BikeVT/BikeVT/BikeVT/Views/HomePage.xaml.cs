using Plugin.GoogleClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            loginLabel.IsVisible = false;
            idLabel.IsVisible = false;
            nameLabel.IsVisible = false;
            givenLabel.IsVisible = false;
            familyLabel.IsVisible = false;
            emailLabel.IsVisible = false;

            tripButton.IsVisible = true;

            CrossGoogleClient.Current.OnLogin += (s, a) =>
            {
                switch (a.Status)
                {
                    case GoogleActionStatus.Completed:
                        App.user.Id = a.Data.Id;
                        App.user.Name = a.Data.Name;
                        App.user.GivenName = a.Data.GivenName;
                        App.user.FamilyName = a.Data.FamilyName;
                        App.user.Email = a.Data.Email;

                        updatePageOnLogin();

                        App.loggedIn = true;
                        break;
                }
            };
        }

        private void updatePageOnLogin()
        {
            idLabel.Text = "ID: " + App.user.Id;
            nameLabel.Text = "Name: " + App.user.Name;
            givenLabel.Text = "Given Name: " + App.user.GivenName;
            familyLabel.Text = "Family Name: " + App.user.FamilyName;
            emailLabel.Text = "Email: " + App.user.Email;

            loginLabel.IsVisible = true;

            idLabel.IsVisible = true;
            nameLabel.IsVisible = true;
            givenLabel.IsVisible = true;
            familyLabel.IsVisible = true;
            emailLabel.IsVisible = true;

            tripButton.IsVisible = true;

            logoutButton.Text = "Logout";
        }

        public void OnLogOutClicked(object sender, EventArgs args)
        {
            App.loggedIn = false;

            CrossGoogleClient.Current.Logout();

            Navigation.InsertPageBefore(new LoginPage(), this);
            Navigation.PopToRootAsync();
        }

        public void OnTripClicked(object sender, EventArgs args) 
        {

            Navigation.PushAsync(new MainPage());

        }

    }
}