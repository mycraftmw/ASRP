namespace Core
{
    public class Connection
    {
        public Station BeginStation { get; }
        public Station EndStation { get; }
        public string LineName { get; }
        public int Type { get; }
        public Connection(Station beginStationName, Station endStationName, string lineName,int type)
        {
            this.BeginStation = beginStationName;
            this.EndStation = endStationName;
            this.LineName = lineName;
            this.Type = type;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return this.LineName == ((Connection)obj).LineName && this.BeginStation.Equals(((Connection)obj).BeginStation) && this.EndStation.Equals(((Connection)obj).EndStation);
        }
        public override int GetHashCode()
        {
            return (LineName + "|" + BeginStation + "|" + EndStation).GetHashCode();
        }
    }
}
