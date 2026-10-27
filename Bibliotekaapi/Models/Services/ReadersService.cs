using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models.Data;
using Microsoft.EntityFrameworkCore;
namespace Bibliotekaapi.Models.Services
{
    public class ReadersService : AbstractionService, ICommonService<Reader, int>
    {
        private int pageSize = 10;
        private readonly AppDbContext db;

        public ReadersService(AppDbContext _db)
        {
            this.db = _db;
        }

        public bool Create(Reader model)
        {
            bool result = DoAction(delegate ()
            {
                db.Readers.Add(model);
                db.SaveChanges();
            });
            return result;
        }

        public bool Delete(int id)
        {
            bool result = DoAction(delegate ()
            {
                Reader res = db.Readers.FirstOrDefault(p => p.TicketNumber == id)!;
                db.Readers.Remove(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<Reader> Get(int id)
        {
            Reader? reader = await db.Readers.FirstOrDefaultAsync(p => p.TicketNumber == id);
            return reader!;
        }

        public async Task<IEnumerable<Reader>> GetAll()
        {
            return await db.Readers.ToListAsync();
        }

        public bool Update(int id, Reader model)
        {
            bool result = DoAction(delegate ()
            {
                Reader res = db.Readers.FirstOrDefault(p => p.TicketNumber == id)!;
                res.FullName = model.FullName;
                res.BirthDate = model.BirthDate;
                res.Phone = model.Phone;
                db.Readers.Update(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<IEnumerable<Reader>> GetPage(int? page)
        {
            return await db.Readers.OrderBy(p => p.TicketNumber)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }

        // Дополнительный метод по условию задания: Должники библиотеки
        public async Task<IEnumerable<object>> GetDebtors()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await db.Issues
                .Include(i => i.TicketNumberNavigation)
                .Include(i => i.InventoryNumberNavigation)
                    .ThenInclude(c => c.Book)
                .Where(i =>
                    (i.ActualReturn == null && i.ReturnDate < today) ||
                    (i.ActualReturn != null && i.ActualReturn > i.ReturnDate))
                .Select(i => new
                {
                    TicketNumber = i.TicketNumber,
                    FullName = i.TicketNumberNavigation.FullName,
                    Phone = i.TicketNumberNavigation.Phone,
                    BookTitle = i.InventoryNumberNavigation.Book.Title,
                    InventoryNumber = i.InventoryNumber,
                    IssueDate = i.IssueDate,
                    ReturnDate = i.ReturnDate,
                    ActualReturn = i.ActualReturn
                })
                .ToListAsync();
        }
    }
}
