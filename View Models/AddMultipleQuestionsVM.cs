using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class AddMultipleQuestionsVM
    {
        public int ExamId { get; set; }
        
        [Required(ErrorMessage = "عدد الأسئلة مطلوب")]
        [Range(1, 50, ErrorMessage = "عدد الأسئلة يجب أن يكون بين 1 و 50")]
        [Display(Name = "عدد الأسئلة")]
        public int QuestionCount { get; set; } = 5;
        
        public List<QuestionItemVM> Questions { get; set; } = new();
    }

    public class QuestionItemVM
    {
        public int Order { get; set; }
        
        [Required(ErrorMessage = "نص السؤال مطلوب")]
        [Display(Name = "نص السؤال")]
        public string Text { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "الخيار الأول مطلوب")]
        [Display(Name = "الخيار الأول")]
        public string OptionA { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "الخيار الثاني مطلوب")]
        [Display(Name = "الخيار الثاني")]
        public string OptionB { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "الخيار الثالث مطلوب")]
        [Display(Name = "الخيار الثالث")]
        public string OptionC { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "الخيار الرابع مطلوب")]
        [Display(Name = "الخيار الرابع")]
        public string OptionD { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "الإجابة الصحيحة مطلوبة")]
        [Display(Name = "الإجابة الصحيحة")]
        public string CorrectAnswer { get; set; } = "A";
        
        [Required(ErrorMessage = "الدرجة مطلوبة")]
        [Range(0.5, 100, ErrorMessage = "الدرجة يجب أن تكون بين 0.5 و 100")]
        [Display(Name = "الدرجة")]
        public decimal Points { get; set; } = 1;
        
        public IFormFile? QuestionImage { get; set; }
    }
}
