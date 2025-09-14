using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class DepartmentService : Services<Department>, IDepartment
    {
        private readonly InventoryDbContext _db;
        public DepartmentService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Department department)
        {
            _db.Update(department);
        }
    }
}
