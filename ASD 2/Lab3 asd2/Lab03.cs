using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{
    public class Lab03 : MarshalByRefObject
    {
        // Część 1
        //  Sprawdzenie czy podany ciąg stopni jest grafowy
        //  0.5 pkt

        int partition(int[] tab, int l, int r)
        {
            int i = l, j = r;
            int pivot = tab[(l + r) / 2];
            while (i <= j)
            {
                while (tab[i] < pivot)
                    i++;
                while (pivot < tab[j])
                    j--;
                if (i <= j)
                {
                    int pom = tab[i];
                    tab[i] = tab[j];
                    tab[j] = pom;
                    i++;
                    j--;
                }
            }
            return i;
        }

        void quicksort(int[] tab, int l, int r)
        {
            if (!(l < r)) return;
            int m = partition(tab, l, r);
            quicksort(tab, l, m - 1);
            quicksort(tab, m, r);
        }
        

        public bool OddSequence(int[] sequence)
        {
            int sum = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                sum += sequence[i];
            }
            return sum % 2 == 1;
        }
        public bool HasOnlyZeros(int[] sequence)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] != 0) return false;
            }
            return true;
        }

        public void PrintTab(int[] sequence)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                Console.Write(sequence[i] + " ");
            }
            Console.WriteLine();
        }

        public void reversetab(int[] sequence)
        {
            for (int i = 0; i < sequence.Length / 2; i++)
            {
                int a = sequence[i];
                sequence[i] = sequence[sequence.Length - 1 - i];
                sequence[sequence.Length - 1 - i] = a;
            }
        }
        public bool IsGraphic(int[] sequence)
        {
            quicksort(sequence, 0, sequence.Length - 1);
            reversetab(sequence);

            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] < 0) return false;
            }

            if (HasOnlyZeros(sequence)) return true;
            if (OddSequence(sequence)) return false;
            //test
            int howmany = sequence[0];
            if (howmany > sequence.Length - 1) return false;
            int [] temp = new int [sequence.Length - 1];
            while (true)
            {
                for(int i = 0; i < temp.Length; i++)
                {
                    temp[i] = sequence[i + 1];
                    if(howmany != 0) { temp[i]--; howmany--; }
                    if (temp[i] < 0) return false;
                }
                quicksort(temp, 0, temp.Length - 1);
                reversetab(temp);
                if (HasOnlyZeros(temp)) return true;
                if (OddSequence(temp)) return false;
                sequence = temp;
                temp = new int[sequence.Length - 1];
                howmany = sequence[0];
                if (howmany > sequence.Length - 1) return false;
            }

            return true;
        }

        //Część 2
        // Konstruowanie grafu na podstawie podanego ciągu grafowego
        // 1.5 pkt
        public Graph ConstructGraph(int[] sequence)
        {
            int count;
            if (!IsGraphic(sequence)) return null;
            int[] sequence1 = (int []) sequence.Clone();

            Graph graph = new AdjacencyListsGraph<AVLAdjacencyList>(false,sequence1.Length);
            quicksort(sequence1, 0, sequence1.Length - 1);
            reversetab(sequence1);

            int[] vertices = new int[sequence1.Length];
            for (int i = 0; i < sequence1.Length; i++)
            {
                vertices[i] = i;
            }
            
            for(int i = 0;i< sequence1.Length; i++)
            {
                count = sequence1[i];
                sequence1[i] = 0;
                for (int j = 0; j < count; j++)
                {
                    graph.Add(new Edge(vertices[i], vertices[i+1+j]));
                    sequence1[i + 1 + j]--;
                }
                for (int m = i + count; m >= i + 1; m--)
                {
                    int k = m;
                    while (k < sequence1.Length - 1 && sequence1[k] < sequence1[k + 1])
                    {
                        int pom = sequence1[k]; sequence1[k] = sequence1[k + 1]; sequence1[k + 1] = pom;
                        pom = vertices[k]; vertices[k] = vertices[k + 1]; vertices[k + 1] = pom;
                        k++;
                    }
                }
                if (HasOnlyZeros(sequence1))return graph.Clone(); 
            }

            return null;
        }

        //Część 3
        // Wyznaczanie minimalnego drzewa (bądź lasu) rozpinającego algorytmem Kruskala
        // 2 pkt
        public Graph MinimumSpanningTree(Graph graph, out double min_weight)
        {
            if (graph.Directed) throw new ArgumentException();

            int n = graph.VerticesCount;
            UnionFind union = new UnionFind(graph.VerticesCount);
            EdgesMinPriorityQueue empq = new EdgesMinPriorityQueue();

            bool[,] tab = new bool[n, n];
            int to, from;
            for(int i = 0; i < graph.VerticesCount; i++)
            {
                foreach (Edge edge in graph.OutEdges(i))
                {
                    from = edge.From; to = edge.To;
                    if (tab[to,from]) continue;
                    empq.Put(edge);
                    tab[from, to] = true;
                }
            }
            Graph result = graph.IsolatedVerticesGraph();

            double weight = 0;
            
            while (!empq.Empty)
            {
                Edge e = empq.Get();
                int v1 = e.From;
                int v2 = e.To;
                int s1 = union.Find(v1);
                int s2 = union.Find(v2);
                if (s1 != s2)
                {
                    union.Union(s1, s2);
                    result.Add(e);
                    weight += e.Weight;
                }
                if (result.EdgesCount == result.VerticesCount - 1) break;
            }
            min_weight = weight;
            return result;
        }
    }
}