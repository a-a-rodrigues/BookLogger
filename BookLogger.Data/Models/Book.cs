using System;
using System.ComponentModel.DataAnnotations;

namespace BookLogger.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int BookMetadataId { get; set; }
        public BookMetadata Metadata { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; } 
        public string? Review { get; set; }
        public DateTime? DateRead { get; set; }
    }
}
