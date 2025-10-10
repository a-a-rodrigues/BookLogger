using BookLogger.Core.Services;
using BookLogger.Desktop.ViewModels;
using System.Windows.Controls;

namespace BookLogger.Desktop.Views
{
    public partial class BookDetailsPage : Page
    {
        public BookDetailsViewModel ViewModel { get; }

        public BookDetailsPage(BookResult book)
        {
            InitializeComponent();
            ViewModel = new BookDetailsViewModel(book);
            DataContext = ViewModel;
        }
    }
}
