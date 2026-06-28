using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;

namespace Center_Management.Repositories
{
    public class SubjectRepository:GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(CenterDBContext ctx) : base(ctx)
        { }
    }
}
