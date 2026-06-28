using Center_Management.Models;
using Center_Management.View_Models;

namespace Center_Management.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByPhoneNumberAsync(string phoneNumber);
        Task<bool> RegisterStudentAsync(CreateStudentVM vm);
        Task<IEnumerable<Student>> GetAllWithGroupsAsync();
        Task<Student?> GetDetailsAsync(int id);
        Task<Student?> GetForEditAsync(int id);
        public Task<bool> UpdateStudentAsync(CreateStudentVM vm);
    }
}
