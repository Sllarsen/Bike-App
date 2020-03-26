using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BikeVT.ViewModels
{
    public class ToolsViewModel : INotifyPropertyChanged
    {
        String name = "GyroScope Stopped";//string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
