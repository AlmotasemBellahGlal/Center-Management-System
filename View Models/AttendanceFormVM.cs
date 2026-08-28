namespace Center_Management.View_Models
{
    public class AttendanceFormVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public string AcademicYearName { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Today;
        public List<GroupScheduleVM> Schedule { get; set; } = new();
        public List<StudentAttendanceItemVM> Students { get; set; } = new();
    }
}
