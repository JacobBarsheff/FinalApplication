using FlightTracker.ViewModels;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlightTracker
{
    /// <summary>
    /// Interaction logic for ViewFlightData.xaml
    /// </summary>
    public partial class ViewFlightData : Window
    {
        public ViewFlightData(ViewFlightDataViewModel vm)
        {
            //ViewFlightDataViewModel vm = new ViewFlightDataViewModel();
            InitializeComponent();
            DataContext = vm;
            foreach (GMapMarker item in vm.mapMarkers)
            {
                if (item.GetType() == typeof(GMapPolygon))
                {  
                    map.RegenerateShape((GMapPolygon)item);
                    (item.Shape as Path).Stroke = Brushes.Green;
                    (item.Shape as Path).StrokeThickness = 1.5;
                    (item.Shape as Path).Effect = null;
                    map.Markers.Add(item);
                }
                else
                {
                    map.Markers.Add(item);
                }
                //map.Markers.Add(item);
            }
            foreach (GMapMarker item in vm.flightsOnRouteToThisAirport)
            {
                if (item.GetType() == typeof(GMapPolygon))
                {
                    map.RegenerateShape((GMapPolygon)item);
                    (item.Shape as Path).Stroke = Brushes.Red;
                    (item.Shape as Path).StrokeThickness = 1.5;
                    (item.Shape as Path).Effect = null;
                    map.Markers.Add(item);
                }
                else
                {
                    map.Markers.Add(item);
                }
            }
            //foreach (GMapPolygon item in vm.flightRouteShapes)
            //{
            //    map.RegenerateShape(item);
            //    (item.Shape as Path).Stroke = Brushes.HotPink;
            //    (item.Shape as Path).StrokeThickness = .1;
            //    map.Markers.Add(item);
            //}
        }

        private void map_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
