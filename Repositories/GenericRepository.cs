using Center_Management.Context;
using Center_Management.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Center_Management.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly CenterDBContext ctx;

        public GenericRepository(CenterDBContext ctx)
        {
            this.ctx = ctx;
        }
        public void Add(T entity)
        {
            ctx.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            ctx.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            ctx.Set<T>().Update(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await ctx.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = ctx.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await ctx.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(
            int id,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = ctx.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var entity = await query.FirstOrDefaultAsync(e =>
                EF.Property<int>(e, "Id") == id);

            return entity;
        }
    }
}
