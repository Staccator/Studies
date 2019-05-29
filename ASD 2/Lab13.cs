using System;
using System.Linq;
using ASD.Graphs;
using System.Collections;
using System.Collections.Generic;


namespace Lab13
{
public class ProgramPlanning  : MarshalByRefObject
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="taskGraph">Graf opisujący zależności procedur</param>
        /// <param name="taskTimes">Tablica długości czasów procedur</param>
        /// <param name="startTimes">Parametr wyjśćiowy z najpóźniejszymi możliwymi startami procedur przy optymalnym czasie całości</param>
        /// <param name="startTimes">Parametr wyjśćiowy z dowolna wybraną ścieżką krytyczną</param>
        /// <returns>Najkrótszy czas w jakim można wykonać cały program</returns>
        public double CalculateTimesLatestPossible(Graph taskGraph, double[] taskTimes, out double[] startTimes, out int[] criticalPath)
            { 
            int n = taskGraph.VerticesCount;
            var reversed = taskGraph.Reverse();
            List<int> startowe = new List<int>();
            for (int  i = 0;  i < taskGraph.VerticesCount;  i++)
            {
                if (taskGraph.InDegree(i) == 0) startowe.Add(i);
            }
            double[] czasy;
            bool[] visited;
            List<int> tovisit;
            int[] FilledWith = new int[n];
            czasy = new double[n];
            visited = new bool[n];
            tovisit = new List<int>(startowe);
            foreach (int start in startowe)
                czasy[start] = taskTimes[start];

            while(tovisit.Count!=0)
            {
                List<int> newtovisit = new List<int>();
                for(int i=0;i<tovisit.Count;i++)
                {
                    int v = tovisit[i];
                    double czas = czasy[v];
                    foreach(int w in taskGraph.OutEdges(v).Select(x=> x.To))
                    {
                        if(taskTimes[w] + czas > czasy[w])
                        {
                            czasy[w] = taskTimes[w] + czas;
                        }
                        FilledWith[w]++;
                        if(FilledWith[w] == taskGraph.InDegree(w)  ) newtovisit.Add(w);
                    }
                }
                tovisit = newtovisit;
            }

            double minimum  = czasy.Max();
            ////////////////////////////////////////////////////
            startowe = new List<int>();
            var StartTimes = new double[n];
            for (int i = 0; i < taskGraph.VerticesCount; i++)
            {
                StartTimes[i] = double.MaxValue;
                if (reversed.InDegree(i) == 0) startowe.Add(i);
            }
            
            
            FilledWith = new int[n];
            tovisit = new List<int>(startowe);
            foreach (int start in startowe)
                StartTimes[start] = minimum -  taskTimes[start];
            while(tovisit.Count!=0)
            {
                List<int> newtovisit = new List<int>();
                for(int i=0;i<tovisit.Count;i++)
                {
                    int v = tovisit[i];
                    double czas = czasy[v];
                    foreach(int w in reversed.OutEdges(v).Select(x=> x.To))
                    {
                        if (StartTimes[w] > StartTimes[v] - taskTimes[w])
                        {
                            StartTimes[w] = StartTimes[v] - taskTimes[w];
                        }
                        FilledWith[w]++;
                        if(FilledWith[w] == reversed.InDegree(w)  ) newtovisit.Add(w);
                    }
                }
                tovisit = newtovisit;
            }

            int index = 0;
            var minstart = czasy.Zip(taskTimes, (a, b) => a - b).ToList();
            for (int i = 0; i < n; i++)
            {
                if (StartTimes[i] == 0)
                {
                    index = i;
                    break;
                }
            }

            var critpath = new List<int>();
            while (taskGraph.OutDegree(index) > 0)
            {
                critpath.Add(index);
                foreach (var w in taskGraph.OutEdges(index).Select(x=>x.To))
                {
                    if (minstart[w] == StartTimes[w])
                    {
                        index = w;
                        break;
                    }
                }
            }
            critpath.Add(index);
            startTimes = StartTimes;
            criticalPath = critpath.ToArray();
            return minimum;
            }
    }
}
