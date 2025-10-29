using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class StockService : Services<Stock>, IStock
    {
        private readonly InventoryDbContext _db;
        public StockService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Stock stock)
        {
            _db.Update(stock);
        }
    }
}
