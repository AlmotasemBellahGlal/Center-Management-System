using System.Linq.Expressions;

namespace Center_Management.Interfaces
{
    public interface IGenericRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(
            CancellationToken cancellationToken,
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken,
            params Expression<Func<T, object>>[] includes);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
