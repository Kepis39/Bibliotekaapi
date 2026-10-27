using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Models.Services
{
    public class BooksService : AbstractionService, ICommonService<Book, string>
    {
        private int pageSize = 10;
        private readonly AppDbContext db;

        public BooksService(AppDbContext _db)
        {
            this.db = _db;
        }

        public bool Create(Book model)
        {
            bool result = DoAction(delegate ()
            {
                db.Books.Add(model);
                db.SaveChanges();
            });
            return result;
        }

        public bool Delete(string id)
        {
            bool result = DoAction(delegate ()
            {
                Book res = db.Books.FirstOrDefault(p => p.BookId == id)!;
                db.Books.Remove(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<Book> Get(string id)
        {
            Book? book = await db.Books.FirstOrDefaultAsync(p => p.BookId == id);
            return book!;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            return await db.Books.ToListAsync();
        }

        public bool Update(string id, Book model)
        {
            bool result = DoAction(delegate ()
            {
                Book res = db.Books.FirstOrDefault(p => p.BookId == id)!;
                res.Title = model.Title;
                res.FirstAuthor = model.FirstAuthor;
                res.Publisher = model.Publisher;
                res.PlaceOfPublication = model.PlaceOfPublication;
                res.YearOfPublication = model.YearOfPublication;
                res.PageCount = model.PageCount;
                res.Price = model.Price;
                db.Books.Update(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<IEnumerable<Book>> GetPage(int? page)
        {
            return await db.Books.OrderBy(p => p.BookId)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }

        // Дополнительный метод по условию задания: Популярные книги
        public async Task<IEnumerable<object>> GetPopularBooks(int top = 5)
        {
            return await db.Issues
                .Include(i => i.InventoryNumberNavigation)
                    .ThenInclude(c => c.Book)
                .GroupBy(i => new
                {
                    i.InventoryNumberNavigation.BookId,
                    i.InventoryNumberNavigation.Book.Title,
                    i.InventoryNumberNavigation.Book.FirstAuthor
                })
                .Select(g => new
                {
                    BookId = g.Key.BookId,
                    Title = g.Key.Title,
                    FirstAuthor = g.Key.FirstAuthor,
                    IssueCount = g.Count()
                })
                .OrderByDescending(x => x.IssueCount)
                .Take(top)
                .ToListAsync();
        }
    }
}
