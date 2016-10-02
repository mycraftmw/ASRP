namespace Core
{
    public class SubwayLine
    {
        public string Name { get; }
        public string Label { get; }
        public string Color { get; }
        public SubwayLine(string name, string label, string color)
        {
            this.Name = name;
            this.Label = label;
            this.Color = color;
        }
    }
}
