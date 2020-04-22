using BikeVT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalInfoPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        HomePage homePage;

        public PersonalInfoPage(HomePage homePage)
        {
            InitializeComponent();

            this.homePage = homePage;

            ageEntry.Text = App.user.Age.ToString();
            bikerStatusPicker.SelectedItem = App.user.BikerStatus;
            genderPicker.SelectedItem = App.user.Gender;
            weightEntry.Text = App.user.Weight.ToString();

        }

        async void OnSaveClicked(object sender, EventArgs args) 
        {
            var age = ageEntry.Text;
            var bstatus = (String)bikerStatusPicker.SelectedItem;
            var gender = (String)genderPicker.SelectedItem;
            var weight = weightEntry.Text;

            if (String.IsNullOrWhiteSpace(age) || String.IsNullOrWhiteSpace(bstatus) || String.IsNullOrWhiteSpace(gender) || String.IsNullOrWhiteSpace(weight))
            {
                await DisplayAlert("Couldn't Save!", "All entries must be filled in order to save changes", "OK");
            }
            else
            {
                App.user.Age = Int32.Parse(age);
                App.user.BikerStatus = bstatus;
                App.user.Gender = gender;
                App.user.Weight = Int32.Parse(weight);

                await firebaseHelper.UpdateUserInfo(App.user);

                await DisplayAlert("Successfully Saved!", "Your personal information has been updated", "OK");

                homePage.ShowTripsButton();
            }
        }

    }
}