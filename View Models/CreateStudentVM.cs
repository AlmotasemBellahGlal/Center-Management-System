using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class CreateStudentVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression("^(010|011|012|015)\\d{8}$",
            ErrorMessage = "يرجى إعادة كتابة الرقم بشكل صحيح")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "رقم هاتف ولي الأمر مطلوب")]
        [RegularExpression("^(010|011|012|015)\\d{8}$",
            ErrorMessage = "يرجى إعادة كتابة الرقم بشكل صحيح")]
        public string ParentPhoneNumber { get; set; }

        [Required(ErrorMessage = "يجب اختيار مجموعة دراسية واحدة")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار مجموعة دراسية")]
        public int SelectedGroupId { get; set; }

        public List<AcademicYearGroupsVM> AcademicYears { get; set; } = new();
    }
}
