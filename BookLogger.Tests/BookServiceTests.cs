using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using BookLogger.Data;
using BookLogger.Data.Models;

public class BookServiceTests
{
    private async Task<BookService> GetInMemoryBookServiceAsync()
    {
        var options = new DbContextOptionsBuilder<BookLoggerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
            .Options;

        var context = new BookLoggerContext(options);

        // Add a dummy user for testing
        var user = new User { Id = 1, Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new BookService(context);
    }

    [Fact]
    public async Task AddBookAsync_ShouldAddNewBookAndMetadata()
    {
        var service = await GetInMemoryBookServiceAsync();

        var book = await service.AddBookAsync(1, "1984", "George Orwell", "1234567890", "Dystopian", "cover.jpg");

        Assert.NotNull(book);
        Assert.Equal("1984", book.Metadata.Title);
        Assert.Equal("George Orwell", book.Metadata.Author);
        Assert.Equal(1, book.UserId);
    }

    [Fact]
    public async Task AddBookAsync_ShouldReuseExistingMetadata()
    {
        var service = await GetInMemoryBookServiceAsync();

        // First book
        await service.AddBookAsync(1, "1984", "George Orwell");
        // Second with same metadata
        await service.AddBookAsync(1, "1984", "George Orwell");

        var books = await service.GetBooksByUserAsync(1);
        var metadataCount = books.Select(b => b.Metadata.Id).Distinct().Count();

        Assert.Equal(2, books.Count);
        Assert.Equal(1, metadataCount); // Reused metadata
    }

    [Fact]
    public async Task GetBooksByUserAsync_ShouldReturnOnlyUserBooks()
    {
        var service = await GetInMemoryBookServiceAsync();
        await service.AddBookAsync(1, "Dune", "Frank Herbert");
        await service.AddBookAsync(1, "Dune Messiah", "Frank Herbert");

        var books = await service.GetBooksByUserAsync(1);

        Assert.Equal(2, books.Count);
        Assert.All(books, b => Assert.Equal(1, b.UserId));
    }

    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnCorrectBook()
    {
        var service = await GetInMemoryBookServiceAsync();
        var book = await service.AddBookAsync(1, "Foundation", "Isaac Asimov");

        var found = await service.GetBookByIdAsync(book.Id);

        Assert.NotNull(found);
        Assert.Equal("Foundation", found!.Metadata.Title);
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldModifyBookFields()
    {
        var service = await GetInMemoryBookServiceAsync();
        var book = await service.AddBookAsync(1, "Brave New World", "Aldous Huxley");

        var updated = await service.UpdateBookAsync(book.Id, rating: "5/5", review: "Classic", dateRead: DateTime.Today);
        var fetched = await service.GetBookByIdAsync(book.Id);

        Assert.True(updated);
        Assert.Equal("5/5", fetched!.Rating);
        Assert.Equal("Classic", fetched.Review);
        Assert.NotNull(fetched.DateRead);
    }

    [Fact]
    public async Task DeleteBookAsync_ShouldRemoveBook()
    {
        var service = await GetInMemoryBookServiceAsync();
        var book = await service.AddBookAsync(1, "Neuromancer", "William Gibson");

        var deleted = await service.DeleteBookAsync(book.Id);
        var retrieved = await service.GetBookByIdAsync(book.Id);

        Assert.True(deleted);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task SearchBooksAsync_ShouldReturnMatchingTitlesOrAuthors()
    {
        var service = await GetInMemoryBookServiceAsync();
        await service.AddBookAsync(1, "The Hobbit", "J.R.R. Tolkien");
        await service.AddBookAsync(1, "The Lord of the Rings", "J.R.R. Tolkien");
        await service.AddBookAsync(1, "Dune", "Frank Herbert");

        var tolkienBooks = await service.SearchBooksAsync(1, author: "Tolkien");
        var duneBooks = await service.SearchBooksAsync(1, title: "Dune");

        Assert.Equal(2, tolkienBooks.Count);
        Assert.Single(duneBooks);
    }

    [Fact]
    public async Task AddBookAsync_ShouldThrowException_WhenTitleOrAuthorMissing()
    {
        var service = await GetInMemoryBookServiceAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddBookAsync(1, "", "Some Author")); 

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddBookAsync(1, "Some Title", "")); 
    }

}
