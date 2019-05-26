using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public static class GraphColoring
    {
        /// <summary>
        /// Ogólna metoda - kolorowanie zachłanne na podstawie ustalonego ciągu wierzchołków.
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <param name="order">Porządek wierzchołków, w jakim mają być one przetwarzane. W przypadku null użyj pierwotnego numerowania wierzchołków</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] GreedyColoring(this Graph g, int[] order=null)
        {
            int n = g.VerticesCount;
            int[] results = new int[n];
            if(order == null)
            {
                order = Enumerable.Range(0, n).ToArray();
            }
            
            for (int i = 0; i < n; i++)
            {
                if (g.OutDegree(order[i]) == 0) results[order[i]] = 1;
                else
                {
                    var zajete = new int[n + 1];
                    foreach(int k in g.OutEdges(order[i]).Select(x =>  results[x.To] ))
                    {
                        zajete[k] = 1;
                    }

                    for(int j =1;j<=n;j++)
                        if (zajete[j] == 0)
                        {
                            results[order[i]] = j;
                            break;
                        }
                } 
                
            }
            return results;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą BFS
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] BFSColoring(this Graph g)
        {
            int n = g.VerticesCount;
            Queue<int> queue = new Queue<int>();
            Graph graph = g.Clone();
            var visited = new bool[n];
            int[] order = new int[n];
            int ind = 0;
            do
            {
                int start = 0;
                for (int i = 0; i < visited.Length; i++)
                {
                    if(!visited[i]) {start = i; break; }
                }
                queue.Enqueue(start);
                while (queue.Any())
                {
                    int d = queue.Dequeue();
                    visited[d] = true;
                    order[ind++] = d;
                    foreach(int w in g.OutEdges(d).Select(x=> x.To))
                    {
                        if (!visited[w])
                        {
                            visited[w] = true;
                            queue.Enqueue(w);
                        }
                    }
                }

            } while (visited.Any(x => !x));

            return GreedyColoring(g,order);
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą LargestBackDegree
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] LargestBackDegree(this Graph g)
        {
            int n = g.VerticesCount;
            var stopniewsteczne = new int[n];
            for (int v = 0; v < n; v++)
            {
                int suma = 0;
                var outz = g.OutEdges(v).Select(x => x.To);
                foreach (int w in outz )
                {
                    if (w > v) suma++;
                }
                stopniewsteczne[v] = suma;
            }

            var seq = from x in Enumerable.Range(0, n)
                      orderby stopniewsteczne[x] descending,x ascending
                      select x;
            return GreedyColoring(g,seq.ToArray());
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą ColorDegreeOrdering
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] ColorDegreeOrdering(this Graph g)
        {
            int n = g.VerticesCount;
            int[] order = new int[n];
            bool[] visited = new bool[n];
            int ind = 0;
            int[] colorDegree = new int[n];
            int[] results = new int[n];

            int maxind = 0;
            for(int i = 0; i < n; i++)
            {
                int j = 0;
                for(int k = 0; k < n; k++)
                {
                    if (!visited[k]) { j = k; break; }
                }
                maxind = j; 
                for (int ii = maxind; ii < n; ii++)
                {
                    if (colorDegree[ii] > colorDegree[maxind] && !visited[ii]) maxind = ii;
                }
                visited[maxind] = true;

                var zajete = new int[n + 1];
                foreach (int k in g.OutEdges(maxind).Select(x => x.To))
                {
                    colorDegree[k]++;
                    zajete[results[k]] = 1;
                }
                int jj;
                for (jj = 1; jj <= n; jj++)
                    if (zajete[jj] == 0)
                    {
                        results[maxind] = jj;
                        break;
                    }
                //if (jj > n) results[maxind] = 1;
                
            }
            
            return results;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą Incremental
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] Incremental(this Graph g)
        {
            int n = g.VerticesCount;
            int[] results = new int[n];

            int color = 0;
            while (results.Any(x => x == 0))
            {
                color++;
                List<int> to_color = new List<int>();
                bool[] temp_visited = new bool[n];
                for (int i = 0; i < n; i++)
                {
                    if (results[i]!=0) continue;
                    bool flag = true;
                    foreach (int w in g.OutEdges(i).Select(x => x.To))
                    {
                        if (temp_visited[w]) flag = false;
                    }
                    if (flag) //mozna dodac;
                    {
                        temp_visited[i] = true;
                        results[i] = color;
                    }
                }
            }

            return results;
        }
    }
}
