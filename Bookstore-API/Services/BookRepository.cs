using Bookstore_API.Contracts;
using Bookstore_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_API.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public async Task<bool> Create(Book entity)
        {
            await _dbContext.Books.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Book entity)
        {
            _dbContext.Books.Remove(entity);
            return await Save();
        }

        public async Task<bool> Exists(int id)
        {
            var exists = await _dbContext.Books.AnyAsync(b => b.Id == id);
            return exists;
        }

        public async Task<IList<Book>> FindAll()
        {
            var books = await _dbContext.Books.Include(q => q.Author).ToListAsync();
            return books;
        }

        public async Task<Book> FindById(int id)
        {
            var book = await _dbContext.Books.Include(q=>q.Author).FirstOrDefaultAsync(a => a.Id == id);
            return book;
        }

        public async Task<bool> Save()
        {
            var changes = await _dbContext.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Book entity)
        {
            _dbContext.Books.Update(entity);
            return await Save();
        }
    }
}
