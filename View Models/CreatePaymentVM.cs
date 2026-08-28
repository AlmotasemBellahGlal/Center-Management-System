using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class CreatePaymentVM
    {
        public int GroupId { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Range(1, 12)]
        public int Month { get; set; } = DateTime.Now.Month;
        
        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.Now.Year;
        
        // Amount يُملأ تلقائياً من MonthlyPrice — لا يُدخله المستخدم
        public List<StudentSelectVM> ActiveStudents { get; set; } = new();
        public string GroupName { get; set; } = "";
        public decimal MonthlyPrice { get; set; }
    }
}