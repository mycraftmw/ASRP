using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections;
using Core;

namespace AdvancedSubwayRoutePlanning
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<DisplayRouteUnit> displayRouteUnitList;
        private ObservableCollection<string> displayStationsName = new ObservableCollection<string>();
        private SubwayMap subwayMap;
        private List<Connection> curRoute;
        private Station startStation;
        private Station endStation;

        public MainWindow()
        {
            InitializeComponent();
            this.displayRouteUnitList = ((App)App.Current).DisplayRouteUnitList;
            this.listView_Route.ItemsSource = displayRouteUnitList;
            this.comboBox_StartStation.ItemsSource = displayStationsName;
            this.comboBox_EndStation.ItemsSource = displayStationsName;
            this.subwayMap = ((App)App.Current).BackgroundCore.SubwayMap;
            this.curRoute = ((App)App.Current).CurRoute;
            this.startStation = ((App)App.Current).StartStation;
            this.endStation = ((App)App.Current).EndStation;
            ((App)App.Current).IsShortestPlaning = (bool)radioButton_Shortest.IsChecked;

            ((App)App.Current).BackgroundCore.SelectFunction(this, ((App)App.Current).Args);
            foreach (Station station in subwayMap.Stations)
            {
                displayStationsName.Add(station.Name);
            }
        }

        private void searchRoute(object sender, RoutedEventArgs e)
        {
            string mode;

            if ((bool)radioButton_Shortest.IsChecked)
                mode = "-b";
            else
                mode = "-c";

            startStation = subwayMap.GetStation(comboBox_StartStation.Text);
            endStation = subwayMap.GetStation(comboBox_EndStation.Text);
            curRoute = subwayMap.GetDirections(startStation.Name, endStation.Name, mode);

            displayRouteUnitList.Clear();

            displayRouteUnitList.Add(new DisplayRouteUnit(curRoute[0].BeginStation.Name, curRoute[0].LineName));
            foreach (Connection connection in curRoute)
            {
                displayRouteUnitList.Add(new DisplayRouteUnit(connection.EndStation.Name, connection.LineName));
            }

            subwayGraph.InvalidateVisual();
        }

        private void comboBox_StartStation_GotMouseCapture(object sender, MouseEventArgs e)
        {
            this.comboBox_StartStation.IsDropDownOpen = true;
        }

        private void comboBox_EndStation_GotMouseCapture(object sender, MouseEventArgs e)
        {
            this.comboBox_EndStation.IsDropDownOpen = true;
        }

        private void comboBox_StartStation_GotFocus(object sender, RoutedEventArgs e)
        {
            this.comboBox_StartStation.IsDropDownOpen = true;
        }

        private void comboBox_EndStation_GotFocus(object sender, RoutedEventArgs e)
        {
            this.comboBox_EndStation.IsDropDownOpen = true;
        }
    }

    public class Cities : ObservableCollection<string>
    {
        public Cities()
        {
            Add("北京");
        }
    }

    public class DisplayRouteUnit
    {
        public string StationName { get; }
        public string LineName { get; }

        public DisplayRouteUnit(string stationName, string lineName)
        {
            this.StationName = stationName;
            this.LineName = lineName;
        }
    }

}
