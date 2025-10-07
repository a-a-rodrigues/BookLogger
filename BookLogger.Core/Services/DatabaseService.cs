using System.Collections.Generic;
using System.Threading.Tasks;
using BookLogger.Data;
using BookLogger.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLogger.Core.Services
{
    public class DatabaseService
    {
        private readonly BookLoggerContext _context;

        public DatabaseService(BookLoggerContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Metadata)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}