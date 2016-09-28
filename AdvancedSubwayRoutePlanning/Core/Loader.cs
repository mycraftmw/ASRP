using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Core
{
    class Loader
    {
        public SubwayMap SubwayMap { get; private set; }
        public List<string> CityList { get; }
        public Loader()
        {
            CityList = new List<string>();
        }
        public List<string> LoadCityList(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("文件不存在！");
            }
            if (new FileInfo(path).Extension.ToLowerInvariant() != ".xml")
            {
                throw new FormatException("文件类型错误！");
            }
            CityList.Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList cities = doc.DocumentElement.ChildNodes;
            foreach (XmlNode city in cities)
            {
                CityList.Add(city.Attributes.GetNamedItem("name").InnerXml);
            }

            return CityList;
        }
        public SubwayMap LoadSubwayMap(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("文件不存在！");
            }
            if (new FileInfo(path).Extension.ToLowerInvariant() != ".xml")
            {
                throw new FormatException("文件类型错误！");
            }
            SubwayMap = new SubwayMap();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList lines = doc.DocumentElement.ChildNodes;

            foreach (XmlNode eachline in lines)
            {
                string lineName = eachline.Attributes.GetNamedItem("lid").InnerXml;
                string lineColor = eachline.Attributes.GetNamedItem("lc").InnerXml.Remove(0,2);
                SubwayMap.AddSubwayLine(lineName, "#" + lineColor);
                XmlNodeList stations = eachline.ChildNodes;
                string lastName = "";
                foreach (XmlNode sta in stations)
                {
                    XmlAttributeCollection a = sta.Attributes;
                    bool b;
                    if (a.GetNamedItem("iu").InnerXml == null || a.GetNamedItem("iu").InnerXml == "" || a.GetNamedItem("iu").InnerXml == "false") b = false;
                    else b = true;
                    if (!b) continue;
                    string stationName= a.GetNamedItem("sid").InnerXml;
                    double x = Convert.ToDouble(a.GetNamedItem("x").InnerXml);
                    double y = Convert.ToDouble(a.GetNamedItem("y").InnerXml);
                    bool isTransfer = Convert.ToBoolean(a.GetNamedItem("ex").InnerXml);
                    SubwayMap.AddStation(stationName, x, y, isTransfer);
                    if (lastName != "")
                    {
                        CheckDoubleLine(stationName, lastName, lineName);
                    }
                    lastName = stationName;
                }
                if (eachline.Attributes.GetNamedItem("loop").InnerXml == "true")
                {
                    CheckDoubleLine(lastName, stations[0].Attributes.GetNamedItem("sid").InnerXml, lineName);
                }
            }
            return this.SubwayMap;
        }

        private void CheckDoubleLine(string sa, string sb, string lineName)
        {
            string a=sa.CompareTo(sb)>0?sa:sb;
            string b=a==sa?sb:sa;
            if (SubwayMap.CountConnection(a, b) > 0)
            {
                SubwayMap.Connections.Find(x => x.BeginStation.Name == a && x.EndStation.Name == b).Type = 1;
                SubwayMap.AddConnection(a, b, lineName, 2);
                SubwayMap.AddConnection(b, a, lineName, -1);
            }
            else
            {
                SubwayMap.AddConnection(a, b, lineName, 0);
                SubwayMap.AddConnection(b, a, lineName, -1);
            }

        }
    }
}
