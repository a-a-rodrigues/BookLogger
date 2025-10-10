using BookLogger.Core.Services;
using BookLogger.Data;
using BookLogger.Data.Models;
using BookLogger.Desktop.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public readonly BookLoggerContext _context;
        private readonly OpenLibraryService _bookSearchService;
        private readonly HttpClient _httpClient;
        private readonly string _imageFolder;
        public readonly User _currentUser;
        public bool IsAdmin => _currentUser.Id == 1;

        public ICommand ChangeProfilePictureCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ResetDatabaseCommand { get; }
        public ICommand LogoutCommand { get; }

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

            RefreshUserStatistics();
            LoadUserBooks();

            SearchCommand = new RelayCommand(
                async _ => await PerformSearchAsync(),
                _ => !IsSearching && !string.IsNullOrWhiteSpace(SearchQuery)
            );

            ChangeProfilePictureCommand = new RelayCommand(_ => ChangeProfilePicture());

            ResetDatabaseCommand = new RelayCommand(_ =>
            {
                var _dbPath = @"C:/Users/Augie/Documents/Github/BookLogger/BookLogger.Data/booklogger.db";

                var result = MessageBox.Show(
                    "Are you sure you want to delete all data? This action cannot be undone.",
                    "Confirm Reset",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    _context?.Dispose();

                    var options = new DbContextOptionsBuilder<BookLoggerContext>()
                        .UseSqlite($"Filename={_dbPath}")
                        .Options;

                    using (var newContext = new BookLoggerContext(options))
                    {
                        newContext.Database.EnsureDeleted();
                        newContext.Database.EnsureCreated();
                    }

                    var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    if (mainWindow != null)
                        mainWindow.MainFrame.Navigate(new LoginView());

                    MessageBox.Show("Database has been successfully reset.", "Reset Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to reset database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            LogoutCommand = new RelayCommand(_ =>
            {
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                    mainWindow.MainFrame.Navigate(new LoginView());
            });
        }

        // --- Bindable Properties ---
        private string _username;
        public string Username
        {
            get => _username;
            private set { _username = value; OnPropertyChanged(); }
        }

        public string ProfilePicturePath =>
            !string.IsNullOrWhiteSpace(_currentUser.ProfilePicturePath) && File.Exists(_currentUser.ProfilePicturePath)
                ? _currentUser.ProfilePicturePath
                : "pack://application:,,,/Resources/default-icon.jpg";

        private int _booksCount;
        public int BooksCount
        {
            get => _booksCount;
            private set { _booksCount = value; OnPropertyChanged(); }
        }

        private int _reviewsCount;
        public int ReviewsCount
        {
            get => _reviewsCount;
            private set { _reviewsCount = value; OnPropertyChanged(); }
        }

        private int _ratingsCount;
        public int RatingsCount
        {
            get => _ratingsCount;
            private set { _ratingsCount = value; OnPropertyChanged(); }
        }

        private double _averageRating;
        public double AverageRating
        {
            get => _averageRating;
            private set { _averageRating = value; OnPropertyChanged(); }
        }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set { _isSearching = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BookMetadata> _userBooks;
        public ObservableCollection<BookMetadata> UserBooks
        {
            get => _userBooks;
            set { _userBooks = value; OnPropertyChanged(); }
        }

        private BookMetadata _selectedUserBook;
        public BookMetadata SelectedUserBook
        {
            get => _selectedUserBook;
            set { _selectedUserBook = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BookMetadata> _searchResults = new();
        public ObservableCollection<BookMetadata> SearchResults
        {
            get => _searchResults;
            set { _searchResults = value; OnPropertyChanged(); }
        }

        private BookMetadata _selectedBook;
        public BookMetadata SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        // --- Methods ---
        public void RefreshUserStatistics()
        {
            // Include metadata so you can access Title, Author, CoverUrl
            var userBooks = _context.Books
                .Include(b => b.Metadata)
                .Where(b => b.UserId == _currentUser.Id)
                .ToList();

            // Update stats
            BooksCount = userBooks.Count;
            ReviewsCount = userBooks.Count(b => !string.IsNullOrWhiteSpace(b.Review));
            RatingsCount = userBooks.Count(b => b.Rating.HasValue);
            AverageRating = RatingsCount > 0
                ? userBooks.Where(b => b.Rating.HasValue).Average(b => b.Rating!.Value)
                : 0;

            // Update UserBooks collection with metadata
            UserBooks = new ObservableCollection<BookMetadata>(
                userBooks.Select(b => b.Metadata)
                         .OrderBy(m => m.Title)
            );
        }


        private void LoadUserBooks()
        {
            var userBooks = _context.Books
                .Where(b => b.UserId == _currentUser.Id)
                .Include(b => b.Metadata)
                .Select(b => b.Metadata)
                .OrderBy(m => m.Title)
                .ToList();

            UserBooks = new ObservableCollection<BookMetadata>(userBooks);
        }

        private async Task PerformSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;

            IsSearching = true;

            var results = await _bookSearchService.SearchBooksAsync(SearchQuery);

            foreach (var book in results)
                book.CoverUrl = await GetLocalImagePathAsync(book.CoverUrl, book.Title);

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

        private void ChangeProfilePicture()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Profile Picture",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var ext = Path.GetExtension(dialog.FileName);
                    var safeFileName = $"{_currentUser.Id}_profile{ext}";
                    var destination = Path.Combine(_imageFolder, safeFileName);

                    File.Copy(dialog.FileName, destination, true);

                    _currentUser.ProfilePicturePath = destination;
                    _context.Users.Update(_currentUser);
                    _context.SaveChanges();

                    OnPropertyChanged(nameof(ProfilePicturePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update profile picture: {ex.Message}");
                }
            }
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
