using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using BookLogger.Data;
using BookLogger.Data.Models;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
    private readonly BookLoggerContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public AuthService(BookLoggerContext context)
    {
        _context = context;
    }

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }

    public async Task<User> RegisterAsync(string username, string email, string password, string confirmPassword)
    {
        await ValidateUsernameAsync(username);
        await ValidateEmailAsync(email);
        ValidatePassword(password);
        ValidateConfirmPassword(password, confirmPassword);

        var user = new User
        {
            Username = username,
            Email = email
        };

        user.PasswordHash = HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

        if (user == null || !VerifyPassword(user, password))
            throw new Exception("Invalid username or password.");

        return user;
    }

    public async Task ResetPasswordAsync(string username, string newPassword)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null)
            throw new Exception("User not found.");

        user.PasswordHash = HashPassword(user, newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("Username is required.");

        if (username.Length < 3 || username.Length > 20)
        {
            throw new ArgumentException("Username must be 3-20 characters long.");
        }

        if (!username.All(char.IsLetterOrDigit))
            throw new ArgumentException("Username must be alphanumeric.");

        if (await _context.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException("Username already exists");
    }

    private async Task ValidateEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
            throw new ArgumentException("Email format is invalid.");

        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("Email is already in use."); 
    }

    private void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");

        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.");

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            throw new ArgumentException("Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one number.");
    }

    private void ValidateConfirmPassword(string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(confirmPassword))
            throw new ArgumentException("Confirm password is required.");

        if (password != confirmPassword)
            throw new ArgumentException("Passwords do not match.");
    }

}