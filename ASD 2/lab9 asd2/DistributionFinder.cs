using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab9
{
    public class DistributionFinder : MarshalByRefObject
    {
        public (int satisfactionLevel, int[] bestDistribution) FindBestDistribution(int[] limits, int[][] preferences, bool[] isSportActivity)
        {
            int n = preferences.Length; //uzytkownicy
            int k = limits.Length; //zajecia
            int vertices_count = n + k + 2;
            int s = vertices_count - 2;
            int t = vertices_count - 1;

            Graph graph = new AdjacencyListsGraph<HashTableAdjacencyList>(true,vertices_count);
            //Graph graph = new AdjacencyMatrixGraph(true, vertices_count);

            for (int i = n-1; i >=0; i--)
            {
                graph.AddEdge(s, i, 1);
            }

            for (int i = n - 1; i >= 0; i--)
            {
                var user_preferences = preferences[i];
                foreach(int w in user_preferences)
                {
                    if (isSportActivity != null && !isSportActivity[w]) continue;
                    graph.AddEdge(i, n + w, 1);
                }
            }
            for (int i = k-1; i>=0 ; i--)
            {
                graph.AddEdge(n + i, t, limits[i]);
            }
            (double value,Graph Flow) = graph.FordFulkersonDinicMaxFlow(s, t, MaxFlowGraphExtender.DFSBlockingFlow);


            int[] bestdistribution = new int[n];
            for (int i = n - 1; i >= 0; i--)
            {
                bestdistribution[i] = -1;
            }

            for (int i = n - 1; i >= 0; i--)
            {
                foreach(Edge e in Flow.OutEdges(i))
                {
                    if(e.Weight == 1)
                    {
                        bestdistribution[i] = e.To - n;
                        break;
                    }
                }
            }

            if (isSportActivity != null)
            {
                for (int i = 0; i < k; i++)
                {
                    if (Flow.GetEdgeWeight(n+i,t) != limits[i] && isSportActivity[i]) return (0, null);
                }

                return (1, bestdistribution);
            }


            return ((int)value,bestdistribution);
        }

    }
}
