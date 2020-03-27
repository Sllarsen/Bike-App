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

        string destination;
        string coordMsg;
        double latitude;
        double longitude;
        bool mapButtonIsEnabled = false;

        public ICommand GetPositionCommand { get; }


        public String Destination
        {
            get => destination;
            set => SetProperty(ref destination, value);

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
                var locations = await Geocoding.GetLocationsAsync(Destination);
                Location location = locations.FirstOrDefault();
                if (location == null)
                {
                    CoordMsg = "Unable to detect locations";
                    MapButtonIsEnabled = false;
                    //TODO: Check if they just typed in coordinates
                }
                else
                {

                    latitude = location.Latitude;
                    longitude = location.Longitude;
                    CoordMsg = "(" + latitude + ", " + longitude + ")";
                    MapButtonIsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                CoordMsg = $"Unable to detect locations: {ex.Message}";
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
            if (! MapButtonIsEnabled)   //Coords have not been set
                return;
            if (Destination == null)
                return;

            // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.textinfo.totitlecase?view=netframework-4.8
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            string formattedTitle = myTI.ToTitleCase(Destination);

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
