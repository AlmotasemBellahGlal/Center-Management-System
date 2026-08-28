namespace Center_Management.View_Models
{
    public class AttendanceReportVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public List<AttendanceRecordVM> Records { get; set; } = new();
    }
}