namespace Center_Management.View_Models
{
    public class AttendanceRecordVM
    {
        public string StudentFullName { get; set; } = "";
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}