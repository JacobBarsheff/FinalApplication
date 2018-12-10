using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTracker.Models
{
    public struct LocationCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public LocationCoordinates(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}
