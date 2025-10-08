using BookLogger.Desktop.ViewModels;
using BookLogger.Data;
using BookLogger.Data.Models;
using System.Windows.Controls;

namespace BookLogger.Desktop.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView(BookLoggerContext context, User currentUser)
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(context, currentUser);
        }
    }
}
