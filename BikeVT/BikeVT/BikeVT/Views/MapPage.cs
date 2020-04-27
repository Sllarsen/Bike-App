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
using BikeVT.Models;
using Plugin.Geolocator;
using System.Collections;

/**
 * some code from 
 * https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
 */
namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {

        private FirebaseHelper fbh = new FirebaseHelper();
        public Trip t = new Trip();

        String acelPackage = "";
        delegate void AcelPackageEventHandler();
        event AcelPackageEventHandler SendAcelPackage;
        async void HandleSendAcelPackage()
        {
            string temp = String.Copy(acelPackage);
            acelPackage = "";

            await fbh.AddAcelData(App.user, t, temp);
        }

        String gyroPackage = "";
        delegate void GyroPackageEventHandler();
        event GyroPackageEventHandler SendGryoPackage;
        async void HandleSendGyroPackage()
        {
            string temp = String.Copy(gyroPackage);
            gyroPackage = "";

            await fbh.AddGyroData(App.user, t, temp);
        }

            SensorSpeed speed = SensorSpeed.UI;



        //Events for uplaoding GPS Data
        String GPSPackage = "";
        delegate void GPSPackageEventHandler();
        event GPSPackageEventHandler SendGPSPackage;
        async void HandleSendGPSPackage()
        {
            string temp = String.Copy(GPSPackage);
            GPSPackage = "";

            await fbh.AddGPSData(App.user, t, temp);
        }





        //constructor
        public MapPage()
        { 
            InitializeComponent();

            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            this.SendAcelPackage += HandleSendAcelPackage;
            this.SendGryoPackage += HandleSendGyroPackage;
            this.SendGPSPackage += HandleSendGPSPackage;

            GetPositionCommand = new Command(async () => await OnGetPosition());
            BindingContext = this;  // Allows you to use "{Binding VAR}" in .xaml
            EndTrip.IsVisible = false;
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

        private async void EndTrip_Clicked (object sender, EventArgs e)
        {
            ToggleAccelerometer();
            ToggleGyroscope();
            breakGPS = true;
            await fbh.AddAcelData(App.user, t, acelPackage);
            await fbh.AddGyroData(App.user, t, gyroPackage);
            await fbh.AddGPSData(App.user, t, GPSPackage);
            acelPackage = "";
            gyroPackage = "";
            GPSPackage = "";

            ButtonOpenCoords.IsVisible = true;
            EndTrip.IsVisible = false;
            var c_locator = CrossGeolocator.Current;
            var test_loc = Task.Run(() => c_locator.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;
            t.EndLocation = "lat: " + test_loc.Latitude + ", long: " + test_loc.Longitude;
            t.EndTime = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff");
            await fbh.UpdateTripToUser(App.user, t);
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

            t.StartTime = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff");
            t.WeatherData = "not clicked"; //FIXME
            var c_locator = CrossGeolocator.Current;
            var test_loc = Task.Run(() => c_locator.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;
            t.StartLocation = "lat: " + test_loc.Latitude + ", long: " + test_loc.Longitude;

            ButtonOpenCoords.IsVisible = false;
            EndTrip.IsVisible = true;

            await fbh.AddTripToUser(App.user, t);
            ToggleAccelerometer();
            ToggleGyroscope();
            breakGPS = false;
            _ = Task.Run(() => updateGPS()); 



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

        public void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;

            acelPackage += $"{DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff")},{data.Acceleration.X},{data.Acceleration.Y},{data.Acceleration.Z}/";


            if (acelPackage.Length >= 100000) {
                SendAcelPackage();
            }
        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                {
                    //viewModel.AccelerometerData = "Accelerometer Stopped";
                    Accelerometer.Stop();
                }

                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
            }
            catch (Exception)
            {
                // Other error has occurred.
            }
        }

        public void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;

            gyroPackage += $"{DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff")},{data.AngularVelocity.X},{data.AngularVelocity.Y},{data.AngularVelocity.Z}/";

            if (gyroPackage.Length >= 100000) {
                SendGryoPackage();
            }
        }

        public void ToggleGyroscope()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                {
                    Gyroscope.Stop();
                }

                else
                    Gyroscope.Start(speed);
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
            }
            catch (Exception)
            {
                // Other error has occurred.
            }
        }

        //Bool to brak out of GPS loops
        bool breakGPS = false;
        public void updateGPS()
        {

            for (; !breakGPS ;)
            {
                var c_locator = CrossGeolocator.Current;
                if (c_locator.IsGeolocationAvailable && c_locator.IsGeolocationEnabled)
                {
                    var test_loc = Task.Run(() => c_locator.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;
                    if(gpsData != test_loc.Latitude + "," + test_loc.Longitude)
                    {
                        gpsData =  test_loc.Latitude + "," + test_loc.Longitude;

                        long curr = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        GPSPackage += DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff") + "," + gpsData + "/";
                        if(GPSPackage.Length >= 100000)
                        {
                            SendGPSPackage();

                        }

                    }
 
                }
                else
                {
                    gpsData = "gps services not enabled";
                }

                
                //Console.WriteLine(DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff") + " gps updated: " + gpsData );
            }
        }


        private String gpsData = "GPS Stopped";
        public string GPSData
        {
            get
            {
                var c_locator = CrossGeolocator.Current;
                if (c_locator.IsGeolocationAvailable && c_locator.IsGeolocationEnabled)
                {
                    var test_loc = Task.Run(() => c_locator.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;
                    gpsData = "lat = " + test_loc.Latitude + " long: " + test_loc.Longitude;
                }
                else
                {
                    gpsData = "gps services not enabled";
                }
                return gpsData;
            }
            set
            {
                var c_locator = CrossGeolocator.Current;
                if (c_locator.IsGeolocationAvailable && c_locator.IsGeolocationEnabled)
                {
                    var test_loc = Task.Run(() => c_locator.GetPositionAsync(TimeSpan.FromSeconds(.5))).Result;
                    gpsData = "lat = " + test_loc.Latitude + " long: " + test_loc.Longitude;
                }
                else
                {
                    gpsData = "gps services not enabled";
                }
            }
        }





    }








}
