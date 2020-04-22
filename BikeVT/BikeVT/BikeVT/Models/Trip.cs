using System;
using System.Collections.Generic;
using System.Text;

namespace BikeVT.Models
{
    public class Trip
    {

        public string WeatherData { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string StartLocation { get; set; }

        public string EndLocation { get; set; }

        public Acel [] acel { get; set; }

    }
}
