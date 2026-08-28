using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Center_Management.Repositories
{
    public class AcadimicYearsRepository : GenericRepository<AcademicYear>, IAcadimicYearsRepository
    {
        public AcadimicYearsRepository(CenterDBContext ctx) : base(ctx)
        {
        }
        public async Task<AcademicYear?> GetDetailsAsync(int id)
        {
            return await ctx.AcademicYears
                
                .Include(a => a.Groups)
                .ThenInclude(g => g.Schedules)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }

}

