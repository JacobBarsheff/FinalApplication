using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTracker.Models
{
    public class Geography
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
        public double direction { get; set; }
    }

    public class Speed
    {
        public double horizontal { get; set; }
        public double isGround { get; set; }
        public double vertical { get; set; }
    }

    public class Departure
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
    }

    public class Arrival
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
    }

    public class Aircraft
    {
        public string regNumber { get; set; }
        public string icaoCode { get; set; }
        public string icao24 { get; set; }
        public string iataCode { get; set; }
    }

    public class Airline
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
    }

    public class Flight
    {
        public string iataNumber { get; set; }
        public string icaoNumber { get; set; }
        public string number { get; set; }
    }

    public class System
    {
        public string updated { get; set; }
        public string squawk { get; set; }
    }
    public class FlightData
    {
        public Geography geography { get; set; }
        public Speed speed { get; set; }
        public Departure departure { get; set; }
        public Arrival arrival { get; set; }
        public Aircraft aircraft { get; set; }
        public Airline airline { get; set; }
        public Flight flight { get; set; }
        public System system { get; set; }
        public string status { get; set; }
    }
}
