using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;

namespace Center_Management.Repositories
{
    public class MatrialRepository : GenericRepository<Matrial>, IMatrialRepository
    {
        public MatrialRepository(CenterDBContext ctx) : base(ctx)
        {
        }
    }
}
