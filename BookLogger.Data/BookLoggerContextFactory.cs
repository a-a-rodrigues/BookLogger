using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookLogger.Data
{
    public class BookLoggerContextFactory : IDesignTimeDbContextFactory<BookLoggerContext>
    {
        public BookLoggerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookLoggerContext>();
            optionsBuilder.UseSqlite("Data Source=BookLogger.db"); // same connection as runtime

            return new BookLoggerContext(optionsBuilder.Options);
        }
    }
}
