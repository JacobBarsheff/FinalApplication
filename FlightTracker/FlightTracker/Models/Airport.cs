using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTracker.Models
{
    public class Airport
    {
        public string code { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string woeid { get; set; }
        public string tz { get; set; }
        public string phone { get; set; }
        public string type { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public string runway_length { get; set; }
        public string elev { get; set; }
        public string icao { get; set; }
        public string direct_flights { get; set; }
        public string carriers { get; set; }
    }
}
