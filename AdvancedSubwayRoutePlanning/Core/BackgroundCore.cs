using System;
using System.Collections.Generic;
using System.Collections;
using AdvancedSubwayRoutePlanning;

namespace Core
{
    class BackgroundCore
    {
        public SubwayMap SubwayMap { get; private set; }
        public Printer Printer { get; }
        public List<string> CityList { get; }

        private Hashtable CityMap;
        private Loader loader;
        private List<Connection> route;
        private static BackgroundCore backgroundCore;

        private BackgroundCore()
        {
            loader = new Loader();
            CityMap = loader.LoadCityMap(@"map/subway-list.xml");
            CityList = new List<string>();
            foreach (string k in CityMap.Keys)
                CityList.Add(Convert.ToString(CityMap[k]));
            SubwayMap = loader.LoadSubwayMap(@"map/beijing-subway.xml");
            Printer = new Printer(System.Console.OpenStandardOutput());
        }

        public static BackgroundCore GetBackgroundCore()
        {
            if (backgroundCore == null) backgroundCore = new BackgroundCore();
            return backgroundCore;
        }

        public void RefreshMap(string CityName)
        {
            SubwayMap = null;
            if (!CityList.Contains(CityName))
            {
                throw new ArgumentException("The city does not exist.");
            }
            SubwayMap = loader.LoadSubwayMap(@"map/" + (string)CityMap[CityName]);
        }

        public void SelectFunction(MainWindow mainWindow, string[] args)
        {
            //try
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
                    SubwayMap.SetStartStation(args[1]);
                    SubwayMap.SetEndStation(args[2]);
                    route = SubwayMap.GetDirections(args[0]);
                    Printer.PrintDirections(route);
                    mainWindow.Close();
                    return;
                }
                else
                {
                    Printer.WriteLine("输入格式错误");
                    mainWindow.Close();
                    return;
                }

            }
            /*catch (System.Exception e)
            {
                Console.WriteLine(e);
                mainWindow.Close();
                return;
            }*/
        }
    }
}
