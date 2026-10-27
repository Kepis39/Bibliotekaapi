using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Models.Services
{
    public class BookCatalogsService : AbstractionService, ICommonService<BookCatalog, int>
    {
        private int pageSize = 10;
        private readonly AppDbContext db;

        public BookCatalogsService(AppDbContext _db)
        {
            this.db = _db;
        }

        public bool Create(BookCatalog model)
        {
            bool result = DoAction(delegate ()
            {
                db.BookCatalogs.Add(model);
                db.SaveChanges();
            });
            return result;
        }

        public bool Delete(int id)
        {
            bool result = DoAction(delegate ()
            {
                BookCatalog res = db.BookCatalogs.FirstOrDefault(p => p.CatalogId == id)!;
                db.BookCatalogs.Remove(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<BookCatalog> Get(int id)
        {
            BookCatalog? bookCatalog = await db.BookCatalogs.FirstOrDefaultAsync(p => p.CatalogId == id);
            return bookCatalog!;
        }

        public async Task<IEnumerable<BookCatalog>> GetAll()
        {
            return await db.BookCatalogs.ToListAsync();
        }

        public bool Update(int id, BookCatalog model)
        {
            bool result = DoAction(delegate ()
            {
                BookCatalog res = db.BookCatalogs.FirstOrDefault(p => p.CatalogId == id)!;
                res.BookId = model.BookId;
                res.ThemeCode = model.ThemeCode;
                db.BookCatalogs.Update(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<IEnumerable<BookCatalog>> GetPage(int? page)
        {
            return await db.BookCatalogs.OrderBy(p => p.CatalogId)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }
    }
}
