using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Center_Management.Repositories
{
    public class MatrialRepository : GenericRepository<Matrial>, IMatrialRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public MatrialRepository(CenterDBContext ctx, UserManager<AppUser> userManager) : base(ctx)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves all materials with their related AcademicYear details,
        /// ordered by AcademicYear.Name.
        /// </summary>
        public async Task<IEnumerable<Matrial>> GetAllWithDetailsAsync(CancellationToken cancellationToken)
        {
            return await ctx.Matrials
                .Include(m => m.AcademicYear)
                .OrderBy(m => m.AcademicYear!.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a single material by ID with its related AcademicYear details.
        /// </summary>
        public async Task<Matrial?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken)
        {
            return await ctx.Matrials
                .Include(m => m.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        /// <summary>
        /// Checks if a student is enrolled in an active group for the specified academic year.
        /// A student is considered enrolled if they have a StudentGroup record with IsActive = true
        /// in any group belonging to the specified AcademicYear.
        /// </summary>
        public async Task<bool> IsStudentEnrolledAsync(int studentId, int academicYearId, CancellationToken cancellationToken)
        {
            return await ctx.StudentGroups
                .Include(sg => sg.Group)
                .AnyAsync(sg => sg.StudentId == studentId
                             && sg.Group.AcademicYearId == academicYearId
                             && sg.IsActive, cancellationToken);
        }

        /// <summary>
        /// Gets the current user with their Student relationship loaded.
        /// </summary>
        public async Task<AppUser?> GetCurrentUserWithStudentAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == user.Identity!.Name, cancellationToken);
        }

        /// <summary>
        /// Gets all AcademicYear IDs for a student's active groups.
        /// Used to filter materials for student view.
        /// </summary>
        public async Task<List<int>> GetStudentAcademicYearIdsAsync(int studentId, CancellationToken cancellationToken)
        {
            return await ctx.StudentGroups
                .Where(sg => sg.StudentId == studentId && sg.IsActive)
                .Include(sg => sg.Group)
                .Select(sg => sg.Group.AcademicYearId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
