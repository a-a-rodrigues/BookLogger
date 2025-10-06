using Microsoft.EntityFrameworkCore;
using BookLogger.Data.Models;

namespace BookLogger.Data
{
    public class BookLoggerContext : DbContext
    {
        public BookLoggerContext(DbContextOptions<BookLoggerContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Username).IsUnique();
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.Username).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<Book>(b =>
            {
                b.Property(x => x.Title).IsRequired();

                b.HasOne(bk => bk.User)
                 .WithMany(u => u.Books)
                 .HasForeignKey(bk => bk.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
