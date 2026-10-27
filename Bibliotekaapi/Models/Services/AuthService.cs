using Bibliotekaapi.Models.Data;

namespace Bibliotekaapi.Models.Services
{
    public class AuthService
    {
        private readonly AppDbContext db;

        public AuthService(AppDbContext _db)
        {
            db = _db;
        }

        public Reader Login(string login, string password)
        {
            Reader reader = db.Readers.FirstOrDefault(x => x.FullName == login && x.Phone == password)!;
            if (reader != null) return reader;
            return null!;
        }

        public bool Register(Reader regReader)
        {
            Reader reader = db.Readers.FirstOrDefault(p => p.FullName == regReader.FullName)!;
            if (reader == null)
            {
                db.Readers.Add(regReader);
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
