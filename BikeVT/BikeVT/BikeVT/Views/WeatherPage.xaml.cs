using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Geolocator;
using Plugin.Permissions;

namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherPage : ContentPage
    {
        RestService _restService;
        public WeatherPage()
        {
            InitializeComponent();
            _restService = new RestService();
        }

        async void OnGetWeatherButtonClicked(object sender, EventArgs e)
        {
            string req_uri = await GenerateRequestUri(Constants.OpenWeatherMapEndpoint);
            if(req_uri != null){
                WeatherData weatherData = await _restService.GetWeatherData(req_uri);
                BindingContext = weatherData;
            }
       
        }

        /**
         * Gets weather conditions. If we don't have location permissions, returns NULL
         */
        async Task<string> GenerateRequestUri(string endpoint)
        {
            //https://github.com/jamesmontemagno/PermissionsPlugin
            try
            {
                if (await hasLocationPermissions())
                {
                    //Query permission

                    var c_locator = CrossGeolocator.Current;
                    var test_loc = Task.Run(() => c_locator.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;

                    string requestUri = endpoint;
                    requestUri += "?lat=" + test_loc.Latitude;
                    requestUri += "&lon=" + test_loc.Longitude;
                    requestUri += "&units=imperial"; // or units=metric
                    requestUri += $"&APPID={Constants.OpenWeatherMapAPIKey}";
                    return requestUri;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                //Something went wrong
                await DisplayAlert("Error getting location","Something went wrong.", "OK");
                Console.WriteLine("Something went wrong when getting location for weather.\n" + ex);

                return null;
            }
        }

        /**
		 * Includes code from https://github.com/jamesmontemagno/PermissionsPlugin
		 * Returns if we have location permissions.
		 * Will prompt users for permission if False (but will still return False regardless if they change settings)
		 */
        async Task<bool> hasLocationPermissions()
        {
            Plugin.Permissions.Abstractions.PermissionStatus device_status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

            if (device_status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await DisplayAlert("Location required", "Location permissions are required to look up destination.", "OK");

                //Open prompt to settings:
                await Utils.CheckPermissions(new LocationPermission());

                //The next lines dont wait for you to return from the Settings app. It'll think you denied permissions
                Console.WriteLine("refreshing");
                device_status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }
            return device_status == Plugin.Permissions.Abstractions.PermissionStatus.Granted;
        }
    }

}