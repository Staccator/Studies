using System;
using System.Collections.Generic;
using ASD.Graphs;
using System.Linq;
using System.Collections;

namespace ASD
{
    public class RoutePlanner : MarshalByRefObject
    {

        // mozna dodawac metody pomocnicze

        /// <summary>
        /// Znajduje dowolny cykl skierowany w grafie lub stwierdza, że go nie ma.
        /// </summary>
        /// <param name="g">Graf wejściowy</param>
        /// <returns>Kolejne wierzchołki z cyklu lub null, jeśli cyklu nie ma.</returns>
        public int[] FindCycle(Graph g)
        {
                Stack<int> stack = new Stack<int>();
                int start = -1;
                int ss;
                bool[] visited = new bool[g.VerticesCount];
                g.GeneralSearchAll<EdgesStack>((int x) => { stack.Push(x); visited[x] = true; return true; }, (int x) => { stack.Pop(); visited[x] = false; return true; },
                    (Edge e) => { if (visited[e.To]) { start = e.To; return false; } return true; }, out ss);
                if (start == -1) return null;
                List<int> cycle = new List<int>();
                while ((stack.Peek() != start))
                {
                    int nowy = stack.Pop();
                    cycle.Add(nowy);
                }
                cycle.Add(start);
                cycle.Reverse();
                return cycle.ToArray();
        }

        public static List<int> DFSfindcycle2(Graph g, int v, Stack stack,out bool []visited)
        {
            //PrintGraph(g);
            List<int> cycle = null;
            bool[] visitedVertices = new bool[g.VerticesCount];
            g.GeneralSearchFrom<EdgesStack>(v,(int x) =>
                {
                   // Console.WriteLine("Wierzcholek" + x);
                    stack.Push(x);
                    bool fuckup = true;
                    foreach (int To in g.OutEdges(x).Select(e => e.To))
                    {
                        if (visitedVertices[To])
                        {
                            cycle = new List<int>();
                            while ((int)stack.Peek() != To)
                            {
                                int nowy = (int)stack.Pop();
                                cycle.Add(nowy);
                            }
                            cycle.Add(To);
                            cycle.Reverse();
                            //PrintEnumerable(cycle);
                            return false;
                        }
                        if (!visitedVertices[To]) fuckup = false;
                    }
                    if (fuckup) return false;
                    return true;
                }
                , null, null,visitedVertices);

            visited = visitedVertices;
            return cycle;
        }

        public static void PrintEnumerable<T>(IEnumerable<T> enu)
        {
            foreach(var x in enu)
            {
                Console.Write($"{x,-3 }");
            }
            Console.WriteLine();
        }

        public static void PrintGraph(Graph g)
        {
            Console.WriteLine();
            for(int i = 0; i < g.VerticesCount; i++)
            {
                Console.Write(i + " : ");
                PrintEnumerable(g.OutEdges(i).Select(e => e.To));
            }
        }

        public static List<int> DFSfindcycle(Graph g, int v, Stack stack)
        {
                stack.Push(v);
                //int[] tab = (from e in g.OutEdges(v) select e.To).ToArray();
                foreach (int To in g.OutEdges(v).Select(e=> e.To))
                {
                    if (stack.Contains(To))
                    {
                        List<int> cycle = new List<int>();
                        while ((int)stack.Peek() != To)
                        {
                            int nowy = (int)stack.Pop();
                            cycle.Add(nowy);
                        }
                        cycle.Add(To);
                        cycle.Reverse();
                        return cycle;
                    }

                    List<int> cykl = DFSfindcycle(g, To, stack);
                    if (cykl != null) return cykl;
                    else return null;
                }
                return null;
        }

