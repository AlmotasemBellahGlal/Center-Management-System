
using Center_Management.Models;
using Center_Management.View_Models;

namespace Center_Management.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        void AddSchedule(GroupSchedule schedule);

        void RemoveSchedule(GroupSchedule schedule);
        Task UpdateGroupAsync(CreateGroupVM vm, CancellationToken cancellationToken);
        Task<List<Group>> GetGroupsByIdsAsync(List<int> groupIds, CancellationToken cancellationToken);
        Task<IEnumerable<Group>> GetGroupsWithAcademicYearAndSubjectAsync(CancellationToken cancellationToken);
        Task<Group?> GetGroupWithAcademicYearAsync(int groupId, CancellationToken cancellationToken);
    }
}
