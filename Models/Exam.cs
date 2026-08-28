namespace Center_Management.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public ExamType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        /// <summary>
        /// مدة الامتحان بالدقائق — يبدأ العد من لحظة دخول الطالب.
        /// null = لا يوجد حد زمني للطالب (ينتهي بنهاية الامتحان).
        /// </summary>
        public int? DurationMinutes { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
        public ICollection<StudentExamAttempt> Attempts { get; set; } = new List<StudentExamAttempt>();
    }

    public enum ExamType { MCQ, Written }
}