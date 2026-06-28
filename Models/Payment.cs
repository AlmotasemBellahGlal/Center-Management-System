namespace Center_Management.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
