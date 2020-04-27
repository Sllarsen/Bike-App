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
            var string_age = ageEntry.Text;
            var bstatus = (String)bikerStatusPicker.SelectedItem;
            var gender = (String)genderPicker.SelectedItem;
            var weight = weightEntry.Text;

            int int_age;
            bool isNumeric = int.TryParse(string_age, out int_age);

            if (String.IsNullOrWhiteSpace(string_age) || String.IsNullOrWhiteSpace(bstatus) || String.IsNullOrWhiteSpace(gender) || String.IsNullOrWhiteSpace(weight))
            {
                await DisplayAlert("Couldn't Save!", "All entries must be filled in order to save changes", "OK");
            }else if (!isNumeric)
            {
                await DisplayAlert("Invalid Age", "Age must be an integer.", "OK");
            }
            else
            {
                App.user.Age = int_age;
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