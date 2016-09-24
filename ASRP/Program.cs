using System;
using System.Collections.Generic;
namespace ASRP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Loader Loader = new Loader();
                Printer Printer = new Printer(System.Console.OpenStandardOutput());
                SubwayMap SubwayMap = Loader.LoadFromXMLFile("subway.xml");
                List<Connection> route = null;

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
                    throw new NotImplementedException("图形化界面未完成");
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
                return;
            }

        }
    }
}
