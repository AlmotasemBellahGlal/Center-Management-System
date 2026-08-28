using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IAttendenceRepository : IGenericRepository<Attendence>
    {
        Task<bool> HasAttendanceForDateAsync(int groupId, DateTime date);
        Task<IEnumerable<Attendence>> GetGroupAttendanceAsync(int groupId);
        Task<IEnumerable<Attendence>> GetStudentAttendanceInGroupAsync(int studentId, int groupId);
        Task AddRangeAsync(IEnumerable<Attendence> attendances);
    }
}
