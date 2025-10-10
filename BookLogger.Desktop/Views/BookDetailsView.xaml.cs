using BookLogger.Core.Services;
using BookLogger.Data.Models;
using BookLogger.Desktop.ViewModels;
using System.Windows.Controls;

namespace BookLogger.Desktop.Views
{
    public partial class BookDetailsView : Page
    {
        public BookDetailsViewModel ViewModel { get; }

        public BookDetailsView(BookMetadata book)
        {
            InitializeComponent();
            ViewModel = new BookDetailsViewModel(book);
            DataContext = ViewModel;
        }
    }
}
