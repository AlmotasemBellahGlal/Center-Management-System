namespace Center_Management.Models
{
    public class ExamQuestion
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = "";
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
        public int Order { get; set; }
        // For MCQ only
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectAnswer { get; set; } // "A", "B", "C", or "D" for MCQ
        public int Points { get; set; } = 1;
        /// <summary>مسار الصورة المرفقة بالسؤال (اختياري)</summary>
        public string? ImagePath { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
