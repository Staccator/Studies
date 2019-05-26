using System;
using System.Collections.Generic;
using System.Linq;

namespace lab14
{
    
    class Lab14
    {
        class cop : IEqualityComparer<Produkt>
        {
            bool IEqualityComparer<Produkt>.Equals(Produkt x, Produkt y)
            {
                throw new NotImplementedException();
            }

            int IEqualityComparer<Produkt>.GetHashCode(Produkt obj)
            {
                throw new NotImplementedException();
            }
        }

        public class GenericCompare<T> : IEqualityComparer<T> where T : class
        {
            private Func<T, object> _expr { get; set; }
            public GenericCompare(Func<T, object> expr)
            {
                this._expr = expr;
            }
            public bool Equals(T x, T y)
            {
                var first = _expr.Invoke(x);
                var sec = _expr.Invoke(y);
                if (first != null && first.Equals(sec))
                    return true;
                else
                    return false;
            }
            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        class Produkt
        {
            public int Kod;
            public string Nazwa;
            public double Wartosc;
            public int LiczbaSztuk;
            public int Klient;
            public Produkt(int ko, string n, double c, int ls, int kl) { Kod = ko; Nazwa = n; Wartosc = c; LiczbaSztuk = ls; Klient = kl; }        
        }

        class Klient
        {
            public int Id;
            public string Imie;
            public string Nazwisko;
            public string Adres;
            public int RokUrodzenia;
            public Klient(int id, string i, string naz, string a, int ru) { Id = id;  Imie = i; Nazwisko = naz; Adres = a; RokUrodzenia = ru; }
        }

        public static void Print(IEnumerable<object> i)
        {
            foreach (var x in i) Console.WriteLine(x);
        }

        public static void Main()
        {
            List<Klient> klienci = new List<Klient>
            {
            new Klient(1,"Jan","Kowalski", "Górna 1, Warszawa", 1989),
            new Klient(2,"Tomasz","Nowak", "Dolna 2/3, Warszawa", 1983),
            new Klient(3,"Anna","Lewandowska", "Lewandowskiego 28, Gdańsk", 1959),
            new Klient(4,"Waldemar","Dąbrowski", "Zielona 17, Kraków", 1967),
            new Klient(5,"Irena","Wiśniewska", "Bohaterów 8/11, Pruszków", 1990)                        
            };
            

            List<Produkt> produkty = new List<Produkt>
            {
            new Produkt(1,"Szpachel",12.5,11,3),
            new Produkt(2,"Wiertarka elektryczna",79.99,2,2),
            new Produkt(3,"Tapeta zmywalna",159.99,3,3),
            new Produkt(4,"Wyrzynarka",25.0,2,1),
            new Produkt(5,"Komplet kluczy nasadowych",34.0,1,5),
            new Produkt(6,"Wyrzynarka",25.0,1,4),
            new Produkt(7,"Zestaw pilników",21.50,2,3),
            new Produkt(8,"Szlifierka kątowa",21.50,1,4)
            };


            Print(
                from p in produkty.Distinct(new GenericCompare<Produkt>(t => t.Nazwa))
                select p.Nazwa);
            // ZADANIE 1. 

            // 1.1 Wypisz nazwiska i imiona klientów o parzystych ID mniejszych od 5
            Console.WriteLine("--- 1.1 ------------------------------------");
            var seq = from k in klienci
                      where k.Id % 2 == 0 && k.Id < 5
                      select $" {k.Nazwisko,-13}{k.Imie,-10}";
            Print(seq);
            // Tutaj wpisz rozwiązanie

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" Nowak           Tomasz");
            Console.WriteLine(" Dąbrowski       Waldemar");


            // 1.2 Wypisz unikalne nazwy produktów i wartosci zakupów (nazwy nie mogą się powtarzać) 
            // w porządku od najdroższego do najtańszego. Produkty o tych samych wartościach  
            // powinny być wyświetlane w kolejności alfabetycznej ich nazw 
            Console.WriteLine("--- 1.2 ------------------------------------");

            // Tutaj wpisz rozwiązanie
            Print(from p1 in produkty
                  group p1 by p1.Nazwa
                  into gr
                  select gr.ElementAt(0)
                  into p
                  orderby p.Wartosc descending, p.Nazwa 
                  select $"{p.Nazwa,-25} {p.Wartosc}"                
                );

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" Tapeta zmywalna           159,99");
            Console.WriteLine(" Wiertarka elektryczna     79,99");
            Console.WriteLine(" Komplet kluczy nasadowych 34");
            Console.WriteLine(" Wyrzynarka                25");
            Console.WriteLine(" Szlifierka kątowa         21,5");
            Console.WriteLine(" Zestaw pilników           21,5");
            Console.WriteLine(" Szpachel                  12,5");
                                                                             

            // 1.3 Wypisz nazwy i wartości drugiego oraz trzeciego najdroższego produktu 
            // sprzedanych jednorazowo w liczbie sztuk więcej niż 1
            Console.WriteLine("--- 1.3 ------------------------------------");

