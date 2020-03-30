using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using System.Collections.ObjectModel;
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
            //TODO: probably want to check for location here if we aren't making sure it isn't enabled elsewhere
                WeatherData weatherData = await _restService.GetWeatherData(GenerateRequestUri(Constants.OpenWeatherMapEndpoint));
                BindingContext = weatherData;
        }

        string GenerateRequestUri(string endpoint)
        {
            //TODO: check or otherwise verify that location services are enabled
            var test_loc = Task.Run(() => CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;
            string requestUri = endpoint;
            requestUri += "?lat=" + test_loc.Latitude;
            requestUri += "&lon=" + test_loc.Longitude;
            requestUri += "&units=imperial"; // or units=metric
            requestUri += $"&APPID={Constants.OpenWeatherMapAPIKey}";
            return requestUri;
        }
    }

}