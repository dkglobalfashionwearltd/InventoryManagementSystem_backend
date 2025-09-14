using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DkGLobalBackend.WebApi.Services
{
    public class Services<T> : IServices<T> where T : class
    {
        private readonly InventoryDbContext _db;
        private readonly DbSet<T> _dbSet;

        public Services(InventoryDbContext db)
        {
            _db = db;
            this._dbSet = db.Set<T>();
        }

        public async Task<bool> AnyAsync(GenericServiceRequest<T> request)
        {
            IQueryable<T> query = request.Tracked ? _dbSet.AsNoTracking() : _dbSet;
            if (request.Expression != null)
            {
                return await query.AnyAsync(request.Expression,request.CancellationToken);
            }
            return await query.AnyAsync(request.CancellationToken);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(GenericServiceRequest<T> request)
        {
            IQueryable<T> query = request.Tracked ? _dbSet.AsNoTracking() : _dbSet;
            if(request.Expression != null)
            {
                query = query.Where(request.Expression);
            }
            if(request.IncludeProperties != null)
            {
                foreach (var includeProperty in request.IncludeProperties.Split([","],StringSplitOptions.RemoveEmptyEntries)) {
                
                    query = query.Include(includeProperty.Trim());
                         
                };
            }
            return await query.ToListAsync(request.CancellationToken);
        }

        public async Task<T> GetAsync(GenericServiceRequest<T> request)
        {
            IQueryable<T> query = request.Tracked ? _dbSet.AsNoTracking() : _dbSet;
            if (request.Expression != null)
            {
                query = query.Where(request.Expression);
            }
            if (request.IncludeProperties != null)
            {
                foreach (var includeProperty in request.IncludeProperties.Split([","], StringSplitOptions.RemoveEmptyEntries)) {

                    query = query.Include(includeProperty.Trim());

                }
            }
            return await query.FirstOrDefaultAsync(request.CancellationToken);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
