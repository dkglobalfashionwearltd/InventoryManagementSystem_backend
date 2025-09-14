using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class ItemUserService : Services<ItemUser>, IItemUser
    {
        private readonly InventoryDbContext _db;
        public ItemUserService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ItemUser itemUser)
        {
            _db.Update(itemUser);
        }
    }
}
