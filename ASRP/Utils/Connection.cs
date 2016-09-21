namespace ASRP
{
    class Connection
    {
        public Station Station1 { get; }
        public Station Station2 { get; }
        public string LineName { get; }
        public Connection(Station station1, Station station2, string lineName)
        {
            this.Station1 = station1;
            this.Station2 = station2;
            this.LineName = lineName;
        }
    }
}
