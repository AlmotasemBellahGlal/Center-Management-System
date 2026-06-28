using System.ComponentModel.DataAnnotations;

namespace Center_Management.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("^(010|011|012|015)\\d{8}$",ErrorMessage ="يرجى اعادة كتابة الرقم بشكل صحيح")]
        public string PhoneNumber { get; set; }
        [RegularExpression("^(010|011|012|015)\\d{8}$", ErrorMessage = "يرجى اعادة كتابة الرقم بشكل صحيح")]
        public string ParentPhoneNumber { get; set; }
        public ICollection<StudentGroup> StudentGroups { get; set; }
             = new List<StudentGroup>();
        public ICollection<Attendence>? Attendences { get; set; }
        public ICollection<Payment>? Payments { get; set; }

    }
}
