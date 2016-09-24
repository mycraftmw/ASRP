namespace ASRP
{
	class Connection
	{
		public Station BeginStation { get; }
		public Station EndStation { get; }
		public string LineName { get; }
		public Connection(Station beginStation, Station endStation, string lineName)
		{
			this.BeginStation = beginStation;
			this.EndStation = endStation;
			this.LineName = lineName;
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
