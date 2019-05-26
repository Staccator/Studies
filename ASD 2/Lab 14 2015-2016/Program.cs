using System;
using System.Collections.Generic;
using System.Linq;

namespace Linq
{
    class Program
    {
        static Random r = new Random(123);

        private static List<double> generateCoordinates(int n)
        {
            List<double> ret = new List<double>();
            for (int i = 0; i < n / 2; i++)
            {     
                ret.Add(r.NextDouble()*100);
            }
            return ret;
        }

        public static IEnumerable<ulong> FibonacciNumbers()
        {
            yield return 0;
            yield return 1;

            ulong previous = 0, current = 1;
            while (true)
            {
                ulong next = checked(previous + current);
                yield return next;
                previous = current;
                current = next;

            }
        }


        static void Main(string[] args)
        {
            //Etap 1
            const int pointsCount = 50;
            Console.WriteLine("-----------1.1 i 1.2----------");
            List<double> xCoordinates = generateCoordinates(pointsCount);
            List<double> yCoordinates = generateCoordinates(pointsCount);
            
            //1.1 Wygeneruj liste punktow na podstawie tablic ze wspolrzednymi, 0p (punktowane, gdy zrobione razem z 1.2)
            List<Point2D> points2D = xCoordinates.Join(yCoordinates,x => 1,y => 1,(x,y) => new Point2D(x,y)).ToList(); //tutaj ma sie znalezc wynik

            double minX = points2D.Min(a => a.X); //tutaj ma sie znalezc wynik
            double maxX = points2D.Max(a => a.X); //tutaj ma sie znalezc wynik
            double minY = points2D.Min(a => a.Y); //tutaj ma sie znalezc wynik
            double maxY = points2D.Max(a => a.Y); //tutaj ma sie znalezc wynik
            
            //1.2 oblicz minimum bounding box (minimalny prostokat okalajacy), 0.5p

            Console.WriteLine("minX: {0:F2}, maxX: {1:F2}, minY: {2:F2}, maxY: {3:F2}", minX, maxX, minY, maxY);
            Console.WriteLine("Powinno być:");
            Console.WriteLine("minX: 0,22, maxX: 98,46, minY: 7,22, maxY: 93,17");
            Console.WriteLine("-------1.3---------");


            int n = points2D.Count;
            for (int i = 0; i < n; i++)
                points2D.Add(new Point2D(2 * points2D[i].X, 2 * points2D[i].Y));

            //1.3 sortuj najpierw po tangensie, jaki tworzy kat wektora z osia X (rosnaco), a nastepnie po odleglosci od poczatku ukladu wspolrzednych (malejaco) 0.5p
            List<Point2D> points2DOrderedByTangens = points2D.OrderBy(x => x.Y/x.X ).ThenByDescending(x => Math.Sqrt(x.X * x.X + x.Y * x.Y)).ToList(); //tutaj ma sie znalezc wynik
           // foreach (var p in points2DOrderedByTangens)
           //    Console.WriteLine("({0:F2}, {1:F2})", p.X, p.Y);
            Console.WriteLine("-------1.4------");
            
            //1.4 Wybierz punkty z ciągu Fibonacciego (ciąg nieskonczony, zwracany przez funkcje FibonacciNumbers()), 
            //ktore sa wieksze niz 100, mniejsze niz 1000 oraz sa nieparzyste 0.5p
            var fn = FibonacciNumbers().TakeWhile(x => x<1000).Where(x => x>100 && x<1000 && x%2==1); //tutaj ma sie znalezc wynik
            
            foreach (var num in fn)
                Console.WriteLine("{0}, ", num);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("233");
            Console.WriteLine("377");
            Console.WriteLine("987");

            //ETAP 2

            Console.WriteLine("-------2.1------");
            List<Student> students = new List<Student>
            {
                new Student("Andrzej", "Nowak", "94092905436", "634141", true),
                new Student("Adrian", "Kowalski", "91092905235", "634142", true),
                new Student("Michal", "Lewandowski", "90092905135", "634143", true),
                new Student("Weronika", "Zdon", "89112805436", "634146", false),
                new Student("Andrzej", "Dabrowski", "89092905435", "634144", true),
                new Student("Anna", "Wisniewska", "89102905438", "634145", false),
                new Student("Maria", "Nowakowska", "89112805438", "634147", false),
            };

            List<Obywatel> citizens = new List<Obywatel>
            {
                new Obywatel("Andrzej","Nowak","94092905436", true,false,true,5000),
                new Obywatel("Krzysztof","Wrobel","92031905435", true,true,false,2000),
                new Obywatel("Andrianna","Wrobel","94090905435", true,false,true,1000),
                new Obywatel("Maria","Nowakowska","89112805438", true,false,true,15000),
                new Obywatel("Karolina","Wyszynska","89102905438", false,false,false,7000),
                new Obywatel("Anna","Wyszynska","96110905436", true,false,true,6000),
            };

            List<GradeInfo> grades = new List<GradeInfo>
            {
                new GradeInfo("634141", 3.0, "AiSD"),
                new GradeInfo("634141", 5.0, "MD2"),
                new GradeInfo("634141", 4.0, "C++"),
                
                new GradeInfo("634142", 5.0, "AiSD"),
                new GradeInfo("634142", 5.0, "MD2"),
                new GradeInfo("634142", 4.0, "C++"),

                new GradeInfo("634143", 3.5, "AiSD"),
                new GradeInfo("634143", 5.0, "MD2"),
                new GradeInfo("634143", 4.5, "C++"),

                new GradeInfo("634146", 4.0, "AiSD"),
                new GradeInfo("634146", 5.0, "MD2"),
                new GradeInfo("634146", 5.0, "C++"),

                new GradeInfo("634144", 2.0, "AiSD"),
                new GradeInfo("634144", 5.0, "MD2"),
                new GradeInfo("634144", 5.0, "C++"),

                new GradeInfo("634145", 2.0, "AiSD"),
                new GradeInfo("634145", 2.0, "MD2"),
                new GradeInfo("634145", 3.0, "C++"),
                
                new GradeInfo("634147", 5.0, "AiSD"),
                new GradeInfo("634147", 5.0, "MD2"),
                new GradeInfo("634147", 5.0, "C++"),
            };

            List<PhoneNumber> phones = new List<PhoneNumber>
            {
                new PhoneNumber("94092905436","111-111-111"),
                new PhoneNumber("94092905436","111-111-222"),

                new PhoneNumber("92031905435","991-111-111"),
                new PhoneNumber("92031905435","191-111-222"),

                new PhoneNumber("94090905435","111-111-111"),
                new PhoneNumber("94090905435","188-111-222"),
                new PhoneNumber("94090905435","111-134-222"),
                new PhoneNumber("94090905435","111-124-222"),

                new PhoneNumber("89112805438","111-111-111"),
                
                new PhoneNumber("89102905438","111-177-111"),
                new PhoneNumber("89102905438","111-551-222"),
                new PhoneNumber("89102905438","111-561-222"),

                new PhoneNumber("96110905436","111-211-111"),
                new PhoneNumber("96110905436","111-311-222"),
                new PhoneNumber("96110905436","111-341-222"),
            };

            //2.1 wybierz osoby (imie i nazwisko), ktore sa w obu bazach danych (studenci i obywatele) 0.5p
            var seq1 = from s in students
                       from c in citizens
                       where s.PESEL == c.PESEL
                       select new { Imie = s.Name, Nazwisko = s.Surname }
                       ;
            foreach (var os in seq1)
                Console.WriteLine("{0} {1}", os.Imie, os.Nazwisko);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("Andrzej Nowak");
            Console.WriteLine("Anna Wyszynska");
            Console.WriteLine("Maria Nowakowska");
            Console.WriteLine("------2.2-------");

            //2.2 Policz ile numerow telefonow ma kazdy obywatel, wybierz 3 obywateli z najwieksza liczba numerow telefonow, 0.5p
            //zwracana klasa powinna zawierac imie, nazwisko, pesel i liczbe telefonow
            var seq2 = from c in citizens
                       select (c,phones.Count(x => x.PESEL == c.PESEL))
                       into pair
                       orderby pair.Item2 descending
                       select new {Imie = pair.Item1.Name,Nazwisko = pair.Item1.Surname,PESEL = pair.Item1.PESEL,Count = pair.Item2 }
                      ;
            seq2 = seq2.Take(3);

            foreach (var os in seq2)
                Console.WriteLine("{0} {1} {2} {3}", os.Imie, os.Nazwisko, os.PESEL, os.Count);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("Adrianna Wrobel 94090905435 4");
            Console.WriteLine("Karolina Wyszynska 89102905438 3");
            Console.WriteLine("Anna Wyszynska 96110905436 3");
            Console.WriteLine("------2.3--------");

            //2.3 Na podstawie numeru PESEL wybierz obywateli, ktorzy maja rocznikowo 23 lata, 0.5p
            var seq3 = from c in citizens
                       let tmp = long.Parse(c.PESEL) / 10000_00000 
                       where tmp <= 93
                       select new {Imie = c.Name,Nazwisko = c.Surname,PESEL = c.PESEL,wiek = 116 - tmp}
                       ;

            foreach (var os in seq3)
                Console.WriteLine("{0} {1} {2} {3}", os.Imie, os.Nazwisko, os.PESEL, os.wiek);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("Krzysztof Wrobel 92031905435 24");
            Console.WriteLine("Maria Nowakowska 89112805438 27");
            Console.WriteLine("Karolina Wyszynska 89102905438 27");
            Console.WriteLine("-------2.4--------");

            // 2.4 Stworz srednia ocen w rozbiciu na płcie. Oznacza to, 
            //ze nalezy policzyc srednia ocen osobno dla kobiet, a osobno dla mezczyzn, 0.5p
            var seq4 = from s in students
                       group s by s.Gender
                       into gendergroup
                       select new {Gender = gendergroup.Key,
                       Average = (from x in gendergroup
                                 select (from g in grades where x.IndexNo == g.IndexNo select g.Grade).Average() ).Average()
                       }
                       ;

            foreach (var os in seq4)
                Console.WriteLine("{0} {1} ", os.Gender ? "Mezczyzni" : "Kobiety", os.Average);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("Mezczyzni 4.25");
            Console.WriteLine("Kobiety 4");
            Console.WriteLine("-----2.5--------");

            //2.5 Wybierz numery telefonow osob, ktore zarabiaja wiecej (nieostro) niz 5000. Posortuj po zarobkach malejaco, wybierz pierwsze 5. 
            //Na liscie moze znalezc sie wiele telefonow do tego samego czlowieka 0.5p
            var seq5 = from c in citizens
                       where c.Salary >=5000
                       orderby c.Salary descending
                       let numbers = from p in phones where p.PESEL == c.PESEL select p.Phone
                       from nr in numbers
                       select new {c.Salary,Phone = nr}
                       ;
            seq5 = seq5.Take(5);
            foreach (var os in seq5)
                Console.WriteLine("{0} {1} ", os.Salary, os.Phone);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("15000 111-111-111");
            Console.WriteLine("7000 111-177-111");
            Console.WriteLine("7000 111-551-222");
            Console.WriteLine("7000 111-561-222");
            Console.WriteLine("6000 111-211-111");
            Console.WriteLine("------2.6---------");

            //2.6 Wybierz osoby, ktore maja co najmniej jedna ocene 2.0, a nastepnie wypisz ich zarobki. Jesli osoby nie ma w bazie danych obywateli, przypisz 0 w zarobkach. 1p
            var seq6 = from s in students
                       where (from g in grades where g.IndexNo == s.IndexNo select g.Grade).Any(x => x == 2.0)
                       let o = (from c in citizens where c.PESEL == s.PESEL select c).SingleOrDefault()
                       select new {s.Name,s.Surname,Salary = o==null? 0.0 : o.Salary}
                      ;
            foreach (var os in seq6)
                Console.WriteLine("{0} {1} {2}", os.Name, os.Surname, os.Salary);

            Console.WriteLine();
            Console.WriteLine("Powinno być:");
            Console.WriteLine("Andrzej Dabrowski 0");
            Console.WriteLine("Anna Wisniewska 7000");
            Console.WriteLine("---------------------");
        }

        
    }
}
