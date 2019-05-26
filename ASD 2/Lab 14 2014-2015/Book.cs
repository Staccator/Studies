using System;

namespace Lab14
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public int Pages { get; set; }

        public DateTime PublishingDate { get; set; }
    }
}
