using BookLogger.Data;
using BookLogger.Data.Models;
using BookLogger.Desktop.ViewModels;
using System.Linq;
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
            var viewModel = new DashboardViewModel(context, user);
            DataContext = viewModel;
        }

        private void BookResult_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is DashboardViewModel viewModel && viewModel.SelectedBook != null)
            {
                var selectedBook = viewModel.SelectedBook;

                // Find the main window
                var mainWindow = Application.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault();

                if (mainWindow != null)
                {
                    // Create the BookDetailsView, passing context, current user, and refresh callback
                    var detailsPage = new BookDetailsView(
                        selectedBook,
                        viewModel._context,                // Ensure DashboardViewModel exposes 'Context'
                        viewModel._currentUser,            // Ensure DashboardViewModel exposes 'CurrentUser'
                        () => viewModel.RefreshUserStatistics()  // Callback to refresh stats
                    );

                    // Navigate to the BookDetailsView
                    mainWindow.MainFrame.Navigate(detailsPage);
                }
            }
        }
    }
}
