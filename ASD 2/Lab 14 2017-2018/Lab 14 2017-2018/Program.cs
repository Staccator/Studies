using System;
using System.Collections.Generic;

namespace lab14b
{
class Program
    {
        static void Main(string[] args)
            {
            Zoo zoo1 = new Zoo(new List<Zwierze>
                {
                new Zwierze("Żubr", "Polska", false, 32),
                new Zwierze("Zebra", "Botswana", false, 12),
                new Zwierze("Nietoperz", "Mołdawia", true, 1642.5),
                new Zwierze("Słoń", "Indie", false, 60),
                new Zwierze("Bizon", "USA", false, 13)
                });

            Zoo zoo2 = new Zoo(new List<Zwierze>
                {
                new Zwierze("Koń", "Polska", false, 9),
                new Zwierze("Kot", "Polska", false, 3),
                new Zwierze("Bocian", "Polska", true, 11),
                new Zwierze("Bocian", "Polska", true, 10),
                new Zwierze("Bocian", "Polska", true, 1.5)
                });

            Zoo zoo3 = new Zoo(new List<Zwierze>
                {
                new Zwierze("Jednorożec", "Polska", false, 28),
                new Zwierze("Alikorn", "Norwegia", true, 123),
                new Zwierze("Centaur", "Grecja", false, 1024),
                new Zwierze("Faun", "Narnia", false, 3)
                });


            Console.Out.WriteLine("Etap 1");
            Console.Out.WriteLine("Zoo1");
            zoo1.StatystykiZoo();
            Console.Out.WriteLine("Zoo2");
            zoo2.StatystykiZoo();
            Console.Out.WriteLine("Zoo3");
            zoo3.StatystykiZoo();
            Console.Out.WriteLine();


            Console.Out.WriteLine("Etap 2");
            Console.Out.WriteLine("Zoo1.NajmlodszeZwierzeStarszeOd(Nietoperz): {0}", zoo1.NajmlodszeZwierzeStarszeOd("Nietoperz"));
            Console.Out.WriteLine("Zoo1.NajmlodszeZwierzeStarszeOd(Zebra): {0}", zoo1.NajmlodszeZwierzeStarszeOd("Zebra"));
            Console.Out.WriteLine("Zoo2.NajmlodszeZwierzeStarszeOd(Bocian): {0}", zoo2.NajmlodszeZwierzeStarszeOd("Bocian"));

            Console.Out.WriteLine("Zoo1.CzySaMlodeZwierzeta: {0}", zoo1.CzySaMlodeZwierzeta());
            Console.Out.WriteLine("Zoo2.CzySaMlodeZwierzeta: {0}", zoo2.CzySaMlodeZwierzeta());
            Console.Out.WriteLine("Zoo3.CzySaMlodeZwierzeta: {0}", zoo3.CzySaMlodeZwierzeta());
            Console.Out.WriteLine();

            Console.Out.WriteLine("Etap 3");
            Console.Out.WriteLine("Zoo1.Szukajwedlugwieku(1000,2000): {0}", zoo1.SzukajWedlugWieku(1000, 2000));
            Console.Out.WriteLine("Zoo1.Szukajwedlugwieku(100,200): {0}", zoo1.SzukajWedlugWieku(100, 200));
            Console.Out.WriteLine("Zoo3.Szukajwedlugwieku(3,5): {0}", zoo1.SzukajWedlugWieku(3, 5));

            Console.Out.WriteLine("Zoo1.SredniWiek: {0}", zoo1.SredniWiek());
            Console.Out.WriteLine("Zoo1.SredniaWiekuStarszychNiz(50): {0}", zoo1.SredniaWiekuStarszychNiz(50));
            Console.Out.WriteLine("Zoo2.SredniaWiekuStarszychNiz(50): {0}", zoo2.SredniaWiekuStarszychNiz(50));
            Console.Out.WriteLine();


            Console.Out.WriteLine("Etap 4");
            var kraje = new List<Kraj>{
                    new Kraj("Polska", "Europa"),
                    new Kraj("Botswana", "Afryka"),
                    new Kraj("Mołdawia", "Europa"),
                    new Kraj("Inidie", "Azja"),
                    new Kraj("USA", "Ameryka"),
                    new Kraj("Norwegia", "Europa"),
                    new Kraj("Grecja", "Europa"),
                    new Kraj("Narnia", "Narnia")

                    };

            Console.Out.WriteLine("Zoo1");
            foreach (var s in zoo1.PochodzenieZwierzat(kraje))
            {
                Console.Out.WriteLine(s);
            }
            Console.Out.WriteLine("Zoo3");
            foreach (var s in zoo3.PochodzenieZwierzat(kraje))
            {
                Console.Out.WriteLine(s);
            }

            // Console.WriteLine(Math.Round(2342.35235/10, 0)*10);

            Console.Out.WriteLine("Zoo1: suma długości wg wieku");
            IEnumerable<(double, int)> pairs = zoo1.SumaDlugosciNazwWedlugWieku();

            foreach (var p in pairs)
            {
                Console.Out.WriteLine(p);
            }

            Console.Out.WriteLine("Zoo2: suma długości wg wieku");


            foreach (var p in zoo2.SumaDlugosciNazwWedlugWieku())
            {
                Console.Out.WriteLine(p);
            }
            Console.Out.WriteLine();


            Console.Out.WriteLine("Etap 5");
            //var pairs2 = zoo2.DwaNajmlodsze();
            //foreach(var p in pairs2)
           // {
             //   Console.Out.WriteLine(p);
           // }
            var (n1, n2) = zoo2.DwaNajmlodsze();
            Console.Out.WriteLine("Dwa najmłodsze w zoo2: {0}, {1}", n1, n2);

            Console.Out.Write("Zoo1 -- wszystkie litery z nazw: ");
            foreach (var c in zoo1.WszystkieLiteryZNazw())
            {
                Console.Out.Write(c);
            }

            Console.Out.WriteLine();

        }
    }
}
