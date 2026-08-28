namespace Center_Management.View_Models
{
    public class StudentPaymentHistoryVM
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = "";
        public List<PaymentRecordVM> Payments { get; set; } = new();
    }
}