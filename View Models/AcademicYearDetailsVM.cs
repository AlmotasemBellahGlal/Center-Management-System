using Center_Management.Models;

namespace Center_Management.View_Models
{
    public class AcademicYearDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal MonthlyPrice { get; set; }

        public string SubjectName { get; set; }

        public List<Group> Groups { get; set; } = new();
    }
}
