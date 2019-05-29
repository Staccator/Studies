using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab9
{
public struct MuseumRoutes
    {
        public MuseumRoutes(int count, int[][] routes)
            {
            this.liczba = count;
            this.trasy = routes;
            }

        public readonly int liczba;
        public readonly int[][] trasy;
    }


static class Muzeum
    {
        /// <summary>
        /// Znajduje najliczniejszy multizbiór tras
        /// </summary>
        /// <returns>Znaleziony multizbiór</returns>
        /// <param name="g">Graf opisujący muzeum</param>
        /// <param name="cLevel">Tablica o długości równej liczbie wierzchołków w grafie -- poziomy ciekawości wystaw</param>
        /// <param name="entrances">Wejścia</param>
        /// <param name="exits">Wyjścia</param>
        public static MuseumRoutes FindRoutes(Graph g, int[] cLevel, int[] entrances, int[] exits)
            {
            int n = g.VerticesCount;
            Graph graph = new AdjacencyListsGraph<AVLAdjacencyList>(true, 2 * n + 2);
            for (int i = 0; i < entrances.Length; i++)
            {
                graph.AddEdge(2 * n, entrances[i], int.MaxValue);
            }
            for (int i = 0; i < exits.Length; i++)
            {
                graph.AddEdge(exits[i]+n,2*n+1, int.MaxValue);
            }
            for (int i = 0; i < n; i++)
            {
                graph.AddEdge(i, i + n, cLevel[i]);
            }

            bool[,] tab = new bool[n, n];
            int to, from;
            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (Edge edge in g.OutEdges(i))
                {
                    from = edge.From; to = edge.To;

                    if (tab[to, from]) continue;

                    graph.AddEdge(from + n, to, int.MaxValue);
                    graph.AddEdge(to + n, from, int.MaxValue);
                    tab[from, to] = true;
                    tab[to, from] = true;
                }
            }
            var result =  graph.FordFulkersonDinicMaxFlow(2 * n, 2 * n + 1, out Graph flow, MaxFlowGraphExtender.MaxFlowPath, false);
            var results = new List<int[]>();

            for(int i = 0; i < entrances.Length; i++)
            {
                int entrance = entrances[i];
                while(flow.GetEdgeWeight(entrance,entrance+n) > 0)
                {
                    
                    List<int> path = new List<int>();
                    path.Add(entrance);
                    int v = entrance;
                    while (!exits.Contains(v) || (exits.Contains(v) && flow.GetEdgeWeight(v+n, 2*n+1)==0) )
                    {
                        foreach(int w in flow.OutEdges(v+n).Select(x => x.To))
                        {
                            if (flow.GetEdgeWeight(v + n, w) > 0)
                            {

                                path.Add(w);
                                flow.ModifyEdgeWeight(w, w + n, -1);
                                flow.ModifyEdgeWeight(v + n, w, -1);
                                v = w;
                                break;
                            }
                        }
                    }
                    flow.ModifyEdgeWeight(v+n, 2*n+1, -1);
                    flow.ModifyEdgeWeight(entrance, entrance + n, -1);
                    // path.Add(v);
                    results.Add(path.ToArray());
                }
            }

            return new MuseumRoutes((int) result , results.ToArray());
            }
    }
}

