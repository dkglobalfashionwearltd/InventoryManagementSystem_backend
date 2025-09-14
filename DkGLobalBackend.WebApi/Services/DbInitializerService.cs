using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;
using DkGLobalBackend.WebApi.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DkGLobalBackend.WebApi.Services
{
    public class DbInitializerService : IDbInitializerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly InventoryDbContext _inventoryDbContext;

        public DbInitializerService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, InventoryDbContext inventoryDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _inventoryDbContext = inventoryDbContext;
        }

        
        public async Task InitializeAsync()
        {
            await ApplyMigrationsAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task ApplyMigrationsAsync()
        {
            if ((await _inventoryDbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                await _inventoryDbContext.Database.MigrateAsync();
            }
            else
            {
                Console.WriteLine("ℹ️ No pending migrations.");
            }
        }
        private async Task SeedRolesAsync()
        {
            string[] roles = { Roles.ADMIN, Roles.IT, Roles.STORE, Roles.USER };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            const string adminEmail = "itdkglobalfashion@gmail.com";
            const string adminPassword = "aDmin@00#";

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    PhoneNumber = "01970806028",
                    Password = adminPassword,
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, Roles.ADMIN);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
