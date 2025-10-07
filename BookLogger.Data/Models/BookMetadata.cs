using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLogger.Data.Models
{
    public class BookMetadata
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public string? ISBN { get; set; }
        public string? CoverUrl { get; set; }
        public string? Genre { get; set; }

        public List<Book> UserBooks { get; set; } = new List<Book>();
    }
}
