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
            if (sender is ListViewItem item && item.Content != null)
            {
                MessageBox.Show("Book selected!");
            }
        }

    }
}
