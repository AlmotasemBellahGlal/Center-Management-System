using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CenterDBContext ctx) : base(ctx)
        {
        }

        public async Task<bool> PaymentExistsAsync(int studentId, int groupId, int month, int year, CancellationToken cancellationToken)
        {
            return await ctx.Payments
                .AnyAsync(p => p.StudentId == studentId
                            && p.GroupId == groupId
                            && p.Month == month
                            && p.Year == year, cancellationToken);
        }

        public async Task<IEnumerable<Student>> GetUnpaidStudentsAsync(int groupId, int month, int year, CancellationToken cancellationToken)
        {
            // Get all active students in the group
            var enrolledStudentIds = await ctx.StudentGroups
                .Where(sg => sg.GroupId == groupId && sg.IsActive)
                .Select(sg => sg.StudentId)
                .ToListAsync(cancellationToken);

            // Get student IDs who have already paid for this group/month/year
            var paidStudentIds = await ctx.Payments
                .Where(p => p.GroupId == groupId && p.Month == month && p.Year == year && p.IsPaid)
                .Select(p => p.StudentId)
                .ToListAsync(cancellationToken);

            var unpaidIds = enrolledStudentIds.Except(paidStudentIds).ToList();

            return await ctx.Students
                .Where(s => unpaidIds.Contains(s.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetStudentPaymentsAsync(int studentId, CancellationToken cancellationToken)
        {
            return await ctx.Payments
                .Where(p => p.StudentId == studentId)
                .Include(p => p.Group)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }
    }
}
