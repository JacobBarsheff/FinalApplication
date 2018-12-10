using FlightTracker.DataService;
using FlightTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FlightTracker.ViewModels.Commands;
using System.Windows;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Windows.Media.Imaging;
using GMap.NET.WindowsPresentation;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FlightTracker.ViewModels
{
    public class AirportSearchViewModel : ObservableObject
    {
        List<string> errorList = new List<string>();                
        //ObservableCollection
        public ObservableCollection<Airport> filteredAirports { get; set; }

        //Commands
        public MessageCommands SortResultsCommand { get; private set; }
        public MapCommands setMapLatLong { get; private set; }
        public MapCommands selectionMade { get; private set; }
        public MapCommands addToFavorites { get; private set; }
        public MapCommands openHelp { get; private set; }

        //strings
        public string Airport { get; set; }

        //Model
        public Airport selectedAirport { get; set; }

        private PointLatLng _center;

        public PointLatLng Center
        {
            get { 
                if (_center == null)
                {
                    _center = new PointLatLng(44, 88);
                }
                return _center;
            }
            set { _center = value;
                OnPropertyChanged("Center");
            }
        }

        private string _airportCount;

        public string AirportCount
        {
            get { return _airportCount; }
            set { _airportCount = value;
                OnPropertyChanged("AirportCount");
            
            }
        }

        private string _airportName;

        public string lbl_AirportInfo
        {
            get {
                if (string.IsNullOrEmpty(_airportName))
                {
                    return "";
                }
                return _airportName; }
            set { _airportName = value;
                OnPropertyChanged("lbl_AirportInfo");
            }
        }

        private int _MapZoom;

        public int MapZoom
        {
            get { return _MapZoom; }
            set {

                _MapZoom = value;
                OnPropertyChanged("MapZoom");
            }
        }

        public AirportSearchViewModel()
        {
            filteredAirports = new ObservableCollection<Airport>(AirportDataService.ReadAll());

            SortResultsCommand = new MessageCommands(FilterAirportList);
            selectionMade = new MapCommands(GatherFlightData);
            setMapLatLong = new MapCommands(SetMapLatLong);
            addToFavorites = new MapCommands(AddToFavorites);
            openHelp = new MapCommands(OpenHelp);
            AirportCount = "Airport Count = " + filteredAirports.Count().ToString();
            Center = new PointLatLng(39.9, -84.4);
            MapZoom = 4;
        }

        public List<Airport> GetAirports()
        {
            List<Airport> airports = new List<Airport>();
            return airports = AirportDataService.ReadAll();
        }

        public void GetFilteredAirports(string searchBoxText)
        {
            //List<Airport> airports = new List<Airport>();
            //airports = AirportDataService.ReadAll();

            List<Airport> airports = AirportDataService.ReadAll();
            List<Airport> filteredList = airports.Where(a => a.name.ToUpper().Contains(searchBoxText.ToUpper())).ToList();
            
                foreach (Airport item in filteredList)
                {
                    filteredAirports.Add(item);
                }

                if (filteredAirports.Count == 0)
                {
                    AirportCount = "Airport Count = " + filteredAirports.Count().ToString() + ". Try to broaden your search!";
                }
                else
                {
                    AirportCount = "Airport Count = " + filteredAirports.Count().ToString();
                }            
        }

        public void FilterAirportList()
        {
            filteredAirports.Clear();
            GetFilteredAirports(Airport);
        }

        public void GatherFlightData()
        {

            if (selectedAirport != null)
            {
                //generate airport MasterList                
                List<Airport> airportMasterList = AirportDataService.ReadAll();

                //generate view model to send results to
                ViewFlightDataViewModel vm = new ViewFlightDataViewModel();

                //generate url request parameters
                string departureAirport = "depIcao=" + selectedAirport.code;
                string arrivalAirport = "arrIcao=" + selectedAirport.code;
             
                //load results in...
                List<FlightData> flightsArrivingToThisAirport = AviationEdgeAPIDataService.GetCurrentFlightData(arrivalAirport, out APIErrorCode arrivalFlightsError);
                //add second request
                List<FlightData> flightsLeavingThisAirport = AviationEdgeAPIDataService.GetCurrentFlightData(departureAirport, out APIErrorCode departureFlightsError);

                ////Test purpose    
                //List<FlightData> test = AviationEdgeAPIDataService.ReadAll();


                if (arrivalFlightsError == APIErrorCode.VALIDFLIGHTDATA)
                {
                    foreach (FlightData item in flightsArrivingToThisAirport)
                    {
                        vm.flightDataRecords.Add(item);

                        GMapMarker marker = GenerateDestinationMarkers(item, airportMasterList, out MarkerErrorCode destinationMarkerErrorCode);
                        if (destinationMarkerErrorCode == MarkerErrorCode.VALID)
                        {
                            vm.flightsOnRouteToThisAirport.Add(marker);

                        }
                        else
                        {
                            errorList.Add("Destination marker missing for flight number: " + item.flight.number);
                        }
                        GMapMarker airplaneMarker = GenerateMapAirplaneMarkers(item, out MarkerErrorCode planeMarkerError);
                        if (planeMarkerError == MarkerErrorCode.VALID)
                        {
                            vm.flightsOnRouteToThisAirport.Add(airplaneMarker);

                        }
                        else
                        {
                            errorList.Add("Airplane marker missing for flight number: " + item.flight.number);
                        }
                        GMapPolygon gMapPolygon = GenerateMapRoutesFromDep(item, airportMasterList, out MarkerErrorCode errorCode);
                        if (errorCode == MarkerErrorCode.VALID)
                        {
                            vm.flightsOnRouteToThisAirport.Add(gMapPolygon);
                        }
                        else
                        {
                            errorList.Add("Partial map route missing for flight number: " + item.flight.number);
                        }
                        GMapPolygon routeFromFlightToArrv = GenerateMapRoutesFromCurrentToArv(item, airportMasterList, out MarkerErrorCode routeToError);
                        if (routeToError == MarkerErrorCode.VALID)
                        {
                            vm.flightsOnRouteToThisAirport.Add(routeFromFlightToArrv);
                        }
                        else
                        {
                            errorList.Add("Partial map route missing for flight number: " + item.flight.number);
                        }
                    }
                }
                if (departureFlightsError == APIErrorCode.VALIDFLIGHTDATA)
                {
                    foreach (FlightData item in flightsLeavingThisAirport)
                    {
                        vm.flightDataRecords.Add(item);
                        GMapMarker destinationMarker = GenerateDestinationMarkers(item, airportMasterList, out MarkerErrorCode destinationMarkerError);
                        if (destinationMarkerError == MarkerErrorCode.VALID)
                        {
                            vm.mapMarkers.Add(destinationMarker);
                        }
                        else
                        {
                            errorList.Add("Destination marker missing for flight number: " + item.flight.number);
                        }
                        GMapMarker airplaneIcon = GenerateMapAirplaneMarkers(item, out MarkerErrorCode airplaneIconError);
                        if (airplaneIconError == MarkerErrorCode.VALID)
                        {
                            vm.mapMarkers.Add(airplaneIcon);
                        }
                        else
                        {
                            errorList.Add("Airplane marker missing for flight number: " + item.flight.number);
                        }
                        GMapPolygon fromDep = GenerateMapRoutesFromDep(item, airportMasterList, out MarkerErrorCode routeFromDepError);
                        if (routeFromDepError == MarkerErrorCode.VALID)
                        {
                            vm.mapMarkers.Add(fromDep);
                        }
                        else
                        {
                            errorList.Add("Partial map route missing for flight number: " + item.flight.number);
                        }
                        GMapPolygon toArriv = GenerateMapRoutesFromCurrentToArv(item, airportMasterList, out MarkerErrorCode routeToArrvError);
                        if (routeToArrvError == MarkerErrorCode.VALID)
                        {
                            vm.mapMarkers.Add(toArriv);
                        }
                        else
                        {
                            errorList.Add("Partial map route missing for flight number: " + item.flight.number);
                        }

                    }
                }
                if (arrivalFlightsError == APIErrorCode.INVALIDFLIGHTDATA && departureFlightsError == APIErrorCode.INVALIDFLIGHTDATA)
                {
                    MessageBox.Show($"There are no flights found arriving or departing {selectedAirport.name}");
                }
                else
                {
                    vm.airport = selectedAirport;
                    vm.RecordCount = "Flight Count= " + vm.flightDataRecords.Count.ToString();
                    foreach (string error in errorList)
                    {
                        vm.errors.Add(error);
                    }
                    vm.ErrorLabel = $"Errors({errorList.Count()})";
                    vm.Center = new PointLatLng(double.Parse(selectedAirport.lat), double.Parse(selectedAirport.lon));
                    ViewFlightData viewFlightDataUI = new ViewFlightData(vm);
                    viewFlightDataUI.Show();
                    errorList.Clear();
                }

            }
            else
            {
                //If no airport has been selected...
                MessageBox.Show("No Airport Selection Has Been Made! Please select an airport or click 'Help' for more information.");              
            }

        }

        public void SetMapLatLong()
        {
            if (selectedAirport != null)
            {
                PointLatLng pointLatLng = new PointLatLng(double.Parse(selectedAirport.lat), double.Parse(selectedAirport.lon));
                lbl_AirportInfo ="Latitude: " + pointLatLng.Lat + " Longitude: " + pointLatLng.Lng;
                Center = pointLatLng;
                MapZoom =8;
            }
        }

        public GMapMarker GenerateMapAirplaneMarkers(FlightData flight, out MarkerErrorCode errorCode)
        {
            BitmapImage airplaneMarker = new BitmapImage();
            GMapMarker marker = new GMapMarker(new PointLatLng(0, 0));
            try
            {
                airplaneMarker.BeginInit();
                airplaneMarker.UriSource =  new Uri(@"\Resources\plane.png", UriKind.Relative);
                switch (flight.geography.direction)
                {
                    case double d when (d > 45 && d <= 135 ):
                        airplaneMarker.Rotation = Rotation.Rotate270;
                        break;
                    case double d when (d > 135 && d <= 225):
                        airplaneMarker.Rotation = Rotation.Rotate180;
                        break;
                    case double d when (d > 225 && d <= 315):
                        airplaneMarker.Rotation = Rotation.Rotate90;
                        break;
                    default:
                        break;
                }
                airplaneMarker.EndInit();
                marker.Position = new PointLatLng(flight.geography.latitude, flight.geography.longitude);
                Image image = new Image();            
                image.Source = airplaneMarker;
                //size image
                image.Height = 20;
                image.Width = 20;           
                //set the marker shape to the image
                marker.Shape = image;
                errorCode = MarkerErrorCode.VALID;
            }
            catch (Exception)
            {
                errorCode = MarkerErrorCode.INVALID;
            }

            return marker;
        }

        public GMapMarker GenerateDestinationMarkers(FlightData flight, List<Airport> airportList, out MarkerErrorCode error)
        {

            GMapMarker marker = new GMapMarker(new PointLatLng(0, 0));
            try
            {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(@"\Resources\destinationMarker.png", UriKind.Relative));  
            string arrivalLocationLat = airportList.Find(a => a.code == flight.arrival.icaoCode).lat;
            string arrivalLocationLong = airportList.Find(a => a.code == flight.arrival.icaoCode).lon;
            marker.Position = new PointLatLng(double.Parse(arrivalLocationLat), double.Parse(arrivalLocationLong));
            //size image
            image.Height = 15;
            image.Width = 15;
            //set the marker shape to the image
            marker.Shape = image;
            error = MarkerErrorCode.VALID;
            }
            catch (Exception)
            {
                error = MarkerErrorCode.INVALID;
            }

            return marker;
        }
        public GMapPolygon GenerateMapRoutesFromDep(FlightData flight, List<Airport> airportList, out MarkerErrorCode errorCode)
        {
            //Declare List for pointlatlang
            List<PointLatLng> pointlatlang = new List<PointLatLng>();
            try
            {
                string departureLocationLat = airportList.Find(a => a.code == flight.departure.icaoCode).lat;
                string departureLocationLong = airportList.Find(a => a.code == flight.departure.icaoCode).lon;

                string flightCurrentLocationLat = flight.geography.latitude.ToString();
                string flightCurrentLocationLong = flight.geography.longitude.ToString();

                pointlatlang.Add(new PointLatLng(double.Parse(departureLocationLat), double.Parse(departureLocationLong)));
                pointlatlang.Add(new PointLatLng(double.Parse(flightCurrentLocationLat), double.Parse(flightCurrentLocationLong)));

                errorCode = MarkerErrorCode.VALID;
            }
            catch (Exception)
            {
                errorCode = MarkerErrorCode.INVALID;
            }

            GMapPolygon polygon = new GMapPolygon(pointlatlang);

            return polygon;
        }
        public GMapPolygon GenerateMapRoutesFromCurrentToArv(FlightData flight, List<Airport> airportList, out MarkerErrorCode errorCode)
        {
            //Declare List for pointlatlang
            List<PointLatLng> pointlatlang = new List<PointLatLng>();
            try
            {
                string flightCurrentLocationLat = flight.geography.latitude.ToString();
                string flightCurrentLocationLong = flight.geography.longitude.ToString();
                string arrivalLocationLat = airportList.Find(a => a.code == flight.arrival.icaoCode).lat;
                string arrivalLocationLong = airportList.Find(a => a.code == flight.arrival.icaoCode).lon;

                pointlatlang.Add(new PointLatLng(double.Parse(flightCurrentLocationLat), double.Parse(flightCurrentLocationLong)));
                pointlatlang.Add(new PointLatLng(double.Parse(arrivalLocationLat), double.Parse(arrivalLocationLong)));
                errorCode = MarkerErrorCode.VALID;
            }
            catch (Exception)
            {
                errorCode = MarkerErrorCode.INVALID;
            }

            GMapPolygon polygon = new GMapPolygon(pointlatlang);

            return polygon;
        }
        public void AddToFavorites()
        {

            if (selectedAirport != null)
            {

            {
                MessageBox.Show("Item cannot be added at this time");
            }
            }

        }
        public void OpenHelp()
        {
            Help help = new Help();
            help.Show();
        }
    }
}
