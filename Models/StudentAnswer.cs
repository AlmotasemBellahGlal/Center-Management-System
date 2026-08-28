namespace Center_Management.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public int AttemptId { get; set; }
        public StudentExamAttempt? Attempt { get; set; }
        public int QuestionId { get; set; }
        public ExamQuestion? Question { get; set; }
        public string? AnswerText { get; set; } // "A"/"B"/"C"/"D" for MCQ, free text for Written
        public bool? IsCorrect { get; set; } // auto-set for MCQ
        public int? TeacherScore { get; set; } // for Written grading
    }
}
