using BikeVT.Models;
using Plugin.GoogleClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool isNewUser = false;
        bool noPersonalInfo = false;

        public HomePage()
        {
            InitializeComponent();

            loginLabel.Text = "You are not logged in!\nGo back and try again.";

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
                        loginLabel.Text = "Logged in! Loading your data...";
                        logoutButton.IsVisible = false;

                        App.user.Id = a.Data.Id;
                        App.user.GivenName = a.Data.GivenName;
                        App.user.FamilyName = a.Data.FamilyName;
                        App.user.Email = a.Data.Email;

                        var currentUser = await firebaseHelper.GetUser(App.user.Id);
                        if (currentUser == null)
                        {
                            isNewUser = true;
                            await firebaseHelper.AddUser(App.user);
                        }
                        else
                        {
                            App.user.Age = currentUser.Age;
                            App.user.BikerStatus = currentUser.BikerStatus;
                            App.user.Gender = currentUser.Gender;
                            App.user.Weight = currentUser.Weight;

                            if (App.user.Gender.Equals("N/A")) 
                            {
                                noPersonalInfo = true;
                            }

                        }

                        updatePageOnLogin();

                        App.loggedIn = true;
                        break;
                }
            };
        }

        private void updatePageOnLogin()
        {
            // Put an alert to tell them to add personal information
            DisplayAlert("Welcome new user!", "Please update your personal info before continuing.", "OK");

            tripButton.IsVisible = !(isNewUser || noPersonalInfo);

            loginLabel.Text = "Welcome " + App.user.GivenName + "!";
            idLabel.Text = "ID: " + App.user.Id;
            givenLabel.Text = "First Name: " + App.user.GivenName;
            familyLabel.Text = "Last Name: " + App.user.FamilyName;
            emailLabel.Text = "Email: " + App.user.Email;

            idLabel.IsVisible = true;
            givenLabel.IsVisible = true;
            familyLabel.IsVisible = true;
            emailLabel.IsVisible = true;

            personalInfoButton.IsVisible = true;
            logoutButton.IsVisible = true;
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

            Navigation.PushAsync(new PersonalInfoPage(this));

        }

        public void ShowTripsButton() 
        {
            tripButton.IsVisible = true;
            loginLabel.Text = "Welcome " + App.user.GivenName + "!";
        }

    }
}