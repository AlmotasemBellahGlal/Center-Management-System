namespace Center_Management.Models
{
    public class StudentExamAttempt
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public bool IsSubmitted { get; set; }
        public int? Score { get; set; } // auto-calculated for MCQ
        public ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }
}
