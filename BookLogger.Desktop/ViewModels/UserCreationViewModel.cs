using BookLogger.Data;
using BookLogger.Desktop.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class UserCreationViewModel
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        private readonly AuthService _authService;

        public UserCreationViewModel()
        {
            var dbPath = @"C:/Users/Augie/Documents/Github/BookLogger/BookLogger.Data/booklogger.db";
            var options = new DbContextOptionsBuilder<BookLoggerContext>()
                .UseSqlite($"Filename={dbPath}")
                .Options;

            var context = new BookLoggerContext(options);
            _authService = new AuthService(context);

            // Back to Login button
            BackToLoginCommand = new RelayCommand(_ =>
            {
                var mainWindow = Application.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault();

                if (mainWindow != null)
                {
                    var loginPage = new LoginView();
                    mainWindow.MainFrame.Navigate(loginPage);
                }
            });

            RegisterCommand = new RelayCommand(async _ =>
            {
                try
                {
                    var user = await _authService.RegisterAsync(Username, Email, Password, ConfirmPassword);
                    MessageBox.Show($"User {user.Username} registered!", "Register", MessageBoxButton.OK, MessageBoxImage.Information);

                    var mainWindow = Application.Current.Windows
                        .OfType<MainWindow>()
                        .FirstOrDefault();

                    // Null check before navigating
                    if (mainWindow?.MainFrame != null)
                    {
                        var loginPage = new LoginView();
                        mainWindow.MainFrame.Navigate(loginPage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Register Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
        }   
    }
}
