using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BookLogger.Data.Models;

namespace BookLogger.Core.Services
{
    public class BookJsonService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public async Task ExportBooksAsync(List<Book> books, string filePath)
        {
            var json = JsonSerializer.Serialize(books, _options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<Book>> ImportBooksAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found: ", filePath);

            var json = await File.ReadAllTextAsync(filePath);
            var books = JsonSerializer.Deserialize<List<Book>>(json, _options);
            return books ?? new List<Book>();
        }
    }
}
