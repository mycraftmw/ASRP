using System.Collections;
namespace ASRP
{
    class SubwayMap
    {
        private ArrayList stations;
        private ArrayList connections;
        public SubwayMap()
        {
            stations = new ArrayList();
            connections = new ArrayList();
        }

        public bool HasStation(string stationName)
        {
            return stations.Contains(new Station(stationName));
        }
        public void AddStation(string stationName)
        {
            if (!HasStation(stationName))
            {
                Station station = new Station(stationName);
                stations.Add(station);
            }
        }
        public void AddConnection(string station1Name, string station2Name, string lineName)
        {
            if (HasStation(station1Name) && HasStation(station2Name))
            {
                Station station1 = new Station(station1Name);
                Station station2 = new Station(station2Name);
                Connection connection = new Connection(station1, station2, lineName);
                connections.Add(connection);
            }
            else
            {
                throw new System.ArgumentException("Invalid Connection!");
            }
        }
    }
}
