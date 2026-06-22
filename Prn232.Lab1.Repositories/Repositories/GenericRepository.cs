using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories;
using Prn232.Lab1.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Prn232.Lab1.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected Prn232Lab1DbContext _context;

        public GenericRepository()
        {
            _context ??= new Prn232Lab1DbContext();
        }

        public GenericRepository(Prn232Lab1DbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<int> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes) query = query.Include(include);

            if (predicate != null) query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
