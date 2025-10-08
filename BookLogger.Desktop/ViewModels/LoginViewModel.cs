using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel()
        {
            // Login button: currently shows the entered username/password
            LoginCommand = new RelayCommand(_ =>
            {
                System.Windows.MessageBox.Show(
                    $"Username: {Username}\nPassword: {Password}",
                    "Login Attempt"
                );
            });

            // Register button: just shows a test message
            RegisterCommand = new RelayCommand(_ =>
            {
                System.Windows.MessageBox.Show(
                    "Attempted to register!",
                    "Register"
                );
            });
        }
    }
}
