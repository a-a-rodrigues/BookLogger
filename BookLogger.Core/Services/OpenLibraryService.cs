using BookLogger.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class OpenLibraryService
{
    private readonly HttpClient _client;
    private const string BaseUrl = "https://openlibrary.org/search.json";
    private const string CoverBaseUrl = "https://covers.openlibrary.org/b/id/";

    private readonly JsonSerializerOptions _options;

    public OpenLibraryService(HttpClient? client = null)
    {
        _client = client ?? new HttpClient();
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    /// <summary>
    /// Searches Open Library and returns a list of BookMetadata objects.
    /// Fully populates Title, Author, ISBN, Genre, and CoverUrl.
    /// </summary>
    public async Task<List<BookMetadata>> SearchBooksAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<BookMetadata>();

        var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}";
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<OpenLibraryResponse>(json, _options);

        if (data?.Docs == null) return new List<BookMetadata>();

        return data.Docs.Select(doc => new BookMetadata
        {
            Title = doc.Title ?? "Untitled",
            Author = doc.AuthorName?.FirstOrDefault() ?? "Unknown Author",
            ISBN = doc.ISBN?.FirstOrDefault() ?? "N/a",
            Genre = doc.Subjects?.FirstOrDefault() ?? "N/a",
            FirstPublishYear = doc.FirstPublishYear,
            CoverUrl = doc.CoverId.HasValue ? $"{CoverBaseUrl}{doc.CoverId}-L.jpg" : null
        }).ToList();
    }

    // --- Internal JSON mapping classes ---
    private class OpenLibraryResponse
    {
        [JsonPropertyName("docs")]
        public List<OpenLibraryDoc> Docs { get; set; } = new();
    }

    private class OpenLibraryDoc
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("author_name")]
        public List<string>? AuthorName { get; set; }

        [JsonPropertyName("isbn")]
        public List<string>? ISBN { get; set; }

        [JsonPropertyName("cover_i")]
        public int? CoverId { get; set; }
        
        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }

        [JsonPropertyName("subject")]
        public List<string>? Subjects { get; set; }
    }
}
