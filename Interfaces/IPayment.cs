using Center_Management.Models;

namespace Center_Management.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<bool> PaymentExistsAsync(int studentId, int groupId, int month, int year);
        Task<IEnumerable<Student>> GetUnpaidStudentsAsync(int groupId, int month, int year);
        Task<IEnumerable<Payment>> GetStudentPaymentsAsync(int studentId);
    }
}
