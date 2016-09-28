using System;
using System.Collections.Generic;
using System.Xml;
using AdvancedSubwayRoutePlanning;

namespace Core
{
    class BackgroundCore
    {
        public SubwayMap SubwayMap { get; private set; }
        public Printer Printer { get; }
        private Loader loader;
        private List<Connection> route;
        private static BackgroundCore backgroundCore;
        private BackgroundCore()
        {
            loader = new Loader();
            SubwayMap = loader.LoadFromXMLFile(@"map\beijing-subway.xml");
            Printer = new Printer(System.Console.OpenStandardOutput());
        }
        public static BackgroundCore GetBackgroundCore()
        {
            if (backgroundCore == null) backgroundCore = new BackgroundCore();
            return backgroundCore;
        }
        public void RefreshMap(string CityName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"map\subway-list.xml");
            XmlNodeList cities = doc.DocumentElement.ChildNodes;
            SubwayMap = null;
            foreach (XmlNode city in cities)
                if (city.Attributes.GetNamedItem("name").InnerXml == CityName)
                    SubwayMap = loader.LoadFromXMLFile(city.Attributes.GetNamedItem("src").InnerXml);
            if (SubwayMap == null)
                throw new ArgumentException("The City does not exist!");
        }
        public void SelectFunction(MainWindow mainWindow, string[] args)
        {
            try
            {
                mainWindow.Hide();

                if (args.Length == 0)
                {
                    while (true)
                    {
                        string input = System.Console.ReadLine();
                        if (input != "exit")
                        {
                            Printer.PrintSubwayLine(SubwayMap.GetLine(input));
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else if (args.Length == 1 && args[0] == "-g")
                {
                    mainWindow.Show();
                }
                else if (args.Length == 3)
                {
                    route = SubwayMap.GetDirections(args[1], args[2], args[0]);
                    Printer.PrintDirections(route);
                }
                else
                {
                    Printer.WriteLine("输入格式错误");
                }

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                mainWindow.Close();
                return;
            }
        }
    }
}
