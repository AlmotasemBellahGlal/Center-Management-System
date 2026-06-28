
using Center_Management.Models;
using Center_Management.View_Models;

namespace Center_Management.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        void AddSchedule(GroupSchedule schedule);

        void RemoveSchedule(GroupSchedule schedule);
        Task UpdateGroupAsync(CreateGroupVM vm);
        Task<List<Group>> GetGroupsByIdsAsync(List<int> groupIds);
        Task<IEnumerable<Group>> GetGroupsWithAcademicYearAndSubjectAsync();
    }
}
