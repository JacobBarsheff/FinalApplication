using FlightTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTracker.DataService
{
    public class AirportDataService
    {

        /// <summary>
        /// read the json file and load a list of character objects
        /// </summary>
        /// <returns>list of characters</returns>
        public static List<Airport> ReadAll()
        {

            List<Airport> airports = new List<Airport>();
            try
            {
                using (StreamReader sr = new StreamReader("Airport.json"))
                {
                    string jsonString = sr.ReadToEnd();

                    airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);

                }

            }
            catch (Exception)
            {
                throw;
            }
            return airports;
        }
    }
}
