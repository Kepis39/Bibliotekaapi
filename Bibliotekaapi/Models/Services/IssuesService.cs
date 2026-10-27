using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models.Data;
using Microsoft.EntityFrameworkCore;
namespace Bibliotekaapi.Models.Services
{
    public class IssuesService : AbstractionService, ICommonService<Issue, int>
    {
        private int pageSize = 10;
        private readonly AppDbContext db;

        public IssuesService(AppDbContext _db)
        {
            this.db = _db;
        }

        public bool Create(Issue model)
        {
            bool result = DoAction(delegate ()
            {
                db.Issues.Add(model);
                db.SaveChanges();
            });
            return result;
        }

        public bool Delete(int id)
        {
            bool result = DoAction(delegate ()
            {
                Issue res = db.Issues.FirstOrDefault(p => p.IssueId == id)!;
                db.Issues.Remove(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<Issue> Get(int id)
        {
            Issue? issue = await db.Issues.FirstOrDefaultAsync(p => p.IssueId == id);
            return issue!;
        }

        public async Task<IEnumerable<Issue>> GetAll()
        {
            return await db.Issues.ToListAsync();
        }

        public bool Update(int id, Issue model)
        {
            bool result = DoAction(delegate ()
            {
                Issue res = db.Issues.FirstOrDefault(p => p.IssueId == id)!;
                res.TicketNumber = model.TicketNumber;
                res.InventoryNumber = model.InventoryNumber;
                res.IssueDate = model.IssueDate;
                res.ReturnDate = model.ReturnDate;
                res.ActualReturn = model.ActualReturn;
                db.Issues.Update(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<IEnumerable<Issue>> GetPage(int? page)
        {
            return await db.Issues.OrderBy(p => p.IssueId)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }
    }
}
