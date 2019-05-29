using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs
{
    class Program
    {
        public static void PrintEdges(IGraphEnumerable graph)
        {
            foreach(var item in graph)
            {
                Console.Write(item+" ");
            }
            Console.WriteLine();
        }

        public static bool IsEuler(IGraphEnumerable graph)
        {
            var indegrees = new int[graph.verticesCount];
            var outdegrees = new int[graph.verticesCount];

            foreach(var edge in graph)
            {
                outdegrees[edge.From]++;
                indegrees[edge.To]++;
            }

            for (int i = 0; i < graph.verticesCount; i++)
            {
                if (outdegrees[i] != indegrees[i]) return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            Cycle cycle = new Cycle(new int[] { 0, 2, 1, 3 });
            PrintEdges(cycle);
            Console.WriteLine("Cycle - is Euler: " + IsEuler(cycle));
            Console.WriteLine("---");
            Console.WriteLine("---");
            Grid grid = new Grid(3, 2);
            PrintEdges(grid);
            Console.WriteLine("Grid - is Euler: " + IsEuler(grid));
            Console.WriteLine("---");
            MatrixGraph matrixGraph1 = new MatrixGraph(4);
            matrixGraph1.AddEdge(0, 3);
            matrixGraph1.AddEdge(1, 2);
            matrixGraph1.AddEdge(3, 1);
            PrintEdges(matrixGraph1);
            Console.WriteLine("MatrixGraph1 - is Euler: " + IsEuler(matrixGraph1));
            Console.WriteLine("---");
            MatrixGraph matrixGraph2 = new MatrixGraph(4);
            matrixGraph2.AddEdge(0, 3);
            matrixGraph2.AddEdge(3, 2);
            matrixGraph2.AddEdge(2, 0);
            matrixGraph2.AddEdge(1, 3);
            matrixGraph2.AddEdge(3, 1);
            PrintEdges(matrixGraph2);
            Console.WriteLine("MatrixGraph2 - is Euler: " + IsEuler(matrixGraph2));
            Console.WriteLine("---");
        }
    }
}
