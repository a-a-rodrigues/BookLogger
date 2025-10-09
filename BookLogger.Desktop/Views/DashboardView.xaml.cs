using BookLogger.Data;
using BookLogger.Data.Models;
using BookLogger.Desktop.ViewModels;
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
            if (DataContext is not DashboardViewModel vm)
                return;

            var selectedBook = vm.SelectedBook;
            if (selectedBook == null)
                return;

            // Navigate to BookDetailsPage
            var detailsPage = new BookDetailsPage(selectedBook);

            // Replace right-hand content with details page
            if (this.Parent is Grid parentGrid)
            {
                var rightContentGrid = parentGrid.FindName("RightContentGrid") as Grid;
                if (rightContentGrid != null)
                {
                    rightContentGrid.Children.Clear();
                    rightContentGrid.Children.Add(detailsPage);
                }
            }
        }
    }
}
