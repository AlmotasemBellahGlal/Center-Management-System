using System.ComponentModel.DataAnnotations;

namespace Center_Management.Models
{
    public class AcademicYear//السنه الدراسية (اولى اعدادي)
    {
        public int Id { get; set; }
        [MaxLength(60)]
        public string Name { get; set; }
        [Range(50, 500)]
        [Required]
        public decimal MonthlyPrice { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public ICollection<Group>? Groups { get; set; }
        public ICollection<Matrial>? Matrials { get; set; }

    }
}
