namespace Center_Management.View_Models
{
    public class PaymentRecordVM
    {
        public string GroupName { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}