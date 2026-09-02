using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IAcadimicYearsRepository : IGenericRepository<AcademicYear>
    {
        Task<AcademicYear?> GetDetailsAsync(int id, CancellationToken cancellationToken);
    }
}
