using System;
using System.Collections.Generic;

namespace Lab07
{

    public class SmellsChecker
    {

        private readonly int smellCount;
        private readonly int[][] customerPreferences;
        private readonly int satisfactionLevel;

        /// <summary>
        ///   
        /// </summary>
        /// <param name="smellCount">Liczba zapachów, którymi dysponuje sklep</param>
        /// <param name="customerPreferences">Preferencje klientów
        /// Każda tablica -- element tablicy tablic -- to preferencje jednego klienta.
        /// Preferencje klienta mają długość smellCount, na i-tej pozycji jest
        ///  1 -- klient preferuje zapach
        ///  0 -- zapach neutralny
        /// -1 -- klient nie lubi zapachu
        /// 
        /// Zapachy numerujemy od 0
        /// </param>
        /// <param name="satisfactionLevel">Oczekiwany poziom satysfakcji</param>
        public SmellsChecker(int smellCount, int[][] customerPreferences, int satisfactionLevel)
        {
            this.smellCount = smellCount;
            this.customerPreferences = customerPreferences;
            this.satisfactionLevel = satisfactionLevel;
        }

        public static bool checkForSatisfaction(int[] tab,int sat)
        {
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] < sat) return false;
            }
            return true;
        }

        public static int TabMinimum(int[] tab)
        {
            int min = tab[0];
            for(int i = 1; i < tab.Length; i++)
            {
                if (tab[i] < min) min = tab[i];
            }
            return min;
        }

        public List<int> FindSmells(ref int[] satisfactiontab,ref List<int> smells,int index,bool changed)
        {
            //int a = satisfactionLevel - smellCount + index;
            if(changed) if (checkForSatisfaction(satisfactiontab, satisfactionLevel)) return smells;

            if (index == smellCount) return null;

            //if (smellCount - index + smells.Count < satisfactionLevel) { Console.WriteLine("x"); return null; }

            //int min = TabMinimum(satisfactiontab);

            //if (smellCount - index +min < satisfactionLevel) { return null; }

            //int tofull = index - smellCount + satisfactionLevel;
            //for (int i = 0; i < satisfactiontab.Length; i++)
            //{
            //    if (satisfactiontab[i] < tofull) { return null; }
            //}

            {
                smells.Add(index);
                int check = satisfactionLevel - smellCount + index + 1;
                int lengt = customerPreferences.Length-1;
                for (int j = 0; j < customerPreferences.Length; j++)
                {
                    satisfactiontab[j] += customerPreferences[j][index];
                    if (satisfactiontab[j] < check) { lengt = j; goto et; }
                }
                var result = FindSmells(ref satisfactiontab, ref smells,index+1,true);
                if (result != null) return result;
                et:
                bool flag = true;
                for (int j = lengt; j >=0; j--)
                {
                    satisfactiontab[j] -= customerPreferences[j][index];
                    if (satisfactiontab[j] < check) { flag = false;  }
                }
                smells.Remove(index);
                if (flag)
                {
                    result = FindSmells(ref satisfactiontab, ref smells, index + 1,false);
                    if (result != null) return result; 
                }
            }
            //et2:
            return null;
        }

        /// <summary>
        /// Implementacja etapu 1
        /// </summary>
        /// <returns><c>true</c>, jeśli przypisanie jest możliwe <c>false</c> w p.p.</returns>
        /// <param name="smells">Wyjściowa tablica rozpylonych zapachów realizująca rozwiązanie, jeśli się da. null w p.p. </param>
        public Boolean AssignSmells(out bool[] smells)
        {
            smells = null;
            int[] satisfactiontab = new int[customerPreferences.Length];
            List<int> smellz = new List<int>();
            var result = FindSmells(ref satisfactiontab, ref smellz,0,true);
            if (result == null) return false;
            smells = new bool[smellCount];
            foreach (var smel in result)
                smells[smel] = true;
            return true;
        }

        /// <summary>
        /// Implementacja etapu 2
        /// </summary>
        /// <returns>Maksymalna liczba klientów, których można usatysfakcjonować</returns>
        /// <param name="smells">Wyjściowa tablica rozpylonych zapachów, realizująca ten poziom satysfakcji</param>
        public int AssignSmellsMaximizeHappyCustomers(out bool[] smells)
        {
            smells = new bool[smellCount];
            return -1;
        }

    }

}

