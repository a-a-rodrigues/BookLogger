using BookLogger.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class BookDetailsViewModel : INotifyPropertyChanged
    {
        // The selected book
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
        public List<int> RatingOptions { get; } = new List<int> { 1, 2, 3, 4, 5 };

        // Commands
        public ICommand AddBookCommand { get; }
        public ICommand CancelCommand { get; }

        // Event to notify page to close or navigate back
        public event Action? RequestClose;

        public BookDetailsViewModel(BookMetadata book)
        {
            SelectedBook = book;

            AddBookCommand = new RelayCommand(_ => AddBookToLibrary());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }

        private void AddBookToLibrary()
        {
            // Save book to database here with optional fields: UserRating, UserReview, DateRead
            // For now, just simulate the action
            Console.WriteLine($"Added '{SelectedBook.Title}' to library.");
            Console.WriteLine($"Rating: {UserRating}, Review: {UserReview}, DateRead: {DateRead}");

            // Notify the page to close or navigate back
            RequestClose?.Invoke();
        }

        // --- INotifyPropertyChanged implementation ---
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
