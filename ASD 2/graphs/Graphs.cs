using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs
{
    public struct Edge
    {
        public int From { get; set; }
        public int To { get; set; }

        public Edge(int from, int to)
        {
            this.From = from;
            this.To = to;
        }

        public override string ToString()
        {
            return $"{From}=>{To}";
        }
    }

    public interface IGraphEnumerable : IEnumerable<Edge>
    {
        int verticesCount { get; }
    }

    public class Cycle : IGraphEnumerable
    {
        private int[] vertices;

        public Cycle(int[] vertices)
        {
            this.vertices = vertices;
        }

        public int verticesCount { get { return vertices.Count(); } }

        public IEnumerator<Edge> GetEnumerator()
        {
            for(int i = 0; i < vertices.Length-1; i++)
            {
                yield return new Edge(vertices[i], vertices[i + 1]);
            }
            yield return new Edge(vertices[vertices.Length-1],vertices[0]);
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Grid : IGraphEnumerable
    {
        int width;
        int height;

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public int verticesCount { get { return width * height; } }

        public IEnumerator<Edge> GetEnumerator()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height-1; j++)
                {
                    yield return new Edge(j * width + i, (j + 1) * width + i);
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width - 1; j++)
                {
                    yield return new Edge(i * width + j, i * width + j +1);
                }
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MatrixGraph : IGraphEnumerable
    {
        int[,] matrix;
        public MatrixGraph(int vertices)
        {
            matrix = new int[vertices, vertices];
        }

        public int verticesCount { get { return matrix.GetLength(0); } }

        public void AddEdge(int from, int to)
        {
            matrix[from, to] = 1;
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                    if (matrix[i, j] == 1) yield return new Edge(i, j);
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
