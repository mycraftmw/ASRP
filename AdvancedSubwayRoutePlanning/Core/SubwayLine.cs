using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SubwayLine
    {
        public string Name { get; }
        public string Color { get; }
        public SubwayLine(string name, string color)
        {
            this.Name = Name;
            this.Color = color;
        }
    }
}
