using FlightTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlightTracker.DataService
{
    public class WeatherAPIDataService
    {

        public static WeatherData GetCurrentWeatherData(LocationCoordinates location, out WeatherAPIError weatherAPIError)
        {
            string url;
            
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("http://api.openweathermap.org/data/2.5/weather?");
            sb.Append("&lat=" + location.Latitude + "&lon=" + location.Longitude);
            sb.Append("&appid=668f986bf179bbac7174d55d13ad1a71");

            url = sb.ToString();

            WeatherData currentWeather = new WeatherData();

            try
            {
                string result = null;

                using (WebClient syncClient = new WebClient())
                {
                    result = syncClient.DownloadString(url);
                }

                currentWeather = JsonConvert.DeserializeObject<WeatherData>(result);
                currentWeather.Main.Temp = Math.Round(ConvertToFahrenheit(currentWeather.Main.Temp));
                weatherAPIError = WeatherAPIError.VALID;
            }
            catch (Exception)
            {

                weatherAPIError = WeatherAPIError.INVALID;
            }

            return currentWeather;
        }
        static double ConvertToFahrenheit(double degreesKalvin)
        {
            return (degreesKalvin - 273.15) * 1.8 + 32;
        }
    }
}
