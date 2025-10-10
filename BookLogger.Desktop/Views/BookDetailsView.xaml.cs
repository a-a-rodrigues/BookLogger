using BookLogger.Core.Services;
using BookLogger.Desktop.ViewModels;
using System.Windows.Controls;

namespace BookLogger.Desktop.Views
{
    public partial class BookDetailsView : Page
    {
        public BookDetailsViewModel ViewModel { get; }

        public BookDetailsView(BookResult book)
        {
            InitializeComponent();
            ViewModel = new BookDetailsViewModel(book);
            DataContext = ViewModel;
        }
    }
}
