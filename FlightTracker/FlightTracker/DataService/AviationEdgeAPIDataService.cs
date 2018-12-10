using FlightTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlightTracker.DataService
{
    public class AviationEdgeAPIDataService
    {
        public static List<FlightData> GetCurrentFlightData(string urlParameter, out APIErrorCode errorCode)
        {
            string url;
            string key = "d99543-996767";

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("http://aviation-edge.com/v2/public/flights?");
            sb.Append($"key={key}");
            sb.Append("&" + urlParameter);


            url = sb.ToString();

            List<FlightData> currentFlights = new List<FlightData>();

            string result = null;
            List<FlightData> results = new List<FlightData>();
            try
            {
                using (WebClient syncClient = new WebClient())
                {
                    result = syncClient.DownloadString(url);
                }
                results = JsonConvert.DeserializeObject<List<FlightData>>(result);
                errorCode = APIErrorCode.VALIDFLIGHTDATA;
            }
            catch (Exception)
            {
                errorCode = APIErrorCode.INVALIDFLIGHTDATA;
            }

            currentFlights = results;

            return currentFlights;
        }
        //public static List<FlightData> HttpGetCurrentFlightDataByLimit(string url)
        //{
        //    string result = null;
        //    try
        //    {
        //        using (WebClient syncClient = new WebClient())
        //        {
        //            result = syncClient.DownloadString(url);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }

        //    var results = JsonConvert.DeserializeObject<List<FlightData>>(result);
        //    return results;

        //}
        /// <summary>
        /// read the json file and load a list of character objects
        /// </summary>
        /// <returns>list of characters</returns>
        public static List<FlightData> ReadAll()
        {

            List<FlightData> characters = new List<FlightData>();
            try
            {
                using (StreamReader sr = new StreamReader("json1.json"))
                {
                    string jsonString = sr.ReadToEnd();

                    characters = JsonConvert.DeserializeObject<List<FlightData>>(jsonString);

                }

            }
            catch (Exception)
            {
                throw;
            }
            foreach (FlightData item in characters)
            {

                List<Airport> list = AirportDataService.ReadAll();
                foreach (Airport airport in list)
                {
                    if (airport.icao.ToString() == item.arrival.icaoCode)
                    {
                        item.arrival.icaoCode = airport.city;
                    }
                }

            }
            return characters;
        }
    }
}
