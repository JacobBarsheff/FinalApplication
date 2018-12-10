using FlightTracker.Models;
using FlightTracker.ViewModels;
using GMap.NET;
using GMap.NET.MapProviders;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlightTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
  
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AirportSearchViewModel();

        }
      
        private void GMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            map.MapProvider = GMapProviders.GoogleMap;
           // PointLatLng center = new PointLatLng(39.9, -84.4);
           // map.CenterPosition = center;
           // map.Zoom = 4;
            //map.MinZoom = 1;
            //map.MaxZoom = 10;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {

            }
            btn_submit.Visibility = Visibility.Visible;
        }

    }
}