        /// <summary>
        /// Rozwiązanie wariantu 1.
        /// </summary>
        /// <param name="g">Graf połączeń, które trzeba zrealizować</param>
        /// <returns>Lista tras autobusów lub null, jeśli zadanie nie ma rozwiązania</returns>
        public int[][] FindShortRoutes(Graph g)
        {
            for (int i = 0; i < g.VerticesCount; i++)
            {
                if (g.OutDegree(i) != g.InDegree(i)) return null;
            }

            List<List<int>> results = new List<List<int>>();

            Graph graph = g.Clone();
            for(;;)
            {
                if (graph.EdgesCount == 0) break;   
                var cykl1 = FindCycle(graph);
                if (cykl1 != null)
                {
                    var cykl = cykl1.ToList();
                    results.Add(cykl);
                    for (int j = 0; j < cykl.Count() - 1; j++)
                    { graph.DelEdge(new Edge(cykl[j], cykl[j + 1])); }

                    graph.DelEdge(cykl[cykl.Count() - 1], cykl[0]);
                }
                else return null;
            }

            if (graph.EdgesCount != 0) return null;

            int[][] res = new int[results.Count()][];
            for(int i =0;i< results.Count();i++)
            {
                res[i] = results[i].ToArray();
            }
            return res;
        }
        

        /// <summary>
        /// Rozwiązanie wariantu 2.
        /// </summary>
        /// <param name="g">Graf połączeń, które trzeba zrealizować</param>
        /// <returns>Lista tras autobusów lub null, jeśli zadanie nie ma rozwiązania</returns>
        /// </summary>
        public int[][] FindLongRoutes(Graph g)
        {
            int[][] results = FindShortRoutes(g);
            if (results == null) return null;

            List<List<int>> temp = new List<List<int>>();
            for (int i = 0; i < results.Count(); i++)
            {
                temp.Add(results[i].ToList());
            }

            et1:
            for(int i =0;i< temp.Count();i++)
            for(int j =i+1;j< temp.Count(); j++)
                {
                    var cycle1 = temp[i];
                    var cycle2 = temp[j];
                    for (int k = 0; k < cycle1.Count(); k++)
                        for (int l = 0; l < cycle2.Count(); l++)
                        {
                            if(cycle1[k] == cycle2[l])
                            {
                                List<int> nowycykl = new List<int>();
                                for (int m = 0; m < k; m++)
                                    nowycykl.Add(cycle1[m]);
                                for (int m = l; m < cycle2.Count(); m++)
                                    nowycykl.Add(cycle2[m]);
                                for (int m = 0; m < l; m++)
                                    nowycykl.Add(cycle2[m]);
                                for (int m = k; m < cycle1.Count(); m++)
                                    nowycykl.Add(cycle1[m]);
                                temp.Remove(cycle1);temp.Remove(cycle2);
                                temp.Add(nowycykl);
                                goto et1;
                            }
                        }
                }



            int[][] res = new int[temp.Count()][];
            for (int i = 0; i < temp.Count(); i++)
            {
                res[i] = temp[i].ToArray();
            }
            return res;
        }

        public int CheckForOtherCycles(List<int> cykl, List<List<int>> res)
        {
            int startnumber = res.Count();
            //Print(cykl);
            List<int> todelet = null;
            res.Remove(cykl);
            et:
            if (todelet != null) { res.Remove(todelet); }

            for(int i = 0; i < cykl.Count(); i++)
            {
                int a = cykl[i];
                foreach(var list in res)
                {
                    if (list[0] == a)
                    {
                        List<int> nowycykl = new List<int>();
                        for (int k = 0; k < i; k++)
                            nowycykl.Add(cykl[k]);
                        nowycykl = nowycykl.Union(list).ToList();
                        for (int k = i; k < cykl.Count(); k++)
                            nowycykl.Add(cykl[k]);
                        cykl = nowycykl;
                        todelet = list;
                        goto et;
                    }
                }
            }
            //Print(cykl);
            res.Add(cykl);
            startnumber -= res.Count();
            return startnumber;
        }

        public static void Print<T>(IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable) Console.Write($"{t,3}");
            Console.WriteLine();
        }
    }

}
