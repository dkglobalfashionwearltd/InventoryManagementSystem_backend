using DkGLobalBackend.WebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DkGLobalBackend.WebApi.Database
{
    public class InventoryDbContext : IdentityDbContext<ApplicationUser>
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
            
        }
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<ItemUser> ItemUsers => Set<ItemUser>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<AssignItemUser> AssignItemUser => Set<AssignItemUser>();
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Many-to-Many relationship
            modelBuilder.Entity<AssignItemUser>(entity =>
            {
                entity.ToTable("AssignItemUser"); // must match your DB table name

                entity.HasKey(au => new { au.ItemId, au.ItemUserId });

                entity.HasOne(au => au.Item)
                      .WithMany(i => i.AssignItemUsers)
                      .HasForeignKey(au => au.ItemId);

                entity.HasOne(au => au.ItemUser)
                      .WithMany(u => u.AssignItemUsers)
                      .HasForeignKey(au => au.ItemUserId);
            });
        }
    }
}
