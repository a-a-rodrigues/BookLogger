using BookLogger.Desktop.ViewModels;
using System.Windows.Controls;
using System.Windows;

namespace BookLogger.Desktop.Views
{
    public partial class UserCreationView : Page
    {
        public UserCreationView()
        {
            InitializeComponent();
            this.DataContext = new UserCreationViewModel();
        }

        // Bind PasswordBox Password to ViewModel.Password
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserCreationViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }

        // Bind Confirm PasswordBox Password to ViewModel.ConfirmPassword
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserCreationViewModel vm)
            {
                vm.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
