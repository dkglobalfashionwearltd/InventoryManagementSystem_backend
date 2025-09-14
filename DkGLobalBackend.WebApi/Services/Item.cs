using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class ItemService : Services<Item>, IItem
    {
        private readonly InventoryDbContext _db;
        public ItemService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Item item)
        {
            _db.Update(item);
        }
    }
}
