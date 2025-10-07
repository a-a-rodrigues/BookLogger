using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BookLogger.Core.Services;
using BookLogger.Data;
using BookLogger.Data.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class BookWorkflowTests
{
    private async Task<(BookLoggerContext, BookService, BookJsonService, OpenLibraryService)> GetSetupAsync()
    {
        var options = new DbContextOptionsBuilder<BookLoggerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new BookLoggerContext(options);
        var bookService = new BookService(context);
        var jsonService = new BookJsonService();
        var apiService = new OpenLibraryService();

        // Ensure DB exists and is empty
        await context.Database.EnsureCreatedAsync();
        return (context, bookService, jsonService, apiService);
    }

    [Fact]
    public async Task FullWorkflow_ShouldFetch_Add_Save_Export_Import()
    {
        // Arrange
        var (context, bookService, jsonService, apiService) = await GetSetupAsync();
        var user = new User { Username = "tester", PasswordHash = "hashed123", Email = "augie@gmail.com"};
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Step 1: Fetch a book from OpenLibrary
        var results = await apiService.SearchBooksAsync("Pride and Prejudice", "Austen");

        Assert.NotEmpty(results); // ensure API is live
        var metadata = results[0];

        // Step 2: Add that book to DB
        var book = await bookService.AddBookAsync(
            user.Id,
            metadata.Title,
            metadata.Author,
            metadata.ISBN,
            metadata.Genre,
            metadata.CoverUrl
        );

        Assert.NotNull(book);
        Assert.Equal(metadata.Title, book.Metadata.Title);

        // Step 3: Export user’s books
        var filePath = Path.Combine(Path.GetTempPath(), $"export_{Guid.NewGuid()}.json");
        await jsonService.ExportBooksAsync(new List<Book> { book }, filePath);

        Assert.True(File.Exists(filePath));

        // Step 4: Import and verify
        var imported = await jsonService.ImportBooksAsync(filePath);
        Assert.Single(imported);
        Assert.Equal(metadata.Title, imported[0].Metadata.Title);

        // Cleanup
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
