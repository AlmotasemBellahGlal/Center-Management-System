namespace Center_Management.View_Models
{
    /// <summary>
    /// معلومات اشتراك الطالب في مجموعة معينة
    /// </summary>
    public class StudentGroupInfoVM
    {
        public string GroupName { get; set; } = "";
        public string AcademicYearName { get; set; } = "";
        public DateOnly EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        
        /// <summary>
        /// تسمية حالة الاشتراك
        /// </summary>
        public string StatusLabel => IsActive ? "نشط" : "غير نشط";
    }
}
