namespace Center_Management.View_Models
{
    public class GroupPaymentStatusVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public List<string> PaidStudents { get; set; } = new();
        public List<string> UnpaidStudents { get; set; } = new();
    }
}