using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BookLogger.Data.Models;

public class OpenLibraryService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _options;

    public OpenLibraryService(HttpClient? client = null)
    {
        _client = client ?? new HttpClient();
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive  = true };
    }

    public async Task<List<BookMetadata>> SearchBooksAsync(string title, string? author = null)
    {
        var url = $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(title)}";
        if (!string.IsNullOrWhiteSpace(author))
            url += $"&author={Uri.EscapeDataString(author)}";

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<OpenLibraryResponse>(json, _options);

        var results = new List<BookMetadata>();
        if (data?.Docs == null) return results;

        foreach (var doc in data.Docs) 
        {
            results.Add(new BookMetadata
            {
                Title = doc.Title ?? "Unkown",
                Author = doc.AuthorName?.FirstOrDefault() ?? "Unknown",
                ISBN = doc.Isbn?.FirstOrDefault(),
                CoverUrl = doc.CoverI != null
                    ? $"https://covers.openlibrary.org/b/id/{doc.CoverI}-L.jpg"
                    : null,
                Genre = doc.Subject?.FirstOrDefault()
            });
        }

        return results;
    }

    public class OpenLibraryResponse
    {
        public List<OpenLibraryDoc>? Docs { get; set; }
    }

    public class OpenLibraryDoc
    {
        public string? Title { get; set; }
        public List<string>? AuthorName { get; set; }
        public List<string>? Isbn { get; set; }
        public int? CoverI { get; set; }
        public List<string>? Subject { get; set; }
    }

}