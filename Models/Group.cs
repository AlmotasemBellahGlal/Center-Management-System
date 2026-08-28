namespace Center_Management.Models
{
    
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<GroupSchedule> Schedules { get; set; }=new List<GroupSchedule>();
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
        public ICollection<StudentGroup> StudentGroups { get; set; }
       = new List<StudentGroup>();
        public ICollection<Attendence>? Attendences { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
