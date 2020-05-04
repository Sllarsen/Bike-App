using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeVT.Models;
using BikeVT.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace BikeVT.Views
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ToolsPage : ContentPage
	{
        private ToolsViewModel viewModel;
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;

        public ToolsPage ()
		{
			InitializeComponent ();
            viewModel = new ToolsViewModel();
            BindingContext = viewModel;

            // Register for reading changes.
            //Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            // Register for reading changes, be sure to unsubscribe when finished
            //Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;

            Task.Run(() => updateGPS());
        }

        void updateGPS()
        {
            for (; ;)
            {
                viewModel.updateGPSData();
            }
        }
        /*void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;
            // Process Angular Velocity X, Y, and Z reported in rad/s
            viewModel.Gyrodata = $"Reading: X: {data.AngularVelocity.X}, Y: {data.AngularVelocity.Y}, Z: {data.AngularVelocity.Z}";
            Console.WriteLine($"Reading: X: {data.AngularVelocity.X}, Y: {data.AngularVelocity.Y}, Z: {data.AngularVelocity.Z}");
        }*/

        public void ToggleGyroscope(object sender, EventArgs args)
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                {
                    Gyroscope.Stop();
                    viewModel.Gyrodata = "GyroScope Stopped";
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


        /*void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            viewModel.AccelerometerData = $"Reading: X: {data.Acceleration.X}, Y: {data.Acceleration.Y}, Z: {data.Acceleration.Z}";
            Console.WriteLine($"Reading: X: {data.Acceleration.X}, Y: {data.Acceleration.Y}, Z: {data.Acceleration.Z}");
            // Process Acceleration X, Y, and Z
        }*/

        public void ToggleAccelerometer(object sender, EventArgs args)
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                {
                    viewModel.AccelerometerData = "Accelerometer Stopped";
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

    }
}