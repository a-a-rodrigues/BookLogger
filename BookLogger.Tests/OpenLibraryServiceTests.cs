using Xunit;
using System.Threading.Tasks;
using System.Linq;

public class OpenLibraryServiceTests
{
    [Fact]
    public async Task SearchBooksAsync_ReturnsResults_ForValidTitle()
    {
        var service = new OpenLibraryService();

        var results = await service.SearchBooksAsync("Frankenstein", "Mary Shelley");

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Contains(results, b => b.Title!.Contains("Prometheus", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchBooksAsync_ReturnsResults_ForValidTitle_NoAuthor()
    {
        var service = new OpenLibraryService();

        var results = await service.SearchBooksAsync("Frankenstein", null);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Contains(results, b => b.Title!.Contains("Prometheus", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchBooksAsync_ReturnsEmptyList_ForGibberish()
    {
        var service = new OpenLibraryService();

        var results = await service.SearchBooksAsync("asdkjhfsadfkjhasdfkjasdhfkj", null);

        Assert.NotNull(results);
        Assert.Empty(results);
    }
}
