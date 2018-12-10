using FlightTracker.DataService;
using FlightTracker.Models;
using FlightTracker.ViewModels.Commands;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlightTracker.ViewModels
{
    public class ViewFlightDataViewModel : ObservableObject
    {

        public ObservableCollection<GMapMarker> mapMarkers { get; set; }
        public ObservableCollection<GMapMarker> flightsOnRouteToThisAirport { get; set; }
        public ObservableCollection<FlightData> flightDataRecords { get; set; }
        public ObservableCollection<GMapPolygon> flightRouteShapes { get; set; }
        public ObservableCollection<string> errors { get; set; }
        public MapCommands setMapLatLong { get; private set; }
        public MapCommands showError { get; private set; }
        public FlightData selectedFlight { get; set; }
        private WeatherData _weatherData;
        private string flightID;

        private string _errorLabel;

        public string ErrorLabel
        {
            get { return _errorLabel; }
            set { _errorLabel = value;
                OnPropertyChanged("ErrorLabel");
            }
        }

        public string FlightId
        {
            get { return flightID; }
            set { flightID = value;
                OnPropertyChanged("FlightId");

            }
        }

        public WeatherData weatherData
        {
            get { return _weatherData; }
            set { _weatherData = value;
                OnPropertyChanged("weatherData");

            }
        }


        public Airport airport { get; set; }
        public MapCommands loadFlightData { get; set; }

        public GMapProvider mapProvider { get; set; }
        private PointLatLng _center;
        private int _MapZoom;

        public int MapZoom
        {
            get { return _MapZoom; }
            set
            {

                _MapZoom = value;
                OnPropertyChanged("MapZoom");
            }
        }

        public PointLatLng Center
        {
            get
            {
                if (_center == null)
                {
                    _center = new PointLatLng(39.9, -84.4);
                }
                return _center;
            }
            set
            {
                _center = value;
                OnPropertyChanged("Center");
            }
        }
        private string _recordCount;

        public string RecordCount
        {
            get { return _recordCount; }
            set { _recordCount = value;
                OnPropertyChanged("RecordCount");

            }
        }

        public ViewFlightDataViewModel()
        {
            flightDataRecords = new ObservableCollection<FlightData>();
            mapMarkers = new ObservableCollection<GMapMarker>();
            flightsOnRouteToThisAirport = new ObservableCollection<GMapMarker>();
            // GenerateMapMarkers();
            mapProvider = GMapProviders.GoogleHybridMap;
            MapZoom = 4;
            flightRouteShapes = new ObservableCollection<GMapPolygon>();
            errors = new ObservableCollection<string>();
            setMapLatLong = new MapCommands(SetMapLatLong);
            showError = new MapCommands(showErrorView);
        }
        public void SetMapLatLong()
        {
            if (selectedFlight != null)
            {
                
                PointLatLng pointLatLng = new PointLatLng(selectedFlight.geography.latitude, selectedFlight.geography.longitude);
                Center = pointLatLng;

                WeatherData validatedData = WeatherAPIDataService.GetCurrentWeatherData(new LocationCoordinates(selectedFlight.geography.latitude, selectedFlight.geography.longitude), out WeatherAPIError error);
                if (error == WeatherAPIError.VALID)
                {
                    weatherData = validatedData;
                    weatherData.Name = "Location: " + weatherData.Name;
                }
                else
                {
                    weatherData.Name = "No Data Available";
                }

                FlightId = selectedFlight.aircraft.regNumber;
                MapZoom = 8;
            }

        }
        public void GenerateMapMarkers()
        {
            BitmapImage airplaneMarker = new BitmapImage();
            airplaneMarker.BeginInit();
            airplaneMarker.UriSource = new Uri(@"\Resources\plane.png", UriKind.Relative);
            airplaneMarker.EndInit();

            GMapMarker marker = new GMapMarker(new PointLatLng(44.7, -85.6));
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = airplaneMarker;
            //size image
            image.Height = 20;
            image.Width = 20;
            //set the marker shape to the image
            marker.Shape = image;
            //add marker to the map!
            mapMarkers.Add(marker);               
        }
        public void showErrorView()
        {
            ErrorView ev = new ErrorView(errors);
            ev.Show();
        }

    }
}
