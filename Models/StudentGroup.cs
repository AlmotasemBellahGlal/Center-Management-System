namespace Center_Management.Models
{
    public class StudentGroup
    {
        public int StudentId { get; set; }

        public Student Student { get; set; }

        public int GroupId { get; set; }

        public Group Group { get; set; }
        public DateOnly EnrollmentDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
