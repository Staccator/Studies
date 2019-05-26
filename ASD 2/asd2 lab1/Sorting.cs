
using System;

namespace ASD
{

    //
    // Za kazdy z algorytmow jest 1 pkt.
    //
    
    public class SortingMethods : MarshalByRefObject
    {
        
        public int[] QuickSort(int[] tab)
        {
            if (tab.Length == 0) return tab;

            quicksort(tab,0, tab.Length - 1);
            return tab;
        }
        static int partition(int[] tab, int l, int r)
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



        static void quicksort(int[] tab, int l, int r)
        {
            if (!(l < r)) return;
            int m = partition(tab, l, r);
            quicksort(tab, l, m - 1);
            quicksort(tab, m, r);
        }



        public int[] ShellSort(int[] tab)
        {

            if (tab.Length == 0) return tab;

            int p = 2,n = tab.Length - 1;

            while (p-1<n/2)
                         p *= 2;
                p--;
            while (p >= 1){
                for (int k = p;k<=n; k++)
                {
                    int v = tab[k];
                    int i = k - p;
                    while (i >= 0 && tab[i] > v)
                    { tab[p+i] = tab[i]; i-=p; }
                    tab[p+i] = v;
                }
                p = (p + 1) / 2 - 1;
            }
            return tab;
        }

        public static void downheaper(int[]ost,int p,int n)
        {
            if (ost.Length == 0) return;
            int r;if (p==0) r=1;else r = 2 * p;
            int v = ost[p];



            while (r<n)
            {if (r + 1 < n)
                    if (ost[r+1] > ost[r]) r++;
                        if (ost[r]>v)
                {ost[p] = ost[r];p = r; r = 2 * p;} else break;
                  }
                    ost[p] = v;
        }
        public int[] HeapSort(int[] tab)
        {


            if (tab.Length   == 0)      return tab;   int height = tab.Length;
                    for (int k=height/2;k>=0;k--)     downheaper(tab,k,height);
                 while (height > 0)
            { int pom = tab[0]; tab[0] = tab[height-1]; tab[height-1] = pom;
                height--;downheaper(tab,0, height);
                        }
                         return tab;
        }

        public int[] MergeSort(int[] tab)
        {

            if (tab.Length == 0) return tab;
            mergesort(tab,0, tab.Length - 1);
            return tab;


        }


        static void merge(int[] tab, int l, int m, int r)
        {
            int i = l;

            int j = m + 1;
            int []tmp = new int[r - l + 1]; int k = 0;
            while (i != m + 1 && j != r + 1)
            {
                if (tab[i] < tab[j])
                    tmp[k++] = tab[i++];
                else tmp[k++] = tab[j++];
            }
            if (i != m + 1) { for (int ll = i; ll < m + 1; ll++) tmp[k++] = tab[i++]; }
            else { for (int ll = j; ll < r + 1; ll++) tmp[k++] = tab[j++]; }

            for (int ii = 0; ii < r - l + 1; ii++)
            {

                tab[l + ii] = tmp[ii];
            }
        }
        static void mergesort(int[] tab, int l, int r)
        {
            if (!(l < r)) return;
            int m = (l + r) / 2;

            mergesort(tab, l, m);
            mergesort(tab, m + 1, r);

            merge(tab, l, m, r);
        }

    }
}
