using BookLogger.Core.Services;
using BookLogger.Data;
using BookLogger.Data.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly BookLoggerContext _context;
        private readonly BookSearchService _bookSearchService;
        private User _currentUser;

        // --- Constructor ---
        public DashboardViewModel(BookLoggerContext context, User currentUser)
        {
            _context = context;
            _currentUser = currentUser;
            _bookSearchService = new BookSearchService();

            // Initialize default values
            BooksCount = 0;
            ReviewsCount = 0;
            RatingsCount = 0;
            AverageRating = 0.0;

            LoadUserStatistics();

            SearchCommand = new RelayCommand(async _ => await PerformSearchAsync(), _ => !IsSearching && !string.IsNullOrWhiteSpace(SearchQuery));
        }

        // --- Bindable Properties (Existing) ---
        public string ProfilePicturePath => _currentUser.ProfilePicturePath ?? "pack://application:,,,/Resources/default-icon.jpg";

        public int BooksCount { get; private set; }
        public int ReviewsCount { get; private set; }
        public int RatingsCount { get; private set; }
        public double AverageRating { get; private set; }

        // --- Bindable Properties (New Search Integration) ---
        private string _searchQuery = "";
        private bool _isSearching;
        private ObservableCollection<BookResult> _searchResults = new();

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BookResult> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                _isSearching = value;
                OnPropertyChanged();
            }
        }

        // --- Commands ---
        public ICommand SearchCommand { get; }

        // --- Load or compute statistics (from your version) ---
        private void LoadUserStatistics()
        {
            var userBooks = _context.Books.Where(b => b.UserId == _currentUser.Id).ToList();

            BooksCount = userBooks.Count;
            RatingsCount = userBooks.Count(b => b.Rating.HasValue);
            AverageRating = RatingsCount > 0
                ? userBooks.Where(b => b.Rating.HasValue).Average(b => b.Rating!.Value)
                : 0;

            // Optional: compute ReviewsCount here later

            OnPropertyChanged(nameof(BooksCount));
            OnPropertyChanged(nameof(RatingsCount));
            OnPropertyChanged(nameof(AverageRating));
        }

        // --- Search functionality (new) ---
        private async Task PerformSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            IsSearching = true;

            var results = await _bookSearchService.SearchBooksAsync(SearchQuery);

            SearchResults = new ObservableCollection<BookResult>(results);

            IsSearching = false;
        }

        // --- INotifyPropertyChanged implementation ---
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
