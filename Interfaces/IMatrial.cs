using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IMatrialRepository : IGenericRepository<Matrial>
    {
        Task<IEnumerable<Matrial>> GetAllWithDetailsAsync();
        Task<Matrial?> GetByIdWithDetailsAsync(int id);
        Task<bool> IsStudentEnrolledAsync(int studentId, int academicYearId);
        Task<AppUser?> GetCurrentUserWithStudentAsync(System.Security.Claims.ClaimsPrincipal user);
        Task<List<int>> GetStudentAcademicYearIdsAsync(int studentId);
    }
}
