namespace ASRP
{
    class Station
    {
        private static int ID = 0;
        public int Id { get; }
        public string Name { get; }
        public Station(string name)
        {
            this.Name = name;
            this.Id = ++ID;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return string.Equals(((Station)obj).Name, this.Name, System.StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.Name.ToLower().GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
