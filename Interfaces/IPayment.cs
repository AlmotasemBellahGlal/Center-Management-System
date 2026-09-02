using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<bool> PaymentExistsAsync(int studentId, int groupId, int month, int year, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetUnpaidStudentsAsync(int groupId, int month, int year, CancellationToken cancellationToken);
        Task<IEnumerable<Payment>> GetStudentPaymentsAsync(int studentId, CancellationToken cancellationToken);
    }
}
