using BookLogger.Data;
using Microsoft.EntityFrameworkCore;

public class AuthServiceTests
{
    private BookLoggerContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BookLoggerContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // unique DB per test class
            .Options;

        return new BookLoggerContext(options);
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_CreatesUser()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var authService = new AuthService(context);

        string username = "testuser";
        string email = "test@example.com";
        string password = "Password1";
        string confirmPassword = "Password1";

        // Act
        var user = await authService.RegisterAsync(username, email, password, confirmPassword);

        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        Assert.NotNull(savedUser);                    // user exists in DB
        Assert.Equal(username, savedUser.Username);   // username matches
        Assert.Equal(email, savedUser.Email);         // email matches
        Assert.NotEqual(password, savedUser.PasswordHash); // password is hashed
    }
}
