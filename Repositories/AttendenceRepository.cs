using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Repositories
{
    public class AttendenceRepository : GenericRepository<Attendence>, IAttendenceRepository
    {
        public AttendenceRepository(CenterDBContext ctx) : base(ctx)
        {
        }

        public async Task<bool> HasAttendanceForDateAsync(int groupId, DateTime date)
        {
            return await ctx.Attendences
                .AnyAsync(a => a.GroupId == groupId && a.Date.Date == date.Date);
        }

        public async Task<IEnumerable<Attendence>> GetGroupAttendanceAsync(int groupId)
        {
            return await ctx.Attendences
                .Where(a => a.GroupId == groupId)
                .Include(a => a.Student)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendence>> GetStudentAttendanceInGroupAsync(int studentId, int groupId)
        {
            return await ctx.Attendences
                .Where(a => a.StudentId == studentId && a.GroupId == groupId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Attendence> attendances)
        {
            await ctx.Attendences.AddRangeAsync(attendances);
        }
    }
}
