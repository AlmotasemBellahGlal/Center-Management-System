namespace Center_Management.View_Models
{
    /// <summary>
    /// ViewModel للوحة التحكم Dashboard يعرض إحصائيات رئيسية للنظام
    /// </summary>
    public class DashboardVM
    {
        /// <summary>
        /// عدد الطلاب النشطين (الذين لديهم على الأقل سجل StudentGroup.IsActive = true)
        /// </summary>
        public int ActiveStudentsCount { get; set; }

        /// <summary>
        /// عدد الطلاب النشطين غير المدفوعين للشهر الحالي
        /// </summary>
        public int UnpaidStudentsCurrentMonth { get; set; }

        /// <summary>
        /// نسبة التغيير في التسجيلات بين الشهر الحالي والشهر السابق
        /// القيم الممكنة: "جديد" أو "0%" أو "+X%" أو "-X%"
        /// </summary>
        public string GrowthRate { get; set; } = "0%";

        /// <summary>
        /// قائمة بيانات التسجيلات الشهرية لآخر 12 شهراً
        /// </summary>
        public List<MonthlyEnrollmentData> MonthlyEnrollments { get; set; } = new();

        /// <summary>
        /// إيراد الشهر الحالي (مجموع المدفوعات المؤكدة)
        /// </summary>
        public decimal MonthlyRevenue { get; set; }
    }
}
