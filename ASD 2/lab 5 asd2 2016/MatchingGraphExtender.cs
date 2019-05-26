using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD
{
    public static class MatchingGraphExtender
    {
        /// <summary>
        /// Podział grafu na cykle. Zakładamy, że dostajemy graf nieskierowany i wszystkie wierzchołki grafu mają parzyste stopnie
        /// (nie trzeba sprawdzać poprawności danych).
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Tablica cykli; krawędzie każdego cyklu powinny być uporządkowane zgodnie z kolejnością na cyklu, zaczynając od dowolnej</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m)
        /// </remarks>
        public static Edge[][] cyclePartition(this Graph G)
        {
            Graph g = G.Clone();

            int i = 0;
            Stack st = new Stack();
            List<List<Edge>> ListofLists = new List<List<Edge>>();
            //for (int j = 0; j < g.VerticesCount; j++)
            //{
            //    Console.Write(j + " : ");
            //    foreach (Edge e in g.OutEdges(j)) Console.Write($"{e,10}");
            //    Console.WriteLine();
            //}
            while (g.EdgesCount!=0)
            {
                if (g.OutDegree(i) == 0) { i++;continue; }
                int v = i;
                DFSfindcycles(g, v,st,ListofLists);
            }
            Edge[][] result = new Edge[ListofLists.Count][];
            i = 0;
            foreach(var list in ListofLists)
            {
                result[i++] = list.ToArray();
            }
            return result;
        }
        public static void DFSfindcycles(Graph g,int v, Stack stack, List<List<Edge>> list)
        {
            int top = -1;
            if (stack.Count > 0)
            {
                top = (int)stack.Peek();
            }
                stack.Push(v);
            {
                int[] tab = (from e in g.OutEdges(v) select e.To).ToArray();
                foreach(int To in tab)
                {
                    if (stack.Count > 0)
                    {
                        if ((int)stack.Peek() != v) break;
                        if (To == top) continue;
                    }

                    int to = To;
                    if(stack.Contains(to))
                    {
                        
                        List<Edge> cycle = new List<Edge>();
                        int last = to;
                        while ((int)stack.Peek() != to)
                        {
                            int nowy = (int)stack.Pop();
                            cycle.Add(new Edge(last, nowy));
                            g.DelEdge(last, nowy);
                            last = nowy;
                        }
                        g.DelEdge(last, to);

                        if (g.OutDegree(to) == 0) stack.Pop();
                        cycle.Add(new Edge(last, to));
                        
                        list.Add(cycle);
                        break;
                    }
                    {
                        bool check = false;
                        foreach (Edge e in g.OutEdges(v))
                        {
                            if (e.To == To) check = true;
                        }
                        if (!check) continue;
                    }
                    DFSfindcycles(g, To, stack, list);
                }
            }
        }

        /// <summary>
        /// Szukanie skojarzenia doskonałego w grafie nieskierowanym o którym zakładamy, że jest dwudzielny i 2^r-regularny
        /// (nie trzeba sprawdzać poprawności danych)
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Skojarzenie doskonałe w G</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m), gdzie m jest liczbą krawędzi grafu G
        /// </remarks>
        public static Graph perfectMatching(this Graph G)
        {
            Graph g = G.Clone();
            int r = (int)Math.Log( g.OutDegree(0) , 2);

            for (int i = 0; i < r; i++)
            {
                var cycles = g.cyclePartition();
                foreach(var cycle in cycles)
                {
                    for(int j = 0; j < cycle.Length; j += 2)
                    {
                        g.DelEdge(cycle[j].From, cycle[j].To);
                    }
                }
            }
            return g;
        }
    }
}
