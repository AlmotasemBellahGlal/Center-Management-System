
using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(CenterDBContext ctx) : base(ctx)
        {

        }
        public void AddSchedule(GroupSchedule schedule)
        {
            ctx.GroupSchedules.Add(schedule);
        }

        public void RemoveSchedule(GroupSchedule schedule)
        {
            ctx.GroupSchedules.Remove(schedule);
        }
        public async Task<IEnumerable<Group>> GetGroupsWithAcademicYearAndSubjectAsync()
        {
            return await ctx.Groups
                .Include(g => g.AcademicYear)
                    
                .ToListAsync();
        }
        public async Task<List<Group>> GetGroupsByIdsAsync(List<int> groupIds)
        {
            return await ctx.Groups
                .Include(g => g.AcademicYear)
                    
                .Where(g => groupIds.Contains(g.Id))
                .ToListAsync();
        }
        public async Task<Group?> GetGroupWithAcademicYearAsync(int groupId)
        {
            return await ctx.Groups
                .Include(g => g.AcademicYear)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }
        public async Task UpdateGroupAsync(CreateGroupVM vm)
        {
            var group = await ctx.Groups
    .Include(g => g.Schedules)
    .FirstOrDefaultAsync(g => g.Id == vm.Id);

            if (group == null)
            {
                throw new Exception("Group Not Found");
            }
            group.Name = vm.Name;
            group.AcademicYearId = vm.AcademicYearId;
            foreach (var schedule in vm.Schedules.Where(s => s.Id != 0))
            {
                var existing = group.Schedules
                    .FirstOrDefault(x => x.Id == schedule.Id);

                if (existing != null)
                {
                    existing.Day = schedule.Day;
                    existing.StartTime = schedule.StartTime;
                }
            }
            foreach (var schedule in vm.Schedules.Where(s => s.Id == 0))
            {
                group.Schedules.Add(new GroupSchedule
                {
                    Day = schedule.Day,
                    StartTime = schedule.StartTime
                });
            }
            var deletedSchedules = group.Schedules
    .Where(db => !vm.Schedules.Any(x => x.Id == db.Id))
    .ToList();

            foreach (var item in deletedSchedules)
            {
                ctx.GroupSchedules.Remove(item);
            }
            await ctx.SaveChangesAsync();
        }

    }
}
