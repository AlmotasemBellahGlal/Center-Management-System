using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;

namespace Center_Management.Repositories
{
    public class AttendenceRepository : GenericRepository<Attendence>, IAttendenceRepository
    {
        public AttendenceRepository(CenterDBContext ctx) : base(ctx)
        {
        }
    }
}
