using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models.Data;
using Microsoft.EntityFrameworkCore;
namespace Bibliotekaapi.Models.Services
{
    public class ThematicCatalogsService : AbstractionService, ICommonService<ThematicCatalog, int>
    {
        private int pageSize = 10;
        private readonly AppDbContext db;

        public ThematicCatalogsService(AppDbContext _db)
        {
            this.db = _db;
        }

        public bool Create(ThematicCatalog model)
        {
            bool result = DoAction(delegate ()
            {
                db.ThematicCatalogs.Add(model);
                db.SaveChanges();
            });
            return result;
        }

        public bool Delete(int id)
        {
            bool result = DoAction(delegate ()
            {
                ThematicCatalog res = db.ThematicCatalogs.FirstOrDefault(p => p.ThemeCode == id)!;
                db.ThematicCatalogs.Remove(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<ThematicCatalog> Get(int id)
        {
            ThematicCatalog? catalog = await db.ThematicCatalogs.FirstOrDefaultAsync(p => p.ThemeCode == id);
            return catalog!;
        }

        public async Task<IEnumerable<ThematicCatalog>> GetAll()
        {
            return await db.ThematicCatalogs.ToListAsync();
        }

        public bool Update(int id, ThematicCatalog model)
        {
            bool result = DoAction(delegate ()
            {
                ThematicCatalog res = db.ThematicCatalogs.FirstOrDefault(p => p.ThemeCode == id)!;
                res.ThemeName = model.ThemeName;
                db.ThematicCatalogs.Update(res);
                db.SaveChanges();
            });
            return result;
        }

        public async Task<IEnumerable<ThematicCatalog>> GetPage(int? page)
        {
            return await db.ThematicCatalogs.OrderBy(p => p.ThemeCode)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }
    }
}
