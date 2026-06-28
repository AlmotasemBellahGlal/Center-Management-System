using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class CreateStudentVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression("^(010|011|012|015)\\d{8}$",
            ErrorMessage = "يرجى إعادة كتابة الرقم بشكل صحيح")]
        public string PhoneNumber { get; set; }

        [RegularExpression("^(010|011|012|015)\\d{8}$",
            ErrorMessage = "يرجى إعادة كتابة الرقم بشكل صحيح")]
        public string ParentPhoneNumber { get; set; }

        public List<AcademicYearGroupsVM> AcademicYears { get; set; } = new();
    }
}
