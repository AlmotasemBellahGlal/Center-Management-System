namespace Center_Management.Models
{
    public class Attendence
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
