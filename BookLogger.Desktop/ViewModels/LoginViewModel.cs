using BookLogger.Core.Services;
using BookLogger.Data;
using BookLogger.Desktop.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Input;

namespace BookLogger.Desktop.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private readonly AuthService _authService;

        public LoginViewModel()
        {
            var dbPath = @"C:/Users/Augie/Documents/Github/BookLogger/BookLogger.Data/booklogger.db";                
            var options = new DbContextOptionsBuilder<BookLoggerContext>()
                .UseSqlite($"Filename={dbPath}")
                .Options;

            var context = new BookLoggerContext(options);
            _authService = new AuthService(context);

            // Login button
            LoginCommand = new RelayCommand(async _ =>
            {
                try
                {
                    var user = await _authService.LoginAsync(Username, Password);
                    MessageBox.Show($"Welcome back, {user.Username}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    var mainWindow = Application.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault();

                    if (mainWindow != null)
                    {
                        var dashboardPage = new DashboardView(context, user);
                        dashboardPage.DataContext = new DashboardViewModel(context, user);

                        mainWindow.MainFrame.Navigate(dashboardPage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            // Register button
            RegisterCommand = new RelayCommand(async _ =>
            {
                try
                {
                    // For demo, we just auto-fill confirmPassword same as password
                    var user = await _authService.RegisterAsync(Username, "example2@example.com", Password, Password);
                    MessageBox.Show($"User {user.Username} registered!", "Register", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Register Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
        }
    }
}
