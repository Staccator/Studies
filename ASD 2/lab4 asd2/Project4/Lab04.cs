using System;
using System.Collections.Generic;
namespace ASD
{
    public enum TaxAction
    {
        Empty,
        TakeMoney,
        TakeCarrots
    }

    public class TaxCollectorManager : MarshalByRefObject
    {        
        public int CollectMaxTax(int[] dist, int[] money, int[] carrots, int maxCarrots, int startingCarrots, out TaxAction[] collectingPlan)
        {
            collectingPlan = null;
            //var planList = new List<TaxAction>();
            var xd = new List<int>(); 
            int max = -1;

            int[,] tab = new int[dist.Length + 1, maxCarrots + 1];
            TaxAction[,] acts = new TaxAction[dist.Length + 1, maxCarrots + 1];
            int[,] prev = new int[dist.Length + 1, maxCarrots + 1];
            //List<bool>[,] TaxList = new List<bool>[dist.Length + 1, maxCarrots + 1]; 
            for (int i=0; i< dist.Length + 1; i++)
            {
                for (int j=0; j<=maxCarrots; j++)
                {
                    tab[i, j] = -1;
                }
            }

            //TaxList[0, startingCarrots] = new List<bool>();
            tab[0,startingCarrots]=0;
            int geld, maxgeld, marchewki, maxmarchewki, distance;

            for (int i=0; i < dist.Length; i++)
                for (int j = 0; j <= maxCarrots; j++)
                {
                    geld = tab[i, j];
                    if (geld < 0) continue;
                    maxgeld = geld + money[i];
                    marchewki = j;
                    maxmarchewki = j + carrots[i];
                    if (maxmarchewki > maxCarrots) maxmarchewki = maxCarrots;
                    if (dist.Length - 1 == i)
                    {
                        if (maxmarchewki < startingCarrots) continue;
                        distance = 0;
                    }
                    else distance = dist[i+1];
                    //carrots
                    int a = maxmarchewki - distance;
                    if (a >= 0)
                    {
                        if (tab[i + 1, a] < geld)
                        {
                            tab[i + 1, a] = geld;
                            //TaxList[i + 1, a] = new List<bool>(TaxList[i, j]) { true };
                            acts[i + 1, a] = TaxAction.TakeCarrots;
                            prev[i + 1, a] = j;
                            //if (i == 0) TaxList[i + 1, maxmarchewki - distance] = new List<bool>() { true };
                            //else
                            //{
                            //    var temp = TaxList[i, j].ConvertAll(x => x);
                            //    temp.Add(true);
                            //    TaxList[i + 1, maxmarchewki - distance] = temp;
                            //}
                        }
                    }
                    else continue;
                    //geld
                   int b = marchewki - distance;
                    if (b >= 0)
                    {
                        if (tab[i + 1, b] < maxgeld)
                        {
                            tab[i + 1, b] = maxgeld;
                            acts[i + 1, b] = TaxAction.TakeMoney;
                            prev[i + 1, b] = j;
                            //TaxList[i, j].Add(false);
                            //TaxList[i + 1, b] =TaxList[i, j];
                            //    if (i == 0) TaxList[i + 1, marchewki - distance] = new List<bool>() { false };
                            //    else
                            //    {
                            //        var temp = TaxList[i, j]; temp.Add(false);
                            //        TaxList[i + 1, marchewki - distance] = temp;
                            //    }
                            //}
                        }
                    }
                }
                      
            int maxind = -1;

            for (int i = startingCarrots; i <= maxCarrots; i++)
            {
                if (tab[dist.Length, i] > max)
                {
                    max = tab[dist.Length, i];
                    maxind = i;
                }
            }
            if (maxind != -1)
            {
                collectingPlan = new TaxAction[money.Length];
                int j = maxind;
                for (int i = dist.Length; i > 0; i--)
                {
                    collectingPlan[i - 1] = acts[i, j];
                    j = prev[i, j];
                }
            }
            return max;
        }

    }
}
