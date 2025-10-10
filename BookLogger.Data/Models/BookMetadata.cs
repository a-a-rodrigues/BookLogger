using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookLogger.Data.Models
{
    public class BookMetadata
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string ISBN { get; set; } = "";  // <-- note uppercase
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public int? FirstPublishYear { get; set; }

        public List<Book> UserBooks { get; set; } = new List<Book>();
    }
}
