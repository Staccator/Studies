using System;
using System.Collections.Generic;
using System.Collections;

namespace AsdLab5
{
	public class InvalidExchangeException : Exception
	{
		public InvalidExchangeException ()
		{
		}

		public InvalidExchangeException (string msg) : base(msg)
		{
		}

		public InvalidExchangeException (string msg, Exception ex) : base(msg, ex)
		{
		}
	}

	public struct ExchangePair
	{
		public readonly int From;
		public readonly int To;
		public readonly double Price;

		public ExchangePair (int from, int to, double price)
		{
			if (to < 0 || from < 0 || price <= 0.0)
				throw new InvalidExchangeException ();
			From = from;
			To = to;
			Price = price;
		}
	}

    public class CurrencyGraph
    {
        private static double priceToWeight(double price)
        {
            return -Math.Log(price);
        }

        private static double weightToPrice(double weight)
        {
            return Math.Exp(-weight);
        }

        private double[,] weights;

        public CurrencyGraph(int n, ExchangePair[] exchanges)
        {
            weights = new double[n, n];
            for (int i = 0; i < weights.GetLength(0); i++)
                for (int j = weights.GetLength(1) - 1; j >= 0; j--)
                {
                    weights[i, j] = 1;
                }
            
            foreach (var exchange in exchanges)
            {
                weights[exchange.From, exchange.To] = priceToWeight(exchange.Price);
            }
        }

        // wynik: true jesli nie na cyklu ujemnego
        // currency: waluta "startowa"
        // bestPrices: najlepszy (najwyzszy) kurs wszystkich walut w stosunku do currency (byc mo¿e osiagalny za pomoca wielu wymian)
        //   jesli wynik == false to bestPrices = null
        public bool findBestPrice(int currency, out double[] bestPrices)
        {
            double[] dist;
            int[] prev;
            int n = weights.GetLength(0);
            bestPrices = new double[n];
            if (!FordBellmanShortestPaths(currency, out dist, out prev))
            { bestPrices = null; return false; }
            for (int i = 0; i < n; i++)
            {
                bestPrices[i] = weightToPrice(dist[i]);
            }

            return true;
        }

        // wynik: true jesli jest mozliwosc arbitrazu, false jesli nie ma (nie rzucamy wyjatkow!)
        // currency: waluta "startowa"
        // exchangeCycle: a cycle of currencies starting from 'currency' and ending with 'currency'
        //  jesli wynik == false to exchangeCycle = null
        public bool findArbitrage(int currency, out int[] exchangeCycle)
        {
            exchangeCycle = null;
            double[] bestPrices;
            if (FordBellmanShortestPaths(currency, out double[] dist, out int[] prev)) return false;
            //for (int i = 0; i < bestPrices.Length; i++)
            //{
            //    if (i == currency) continue;
            //    if (bestPrices[i] != 0 && weights[i,currency]!=1)
            //    {
            //        if (bestPrices[i] * weightToPrice(weights[i, currency]) > 1) return true;
            //    }
            //}
            // Czêœæ 1: wywolac odpowiednio FordBellmanShortestPaths

            // Czêœæ 2: dodatkowo wywolac odpowiednio FindNegativeCostCycle
            //
            FindNegativeCostCycle(dist, prev, out int[] cycle);
            exchangeCycle = cycle;
            return true;
        }

        // wynik: true jesli nie na cyklu ujemnego
        // s: wierzcho³ek startowy
        // dist: obliczone odleglosci
        // prev: tablica "poprzednich"
        private bool FordBellmanShortestPaths(int s, out double[] dist, out int[] prev)
        {
            dist = null;
            prev = null;
            int n = weights.GetLength(0);
            dist = new double[n];
            prev = new int[n];
            for (int i = 0; i < n; i++)
            {
                prev[i] = -1;
                dist[i] = double.MaxValue;
            }

            dist[s] = 0;
            for (int k = 1; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        double w = weights[i, j];
                        if (w == 1) continue;
                        if (dist[i] + w < dist[j])
                        {
                            dist[j] = dist[i] + w;
                            prev[j] = i;
                        }
                    }
            }
            


            for (int k = 1; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        double w = weights[i, j];
                        if (w == 1) continue;
                        if (dist[i] + w < dist[j])
                        {
                            return false;
                        }
                    }
            return true;
        }

        // wynik: true jesli JEST cykl ujemny
        // dist: tablica odleglosci
        // prev: tablica "poprzednich"
        // cycle: wyznaczony cykl (kolejne elementy to kolejne wierzcholki w cyklu, pierwszy i ostatni element musza byc takie same - zamkniêcie cyklu)
        private bool FindNegativeCostCycle(double[] dist, int[] prev, out int[] cycle)
        {
            cycle = null;
            //
            // wyznaczanie cyklu ujemnego
            // przykladowy pomysl na algorytm
            // 1) znajdowanie wierzcholka, którego odleglosc zostalaby poprawiona w kolejnej iteracji algorytmu Forda-Bellmana
            // 2) cofanie sie po lancuchu poprzednich (prev) - gdy zaczna sie powtarzac to znaleŸlismy wierzcholek nale¿acy do cyklu o ujemnej dlugosci
            // 3) konstruowanie odpowiedzi zgodnie z wymogami zadania
            //
            int n = weights.GetLength(0);

            for (int k = 1; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        double w = weights[i, j];
                        if (w == 1) continue;
                        if (dist[i] + w < dist[j])
                        {
                            int start = j;
                            List<int> cykl = new List<int>();
                            cykl.Add(start);
                            int m = i;
                            cykl.Add(m);
                            while(!cykl.Contains(prev[m]))
                            {
                                cykl.Add(prev[m]);
                                m = prev[m];
                            }
                            int a = prev[m];
                            int ind = cykl.IndexOf(a);
                            var cykl2 = new List<int>();
                            for(int x = ind; x < cykl.Count; x++)
                            {
                                cykl2.Add(cykl[x]);
                            }
                            cykl2.Add(a);
                            cykl2.Reverse();
                            cycle = cykl2.ToArray();
                            return true;
                        }
                    }
            return false;

        }

        public static void PrintTab<T>(T[] tab)
        {
            for(int i =0;i< tab.Length; i++)
            {
                Console.Write($"{tab[i],-3}");
            }
            Console.WriteLine();
        }
	}
}