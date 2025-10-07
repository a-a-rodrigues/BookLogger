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
        public DbSet<BookMetadata> BookMetadatas { get; set; }


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

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Metadata)
                .WithMany(m => m.UserBooks)
                .HasForeignKey(b => b.BookMetadataId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.User)
                .WithMany(u => u.Books)
                .HasForeignKey(b => b.UserId);
        }

    }
}
