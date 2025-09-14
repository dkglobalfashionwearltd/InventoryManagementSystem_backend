using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace DkGLobalBackend.WebApi.Services
{
    public class ServiceManager : IServiceManager
    {
        public IItem Items {  get; private set; }
        public ICategory Categories {  get; private set; }
        public IDepartment Departments {  get; private set; }
        public IItemUser ItemUsers {  get; private set; }
        public IAuth Auth {  get; private set; }
        public IAssignItem AssignItemUsers {  get; private set; }

        private readonly InventoryDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _secretKey;
        public ServiceManager(InventoryDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _db = db;
            _secretKey = configuration["TokenSetting:SecretKey"] ?? "";
            _userManager = userManager;
            _roleManager = roleManager;
            Items =new ItemService(_db);
            Categories =new CategoryService(_db);
            Departments =new DepartmentService(_db);
            ItemUsers =new ItemUserService(_db);
            Auth = new AuthService(_db, _userManager, _roleManager,_secretKey
                );
            AssignItemUsers = new AssignItemService(_db);
        }

        public async Task<int> Save()
        {
           return await _db.SaveChangesAsync();
        }
    }
}
