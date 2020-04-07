using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//For Capitalizing strings
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//For Binding:
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Xamarin.Essentials;

/**
 * some code from 
 * https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
 */
namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
            GetPositionCommand = new Command(async () => await OnGetPosition());
            BindingContext = this;  // Allows you to use "{Binding VAR}" in .xaml
        }

        string searchDest;
        string identifiedDest;
        string coordMsg;
        double latitude;
        double longitude;
        bool mapButtonIsEnabled = false;

        public ICommand GetPositionCommand { get; }

        public String SearchDest
        {
            get => searchDest;
            set => SetProperty(ref searchDest, value);
        }
        public String IdentifiedDest
        {
            get => identifiedDest;
            set => SetProperty(ref identifiedDest, value);
        }
        public String CoordMsg
        {
            get => coordMsg;
            set => SetProperty(ref coordMsg, value);
        }

        public Boolean MapButtonIsEnabled
        {
            get => mapButtonIsEnabled;
            set => SetProperty(ref mapButtonIsEnabled, value);
        }

        /**
         * Code from https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
         * but modified
         */
        async Task OnGetPosition()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var locations = await Geocoding.GetLocationsAsync(SearchDest);
                // TODO: Supposedly `locations` contains multiple locations based on the search query.
                //       Ideally we'd like to sort by places by closest to the user's current location,
                //       but `locations` seems to only have one location.
                
                // Console.WriteLine("Number of items in `locations`:"+locations.Count());               
                // foreach (var item in locations)
                // {Console.WriteLine("locations:" + item.ToString());}

                Location location = locations.FirstOrDefault();

                if (location == null)
                {
                    CoordMsg = "";
                    IdentifiedDest = "Unable to locate destination";
                    MapButtonIsEnabled = false;
                    //TODO: Check if they just typed in coordinates and use that
                }
                else
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                    CoordMsg = "[" + latitude + ", " + longitude + "]";

                    // Find Address info (text)
                    var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                    Placemark placemark = placemarks.FirstOrDefault();
                    if (placemark == null)
                    {
                        IdentifiedDest = "";
                    }
                    else
                    {
                        // If .FeatureName is the name of the building, we want it on a new line
                        // if it is just the street number, we want it on the same line

                        IdentifiedDest =
                                placemark.FeatureName +
                                //placemark.SubThoroughfare seems to be the same as .FeatureName

                                (double.TryParse(placemark.FeatureName, out double unusedVar) ? " " : "\n") +
                                // If .FeatureName is the name of the building, we want it on a new line
                                // if it is just the street number, we want it on the same line

                                placemark.Thoroughfare + "\n" +
                                placemark.Locality + ", " +
                                placemark.AdminArea + " " +
                                placemark.PostalCode + "\n" +
                                "(" +
                                (string.IsNullOrEmpty(placemark.SubAdminArea) ? "" : (placemark.SubAdminArea + ", ")) +
                                placemark.CountryCode +
                                ")";
                    }

                    // Allow users to open google maps
                    MapButtonIsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                CoordMsg = "";
                IdentifiedDest = $"Unable to identify destination: {ex.Message}";
                MapButtonIsEnabled = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        //https://docs.microsoft.com/en-us/xamarin/essentials/maps?context=xamarin%2Fxamarin-forms&tabs=android
        private async void ButtonOpenCoords_Clicked(object sender, EventArgs e)
        {
            if (!MapButtonIsEnabled)   //Coords have not been set
                return;
            if (SearchDest == null)
                return;

            // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.textinfo.totitlecase?view=netframework-4.8
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            string formattedTitle = myTI.ToTitleCase(SearchDest);

            // Open default "Maps" application
            await Map.OpenAsync(latitude, longitude, new MapLaunchOptions
            {
                Name = formattedTitle,
                NavigationMode = NavigationMode.Bicycling
                //Automatically sets directions in "Biking" mode
                //Use `NavigationMode.None` to just show them the location
            });
        }

        protected virtual bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null, Func<T, T, bool> validateValue = null)
        {
            // Copied from https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            if (validateValue != null && !validateValue(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
