using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookLogger.Data;
using BookLogger.Data.Models;

namespace BookLogger.Desktop.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly BookLoggerContext _context;
        private User _currentUser;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DashboardViewModel(BookLoggerContext context, User currentUser)
        {
            _context = context;
            _currentUser = currentUser;

            LoadUserStatistics();
        }

        // Bindable Properties
        public string ProfilePicturePath => _currentUser.ProfilePicturePath ?? "pack://application:,,,/Assets/default_profile.png";

        public int BooksCount { get; private set; }
        public int ReviewsCount { get; private set; } // optional if you track reviews separately
        public int RatingsCount { get; private set; }
        public double AverageRating { get; private set; }

        // Load or compute statistics
        private void LoadUserStatistics()
        {
            var userBooks = _context.Books.Where(b => b.UserId == _currentUser.Id).ToList();

            BooksCount = userBooks.Count;
            RatingsCount = userBooks.Count(b => b.Rating.HasValue);
            AverageRating = RatingsCount > 0 ? userBooks.Where(b => b.Rating.HasValue).Average(b => b.Rating!.Value) : 0;

            // If you implement reviews later, compute ReviewsCount here

            OnPropertyChanged(nameof(BooksCount));
            OnPropertyChanged(nameof(RatingsCount));
            OnPropertyChanged(nameof(AverageRating));
        }

        // INotifyPropertyChanged helper
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
