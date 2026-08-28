namespace Center_Management.View_Models
{
    public class StudentAttendanceItemVM
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = "";
        public bool? IsPresent { get; set; } = null; // null = not yet recorded
    }
}
