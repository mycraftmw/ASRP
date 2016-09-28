using System;
using System.Collections.Generic;
using AdvancedSubwayRoutePlanning;
namespace Core
{
    public class BackgroundCore
    {
        private Loader Loader = new Loader();
        public Printer Printer = new Printer(System.Console.OpenStandardOutput());
        public SubwayMap SubwayMap { get; }
        private List<Connection> route = null;

        public BackgroundCore()
        {
            SubwayMap = Loader.LoadFromXMLFile("subway.xml");
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
