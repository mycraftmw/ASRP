namespace ASRP
{
    class Station
    {
        private static int ID = 0;
        public int Id { get; }
        public string Name { get; }
        public double X { get; }
        public double Y { get; }
        public Station(string name, double x, double y)
        {
            this.Name = name;
            this.Id = ++ID;
            this.X = x;
            this.Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Station o = (Station)obj;
            return o.Name == this.Name && o.X == this.X && o.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
