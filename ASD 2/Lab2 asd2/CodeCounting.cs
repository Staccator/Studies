using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class CodesCounting : MarshalByRefObject
    {
        static void Main(string[] args)
        {
            Console.WriteLine(CountCodes("abcabc", new string[] { "ab", "abc", "a", "b", "c" },out int[][]soul));
            foreach(var item in soul)
            {
                foreach (var x in item) Console.Write(x + " ");
                Console.WriteLine();
            }
        }


        public static int CountCodes(string text, string[] codes, out int[][] solutions)
        {
            int[] tab = new int[text.Length];
            List<List<int>>[] soul = new List<List<int>>[text.Length];
            for (int i = 0; i < soul.Length; i++)
            {
                soul[i] = new List<List<int>>();
            }
            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j < codes.Length; j++)
                {
                    string str = codes[j];
                    if (i + str.Length - 1 < text.Length)
                    {
                        if (text.Substring(i, str.Length) == str)
                        {
                            if (i == 0)
                            {
                                tab[i + str.Length - 1]++;
                                soul[i + str.Length - 1].Add(new List<int> { j });
                            }
                            else
                            {
                                tab[i + str.Length - 1] += tab[i - 1];
                                foreach (List<int> list in soul[i - 1])
                                {
                                    List<int> newlist = list.ToList();
                                    newlist.Add(j);
                                    soul[i + str.Length - 1].Add(newlist);
                                }
                            }
                        }
                    }
                }
            }

            solutions = new int[soul[soul.Length - 1].Count][];
            int k = 0;
            foreach (List<int> list in soul[text.Length - 1])
            {
                solutions[k++] = list.ToArray();
            }
            return tab[tab.Length - 1];
        }

    }

}
