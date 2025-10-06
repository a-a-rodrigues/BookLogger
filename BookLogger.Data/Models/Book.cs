using System;
using System.ComponentModel.DataAnnotations;

namespace BookLogger.Data.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        // Consider using int (1-5) for Rating for easier queries/sorting.
        // Keeping string is fine if you want flexible input (e.g., "4/5", "Liked it").
        public string Rating { get; set; }

        public string Review { get; set; }

        // Consider making this nullable if a book may be in the list but not yet read:
        public DateTime? DateRead { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
