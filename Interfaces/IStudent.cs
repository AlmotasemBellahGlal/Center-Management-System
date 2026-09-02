using Center_Management.Models;
using Center_Management.View_Models;

namespace Center_Management.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<bool> RegisterStudentAsync(CreateStudentVM vm, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetAllWithGroupsAsync(CancellationToken cancellationToken);
        Task<Student?> GetDetailsAsync(int id, CancellationToken cancellationToken);
        Task<Student?> GetForEditAsync(int id, CancellationToken cancellationToken);
        public Task<bool> UpdateStudentAsync(CreateStudentVM vm, CancellationToken cancellationToken);
    }
}
