using System;
using System.Collections.Generic;
using System.Text;

namespace BikeVT.Models
{
    public class User
    {
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }

        public int Age { get; set; }
        public string Gender { get; set; }
        public int Weight { get; set; }
        public string BikerStatus { get; set; }

    }
}
