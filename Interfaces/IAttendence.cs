using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IAttendenceRepository : IGenericRepository<Attendence>
    {
        Task<bool> HasAttendanceForDateAsync(int groupId, DateTime date, CancellationToken cancellationToken);
        Task<IEnumerable<Attendence>> GetGroupAttendanceAsync(int groupId, CancellationToken cancellationToken);
        Task<IEnumerable<Attendence>> GetStudentAttendanceInGroupAsync(int studentId, int groupId, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<Attendence> attendances, CancellationToken cancellationToken);
    }
}
