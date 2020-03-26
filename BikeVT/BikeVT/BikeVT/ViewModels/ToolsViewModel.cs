using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

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



        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
