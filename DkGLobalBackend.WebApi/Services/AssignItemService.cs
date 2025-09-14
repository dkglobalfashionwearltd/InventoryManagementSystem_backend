using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class AssignItemService : Services<AssignItemUser>, IAssignItem
    {
        private readonly InventoryDbContext _db;
        public AssignItemService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AssignItemUser assignitemuser)
        {
            _db.Update(assignitemuser);
        }
    }
}