            // Tutaj wpisz rozwiązanie
            Print((from p in produkty
                   where p.LiczbaSztuk > 1
                   orderby p.Wartosc descending
                   select $"{p.Nazwa,-25} {p.Wartosc}"
            ).Skip(1).Take(2));

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" Wiertarka elektryczna     79,99");
            Console.WriteLine(" Wyrzynarka                25");



            // ZADANIE 2

            // 2.1  Wypisz sumę długości nazw produktów, które zaczynają się na literę "S"
            Console.WriteLine("--- 2.1 ------------------------------------");

            // Tutaj wpisz rozwiązanie
            Console.WriteLine((from p in produkty
                  where p.Nazwa[0] =='S'
                  select p.Nazwa.Length
                 ).Sum() );

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" 25");


            // 2.2 Wypisz kody, nazwy i liczby sztuk dla produktów, których liczbySztuk są większe 
            // od jeden dla kodów parzystych oraz mniejsze lub równe jeden dla pozostałych kodów.
            Console.WriteLine("--- 2.2 ------------------------------------");

            // Tutaj wpisz rozwiązanie
            Print(from p in produkty
                  where p.Kod % 2 == 0 ? (p.LiczbaSztuk > 1 ? true : false) : (p.LiczbaSztuk <= 1 ? true : false)
                  select $"{p.Kod,-5} {p.Nazwa,-30} {p.LiczbaSztuk}"
                );

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" 2     Wiertarka elektryczna     2");
            Console.WriteLine(" 4     Wyrzynarka                2");
            Console.WriteLine(" 5     Komplet kluczy nasadowych 1");



            // ZADANIE 3

            // 3.1 Wypisz nazwiska i imiona klientów, którzy zakupili produkt o nazwie "Wyrzynarka"

            Console.WriteLine("--- 3.1 ------------------------------------");
            Print(from k in klienci
                  where (from p in produkty where p.Klient == k.Id select p.Nazwa).Any(x => x == "Wyrzynarka")
                  select $"{k.Nazwisko,-20} {k.Imie}"
                );
            // Tutaj wpisz rozwiązanie


            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" Kowalski        Jan");
            Console.WriteLine(" Dąbrowski       Waldemar");


            // 3.2 Znajdź liczbę różnych klientów mających mniej niż 50 lat,
            // którzy zakupili kiedykowliek produkty za mniej niż 50.

            Console.WriteLine("--- 3.2 ------------------------------------");

            // Tutaj wpisz rozwiązanie
            Console.WriteLine((from k in klienci
                               where k.RokUrodzenia > 1964
                               where (from p in produkty where p.Klient == k.Id select p.Wartosc).Any(x => x < 50)
                               select k).Count());

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" 3");



            // ZADANIE 4

            // Dla każdego zakupu, którego dokonał klient młodszy niż 40 lat wypisz: 
            // kod produktu, nazwę produktu oraz cenę jednostkową obliczaną 
            // jako iloraz wartości do liczby produktów. Wyniki uporządkuj rosnąco wg ceny jednostkowej.
            // Zadbaj, aby wyliczanie potrzebnych wyrażeń odbywało się maksymalnie jeden raz. 
 
            
            Console.WriteLine("--- 4 --------------------------------------");
            Print(from p in produkty
                  join k in klienci
                  on p.Klient equals k.Id
                  select (p, 2014 - k.RokUrodzenia)
            into para
                  where para.Item2 < 40
                  let cj = para.p.Wartosc / para.p.LiczbaSztuk
                  orderby cj
                  select $"{para.p.Kod,-5} {para.p.Nazwa,-30} {cj}");
            // Tutaj wpisz rozwiązanie
            //Enumerable.Range(1, 5)[1..3]; 

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" 4   Wyrzynarka                12,5");
            Console.WriteLine(" 5   Komplet kluczy nasadowych 34");
            Console.WriteLine(" 2   Wiertarka elektryczna     39,995");


          
            // ZADANIE 5

            // Dla każdego nazwiska klienta wypisz liczbę jego zakupów (czyli pozycji na liście produkty) 
            // oraz sumę wartości produktów zakupionych przez niego we wszystkich jego zakupach
            Console.WriteLine("--- 5 --------------------------------------");

            // Tutaj wpisz rozwiązanie
            Print(from k in klienci
                  join p in produkty
                  on k.Id equals p.Klient
                  into pgroup
                  select $"{k.Nazwisko,-20} {pgroup.Count(),-5} {pgroup.Sum(x => x.Wartosc)}"
            );
            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine(" Kowalski        1     25");
            Console.WriteLine(" Nowak           1     79,99");
            Console.WriteLine(" Lewandowska     3     193,99");
            Console.WriteLine(" Dąbrowski       2     46,5");
            Console.WriteLine(" Wiśniewska      1     34");

            Console.WriteLine();
        }
    }
}
