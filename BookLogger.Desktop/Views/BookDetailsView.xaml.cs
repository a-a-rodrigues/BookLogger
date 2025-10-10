using BookLogger.Data;
using BookLogger.Data.Models;
using BookLogger.Desktop.ViewModels;
using System.Windows.Controls;

namespace BookLogger.Desktop.Views
{
    public partial class BookDetailsView : Page
    {
        public BookDetailsView(BookMetadata book, BookLoggerContext context, User currentUser, Action refreshDashboardStats)
        {
            InitializeComponent();

            // Pass context, user, and refresh callback to the ViewModel
            var viewModel = new BookDetailsViewModel(book, context, currentUser, refreshDashboardStats);
            DataContext = viewModel;

            // Navigate back when requested
            viewModel.RequestClose += () =>
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            };
        }
    }
}
