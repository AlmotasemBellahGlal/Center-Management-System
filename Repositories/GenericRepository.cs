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

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await ctx.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await ctx.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            CancellationToken cancellationToken,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = ctx.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await ctx.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = ctx.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var entity = await query.FirstOrDefaultAsync(e =>
                EF.Property<int>(e, "Id") == id, cancellationToken);

            return entity;
        }
    }
}
