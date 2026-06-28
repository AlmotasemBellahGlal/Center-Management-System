using System.Linq.Expressions;

namespace Center_Management.Interfaces
{
    public interface IGenericRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(
            int id,
            params Expression<Func<T, object>>[] includes);

        Task<int> SaveChangesAsync();
    }
}
