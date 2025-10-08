using BookLogger.Core.Services;
using BookLogger.Data;
using BookLogger.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookLogger.Tests
{
    public class AddBookTests
    {
        private DbContextOptions<BookLoggerContext> CreateFileOptions()
        {
            // Path to your real database file
            var dbPath = @"C:/Users/Augie/Documents/Github/BookLogger/BookLogger.Data/booklogger.db";
            return new DbContextOptionsBuilder<BookLoggerContext>()
                .UseSqlite($"Filename={dbPath}")
                .Options;
        }

        [Fact]
        public async Task AddBook_AssociatesWithUser()
        {
            var options = CreateFileOptions();

            // Ensure migrations are applied
            using (var initContext = new BookLoggerContext(options))
            {
                await initContext.Database.MigrateAsync();
            }

            using (var context = new BookLoggerContext(options))
            {
                var authService = new AuthService(context);
                var bookService = new BookService(context);

                // Ensure the user exists
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == 1);
                if (user == null)
                {
                    user = await authService.RegisterAsync("testuser", "testuser@example.com", "Password123", "Password123");
                }

                // Add book for user
                var newBook = await bookService.AddBookAsync(
                    user.Id,
                    "Dune",
                    "Frank Herbert",
                    isbn: "9780441013593",
                    genre: "Science Fiction"
                );

                // Fetch user's books to verify
                var userBooks = await bookService.GetBooksByUserAsync(user.Id);

                Assert.Contains(userBooks, b => b.Id == newBook.Id);
                Assert.Equal("Dune", newBook.Metadata.Title);
                Assert.Equal(user.Id, newBook.UserId);
            }
        }
    }
}
