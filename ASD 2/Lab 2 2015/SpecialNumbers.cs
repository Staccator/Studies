
using System;

namespace ASD
{
    class SpecialNumbers
    {
        const int mod = 10000;
        
        public static int SpecialNumbersRec(int n)
        {
            if (n == 0) return 0;
            if (n == 1) return 9;
            int suma = 0;
            for (int i = 1; i < 10; i++)
            {
                SpecialRec(1, i, ref suma, n);
            }
            return suma;
        }

        public static void SpecialRec(int n, int k,ref int suma,int max)
        {
            for(int i = 1; i < 10; i++)
            {
                if (k >= i && ( k == i || (k+i) %2 == 1))
                {
                    if (n  >= max)
                    {
                        suma++;
                        if (suma > mod) suma = 1;
                        return;
                    }
                    SpecialRec(n + 1 , i,ref suma,max);
                }
                
            }
        }

        // programowanie dynamiczne
        // n cyfr
        public static int SpecialNumbersDP(int n)
        {
            if (n == 0) return 0;
            if (n == 1) return 9;
            int[] tab = new int[10], next = new int[10];
            for(int i = 0;i<10;i++) { tab[i] = 1; next[i] = 0; }
            for(int i = 2;i<=n; i++)
            {
                for(int j = 1; j <= 9; j++)
                {
                    for(int k = j; k <= 9; k++)
                    {
                        if(k==j || (k+j)%2 == 1)
                        {
                            next[k] += tab[j];
                            if (next[k] > mod) next[k] -= mod;
                        }
                    }
                }
                for(int l = 1; l <= 9; l++)
                {
                    tab[l] = next[l];
                    next[l] = 0;
                }
            }
            int suma = 0;
            for(int i = 1; i < 10; i++)
            {
                suma += tab[i];
            }
            suma %= mod;
            return suma;
        }

        // programowanie dynamiczne
        // n cyfr
        // req - tablica z wymaganiami, jezeli req[i, j] == 0 to znaczy, ze  i + 1 nie moze stac PRZED j + 1
        public static int SpecialNumbersDP(int n, bool[,] req)
        {
            // ZMIEN
            return 0;
        }

    }//class SpecialNumbers

}//namespace ASD