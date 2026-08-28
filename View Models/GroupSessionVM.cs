namespace Center_Management.View_Models
{
    /// <summary>
    /// Combined attendance + payment session view for one group on a specific date/month.
    /// </summary>
    public class GroupSessionVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public string AcademicYearName { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public decimal MonthlyPrice { get; set; }

        // Selected session date (for attendance)
        public DateTime SessionDate { get; set; } = DateTime.Today;

        // Selected month/year (for payment)
        public int Month { get; set; } = DateTime.Now.Month;
        public int Year { get; set; } = DateTime.Now.Year;

        // Whether attendance was already recorded for this date
        public bool AttendanceAlreadySaved { get; set; }

        public List<StudentSessionItemVM> Students { get; set; } = new();
    }

    public class StudentSessionItemVM
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = "";

        // Attendance for the selected date
        public bool? IsPresent { get; set; }          // null = not recorded yet
        public int? AttendanceId { get; set; }         // existing Attendence.Id if recorded

        // Payment for the selected month/year
        public bool IsPaid { get; set; }
        public int? PaymentId { get; set; }            // existing Payment.Id if recorded
        public decimal MonthlyPrice { get; set; }
    }
}
