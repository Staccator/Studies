using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab7
{
    public class BestCitiesSolver : MarshalByRefObject
    {

        public static Edge[] ListToTab(List<int> tab)
        {
            Edge[] res = new Edge[tab.Count - 1];
            for (int i = 0; i < tab.Count-1; i++)
            {
                res[i] = new Edge(tab[i], tab[i + 1]);
            }
            return res;
        }
        public static List<int> TabToList(Edge[] tab)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < tab.Length; i++)
            {
                res.Add(tab[i].From);
            }
            res.Add(tab.Last().To);
            return res;
        }

        public static void ReverseTab(List<int> tab)
        {
            int n = tab.Count;
            for(int i = 0; i < tab.Count/2; i++)
            {
                int a = tab[i];
                tab[i] = tab[n - 1 - i];
                tab[n - 1 - i] = a;
            }
        }

        public (int c1, int c2, int? bypass, double time, Edge[] path)? FindBestCitiesPair(Graph times, double[] passThroughCityTimes, int[] nominatedCities, bool buildBypass)
        {
            Graph graph = times.IsolatedVerticesGraph();

            int n = graph.VerticesCount;
            
            for (int i = 0; i < times.VerticesCount; i++)
            {
                foreach (var e in times.OutEdges(i))
                {
                    if (e.To > e.From)
                    {
                        graph.AddEdge(new Edge(e.From, e.To, e.Weight + passThroughCityTimes[e.To]/2 + passThroughCityTimes[e.From] / 2));
                    }
                }
            }

            double bestlength = double.MaxValue;
            int bestfrom = -1; int bestto = -1;
            Edge [] bestpath = null;
            //List<int>[,] tracks = null;
            Dictionary<int, double[]> Path_Lenghts = null;
            Dictionary<int ,PathsInfo[]> pathsInfos = new Dictionary<int, PathsInfo[]>();

            if (buildBypass)
            {
                //tracks = new List<int>[n, n];
                Path_Lenghts = new Dictionary<int, double[]>();
                for(int i = 0; i < nominatedCities.Length; i++)
                {
                    double[] tab = new double[n];
                    for (int j = 0; j < n; j++)
                        tab[j] = double.MaxValue;
                    Path_Lenghts.Add(nominatedCities[i], tab);
                }
                
            }
            bool[] czy_oznaczone = new bool[n];
            for (int i = 0; i < nominatedCities.Length; i++)
            {
                czy_oznaczone[nominatedCities[i]] = true;
            }
            var czy_sprawdzone = new bool[n];
            

            Edge[] path;
            double suma = 0;

            for (int i = 0; i < nominatedCities.Length; i++)
            {
                int s = nominatedCities[i];
                czy_sprawdzone[s] = true;
                graph.DijkstraShortestPaths(s, out PathsInfo[] pathinfo);
                pathsInfos[s] = pathinfo;
                for (int t = 0; t < n; t++)
                {
                    if (s == t) continue;
                    suma = pathinfo[t].Dist;
                    if (double.IsNaN(suma)) continue;
                    
                    suma -= (passThroughCityTimes[s]) / 2 + (passThroughCityTimes[t] / 2);
                    if (czy_oznaczone[t])
                    {
                        if (czy_sprawdzone[t]) continue;
                            if (suma < bestlength)
                            {
                                bestlength = suma;
                                bestfrom = s;
                                bestto = t;
                            }
                        
                    }
                    else if (buildBypass)
                    {
                            if (suma < Path_Lenghts[s][t])
                            {
                            
                               Path_Lenghts[s][t] = suma;
                            }
                        
                    }
                }
            }
            
            if (bestlength == double.MaxValue)
            {
                return null;
            }
            else
            {
                if (!buildBypass)
                {
                    bestpath = PathsInfo.ConstructPath(bestfrom, bestto, pathsInfos[bestfrom]);
                    //pathsInfos[bestfrom]
                    
                    return (bestfrom, bestto, null, bestlength, bestpath.ToArray());
                }
                else // check for better track
                {
                    double maxbypass = bestlength;
                    int autostrada = 0;
                    for (int i = 0; i < nominatedCities.Length; i++)
                    {
                        int s = nominatedCities[i];
                        for (int j = 0; j < nominatedCities.Length; j++)
                        {
                            int t = nominatedCities[j];
                            if (s == t) continue;
                            for(int k = 0; k < n; k++)
                            {
                                if (nominatedCities.Contains(k)) continue;
                                if(Path_Lenghts[s][k] + Path_Lenghts[t][k] < maxbypass)
                                {
                                    maxbypass = Path_Lenghts[s][k] + Path_Lenghts[t][k];
                                    autostrada = k;
                                    bestfrom = s; bestto = t;

                                }
                            }
                        }
                    }
                    if (maxbypass < bestlength)
                    {
                        int k = autostrada;
                        int s = bestfrom, t = bestto;
                        var track = TabToList(PathsInfo.ConstructPath(t, k, pathsInfos[t]));
                        track.Reverse();
                        //tracks[t, s] = track;
                        bestpath = PathsInfo.ConstructPath(s, k, pathsInfos[s]).Union(ListToTab(track)).ToArray();
                        
                        return (bestfrom, bestto, autostrada, maxbypass, bestpath.ToArray());
                    }
                    bestpath = PathsInfo.ConstructPath(bestfrom, bestto, pathsInfos[bestfrom]);
                    
                    return (bestfrom, bestto, null, bestlength, bestpath.ToArray());
                }
            }
            
            //return (-1,-1, null, -1, null);
        }

    }

}

