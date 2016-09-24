using System.Collections.Generic;
using System.Collections;
using System;

namespace ASRP
{
    class SubwayMap
    {
        public List<Station> Stations { get; }
        public List<Connection> Connections { get; }
        private Hashtable Map;
        public SubwayMap()
        {
            this.Stations = new List<Station>();
            this.Connections = new List<Connection>();
            this.Map = new Hashtable();
        }

        public bool HasStation(string stationName)
        {
            return Stations.Exists(x => x.Name == stationName && stationName != "*");
        }
        public void AddStation(string stationName, double x, double y)
        {
            if (!HasStation(stationName))
            {
                Station station = new Station(stationName, x, y);
                Stations.Add(station);
            }
        }
        public bool HasConnection(string beginStationName, string endStationName, string lineName)
        {
            return Connections.Exists(x => x.BeginStation.Name == beginStationName && x.EndStation.Name == endStationName && x.LineName == lineName);
        }
        public void AddConnection(string beginStationName, string endStationName, string lineName)
        {
            if (HasStation(beginStationName) && HasStation(endStationName))
            {
                if (!HasConnection(beginStationName, endStationName, lineName))
                {
                    Station beginStation = Stations.Find(x => x.Name == beginStationName);
                    Station endStation = Stations.Find(x => x.Name == endStationName);
                    Connection connection = new Connection(beginStation, endStation, lineName);
                    Connections.Add(connection);
                    if (Map.Contains(beginStation))
                    {
                        List<Connection> cons = (List<Connection>)Map[beginStation];
                        cons.Add(connection);
                    }
                    else
                    {
                        List<Connection> cons = new List<Connection>();
                        cons.Add(connection);
                        Map.Add(beginStation, cons);
                    }

                }
            }
            else
            {
                throw new System.ArgumentException("Invalid Connection!");
            }
        }
        public List<Connection> GetDirections(string beginStationName, string endStationName, string mode)
        {
            if (!HasStation(beginStationName) || !HasStation(endStationName)) throw new ArgumentException("站点不存在！");
            int transferCost =
            mode == "-b" ? 0 :
            mode == "-c" ? 10000 :
            -1;
            if (transferCost < 0) throw new AggregateException("模式错误");

            if (beginStationName == endStationName) return new List<Connection>();

            Station beginStation = Stations.Find(x => x.Name == beginStationName);
            Station endStation = Stations.Find(x => x.Name == endStationName);
            Queue<Station> queue = new Queue<Station>();
            Hashtable routeMap = new Hashtable();
            List<List<Connection>> initRouteList = new List<List<Connection>>();
            List<Connection> initRoute = new List<Connection>();
            Connection initConnection = new Connection(Stations.Find(x => x.Id == 0), Stations.Find(x => x.Id == 0), "*");
            initRoute.Add(initConnection);
            initRouteList.Add(initRoute);
            routeMap.Add(beginStation, initRouteList);
            queue.Enqueue(beginStation);
            List<List<Connection>> tRouteList = null;
            List<Connection> tRoute = null;
            while (queue.Count != 0)
            {
                Station now = queue.Dequeue();
                List<List<Connection>> routeList = (List<List<Connection>>)routeMap[now];
                ((List<Connection>)Map[now]).ForEach(line =>
                {
                    routeList.ForEach(route =>
                    {
                        if (!routeMap.Contains(line.EndStation)) routeMap.Add(line.EndStation, new List<List<Connection>>());
                        tRouteList = (List<List<Connection>>)routeMap[line.EndStation];
                        int alreadyCost = CountCost(mode, tRouteList.Find(x => true));
                        int nowCost = CountCost(mode, route) + (mode == "-b" ? 1 : mode == "-c" && route.Count != 1 && route.FindLast(x => true).LineName != line.LineName ? 1 : 0);
                        if (nowCost < alreadyCost)
                        {
                            tRouteList = new List<List<Connection>>();
                            tRoute = new List<Connection>(route);
                            tRoute.Add(line);
                            tRouteList.Add(tRoute);
                            routeMap[line.EndStation] = tRouteList;
                            if (!queue.Contains(line.EndStation)) queue.Enqueue(line.EndStation);
                        }
                        else if (nowCost == alreadyCost)
                        {
                            tRoute = new List<Connection>(route);
                            tRoute.Add(line);
                            int k = CountCost(mode, tRoute);
                            if (!tRouteList.Exists(x => x.FindLast(t => true).LineName == line.LineName && CountCost(mode, x) <= k))
                            {
                                tRouteList.Add(tRoute);
                                if (!queue.Contains(line.EndStation)) queue.Enqueue(line.EndStation);
                            }
                        }
                    });
                });
            }

            if (!routeMap.Contains(endStation)) throw new ArgumentException("站点不连通");

            tRouteList = (List<List<Connection>>)routeMap[endStation];
            tRoute = tRouteList[0];
            tRouteList.ForEach(x =>
            {
                if (tRoute.Count > x.Count) tRoute = x;
                else if (tRoute.Count == x.Count) tRoute = CountCost("-c", tRoute) > CountCost("-c", x) ? x : tRoute;
            });
            return tRoute;
        }

        private int CountCost(string mode, List<Connection> list)
        {
            if (list == null || list.Count == 0) return int.MaxValue;
            if (mode == "-b")
            {
                return list.Count;
            }
            else if (mode == "-c")
            {
                if (list.Count <= 2) return 0;
                int a = 0;
                string last = "*";
                list.ForEach(x => { if (x.LineName != last) { a++; last = x.LineName; } });
                return a;
            }
            else
                return -1;
            throw new NotImplementedException();
        }
        public List<Station> GetLine(string lineName)
        {
            List<Station> line = new List<Station>();
            Connections.FindAll(x => x.LineName == lineName).ForEach(x =>
            {
                if (!line.Contains(x.BeginStation)) line.Add(x.BeginStation);
            });
            return line;
        }
    }
}
