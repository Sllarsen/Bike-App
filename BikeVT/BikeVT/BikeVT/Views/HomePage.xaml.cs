using BikeVT.Models;
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
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public HomePage()
        {
            InitializeComponent();

            loginLabel.IsVisible = false;
            idLabel.IsVisible = false;
            givenLabel.IsVisible = false;
            familyLabel.IsVisible = false;
            emailLabel.IsVisible = false;

            personalInfoButton.IsVisible = false;
            tripButton.IsVisible = false;

            CrossGoogleClient.Current.OnLogin += async (s, a) =>
            {
                switch (a.Status)
                {
                    case GoogleActionStatus.Completed:
                        App.user.Id = a.Data.Id;
                        App.user.GivenName = a.Data.GivenName;
                        App.user.FamilyName = a.Data.FamilyName;
                        App.user.Email = a.Data.Email;

                        updatePageOnLogin();
                        var currentUser = await firebaseHelper.GetUser(App.user.Id);
                        if (currentUser == null) {
                            loginLabel.Text = "User was not found in database!";
                            await firebaseHelper.AddUser(App.user);
                        }

                        await firebaseHelper.AddTripToUser(App.user, "Trip Name");

                        App.loggedIn = true;
                        break;
                }
            };
        }

        private void updatePageOnLogin()
        {
            idLabel.Text = "ID: " + App.user.Id;
            givenLabel.Text = "Given Name: " + App.user.GivenName;
            familyLabel.Text = "Family Name: " + App.user.FamilyName;
            emailLabel.Text = "Email: " + App.user.Email;

            loginLabel.IsVisible = true;
            idLabel.IsVisible = true;
            givenLabel.IsVisible = true;
            familyLabel.IsVisible = true;
            emailLabel.IsVisible = true;

            personalInfoButton.IsVisible = true;
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

        public void OnPersonalInfoClicked(object sender, EventArgs args) 
        {

            Navigation.PushAsync(new PersonalInfoPage());

        }

    }
}