using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Essentials;

namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Directions : ContentPage
    {
        public Directions()
        {
            InitializeComponent();
        }
        
        //https://docs.microsoft.com/en-us/xamarin/essentials/maps?context=xamarin%2Fxamarin-forms&tabs=android
        private async void ButtonOpenCoords_Clicked(object sender, EventArgs e)
        {
            if (!double.TryParse(EntryLatitude.Text, out double lat))
                return;
            if (!double.TryParse(EntryLongitutde.Text, out double lng))
                return;

            // Open default "Maps" application
            await Map.OpenAsync(lat, lng, new MapLaunchOptions
            {
                Name = EntryName.Text,
                NavigationMode = NavigationMode.Bicycling
                //Automatically sets directions in "Biking" mode
            });

        }
    }
}