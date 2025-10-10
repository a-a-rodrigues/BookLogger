using BookLogger.Core.Services;
using BookLogger.Data;
using BookLogger.Data.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly BookLoggerContext _context;
        private readonly OpenLibraryService _bookSearchService;
        private readonly HttpClient _httpClient;
        private readonly string _imageFolder;
        private readonly User _currentUser;

        public DashboardViewModel(BookLoggerContext context, User currentUser)
        {
            _context = context;
            _currentUser = currentUser;
            _bookSearchService = new OpenLibraryService();
            _httpClient = new HttpClient();

            Username = _currentUser.Username ?? "Unknown User";

            _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "BookImages");
            if (!Directory.Exists(_imageFolder))
                Directory.CreateDirectory(_imageFolder);

            LoadUserStatistics();

            SearchCommand = new RelayCommand(
                async _ => await PerformSearchAsync(),
                _ => !IsSearching && !string.IsNullOrWhiteSpace(SearchQuery)
            );
        }

        // --- Bindable Properties ---
        private string _username;
        public string Username
        {
            get => _username;
            private set { _username = value; OnPropertyChanged(); }
        }

        public string ProfilePicturePath =>
            _currentUser.ProfilePicturePath ?? "pack://application:,,,/Resources/default-icon.jpg";

        public int BooksCount { get; private set; }
        public int ReviewsCount { get; private set; }
        public int RatingsCount { get; private set; }
        public double AverageRating { get; private set; }

        private string _searchQuery = "";
        private bool _isSearching;
        private ObservableCollection<BookMetadata> _searchResults = new();

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        public ObservableCollection<BookMetadata> SearchResults
        {
            get => _searchResults;
            set { _searchResults = value; OnPropertyChanged(); }
        }

        public bool IsSearching
        {
            get => _isSearching;
            set { _isSearching = value; OnPropertyChanged(); }
        }

        private BookMetadata _selectedBook;
        public BookMetadata SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        // --- Commands ---
        public ICommand SearchCommand { get; }

        // --- User Statistics ---
        private void LoadUserStatistics()
        {
            var userBooks = _context.Books.Where(b => b.UserId == _currentUser.Id).ToList();

            BooksCount = userBooks.Count;
            RatingsCount = userBooks.Count(b => b.Rating.HasValue);
            AverageRating = RatingsCount > 0
                ? userBooks.Where(b => b.Rating.HasValue).Average(b => b.Rating!.Value)
                : 0;

            OnPropertyChanged(nameof(BooksCount));
            OnPropertyChanged(nameof(RatingsCount));
            OnPropertyChanged(nameof(AverageRating));
        }

        // --- Search functionality ---
        private async Task PerformSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;

            IsSearching = true;

            var results = await _bookSearchService.SearchBooksAsync(SearchQuery);

            // Download and attach local image paths
            foreach (var book in results)
            {
                book.CoverUrl = await GetLocalImagePathAsync(book.CoverUrl, book.Title);
            }

            SearchResults = new ObservableCollection<BookMetadata>(results);
            IsSearching = false;
        }

        private async Task<string> GetLocalImagePathAsync(string? imageUrl, string title)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return "pack://application:,,,/Resources/default-book.jpg";

            try
            {
                var safeFileName = string.Join("_", title.Split(Path.GetInvalidFileNameChars())) + ".jpg";
                var filePath = Path.Combine(_imageFolder, safeFileName);

                if (File.Exists(filePath)) return filePath;

                var bytes = await _httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(filePath, bytes);

                return filePath;
            }
            catch
            {
                return "pack://application:,,,/Resources/default-book.jpg";
            }
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
