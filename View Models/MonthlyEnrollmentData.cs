namespace Center_Management.View_Models
{
    /// <summary>
    /// بيانات التسجيلات الشهرية لاستخدامها في الرسم البياني بلوحة التحكم
    /// </summary>
    public class MonthlyEnrollmentData
    {
        /// <summary>
        /// السنة الميلادية
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// الشهر (من 1 إلى 12)
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// عدد التسجيلات الجديدة في هذا الشهر
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// تسمية توضيحية للعرض في الرسم البياني بصيغة "شهر/سنة"
        /// </summary>
        public string Label => $"{Month}/{Year}";
    }
}
