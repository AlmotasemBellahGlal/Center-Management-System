using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;

namespace Center_Management.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CenterDBContext ctx) : base(ctx)
        {
        }
    }
}
