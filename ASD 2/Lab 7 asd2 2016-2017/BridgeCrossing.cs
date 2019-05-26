using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class BridgeCrossing
    {

        /// <summary>
        /// Metoda rozwiązuje zadanie optymalnego przechodzenia przez most.
        /// </summary>
        /// <param name="times">Tablica z czasami przejścia poszczególnych osób</param>
        /// <param name="strategy">Strategia przekraczania mostu: lista list identyfikatorów kolejnych osób,
        /// które przekraczają most (na miejscach parzystych przejścia par przez most,
        /// na miejscach nieparzystych powroty jednej osoby z latarką). Jeśli istnieje więcej niż jedna strategia
        /// realizująca przejście w optymalnym czasie wystarczy zwrócić dowolną z nich.</param>
        /// <returns>Minimalny czas, w jakim wszyscy turyści mogą pokonać most</returns>
        public static int CrossBridge(int[] times, out List<List<int>> strategy)
        {

            int besttime = int.MaxValue;
            List<List<int>> beststrat = new List<List<int>>();
            crossBridge(times, new bool[times.Length], new List<List<int>>(), 0,ref beststrat, ref besttime);

            strategy = beststrat;
            return besttime;
        }

        public static void crossBridge(int[] times, bool[] at_other_side, List<List<int>> tempstrategy, int temptime,ref List<List<int>> bestStrategy,ref int besttime)
        {
            if (temptime > besttime) return;

            if(times.Count() == 1)
            {
                besttime = times[0];
                bestStrategy = new List<List<int>> { new List<int> { 0 } };
                return;
            }

            if (AllFinished(at_other_side))
            {
                if(temptime < besttime)
                {
                    besttime = temptime;
                    bestStrategy = tempstrategy.ConvertAll(x=>x);
                }
                return;
            }

            List<int> people_to_go = new List<int>();
            
                int[] people_tab = new int[at_other_side.Count(x => !x)];
                int k = 0;
                for (int i = 0; i < times.Length; i++)
                {
                    if (!at_other_side[i]) people_tab[k++] = i;
                }

                for (int i = 0; i < people_tab.Length; i++)
                    for (int j = i+1; j < people_tab.Length; j++)
                    {
                        int a = people_tab[i], b = people_tab[j];
                        people_to_go = new List<int> { a, b };
                        at_other_side[a] = true; at_other_side[b] = true;
                        if (AllFinished(at_other_side))
                        {
                            tempstrategy.Add(people_to_go);
                            //temptime += Math.Max(times[a], times[b]);
                            crossBridge(times, at_other_side, tempstrategy, temptime + Math.Max(times[a], times[b]),ref bestStrategy, ref besttime);
                            tempstrategy.Remove(people_to_go);
                            at_other_side[a] = false; at_other_side[b] = false;
                        }
                        else
                        {
                            int bestind = -1,bestbacktime = int.MaxValue;
                            for(int m = 0; m < times.Length; m++)
                            {
                                if (at_other_side[m])
                                {
                                    if(times[m] < bestbacktime) { bestbacktime = times[m];bestind = m; }
                                }
                            }
                            List<int> backer = new List<int> { bestind };
                            tempstrategy.Add(people_to_go);
                            tempstrategy.Add(backer);
                            at_other_side[bestind] = false;
                            crossBridge(times, at_other_side, tempstrategy, temptime + Math.Max(times[a], times[b]) + times[bestind],ref bestStrategy, ref besttime);
                            tempstrategy.Remove(backer);
                            tempstrategy.Remove(people_to_go);
                            at_other_side[bestind] = true;
                            at_other_side[a] = false; at_other_side[b] = false;
                        }
                    }
            
           
                
        }

        public static bool AllFinished(bool[] at_other_side)
        {
            foreach(var it in at_other_side)
            {
                if (!it) return false;
            }
            return true;
        }

        // MOŻESZ DOPISAĆ POTRZEBNE POLA I METODY POMOCNICZE
        // MOŻESZ NAWET DODAĆ CAŁE KLASY (ALE NIE MUSISZ)

    }
}