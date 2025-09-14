using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;

namespace DkGLobalBackend.WebApi.Services
{
    public class CategoryService : Services<Category>, ICategory
    {
        private readonly InventoryDbContext _db;
        public CategoryService(InventoryDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            _db.Update(category);
        }
    }
}
