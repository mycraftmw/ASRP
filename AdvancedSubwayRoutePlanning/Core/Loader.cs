using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Core
{
    class Loader
    {
        public SubwayMap SubwayMap { get; private set; }
        public Loader()
        {
            SubwayMap = new SubwayMap();
        }
        public SubwayMap LoadFromXMLFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("文件不存在！");
            }
            if (new FileInfo(path).Extension.ToLowerInvariant() != ".xml")
            {
                throw new FormatException("文件类型错误！");
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList lines = doc.DocumentElement.ChildNodes;

            foreach (XmlNode eachline in lines)
            {
                string lineName = eachline.Attributes.GetNamedItem("lid").InnerXml;
                string lineColor = eachline.Attributes.GetNamedItem("lc").InnerXml;
                SubwayMap.AddSubwayLine(lineName, lineColor);
                XmlNodeList stations = eachline.SelectSingleNode("stations").ChildNodes;
                string lastName = "";
                foreach (XmlNode sta in stations)
                {
                    XmlAttributeCollection a = sta.Attributes;
                    bool b = Convert.ToBoolean(a.GetNamedItem("iu").InnerXml);
                    if (!b) continue;
                    string stationName= a.GetNamedItem("sid").InnerXml;
                    double x = Convert.ToDouble(a.GetNamedItem("x").InnerXml);
                    double y = Convert.ToDouble(a.GetNamedItem("y").InnerXml);
                    bool isTransfer = Convert.ToBoolean(a.GetNamedItem("ex").InnerXml);
                    SubwayMap.AddStation(stationName, x, y, isTransfer);
                    if (lastName != "")
                    {
                        string c = lastName.CompareTo(stationName)>0?stationName:lastName;
                        string d = lastName.CompareTo(stationName)>0?lastName:stationName;
                        SubwayMap.AddConnection(c, d, lineName, SubwayMap.CountConnection(c, d));
                        SubwayMap.AddConnection(d, c, lineName, -1);
                        lastName = stationName;
                    }
                }
                if (eachline.Attributes.GetNamedItem("loop").InnerXml == "true")
                {
                    string fs = stations.Item(0).Attributes.GetNamedItem("sid").InnerXml;
                    string c = lastName.CompareTo(fs)>0?fs:lastName;
                    string d = lastName.CompareTo(fs)>0?lastName:fs;
                    SubwayMap.AddConnection(c, d, lineName, SubwayMap.CountConnection(c, d));
                    SubwayMap.AddConnection(c, d, lineName, -1);
                }
            }
            return this.SubwayMap;
        }
    }
}
