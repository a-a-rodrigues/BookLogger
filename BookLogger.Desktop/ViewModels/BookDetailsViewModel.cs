using BookLogger.Data;
using BookLogger.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class BookDetailsViewModel : INotifyPropertyChanged
    {
        private readonly BookLoggerContext _context;
        private readonly User _currentUser;
        private readonly Action _refreshDashboardStatistics;

        // The selected book metadata (may not yet exist in DB)
        public BookMetadata SelectedBook { get; }

        // --- Bindable image property ---
        public string BookImage => !string.IsNullOrEmpty(SelectedBook.CoverUrl)
            ? SelectedBook.CoverUrl
            : "pack://application:,,,/Resources/default-book.jpg";

        // --- User input properties ---
        private int? _userRating;
        public int? UserRating
        {
            get => _userRating;
            set { _userRating = value; OnPropertyChanged(); }
        }

        private string _userReview;
        public string UserReview
        {
            get => _userReview;
            set { _userReview = value; OnPropertyChanged(); }
        }

        private DateTime? _dateRead;
        public DateTime? DateRead
        {
            get => _dateRead;
            set { _dateRead = value; OnPropertyChanged(); }
        }

        // Rating options
        public List<int> RatingOptions { get; } = new() { 1, 2, 3, 4, 5 };

        // Commands
        public ICommand AddBookCommand { get; }
        public ICommand CancelCommand { get; }

        // Event to notify page to close or navigate back
        public event Action? RequestClose;

        public BookDetailsViewModel(BookMetadata book, BookLoggerContext context, User currentUser, Action refreshDashboardStatistics)
        {
            SelectedBook = book;
            _context = context;
            _currentUser = currentUser;
            _refreshDashboardStatistics = refreshDashboardStatistics;

            AddBookCommand = new RelayCommand(_ => AddBookToLibrary());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }

        private void AddBookToLibrary()
        {
            // Check if this BookMetadata already exists in DB
            var metadata = _context.BookMetadatas
                .FirstOrDefault(b => b.Title == SelectedBook.Title &&
                                     b.Author == SelectedBook.Author &&
                                     b.FirstPublishYear == SelectedBook.FirstPublishYear);

            // If not, create a new metadata record
            if (metadata == null)
            {
                metadata = new BookMetadata
                {
                    Title = SelectedBook.Title,
                    Author = SelectedBook.Author,
                    CoverUrl = SelectedBook.CoverUrl,
                    ISBN = SelectedBook.ISBN,
                    Genre = SelectedBook.Genre,
                    FirstPublishYear = SelectedBook.FirstPublishYear
                };
                _context.BookMetadatas.Add(metadata);
                _context.SaveChanges(); // EF assigns ID
            }

            // Create the user-centered Book entry
            var userBook = new Book
            {
                UserId = _currentUser.Id,
                BookMetadataId = metadata.Id,
                Rating = UserRating,
                Review = UserReview,
                DateRead = DateRead
            };

            _context.Books.Add(userBook);
            _context.SaveChanges();

            // Refresh dashboard stats dynamically
            _refreshDashboardStatistics?.Invoke();

            // Close the page
            RequestClose?.Invoke();
        }


        // --- INotifyPropertyChanged implementation ---
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
