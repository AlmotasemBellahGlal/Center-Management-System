using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class StudentRegisterVM
    {
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression("^(010|011|012|015)\\d{8}$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح (يبدأ بـ 010، 011، 012، أو 015)")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمات المرور غير متطابقة")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; } = "";
    }
}
