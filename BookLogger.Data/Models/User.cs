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

        // If you have no existing user rows in the DB, you can add [Required] here.
        // If you already have data, leave off [Required] and make the column nullable first.
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
