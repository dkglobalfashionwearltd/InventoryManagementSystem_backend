using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class HistoryService : Services<History>, IHistory
    {
        private readonly InventoryDbContext _db;
        public HistoryService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(History history)
        {
            _db.Update(history);
        }
    }
}
