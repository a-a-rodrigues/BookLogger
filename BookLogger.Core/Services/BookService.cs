using Microsoft.EntityFrameworkCore;
using BookLogger.Data;
using BookLogger.Data.Models;

public class BookService
{
    private readonly BookLoggerContext _context;

    public BookService(BookLoggerContext context)
    {
        _context = context;
    }

    public async Task<Book> AddBookAsync(int userId, string title, string author, string? isbn = null, string? genre = null, string? coverUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Title and author are required.");

        var metadata = await _context.BookMetadatas.FirstOrDefaultAsync(m => m.Title == title && m.Author == author);

        if (metadata == null)
        {
            metadata = new BookMetadata { 
                Title = title, 
                Author = author, 
                ISBN = isbn,
                Genre = genre,
                CoverUrl = coverUrl
            };
            _context.BookMetadatas.Add(metadata);
        }

        var book = new Book
        {
            UserId = userId,
            BookMetadataId = metadata.Id,
            Metadata = metadata
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<List<Book>> GetBooksByUserAsync(int userId)
    {
        return await _context.Books
            .Include(b => b.Metadata)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int bookId)
    {
        return await _context.Books
            .Include(b => b.Metadata)
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<bool> UpdateBookAsync(int bookId, string? rating = null, string? review = null, DateTime? dateRead = null)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null) return false;
        if (rating != null) book.Rating = rating;
        if (review != null) book.Review = review;
        if (dateRead != null) book.DateRead = dateRead;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBookAsync(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Book>> SearchBooksAsync(int userId, string? title = null, string? author = null)
    {
        var query = _context.Books
        .Include(b => b.Metadata)
        .Where(b => b.UserId == userId);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Metadata.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => b.Metadata.Author.Contains(author));

        return await query.ToListAsync();
    }
}