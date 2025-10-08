using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookLogger.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string? ProfilePicturePath { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
