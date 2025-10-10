using BookLogger.Data;
using BookLogger.Data.Models;
using BookLogger.Desktop.ViewModels;
using System.DirectoryServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookLogger.Desktop.Views
{
    public partial class DashboardView : UserControl
    {
        // Parameterless constructor for XAML designer
        public DashboardView()
        {
            InitializeComponent();
        }

        // Constructor that accepts context and user
        public DashboardView(BookLoggerContext context, User user)
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(context, user);
        }

        private void BookResult_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get the view model bound to this DashboardView
            if (DataContext is DashboardViewModel viewModel && viewModel.SelectedBook != null)
            {
                var selectedBook = viewModel.SelectedBook;

                // Find the main window
                var mainWindow = Application.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault();

                // Create the details page, passing in the selected book
                var detailsPage = new BookDetailsView(selectedBook);

                // Navigate to the page (assumes MainWindow has a Frame named MainFrame)
                (mainWindow?.MainFrame)?.Navigate(detailsPage);
            }
        }


    }
}
