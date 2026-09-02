using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IMatrialRepository : IGenericRepository<Matrial>
    {
        Task<IEnumerable<Matrial>> GetAllWithDetailsAsync(CancellationToken cancellationToken);
        Task<Matrial?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken);
        Task<bool> IsStudentEnrolledAsync(int studentId, int academicYearId, CancellationToken cancellationToken);
        Task<AppUser?> GetCurrentUserWithStudentAsync(System.Security.Claims.ClaimsPrincipal user, CancellationToken cancellationToken);
        Task<List<int>> GetStudentAcademicYearIdsAsync(int studentId, CancellationToken cancellationToken);
    }
}
