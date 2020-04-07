using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Plugin.Geolocator;
using System.Threading.Tasks;

namespace BikeVT.ViewModels
{
    public class ToolsViewModel : INotifyPropertyChanged
    {
        String gyrodata = "GyroScope Stopped";//string.Empty;
        public string Gyrodata
        {
            get { return gyrodata; }
            set { gyrodata = value; OnPropertyChanged(); }
        }

        String accelerometerdata = "Accelerometer Stopped";//string.Empty;
        public string AccelerometerData
        {
            get { return accelerometerdata; }
            set { accelerometerdata = value; OnPropertyChanged(); }
        }

        String gpsData = "GPS Stopped";

        public string GPSData
        {
            get { return gpsData; }
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



        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
