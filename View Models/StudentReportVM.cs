namespace Center_Management.View_Models
{
    /// <summary>
    /// ViewModel لتقرير الطالب الشامل يجمع جميع معلومات الطالب في صفحة واحدة
    /// </summary>
    public class StudentReportVM
    {
        // بيانات الطالب الأساسية
        public int StudentId { get; set; }
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string ParentPhoneNumber { get; set; } = "";

        // المجموعات
        public List<StudentGroupInfoVM> Groups { get; set; } = new();

        // ملخص الحضور الإجمالي
        public int TotalAttendanceSessions { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int OverallAttendanceRate { get; set; }

        // المدفوعات
        public List<PaymentRecordVM> Payments { get; set; } = new();
    }
}
