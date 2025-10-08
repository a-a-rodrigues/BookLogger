using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookLogger.Core.Services
{
    public class BookSearchService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://openlibrary.org/search.json";

        public BookSearchService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Searches for books using the Open Library API.
        /// </summary>
        /// <param name="query">The search text entered by the user.</param>
        /// <returns>A list of simplified BookResult objects.</returns>
        public async Task<List<BookResult>> SearchBooksAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<BookResult>();

            try
            {
                var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<BookResult>();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var searchResponse = JsonSerializer.Deserialize<OpenLibraryResponse>(json, options);

                if (searchResponse?.Docs == null)
                    return new List<BookResult>();

                return searchResponse.Docs.Select(doc => new BookResult
                {
                    Title = doc.Title ?? "Untitled",
                    Author = doc.AuthorName?.FirstOrDefault() ?? "Unknown Author",
                    FirstPublishYear = doc.FirstPublishYear ?? 0
                }).ToList();
            }
            catch
            {
                // In a production app, you'd log the error or show a message.
                return new List<BookResult>();
            }
        }
    }

    // --- Supporting Models ---

    public class OpenLibraryResponse
    {
        [JsonPropertyName("docs")]
        public List<OpenLibraryDoc> Docs { get; set; } = new();
    }

    public class OpenLibraryDoc
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author_name")]
        public List<string> AuthorName { get; set; }

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }
    }

    public class BookResult
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int FirstPublishYear { get; set; }
    }
}
