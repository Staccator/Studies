using System;

namespace Lab14
{
    class Program
    {
        static void Main(string[] args)
        {
            var library = new Library();

            //UWAGA: w żadnym z etapów nie można używać pętli (for, while, foreach). 
            //Wszystkie zadania muszą być wykonane w technologii LINQ
            // Modyfikować można jedynie plik Library.cs, pozostałe pliki powinny pozostać niezmienione

            //Etap pierwszy - 0.5 punktu
            Console.WriteLine("==== Etap pierwszy ====");

            var booksAndYears = library.GetBookNamesAndPublishingYear();

            foreach (var book in booksAndYears)
                Console.WriteLine(book);

            Console.WriteLine();

            //Etap drugi - 1 punkt
            Console.WriteLine("==== Etap drugi ====");

            var booksForAuthor = library.GetBooksForAuthor("Andrew", "Troelsen");

            foreach (var book in booksForAuthor)
                Console.WriteLine(book.Title);

            Console.WriteLine();

            //Etap trzeci - 1.5 punktu
            Console.WriteLine("==== Etap trzeci ====");

            var genresWithPagesSums = library.GetGenresWithPagesSums();

            foreach (var genre in genresWithPagesSums)
                Console.WriteLine(genre);

            Console.WriteLine();

            //Etap czwraty - 2 punkty
            Console.WriteLine("==== Etap czwarty ====");

            var authorsByOldestBook = library.GetAuthorsSortedByOldestBook();

            foreach (var author in authorsByOldestBook)
                Console.WriteLine(author);

            Console.WriteLine();
        }
    }
}
