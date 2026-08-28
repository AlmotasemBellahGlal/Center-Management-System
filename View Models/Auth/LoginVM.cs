using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "رقم الهاتف أو البريد الإلكتروني مطلوب")]
        [Display(Name = "رقم الهاتف أو البريد الإلكتروني")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = "";

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }
    }
}
