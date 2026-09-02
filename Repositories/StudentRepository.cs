using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(CenterDBContext ctx) : base(ctx)
        {
        }
        public async Task<bool> UpdateStudentAsync(CreateStudentVM vm, CancellationToken cancellationToken)
        {
            var student = await ctx.Students
                .Include(s => s.StudentGroups)
                    .ThenInclude(sg => sg.Group)
                        .ThenInclude(g => g.AcademicYear)
                             
                .FirstOrDefaultAsync(s => s.Id == vm.Id, cancellationToken);

            if (student == null)
                return false;

            // ????? ???????? ????????
            student.FullName = vm.FullName;
            student.PhoneNumber = vm.PhoneNumber;
            student.ParentPhoneNumber = vm.ParentPhoneNumber;

            // ????? ???...
            var selectedGroupIds = vm.AcademicYears
                .Where(a => a.SelectedGroupId.HasValue)
                .Select(a => a.SelectedGroupId!.Value)
                .ToList();
            var selectedGroups = await ctx.Groups
                .Include(g => g.AcademicYear)
                     
                .Where(g => selectedGroupIds.Contains(g.Id))
                .ToListAsync(cancellationToken);
            var duplicatedSubjects = selectedGroups
.GroupBy(g => new
{

g.AcademicYearId
})
.Any(g => g.Count() > 1);

            if (duplicatedSubjects)
            {
                return false;
            }
            foreach (var group in selectedGroups)
            {
                var existing = student.StudentGroups
                    .FirstOrDefault(sg => sg.GroupId == group.Id);

                if (existing != null)
                {
                    existing.IsActive = true;
                    continue;
                }

                student.StudentGroups.Add(new StudentGroup
                {
                    GroupId = group.Id,
                    IsActive = true,
                    EnrollmentDate = DateOnly.FromDateTime(DateTime.Now)
                });
            }
            foreach (var sg in student.StudentGroups.Where(s => s.IsActive))
            {
                if (!selectedGroupIds.Contains(sg.GroupId))
                {
                    sg.IsActive = false;
                }
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return true;
        }
        public async Task<Student?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return await ctx.Students
                .Include(s => s.StudentGroups)
                .FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber, cancellationToken);
        }
        public async Task<IEnumerable<Student>> GetAllWithGroupsAsync(CancellationToken cancellationToken)
        {
            return await ctx.Students
                .Include(s => s.StudentGroups)
                    .ThenInclude(sg => sg.Group)
                        .ThenInclude(g => g.AcademicYear)
                             
                .ToListAsync(cancellationToken);
        }
        public async Task<Student?> GetDetailsAsync(int id, CancellationToken cancellationToken)
        {
            return await ctx.Students
                .Include(s => s.StudentGroups)
                    .ThenInclude(sg => sg.Group)
                        .ThenInclude(g => g.AcademicYear)
                             
                .Include(s => s.StudentGroups)
                    .ThenInclude(sg => sg.Group)
                        .ThenInclude(g => g.Schedules)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        public async Task<Student?> GetForEditAsync(int id, CancellationToken cancellationToken)
        {
            return await ctx.Students
                .Include(s => s.StudentGroups.Where(sg => sg.IsActive))
                    .ThenInclude(sg => sg.Group)
                        .ThenInclude(g => g.AcademicYear)
                             
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        public async Task<bool> RegisterStudentAsync(CreateStudentVM vm, CancellationToken cancellationToken)
        {
            var selectedGroupIds = vm.AcademicYears
                .Where(a => a.SelectedGroupId.HasValue)
                .Select(a => a.SelectedGroupId!.Value)
                .ToList();

            if (!selectedGroupIds.Any())
                return false;

            var selectedGroups = await ctx.Groups
    .Include(g => g.AcademicYear)
         
    .Where(g => selectedGroupIds.Contains(g.Id))
    .ToListAsync(cancellationToken);

            var student = await ctx.Students
                .Include(s => s.StudentGroups)
                    .ThenInclude(sg => sg.Group)
                        .ThenInclude(g => g.AcademicYear)
                             
                                .FirstOrDefaultAsync(s => s.PhoneNumber == vm.PhoneNumber, cancellationToken);

            //=========================
            // ???? ????
            //=========================

            if (student == null)
            {
                student = new Student
                {
                    FullName = vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    ParentPhoneNumber = vm.ParentPhoneNumber
                };

                foreach (var groupId in selectedGroupIds)
                {
                    student.StudentGroups.Add(new StudentGroup
                    {
                        GroupId = groupId
                    });
                }

                ctx.Students.Add(student);

                await ctx.SaveChangesAsync(cancellationToken);

                return true;
            }

            //=========================
            // ???? ?????
            //=========================

            foreach (var group in selectedGroups)
            {
                if (HasSubjectConflict(student, group))
                {
                    return false;
                }

                if (AlreadyInGroup(student, group.Id))
                {
                    continue;
                }

                student.StudentGroups.Add(new StudentGroup
                {
                    StudentId = student.Id,
                    GroupId = group.Id
                });
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return true;
        }
        private bool AlreadyInGroup(Student student, int groupId)
        {
            return student.StudentGroups.Any(sg =>

                sg.GroupId == groupId

                &&

                sg.IsActive
            );
        }
        private bool HasSubjectConflict(Student student, Group group)
        {
            return student.StudentGroups.Any(sg =>


                sg.Group.AcademicYearId ==
                group.AcademicYearId

                &&

                sg.IsActive
            );
        }

    }
}
