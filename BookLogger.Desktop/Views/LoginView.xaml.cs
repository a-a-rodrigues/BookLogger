using BookLogger.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BookLogger.Desktop.Views
{
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
