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
        public SubwayMap subwayMap;

        public MainWindow()
        {
            InitializeComponent();
            this.subwayMap = BackgroundCore.GetBackgroundCore().SubwayMap;
            this.displayRouteUnitList = ((App)App.Current).DisplayRouteUnitList;
            this.listView_Route.ItemsSource = displayRouteUnitList;
            this.comboBox_StartStation.ItemsSource = displayStationsName;
            this.comboBox_EndStation.ItemsSource = displayStationsName;
            ((App)App.Current).IsShortestPlaning = (bool)radioButton_Shortest.IsChecked;

            BackgroundCore.GetBackgroundCore().SelectFunction(this, ((App)App.Current).Args);
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

            subwayMap.StartStation = subwayMap.GetStation(comboBox_StartStation.Text);
            subwayMap.EndStation = subwayMap.GetStation(comboBox_EndStation.Text);
            subwayMap.CurRoute = subwayMap.GetDirections(comboBox_StartStation.Text, comboBox_EndStation.Text, mode);

            displayRouteUnitList.Clear();

            displayRouteUnitList.Add(new DisplayRouteUnit(subwayMap.CurRoute[0].BeginStation.Name, subwayMap.CurRoute[0].LineName));
            foreach (Connection connection in (subwayMap.CurRoute))
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

        private void comboBox_Cities_DropDownOpened(object sender, EventArgs e)
        {
            ((Cities)this.stackPanel_FunctionArea.Resources["cities"]).Clear();
            foreach (string city in BackgroundCore.GetBackgroundCore().CityList)
                ((Cities)this.stackPanel_FunctionArea.Resources["cities"]).Add(city);
        }

        private void comboBox_Cities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BackgroundCore.GetBackgroundCore().RefreshMap(this.comboBox_Cities.Text);
        }
    }

    public class Cities : ObservableCollection<string>
    {
        public Cities()
        {
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
