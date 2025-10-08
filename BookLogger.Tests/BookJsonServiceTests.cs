using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BookLogger.Core.Services;
using BookLogger.Data.Models;
using Xunit;

public class BookJsonServiceTests
{
    [Fact]
    public async Task ExportAndImportBooksAsync_ShouldPreserveData()
    {
        // Arrange
        var service = new BookJsonService();
        var tempFile = Path.Combine(Path.GetTempPath(), $"books_{Guid.NewGuid()}.json");

        var books = new List<Book>
        {
            new Book
            {
                Id = 1,
                UserId = 1,
                Rating = 5,
                Review = "Excellent read.",
                DateRead = DateTime.Now,
                Metadata = new BookMetadata
                {
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "9780451524935",
                    Genre = "Dystopian"
                }
            }
        };

        try
        {
            // Act
            await service.ExportBooksAsync(books, tempFile);
            var importedBooks = await service.ImportBooksAsync(tempFile);

            // Assert
            Assert.Single(importedBooks);
            Assert.Equal("1984", importedBooks[0].Metadata.Title);
            Assert.Equal("George Orwell", importedBooks[0].Metadata.Author);
            Assert.Equal(5, importedBooks[0].Rating);
            Assert.Equal("Excellent read.", importedBooks[0].Review);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ImportBooksAsync_ShouldThrow_WhenFileMissing()
    {
        var service = new BookJsonService();
        var invalidPath = Path.Combine(Path.GetTempPath(), "nonexistent_file.json");

        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await service.ImportBooksAsync(invalidPath);
        });
    }
}
