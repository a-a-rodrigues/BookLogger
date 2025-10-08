using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookLogger.Data;
using BookLogger.Data.Models;
using BookLogger.Core.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class FullWorkflowIntegrationTests
{
    private DbContextOptions<BookLoggerContext> CreateFileOptions()
    {
        // Use your real database file path
        var dbPath = @"C:/Users/Augie/Documents/Github/BookLogger/BookLogger.Data/booklogger.db";
        return new DbContextOptionsBuilder<BookLoggerContext>()
            .UseSqlite($"Filename={dbPath}")
            .Options;
    }

    [Fact]
    public async Task FullWorkflowTestAsync()
    {
        var options = CreateFileOptions();

        // --- STEP 0: Ensure database and tables exist ---
        using (var initContext = new BookLoggerContext(options))
        {
            await initContext.Database.MigrateAsync(); // applies any pending migrations
        }

        // --- STEP 1: Run the workflow ---
        using (var context = new BookLoggerContext(options))
        {
            var authService = new AuthService(context);
            var bookService = new BookService(context);
            var dbService = new DatabaseService(context);

            // Register user
            var username = "integrationUser";
            var email = "test@example.com";

            // Cleanup in case user already exists
            var existingUser = await context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (existingUser != null)
            {
                context.Users.Remove(existingUser);
                await context.SaveChangesAsync();
            }

            var user = await authService.RegisterAsync(username, email, "Password1", "Password1");
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);

            // Add a book
            var book = await bookService.AddBookAsync(user.Id, "The Hobbit", "J.R.R. Tolkien", "978-0547928227", "Fantasy", null);
            Assert.NotNull(book);
            Assert.Equal(user.Id, book.UserId);
            Assert.NotNull(book.Metadata);
            Assert.Equal("The Hobbit", book.Metadata.Title);

            // Query books by user
            var userBooks = await bookService.GetBooksByUserAsync(user.Id);
            Assert.Single(userBooks);

            // Update the book
            bool updated = await bookService.UpdateBookAsync(book.Id, rating: 5, review: "Classic fantasy", dateRead: DateTime.Today);
            Assert.True(updated);

            var updatedBook = await bookService.GetBookByIdAsync(book.Id);
            Assert.Equal(5, updatedBook!.Rating);
            Assert.Equal("Classic fantasy", updatedBook.Review);

            // Reset password and login
            await authService.ResetPasswordAsync(user.Username, "NewPassword1");
            var loggedInUser = await authService.LoginAsync(user.Username, "NewPassword1");
            Assert.NotNull(loggedInUser);

            // Export books to JSON
            var jsonService = new BookJsonService();
            var tempFile = Path.GetTempFileName();
            await jsonService.ExportBooksAsync(userBooks, tempFile);
            Assert.True(File.Exists(tempFile));

            // Import books from JSON
            var importedBooks = await jsonService.ImportBooksAsync(tempFile);
            Assert.Single(importedBooks);
            Assert.Equal("The Hobbit", importedBooks.First().Metadata.Title);

            // Delete the book
            bool deleted = await bookService.DeleteBookAsync(book.Id);
            Assert.True(deleted);

            var allBooks = await dbService.GetAllBooksAsync();

            File.Delete(tempFile);
        }
    }
}
