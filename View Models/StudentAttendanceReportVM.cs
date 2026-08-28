namespace Center_Management.View_Models
{
    public class StudentAttendanceReportVM
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = "";
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int AttendanceRate { get; set; } // نسبة مئوية صحيحة
        public List<AttendanceRecordVM> Records { get; set; } = new();
    }
}
