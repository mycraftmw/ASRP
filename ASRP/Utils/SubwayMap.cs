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
            int TRANSFERCOST =
            mode == "-b" ? 0 :
            mode == "-c" ? 10000 :
            -1;
            if (TRANSFERCOST < 0)
            {
                throw new AggregateException("模式错误");
            }
            if (!HasStation(beginStationName) || !HasStation(endStationName))
            {
                throw new ArgumentException("站点不存在！");
            }
            if (beginStationName == endStationName)
            {
                return new List<Connection>();
            }

            Station beginStation = Stations.Find(x => x.Name == beginStationName);
            Station endStation = Stations.Find(x => x.Name == endStationName);
            Queue<Station> queue = new Queue<Station>();
            Hashtable routeMap = new Hashtable();
            routeMap.Add(beginStation, new List<List<Connection>>());
            Connection initConnection = new Connection(Stations.Find(x => x.Id == 0), Stations.Find(x => x.Id == 0), "*");
            List<Connection> initRoute = new List<Connection>();
            initRoute.Add(initConnection);
            ((List<List<Connection>>)routeMap[beginStation]).Add(initRoute);

            queue.Enqueue(beginStation);
            while (queue.Count != 0)
            {
                Station now = queue.Dequeue();
                List<List<Connection>> routeList = (List<List<Connection>>)routeMap[now];
                ((List<Connection>)Map[now]).ForEach(line =>
                {
                    routeList.ForEach(route =>
                    {
                        int cost = route.Count + 1 + (route.Count != 1 && route.FindLast(x => true).LineName != line.LineName ? TRANSFERCOST : 0);
                        if (!routeMap.Contains(line.EndStation))
                        {
                            routeMap.Add(line.EndStation, new List<List<Connection>>());
                        }
                        List<List<Connection>> endStationRouteList = (List<List<Connection>>)routeMap[line.EndStation];
                        int judgeOld = endStationRouteList.Count == 0 ? int.MaxValue : endStationRouteList[0].Count;
                        int judgeCost = cost;
                        if (TRANSFERCOST > 0)
                        {
                            judgeCost /= TRANSFERCOST;
                            judgeOld /= TRANSFERCOST;
                        }
                        // System.Console.WriteLine(now + " " + line.LineName + " " + line.EndStation + " " + (int)dis[now] + " " + cost);
                        if (judgeOld > judgeCost)
                        {
                            // System.Console.WriteLine("update " + line.EndStation + " with " + (int)dis[line.EndStation]);
                            endStationRouteList = new List<List<Connection>>();
                            List<Connection> tRoute = new List<Connection>(route);
                            tRoute.Add(line);
                            endStationRouteList.Add(tRoute);
                            if (!queue.Contains(line.EndStation))
                            {
                                queue.Enqueue(line.EndStation);
                            }
                        }
                        else if (judgeOld == judgeCost)
                        {
                            List<Connection> tRoute = new List<Connection>(route);
                            tRoute.Add(line);
                            endStationRouteList.Add(tRoute);
                        }
                    });
                });
            }
            if (!routeMap.Contains(endStation))
            {
                throw new ArgumentException("站点不连通");
            }
            List<List<Connection>> tRouteList = (List<List<Connection>>)routeMap[endStation];
            System.Console.WriteLine(tRouteList.Count);
            List<Connection> ansList = new List<Connection>();
            tRouteList.ForEach(x =>
            {
                ansList = ansList.Count == 0 ? x : ansList.Count > x.Count ? x : ansList;
            });
            return ansList;
        }
        public List<Station> GetLine(string lineName)
        {
            List<Station> line = new List<Station>();
            Connections.FindAll(x => x.LineName == lineName).ForEach(x =>
            {
                if (!line.Contains(x.BeginStation))
                {
                    line.Add(x.BeginStation);
                }
            });
            return line;
        }
    }
}
