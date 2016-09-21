using System.Collections;
namespace ASRP
{
    class SubwayMap
    {
        public ArrayList Stations { get; }
        public ArrayList Connections { get; }
        public SubwayMap()
        {
            Stations = new ArrayList();
            Connections = new ArrayList();
        }

        public bool HasStation(string stationName)
        {
            return Stations.Contains(new Station(stationName));
        }
        public void AddStation(string stationName)
        {
            if (!HasStation(stationName))
            {
                Station station = new Station(stationName);
                Stations.Add(station);
            }
        }
        public void AddConnection(string station1Name, string station2Name, string lineName)
        {
            if (HasStation(station1Name) && HasStation(station2Name))
            {
                Station station1 = new Station(station1Name);
                Station station2 = new Station(station2Name);
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
