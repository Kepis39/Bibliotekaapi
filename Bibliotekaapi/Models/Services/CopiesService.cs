using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Models.Services
{
    public class CopiesService : AbstractionService, ICommonService<Copy, string>
    {
        private int pageSize = 10;
        private readonly AppDbContext db;

        public CopiesService(AppDbContext _db)
        {
            this.db = _db;
        }

        public bool Create(Copy model)
        {
            bool result = DoAction(delegate ()
            {
                db.Copies.Add(model);
                db.SaveChanges();
            });
            return result;
        }

        public bool Delete(string id)
        {
            bool result = DoAction(delegate ()
            {
                Copy res = db.Copies.FirstOrDefault(p => p.InventoryNumber == id)!;
                db.Copies.Remove(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<Copy> Get(string id)
        {
            Copy? copy = await db.Copies.FirstOrDefaultAsync(p => p.InventoryNumber == id);
            return copy!;
        }

        public async Task<IEnumerable<Copy>> GetAll()
        {
            return await db.Copies.ToListAsync();
        }

        public bool Update(string id, Copy model)
        {
            bool result = DoAction(delegate ()
            {
                Copy res = db.Copies.FirstOrDefault(p => p.InventoryNumber == id)!;
                res.BookId = model.BookId;
                res.ThemeCode = model.ThemeCode;
                res.CopyCount = model.CopyCount;
                db.Copies.Update(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<IEnumerable<Copy>> GetPage(int? page)
        {
            return await db.Copies.OrderBy(p => p.InventoryNumber)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }
    }
}
