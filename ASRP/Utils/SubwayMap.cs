using System.Collections.Generic;

namespace ASRP
{
    class SubwayMap
    {
        public List<Station> Stations { get; }
        public List<Connection> Connections { get; }
        public SubwayMap()
        {
            Stations = new List<Station>();
            Connections = new List<Connection>();
        }

        public bool HasStation(string stationName)
        {
            return Stations.Exists(which => which.Name == stationName);
        }
        public void AddStation(string stationName, double x, double y)
        {
            if (!HasStation(stationName))
            {
                Station station = new Station(stationName, x, y);
                Stations.Add(station);
            }
        }
        public void AddConnection(string station1Name, string station2Name, string lineName)
        {
            if (HasStation(station1Name) && HasStation(station2Name))
            {
                Station station1 = Stations.Find(which => which.Name.Equals(station1Name));
                Station station2 = Stations.Find(which => which.Name.Equals(station2Name));
                Connection connection = new Connection(station1, station2, lineName);
                Connections.Add(connection);
            }
            else
            {
                throw new System.ArgumentException("Invalid Connection!");
            }
        }
    }
}
